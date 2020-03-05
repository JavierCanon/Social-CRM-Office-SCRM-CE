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
using System.Web;
/*
using System.Web.SessionState;
using System.Text;
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;
*/

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidDefaults.
	/// </summary>
	public class SplendidDefaults
	{
		// 12/22/2007 Paul.  Inside the timer event, there is no current context, so we need to pass the application. 
		public static string Culture(HttpApplicationState Application)
		{
			string sCulture = Sql.ToString(Application["CONFIG.default_language"]);
			// 12/22/2007 Paul.  The cache is not available when we are inside the timer event. 
			if ( HttpContext.Current != null && HttpContext.Current.Cache != null )
			{
				DataView vwLanguages = new DataView(SplendidCache.Languages());
				vwLanguages.RowFilter = "NAME = '" + sCulture +"'";
				if ( vwLanguages.Count > 0 )
					sCulture = Sql.ToString(vwLanguages[0]["NAME"]);
			}
			if ( Sql.IsEmptyString(sCulture) )
				sCulture = "en-US";
			return L10N.NormalizeCulture(sCulture);
		}

		public static string Culture()
		{
			return Culture(HttpContext.Current.Application);
		}

		public static string Theme()
		{
			string sTheme = Sql.ToString(HttpContext.Current.Application["CONFIG.default_theme"]);
			if ( Sql.IsEmptyString(sTheme) )
				sTheme = "Sugar";
			return sTheme;
		}

		public static string MobileTheme()
		{
			string sTheme = Sql.ToString(HttpContext.Current.Application["CONFIG.default_mobile_theme"]);
			if ( Sql.IsEmptyString(sTheme) )
				sTheme = "Mobile";
			return sTheme;
		}

		public static string DateFormat()
		{
			string sDateFormat = Sql.ToString(HttpContext.Current.Application["CONFIG.default_date_format"]);
			if ( Sql.IsEmptyString(sDateFormat) )
				sDateFormat = "MM/dd/yyyy";
			// 11/28/2005 Paul.  Need to make sure that the default format is valid. 
			else if ( !IsValidDateFormat(sDateFormat) )
				sDateFormat = DateFormat(sDateFormat);
			return sDateFormat;
		}

		public static bool IsValidDateFormat(string sDateFormat)
		{
			if ( sDateFormat.IndexOf("m") >= 0 || sDateFormat.IndexOf("yyyy") < 0 )
				return false;
			return true;
		}

		public static string DateFormat(string sDateFormat)
		{
			// 11/12/2005 Paul.  "m" is not valid for .NET month formatting.  Must use MM. 
			if ( sDateFormat.IndexOf("m") >= 0 )
			{
				sDateFormat = sDateFormat.Replace("m", "M");
			}
			// 11/12/2005 Paul.  Require 4 digit year.  Otherwise default date in Pipeline of 12/31/2100 would get converted to 12/31/00. 
			if ( sDateFormat.IndexOf("yyyy") < 0 )
			{
				sDateFormat = sDateFormat.Replace("yy", "yyyy");
			}
			return sDateFormat;
		}

		public static string TimeFormat()
		{
			string sTimeFormat = Sql.ToString(HttpContext.Current.Application["CONFIG.default_time_format"]);
			if ( Sql.IsEmptyString(sTimeFormat) || sTimeFormat == "H:i" )
				sTimeFormat = "h:mm tt";
			return sTimeFormat;
		}

		public static string TimeZone()
		{
			// 08/08/2006 Paul.  Pull the default timezone and fall-back to Eastern US only if empty. 
			string sDEFAULT_TIMEZONE = Sql.ToString(HttpContext.Current.Application["CONFIG.default_timezone"]);
			if ( Sql.IsEmptyGuid(sDEFAULT_TIMEZONE) )
				sDEFAULT_TIMEZONE = "BFA61AF7-26ED-4020-A0C1-39A15E4E9E0A";
			return sDEFAULT_TIMEZONE;
		}

		public static string TimeZone(int nTimez)
		{
			string sTimeZone = String.Empty;
			DataView vwTimezones = new DataView(SplendidCache.Timezones());
			vwTimezones.RowFilter = "BIAS = " + nTimez.ToString();
			if ( vwTimezones.Count > 0 )
				sTimeZone = Sql.ToString(vwTimezones[0]["ID"]);
			else
				sTimeZone = TimeZone();
			return sTimeZone;
		}

		public static string CurrencyID()
		{
			// 08/08/2006 Paul.  Pull the default currency and fall-back to Dollars only if empty. 
			string sDEFAULT_CURRENCY = Sql.ToString(HttpContext.Current.Application["CONFIG.default_currency"]);
			if ( Sql.IsEmptyGuid(sDEFAULT_CURRENCY) )
			{
				sDEFAULT_CURRENCY = "E340202E-6291-4071-B327-A34CB4DF239B";
			}
			return sDEFAULT_CURRENCY;
		}

		public static string GroupSeparator()
		{
			string sGROUP_SEPARATOR = Sql.ToString(HttpContext.Current.Application["CONFIG.default_number_grouping_seperator"]);
			if ( Sql.IsEmptyString(sGROUP_SEPARATOR) )
				sGROUP_SEPARATOR  = ",";
			return sGROUP_SEPARATOR;
		}

		public static string DecimalSeparator()
		{
			string sDECIMAL_SEPARATOR = Sql.ToString(HttpContext.Current.Application["CONFIG.default_decimal_seperator"]);
			if ( Sql.IsEmptyString(sDECIMAL_SEPARATOR) )
				sDECIMAL_SEPARATOR = ".";
			return sDECIMAL_SEPARATOR;
		}

		public static string generate_graphcolor(string sInput, int nInstance)
		{
			string sColor = String.Empty;
			if ( nInstance < 20 )
			{
				string[] arrGraphColor =
				{
					"0xFF0000"
					, "0x00FF00"
					, "0x0000FF"
					, "0xFF6600"
					, "0x42FF8E"
					, "0x6600FF"
					, "0xFFFF00"
					, "0x00FFFF"
					, "0xFF00FF"
					, "0x66FF00"
					, "0x0066FF"
					, "0xFF0066"
					, "0xCC0000"
					, "0x00CC00"
					, "0x0000CC"
					, "0xCC6600"
					, "0x00CC66"
					, "0x6600CC"
					, "0xCCCC00"
					, "0x00CCCC"
				};
				sColor = arrGraphColor[nInstance];
			}
			else
			{
				sColor = "0x00CCCC";
				//sColor = "0x" + substr(md5(sInput), 0, 6);
			}
			return sColor;
		}

	}
}
