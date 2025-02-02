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
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Web.Core.Files;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Tariffs : MainPage
    {
        protected override bool MayNotPaid
        {
            get { return true; }
            set { }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (CoreContext.Configuration.Personal)
                Context.Response.Redirect(FilesLinkUtility.FilesBaseAbsolutePath);

            if (!TenantExtra.EnableTarrifSettings ||
                (TariffSettings.HidePricingPage &&
                 !CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID)))
                Response.Redirect("~/", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.DisabledSidePanel = true;

            if (TenantStatisticsProvider.IsNotPaid())
            {
                Master.TopStudioPanel.DisableProductNavigation = true;
                Master.TopStudioPanel.DisableSettings = true;
                Master.TopStudioPanel.DisableSearch = true;
            }

            Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Tariffs);

            if (Request.DesktopApp())
            {
                Master.DisabledTopStudioPanel = true;
                pageContainer.Controls.Add(LoadControl(TariffDesktop.Location));
            }
            else if (CoreContext.Configuration.Standalone)
            {
                pageContainer.Controls.Add(LoadControl(TariffStandalone.Location));
            }
            else
            {
                pageContainer.Controls.Add(LoadControl(TariffUsage.Location));

                var payments = CoreContext.PaymentManager.GetTariffPayments(TenantProvider.CurrentTenantID).ToList();
                if (payments.Any()
                    && !TenantExtra.GetTenantQuota().Trial
                    && CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID))
                {
                    var tariffHistory = (TariffHistory)LoadControl(TariffHistory.Location);
                    tariffHistory.Payments = payments;
                    pageContainer.Controls.Add(tariffHistory);
                }
            }
        }
    }
}