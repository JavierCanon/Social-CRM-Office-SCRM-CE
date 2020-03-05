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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Diagnostics;

namespace SplendidCRM
{
	public class SchedulerUtils
	{
		private static bool bInsideTimer = false;

		public static string[] Jobs = new string[]
			{ "pollMonitoredInboxes"
			, "runMassEmailCampaign"
			, "pruneDatabase"
			, "pollMonitoredInboxesForBouncedCampaignEmails"
			, "BackupDatabase"
			, "BackupTransactionLog"
			, "CheckVersion"
			};

		#region CronDescription
		/// <summary>
		/// CronDescription
		/// </summary>
		public static string CronDescription(L10N L10n, string sCRON)
		{
			StringBuilder sb = new StringBuilder();
			sCRON = sCRON.Replace(" ", "");
			if ( sCRON == "*::*::*::*::*" )
				return L10n.Term("Schedulers.LBL_OFTEN");

			CultureInfo culture = CultureInfo.CreateSpecificCulture(L10n.NAME);
			string sCRON_MONTH       = "*";
			string sCRON_DAYOFMONTH  = "*";
			string sCRON_DAYOFWEEK   = "*";
			string sCRON_HOUR        = "*";
			string sCRON_MINUTE      = "*";
			string[] arrCRON         = sCRON.Replace("::", "|").Split('|');
			string[] arrCRON_TEMP    = new string[] {};
			string[] arrCRON_VALUE   = new string[] {};
			string[] arrDaySuffixes  = new string[32];
			int    nCRON_VALUE       = 0;
			int    nCRON_VALUE_START = 0;
			int    nCRON_VALUE_END   = 0;
			int    nON_THE_MINUTE    = -1;
			for ( int n = 0; n < arrDaySuffixes.Length; n++ )
				arrDaySuffixes[n] = "th";
			arrDaySuffixes[0] = "";
			arrDaySuffixes[1] = "st";
			arrDaySuffixes[2] = "nd";
			arrDaySuffixes[3] = "rd";

			// minute  hour  dayOfMonth  month  dayOfWeek
			if ( arrCRON.Length > 0 ) sCRON_MINUTE     = arrCRON[0];
			if ( arrCRON.Length > 1 ) sCRON_HOUR       = arrCRON[1];
			if ( arrCRON.Length > 2 ) sCRON_DAYOFMONTH = arrCRON[2];
			if ( arrCRON.Length > 3 ) sCRON_MONTH      = arrCRON[3];
			if ( arrCRON.Length > 4 ) sCRON_DAYOFWEEK  = arrCRON[4];
			if ( sCRON_MINUTE != "*" )
			{
				arrCRON_TEMP = sCRON_MINUTE.Split(',');
				// 12/31/2007 Paul.  Check for either comma or dash. 
				if ( sCRON_MINUTE.Split(",-".ToCharArray()).Length == 1 )
				{
					nON_THE_MINUTE = Sql.ToInteger(sCRON_MINUTE);
					sb.Append(L10n.Term("Schedulers.LBL_ON_THE"));
					if ( nON_THE_MINUTE == 0 )
					{
						sb.Append(L10n.Term("Schedulers.LBL_HOUR_SING"));
					}
					else
					{
						sb.Append(nON_THE_MINUTE.ToString("00"));
						sb.Append(L10n.Term("Schedulers.LBL_MIN_MARK"));
					}
				}
				else
				{
					for ( int i = 0, nCronEntries = 0; i < arrCRON_TEMP.Length; i++ )
					{
						if ( arrCRON_TEMP[i].IndexOf('-') >= 0 )
						{
							arrCRON_VALUE = arrCRON_TEMP[i].Split('-');
							if ( arrCRON_VALUE.Length >= 2 )
							{
								nCRON_VALUE_START = Sql.ToInteger(arrCRON_VALUE[0]);
								nCRON_VALUE_END   = Sql.ToInteger(arrCRON_VALUE[1]);
								if ( nCRON_VALUE_START >= 0 && nCRON_VALUE_START <= 23 && nCRON_VALUE_END >= 0 && nCRON_VALUE_END <= 23 )
								{
									if ( nCronEntries > 0 )
										sb.Append(L10n.Term("Schedulers.LBL_AND"));
									sb.Append(L10n.Term("Schedulers.LBL_FROM"));
									sb.Append(L10n.Term("Schedulers.LBL_ON_THE"));
									if ( nCRON_VALUE_START == 0 )
									{
										sb.Append(L10n.Term("Schedulers.LBL_HOUR_SING"));
									}
									else
									{
										sb.Append(nCRON_VALUE_START.ToString("0"));
										sb.Append(L10n.Term("Schedulers.LBL_MIN_MARK"));
									}
									sb.Append(L10n.Term("Schedulers.LBL_RANGE"));
									sb.Append(L10n.Term("Schedulers.LBL_ON_THE"));
									sb.Append(nCRON_VALUE_END.ToString("0"));
									sb.Append(L10n.Term("Schedulers.LBL_MIN_MARK"));
									nCronEntries++;
								}
							}
						}
						else
						{
							nCRON_VALUE = Sql.ToInteger(arrCRON_TEMP[i]);
							if ( nCRON_VALUE >= 0 && nCRON_VALUE <= 23 )
							{
								if ( nCronEntries > 0 )
									sb.Append(L10n.Term("Schedulers.LBL_AND"));
								sb.Append(L10n.Term("Schedulers.LBL_ON_THE"));
								if ( nCRON_VALUE == 0 )
								{
									sb.Append(L10n.Term("Schedulers.LBL_HOUR_SING"));
								}
								else
								{
									sb.Append(nCRON_VALUE.ToString("0"));
									sb.Append(L10n.Term("Schedulers.LBL_MIN_MARK"));
								}
								nCronEntries++;
							}
						}
					}
				}
			}
			if ( sCRON_HOUR != "*" )
			{
				if ( sb.Length > 0 )
					sb.Append("; ");
				arrCRON_TEMP = sCRON_HOUR.Split(',');
				for ( int i = 0, nCronEntries = 0; i < arrCRON_TEMP.Length; i++ )
				{
					if ( arrCRON_TEMP[i].IndexOf('-') >= 0 )
					{
						arrCRON_VALUE = arrCRON_TEMP[i].Split('-');
						if ( arrCRON_VALUE.Length >= 2 )
						{
							nCRON_VALUE_START = Sql.ToInteger(arrCRON_VALUE[0]);
							nCRON_VALUE_END   = Sql.ToInteger(arrCRON_VALUE[1]);
							if ( nCRON_VALUE_START >= 1 && nCRON_VALUE_START <= 31 && nCRON_VALUE_END >= 1 && nCRON_VALUE_END <= 31 )
							{
								if ( nCronEntries > 0 )
									sb.Append(L10n.Term("Schedulers.LBL_AND"));
								sb.Append(L10n.Term("Schedulers.LBL_FROM"));
								sb.Append(arrCRON_VALUE[0]);
								if ( nON_THE_MINUTE >= 0 )
									sb.Append(":" + nON_THE_MINUTE.ToString("00"));
								sb.Append(L10n.Term("Schedulers.LBL_RANGE"));
								sb.Append(arrCRON_VALUE[1]);
								if ( nON_THE_MINUTE >= 0 )
									sb.Append(":" + nON_THE_MINUTE.ToString("00"));
								nCronEntries++;
							}
						}
					}
					else
					{
						nCRON_VALUE = Sql.ToInteger(arrCRON_TEMP[i]);
						if ( nCRON_VALUE >= 1 && nCRON_VALUE <= 31 )
						{
							if ( nCronEntries > 0 )
								sb.Append(L10n.Term("Schedulers.LBL_AND"));
							sb.Append(arrCRON_TEMP[i]);
							if ( nON_THE_MINUTE >= 0 )
								sb.Append(":" + nON_THE_MINUTE.ToString("00"));
							nCronEntries++;
						}
					}
				}
			}
			if ( sCRON_DAYOFMONTH != "*" )
			{
				if ( sb.Length > 0 )
					sb.Append("; ");
				arrCRON_TEMP = sCRON_DAYOFMONTH.Split(',');
				for ( int i = 0, nCronEntries = 0; i < arrCRON_TEMP.Length; i++ )
				{
					if ( arrCRON_TEMP[i].IndexOf('-') >= 0 )
					{
						arrCRON_VALUE = arrCRON_TEMP[i].Split('-');
						if ( arrCRON_VALUE.Length >= 2 )
						{
							nCRON_VALUE_START = Sql.ToInteger(arrCRON_VALUE[0]);
							nCRON_VALUE_END   = Sql.ToInteger(arrCRON_VALUE[1]);
							if ( nCRON_VALUE_START >= 1 && nCRON_VALUE_START <= 31 && nCRON_VALUE_END >= 1 && nCRON_VALUE_END <= 31 )
							{
								if ( nCronEntries > 0 )
									sb.Append(L10n.Term("Schedulers.LBL_AND"));
								sb.Append(L10n.Term("Schedulers.LBL_FROM"));
								sb.Append(nCRON_VALUE_START.ToString() + arrDaySuffixes[nCRON_VALUE_START]);
								sb.Append(L10n.Term("Schedulers.LBL_RANGE"));
								sb.Append(nCRON_VALUE_END.ToString() + arrDaySuffixes[nCRON_VALUE_END]);
								nCronEntries++;
							}
						}
					}
					else
					{
						nCRON_VALUE = Sql.ToInteger(arrCRON_TEMP[i]);
						if ( nCRON_VALUE >= 1 && nCRON_VALUE <= 31 )
						{
							if ( nCronEntries > 0 )
								sb.Append(L10n.Term("Schedulers.LBL_AND"));
							sb.Append(nCRON_VALUE.ToString() + arrDaySuffixes[nCRON_VALUE]);
							nCronEntries++;
						}
					}
				}
			}
			if ( sCRON_MONTH != "*" )
			{
				if ( sb.Length > 0 )
					sb.Append("; ");
				arrCRON_TEMP = sCRON_MONTH.Split(',');
				for ( int i = 0, nCronEntries = 0; i < arrCRON_TEMP.Length; i++ )
				{
					if ( arrCRON_TEMP[i].IndexOf('-') >= 0 )
					{
						arrCRON_VALUE = arrCRON_TEMP[i].Split('-');
						if ( arrCRON_VALUE.Length >= 2 )
						{
							nCRON_VALUE_START = Sql.ToInteger(arrCRON_VALUE[0]);
							nCRON_VALUE_END   = Sql.ToInteger(arrCRON_VALUE[1]);
							if ( nCRON_VALUE_START >= 1 && nCRON_VALUE_START <= 12 && nCRON_VALUE_END >= 1 && nCRON_VALUE_END <= 12 )
							{
								if ( nCronEntries > 0 )
									sb.Append(L10n.Term("Schedulers.LBL_AND"));
								sb.Append(L10n.Term("Schedulers.LBL_FROM"));
								sb.Append(culture.DateTimeFormat.MonthNames[nCRON_VALUE_START]);
								sb.Append(L10n.Term("Schedulers.LBL_RANGE"));
								sb.Append(culture.DateTimeFormat.MonthNames[nCRON_VALUE_END]);
								nCronEntries++;
							}
						}
					}
					else
					{
						nCRON_VALUE = Sql.ToInteger(arrCRON_TEMP[i]);
						if ( nCRON_VALUE >= 1 && nCRON_VALUE <= 12 )
						{
							if ( nCronEntries > 0 )
								sb.Append(L10n.Term("Schedulers.LBL_AND"));
							sb.Append(culture.DateTimeFormat.MonthNames[nCRON_VALUE]);
							nCronEntries++;
						}
					}
				}
			}
			if ( sCRON_DAYOFWEEK != "*" )
			{
				if ( sb.Length > 0 )
					sb.Append("; ");
				arrCRON_TEMP = sCRON_DAYOFWEEK.Split(',');
				for ( int i = 0, nCronEntries = 0; i < arrCRON_TEMP.Length; i++ )
				{
					if ( arrCRON_TEMP[i].IndexOf('-') >= 0 )
					{
						arrCRON_VALUE = arrCRON_TEMP[i].Split('-');
						if ( arrCRON_VALUE.Length >= 2 )
						{
							nCRON_VALUE_START = Sql.ToInteger(arrCRON_VALUE[0]);
							nCRON_VALUE_END   = Sql.ToInteger(arrCRON_VALUE[1]);
							if ( nCRON_VALUE_START >= 0 && nCRON_VALUE_START <= 6 && nCRON_VALUE_END >= 0 && nCRON_VALUE_END <= 6 )
							{
								if ( nCronEntries > 0 )
									sb.Append(L10n.Term("Schedulers.LBL_AND"));
								sb.Append(L10n.Term("Schedulers.LBL_FROM"));
								sb.Append(culture.DateTimeFormat.DayNames[nCRON_VALUE_START]);
								sb.Append(L10n.Term("Schedulers.LBL_RANGE"));
								sb.Append(culture.DateTimeFormat.DayNames[nCRON_VALUE_END]);
								nCronEntries++;
							}
						}
					}
					else
					{
						nCRON_VALUE = Sql.ToInteger(arrCRON_TEMP[i]);
						if ( nCRON_VALUE >= 0 && nCRON_VALUE <= 6 )
						{
							if ( nCronEntries > 0 )
								sb.Append(L10n.Term("Schedulers.LBL_AND"));
							sb.Append(culture.DateTimeFormat.DayNames[nCRON_VALUE]);
							nCronEntries++;
						}
					}
				}
			}
			return sb.ToString();
		}
		#endregion

		public static void OnTimer(Object sender)
		{
			// 12/22/2007 Paul.  In case the timer takes a long time, only allow one timer event to be processed. 
			if ( !bInsideTimer )
			{
				bInsideTimer = true;
				HttpApplication global = sender as HttpApplication;
				try
				{
					// 12/30/2007 Paul.  Workflow events always get processed. 
					WorkflowUtils.Process(global.Application);
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory(global.Application);
					using ( DataTable dt = new DataTable() )
					{
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwSCHEDULERS_Run" + ControlChars.CrLf
							     + " order by NEXT_RUN     " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 01/01/2008 Paul.  The scheduler query should always be very fast. 
								// In the off chance that there is a problem, abort after 15 seconds. 
								cmd.CommandTimeout = 15;

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
								}
							}
						}
						// 01/13/2008 Paul.  Loop outside the connection so that only one connection will be used. 
						foreach ( DataRow row in dt.Rows )
						{
							Guid     gID        = Sql.ToGuid    (row["ID"      ]);
							string   sJOB       = Sql.ToString  (row["JOB"     ]);
							DateTime dtNEXT_RUN = Sql.ToDateTime(row["NEXT_RUN"]);
							try
							{
								switch ( sJOB )
								{
									case "function::BackupDatabase":
									{
										// 01/28/2008 Paul.  Cannot perform a backup or restore operation within a transaction. BACKUP DATABASE is terminating abnormally.
										using ( IDbConnection con = dbf.CreateConnection() )
										{
											con.Open();
											try
											{
												string sFILENAME = String.Empty;
												string sTYPE     = "FULL";
												//SqlProcs.spSqlBackupDatabase(ref sNAME, "FULL", trn);
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.CommandType = CommandType.StoredProcedure;
													cmd.CommandText = "spSqlBackupDatabase";
													IDbDataParameter parFILENAME = Sql.AddParameter(cmd, "@FILENAME", sFILENAME  , 255);
													IDbDataParameter parTYPE     = Sql.AddParameter(cmd, "@TYPE"    , sTYPE      ,  20);
													parFILENAME.Direction = ParameterDirection.InputOutput;
													cmd.ExecuteNonQuery();
													sFILENAME = Sql.ToString(parFILENAME.Value);
												}
												SplendidError.SystemMessage(global.Application, "Information", new StackTrace(true).GetFrame(0), "Database backup complete " + sFILENAME);
											}
											catch(Exception ex)
											{
												SplendidError.SystemMessage(global.Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
											}
										}
										break;
									}
									case "function::BackupTransactionLog":
									{
										// 01/28/2008 Paul.  Cannot perform a backup or restore operation within a transaction. BACKUP DATABASE is terminating abnormally.
										using ( IDbConnection con = dbf.CreateConnection() )
										{
											con.Open();
											try
											{
												string sFILENAME = String.Empty;
												string sTYPE     = "LOG";
												//SqlProcs.spSqlBackupDatabase(ref sNAME, "LOG", trn);
												using ( IDbCommand cmd = con.CreateCommand() )
												{
													cmd.CommandType = CommandType.StoredProcedure;
													cmd.CommandText = "spSqlBackupDatabase";
													IDbDataParameter parFILENAME = Sql.AddParameter(cmd, "@FILENAME", sFILENAME  , 255);
													IDbDataParameter parTYPE     = Sql.AddParameter(cmd, "@TYPE"    , sTYPE      ,  20);
													parFILENAME.Direction = ParameterDirection.InputOutput;
													cmd.ExecuteNonQuery();
													sFILENAME = Sql.ToString(parFILENAME.Value);
												}
												SplendidError.SystemMessage(global.Application, "Information", new StackTrace(true).GetFrame(0), "Transaction Log backup complete " + sFILENAME);
											}
											catch(Exception ex)
											{
												SplendidError.SystemMessage(global.Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
											}
										}
										break;
									}
									case "function::runMassEmailCampaign":
									{
										// 12/30/2007 Paul.  Update the last run date before running so that the date marks the start of the run. 
										EmailUtils.SendQueued(global.Application, Guid.Empty, Guid.Empty, false);
										break;
									}
									case "function::pruneDatabase"       :
									{
										using ( IDbConnection con = dbf.CreateConnection() )
										{
											con.Open();
											using ( IDbTransaction trn = con.BeginTransaction() )
											{
												try
												{
													SqlProcs.spSqlPruneDatabase(trn);
													trn.Commit();
												}
												catch(Exception ex)
												{
													trn.Rollback();
													SplendidError.SystemMessage(global.Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
												}
											}
										}
										break;
									}
									case "function::pollMonitoredInboxes":
									{
										EmailUtils.CheckMonitored(global.Application, Guid.Empty);
										break;
									}
									case "function::pollMonitoredInboxesForBouncedCampaignEmails":
									{
										EmailUtils.CheckBounced(global.Application, Guid.Empty);
										break;
									}
									case "function::CheckVersion":
									{
										DataTable dtVersions = Utils.CheckVersion(global.Application);

										DataView vwVersions = dtVersions.DefaultView;
										vwVersions.RowFilter = "New = '1'";
										if ( vwVersions.Count > 0 )
										{
											global.Application["available_version"            ] = Sql.ToString(vwVersions[0]["Build"      ]);
											global.Application["available_version_description"] = Sql.ToString(vwVersions[0]["Description"]);
										}
										break;
									}
								}
							}
							finally
							{
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									using ( IDbTransaction trn = con.BeginTransaction() )
									{
										try
										{
											// 01/12/2008 Paul.  Make sure the Last Run value is updated after the operation.
											SqlProcs.spSCHEDULERS_UpdateLastRun(gID, dtNEXT_RUN, trn);
											trn.Commit();
										}
										catch(Exception ex)
										{
											trn.Rollback();
											SplendidError.SystemMessage(global.Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
										}
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(global.Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideTimer = false;
				}
			}
		}
	}
}
