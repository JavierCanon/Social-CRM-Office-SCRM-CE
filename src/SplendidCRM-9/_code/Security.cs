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
using System.IO;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Data.Common;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for Security.
	/// </summary>
	public class Security
	{
		public static Guid USER_ID
		{
			get
			{
				// 02/17/2006 Paul.  Throw an exception if Session is null.  This is more descriptive error than "object is null". 
				// We will most likely see this in a SOAP call. 
				// 01/13/2008 Paul.  Return an empty guid if the session does not exist. 
				// This will allow us to reuse lots of SqlProcs code in the scheduler. 
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					return Guid.Empty;
				return  Sql.ToGuid(HttpContext.Current.Session["USER_ID"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["USER_ID"] = value;
			}
		}
		
		public static string USER_NAME
		{
			get
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				return  Sql.ToString(HttpContext.Current.Session["USER_NAME"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["USER_NAME"] = value;
			}
		}
		
		public static string FULL_NAME
		{
			get
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				return  Sql.ToString(HttpContext.Current.Session["FULL_NAME"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["FULL_NAME"] = value;
			}
		}
		
		public static bool IS_ADMIN
		{
			get
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				return Sql.ToBoolean(HttpContext.Current.Session["IS_ADMIN"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["IS_ADMIN"] = value;
			}
		}
		
		public static bool PORTAL_ONLY
		{
			get
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				return Sql.ToBoolean(HttpContext.Current.Session["PORTAL_ONLY"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["PORTAL_ONLY"] = value;
			}
		}
		
		// 11/25/2006 Paul.  Default TEAM_ID. 
		public static Guid TEAM_ID
		{
			get
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				return  Sql.ToGuid(HttpContext.Current.Session["TEAM_ID"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["TEAM_ID"] = value;
			}
		}
		
		public static string TEAM_NAME
		{
			get
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				return  Sql.ToString(HttpContext.Current.Session["TEAM_NAME"]);
			}
			set
			{
				if ( HttpContext.Current == null || HttpContext.Current.Session == null )
					throw(new Exception("HttpContext.Current.Session is null"));
				HttpContext.Current.Session["TEAM_NAME"] = value;
			}
		}
		
		public static bool IsWindowsAuthentication()
		{
			// 11/19/2005 Paul.  AUTH_USER is the clear indication that NTLM is enabled. 
			string sAUTH_USER = Sql.ToString(HttpContext.Current.Request.ServerVariables["AUTH_USER"]);
#if DEBUG
//			sAUTH_USER = String.Empty;
#endif
			// 02/28/2007 Paul.  In order to enable WebParts, we need to set HttpContext.Current.User.Identity. 
			// Doing so will change AUTH_USER, so exclude if AUTH_USER == USER_NAME. 
			// When Windows Authentication is used, AUTH_USER will include the windows domain. 
			return !Sql.IsEmptyString(sAUTH_USER) && sAUTH_USER != USER_NAME;
		}

		public static bool IsAuthenticated()
		{
			return !Sql.IsEmptyGuid(Security.USER_ID);
		}

		// 02/28/2007 Paul.  Centralize session reset to prepare for WebParts. 
		public static void Clear()
		{
			HttpContext.Current.Session.Clear();
		}

		// 11/18/2005 Paul.  SugarCRM stores an MD5 hash of the password. 
		// 11/18/2005 Paul.  SugarCRM also stores the password using the PHP Crypt() function, which is DES. 
		// Don't bother trying to duplicate the PHP Crypt() function because the result is not used in SugarCRM.  
		// The PHP function is located in D:\php-5.0.5\win32\crypt_win32.c
		public static string HashPassword(string sPASSWORD)
		{
			UTF8Encoding utf8 = new UTF8Encoding();
			byte[] aby = utf8.GetBytes(sPASSWORD);
			
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] binMD5 = md5.ComputeHash(aby);
			return Sql.HexEncode(binMD5);
		}

		// 01/08/2008 Paul.  Use the same encryption used in the SplendidCRM Plug-in for Outlook, except we will base64 encode. 
		// 01/09/2008 Paul.  Increase quality of encryption by using an robust IV.
		// 01/09/2008 Paul.  Use Rijndael instead of TripleDES because it allows 128 block and key sizes, so Guids can be used for both. 
		public static string EncryptPassword(string sPASSWORD, Guid gKEY, Guid gIV)
		{
			UTF8Encoding utf8 = new UTF8Encoding(false);

			string sResult = null;
			byte[] byPassword = utf8.GetBytes(sPASSWORD);
			using ( MemoryStream stm = new MemoryStream() )
			{
				Rijndael rij = Rijndael.Create();
				rij.Key = gKEY.ToByteArray();
				rij.IV  = gIV.ToByteArray();
				using ( CryptoStream cs = new CryptoStream(stm, rij.CreateEncryptor(), CryptoStreamMode.Write) )
				{
					cs.Write(byPassword, 0, byPassword.Length);
					cs.FlushFinalBlock();
					cs.Close();
				}
				sResult = Convert.ToBase64String(stm.ToArray());
			}
			return sResult;
		}

		public static string DecryptPassword(string sPASSWORD, Guid gKEY, Guid gIV)
		{
			UTF8Encoding utf8 = new UTF8Encoding(false);

			string sResult = null;
			byte[] byPassword = Convert.FromBase64String(sPASSWORD);
			using ( MemoryStream stm = new MemoryStream() )
			{
				Rijndael rij = Rijndael.Create();
				rij.Key = gKEY.ToByteArray();
				rij.IV  = gIV.ToByteArray();
				using ( CryptoStream cs = new CryptoStream(stm, rij.CreateDecryptor(), CryptoStreamMode.Write) )
				{
					cs.Write(byPassword, 0, byPassword.Length);
					cs.Flush();
					cs.Close();
				}
				byte[] byResult = stm.ToArray();
				sResult = utf8.GetString(byResult, 0, byResult.Length);
			}
			return sResult;
		}

		public static void SetModuleAccess(string sMODULE_NAME, string sACCESS_TYPE, int nACLACCESS)
		{
			if ( HttpContext.Current.Application == null )
				throw(new Exception("HttpContext.Current.Application is null"));
			// 06/04/2006 Paul.  Verify that sMODULE_NAME is not empty.  
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			HttpContext.Current.Application["ACLACCESS_" + sMODULE_NAME + "_" + sACCESS_TYPE] = nACLACCESS;
		}

		public static void SetUserAccess(string sMODULE_NAME, string sACCESS_TYPE, int nACLACCESS)
		{
			if ( HttpContext.Current == null || HttpContext.Current.Session == null )
				throw(new Exception("HttpContext.Current.Session is null"));
			// 06/04/2006 Paul.  Verify that sMODULE_NAME is not empty.  
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			HttpContext.Current.Session["ACLACCESS_" + sMODULE_NAME + "_" + sACCESS_TYPE] = nACLACCESS;
		}

		public static int GetUserAccess(string sMODULE_NAME, string sACCESS_TYPE)
		{
			if ( HttpContext.Current == null || HttpContext.Current.Session == null )
				throw(new Exception("HttpContext.Current.Session is null"));
			// 06/04/2006 Paul.  Verify that sMODULE_NAME is not empty.  
			if ( Sql.IsEmptyString(sMODULE_NAME) )
				throw(new Exception("sMODULE_NAME should not be empty."));
			// 04/27/2006 Paul.  Admins have full access to the site, no matter what the role. 
			if ( IS_ADMIN )
				return 100;

			// 12/05/2006 Paul.  We need to combine Activity and Calendar related modules into a single access value. 
			int nACLACCESS = 0;
			if ( sMODULE_NAME == "Calendar" )
			{
				// 12/05/2006 Paul.  The Calendar related views only combine Calls and Meetings. 
				int nACLACCESS_Calls    = GetUserAccess("Calls"   , sACCESS_TYPE);
				int nACLACCESS_Meetings = GetUserAccess("Meetings", sACCESS_TYPE);
				// 12/05/2006 Paul. Use the max value so that the Activities will be displayed if either are accessible. 
				nACLACCESS = Math.Max(nACLACCESS_Calls, nACLACCESS_Meetings);
			}
			else if ( sMODULE_NAME == "Activities" )
			{
				// 12/05/2006 Paul.  The Activities combines Calls, Meetings, Tasks, Notes and Emails. 
				int nACLACCESS_Calls    = GetUserAccess("Calls"   , sACCESS_TYPE);
				int nACLACCESS_Meetings = GetUserAccess("Meetings", sACCESS_TYPE);
				int nACLACCESS_Tasks    = GetUserAccess("Tasks"   , sACCESS_TYPE);
				int nACLACCESS_Notes    = GetUserAccess("Notes"   , sACCESS_TYPE);
				int nACLACCESS_Emails   = GetUserAccess("Emails"  , sACCESS_TYPE);
				nACLACCESS = nACLACCESS_Calls;
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Meetings);
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Tasks   );
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Notes   );
				nACLACCESS = Math.Max(nACLACCESS, nACLACCESS_Emails  );
			}
			else
			{
				string sAclKey = "ACLACCESS_" + sMODULE_NAME + "_" + sACCESS_TYPE;
				// 04/27/2006 Paul.  If no specific level is provided, then look to the Module level. 
				if ( HttpContext.Current.Session[sAclKey] == null )
					nACLACCESS = Sql.ToInteger(HttpContext.Current.Application[sAclKey]);
				else
					nACLACCESS = Sql.ToInteger(HttpContext.Current.Session[sAclKey]);
				if ( sACCESS_TYPE != "access" && nACLACCESS >= 0 )
				{
					// 04/27/2006 Paul.  The access type can over-ride any other type. 
					// A simple trick is to take the minimum of the two values.  
					// If either value is denied, then the result will be negative. 
					sAclKey = "ACLACCESS_" + sMODULE_NAME + "_access";
					int nAccessLevel = 0;
					if ( HttpContext.Current.Session[sAclKey] == null )
						nAccessLevel = Sql.ToInteger(HttpContext.Current.Application[sAclKey]);
					else
						nAccessLevel = Sql.ToInteger(HttpContext.Current.Session[sAclKey]);
					if ( nAccessLevel < 0 )
						nACLACCESS = nAccessLevel;
				}
			}
			return nACLACCESS;
		}
		
		// 06/05/2007 Paul.  We need an easy way to determine when to allow editing or deleting in sub-panels. 
		// If the record is not assigned to any specific user, then it is accessible by everyone. 
		public static int GetUserAccess(string sMODULE_NAME, string sACCESS_TYPE, Guid gASSIGNED_USER_ID)
		{
			int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sACCESS_TYPE);
			if ( nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID != gASSIGNED_USER_ID && gASSIGNED_USER_ID != Guid.Empty)
			{
				nACLACCESS = ACL_ACCESS.NONE;
			}
			return nACLACCESS;
		}
		
		public static void Filter(IDbCommand cmd, string sMODULE_NAME, string sACCESS_TYPE)
		{
			Filter(cmd, sMODULE_NAME, sACCESS_TYPE, "ASSIGNED_USER_ID");
		}
		
		public static void Filter(IDbCommand cmd, string sMODULE_NAME, string sACCESS_TYPE, string sASSIGNED_USER_ID_Field)
		{
			// 08/04/2007 Paul.  Always wait forever for the data.  No sense in showing a timeout.
			cmd.CommandTimeout = 0;
			// 12/07/2006 Paul.  Not all views use ASSIGNED_USER_ID as the assigned field.  Allow an override. 
			// 11/25/2006 Paul.  Administrators should not be restricted from seeing items because of the team rights.
			// This is so that an administrator can fix any record with a bad team value. 
			// 12/30/2007 Paul.  We need a dynamic way to determine if the module record can be assigned or placed in a team. 
			// Teamed and Assigned flags are automatically determined based on the existence of TEAM_ID and ASSIGNED_USER_ID fields. 
			bool bModuleIsTeamed        = Sql.ToBoolean(HttpContext.Current.Application["Modules." + sMODULE_NAME + ".Teamed"  ]);
			bool bModuleIsAssigned      = Sql.ToBoolean(HttpContext.Current.Application["Modules." + sMODULE_NAME + ".Assigned"]);
			bool bEnableTeamManagement  = Crm.Config.enable_team_management();
			bool bRequireTeamManagement = Crm.Config.require_team_management();
			bool bRequireUserAssignment = Crm.Config.require_user_assignment();
			if ( bModuleIsTeamed )
			{
				if ( IS_ADMIN )
					bRequireTeamManagement = false;

				if ( bEnableTeamManagement )
				{
					if ( bRequireTeamManagement )
						cmd.CommandText += "       inner join vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
					else
						cmd.CommandText += "  left outer join vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
					cmd.CommandText += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID" + ControlChars.CrLf;
					cmd.CommandText += "              and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
					Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
				}
			}
			cmd.CommandText += " where 1 = 1" + ControlChars.CrLf;
			if ( bModuleIsTeamed )
			{
				if ( bEnableTeamManagement && !bRequireTeamManagement && !IS_ADMIN )
					cmd.CommandText += "   and (TEAM_ID is null or vwTEAM_MEMBERSHIPS.MEMBERSHIP_ID is not null)" + ControlChars.CrLf;
			}
			if ( bModuleIsAssigned )
			{
				int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sACCESS_TYPE);
				// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
				// 01/02/2008 Paul.  Make sure owner rule does not apply to admins. 
				if ( nACLACCESS == ACL_ACCESS.OWNER || (bRequireUserAssignment && !IS_ADMIN) )
				{
					string sFieldPlaceholder = Sql.NextPlaceholder(cmd, sASSIGNED_USER_ID_Field);
					// 01/22/2007 Paul.  If ASSIGNED_USER_ID is null, then let everybody see it. 
					// This was added to work around a bug whereby the ASSIGNED_USER_ID was not automatically assigned to the creating user. 
					bool bShowUnassigned = Crm.Config.show_unassigned();
					if ( bShowUnassigned )
					{
						if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
							cmd.CommandText += "   and (" + sASSIGNED_USER_ID_Field + " is null or upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + "))" + ControlChars.CrLf;
						else
							cmd.CommandText += "   and (" + sASSIGNED_USER_ID_Field + " is null or "       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder + ")"  + ControlChars.CrLf;
					}
					else
					{
						if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
							cmd.CommandText += "   and upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + ")" + ControlChars.CrLf;
						else
							cmd.CommandText += "   and "       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder       + ControlChars.CrLf;
					}
					Sql.AddParameter(cmd, "@" + sFieldPlaceholder, Security.USER_ID);
				}
			}
		}
	}
}

