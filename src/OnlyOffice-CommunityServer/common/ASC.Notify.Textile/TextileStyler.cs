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
using System.Text;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using ASC.Common.Notify.Patterns;
using ASC.Core;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Web.Core.WhiteLabel;
using Textile;
using Textile.Blocks;
using ASC.Notify.Textile.Resources;

namespace ASC.Notify.Textile
{
    public class TextileStyler : IPatternStyler
    {
        private static readonly Regex VelocityArguments = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        static TextileStyler()
        {
            const string file = "ASC.Notify.Textile.Resources.style.css";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file))
            using (var reader = new StreamReader(stream))
            {
                BlockAttributesParser.Styler = new StyleReader(reader.ReadToEnd().Replace("\n", "").Replace("\r", ""));
            }
        }

        public void ApplyFormating(NoticeMessage message)
        {
            var output = new StringBuilderTextileFormatter();
            var formatter = new TextileFormatter(output);

            if (!string.IsNullOrEmpty(message.Subject))
            {
                message.Subject = VelocityArguments.Replace(message.Subject, m => m.Result("${arg}"));
            }

            if (string.IsNullOrEmpty(message.Body)) return;

            formatter.Format(message.Body);

            var template = GetTemplate(message);
            var analytics = GetAnalytics(message);
            var imagePath = GetImagePath(message);
            var logoImg = GetLogoImg(message, imagePath);
            var logoText = GetLogoText(message);
            var mailSettings = GetMailSettings(message);
            var unsubscribeText = GetUnsubscribeText(message, mailSettings);

            string footerContent;
            string footerSocialContent;

            InitFooter(message, mailSettings, out footerContent, out footerSocialContent);

            message.Body = template.Replace("%ANALYTICS%", analytics)
                                   .Replace("%CONTENT%", output.GetFormattedText())
                                   .Replace("%LOGO%", logoImg)
                                   .Replace("%LOGOTEXT%", logoText)
                                   .Replace("%SITEURL%", mailSettings == null ? MailWhiteLabelSettings.DefaultMailSiteUrl : mailSettings.SiteUrl)
                                   .Replace("%FOOTER%", footerContent)
                                   .Replace("%FOOTERSOCIAL%", footerSocialContent)
                                   .Replace("%TEXTFOOTER%", unsubscribeText)
                                   .Replace("%IMAGEPATH%", imagePath);
        }

        private static string GetTemplate(NoticeMessage message)
        {
            var template = NotifyTemplateResource.HtmlMaster;

            var templateTag = message.GetArgument("MasterTemplate");
            if (templateTag != null)
            {
                var templateTagValue = templateTag.Value as string;
                if (!string.IsNullOrEmpty(templateTagValue))
                {
                    var templateValue = NotifyTemplateResource.ResourceManager.GetString(templateTagValue);
                    if (!string.IsNullOrEmpty(templateValue))
                        template = templateValue;
                }
            }

            return template;
        }

        private static string GetAnalytics(NoticeMessage message)
        {
            var analyticsTag = message.GetArgument("Analytics");
            return analyticsTag == null ? string.Empty : (string)analyticsTag.Value;
        }

        private static string GetImagePath(NoticeMessage message)
        {
            var imagePathTag = message.GetArgument("ImagePath");
            return imagePathTag == null ? string.Empty : (string)imagePathTag.Value;
        }

        private static string GetLogoImg(NoticeMessage message, string imagePath)
        {
            string logoImg;

            if (CoreContext.Configuration.Personal && !CoreContext.Configuration.CustomMode)
            {
                logoImg = imagePath + "/mail_logo.png";
            }
            else
            {
                logoImg = ConfigurationManager.AppSettings["web.logo.mail"];
                if (String.IsNullOrEmpty(logoImg))
                {
                    var logo = message.GetArgument("LetterLogo");
                    if (logo != null && (string)logo.Value != "")
                    {
                        logoImg = (string)logo.Value;
                    }
                    else
                    {
                        logoImg = imagePath + "/mail_logo.png";
                    }
                }
            }

            return logoImg;
        }

        private static string GetLogoText(NoticeMessage message)
        {
            var logoText = ConfigurationManager.AppSettings["web.logotext.mail"];

            if (String.IsNullOrEmpty(logoText))
            {
                var llt = message.GetArgument("LetterLogoText");
                if (llt != null && (string)llt.Value != "")
                {
                    logoText = (string)llt.Value;
                }
                else
                {
                    logoText = TenantWhiteLabelSettings.DefaultLogoText;
                }
            }

            return logoText;
        }

        private static MailWhiteLabelSettings GetMailSettings(NoticeMessage message)
        {
            var mailWhiteLabelTag = message.GetArgument("MailWhiteLabelSettings");
            return mailWhiteLabelTag == null ? null : mailWhiteLabelTag.Value as MailWhiteLabelSettings;
        }

        private static void InitFooter(NoticeMessage message, MailWhiteLabelSettings settings, out string footerContent, out string footerSocialContent)
        {
            footerContent = string.Empty;
            footerSocialContent = string.Empty;

            var footer = message.GetArgument("Footer");

            if (footer == null) return;

            var footerValue = (string) footer.Value;

            if (string.IsNullOrEmpty(footerValue)) return;

            switch (footerValue)
            {
                case "common":
                    InitCommonFooter(settings, out footerContent, out footerSocialContent);
                    break;
                case "social":
                    InitSocialFooter(settings, out footerSocialContent);
                    break;
                case "personal":
                    footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
                    break;
                case "personalCustomMode":
                    break;
                case "opensource":
                    footerContent = NotifyTemplateResource.FooterOpensourceV10;
                    footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
                    break;
            }
        }

        private static void InitCommonFooter(MailWhiteLabelSettings settings, out string footerContent, out string footerSocialContent)
        {
            footerContent = string.Empty;
            footerSocialContent = string.Empty;

            if (settings == null)
            {
                footerContent =
                    NotifyTemplateResource.FooterCommonV10
                                          .Replace("%SUPPORTURL%", MailWhiteLabelSettings.DefaultMailSupportUrl)
                                          .Replace("%SALESEMAIL%", MailWhiteLabelSettings.DefaultMailSalesEmail)
                                          .Replace("%DEMOURL%", MailWhiteLabelSettings.DefaultMailDemotUrl);
                footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;

            }
            else if (settings.FooterEnabled)
            {
                footerContent =
                    NotifyTemplateResource.FooterCommonV10
                    .Replace("%SUPPORTURL%", String.IsNullOrEmpty(settings.SupportUrl) ? "mailto:" + settings.SalesEmail : settings.SupportUrl)
                    .Replace("%SALESEMAIL%", settings.SalesEmail)
                    .Replace("%DEMOURL%", String.IsNullOrEmpty(settings.DemotUrl) ? "mailto:" + settings.SalesEmail : settings.DemotUrl);
                footerSocialContent = settings.FooterSocialEnabled ? NotifyTemplateResource.SocialNetworksFooterV10 : string.Empty;
            }
        }

        private static void InitSocialFooter(MailWhiteLabelSettings settings, out string footerSocialContent)
        {
            footerSocialContent = string.Empty;

            if (settings == null || (settings.FooterEnabled && settings.FooterSocialEnabled))
                footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
        }

        private static string GetUnsubscribeText(NoticeMessage message, MailWhiteLabelSettings settings)
        {
            var withoutUnsubscribe = message.GetArgument("WithoutUnsubscribe");

            if (withoutUnsubscribe != null && (bool) withoutUnsubscribe.Value)
                return string.Empty;

            var rootPathArgument = message.GetArgument("__VirtualRootPath");
            var rootPath = rootPathArgument == null ? string.Empty : (string) rootPathArgument.Value;

            if (string.IsNullOrEmpty(rootPath))
                return string.Empty;

            var unsubscribeLink = CoreContext.Configuration.CustomMode && CoreContext.Configuration.Personal
                                      ? GetSiteUnsubscribeLink(message, settings)
                                      : GetPortalUnsubscribeLink(message, settings);

            if (string.IsNullOrEmpty(unsubscribeLink))
                return string.Empty;

            return string.Format(NotifyTemplateResource.TextForFooterWithUnsubscribeLink, rootPath, unsubscribeLink);
        }

        private static string GetPortalUnsubscribeLink(NoticeMessage message, MailWhiteLabelSettings settings)
        {
            var unsubscribeLinkArgument = message.GetArgument("ProfileUrl");

            if (unsubscribeLinkArgument != null)
            {
                var unsubscribeLink = (string) unsubscribeLinkArgument.Value;

                if (!string.IsNullOrEmpty(unsubscribeLink))
                    return unsubscribeLink;
            }

            return GetSiteUnsubscribeLink(message, settings);
        }

        private static string GetSiteUnsubscribeLink(NoticeMessage message, MailWhiteLabelSettings settings)
        {
            var mail = message.Recipient.Addresses.FirstOrDefault(r => r.Contains("@"));

            if (string.IsNullOrEmpty(mail))
                return string.Empty;

            var format = CoreContext.Configuration.CustomMode
                             ? "{0}/unsubscribe/{1}"
                             : "{0}/Unsubscribe.aspx?id={1}";

            var site = settings == null
                           ? MailWhiteLabelSettings.DefaultMailSiteUrl
                           : settings.SiteUrl;

            return string.Format(format,
                                 site,
                                 HttpServerUtility.UrlTokenEncode(
                                     Security.Cryptography.InstanceCrypto.Encrypt(
                                         Encoding.UTF8.GetBytes(mail.ToLowerInvariant()))));
        }
    }
}