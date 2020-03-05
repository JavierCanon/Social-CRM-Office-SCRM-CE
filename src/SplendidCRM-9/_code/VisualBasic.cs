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
using System.Text;
using System.Collections;

// 07/31/2006 Paul.  Stop using VisualBasic library to increase compatibility with Mono. 
namespace SplendidCRM
{
	public class ControlChars
	{
		public static string CrLf
		{
			get { return "\r\n"; }
		}

			public static char Cr
		{
			get { return '\r'; }
		}

			public static char Lf
		{
			get { return '\n'; }
		}

			public static char Tab
		{
			get { return '\t'; }
		}
	}

	public enum TriState
	{
		UseDefault = -2,
		True = -1,
		False = 0,
	}
	
	public enum CompareMethod
	{
		Binary = 0,
		Text = 1,
	}

	public class Strings
	{
		public static string Space(int nCount)
		{
			return new string(' ', nCount);
		}

		public static string[] Split(string s, string sDelimiter, int nLimit, CompareMethod Compare)
		{
			ArrayList lst = new ArrayList();
			int nOffset = 0;
			if ( sDelimiter == String.Empty )
				sDelimiter = " ";
			while ( (nOffset = s.IndexOf(sDelimiter)) >= 0 )
			{
				if ( nLimit > 0 && lst.Count == nLimit-1 )
					break;
				lst.Add(s.Substring(0, nOffset));
				s = s.Substring(nOffset + sDelimiter.Length);
			}
			if ( lst.Count == 0 || s.Length > 0 )
				lst.Add(s);
			return lst.ToArray(typeof(System.String)) as string[];
		}

		public static string FormatCurrency(object o, int NumDigitsAfterDecimal, TriState IncludeLeadingDigit, TriState UseParensForNegativeNumbers, TriState GroupDigits)
		{
			// 07/31/2006 Paul.  We will always format with thousands separator and zero decimal places.
			//string sCurrencySymbol = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
			if ( o == null || o is DateTime )
				throw(new Exception("Invalid currency expression"));
			string sValue = String.Format("{0:$#,#}", o);
			return sValue;
		}
	}

	public class Information
	{
		public static bool IsDate(object o)
		{
			if ( o == null )
				return false;
			else if ( o is DateTime )
				return true;
			else if ( o is String )
			{
				try
				{
					DateTime.Parse(o as String);
					return true;
				}
				catch
				{
				}
			}
			return false;
		}

		public static bool IsNumeric(object o)
		{
			if ( o == null || o is DateTime )
				return false;
			else if ( o is Int16 || o is Int32 || o is Int64 || o is Decimal || o is Single || o is Double )
				return true;
			else
			{
				try
				{
					if ( o is String )
						Double.Parse(o as String);
					else
						Double.Parse(o.ToString());
					return true;
				}
				catch
				{
				}
			}
			return false;
		}
	}
}
