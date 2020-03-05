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
using System.Data.SqlClient;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for Procedures.
	/// </summary>
	public class Procedures : System.Web.UI.Page
	{
		private string TabSpace(int nNumber)
		{
			return Strings.Space(nNumber).Replace(' ', '\t');
		}

		private void BuildWrapper(ref StringBuilder sb, string sProcedureName, ref DataRowCollection colRows, bool bCreateCommand, bool bTransaction)
		{
			int nColumnAlignmentSize = 5;
			int nSpace=0;
			string sPrimaryKey     = String.Empty;
			int    nPrimaryDefault = 0;
			if ( colRows.Count > 0 )
			{
				sPrimaryKey     = colRows[0]["ColumnName"].ToString();
				nPrimaryDefault = (int)colRows[0]["cdefault"];
			}
			for ( int j = 0 ; j < colRows.Count; j++ )
			{
				DataRow row = colRows[j];
				string sName = row["ColumnName"].ToString();
				if ( sName.Length >= nColumnAlignmentSize )
					nColumnAlignmentSize = sName.Length + 1;
			}
			int k = 0;
			int nIndent = 2;
			if ( bCreateCommand )
			{
				sb.Append(TabSpace(nIndent) + "#region cmd" + (sProcedureName.StartsWith("sp") ? sProcedureName.Substring(2) : sProcedureName) + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "/// <summary>" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "/// " + sProcedureName + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "/// </summary>" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "public static IDbCommand cmd" + (sProcedureName.StartsWith("sp") ? sProcedureName.Substring(2) : sProcedureName) + "(");
				sb.Append("IDbConnection con");
				k++;
			}
			else
			{
				sb.Append(TabSpace(nIndent) + "#region " + sProcedureName + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "/// <summary>" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "/// " + sProcedureName + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "/// </summary>" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "public static void " + sProcedureName + "(");
				for ( int j = 0; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName     = Sql.ToString (row["ColumnName"]);
					string sCsType   = Sql.ToString (row["CsType"    ]);
					string sCsPrefix = Sql.ToString (row["CsPrefix"  ]);
					int    bIsOutput = Sql.ToInteger(row["isoutparam"]);
					string sBareName = sName.Replace("@", "");
					// 06/23/2005 Paul.  Modified User ID is automatic. 
					if ( sBareName == "MODIFIED_USER_ID" )
						continue;
					if ( k > 0 )
						sb.Append(", ");
					if ( bIsOutput == 1 )
						sb.Append("ref ");
					// 01/24/2006 Paul.  A severe error occurred on the current command. The results, if any, should be discarded. 
					// MS03-031 security patch causes this error because of stricter datatype processing.  
					// http://www.microsoft.com/technet/security/bulletin/MS03-031.mspx.
					// http://support.microsoft.com/kb/827366/
					sCsType = (sCsType == "ansistring") ? "string" : sCsType;
					sb.Append(sCsType + " " + sCsPrefix + sBareName);
					k++;
				}
				if ( bTransaction )
				{
					if ( colRows.Count > 1 )
						sb.Append(", ");
					else if ( colRows.Count == 1 )
					{
						// 11/19/2006 Paul.  Skip first parameter if MODIFIED_USER_ID. 
						if ( Sql.ToString (colRows[0]["ColumnName"]) != "@MODIFIED_USER_ID" )
							sb.Append(", ");
					}
					sb.Append("IDbTransaction trn");
				}
			}
			sb.Append(")" + ControlChars.CrLf);
			sb.Append(TabSpace(nIndent) + "{" + ControlChars.CrLf);
			nIndent++;
			if ( !bCreateCommand )
			{
				if ( bTransaction )
				{
					sb.Append(TabSpace(nIndent) + "IDbConnection con = trn.Connection;" + ControlChars.CrLf);
				}
				else
				{
					sb.Append(TabSpace(nIndent) + "DbProviderFactory dbf = DbProviderFactories.GetFactory();" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "using ( IDbConnection con = dbf.CreateConnection() )" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "{" + ControlChars.CrLf);
					nIndent++;
				}
				// 05/01/2006 Paul.  All commands now use a transaction.  This is because Oracle does not have a transaction hierarchy. 
				// So any COMMIT in a procedure, will commit the entire transaction.
				// We want the web application to be in control of the transaction.
				if ( !bTransaction )
				{
					sb.Append(TabSpace(nIndent) + "con.Open();" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "using ( IDbTransaction trn = con.BeginTransaction() )" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "{" + ControlChars.CrLf);
					nIndent++;
					sb.Append(TabSpace(nIndent) + "try" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "{" + ControlChars.CrLf);
					nIndent++;
				}
				sb.Append(TabSpace(nIndent) + "using ( IDbCommand cmd = con.CreateCommand() )" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "{" + ControlChars.CrLf);
				nIndent++;
				// 05/01/2006 Paul.  All commands now use a transaction. 
				sb.Append(TabSpace(nIndent) + "cmd.Transaction = trn;" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "cmd.CommandType = CommandType.StoredProcedure;" + ControlChars.CrLf);
				// 08/14/2005 Paul.  Truncate procedure names on a case-by-case basis. 
				// Oracle only supports identifiers up to 30 characters. 
				if ( sProcedureName.Length > 30 )
				{
					sb.Append(TabSpace(nIndent) + "if ( Sql.IsOracle(cmd) )" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName.Substring(0, 30) + "\";" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "else" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName + "\";" + ControlChars.CrLf);
				}
				else
				{
					sb.Append(TabSpace(nIndent) + "cmd.CommandText = \"" + sProcedureName + "\";" + ControlChars.CrLf);
				}
			}
			else
			{
				sb.Append(TabSpace(nIndent) + "IDbCommand cmd = con.CreateCommand();" + ControlChars.CrLf);
				sb.Append(TabSpace(nIndent) + "cmd.CommandType = CommandType.StoredProcedure;" + ControlChars.CrLf);
				// 08/14/2005 Paul.  Truncate procedure names on a case-by-case basis. 
				// Oracle only supports identifiers up to 30 characters. 
				if ( sProcedureName.Length > 30 )
				{
					sb.Append(TabSpace(nIndent) + "if ( Sql.IsOracle(cmd) )" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName.Substring(0, 30) + "\";" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "else" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "	cmd.CommandText = \"" + sProcedureName + "\";" + ControlChars.CrLf);
				}
				else
				{
					sb.Append(TabSpace(nIndent) + "cmd.CommandText = \"" + sProcedureName + "\";" + ControlChars.CrLf);
				}
			}
			for ( int j = 0 ; j < colRows.Count; j++ )
			{
				DataRow row = colRows[j];
				string sName      = Sql.ToString (row["ColumnName"]);
				string sSqlDbType = Sql.ToString (row["SqlDbType" ]);
				string sCsPrefix  = Sql.ToString (row["CsPrefix"  ]);
				string sCsType    = Sql.ToString (row["CsType"    ]);
				int    nLength    = Sql.ToInteger(row["length"    ]);
				string sBareName  = sName.Replace("@", "");
				nSpace = nColumnAlignmentSize - sBareName.Length;
				nSpace = Math.Max(2, nSpace);
				int nSpaceSqlType = 26 - sSqlDbType.Length;
				nSpaceSqlType = Math.Max(0, nSpaceSqlType);
				/*
				switch ( sSqlDbType )
				{
					case "SqlDbType.VarBinary":
						sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
						break;
					default:
						sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
						break;
				}
				*/
				int nSpaceCsPrefix = 3 + nColumnAlignmentSize - sBareName.Length - sCsPrefix.Length;
				nSpaceCsPrefix = Math.Max(2, nSpaceCsPrefix);
				if ( !bCreateCommand )
				{
					// 01/24/2006 Paul.  A severe error occurred on the current command. The results, if any, should be discarded. 
					// MS03-031 security patch causes this error because of stricter datatype processing.  
					// http://www.microsoft.com/technet/security/bulletin/MS03-031.mspx.
					// http://support.microsoft.com/kb/827366/
					if ( sBareName == "MODIFIED_USER_ID" )
						sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + " Security.USER_ID" + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
					else if ( sSqlDbType == "SqlDbType.NVarChar" )
						sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + "," + Strings.Space(Math.Max(1, 4-nLength.ToString().Length)) + nLength.ToString() +");" + ControlChars.CrLf);
					else if ( sSqlDbType == "SqlDbType.VarChar" )
						sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddAnsiParam(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + "," + Strings.Space(Math.Max(1, 4-nLength.ToString().Length)) + nLength.ToString() +");" + ControlChars.CrLf);
					else
						sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.AddParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
				}
				else
				{
					sb.Append(TabSpace(nIndent) + "IDbDataParameter par" + sBareName + Strings.Space(nSpace-1) + "= Sql.CreateParameter(cmd, \"" + sName + "\"" + Strings.Space(nSpace-2) + ", \"" + sCsType + "\"," + Strings.Space(Math.Max(1, 4-nLength.ToString().Length)) + nLength.ToString() +");" + ControlChars.CrLf);
				}
			}
			if ( !bCreateCommand )
			{
				for ( int j = 0 ; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName      = row["ColumnName"].ToString();
					string sBareName  = sName.Replace("@", "");
					string sCsPrefix  = row["CsPrefix"  ].ToString();
					string sCsType    = row["CsType"    ].ToString();
					int    bIsOutput  = (int) row["isoutparam"];
					nSpace   = nColumnAlignmentSize - sBareName.Length;
					nSpace   = Math.Max(2, nSpace);
					int nSpaceCsPrefix = 3 + nColumnAlignmentSize - sBareName.Length - sCsPrefix.Length;
					nSpaceCsPrefix = Math.Max(2, nSpaceCsPrefix);
					if ( bIsOutput == 1 )
						sb.Append(TabSpace(nIndent) + "par" + sBareName + ".Direction = ParameterDirection.InputOutput;" + ControlChars.CrLf);
					/*
					switch ( sCsType )
					{
						case "string":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBString  (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "DateTime":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBDateTime(" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "Guid":
							if ( sBareName == "MODIFIED_USER_ID" )
								sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Security.USER_ID;" + ControlChars.CrLf);
							else
								sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBGuid    (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "Int32":
						case "short":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBInteger (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "float":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBFloat   (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "decimal":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBDecimal (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "bool":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBBoolean (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						case "byte[]":
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = Sql.ToDBBinary  (" + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ");" + ControlChars.CrLf);
							break;
						default:
							sb.Append(TabSpace(nIndent) + "par" + sBareName + Strings.Space(nSpace-2) + ".Value     = " + sCsPrefix + sBareName + Strings.Space(nSpaceCsPrefix-2) + ";" + ControlChars.CrLf);
							break;
					}
					*/
				}
				sb.Append(TabSpace(nIndent) + "cmd.ExecuteNonQuery();" + ControlChars.CrLf);
				for ( int j = 0 ; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName      = row["ColumnName"].ToString();
					string sBareName  = sName.Replace("@", "");
					string sCsType    = row["CsType"    ].ToString();
					string sCsPrefix  = row["CsPrefix"  ].ToString();
					int    bIsOutput  = (int) row["isoutparam"];
					if ( bIsOutput == 1 )
					{
						nSpace   = nColumnAlignmentSize - sBareName.Length;
						nSpace   = Math.Max(2, nSpace);
						int nSpaceCsPrefix = 3 + nColumnAlignmentSize - sBareName.Length - sCsPrefix.Length;
						nSpaceCsPrefix = Math.Max(2, nSpaceCsPrefix);
						switch ( sCsType )
						{
							case "string":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToString(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "DateTime":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToDateTime(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "Guid":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToGuid(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "Int32":
							case "short":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToInteger(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "float":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToFloat(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "decimal":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToDecimal(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "bool":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToBoolean(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							case "byte[]":
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = Sql.ToBinary(par" + sBareName + ".Value);" + ControlChars.CrLf);
								break;
							default:
								sb.Append(TabSpace(nIndent) + sCsPrefix + sBareName + " = par" + sBareName + ".Value" + ";" + ControlChars.CrLf);
								break;
						}
					}
				}
			}
			else
			{
				// 02/20/2006 Paul.  Need to set the direction. 
				for ( int j = 0 ; j < colRows.Count; j++ )
				{
					DataRow row = colRows[j];
					string sName      = row["ColumnName"].ToString();
					string sBareName  = sName.Replace("@", "");
					int    bIsOutput  = (int) row["isoutparam"];
					if ( bIsOutput == 1 )
						sb.Append(TabSpace(nIndent) + "par" + sBareName + ".Direction = ParameterDirection.InputOutput;" + ControlChars.CrLf);
				}
			}

			if ( !bCreateCommand )
			{
				if ( !bTransaction )
				{
					nIndent--;
					sb.Append(TabSpace(nIndent) + "}" + ControlChars.CrLf);

					sb.Append(TabSpace(nIndent) + "trn.Commit();" + ControlChars.CrLf);
					nIndent--;
					sb.Append(TabSpace(nIndent) + "}" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "catch(Exception ex)" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "{" + ControlChars.CrLf);
					nIndent++;
					sb.Append(TabSpace(nIndent) + "trn.Rollback();" + ControlChars.CrLf);
					sb.Append(TabSpace(nIndent) + "throw(new Exception(ex.Message, ex.InnerException));" + ControlChars.CrLf);
					nIndent--;
					sb.Append(TabSpace(nIndent) + "}" + ControlChars.CrLf);
					nIndent--;
					sb.Append(TabSpace(nIndent) + "}" + ControlChars.CrLf);
				}
				nIndent--;
				sb.Append(TabSpace(nIndent) + "}" + ControlChars.CrLf);
			}
			else
			{
				sb.Append(TabSpace(nIndent) + "return cmd;" + ControlChars.CrLf);
			}
			nIndent--;
			sb.Append(TabSpace(nIndent) + "}" + ControlChars.CrLf);
			sb.Append(TabSpace(nIndent) + "#endregion" + ControlChars.CrLf);
			sb.Append(ControlChars.CrLf);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			StringBuilder sb = new StringBuilder();
			sb.Append("/**********************************************************************************************************************" + ControlChars.CrLf);
			sb.Append(" * The contents of this file are subject to the SugarCRM Public License Version 1.1.3 (\"License\"); You may not use this" + ControlChars.CrLf);
			sb.Append(" * file except in compliance with the License. You may obtain a copy of the License at http://www.sugarcrm.com/SPL" + ControlChars.CrLf);
			sb.Append(" * Software distributed under the License is distributed on an \"AS IS\" basis, WITHOUT WARRANTY OF ANY KIND, either" + ControlChars.CrLf);
			sb.Append(" * express or implied.  See the License for the specific language governing rights and limitations under the License." + ControlChars.CrLf);
			sb.Append(" *" + ControlChars.CrLf);
			sb.Append(" * All copies of the Covered Code must include on each user interface screen:" + ControlChars.CrLf);
			sb.Append(" *    (i) the \"Powered by SugarCRM\" logo and" + ControlChars.CrLf);
			sb.Append(" *    (ii) the SugarCRM copyright notice" + ControlChars.CrLf);
			sb.Append(" *    (iii) the SplendidCRM copyright notice" + ControlChars.CrLf);
			sb.Append(" * in the same form as they appear in the distribution.  See full license for requirements." + ControlChars.CrLf);
			sb.Append(" *" + ControlChars.CrLf);
			sb.Append(" * The Original Code is: SplendidCRM Open Source" + ControlChars.CrLf);
			sb.Append(" * The Initial Developer of the Original Code is SplendidCRM Software, Inc." + ControlChars.CrLf);
			sb.Append(" * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved." + ControlChars.CrLf);
			sb.Append(" * Contributor(s): ______________________________________." + ControlChars.CrLf);
			sb.Append(" *********************************************************************************************************************/" + ControlChars.CrLf);
			
			sb.Append("using System;" + ControlChars.CrLf);
			sb.Append("using System.Data;" + ControlChars.CrLf);
			sb.Append("using System.Data.Common;" + ControlChars.CrLf);
			sb.Append("//using Microsoft.VisualBasic;" + ControlChars.CrLf);
			sb.Append("using System.Xml;" + ControlChars.CrLf);
			sb.Append(ControlChars.CrLf);
			sb.Append("namespace SplendidCRM" + ControlChars.CrLf);
			sb.Append("{" + ControlChars.CrLf);
			sb.Append("	/// <summary>" + ControlChars.CrLf);
			sb.Append("	/// SqlProcs generated on " + DateTime.Now.ToString() + ControlChars.CrLf);
			sb.Append("	/// </summary>" + ControlChars.CrLf);
			sb.Append("	public class SqlProcs" + ControlChars.CrLf);
			sb.Append("	{" + ControlChars.CrLf);
			sb.Append(ControlChars.CrLf);
			
			// 04/13/2006 Paul.  Use existing connection to generate procedures. 
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( SqlConnection con = dbf.CreateConnection() as SqlConnection )
			{
				string sSQL;
				ArrayList arrProcedures = new ArrayList();
				// Get a list of all tables that will need simple INSERT/UPDATE/DELETE procedures. 
				sSQL = "select name           " + ControlChars.CrLf
				     + "  from vwSqlProcedures" + ControlChars.CrLf
				     + " order by name        " + ControlChars.CrLf;
				using ( SqlCommand cmd = new SqlCommand(sSQL, con) )
				{
					using ( SqlDataAdapter da = new SqlDataAdapter(cmd) )
					{
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							for ( int i = 0 ; i < dt.Rows.Count ; i++ )
							{
								DataRow row = dt.Rows[i];
								arrProcedures.Add(row["name"].ToString());
							}
						}
					}
				}
				// Iterate through each table, get all rows, and build INSERT/UPDATE/DELETE procedure
				for ( int i = 0 ; i < arrProcedures.Count ; i++ )
				{
					sSQL = "select *                       " + ControlChars.CrLf
					     + "  from vwSqlColumns            " + ControlChars.CrLf
					     + " where ObjectName = @ObjectName" + ControlChars.CrLf
					     + "   and ObjectType = 'P'        " + ControlChars.CrLf
					     + " order by colid                " + ControlChars.CrLf;
					using ( SqlCommand cmd = new SqlCommand(sSQL, con) )
					{
						Sql.AddParameter(cmd, "@ObjectName", arrProcedures[i].ToString());
						using ( SqlDataAdapter da = new SqlDataAdapter(cmd) )
						{
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								DataRowCollection colRows = dt.Rows;
								BuildWrapper(ref sb, arrProcedures[i].ToString(), ref colRows, false, false);
								BuildWrapper(ref sb, arrProcedures[i].ToString(), ref colRows, false, true );
								BuildWrapper(ref sb, arrProcedures[i].ToString(), ref colRows, true , false);
							}
						}
					}
				}
				int nMaxProcedureLength = 0;
				for ( int i = 0 ; i < arrProcedures.Count ; i++ )
				{
					string sName = arrProcedures[i].ToString();
					if ( sName.Length > nMaxProcedureLength )
						nMaxProcedureLength = sName.Length;
				}
				sb.Append("		#region Factory" + ControlChars.CrLf);
				sb.Append("		/// <summary>" + ControlChars.CrLf);
				sb.Append("		/// Factory" + ControlChars.CrLf);
				sb.Append("		/// </summary>" + ControlChars.CrLf);
				sb.Append("		public static IDbCommand Factory(IDbConnection con, string sProcedureName)" + ControlChars.CrLf);
				sb.Append("		{" + ControlChars.CrLf);
				sb.Append("			IDbCommand cmd = null;" + ControlChars.CrLf);
				sb.Append("			switch ( sProcedureName.ToUpper() )" + ControlChars.CrLf);
				sb.Append("			{" + ControlChars.CrLf);
				for ( int i = 0 ; i < arrProcedures.Count ; i++ )
				{
					string sName = arrProcedures[i].ToString();
					sb.Append("				case \"" + sName.ToUpper() + "\"" + Strings.Space(nMaxProcedureLength - sName.Length) + ":  cmd = cmd" + (sName.StartsWith("sp") ? sName.Substring(2) : sName) + Strings.Space(nMaxProcedureLength - sName.Length) + "(con);  break;" + ControlChars.CrLf);
				}
				sb.Append("				default:  throw(new Exception(\"Unknown stored procedure \" + sProcedureName));" + ControlChars.CrLf);
				sb.Append("			}" + ControlChars.CrLf);
				sb.Append("			return cmd;" + ControlChars.CrLf);
				sb.Append("		}" + ControlChars.CrLf);
				sb.Append("		#endregion" + ControlChars.CrLf);
				sb.Append(ControlChars.CrLf);
			}
			sb.Append("	}" + ControlChars.CrLf);
			sb.Append("}" + ControlChars.CrLf);
			Response.Write("<pre>");
			Response.Write(sb.ToString());
			Response.Write("</pre>");
			
			try
			{
				// 01/16/2008 Paul.  Procedures.aspx was moved to the _devtools folder, 
				// so we need to make sure to reference SqlProcs in the _code folder. 
				string sSqlProcsPath = Server.MapPath("~/_code/SqlProcs.cs");
				using(StreamWriter stm = File.CreateText(sSqlProcsPath))
				{
					stm.Write(sb.ToString());
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
