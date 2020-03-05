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

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for WeekGrid.
	/// </summary>
	public class WeekGrid : CalendarControl
	{
		protected Label          lblError         ;
		protected DateTime       dtCurrentWeek    ;
		protected PlaceHolder    plcWeekRows      ;
		protected CalendarHeader ctlCalendarHeader;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Week.Previous":
					{
						dtCurrentDate = dtCurrentDate.AddDays(-7);
						dtCurrentWeek = dtCurrentDate.AddDays(DayOfWeek.Sunday - dtCurrentDate.DayOfWeek);
						ViewState["CurrentDate"] = dtCurrentDate;
						break;
					}
					case "Week.Next":
					{
						dtCurrentDate = dtCurrentDate.AddDays(7);
						dtCurrentWeek = dtCurrentDate.AddDays(DayOfWeek.Sunday - dtCurrentDate.DayOfWeek);
						ViewState["CurrentDate"] = dtCurrentDate;
						break;
					}
					case "Day.Current":
					{
						Response.Redirect("default.aspx?" + CalendarQueryString(dtCurrentDate));
						break;
					}
					case "Week.Current":
					{
						ViewState["CurrentDate"] = dtCurrentDate;
						break;
					}
					case "Month.Current":
					{
						Response.Redirect("Month.aspx?" + CalendarQueryString(dtCurrentDate));
						break;
					}
					case "Year.Current":
					{
						Response.Redirect("Year.aspx?" + CalendarQueryString(dtCurrentDate));
						break;
					}
					case "Shared.Current":
					{
						Response.Redirect("Shared.aspx?" + CalendarQueryString(dtCurrentDate));
						break;
					}
				}
				BindGrid();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void BindGrid()
		{
			plcWeekRows.Controls.Clear();
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                                                       " + ControlChars.CrLf
					     + "  from vwACTIVITIES_List                                       " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						DateTime dtDATE_START = new DateTime(Math.Max(1753, dtCurrentWeek.Year), dtCurrentWeek.Month, dtCurrentWeek.Day, 0, 0, 0);
						DateTime dtDATE_END   = dtDATE_START.AddDays(7);
						// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
						Security.Filter(cmd, "Calls", "list");
						// 01/16/2007 Paul.  Use AppendParameter so that duplicate ASSIGNED_USER_ID can be avoided. 
						// 01/19/2007 Paul.  Fix AppendParamenter.  @ should not be used in field name. 
						Sql.AppendParameter(cmd, Security.USER_ID, "ASSIGNED_USER_ID");
						cmd.CommandText += "   and (   DATE_START >= @DATE_START and DATE_START < @DATE_END" + ControlChars.CrLf;
						cmd.CommandText += "        or DATE_END   >= @DATE_START and DATE_END   < @DATE_END" + ControlChars.CrLf;
						cmd.CommandText += "        or DATE_START <  @DATE_START and DATE_END   > @DATE_END" + ControlChars.CrLf;
						cmd.CommandText += "       )                                                       " + ControlChars.CrLf;
						cmd.CommandText += " order by DATE_START asc, NAME asc                             " + ControlChars.CrLf;
						// 03/19/2007 Paul.  Need to query activities based on server time. 
						Sql.AddParameter(cmd, "@DATE_START", T10n.ToServerTime(dtDATE_START));
						Sql.AddParameter(cmd, "@DATE_END"  , T10n.ToServerTime(dtDATE_END  ));

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
									// 07/24/2005 Paul.  Since this is not a dynamic grid, we must convert the status manually. 
									foreach(DataRow row in dt.Rows)
									{
										switch ( Sql.ToString(row["ACTIVITY_TYPE"]) )
										{
											case "Calls"   :  row["STATUS"] = L10n.Term("Call"   ) + " " + L10n.Term(".call_status_dom."   , row["STATUS"]);  break;
											case "Meetings":  row["STATUS"] = L10n.Term("Meeting") + " " + L10n.Term(".meeting_status_dom.", row["STATUS"]);  break;
										}
									}
									CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
									for(int iDay = 0 ; iDay < 7 ; iDay++ )
									{
										DataView vwMain = new DataView(dt);
										DateTime dtDAY_START = dtCurrentWeek;
										dtDAY_START = dtDAY_START.AddDays(iDay);
										DateTime dtDAY_END   = dtDAY_START.AddDays(1);
										// 03/19/2007 Paul.  Need to query activities based on server time. 
										DateTime dtDAY_START_ServerTime = T10n.ToServerTime(dtDAY_START);
										DateTime dtDAY_END_ServerTime   = T10n.ToServerTime(dtDAY_END  );
										// 09/27/2005 Paul.  System.Data.DataColumn.Expression documentation has description how to define dates and strings. 
										// 01/21/2006 Paul.  Brazilian culture is having a problem with date formats.  Try using the european format. 
										// 06/13/2006 Paul.  Italian has a problem with the time separator.  Use the value from the culture from CalendarControl.SqlDateTimeFormat. 
										// 06/14/2006 Paul.  The Italian problem was that it was using the culture separator, but DataView only supports the en-US format. 
										string sDAY_START_ServerTime = dtDAY_START_ServerTime.ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat);
										string sDAY_END_ServerTime   = dtDAY_END_ServerTime  .ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat);
										vwMain.RowFilter = "   DATE_START >= #" + sDAY_START_ServerTime + "# and DATE_START <  #" + sDAY_END_ServerTime + "#" + ControlChars.CrLf
										                 + "or DATE_END   >  #" + sDAY_START_ServerTime + "# and DATE_END   <= #" + sDAY_END_ServerTime + "#" + ControlChars.CrLf
										                 + "or DATE_START <  #" + sDAY_START_ServerTime + "# and DATE_END   >  #" + sDAY_END_ServerTime + "#" + ControlChars.CrLf;
#if DEBUG
//										RegisterClientScriptBlock("vwACTIVITIES_List" + dtDAY_START.ToOADate().ToString(), Sql.EscapeJavaScript(vwMain.RowFilter));
#endif
										WeekRow ctlWeekRow = LoadControl("WeekRow.ascx") as WeekRow;
										// 06/09/2006 Paul.  Add to controls list before bindging. 
										plcWeekRows.Controls.Add(ctlWeekRow);
										ctlWeekRow.DATE_START = dtDAY_START;
										//ctlWeekRow.DATE_END   = dtDAY_END;
										ctlWeekRow.DataSource = vwMain;
										// 06/09/2006 Paul.  Need to bind after specifying the data source. 
										ctlWeekRow.DataBind();
										//ctlWeekRow.DATE_START = new DateTime(dtCurrentDate.Year, dtCurrentDate.Month, dtCurrentDate.Day, iHour, 0, 0);
										//ctlWeekRow.DATE_END   = ctlWeekRow.DATE_START.AddHours(1);
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
			try
			{
				CalendarInitDate();
				dtCurrentWeek = dtCurrentDate.AddDays(DayOfWeek.Sunday - dtCurrentDate.DayOfWeek);
				if ( !IsPostBack )
				{
					BindGrid();
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_DataBind(object sender, System.EventArgs e)
		{
			// 03/19/2007 Paul.  We were having a problem with the calendar data appearing during print view.  We needed to rebind the data. 
			try
			{
				if ( IsPostBack )
				{
					CalendarInitDate();
					dtCurrentWeek = dtCurrentDate.AddDays(DayOfWeek.Sunday - dtCurrentDate.DayOfWeek);
					BindGrid();
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
			this.DataBinding += new System.EventHandler(this.Page_DataBind);
			ctlCalendarHeader.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
