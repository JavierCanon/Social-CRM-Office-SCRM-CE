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


using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ASC.CdnCheck
{
    public class CorsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            bool isCorsRequest = request.Headers.Contains("Origin");
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew<HttpResponseMessage>(() =>
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add("AccessControlAllowOrigin", request.Headers.GetValues("Origin").First());

                        string accessControlRequestMethod =
                            request.Headers.GetValues("AccessControlRequestMethod").FirstOrDefault();
                        if (accessControlRequestMethod != null)
                        {
                            response.Headers.Add("AccessControlAllowMethods", accessControlRequestMethod);
                        }

                        string requestedHeaders = string.Join(", ",
                            request.Headers.GetValues("AccessControlRequestHeaders"));
                        if (!string.IsNullOrEmpty(requestedHeaders))
                        {
                            response.Headers.Add("AccessControlAllowHeaders", requestedHeaders);
                        }

                        return response;
                    }, cancellationToken);
                }
                return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(t =>
                {
                    HttpResponseMessage resp = t.Result;
                    resp.Headers.Add("AccessControlAllowOrigin", request.Headers.GetValues("Origin").First());
                    return resp;
                });
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}