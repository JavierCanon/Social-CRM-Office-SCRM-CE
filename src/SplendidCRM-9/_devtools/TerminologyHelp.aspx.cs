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
	/// Summary description for TerminologyHelp.
	/// </summary>
	public class TerminologyHelp : System.Web.UI.Page
	{
		protected void DumpTerminology(IDbCommand cmd, string sProcedureName, int nNAME_MaxLength)
		{
			using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
			{
				int nCount = 0;
				while ( rdr.Read() )
				{
					// 04/29/2006 Paul.  DB2 is having a heap error SQL0954C.  
					// Increase the size of the heap and decrease the size of the procedure. 
					// db2 => connect to splendid
					// 1)	update db cfg for splendid using applheapsz 1024
					if ( nCount > 250 )
					{
						Response.Write("/* -- #if Oracle" + ControlChars.CrLf);
						Response.Write("	COMMIT WORK;" + ControlChars.CrLf);
						Response.Write("END;" + ControlChars.CrLf);
						Response.Write("/" + ControlChars.CrLf);
						
						Response.Write(ControlChars.CrLf);
						Response.Write("BEGIN" + ControlChars.CrLf);
						Response.Write("-- #endif Oracle */" + ControlChars.CrLf);
						nCount = 0;
					}
					nCount++;
					Response.Write("exec dbo." + sProcedureName + " ");
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
						// 11/21/2005 Paul.  Align the name field. 
						if ( nColumn == 0 )
						{
							string sNAME = rdr.GetString(nColumn);
							if ( nNAME_MaxLength - sNAME.Length > 0 )
								Response.Write(Strings.Space(nNAME_MaxLength - sNAME.Length));
						}
					}
					Response.Write(";" + ControlChars.CrLf);
				}
				if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) )
					Response.Write("/" + ControlChars.CrLf + ControlChars.CrLf);
				if ( Sql.IsSQLServer(cmd) )
					Response.Write("GO" + ControlChars.CrLf + ControlChars.CrLf);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
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
							string sSQL;
							string sLANG = Sql.ToString(Request.QueryString["Lang"]) ;
							if ( Sql.IsEmptyString(sLANG) )
							{
								sSQL = "select *          " + ControlChars.CrLf
								     + "  from vwLANGUAGES" + ControlChars.CrLf
								     + " order by NAME    " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									Response.Write("<html><body><h1>Terminology Help</h1>");
									while ( rdr.Read() )
									{
										Response.Write("<a href=\"TerminologyHelp.aspx?Lang=" + rdr.GetString(rdr.GetOrdinal("NAME")) + "\">" + rdr.GetString(rdr.GetOrdinal("DISPLAY_NAME")) + "</a><br>" + ControlChars.CrLf);
									}
									Response.Write("</body></html>");
								}
							}
							else
							{
								Response.ContentType = "text/sql";
								Response.AddHeader("Content-Disposition", "attachment;filename=TERMINOLOGY_HELP " + sLANG + ".3.sql");

								if ( Sql.IsOracle(cmd) )
								{
									Response.Write("ALTER SESSION SET NLS_DATE_FORMAT='YYYY-MM-DD HH24:MI:SS';" + ControlChars.CrLf);
									Response.Write("BEGIN" + ControlChars.CrLf);
								}
								else
								{
									Response.Write("/* -- #if IBM_DB2" + ControlChars.CrLf);
									Response.Write("call dbo.spSqlDropProcedure('spTERMINOLOGY_HELP_Defaults')" + ControlChars.CrLf);
									Response.Write("/" + ControlChars.CrLf);
									Response.Write("" + ControlChars.CrLf);
									Response.Write("Create Procedure dbo.spTERMINOLOGY_HELP_Defaults()" + ControlChars.CrLf);
									Response.Write("language sql" + ControlChars.CrLf);
									Response.Write("  begin" + ControlChars.CrLf);
									Response.Write("-- #endif IBM_DB2 */" + ControlChars.CrLf);
									Response.Write("" + ControlChars.CrLf);
									
									Response.Write("/* -- #if Oracle" + ControlChars.CrLf);
									Response.Write("BEGIN" + ControlChars.CrLf);
									Response.Write("-- #endif Oracle */" + ControlChars.CrLf);
									
									Response.Write("print 'TERMINOLOGY_HELP " + sLANG + "';" + ControlChars.CrLf);
									Response.Write("GO" + ControlChars.CrLf);
									Response.Write(ControlChars.CrLf);
									Response.Write("set nocount on;" + ControlChars.CrLf);
									Response.Write("GO" + ControlChars.CrLf);
									Response.Write(ControlChars.CrLf);
								}
								sSQL = "select NAME               " + ControlChars.CrLf
								     + "     , LCID               " + ControlChars.CrLf
								     + "     , ACTIVE             " + ControlChars.CrLf
								     + "     , NATIVE_NAME        " + ControlChars.CrLf
								     + "     , DISPLAY_NAME       " + ControlChars.CrLf
								     + "  from LANGUAGES          " + ControlChars.CrLf
								     + " where lower(NAME) = @NAME" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
								Sql.AddParameter(cmd, "@NAME", sLANG.ToLower());
								DumpTerminology(cmd, "spLANGUAGES_InsertOnly", sLANG.Length);

								sSQL = "select max(len(NAME))     " + ControlChars.CrLf
								     + "  from TERMINOLOGY_HELP   " + ControlChars.CrLf
								     + " where lower(LANG) = @LANG" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								cmd.Parameters.Clear();
								// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
								Sql.AddParameter(cmd, "@LANG", sLANG.ToLower());
								int nNAME_MaxLength = Sql.ToInteger(cmd.ExecuteScalar()) + 2;
								
								sSQL = "select NAME                      " + ControlChars.CrLf
								     + "     , LANG                      " + ControlChars.CrLf
								     + "     , MODULE_NAME               " + ControlChars.CrLf
								     + "     , DISPLAY_TEXT              " + ControlChars.CrLf
								     + "  from vwTERMINOLOGY_HELP        " + ControlChars.CrLf
								     + " where lower(LANG) = @LANG       " + ControlChars.CrLf
								     + " order by LANG, MODULE_NAME, NAME" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								cmd.Parameters.Clear();
								// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
								Sql.AddParameter(cmd, "@LANG", sLANG.ToLower());
								DumpTerminology(cmd, "spTERMINOLOGY_HELP_InsertOnly", nNAME_MaxLength);
								Response.Write(ControlChars.CrLf);
								Response.Write("set nocount off;" + ControlChars.CrLf);
								Response.Write("GO" + ControlChars.CrLf);
								Response.Write(ControlChars.CrLf);
								if ( Sql.IsOracle(cmd) )
								{
									Response.Write("	COMMIT WORK;" + ControlChars.CrLf);
									Response.Write("END;" + ControlChars.CrLf);
									Response.Write("/" + ControlChars.CrLf);
								}
								else
								{
									Response.Write("/* -- #if Oracle" + ControlChars.CrLf);
									Response.Write("	COMMIT WORK;" + ControlChars.CrLf);
									Response.Write("END;" + ControlChars.CrLf);
									Response.Write("/" + ControlChars.CrLf);
									Response.Write("-- #endif Oracle */" + ControlChars.CrLf);

									Response.Write("" + ControlChars.CrLf);
									Response.Write("/* -- #if IBM_DB2" + ControlChars.CrLf);
									Response.Write("	commit;" + ControlChars.CrLf);
									Response.Write("  end" + ControlChars.CrLf);
									Response.Write("/" + ControlChars.CrLf);
									Response.Write("" + ControlChars.CrLf);

									Response.Write("call dbo.spTERMINOLOGY_HELP_Defaults()" + ControlChars.CrLf);
									Response.Write("/" + ControlChars.CrLf);
									Response.Write("" + ControlChars.CrLf);

									Response.Write("call dbo.spSqlDropProcedure('spTERMINOLOGY_HELP_Defaults')" + ControlChars.CrLf);
									Response.Write("/" + ControlChars.CrLf);
									Response.Write("-- #endif IBM_DB2 */" + ControlChars.CrLf);
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
