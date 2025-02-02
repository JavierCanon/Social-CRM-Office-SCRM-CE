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


using System;
using System.Web;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Core.Common.Notify.Push;

namespace ASC.Web.Core.Mobile
{
    public class MobileAppInstallRegistrator : IMobileAppInstallRegistrator
    {
        public void RegisterInstall(string userEmail, MobileAppType appType)
        {
            using (var db = GetDbManager())
            {
                db.ExecuteNonQuery(
                    "INSERT INTO `mobile_app_install` (`user_email`, `app_type`, `registered_on`, `last_sign`)" +
                    " VALUES (@user_email, @app_type, @registered_on, @last_sign)" +
                    " ON DUPLICATE KEY UPDATE `last_sign`=@last_sign",
                    new
                        {
                            user_email = userEmail,
                            app_type = (int) appType,
                            registered_on = DateTime.UtcNow,
                            last_sign = DateTime.UtcNow
                        });
            }
        }

        public bool IsInstallRegistered(string userEmail, MobileAppType? appType)
        {
            var query = new SqlQuery("mobile_app_install")
                .SelectCount()
                .Where("user_email", userEmail);

            if (appType.HasValue)
                query.Where("app_type", (int) appType.Value);


            using (var db = GetDbManager())
            {
                return db.ExecuteScalar<int>(query) > 0;
            }
        }

        private IDbManager GetDbManager()
        {
            return DbManager.FromHttpContext("default");
        }
    }
}