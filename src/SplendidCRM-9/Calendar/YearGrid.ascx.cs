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

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for YearGrid.
	/// </summary>
	public class YearGrid : CalendarControl
	{
		protected Label          lblError         ;
		protected HtmlTable      tblDailyCalTable ;
		protected CalendarHeader ctlCalendarHeader;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Year.Previous":
					{
						dtCurrentDate = dtCurrentDate.AddYears(-1);
						ViewState["CurrentDate"] = dtCurrentDate;
						break;
					}
					case "Year.Next":
					{
						dtCurrentDate = dtCurrentDate.AddYears(1);
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
						Response.Redirect("Week.aspx?" + CalendarQueryString(dtCurrentDate));
						break;
					}
					case "Month.Current":
					{
						Response.Redirect("Month.aspx?" + CalendarQueryString(dtCurrentDate));
						break;
					}
					case "Year.Current":
					{
						ViewState["CurrentDate"] = dtCurrentDate;
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

		protected void ctlCalendar_SelectionChanged(Object sender, EventArgs e) 
		{
			System.Web.UI.WebControls.Calendar ctlCalendar = sender as System.Web.UI.WebControls.Calendar;
			Response.Redirect("~/Calendar/default.aspx?" + CalendarQueryString(ctlCalendar.SelectedDate));
			//BindGrid();
		}

		/*
				<asp:Calendar ID="ctlCalendar" Width="100%" CssClass="monthBox" ShowGridLines="true" 
					CalendarSelectionMode="DayWeek" OnSelectionChanged="ctlCalendar_SelectionChanged" OnDayRender="ctlCalendar_DayRender" 
					Runat="server">
					<TitleStyle         CssClass="monthHeader monthHeaderH3"   />
					<NextPrevStyle      CssClass="monthHeader monthFooterPrev" />
					<DayHeaderStyle     CssClass="monthCalBodyTHDay"           />
					<DayStyle           CssClass="monthCalBodyWeekDay monthCalBodyWeekDayDateLink"      VerticalAlign="Top" />
					<TodayDayStyle      CssClass="monthCalBodyTodayWeekDay monthCalBodyWeekDayDateLink" VerticalAlign="Top" />
					<WeekendDayStyle    CssClass="monthCalBodyWeekEnd monthCalBodyWeekDayDateLink"      VerticalAlign="Top" />
					<OtherMonthDayStyle CssClass="monthCalBodyWeekDay" ForeColor="#fafafa"              VerticalAlign="Top" />
				</asp:Calendar>
		*/
		protected void BindGrid()
		{
			try
			{
				tblDailyCalTable.Rows.Clear();
				for(int nQuarter = 0; nQuarter < 4; nQuarter++)
				{
					HtmlTableRow tr = new HtmlTableRow();
					tblDailyCalTable.Rows.Add(tr);
					for(int nQMonth = 1; nQMonth <= 3; nQMonth++)
					{
						HtmlTableCell td = new HtmlTableCell();
						tr.Cells.Add(td);
						td.VAlign = "top";
						td.Align  = "center";
						td.Attributes.Add("class", "yearCalBodyMonth");

						DateTime dtCurrentMonth = new DateTime(dtCurrentDate.Year, 3 * nQuarter + nQMonth, 1);
						try
						{
							// 09/30/2005 Paul.  Attempt to keep the day, but prevent a date overflow. 
							if ( dtCurrentDate.Day <= dtCurrentMonth.AddMonths(1).AddDays(-1).Day )
								dtCurrentMonth = dtCurrentMonth.AddDays(dtCurrentDate.Day-1);
							else
								dtCurrentMonth = dtCurrentMonth.AddMonths(1).AddDays(-1);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						HyperLink lnkMonth = new HyperLink();
						td.Controls.Add(lnkMonth);
						lnkMonth.CssClass    = "yearCalBodyMonthLink";
						lnkMonth.Text        = dtCurrentMonth.ToString("MMMM");
						lnkMonth.NavigateUrl = "Month.aspx?" + CalendarQueryString(dtCurrentMonth);

						System.Web.UI.WebControls.Calendar cal = new System.Web.UI.WebControls.Calendar();
						td.Controls.Add(cal);
						cal.VisibleDate = new DateTime(dtCurrentDate.Year, 3 * nQuarter + nQMonth, 1);
						cal.Width                        = new Unit(100, UnitType.Percentage);
						cal.CssClass                     = "monthBox";
						cal.ShowGridLines                = true;
						cal.ShowTitle                    = false;
						cal.ShowNextPrevMonth            = false;
						cal.SelectionMode                = CalendarSelectionMode.Day;
						cal.TitleStyle.CssClass          = "monthHeader monthHeaderH3";
						cal.DayHeaderStyle.CssClass      = "monthCalBodyTHDay";
						cal.DayStyle.CssClass            = "monthCalBodyWeekDay monthCalBodyWeekDayDateLink";
						cal.TodayDayStyle.CssClass       = "monthCalBodyTodayWeekDay monthCalBodyWeekDayDateLink";
						cal.WeekendDayStyle.CssClass     = "monthCalBodyWeekEnd monthCalBodyWeekDayDateLink";
						cal.OtherMonthDayStyle.CssClass  = "monthCalBodyWeekDay";
						cal.OtherMonthDayStyle.ForeColor = System.Drawing.Color.FromArgb(0xfa, 0xfa, 0xfa);//"#fafafa";
						cal.SelectionChanged += new EventHandler(ctlCalendar_SelectionChanged);
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
				if ( !IsPostBack )
				{
					BindGrid();
				}
				else
				{
					// 09/30/2005 Paul. Need to rebind in order for the calendar event to fire. 
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
			ctlCalendarHeader.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
