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
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for L10n.
	/// </summary>
	public class L10N
	{
		protected string m_sNAME;
		
		public string NAME
		{
			get
			{
				return m_sNAME;
			}
		}

		public L10N(string sNAME)
		{
			// 08/28/2005 Paul.  Default to English if nothing specified. 
			if ( Sql.IsEmptyString(sNAME) )
				sNAME = "en-US";
			// 11/19/2005 Paul.  We may be connecting to MySQL, so the language may have an underscore. 
			m_sNAME = NormalizeCulture(sNAME);
		}

		public bool IsLanguageRTL()
		{
			bool bRTL = false;
			switch ( m_sNAME.Substring(0, 2) )
			{
				case "ar":  bRTL = true;  break;
				case "he":  bRTL = true;  break;
				case "ur":  bRTL = true;  break;
			}
			return bRTL;
		}

		public static string NormalizeCulture(string sCulture)
		{
			return sCulture.Replace("_", "-").ToLower();
		}

		// 08/17/2005 Paul.  Special Term function that helps with a list. 
		public object Term(string sListName, object oField)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sListName) )
				return String.Empty;

			if ( oField == null || oField == DBNull.Value )
				return oField;
			// 11/28/2005 Paul.  Convert field to string instead of cast.  Cast will not work for integer fields. 
			return Term(sListName + oField.ToString());
		}

		public string Term(string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) )
				return String.Empty;

			HttpApplicationState Application = HttpContext.Current.Application;
			//string sNAME = "en-us";
			object oDisplayName = Application[NAME + "." + sEntryName];
			if ( oDisplayName == null )
			{
				// 01/11/2007 Paul.  Default to English if term not found. 
				// There are just too many untranslated terms when importing a SugarCRM Language Pack. 
				oDisplayName = Application["en-US." + sEntryName];
				if ( oDisplayName == null )
				{
					// Prevent parameter out of range errors with <asp:Button AccessKey="" />
					if ( sEntryName.EndsWith("_BUTTON_KEY") )
						return String.Empty;
					return sEntryName;
				}
			}
			return oDisplayName.ToString();
		}

		// 06/30/2007 Paul.  Prevent parameter out of range errors with <asp:Button AccessKey="" />.  Not all access keys end in _BUTTON_KEY. 
		public string AccessKey(string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) )
				return String.Empty;

			HttpApplicationState Application = HttpContext.Current.Application;
			//string sNAME = "en-us";
			object oDisplayName = Application[NAME + "." + sEntryName];
			if ( oDisplayName == null )
			{
				// 01/11/2007 Paul.  Default to English if term not found. 
				// There are just too many untranslated terms when importing a SugarCRM Language Pack. 
				oDisplayName = Application["en-US." + sEntryName];
				if ( oDisplayName == null )
				{
					return String.Empty;
				}
			}
			// 06/30/2007 Paul.  AccessKey too long, cannot be more than one character.
			// 07/03/2007 Paul.  Protect against an empty AccessKey string. 
			string sAccessKey = oDisplayName.ToString();
			if ( sAccessKey.Length == 0 )
				return String.Empty;
			return sAccessKey.Substring(0, 1);
		}

		public string AliasedTerm(string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) )
				return String.Empty;

			HttpApplicationState Application = HttpContext.Current.Application;
			object oAliasedName = Application["ALIAS_" + sEntryName];
			if ( oAliasedName == null )
				return Term(sEntryName);
			return Term(oAliasedName.ToString());
		}

		public static void SetTerm(string sLANG, string sMODULE_NAME, string sNAME, string sDISPLAY_NAME)
		{
			HttpContext.Current.Application[sLANG + "." + sMODULE_NAME + "." + sNAME] = sDISPLAY_NAME;
		}

		public static void SetTerm(string sLANG, string sMODULE_NAME, string sLIST_NAME, string sNAME, string sDISPLAY_NAME)
		{
			// 01/13/2006 Paul.  Don't include MODULE_NAME when used with a list. DropDownLists are populated without the module name in the list name. 
			// 01/13/2006 Paul.  We can remove the module, but not the dot.  Otherwise it breaks all other code that references a list term. 
			sMODULE_NAME = String.Empty;
			HttpContext.Current.Application[sLANG + "." + sMODULE_NAME + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
		}

		public static void SetAlias(string sALIAS_MODULE_NAME, string sALIAS_LIST_NAME, string sALIAS_NAME, string sMODULE_NAME, string sLIST_NAME, string sNAME)
		{
			if ( Sql.IsEmptyString(sALIAS_LIST_NAME) )
				HttpContext.Current.Application["ALIAS_" + sALIAS_MODULE_NAME + "." + sALIAS_NAME] = sMODULE_NAME + "." + sNAME;
			else
				HttpContext.Current.Application["ALIAS_" + sALIAS_MODULE_NAME + "." + sALIAS_LIST_NAME + "." + sALIAS_NAME] = sMODULE_NAME + "." + sLIST_NAME + "." + sNAME;
		}
		
		public string TermJavaScript(string sEntryName)
		{
			string sDisplayName = Term(sEntryName);
			sDisplayName = sDisplayName.Replace("\'", "\\\'");
			sDisplayName = sDisplayName.Replace("\"", "\\\"");
			sDisplayName = sDisplayName.Replace(ControlChars.CrLf, @"\r\n");
			return sDisplayName;
		}
	}
}
