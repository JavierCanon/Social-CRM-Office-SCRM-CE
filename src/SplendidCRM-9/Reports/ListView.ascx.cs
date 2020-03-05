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

namespace SplendidCRM.Reports
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected SplendidCRM._controls.ListHeader ctlListHeaderMySaved  ;
		protected SplendidCRM._controls.ListHeader ctlListHeaderPublished;

		protected DataView      vwMySaved    ;
		protected DataView      vwPublished  ;
		protected SplendidGrid  grdMySaved   ;
		protected SplendidGrid  grdPublished ;
		protected Label         lblError     ;
		protected string        sMODULE_NAME ;
		protected int           nACLACCESS_Export;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Reports.Create":
					{
						Response.Redirect("edit.aspx");
						break;
					}
					case "Reports.Import":
					{
						Response.Redirect("import.aspx");
						break;
					}
					case "Reports.View":
					{
						Guid gID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("view.aspx?ID=" + gID.ToString());
						break;
					}
					case "Reports.Publish":
					{
						Guid gID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spREPORTS_Publish(gID);
						Response.Redirect("default.aspx?MODULE_NAME=" + sMODULE_NAME);
						break;
					}
					case "Reports.Unpublish":
					{
						Guid gID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spREPORTS_Unpublish(gID);
						Response.Redirect("default.aspx?MODULE_NAME=" + sMODULE_NAME);
						break;
					}
					case "Reports.Delete":
					{
						Guid gID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spREPORTS_Delete(gID);
						Response.Redirect("default.aspx?MODULE_NAME=" + sMODULE_NAME);
						break;
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;
			
			nACLACCESS_Export = Security.GetUserAccess(m_sMODULE, "export");
			try
			{
				sMODULE_NAME = Sql.ToString(Request["MODULE_NAME"]);
				ctlListHeaderMySaved  .Title = ".saved_reports_dom."     + sMODULE_NAME;
				ctlListHeaderPublished.Title = ".published_reports_dom." + sMODULE_NAME;
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *             " + ControlChars.CrLf
					     + "  from vwREPORTS_List" + ControlChars.CrLf
					     + " where 1 = 1         " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, sMODULE_NAME, "MODULE_NAME");

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 06/18/2006 Paul.  Translate the report type. 
								foreach(DataRow row in dt.Rows)
								{
									row["REPORT_TYPE"] = L10n.Term(".dom_report_types.", row["REPORT_TYPE"]);
								}

								vwMySaved = new DataView(dt);
								vwMySaved.RowFilter = "PUBLISHED = 0 and ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
								grdMySaved.DataSource = vwMySaved ;
								if ( !IsPostBack )
								{
									grdMySaved.SortColumn = "NAME";
									grdMySaved.SortOrder  = "asc" ;
									grdMySaved.ApplySort();
									grdMySaved.DataBind();
								}
								vwPublished = new DataView(dt);
								// 05/18/2006 Paul.  Lets include unassigned so that they don't get lost. 
								vwPublished.RowFilter = "PUBLISHED = 1 or ASSIGNED_USER_ID is null";
								grdPublished.DataSource = vwPublished;
								if ( !IsPostBack )
								{
									grdPublished.SortColumn = "NAME";
									grdPublished.SortOrder  = "asc" ;
									grdPublished.ApplySort();
									grdPublished.DataBind();
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
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
			m_sMODULE = "Reports";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
