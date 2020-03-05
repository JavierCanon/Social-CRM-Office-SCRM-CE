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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Schedulers
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected _controls.ModuleHeader  ctlModuleHeader ;
		protected _controls.DetailButtons ctlDetailButtons;

		protected Guid    gID            ;
		protected Label   JOB            ;
		protected Label   STATUS         ;
		protected Label   DATE_TIME_START;
		protected Label   TIME_FROM      ;
		protected Label   DATE_TIME_END  ;
		protected Label   TIME_TO        ;
		protected Label   LAST_RUN       ;
		protected Label   JOB_INTERVAL   ;
		protected Label   CATCH_UP       ;
		protected Label   DATE_ENTERED   ;
		protected Label   DATE_MODIFIED  ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Response.Redirect("edit.aspx?DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spSCHEDULERS_Delete(gID);
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDetailButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList.Schedulers"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwSCHEDULERS" + ControlChars.CrLf
							     + " where ID = @ID    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList.Schedulers") + " - " + ctlModuleHeader.Title);
										
										string   sJOB_INTERVAL     = Sql.ToString  (rdr["JOB_INTERVAL"   ]);
										DateTime dtDATE_TIME_START = Sql.ToDateTime(rdr["DATE_TIME_START"]);
										DateTime dtDATE_TIME_END   = Sql.ToDateTime(rdr["DATE_TIME_END"  ]);
										DateTime dtTIME_FROM       = Sql.ToDateTime(rdr["TIME_FROM"      ]);
										DateTime dtTIME_TO         = Sql.ToDateTime(rdr["TIME_TO"        ]);
										DateTime dtLAST_RUN        = Sql.ToDateTime(rdr["LAST_RUN"       ]);
										JOB            .Text = Sql.ToString(rdr["JOB"   ]);
										STATUS         .Text = Sql.ToString(L10n.Term(".scheduler_status_dom.", rdr["STATUS"]));
										DATE_TIME_START.Text = (dtDATE_TIME_START == DateTime.MinValue) ? L10n.Term("Schedulers.LBL_PERENNIAL") : T10n.FromServerTime(dtDATE_TIME_START).ToString();
										DATE_TIME_END  .Text = (dtDATE_TIME_END   == DateTime.MinValue) ? L10n.Term("Schedulers.LBL_PERENNIAL") : T10n.FromServerTime(dtDATE_TIME_END  ).ToString();
										LAST_RUN       .Text = (dtLAST_RUN        == DateTime.MinValue) ? L10n.Term("Schedulers.LBL_NEVER"    ) : T10n.FromServerTime(dtLAST_RUN       ).ToString();
										TIME_FROM      .Text = (dtTIME_FROM       == DateTime.MinValue) ? L10n.Term("Schedulers.LBL_ALWAYS"   ) : dtTIME_FROM.ToShortTimeString();
										TIME_TO        .Text = (dtTIME_TO         == DateTime.MinValue) ? L10n.Term("Schedulers.LBL_ALWAYS"   ) : dtTIME_TO  .ToShortTimeString();
										CATCH_UP       .Text = Sql.ToBoolean(rdr["CATCH_UP"])           ? L10n.Term("Schedulers.LBL_ALWAYS"   ) : L10n.Term("Schedulers.LBL_NEVER");
										JOB_INTERVAL   .Text = sJOB_INTERVAL + "<br>" + SchedulerUtils.CronDescription(L10n, sJOB_INTERVAL);
										DATE_ENTERED   .Text = T10n.FromServerTime(Sql.ToDateTime(rdr["DATE_ENTERED" ])).ToString() + " " + L10n.Term(".LBL_BY") + " " + Sql.ToString(rdr["CREATED_BY" ]);
										DATE_MODIFIED  .Text = T10n.FromServerTime(Sql.ToDateTime(rdr["DATE_MODIFIED"])).ToString() + " " + L10n.Term(".LBL_BY") + " " + Sql.ToString(rdr["MODIFIED_BY"]);
									}
								}
							}
						}
					}
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDetailButtons.ErrorText = ex.Message;
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDetailButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
