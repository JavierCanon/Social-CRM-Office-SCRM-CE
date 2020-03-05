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
using System.Globalization;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Activities
{
	/// <summary>
	///		Summary description for SchedulingGrid.
	/// </summary>
	public class SchedulingGrid : SplendidControl
	{
		protected Label         lblError         ;
		protected PlaceHolder   plcSchedulingRows;
		protected HtmlTable     tblSchedule      ;
		protected DateTime      dtSCHEDULE_START ;
		protected DateTime      dtSCHEDULE_END   ;
		protected DateTime      dtDATE_START = DateTime.MinValue;
		protected DateTime      dtDATE_END   = DateTime.MaxValue;
		protected string[]      arrINVITEES      ;

		public CommandEventHandler Command ;

		public DateTime DATE_START
		{
			get
			{
				return dtDATE_START;
			}
			set
			{
				dtDATE_START = value;
			}
		}
		
		public DateTime DATE_END
		{
			get
			{
				return dtDATE_END;
			}
			set
			{
				dtDATE_END = value;
			}
		}

		public string[] INVITEES
		{
			get
			{
				return arrINVITEES;
			}
			set
			{
				arrINVITEES = value;
			}
		}
		
		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( Command != null )
					Command(this, e) ;
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void AddInvitee(Guid gUSER_ID)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				string sFULL_NAME    = String.Empty;
				string sINVITEE_TYPE = String.Empty;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwINVITEES" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gUSER_ID);

					if ( bDebug )
						RegisterClientScriptBlock("vwINVITEES", Sql.ClientScriptBlock(cmd));

					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							sFULL_NAME    = Sql.ToString(rdr["FULL_NAME"   ]);
							sINVITEE_TYPE = Sql.ToString(rdr["INVITEE_TYPE"]);
						}
					}
				}
				sSQL = "select *                                                       " + ControlChars.CrLf
				     + "  from vwACTIVITIES_List                                       " + ControlChars.CrLf
				     + " where ASSIGNED_USER_ID = @ASSIGNED_USER_ID                    " + ControlChars.CrLf
				     + "   and (   DATE_START >= @DATE_START and DATE_START < @DATE_END" + ControlChars.CrLf
				     + "        or DATE_END   >= @DATE_START and DATE_END   < @DATE_END" + ControlChars.CrLf
				     + "        or DATE_START <  @DATE_START and DATE_END   > @DATE_END" + ControlChars.CrLf
				     + "       )                                                       " + ControlChars.CrLf
				     + " order by DATE_START asc, NAME asc                             " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gUSER_ID        );
					Sql.AddParameter(cmd, "@DATE_START"      , T10n.ToServerTime(dtSCHEDULE_START));
					Sql.AddParameter(cmd, "@DATE_END"        , T10n.ToServerTime(dtSCHEDULE_END  ));

					if ( bDebug )
						RegisterClientScriptBlock("vwACTIVITIES_List", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								HtmlTableRow rowInvitee = new HtmlTableRow();
								tblSchedule.Rows.Add(rowInvitee);
								rowInvitee.Attributes.Add("class", "schedulerAttendeeRow");
								HtmlTableCell cellInvitee = new HtmlTableCell();
								rowInvitee.Cells.Add(cellInvitee);
								cellInvitee.Attributes.Add("class", "schedulerAttendeeCell");
								
								Literal litFULL_NAME = new Literal();
								Image   imgInvitee   = new Image();
								cellInvitee.Controls.Add(imgInvitee);
								cellInvitee.Controls.Add(litFULL_NAME);
								imgInvitee.Width      = 16;
								imgInvitee.Height     = 16;
								imgInvitee.ImageAlign = ImageAlign.AbsMiddle;
								imgInvitee.ImageUrl   = Session["themeURL"] + "images/" + sINVITEE_TYPE + ".gif";
								litFULL_NAME.Text     = sFULL_NAME;
								if ( dt.Rows.Count > 0 )
								{
									DataView vwMain = new DataView(dt);
									CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
									for(DateTime dtHOUR_START = dtSCHEDULE_START; dtHOUR_START < dtSCHEDULE_END ; dtHOUR_START = dtHOUR_START.AddMinutes(15) )
									{
										DateTime dtHOUR_END   = dtHOUR_START.AddMinutes(15);
										DateTime dtHOUR_START_ServerTime = T10n.ToServerTime(dtHOUR_START);
										DateTime dtHOUR_END_ServerTime   = T10n.ToServerTime(dtHOUR_END  );
										// 09/27/2005 Paul.  System.Data.DataColumn.Expression documentation has description how to define dates and strings. 
										// 08/08/2006 Paul.  Use the same ServerTime logic as DayGrid.ascx.cs to solve date formatting issues on international systems. 
										string sHOUR_START_ServerTime = dtHOUR_START_ServerTime.ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat);
										string sHOUR_END_ServerTime   = dtHOUR_END_ServerTime  .ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat);
										vwMain.RowFilter = "   DATE_START >= #" + sHOUR_START_ServerTime + "# and DATE_START <  #" + sHOUR_END_ServerTime + "#" + ControlChars.CrLf
										                 + "or DATE_END   >  #" + sHOUR_START_ServerTime + "# and DATE_END   <= #" + sHOUR_END_ServerTime + "#" + ControlChars.CrLf
										                 + "or DATE_START <  #" + sHOUR_START_ServerTime + "# and DATE_END   >  #" + sHOUR_END_ServerTime + "#" + ControlChars.CrLf;
#if DEBUG
//										RegisterClientScriptBlock("vwACTIVITIES_List" + dtHOUR_START.ToOADate().ToString(), Sql.EscapeJavaScript(vwMain.RowFilter));
#endif
										cellInvitee = new HtmlTableCell();
										rowInvitee.Cells.Add(cellInvitee);
										if ( dtHOUR_START == dtDATE_END )
											cellInvitee.Attributes.Add("class", "schedulerSlotCellEndTime"  );
										else if ( dtHOUR_START == dtDATE_START )
											cellInvitee.Attributes.Add("class", "schedulerSlotCellStartTime");
										else
											cellInvitee.Attributes.Add("class", "schedulerSlotCellHour"     );
										if ( vwMain.Count > 0 )
										{
											if ( dtHOUR_START >= dtDATE_START && dtHOUR_START < dtDATE_END )
												cellInvitee.Attributes.Add("style", "BACKGROUND-COLOR: #aa4d4d");
											else
												cellInvitee.Attributes.Add("style", "BACKGROUND-COLOR: #4d5eaa");
										}
										else
										{
											if ( dtHOUR_START >= dtDATE_START && dtHOUR_START < dtDATE_END )
												cellInvitee.Attributes.Add("style", "BACKGROUND-COLOR: #ffffff");
										}
									}
								}
								else
								{
									for(DateTime dtHOUR_START = dtSCHEDULE_START; dtHOUR_START < dtSCHEDULE_END ; dtHOUR_START = dtHOUR_START.AddMinutes(15) )
									{
										DateTime dtHOUR_END   = dtHOUR_START.AddMinutes(15);
										cellInvitee = new HtmlTableCell();
										rowInvitee.Cells.Add(cellInvitee);
										if ( dtHOUR_START == dtDATE_END )
											cellInvitee.Attributes.Add("class", "schedulerSlotCellEndTime"  );
										else if ( dtHOUR_START == dtDATE_START )
											cellInvitee.Attributes.Add("class", "schedulerSlotCellStartTime");
										else
											cellInvitee.Attributes.Add("class", "schedulerSlotCellHour"     );
										if ( dtHOUR_START >= dtDATE_START && dtHOUR_START < dtDATE_END )
											cellInvitee.Attributes.Add("style", "BACKGROUND-COLOR: #ffffff");
									}
								}
								cellInvitee = new HtmlTableCell();
								rowInvitee.Cells.Add(cellInvitee);
								cellInvitee.Attributes.Add("class", "schedulerAttendeeDeleteCell");
								ImageButton btnDelete = new ImageButton();
								Literal     litSpace  = new Literal();
								LinkButton  lnkDelete = new LinkButton();
								btnDelete.CommandName     = "Invitees.Delete";
								lnkDelete.CommandName     = "Invitees.Delete";
								btnDelete.CommandArgument = gUSER_ID.ToString();
								lnkDelete.CommandArgument = gUSER_ID.ToString();
								btnDelete.Command        += new CommandEventHandler(this.Page_Command);
								lnkDelete.Command        += new CommandEventHandler(this.Page_Command);
								btnDelete.CssClass        = "listViewTdToolsS1";
								lnkDelete.CssClass        = "listViewTdToolsS1";
								
								Guid gID = Sql.ToGuid(Request["ID"]);
								if ( Sql.IsEmptyGuid(gID) )
								{
									btnDelete.AlternateText   = L10n.Term(".LNK_REMOVE");
									lnkDelete.Text            = L10n.Term(".LNK_REMOVE");
								}
								else
								{
									btnDelete.AlternateText   = L10n.Term(".LNK_DELETE");
									lnkDelete.Text            = L10n.Term(".LNK_DELETE");
								}
								litSpace.Text             = " ";
								btnDelete.ImageUrl        = Session["themeURL"] + "images/delete_inline.gif";
								btnDelete.BorderWidth     = 0 ;
								btnDelete.Width           = 12;
								btnDelete.Height          = 12;
								btnDelete.ImageAlign      = ImageAlign.AbsMiddle;
								cellInvitee.Controls.Add(btnDelete);
								cellInvitee.Controls.Add(litSpace );
								cellInvitee.Controls.Add(lnkDelete);
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
			}
		}

		public void BuildSchedule()
		{
			try
			{
				dtSCHEDULE_START = new DateTime(dtDATE_START.Year, dtDATE_START.Month, dtDATE_START.Day, dtDATE_START.Hour, 0, 0, 0);
				// 01/16/2006 Paul.  Date may not allow adding hours.  It may already be at the minimum.  Just ignore the error. 
				dtSCHEDULE_START = dtSCHEDULE_START.AddHours(-4);
				dtSCHEDULE_END   = dtSCHEDULE_START.AddHours(9);
			}
			catch
			{
				return;
			}
			try
			{
				tblSchedule.Rows.Clear();
				
				HtmlTableRow  rowHeader  = new HtmlTableRow();
				tblSchedule.Rows.Add(rowHeader);
				HtmlTableCell cellHeader = new HtmlTableCell();
				rowHeader.Cells.Add(cellHeader);
				rowHeader .Attributes.Add("class", "schedulerTopRow"     );
				cellHeader.Attributes.Add("class", "schedulerTopDateCell");
				cellHeader.Align   = "middle";
				cellHeader.Height  = "20";
				cellHeader.ColSpan = (dtSCHEDULE_END - dtSCHEDULE_START).Hours * 4 + 2;  // 38;
				cellHeader.InnerText = dtDATE_START.ToLongDateString();
				
				HtmlTableRow  rowTime = new HtmlTableRow();
				tblSchedule.Rows.Add(rowTime);
				HtmlTableCell cellTime = new HtmlTableCell();
				cellTime.Attributes.Add("class", "schedulerAttendeeHeaderCell");
				rowTime.Cells.Add(cellTime);
				for(DateTime dtHOUR_START = dtSCHEDULE_START; dtHOUR_START < dtSCHEDULE_END ; dtHOUR_START = dtHOUR_START.AddHours(1) )
				{
					cellTime = new HtmlTableCell();
					cellTime.Attributes.Add("class", "schedulerTimeCell");
					cellTime.ColSpan = 4;
					cellTime.InnerText = dtHOUR_START.ToShortTimeString();
					rowTime.Cells.Add(cellTime);
				}
				cellTime = new HtmlTableCell();
				cellTime.Attributes.Add("class", "schedulerDeleteHeaderCell");
				rowTime.Cells.Add(cellTime);

				if ( arrINVITEES != null )
				{
					foreach(string sINVITEE_ID in arrINVITEES)
					{
						Guid gINVITEE_ID = Guid.Empty;
						try
						{
							gINVITEE_ID = Sql.ToGuid(sINVITEE_ID);
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
						if ( !Sql.IsEmptyGuid(gINVITEE_ID) )
							AddInvitee(gINVITEE_ID);
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
			// 02/21/2006 Paul.  Don't DataBind, otherwise it will cause the DropDownLists to loose their selected value. 
			//Page.DataBind();
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
