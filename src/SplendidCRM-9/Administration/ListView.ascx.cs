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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected Label lblError;

		// 09/11/2007 Paul.  Provide quick access to team management flags. 
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Teams.Enable"   )
				{
					SqlProcs.spCONFIG_Update("system", "enable_team_management", "true");
					Application["CONFIG.enable_team_management"] = true;
				}
				else if ( e.CommandName == "Teams.Disable"  )
				{
					SqlProcs.spCONFIG_Update("system", "enable_team_management", "false");
					Application["CONFIG.enable_team_management"] = false;
				}
				else if ( e.CommandName == "Teams.Require"  )
				{
					SqlProcs.spCONFIG_Update("system", "require_team_management", "true");
					Application["CONFIG.require_team_management"] = true;
				}
				else if ( e.CommandName == "Teams.Optional" )
				{
					SqlProcs.spCONFIG_Update("system", "require_team_management", "false");
					Application["CONFIG.require_team_management"] = false;
				}
				// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
				else if ( e.CommandName == "UserAssignement.Require"  )
				{
					SqlProcs.spCONFIG_Update("system", "require_user_assignment", "true");
					Application["CONFIG.require_user_assignment"] = true;
				}
				else if ( e.CommandName == "UserAssignement.Optional" )
				{
					SqlProcs.spCONFIG_Update("system", "require_user_assignment", "false");
					Application["CONFIG.require_user_assignment"] = false;
				}

				else if ( e.CommandName == "System.RebuildAudit" )
				{
					// 12/31/2007 Paul.  In case there is a problem, we need a way to rebuild the audit tables and triggers. 
					SqlProcs.spSqlBuildAllAuditTables();
				}
				else if ( e.CommandName == "System.RecompileViews" )
				{
					// 12/31/2007 Paul.  Use a special version of spSqlRefreshAllViews that does not timeout. 
					Utils.RefreshAllViews();
				}
				else if ( e.CommandName == "System.Reload" )
				{
					// 01/18/2008 Paul.  Speed the reload by doing directly instead of going to SystemCheck page. 
					SplendidInit.InitApp();
					SplendidInit.LoadUserPreferences(Security.USER_ID, Sql.ToString(Session["USER_SETTINGS/THEME"]), Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
				}
				Response.Redirect("default.aspx");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Administration.LBL_MODULE_NAME"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
