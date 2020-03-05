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
	///		Summary description for MyCalendar.
	/// </summary>
	public class MyCalendar : SplendidControl
	{
		protected System.Web.UI.WebControls.Calendar ctlCalendar;
		protected string        sDetailView    ;

		public string DetailView
		{
			get { return sDetailView; }
			set { sDetailView = value; }
		}

		protected void ctlCalendar_SelectionChanged(Object sender, EventArgs e) 
		{
			// 08/31/2006 Paul.  The date needs to be separated into day, month, year fields to avoid localization issues. 
			Response.Redirect("~/Calendar/default.aspx?" + CalendarControl.CalendarQueryString(ctlCalendar.SelectedDate));
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 09/09/2007 Paul.  We are having trouble dynamically adding user controls to the WebPartZone. 
			// Instead, control visibility manually here.  This approach as the added benefit of hiding the 
			// control even if the WebPartManager has moved it to an alternate zone. 
			if ( this.Visible && this.Visible && !Sql.IsEmptyString(sDetailView) )
			{
				// 01/17/2008 Paul.  We need to use the sDetailView property and not the hard-coded view name. 
				DataView vwFields = new DataView(SplendidCache.DetailViewRelationships(sDetailView));
				vwFields.RowFilter = "CONTROL_NAME = '~/Calendar/MyCalendar'";
				this.Visible = vwFields.Count > 0;
			}
			if ( !this.Visible )
				return;

			ctlCalendar.NextPrevFormat = NextPrevFormat.CustomText ;
			ctlCalendar.PrevMonthText  = "<div class=\"monthFooterPrev\"><img src=\"" + Session["themeURL"] + "images/calendar_previous.gif\" width=\"6\" height=\"9\" alt=\"" + L10n.Term("Calendar.LBL_PREVIOUS_MONTH") + "\" align=\"absmiddle\" border=\"0\">&nbsp;&nbsp;" + L10n.Term("Calendar.LBL_PREVIOUS_MONTH").Replace(" ", "&nbsp;") + "</div>";
			ctlCalendar.NextMonthText  = "<div class=\"monthFooterNext\">" + L10n.Term("Calendar.LBL_NEXT_MONTH").Replace(" ", "&nbsp;") + "&nbsp;&nbsp;<img src=\"" + Session["themeURL"] + "images/calendar_next.gif\" width=\"6\" height=\"9\" alt=\"" + L10n.Term("Calendar.LBL_NEXT_MONTH") + "\" align=\"absmiddle\" border=\"0\"></div>";
			if ( Information.IsDate(Request["Date"]) )
			{
				ctlCalendar.VisibleDate  = Sql.ToDateTime(Request["Date"]);
				ctlCalendar.SelectedDate = Sql.ToDateTime(Request["Date"]);
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
