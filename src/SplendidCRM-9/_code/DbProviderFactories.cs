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
using System.Data;
using System.Data.Common;
using Microsoft.Win32;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for DbProviderFactories.
	/// </summary>
	public class DbProviderFactories
	{
		// 12/22/2007 Paul.  Inside the timer event, there is no current context, so we need to pass the application. 
		public static DbProviderFactory GetFactory(HttpApplicationState Application)
		{
			// 11/14/2005 Paul.  Cache the connection string in the application as config and registry access is expected to be slower. 
			string sSplendidProvider = Sql.ToString(Application["SplendidProvider"]);
			string sConnectionString = Sql.ToString(Application["ConnectionString"]);
#if DEBUG
//			sSplendidProvider = String.Empty;
#endif
			if ( Sql.IsEmptyString(sSplendidProvider) || Sql.IsEmptyString(sConnectionString) )
			{
				sSplendidProvider = Utils.AppSettings["SplendidProvider"];
				switch ( sSplendidProvider )
				{
					case "System.Data.SqlClient":
						sConnectionString = Utils.AppSettings["SplendidSQLServer"];
						break;
					case "System.Data.OracleClient":
						sConnectionString = Utils.AppSettings["SplendidSystemOracle"];
						break;
					case "Oracle.DataAccess.Client":
						sConnectionString = Utils.AppSettings["SplendidOracle"];
						break;
					case "MySql.Data":
						sConnectionString = Utils.AppSettings["SplendidMySql"];
						break;
					case "IBM.Data.DB2":
						sConnectionString = Utils.AppSettings["SplendidDB2"];
						break;
					case "Sybase.Data.AseClient":
						sConnectionString = Utils.AppSettings["SplendidSybase"];
						break;
					case "iAnywhere.Data.AsaClient":
						sConnectionString = Utils.AppSettings["SplendidSQLAnywhere"];
						break;
					case "Npgsql":
						sConnectionString = Utils.AppSettings["SplendidNpgsql"];
						break;
					case "Registry":
					{
						string sSplendidRegistry = Utils.AppSettings["SplendidRegistry"];
						if ( Sql.IsEmptyString(sSplendidRegistry) )
						{
							// 11/14/2005 Paul.  If registry key is not provided, then compute it using the server and the application path. 
							// This will allow a single installation to support multiple databases. 
							// 12/22/2007 Paul.  We can no longer rely upon the Request object being valid as we might be inside the timer event. 
							string sServerName      = Sql.ToString(Application["ServerName"     ]);
							string sApplicationPath = Sql.ToString(Application["ApplicationPath"]);
							sSplendidRegistry  = "SOFTWARE\\SplendidCRM Software\\" ;
							sSplendidRegistry += sServerName;
							if ( sApplicationPath != "/" )
								sSplendidRegistry += sApplicationPath.Replace("/", "\\");
						}
						using (RegistryKey keySplendidCRM = Registry.LocalMachine.OpenSubKey(sSplendidRegistry))
						{
							if ( keySplendidCRM != null )
							{
								sSplendidProvider = Sql.ToString(keySplendidCRM.GetValue("SplendidProvider"));
								sConnectionString = Sql.ToString(keySplendidCRM.GetValue("ConnectionString"));
								// 01/17/2008 Paul.  99.999% percent of the time, we will be hosting on SQL Server. 
								// If the provider is not specified, then just assume SQL Server. 
								if ( Sql.IsEmptyString(sSplendidProvider) )
									sSplendidProvider = "System.Data.SqlClient";
							}
							else
							{
								throw(new Exception("Database connection information was not found in the registry " + sSplendidRegistry));
							}
						}
						break;
					}
					case "HostingDatabase":
					{
						// 09/27/2006 Paul.  Allow a Hosting Database to contain connection strings. 
						/*
						<appSettings>
							<add key="SplendidProvider"          value="HostingDatabase" />
							<add key="SplendidHostingProvider"   value="System.Data.SqlClient" />
							<add key="SplendidHostingConnection" value="data source=(local)\SplendidCRM;initial catalog=SplendidCRM;user id=sa;password=" />
						</appSettings>
						*/
						string sSplendidHostingProvider   = Utils.AppSettings["SplendidHostingProvider"  ];
						string sSplendidHostingConnection = Utils.AppSettings["SplendidHostingConnection"];
						if ( Sql.IsEmptyString(sSplendidHostingProvider) || Sql.IsEmptyString(sSplendidHostingConnection) )
						{
							throw(new Exception("SplendidHostingProvider and SplendidHostingConnection are both required in order to pull the connection from a hosting server. "));
						}
						else
						{
							// 12/22/2007 Paul.  We can no longer rely upon the Request object being valid as we might be inside the timer event. 
							string sSplendidHostingSite = Sql.ToString(Application["ServerName"     ]);
							string sApplicationPath     = Sql.ToString(Application["ApplicationPath"]);
							if ( sApplicationPath != "/" )
								sSplendidHostingSite += sApplicationPath;
							
							DbProviderFactory dbf = GetFactory(sSplendidHostingProvider, sSplendidHostingConnection);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL ;
								sSQL = "select SPLENDID_PROVIDER           " + ControlChars.CrLf
								     + "     , CONNECTION_STRING           " + ControlChars.CrLf
								     + "     , EXPIRATION_DATE             " + ControlChars.CrLf
								     + "  from vwSPLENDID_HOSTING_SITES    " + ControlChars.CrLf
								     + " where HOSTING_SITE = @HOSTING_SITE" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@HOSTING_SITE", sSplendidHostingSite);
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											sSplendidProvider = Sql.ToString(rdr["SPLENDID_PROVIDER"]);
											sConnectionString = Sql.ToString(rdr["CONNECTION_STRING"]);
											// 01/17/2008 Paul.  99.999% percent of the time, we will be hosting on SQL Server. 
											// If the provider is not specified, then just assume SQL Server. 
											if ( Sql.IsEmptyString(sSplendidProvider) )
												sSplendidProvider = "System.Data.SqlClient";
											if ( rdr["EXPIRATION_DATE"] != DBNull.Value )
											{
												DateTime dtEXPIRATION_DATE = Sql.ToDateTime(rdr["EXPIRATION_DATE"]);
												if ( dtEXPIRATION_DATE < DateTime.Today )
													throw(new Exception("The hosting site " + sSplendidHostingSite + " expired on " + dtEXPIRATION_DATE.ToShortDateString()));
											}
											if ( Sql.IsEmptyString(sSplendidProvider) || Sql.IsEmptyString(sSplendidProvider) )
												throw(new Exception("Incomplete database connection information was found on the hosting server for site " + sSplendidHostingSite));
										}
										else
										{
											throw(new Exception("Database connection information was not found on the hosting server for site " + sSplendidHostingSite));
										}
									}
								}
							}
						}
						break;
					}
				}
				Application["SplendidProvider"] = sSplendidProvider;
				Application["ConnectionString"] = sConnectionString;
			}
			return GetFactory(sSplendidProvider, sConnectionString);
		}

		public static DbProviderFactory GetFactory()
		{
			if ( HttpContext.Current == null || HttpContext.Current.Application == null )
				throw(new Exception("DbProviderFactory.GetFactory: Application cannot be NULL."));
			return GetFactory(HttpContext.Current.Application);
		}

		public static DbProviderFactory GetFactory(string sSplendidProvider, string sConnectionString)
		{
			switch ( sSplendidProvider )
			{
				case "System.Data.SqlClient":
				{
					return new SqlClientFactory(sConnectionString);
				}
				case "System.Data.OracleClient":
				{
					return new OracleSystemDataFactory(sConnectionString);
				}
				case "Oracle.DataAccess.Client":
				{
					return new OracleClientFactory(sConnectionString);
				}
				case "MySql.Data":
				{
					return new MySQLClientFactory(sConnectionString);
				}
				case "IBM.Data.DB2":
				{
					return new DB2ClientFactory(sConnectionString);
				}
				case "Sybase.Data.AseClient":
				{
					return new SybaseClientFactory(sConnectionString);
				}
				case "iAnywhere.Data.AsaClient":
				{
					return new SQLAnywhereClientFactory(sConnectionString);
				}
				case "Npgsql":
				{
					return new NpgsqlClientFactory(sConnectionString);
				}
				default:
					throw(new Exception("Unsupported factory " + sSplendidProvider));
			}
		}
	}
}
