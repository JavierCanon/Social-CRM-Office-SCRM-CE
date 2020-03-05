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
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for CalendarControl.
	/// </summary>
	public class CalendarControl : SplendidControl
	{
		public static string SqlDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

		protected DateTime dtCurrentDate  = DateTime.MinValue;

		public static string CalendarQueryString(DateTime dt)
		{
			return "day=" + dt.Day + "&month=" + dt.Month + "&year=" + dt.Year;
		}

		// 09/30/2005 Paul.  Can't initialize in OnInit because ViewState is not ready. 
		// Can't initialize in a Page_Load because it will not get called in the correct sequence. 
		protected void CalendarInitDate()
		{
			if ( !IsPostBack )
			{
				int nYear  = Sql.ToInteger(Request["year" ]);
				int nMonth = Sql.ToInteger(Request["month"]);
				int nDay   = Sql.ToInteger(Request["day"  ]);
				try
				{
					if ( nYear < 1753 || nYear > 9999 || nMonth < 1 || nMonth > 12 || nDay < 1 || nDay > 31 )
						dtCurrentDate = DateTime.Today;
					else
						dtCurrentDate = new DateTime(nYear, nMonth, nDay);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					dtCurrentDate = DateTime.Today;
				}
				// 09/30/2005 Paul.  ViewState is not available in OnInit.  Must wait for the Page_Load event. 
				ViewState["CurrentDate"] = dtCurrentDate;
			}
			else
			{
				dtCurrentDate = Sql.ToDateTime(ViewState["CurrentDate"]);
			}
		}
	}
}
