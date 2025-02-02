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


#region Import

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Api;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;

#endregion

namespace ASC.Web.Community
{
    public partial class Photos : MainPage
    {
        protected ApiServer apiServer = new ApiServer();

        protected void Page_Load(object sender, EventArgs e)
        {
            PublishRecentUploadPhotos("recentPhotos");
            PublishAllAlbumInfo("");
            PublishAlbumsPhotos("",Enumerable.Empty<int>());
            
           // Page.JsonPublisher(contactsForFirstRequest, "contactsForFirstRequest");

        }

        protected String PublishRecentUploadPhotos(String publishByName)
        {
            var apiResponse = apiServer.GetApiResponse(
                String.Format("{0}crm/contact/filter.json?{1}", SetupInfo.WebApiBaseUrl, ""), "GET");

            JsonPublisher(apiResponse, publishByName);

            return apiResponse;
        }

        protected String PublishAllAlbumInfo(String publishByName)
        {
            var apiResponse = apiServer.GetApiResponse(String.Format("{0}files/@photos", SetupInfo.WebApiBaseUrl), "GET");

            JsonPublisher(apiResponse, publishByName);

            return apiResponse;
        }

        protected String PublishAlbumsPhotos(String publishByName, IEnumerable<int> albumIDs)
        {

            throw new NotImplementedException();
        }

        protected void JsonPublisher<T>(T data, String jsonClassName) where T : class
        {

            String json;

            using (var stream = new MemoryStream())
            {

                var serializer = new DataContractJsonSerializer(typeof(T));

                serializer.WriteObject(stream, data);

                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                       Guid.NewGuid().ToString(),
                                                        String.Format(" var {1} = {0};", json, jsonClassName),
                                                        true);
        }
    }
}
