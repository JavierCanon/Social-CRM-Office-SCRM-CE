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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Schedulers
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Schedulers.Delete" )
				{
					Guid gID = Sql.ToGuid(e.CommandArgument);
					SqlProcs.spSCHEDULERS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("~/Administration/default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = Server.HtmlEncode(ex.Message);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.BufferOutput = true;
			SetPageTitle(L10n.Term("Schedulers.LBL_MODULE_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *               " + ControlChars.CrLf
					     + "     , '' as DATE_RANGE" + ControlChars.CrLf
					     + "  from vwSCHEDULERS    " + ControlChars.CrLf
					     + " where 1 = 1           " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string   sJOB_INTERVAL     = Sql.ToString  (row["JOB_INTERVAL"   ]);
									DateTime dtDATE_TIME_START = Sql.ToDateTime(row["DATE_TIME_START"]);
									DateTime dtDATE_TIME_END   = Sql.ToDateTime(row["DATE_TIME_END"  ]);
									DateTime dtLAST_RUN        = Sql.ToDateTime(row["LAST_RUN"       ]);
									row["JOB_INTERVAL"] = sJOB_INTERVAL + "<br>" + SchedulerUtils.CronDescription(L10n, sJOB_INTERVAL);
									if ( dtDATE_TIME_START != DateTime.MinValue )
										row["DATE_RANGE"] = T10n.FromServerTime(dtDATE_TIME_START).ToString() + "-";
									if ( dtDATE_TIME_END == DateTime.MinValue )
										row["DATE_RANGE"] += L10n.Term("Schedulers.LBL_PERENNIAL");
									else
										row["DATE_RANGE"] += T10n.FromServerTime(dtDATE_TIME_END).ToString();
									if ( dtLAST_RUN != DateTime.MinValue )
										row["LAST_RUN"] = T10n.FromServerTime(dtLAST_RUN);
									row["STATUS"] = L10n.Term(".scheduler_status_dom.", row["STATUS"]);
								}
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									grdMain.SortColumn = "NAME";
									grdMain.SortOrder  = "asc" ;
									grdMain.ApplySort();
									grdMain.DataBind();
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
