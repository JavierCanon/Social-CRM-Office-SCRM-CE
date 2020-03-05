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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	public class WorkflowUtils
	{
		private static bool bInsideWorkflow = false;

		#region spWORKFLOW_EVENTS_Delete
		/// <summary>
		/// spWORKFLOW_EVENTS_Delete
		/// </summary>
		public static void spWORKFLOW_EVENTS_Delete(HttpApplicationState Application, Guid gID)
		{
			if ( HttpContext.Current != null && HttpContext.Current.Application != null )
			{
				// 12/22/2007 Paul.  By calling the SqlProcs version, we will ensure a compile-time error if the parameters change. 
				SqlProcs.spWORKFLOW_EVENTS_Delete(gID);
			}
			else
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = con.BeginTransaction() )
					{
						try
						{
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.CommandText = "spWORKFLOW_EVENTS_Delete";
								IDbDataParameter parID                 = Sql.AddParameter(cmd, "@ID"                , gID       );
								IDbDataParameter parMODIFIED_USER_ID   = Sql.AddParameter(cmd, "@MODIFIED_USER_ID"  , Guid.Empty);
								cmd.ExecuteNonQuery();
							}
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception(ex.Message, ex.InnerException));
						}
					}
				}
			}
		}
		#endregion

		#region spWORKFLOW_EVENTS_ProcessAll
		/// <summary>
		/// spWORKFLOW_EVENTS_ProcessAll
		/// </summary>
		public static void spWORKFLOW_EVENTS_ProcessAll(HttpApplicationState Application)
		{
			if ( HttpContext.Current != null && HttpContext.Current.Application != null )
			{
				// 12/22/2007 Paul.  By calling the SqlProcs version, we will ensure a compile-time error if the parameters change. 
				SqlProcs.spWORKFLOW_EVENTS_ProcessAll();
			}
			else
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = con.BeginTransaction() )
					{
						try
						{
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.CommandText = "spWORKFLOW_EVENTS_ProcessAll";
								cmd.ExecuteNonQuery();
							}
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception(ex.Message, ex.InnerException));
						}
					}
				}
			}
		}
		#endregion

		public static void Process(HttpApplicationState Application)
		{
			if ( !bInsideWorkflow )
			{
				bInsideWorkflow = true;
				try
				{
					//SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), "WorkflowUtils.Process Begin");

					spWORKFLOW_EVENTS_ProcessAll(Application);
					/*
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select *                " + ControlChars.CrLf
						     + "  from vwWORKFLOW_EVENTS" + ControlChars.CrLf
						     + " order by AUDIT_VERSION " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							con.Open();

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
										SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), "Processing " + dt.Rows.Count.ToString() + " workflow events");
									foreach ( DataRow row in dt.Rows )
									{
										Guid gID = Sql.ToGuid(row["ID"]);
										// 12/30/2007 Paul.  We are not going to do anything yet, but we do need to clean up the table. 
										spWORKFLOW_EVENTS_Delete(Application, gID);
									}
								}
							}
						}
					}
					*/
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideWorkflow = false;
				}
			}
		}
	}
}
