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

namespace SplendidCRM.Crm
{
	public class Users
	{
		public static string USER_NAME(Guid gID)
		{
			string sUSER_NAME = String.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select USER_NAME" + ControlChars.CrLf
				     + "  from vwUSERS  " + ControlChars.CrLf
				     + " where ID = @ID " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gID);
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							sUSER_NAME = Sql.ToString(rdr["USER_NAME"]);
						}
					}
				}
			}
			return sUSER_NAME;
		}
	}

	public class Modules
	{
		// 12/22/2007 Paul.  Inside the timer event, there is no current context, so we need to pass the application. 
		public static DataTable Parent(HttpApplicationState Application, string sPARENT_TYPE, Guid gPARENT_ID)
		{
			DataTable dt = new DataTable();
			string sTABLE_NAME = Sql.ToString(Application["Modules." + sPARENT_TYPE + ".TableName"]);
			if ( !Sql.IsEmptyString(sTABLE_NAME) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select *"                + ControlChars.CrLf
					     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
					     + " where ID = @ID"         + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gPARENT_ID);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
			}
			return dt;
		}

		public static DataTable Parent(string sPARENT_TYPE, Guid gPARENT_ID)
		{
			return Parent(HttpContext.Current.Application, sPARENT_TYPE, gPARENT_ID);
		}
	}

	public class Config
	{
		public static bool enable_team_management()
		{
			return Sql.ToBoolean(HttpContext.Current.Application["CONFIG.enable_team_management"]);
		}
		public static bool require_team_management()
		{
			return Sql.ToBoolean(HttpContext.Current.Application["CONFIG.require_team_management"]);
		}
		// 01/01/2008 Paul.  We need a quick way to require user assignments across the system. 
		public static bool require_user_assignment()
		{
			return Sql.ToBoolean(HttpContext.Current.Application["CONFIG.require_user_assignment"]);
		}
		public static bool show_unassigned()
		{
			// 01/22/2007 Paul.  If ASSIGNED_USER_ID is null, then let everybody see it. 
			// This was added to work around a bug whereby the ASSIGNED_USER_ID was not automatically assigned to the creating user. 
			return Sql.ToBoolean(HttpContext.Current.Application["CONFIG.show_unassigned"]);
		}
		public static string inbound_email_case_subject_macro()
		{
			string sMacro = Sql.ToString(HttpContext.Current.Application["CONFIG.inbound_email_case_subject_macro"]);
			if ( Sql.IsEmptyString(sMacro) )
				sMacro = "[CASE:%1]";
			return sMacro;
		}
	}
}
