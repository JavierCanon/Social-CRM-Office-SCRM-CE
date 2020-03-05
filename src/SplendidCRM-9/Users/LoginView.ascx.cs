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
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for LoginView.
	/// </summary>
	public class LoginView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;

		protected Label           lblError                        ;
		protected Label           lblInstructions                 ;
		protected TextBox         txtUSER_NAME                    ;
		protected TextBox         txtPASSWORD                     ;
		protected DropDownList    lstTHEME                        ;
		protected DropDownList    lstLANGUAGE                     ;
		protected Table           tblUser                         ;
		protected TableRow        trError                         ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Login" )
			{
				if ( Page.IsValid )
				{
					bool bValidUser = false;
					try
					{
						if ( Security.IsWindowsAuthentication() )
						{
							SplendidInit.ChangeTheme(lstTHEME.SelectedValue, lstLANGUAGE.SelectedValue);
							bValidUser = true;
						}
						else
						{
							bValidUser = SplendidInit.LoginUser(txtUSER_NAME.Text, txtPASSWORD.Text, lstTHEME.SelectedValue, lstLANGUAGE.SelectedValue);
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						trError.Visible = true;
						lblError.Text = ex.Message;
						return;
					}
					// 09/12/2006 Paul.  Move redirect outside try/catch to avoid catching "Thread was being aborted" exception. 
					if ( bValidUser )
					{
						string sDefaultModule = Sql.ToString(Application["CONFIG.default_module"]);
						// 10/06/2007 Paul.  Prompt the user for the timezone. 
						if ( Sql.IsEmptyString(Session["USER_SETTINGS/TIMEZONE/ORIGINAL"]) )
							Response.Redirect("~/Users/SetTimezone.aspx");
						else if ( sDefaultModule.StartsWith("~") )
							Response.Redirect(sDefaultModule);
						else if ( !Sql.IsEmptyString(sDefaultModule) )
							Response.Redirect("~/" + sDefaultModule + "/");
						else
							Response.Redirect("~/Home/");
						return;
					}
					else
					{
						trError.Visible = true;
						lblError.Text = L10n.Term("Users.ERR_INVALID_PASSWORD");
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				if ( !IsPostBack )
				{
					lstLANGUAGE.DataSource = SplendidCache.Languages();
					lstLANGUAGE.DataBind();
					lstTHEME.DataSource = SplendidCache.Themes();
					lstTHEME.DataBind();
					
					string sDefaultUserName = Sql.ToString(Application["CONFIG.default_user_name"]);
					string sDefaultPassword = Sql.ToString(Application["CONFIG.default_password" ]);
					string sDefaultTheme    = Sql.ToString(Application["CONFIG.default_theme"    ]);
					string sDefaultLanguage = Sql.ToString(Application["CONFIG.default_language" ]);
					txtUSER_NAME.Text = sDefaultUserName;
					txtPASSWORD.Text  = sDefaultPassword;
					try
					{
						sDefaultTheme = Sql.IsEmptyString(sDefaultTheme) ? "Sugar" : sDefaultTheme;
						lstTHEME.SelectedValue = sDefaultTheme;
					}
					catch(Exception ex)
					{
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
					}
					try
					{
						sDefaultLanguage = Sql.IsEmptyString(sDefaultLanguage) ? "en-US" : sDefaultLanguage;
						lstLANGUAGE.SelectedValue = L10N.NormalizeCulture(sDefaultLanguage);
					}
					catch(Exception ex)
					{
						SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						lblError.Text += ex.Message;
					}
					// 11/19/2005 Paul.  Don't show the Login & Password if Windows Authentication. 
					tblUser.Visible = !Security.IsWindowsAuthentication();
					lblInstructions.Visible = tblUser.Visible;
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
