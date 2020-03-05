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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Updater
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.EditButtons  ctlEditButtons ;

		protected DataView     vwMain               ;
		protected CheckBox     SEND_USAGE_INFO      ;
		protected CheckBox     CHECK_UPDATES        ;
		protected Table        AVAILABLE_UPDATES    ;
		protected Label        NO_UPDATES           ;
		protected SplendidGrid grdMain              ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					try
					{
						Application["CONFIG.send_usage_info"] = SEND_USAGE_INFO.Checked ? "true" : "false";
						SqlProcs.spCONFIG_Update("system", "send_usage_info", Sql.ToString(Application["CONFIG.send_usage_info"]));
						
						SqlProcs.spSCHEDULERS_UpdateStatus("function::CheckVersion", CHECK_UPDATES.Checked ? "Active" : "Inactive");
						if ( CHECK_UPDATES.Checked )
						{
							Application.Remove("available_version"            );
							Application.Remove("available_version_description");
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlEditButtons.ErrorText = ex.Message;
						return;
					}
					Response.Redirect("../default.aspx");
				}
			}
			else if ( e.CommandName == "CheckNow" )
			{
				try
				{
					DataTable dt = Utils.CheckVersion(Application);

					vwMain = dt.DefaultView;
					vwMain.RowFilter = "New = '1'";
					vwMain.Sort      = "Build desc";
					grdMain.DataSource = vwMain ;
					grdMain.DataBind();
					grdMain.Visible    = (vwMain.Count > 0);
					NO_UPDATES.Visible = (vwMain.Count == 0);

					vwMain.RowFilter = String.Empty;
					if ( CHECK_UPDATES.Checked && vwMain.Count > 0 )
					{
						Application["available_version"            ] = Sql.ToString(vwMain[0]["Build"      ]);
						Application["available_version_description"] = Sql.ToString(vwMain[0]["Description"]);
					}
					else
					{
						Application.Remove("available_version"            );
						Application.Remove("available_version_description");
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlEditButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Administration.LBL_CONFIGURE_UPDATER"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				if ( !IsPostBack )
				{
					SEND_USAGE_INFO.Checked = Sql.ToBoolean(Application["CONFIG.send_usage_info"]);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select STATUS      " + ControlChars.CrLf
						     + "  from vwSCHEDULERS" + ControlChars.CrLf
						     + " where JOB = @JOB  " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@JOB", "function::CheckVersion");
							CHECK_UPDATES.Checked = (Sql.ToString(cmd.ExecuteScalar()) == "Active");
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlEditButtons.ErrorText = ex.Message;
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
			// 05/20/2007 Paul.  The m_sMODULE field must be set in order to allow default export handling. 
			m_sMODULE = "Administration";
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
