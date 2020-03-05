/**********************************************************************************************************************
 * The contents of this file are subject to the SugarCRM Public License Version 1.1.3 ("License"); You may not use this
 * file except in compliance with the License. You may obtain a copy of the License at http://www.sugarcrm.com/SPL
 * Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 * express or implied.  See the License for the specific language governing rights and limitations under the License.
 *
 * All copies of the Covered Code must include on each user interface screen:
 *    (i) the "Powered by SugarCRM" logo and
 *    (ii) the SugarCRM copyright notice
 *    (iii) the SplendidCRM copyright notice
 * in the same form as they appear in the distribution.  See full license for requirements.
 *
 * The Original Code is: SplendidCRM Open Source
 * The Initial Developer of the Original Code is SplendidCRM Software, Inc.
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM.Themes.Mobile
{
	public partial class DefaultView : System.Web.UI.MasterPage
	{
		protected L10N         L10n;

		protected HtmlGenericControl htmlRoot;
		protected System.Web.UI.WebControls.Image imgCompanyLogo  ;
		protected bool          bDebug       = false;

		public L10N GetL10n()
		{
			// 08/30/2005 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( L10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				L10n = Context.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					string sCULTURE  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
					L10n = new L10N(sCULTURE);
				}
			}
			return L10n;
		}

		protected override void OnInit(EventArgs e)
		{
			GetL10n();
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
#if DEBUG
			bDebug = true;
#endif
			if ( !IsPostBack )
			{
				if ( imgCompanyLogo != null )
				{
					// 04/16/2006 Paul.  Company logo can be customized. 
					if ( !Sql.IsEmptyString(Application["CONFIG.header_logo_image"]) )
					{
						imgCompanyLogo.ImageUrl = Sql.ToString(Application["imageURL"]) + Sql.ToString(Application["CONFIG.header_logo_image"]);
						if ( Sql.ToInteger(Application["CONFIG.header_logo_width"]) > 0 )
							imgCompanyLogo.Width    = Sql.ToInteger(Application["CONFIG.header_logo_width" ]);
						if ( Sql.ToInteger(Application["CONFIG.header_logo_height"]) > 0 )
							imgCompanyLogo.Height   = Sql.ToInteger(Application["CONFIG.header_logo_height"]);
						if ( !Sql.IsEmptyString(Application["CONFIG.header_logo_style"]) )
							imgCompanyLogo.Attributes.Add("style", Sql.ToString(Application["CONFIG.header_logo_style"]));
						imgCompanyLogo.AlternateText = L10n.Term(".COMPANY_LOGO");
					}
					else
					{
						imgCompanyLogo.ImageUrl      = Sql.ToString(Application["imageURL"]) + "SplendidCRM_Logo.gif";
						imgCompanyLogo.Width         = 207;
						imgCompanyLogo.Height        =  60;
						imgCompanyLogo.AlternateText = L10n.Term(".COMPANY_LOGO");
					}
				}
			}

			if ( !IsPostBack )
			{
				try
				{
					// http://www.i18nguy.com/temp/rtl.html
					if ( htmlRoot != null )
					{
						if ( L10n.IsLanguageRTL() )
						{
							htmlRoot.Attributes.Add("dir", "rtl");
						}
					}
				}
				catch
				{
				}
			}
		}
	}
}
