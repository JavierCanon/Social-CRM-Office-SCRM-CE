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
using System.Diagnostics;

namespace SplendidCRM.Themes.Sugar
{
	public partial class DefaultView : System.Web.UI.MasterPage
	{
		protected L10N         L10n;

		protected HtmlGenericControl htmlRoot;
		protected SplendidCRM._controls.Shortcuts     ctlShortcuts    ;
		protected SplendidCRM._controls.LastViewed    ctlLastViewed   ;
		protected ContentPlaceHolder cntUnifiedSearch;
		protected System.Web.UI.WebControls.Image imgCompanyLogo  ;
		
		protected PlaceHolder   phFooterMenu     ;
		protected DropDownList  lstTHEME         ;
		protected DropDownList  lstLANGUAGE      ;
		protected HtmlTableRow  trFooterMenu     ;
		protected HtmlTable     tblTheme         ;
		protected HtmlTableCell tdShortcuts      ;
		protected bool          bShowLeftCol = true;
		protected Image         imgShowHandle    ;
		protected Image         imgHideHandle    ;
		protected bool          bDebug       = false;

		protected void lstTHEME_Changed(Object sender, EventArgs e)
		{
			SplendidInit.ChangeTheme(lstTHEME.SelectedValue, lstLANGUAGE.SelectedValue);
			Response.Redirect(Request.RawUrl);
		}

		protected void lstLANGUAGE_Changed(Object sender, EventArgs e)
		{
			SplendidInit.ChangeTheme(lstTHEME.SelectedValue, lstLANGUAGE.SelectedValue);
			Response.Redirect(Request.RawUrl);
		}

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

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Admin.Reload" )
			{
				if( Security.IS_ADMIN )
				{
					SplendidInit.InitApp();
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
						SplendidInit.LoadUserPreferences(Security.USER_ID, Sql.ToString(Session["USER_SETTINGS/THEME"]), Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
					// 06/30/2007 Paul.  Perform a redirect so that the entire page will reload and rebind. 
					Response.Redirect(Request.RawUrl);
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
#if DEBUG
			bDebug = true;
#endif
			if ( Request.Cookies["showLeftCol"] != null )
			{
				bShowLeftCol = Sql.ToBoolean(Request.Cookies["showLeftCol"].Value);
			}
			else
			{
				HttpCookie cShowLeftCol = new HttpCookie("showLeftCol", bShowLeftCol ? "true" : "false");
				cShowLeftCol.Expires = DateTime.Now.AddDays(30);
				cShowLeftCol.Path    = "/";
				Response.Cookies.Add(cShowLeftCol);
			}
			imgHideHandle.Style.Remove("display");
			imgShowHandle.Style.Remove("display");
			tdShortcuts  .Style.Remove("display");
			imgHideHandle.Style.Add("display",  bShowLeftCol ? "inline" : "none");
			imgShowHandle.Style.Add("display", !bShowLeftCol ? "inline" : "none");
			tdShortcuts  .Style.Add("display",  bShowLeftCol ? "inline" : "none");

			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				// 04/28/2006 Paul.  If the user has not authenticated, then this must be during login.  Disable the search. 
				// 11/17/2007 Paul.  New function to determine if user is authenticated. 
				if ( !Security.IsAuthenticated() )
				{
					cntUnifiedSearch.Visible = false;
				}
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
						imgCompanyLogo.ImageUrl = Sql.ToString(Application["imageURL"]) + "SplendidCRM_Logo.gif";
						imgCompanyLogo.Width  = 207;
						imgCompanyLogo.Height =  60;
						imgCompanyLogo.Attributes.Add("style", "margin-left: 10px");
						imgCompanyLogo.AlternateText = L10n.Term(".COMPANY_LOGO");
					}
				}
			}

			string sSeparator = "  ";
			DataTable dt = SplendidCache.TabMenu();
			// 04/28/2006 Paul.  Hide the footer menu if there is no menu to display. 
			if ( dt.Rows.Count == 0 )
			{
				trFooterMenu.Visible = false;
				tblTheme    .Visible = false;
			}
			int nRow = 0;
			int nDisplayedTabs = 0;
			int nMaxTabs = Sql.ToInteger(Session["max_tabs"]);
			// 09/24/2007 Paul.  Max tabs is a config variable and needs the CONFIG in front of the name. 
			if ( nMaxTabs == 0 )
				nMaxTabs = Sql.ToInteger(Application["CONFIG.default_max_tabs"]);
			if ( nMaxTabs == 0 )
				nMaxTabs = 12;
			for ( ; nRow < dt.Rows.Count; nRow++ )
			{
				DataRow row = dt.Rows[nRow];
				Literal litSeparator = new Literal();
				litSeparator.Text = sSeparator;
				phFooterMenu.Controls.Add(litSeparator);
				
				HyperLink lnk = new HyperLink();
				// 05/31/2007 Paul.  Don't specify an ID for the control.  
				// A customer reported an error with a duplicate entry.
				//lnk.ID          = "lnkFooter" + Sql.ToString(row["DISPLAY_NAME"]) ;
				lnk.NavigateUrl = Sql.ToString(row["RELATIVE_PATH"]);
				lnk.Text        = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
				lnk.CssClass    = "footerLink";
				phFooterMenu.Controls.Add(lnk);
				
				nDisplayedTabs++;
				if ( nDisplayedTabs % nMaxTabs == 0 )
					sSeparator = "\r\n<br />\r\n";
				else
					sSeparator = "\r\n| ";
			}
			// 04/28/2006 Paul.  No need to populate the lists if they are not going to be displayed. 
			if ( !IsPostBack && dt.Rows.Count > 0 )
			{
				lstLANGUAGE.DataSource = SplendidCache.Languages();
				lstLANGUAGE.DataBind();

				try
				{
					lstTHEME.DataSource = SplendidCache.Themes();
					lstTHEME.DataBind();
					lstTHEME.SelectedValue = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/THEME"]);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}

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
				try
				{
					lstLANGUAGE.SelectedValue = L10n.NAME;
				}
				catch
				{
				}
			}
		}
	}
}
