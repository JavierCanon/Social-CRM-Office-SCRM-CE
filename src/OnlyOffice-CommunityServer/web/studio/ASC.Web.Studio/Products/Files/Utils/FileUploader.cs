/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web;
using File = ASC.Files.Core.File;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.Utils
{
    public static class FileUploader
    {
        public static File Exec(string folderId, string title, long contentLength, Stream data)
        {
            return Exec(folderId, title, contentLength, data, !FilesSettings.UpdateIfExist);
        }

        public static File Exec(string folderId, string title, long contentLength, Stream data, bool createNewIfExist, bool deleteConvertStatus = true)
        {
            if (contentLength <= 0)
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);

            var file = VerifyFileUpload(folderId, title, contentLength, !createNewIfExist);

            using (var dao = Global.DaoFactory.GetFileDao())
            {
                file = dao.SaveFile(file, data);
            }

            FileMarker.MarkAsNew(file);

            if (FileConverter.EnableAsUploaded && FileConverter.MustConvert(file))
                FileConverter.ExecAsync(file, deleteConvertStatus);

            return file;
        }

        public static File VerifyFileUpload(string folderId, string fileName, bool updateIfExists, string relativePath = null)
        {
            fileName = Global.ReplaceInvalidCharsAndTruncate(fileName);

            if (Global.EnableUploadFilter && !FileUtility.ExtsUploadable.Contains(FileUtility.GetFileExtension(fileName)))
                throw new NotSupportedException(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            folderId = GetFolderId(folderId, string.IsNullOrEmpty(relativePath) ? null : relativePath.Split('/').ToList());

            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                var file = fileDao.GetFile(folderId, fileName);

                if (updateIfExists && CanEdit(file))
                {
                    file.Title = fileName;
                    file.ConvertedType = null;
                    file.Comment = FilesCommonResource.CommentUpload;
                    file.Version++;
                    file.VersionGroup++;
                    file.Encrypted = false;

                    return file;
                }
            }

            return new File {FolderID = folderId, Title = fileName};
        }

        public static File VerifyFileUpload(string folderId, string fileName, long fileSize, bool updateIfExists)
        {
            if (fileSize <= 0)
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);

            var maxUploadSize = GetMaxFileSize(folderId);

            if (fileSize > maxUploadSize)
                throw FileSizeComment.GetFileSizeException(maxUploadSize);

            var file = VerifyFileUpload(folderId, fileName, updateIfExists);
            file.ContentLength = fileSize;
            return file;
        }

        private static bool CanEdit(File file)
        {
            return file != null
                   && Global.GetFilesSecurity().CanEdit(file)
                   && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor()
                   && !EntryManager.FileLockedForMe(file.ID)
                   && !FileTracker.IsEditing(file.ID)
                   && file.RootFolderType != FolderType.TRASH
                   && !file.Encrypted;
        }

        private static string GetFolderId(object folderId, IList<string> relativePath)
        {
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                var folder = folderDao.GetFolder(folderId);

                if (folder == null)
                    throw new DirectoryNotFoundException(FilesCommonResource.ErrorMassage_FolderNotFound);

                if (!Global.GetFilesSecurity().CanCreate(folder))
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

                if (relativePath != null && relativePath.Any())
                {
                    var subFolderTitle = Global.ReplaceInvalidCharsAndTruncate(relativePath.FirstOrDefault());

                    if (!string.IsNullOrEmpty(subFolderTitle))
                    {
                        folder = folderDao.GetFolder(subFolderTitle, folder.ID);

                        if (folder == null)
                        {
                            folderId = folderDao.SaveFolder(new Folder {Title = subFolderTitle, ParentFolderID = folderId});

                            folder = folderDao.GetFolder(folderId);
                            FilesMessageService.Send(folder, HttpContext.Current.Request, MessageAction.FolderCreated, folder.Title);
                        }

                        folderId = folder.ID;

                        relativePath.RemoveAt(0);
                        folderId = GetFolderId(folderId, relativePath);
                    }
                }
            }

            return folderId.ToString();
        }

        #region chunked upload

        public static File VerifyChunkedUpload(string folderId, string fileName, long fileSize, bool updateIfExists, string relativePath = null)
        {
            var maxUploadSize = GetMaxFileSize(folderId, true);

            if (fileSize > maxUploadSize)
                throw FileSizeComment.GetFileSizeException(maxUploadSize);

            var file = VerifyFileUpload(folderId, fileName, updateIfExists, relativePath);
            file.ContentLength = fileSize;

            return file;
        }

        public static ChunkedUploadSession InitiateUpload(string folderId, string fileId, string fileName, long contentLength, bool encrypted)
        {
            if (string.IsNullOrEmpty(folderId))
                folderId = null;

            if (string.IsNullOrEmpty(fileId))
                fileId = null;

            var file = new File
                {
                    ID = fileId,
                    FolderID = folderId,
                    Title = fileName,
                    ContentLength = contentLength
                };

            using (var dao = Global.DaoFactory.GetFileDao())
            {
                var uploadSession = dao.CreateUploadSession(file, contentLength);

                uploadSession.Expired = uploadSession.Created + ChunkedUploadSessionHolder.SlidingExpiration;
                uploadSession.Location = FilesLinkUtility.GetUploadChunkLocationUrl(uploadSession.Id);
                uploadSession.TenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                uploadSession.UserId = SecurityContext.CurrentAccount.ID;
                uploadSession.FolderId = folderId;
                uploadSession.CultureName = Thread.CurrentThread.CurrentUICulture.Name;
                uploadSession.Encrypted = encrypted;

                ChunkedUploadSessionHolder.StoreSession(uploadSession);

                return uploadSession;
            }
        }

        public static ChunkedUploadSession UploadChunk(string uploadId, Stream stream, long chunkLength)
        {
            var uploadSession = ChunkedUploadSessionHolder.GetSession(uploadId);
            uploadSession.Expired = DateTime.UtcNow + ChunkedUploadSessionHolder.SlidingExpiration;

            if (chunkLength <= 0)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);
            }

            if (chunkLength > SetupInfo.ChunkUploadSize)
            {
                throw FileSizeComment.GetFileSizeException(SetupInfo.MaxUploadSize);
            }

            var maxUploadSize = GetMaxFileSize(uploadSession.FolderId, uploadSession.BytesTotal > 0);

            if (uploadSession.BytesUploaded + chunkLength > maxUploadSize)
            {
                AbortUpload(uploadSession);
                throw FileSizeComment.GetFileSizeException(maxUploadSize);
            }

            using (var dao = Global.DaoFactory.GetFileDao())
            {
                dao.UploadChunk(uploadSession, stream, chunkLength);
            }

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                FileMarker.MarkAsNew(uploadSession.File);
                ChunkedUploadSessionHolder.RemoveSession(uploadSession);
            }
            else
            {
                ChunkedUploadSessionHolder.StoreSession(uploadSession);
            }

            return uploadSession;
        }

        public static void AbortUpload(string uploadId)
        {
            AbortUpload(ChunkedUploadSessionHolder.GetSession(uploadId));
        }

        private static void AbortUpload(ChunkedUploadSession uploadSession)
        {
            using (var dao = Global.DaoFactory.GetFileDao())
            {
                dao.AbortUploadSession(uploadSession);
            }

            ChunkedUploadSessionHolder.RemoveSession(uploadSession);
        }

        private static long GetMaxFileSize(object folderId, bool chunkedUpload = false)
        {
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                return folderDao.GetMaxUploadSize(folderId, chunkedUpload);
            }
        }

        #endregion
    }
}