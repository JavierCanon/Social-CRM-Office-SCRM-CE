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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Schedulers
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid         gID            ;
		protected TextBox      NAME           ;
		protected DropDownList STATUS         ;
		protected DropDownList JOB            ;
		protected CheckBox     CATCH_UP       ;
		protected TextBox      CRON_MINUTES   ;
		protected TextBox      CRON_HOURS     ;
		protected TextBox      CRON_DAYOFMONTH;
		protected TextBox      CRON_MONTHS    ;
		protected TextBox      CRON_DAYOFWEEK ;

		protected _controls.DateTimePicker DATE_TIME_START;
		protected _controls.DateTimePicker DATE_TIME_END  ;
		protected _controls.TimePicker     TIME_FROM      ;
		protected _controls.TimePicker     TIME_TO        ;

		protected RequiredFieldValidator NAME_REQUIRED;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				NAME.Text = NAME.Text.Trim();
				NAME_REQUIRED.Enabled = true;
				NAME_REQUIRED.Validate();
				if ( Page.IsValid )
				{
					try
					{
						CRON_MINUTES   .Text = CRON_MINUTES   .Text.Trim();
						CRON_HOURS     .Text = CRON_HOURS     .Text.Trim();
						CRON_DAYOFMONTH.Text = CRON_DAYOFMONTH.Text.Trim();
						CRON_MONTHS    .Text = CRON_MONTHS    .Text.Trim();
						CRON_DAYOFWEEK .Text = CRON_DAYOFWEEK .Text.Trim();
						if ( Sql.IsEmptyString(CRON_MINUTES   .Text) ) CRON_MINUTES   .Text = "*";
						if ( Sql.IsEmptyString(CRON_HOURS     .Text) ) CRON_HOURS     .Text = "*";
						if ( Sql.IsEmptyString(CRON_DAYOFMONTH.Text) ) CRON_DAYOFMONTH.Text = "*";
						if ( Sql.IsEmptyString(CRON_MONTHS    .Text) ) CRON_MONTHS    .Text = "*";
						if ( Sql.IsEmptyString(CRON_DAYOFWEEK .Text) ) CRON_DAYOFWEEK .Text = "*";
						string sJOB_INTERVAL = CRON_MINUTES.Text + "::" + CRON_HOURS.Text + "::" + CRON_DAYOFMONTH.Text + "::" + CRON_MONTHS.Text + "::" + CRON_DAYOFWEEK.Text;
						SqlProcs.spSCHEDULERS_Update(ref gID, NAME.Text, JOB.SelectedValue, DATE_TIME_START.Value, DATE_TIME_END.Value, sJOB_INTERVAL, TIME_FROM.Value, TIME_TO.Value, STATUS.SelectedValue, CATCH_UP.Checked);
					}
					catch(Exception ex)
					{
						ctlEditButtons.ErrorText = ex.Message;
						return;
					}
					Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList.Schedulers"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			NAME_REQUIRED.DataBind();
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					STATUS.DataSource = SplendidCache.List("scheduler_status_dom");
					STATUS.DataBind();
					foreach ( string sJob in SchedulerUtils.Jobs )
					{
						JOB.Items.Add(new ListItem(sJob, "function::" + sJob));
					}

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
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
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList.Schedulers") + " - " + ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										NAME    .Text    = Sql.ToString (rdr["NAME"    ]);
										CATCH_UP.Checked = Sql.ToBoolean(rdr["CATCH_UP"]);

										string JOB_INTERVAL = Sql.ToString(rdr["JOB_INTERVAL"]);
										JOB_INTERVAL = JOB_INTERVAL.Replace(" ", "");
										string[] arrCRON    = JOB_INTERVAL.Replace("::", "|").Split('|');
										// minute  hour  dayOfMonth  month  dayOfWeek
										CRON_MINUTES   .Text = (arrCRON.Length > 0) ? arrCRON[0] : "*";
										CRON_HOURS     .Text = (arrCRON.Length > 1) ? arrCRON[1] : "*";
										CRON_DAYOFMONTH.Text = (arrCRON.Length > 2) ? arrCRON[2] : "*";
										CRON_MONTHS    .Text = (arrCRON.Length > 3) ? arrCRON[3] : "*";
										CRON_DAYOFWEEK .Text = (arrCRON.Length > 4) ? arrCRON[4] : "*";

										if ( rdr["DATE_TIME_START"] != DBNull.Value ) DATE_TIME_START.Value = T10n.FromServerTime(Sql.ToDateTime(rdr["DATE_TIME_START"]));
										if ( rdr["DATE_TIME_END"  ] != DBNull.Value ) DATE_TIME_END  .Value = T10n.FromServerTime(Sql.ToDateTime(rdr["DATE_TIME_END"  ]));
										// 12/31/2007 Paul.  TIME_FROM and TIME_TO are just time components, so they should not be translated. 
										if ( rdr["TIME_FROM"      ] != DBNull.Value ) TIME_FROM      .Value = Sql.ToDateTime(rdr["TIME_FROM"]);
										if ( rdr["TIME_TO"        ] != DBNull.Value ) TIME_TO        .Value = Sql.ToDateTime(rdr["TIME_TO"  ]);
										try
										{
											JOB.SelectedValue = Sql.ToString(rdr["JOB"]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
										}
										try
										{
											STATUS.SelectedValue = Sql.ToString(rdr["STATUS"]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
										}
									}
								}
							}
						}
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList.Schedulers") + " - " + ctlModuleHeader.Title);
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
