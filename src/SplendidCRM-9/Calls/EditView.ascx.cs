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
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;
using SplendidCRM._controls;

namespace SplendidCRM.Calls
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;

		protected InviteesView    ctlInviteesView              ;
		protected HtmlInputHidden txtINVITEE_ID                ;

		protected Activities.SchedulingGrid ctlSchedulingGrid  ;
		//protected _controls.DateTimePicker  ctlDATE_START      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 08/21/2005 Paul.  Redirect to parent if that is where the note was originated. 
			Guid   gPARENT_ID   = Sql.ToGuid(Request["PARENT_ID"]);
			string sMODULE      = String.Empty;
			string sPARENT_TYPE = String.Empty;
			string sPARENT_NAME = String.Empty;
			try
			{
				SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// The only possible error is a connection failure, so just ignore all errors. 
				gPARENT_ID = Guid.Empty;
			}
			if ( e.CommandName == "Save" )
			{
				/*
				DateTimePicker ctlDATE_START = FindControl("DATE_START") as DateTimePicker;
				if ( ctlDATE_START != null )
					ctlDATE_START.Validate();
				*/
				// 01/16/2006 Paul.  Enable validator before validating page. 
				this.ValidateEditViewFields(m_sMODULE + ".EditView");
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "CALLS";
					DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
						DataRow   rowCurrent = null;
						DataTable dtCurrent  = new DataTable();
						if ( !Sql.IsEmptyGuid(gID) )
						{
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwCALLS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtCurrent);
									if ( dtCurrent.Rows.Count > 0 )
									{
										rowCurrent = dtCurrent.Rows[0];
									}
									else
									{
										// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
										// It is possible that the record exists, but that ACL rules prevent it from being selected. 
										gID = Guid.Empty;
									}
								}
							}
						}

						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spCALLS_Update(ref gID
									, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID").ID
									, new DynamicControl(this, rowCurrent, "NAME"            ).Text
									, new DynamicControl(this, rowCurrent, "DURATION_HOURS"  ).IntegerValue
									, new DynamicControl(this, rowCurrent, "DURATION_MINUTES").IntegerValue
									, new DynamicControl(this, rowCurrent, "DATE_START"      ).DateValue
									, new DynamicControl(this, rowCurrent, "PARENT_TYPE"     ).SelectedValue
									, new DynamicControl(this, rowCurrent, "PARENT_ID"       ).ID
									, new DynamicControl(this, rowCurrent, "STATUS"          ).SelectedValue
									, new DynamicControl(this, rowCurrent, "DIRECTION"       ).SelectedValue
									, new DynamicControl(this, rowCurrent, "SHOULD_REMIND"   ).Checked ? new DynamicControl(this, "REMINDER_TIME").IntegerValue : -1
									, new DynamicControl(this, rowCurrent, "DESCRIPTION"     ).Text
									, txtINVITEE_ID.Value
									, new DynamicControl(this, rowCurrent, "TEAM_ID"         ).ID
									, trn
									);
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								ctlEditButtons.ErrorText = ex.Message;
								return;
							}
						}
					}
					if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gPARENT_ID) )
					Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
			else if ( e.CommandName == "Invitees.Add" )
			{
				if ( txtINVITEE_ID.Value.Length > 0 )
					txtINVITEE_ID.Value += ",";
				txtINVITEE_ID.Value += e.CommandArgument;
				ctlInviteesView.INVITEES = txtINVITEE_ID.Value.Split(',');
				BindSchedule();
			}
			else if ( e.CommandName == "Invitees.Delete" )
			{
				string sDELETE_ID = e.CommandArgument.ToString().ToLower();
				string[] arrINVITEES = txtINVITEE_ID.Value.Split(',');
				StringBuilder sb = new StringBuilder();
				foreach(string sINVITEE_ID in arrINVITEES)
				{
					if ( sINVITEE_ID != sDELETE_ID )
					{
						if ( sb.Length > 0 )
							sb.Append(",");
						sb.Append(sINVITEE_ID);
					}
				}
				txtINVITEE_ID.Value = sb.ToString();
				ctlInviteesView.INVITEES = txtINVITEE_ID.Value.Split(',');
				BindSchedule();
			}
			else if ( e.CommandName == "Search" )
			{
				BindSchedule();
			}
		}

		protected void Date_Changed(object sender, System.EventArgs e)
		{
			BindSchedule();
		}

		private void BindSchedule()
		{
			DateTimePicker ctlDATE_START = FindControl("DATE_START" ) as DateTimePicker;
			if ( ctlDATE_START != null )
			{
				int nDURATION_HOURS   = new DynamicControl(this, "DURATION_HOURS"  ).IntegerValue;
				int nDURATION_MINUTES = new DynamicControl(this, "DURATION_MINUTES").IntegerValue;
				DateTime dtDATE_END = ctlDATE_START.Value.AddHours(nDURATION_HOURS).AddMinutes(nDURATION_MINUTES);
				// 07/09/2006 Paul.  The date values are sent to the scheduling grid in TimeZone time. 
				// The dates are converted to server time when the database is queried. 
				ctlSchedulingGrid.DATE_START = ctlDATE_START.Value;
				ctlSchedulingGrid.DATE_END   = dtDATE_END;
				ctlSchedulingGrid.INVITEES   = txtINVITEE_ID.Value.Split(',');
				ctlSchedulingGrid.BuildSchedule();
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwCALLS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AppendParameter(cmd, gDuplicateID, "ID", false);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AppendParameter(cmd, gID, "ID", false);
								}

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
										
										this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, rdr);
										// 07/12/2006 Paul.  Need to enable schedule updates. 
										DateTimePicker ctlDATE_START = FindControl("DATE_START") as DateTimePicker;
										if ( ctlDATE_START != null )
										{
											ctlDATE_START.Changed += new System.EventHandler(this.Date_Changed);
											ctlDATE_START.AutoPostBack = true;
										}
										// 08/02/2005 Paul.  Set status to Held when closing from Home page.
										// 06/21/2006 Paul.  Change parameter to Close so that the same parameter can be used for Calls, Meetings and Tasks. 
										if ( Sql.ToString(Request["Status"]) == "Close" )
											new DynamicControl(this, "STATUS").SelectedValue = "Held";
										else
											new DynamicControl(this, "STATUS").SelectedValue = Sql.ToString(rdr["STATUS"]);
										int nMinutes = Sql.ToInteger(rdr["DURATION_MINUTES"]);
										if ( nMinutes <= 7 )
											new DynamicControl(this, "DURATION_MINUTES").SelectedValue = "00";
										else if ( nMinutes <= 15+7 )
											new DynamicControl(this, "DURATION_MINUTES").SelectedValue = "15";
										else if ( nMinutes <= 30+7 )
											new DynamicControl(this, "DURATION_MINUTES").SelectedValue = "30";
										else
											new DynamicControl(this, "DURATION_MINUTES").SelectedValue = "45";
										int nREMINDER_TIME = Sql.ToInteger(rdr["REMINDER_TIME"]);
										if ( nREMINDER_TIME >= 0 )
										{
											new DynamicControl(this, "REMINDER_TIME").SelectedValue = nREMINDER_TIME.ToString();
											new DynamicControl(this, "SHOULD_REMIND").Checked = true;
										}
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't allow save 
										ctlEditButtons.DisableAll();
										ctlEditButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
							sSQL = "select INVITEE_ID                            " + ControlChars.CrLf
							     + "  from vwCALLS_Invitees                      " + ControlChars.CrLf
							     + " where CALL_ID = @ID                         " + ControlChars.CrLf
							     + " order by INVITEE_TYPE desc, INVITEE_NAME asc" + ControlChars.CrLf;
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

								if ( bDebug )
									RegisterClientScriptBlock("vwCALLS_Invitees", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									StringBuilder sb = new StringBuilder();
									while ( rdr.Read() )
									{
										if ( sb.Length > 0 )
											sb.Append(",");
										sb.Append(Sql.ToString(rdr["INVITEE_ID"]).ToLower());
									}
									txtINVITEE_ID.Value = sb.ToString();
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
						
						DateTimePicker ctlDATE_START = FindControl("DATE_START") as DateTimePicker;
						if ( ctlDATE_START != null )
						{
							ctlDATE_START.Changed += new System.EventHandler(this.Date_Changed);
							ctlDATE_START.AutoPostBack = true;
							// Default start date and time is now. 
							ctlDATE_START.Value = T10n.FromServerTime(DateTime.Now);
						}
						// Default value for duration is 15 minutes. 
						new DynamicControl(this, "DURATION_MINUTES").Text = "15";
						// Default to 0 hours. 
						new DynamicControl(this, "DURATION_HOURS"  ).Text = "0";
						// Default to remind. 
						new DynamicControl(this, "SHOULD_REMIND"   ).Checked = true;

						Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							string sMODULE      = String.Empty;
							string sPARENT_TYPE = String.Empty;
							string sPARENT_NAME = String.Empty;
							SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								new DynamicControl(this, "PARENT_ID"  ).ID   = gPARENT_ID;
								new DynamicControl(this, "PARENT_NAME").Text = sPARENT_NAME;
								new DynamicControl(this, "PARENT_TYPE").SelectedValue = sPARENT_TYPE;
							}
						}
					}
					// Default to current user. 
					if ( txtINVITEE_ID.Value.Length == 0 )
						txtINVITEE_ID.Value = Security.USER_ID.ToString();
					BindSchedule();
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
					// 11/09/2005 Paul.  Need to rebind early so that the Delete event will fire. 
					BindSchedule();
					ctlInviteesView.INVITEES = txtINVITEE_ID.Value.Split(',');
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
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
			ctlInviteesView.Command += new CommandEventHandler(this.Page_Command);
			ctlSchedulingGrid.Command += new CommandEventHandler(this.Page_Command);
			m_sMODULE = "Calls";
			// 02/13/2007 Paul.  Calls should highlight the Activities menu. 
			SetMenu("Activities");
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);

				DateTimePicker ctlDATE_START = FindControl("DATE_START") as DateTimePicker;
				if ( ctlDATE_START != null )
				{
					ctlDATE_START.Changed += new System.EventHandler(this.Date_Changed);
					ctlDATE_START.AutoPostBack = true;
				}
			}
		}
		#endregion
	}
}
