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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for ExportAll.
	/// </summary>
	public class ExportAll : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			// 12/22/2007 Paul.  Allow an admin to dump all their data. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
			Response.ContentType = "Text/SQL";
			Response.AddHeader("Content-Disposition", "attachment;filename=SplendidCRM Dump.sql");
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							string[] aTableNames = new String[]
							{ "ACCOUNTS"
							, "BUGS"
							, "CALLS"
							, "CAMPAIGNS"
							, "CASES"
							, "CONFIG"
							, "CONTACTS"
							, "CURRENCIES"
							, "CUSTOM_FIELDS"
							, "DOCUMENTS"
							, "dtproperties"
							, "EMAIL_TEMPLATES"
							, "EMAILS"
							, "FEEDS"
							, "FIELDS_META_DATA"
							, "FILES"
							, "IFRAMES"
							, "IMPORT_MAPS"
							, "LEADS"
							, "MEETINGS"
							, "NOTES"
							, "OPPORTUNITIES"
							, "PROJECT"
							, "PROJECT_TASK"
							, "PROSPECT_LISTS"
							, "PROSPECTS"
							, "RELEASES"
							, "ROLES"
							, "TASKS"
							, "TERMINOLOGY"
							, "TIMEZONES"
							, "USERS"
							, "USERS_LAST_IMPORT"
							, "VCALS"
							, "VERSIONS"
							, "ACCOUNTS_BUGS"
							, "ACCOUNTS_CASES"
							, "ACCOUNTS_CONTACTS"
							, "ACCOUNTS_OPPORTUNITIES"
							, "CALLS_CONTACTS"
							, "CALLS_USERS"
							, "CASES_BUGS"
							, "CONTACTS_BUGS"
							, "CONTACTS_CASES"
							, "DOCUMENT_REVISIONS"
							, "EMAIL_MARKETING"
							, "EMAILMAN"
							, "EMAILMAN_SENT"
							, "EMAILS_ACCOUNTS"
							, "EMAILS_CASES"
							, "EMAILS_CONTACTS"
							, "EMAILS_OPPORTUNITIES"
							, "EMAILS_USERS"
							, "MEETINGS_CONTACTS"
							, "MEETINGS_USERS"
							, "OPPORTUNITIES_CONTACTS"
							, "PROJECT_RELATION"
							, "PROSPECT_LIST_CAMPAIGNS"
							, "PROSPECT_LISTS_PROSPECTS"
							, "ROLES_MODULES"
							, "ROLES_USERS"
							, "TRACKER"
							, "USERS_FEEDS"
							};
							if ( !Sql.IsEmptyString(Request.QueryString["Table"]) )
							{
								// 12/11//2005 Paul.  Need to allow for multiple tables, comma separated. 
								aTableNames = Sql.ToString(Request.QueryString["Table"]).Split(',');
							}
							string sLANG = Sql.ToString(Request.QueryString["Lang"]) ;
							foreach ( string sTableName in aTableNames )
							{
								cmd.CommandText = "select * from " + Sql.EscapeSQL(sTableName) + " where 1 = 1" + ControlChars.CrLf;
								if ( String.Compare(sTableName, "TERMINOLOGY", true) == 0 && !Sql.IsEmptyString(sLANG) )
								{
									Sql.AppendParameter(cmd, sLANG, "LANG");
								}
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									StringBuilder sb = new StringBuilder();
									if ( Sql.IsOracle(cmd) )
										Response.Write("ALTER SESSION SET NLS_DATE_FORMAT='YYYY-MM-DD HH24:MI:SS';" + ControlChars.CrLf);
									while ( rdr.Read() )
									{
										if ( sb.Length == 0 )
										{
											sb.Append("insert into " + sTableName + "(");
											for ( int nColumn=0 ; nColumn < rdr.FieldCount ; nColumn++ )
											{
												if ( nColumn > 0 )
													sb.Append(", ");
												/*
												// 10/15/2005 Paul.  Table columns have been renamed in SugarCRM 3.5. 
												if ( rdr.GetName(nColumn) == "NUMBER" )
												{
													if ( sTableName == "BUGS" )
														sb.Append("BUG_NUMBER");
													else if ( sTableName == "CASES" )
														sb.Append("CASE_NUMBER");
													else if ( sTableName == "TRACKER" )
														sb.Append("TRACKER_NUMBER");
												}
												else
												*/
												{
													sb.Append(rdr.GetName(nColumn));
												}
											}
											sb.Append(")" + ControlChars.CrLf);
										}
										Response.Write(sb.ToString());
									
										Response.Write(Strings.Space(6+sTableName.Length) + "values(");
										for ( int nColumn=0 ; nColumn < rdr.FieldCount ; nColumn++ )
										{
											if ( nColumn > 0 )
												Response.Write(", ");
											if ( rdr.IsDBNull(nColumn) )
												Response.Write("null");
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Boolean" ) ) Response.Write(rdr.GetBoolean (nColumn) ? "1" : "0" );
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Single"  ) ) Response.Write(rdr.GetDouble  (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Double"  ) ) Response.Write(rdr.GetDouble  (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int16"   ) ) Response.Write(rdr.GetInt16   (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int32"   ) ) Response.Write(rdr.GetInt32   (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Int64"   ) ) Response.Write(rdr.GetInt64   (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Decimal" ) ) Response.Write(rdr.GetDecimal (nColumn).ToString());
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.DateTime") ) Response.Write("\'" + rdr.GetDateTime(nColumn).ToString("yyyy-MM-dd HH:mm:ss") + "\'");
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.Guid"    ) ) Response.Write("\'" + rdr.GetGuid  (nColumn).ToString().ToUpper() + "\'");
											else if ( rdr.GetFieldType(nColumn) == Type.GetType("System.String"  ) ) Response.Write("\'" + rdr.GetString(nColumn).Replace("\'", "\'\'") + "\'");
											else Response.Write("null");
										}
										Response.Write(");" + ControlChars.CrLf);
									}
									if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
										Response.Write("/" + ControlChars.CrLf + ControlChars.CrLf);
									if ( Sql.IsSQLServer(cmd) )
										Response.Write("GO" + ControlChars.CrLf + ControlChars.CrLf);
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message + ControlChars.CrLf);
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
