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
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
		public static void SetPageTitle(Page page, string sTitle)
		{
			try
			{
				Literal litPageTitle = page.FindControl("litPageTitle") as Literal;
				if ( litPageTitle != null )
					litPageTitle.Text = sTitle;
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public static string RegisterEnterKeyPress(string sTextID, string sButtonID)
		{
			// 03/28/2007 Paul.  Fix to support Firefox, which passes the event object in the first parameter. 
			StringBuilder sb = new StringBuilder();
			sb.Append("<script type=\"text/javascript\">\n");
			sb.Append("document.getElementById('" + sTextID + "').onkeypress = function(e)\n");
			sb.Append("{\n");
			sb.Append(" if ( e != null )\n");
			sb.Append(" {\n");
			sb.Append("  if ( e.which == 13 )\n");
			sb.Append("  {\n");
			sb.Append("   document.getElementById('" + sButtonID + "').click();\n");
			sb.Append("   return false;\n");
			sb.Append("  }\n");
			sb.Append(" }\n");
			sb.Append(" else if ( event != null )\n");
			sb.Append(" {\n");
			sb.Append("  if ( event.keyCode == 13 )\n");
			sb.Append("  {\n");
			sb.Append("   event.returnValue = false;\n");
			sb.Append("   event.cancel = true;\n");
			sb.Append("   document.getElementById('" + sButtonID + "').click();\n");
			sb.Append("  }\n");
			sb.Append(" }\n");
			sb.Append("}\n");
			sb.Append("</script>\n");
			return sb.ToString();
		}

		public static string RegisterSetFocus(string sTextID)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script type=\"text/javascript\">\n");
			sb.Append("document.getElementById('" + sTextID + "').focus();\n");
			sb.Append("</script>\n");
			return sb.ToString();
		}
	
		public static WebControl CreateArrowControl(bool bAscending)
		{
			Label lblArrow = new Label();
			lblArrow.Font.Name = "Webdings";
			if ( bAscending )
				lblArrow.Text = "5";
			else
				lblArrow.Text = "6";
			return lblArrow;
		}

		public static string ValidateIDs(string[] arrID, bool bQuoted)
		{
			if ( arrID.Length == 0 )
				return String.Empty;
			if ( arrID.Length > 200 )
			{
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				throw(new Exception(L10n.Term(".LBL_TOO_MANY_RECORDS")));
			}
			
			foreach(string sID in arrID)
			{
				Guid gID = Sql.ToGuid(sID);
				if ( Sql.IsEmptyGuid(gID) )
				{
					// 05/02/2006 Paul.  Provide a more descriptive error message by including the ID. 
					throw(new Exception("Invalid ID: " + sID));
				}
			}
			string sIDs = String.Empty;
			if ( bQuoted )
				sIDs = "'" + String.Join("','", arrID) + "'";
			else
				sIDs = String.Join(",", arrID);
			return sIDs;
		}

		public static string ValidateIDs(string[] arrID)
		{
			return ValidateIDs(arrID, false);
		}

		public static string FilterByACL(string sMODULE_NAME, string sACCESS_TYPE, string[] arrID, string sTABLE_NAME)
		{
			StringBuilder sb = new StringBuilder();
			int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sACCESS_TYPE);
			if ( nACLACCESS >= 0 && arrID.Length > 0 )
			{
				if ( nACLACCESS == ACL_ACCESS.OWNER )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						// 09/26/2006 Paul.  The connection needed to be opened. 
						con.Open();
						string sSQL;
						sSQL = "select ID              " + ControlChars.CrLf
						     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
						     + " where 1 = 1           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendGuids(cmd, arrID, "ID");
							Sql.AppendParameter(cmd, Security.USER_ID, "ASSIGNED_USER_ID", false);
							// 10/16/2006 Paul.  Fix execute to allow more than one row. 
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									if ( sb.Length > 0 )
										sb.Append(",");
									sb.Append(Sql.ToString(rdr["ID"]));
								}
							}
						}
					}
					if ( sb.Length == 0 )
					{
						L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
						throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
					}
				}
				else
				{
					return String.Join(",", arrID);
				}
			}
			return sb.ToString();
		}

		public static string BuildMassIDs(Stack stk, int nCapacity)
		{
			if ( stk.Count == 0 )
				return String.Empty;
			
			StringBuilder sb = new StringBuilder();
			for ( int i = 0; i < nCapacity && stk.Count > 0; i++ )
			{
				string sID = Sql.ToString(stk.Pop());
				if ( sb.Length > 0 )
					sb.Append(",");
				sb.Append(sID);
			}
			return sb.ToString();;
		}

		public static string BuildMassIDs(Stack stkID)
		{
			return BuildMassIDs(stkID, 200);
		}

		public static Stack FilterByACL_Stack(string sMODULE_NAME, string sACCESS_TYPE, string[] arrID, string sTABLE_NAME)
		{
			Stack stk = new Stack();
			StringBuilder sb = new StringBuilder();
			int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sACCESS_TYPE);
			if ( nACLACCESS >= 0 && arrID.Length > 0 )
			{
				if ( nACLACCESS == ACL_ACCESS.OWNER )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						// 09/26/2006 Paul.  The connection needed to be opened. 
						con.Open();
						string sSQL;
						sSQL = "select ID              " + ControlChars.CrLf
						     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
						     + " where 1 = 1           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendGuids(cmd, arrID, "ID");
							Sql.AppendParameter(cmd, Security.USER_ID, "ASSIGNED_USER_ID", false);
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									stk.Push(Sql.ToString(rdr["ID"]));
								}
							}
						}
					}
					if ( stk.Count == 0 )
					{
						L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
						throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
					}
				}
				else
				{
					foreach ( string sID in arrID )
					{
						if ( sID.Length > 0 )
							stk.Push(sID);
					}
				}
			}
			return stk;
		}

		public static void UpdateTracker(Page pParent, string sModule, Guid gID, string sName)
		{
			// 08/21/2005 Paul.  This function is also called after a user clicks Duplicate.
			// In this scenerio, the gID will be NULL, so don't do anything. 
			if ( !Sql.IsEmptyGuid(gID) )
			{
				SqlProcs.spTRACKER_Update(Security.USER_ID, sModule, gID, sName);
				if ( pParent != null )
				{
					// 02/08/2007 Paul.  The control is in the master page. 
					ContentPlaceHolder plcLastViewed = pParent.Master.FindControl("cntLastViewed") as ContentPlaceHolder;
					if ( plcLastViewed != null )
					{
						_controls.LastViewed ctlLastViewed = plcLastViewed.FindControl("ctlLastViewed") as _controls.LastViewed;
						if ( ctlLastViewed != null )
						{
							ctlLastViewed.Refresh();
						}
					}
				}
			}
		}

		public static void AdminShortcuts(Page pParent, bool bAdminShortcuts)
		{
			// 02/08/2007 Paul.  The control is in the master page. 
			ContentPlaceHolder plcShortcuts = pParent.Master.FindControl("cntSidebar") as ContentPlaceHolder;
			if ( plcShortcuts != null )
			{
				_controls.Shortcuts ctlShortcuts = plcShortcuts.FindControl("ctlShortcuts") as _controls.Shortcuts;
				if ( ctlShortcuts != null )
				{
					ctlShortcuts.AdminShortcuts = bAdminShortcuts;
				}
			}
		}

		public static void SetValue(DropDownList lst, string sValue)
		{
			for ( int i=0 ; i < lst.Items.Count; i++ )
			{
				if ( String.Compare(lst.Items[i].Value, sValue, true) == 0 )
				{
					lst.SelectedValue = lst.Items[i].Value;
					break;
				}
			}
		}

		public static string ExpandException(Exception ex)
		{
			StringBuilder sb = new StringBuilder();
			do
			{
				sb.Append(ex.Message);
				// 08/13/2007 Paul.  Only add the line break if there is more data. 
				if ( ex.InnerException != null )
					sb.Append("<br />\r\n");
				ex = ex.InnerException;
			}
			while ( ex != null );
			return sb.ToString();
		}

		public static string GetUserEmail(Guid gID)
		{
			string sEmail = String.Empty;
			if ( !Sql.IsEmptyGuid(gID) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select EMAIL1  " + ControlChars.CrLf
					     + "     , EMAIL2  " + ControlChars.CrLf
					     + "  from vwUSERS " + ControlChars.CrLf
					     + " where ID = @ID" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gID);
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							while ( rdr.Read() )
							{
								sEmail = Sql.ToString(rdr["EMAIL1"]);
								if ( Sql.IsEmptyString(sEmail) )
									sEmail = Sql.ToString(rdr["EMAIL2"]);
							}
						}
					}
				}
			}
			return sEmail;
		}

		public static System.Collections.Specialized.NameValueCollection AppSettings
		{
			get
			{
				#pragma warning disable 618
				return System.Configuration.ConfigurationSettings.AppSettings;
				#pragma warning restore 618
			}
		}

		public static bool IsMobileDevice
		{
			get
			{
				return Sql.ToBoolean(HttpContext.Current.Request.Browser.Capabilities["isMobileDevice"]);
			}
		}

		public static string TableColumnName(L10N L10n, string sModule, string sDISPLAY_NAME)
		{
			// 07/04/2006 Paul.  Some columns have global terms. 
			// 06/05/2007 Paul.  Add Team global term. 
			if (  sDISPLAY_NAME == "DATE_ENTERED" 
			   || sDISPLAY_NAME == "DATE_MODIFIED"
			   || sDISPLAY_NAME == "ASSIGNED_TO"  
			   || sDISPLAY_NAME == "CREATED_BY"   
			   || sDISPLAY_NAME == "MODIFIED_BY"  
			   || sDISPLAY_NAME == "TEAM_NAME"    )
			{
				sDISPLAY_NAME = L10n.Term(".LBL_" + sDISPLAY_NAME).Replace(":", "");
			}
			else
			{
				// 07/04/2006 Paul.  Column names are aliased so that we don't have to redefine terms. 
				sDISPLAY_NAME = L10n.AliasedTerm(sModule + ".LBL_" + sDISPLAY_NAME).Replace(":", "");
			}
			return sDISPLAY_NAME;
		}

		public static string MassEmailerSiteURL(HttpApplicationState Application)
		{
			string sSiteURL = Sql.ToString(Application["CONFIG.site_url"]);
			if ( Sql.ToString(Application["CONFIG.massemailer_tracking_entities_location_type"]) == "2" && !Sql.IsEmptyString(Application["CONFIG.massemailer_tracking_entities_location"]) )
				sSiteURL = Sql.ToString(Application["CONFIG.massemailer_tracking_entities_location"]);
			if ( Sql.IsEmptyString(sSiteURL) )
			{
				// 12/15/2007 Paul.  Use the environment as it is always available. 
				// The Request object is not always available, such as when inside a timer event. 
				// 12/22/2007 Paul.  We are now storing the server name in an application variable. 
				string sServerName      = Sql.ToString(Application["ServerName"     ]);
				string sApplicationPath = Sql.ToString(Application["ApplicationPath"]);
				sSiteURL = sServerName + sApplicationPath;
			}
			if ( !sSiteURL.StartsWith("http") )
				sSiteURL = "http://" + sSiteURL;
			if ( !sSiteURL.EndsWith("/") )
				sSiteURL += "/";
			return sSiteURL;
		}

		public static void RefreshAllViews()
		{
			// 05/08/2007 Paul.  Keep the original procedure call so that we will get a compiler error if something changes. 
			bool bIncreaseTimeout = true;
			if ( !bIncreaseTimeout )
			{
				SqlProcs.spSqlRefreshAllViews();
			}
			else
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
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
								cmd.CommandText = "spSqlRefreshAllViews";
								// 05/08/2007 Paul.  Allow this to run until it completes. 
								cmd.CommandTimeout = 0;
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

		public static DataTable CheckVersion(HttpApplicationState Application)
		{
			XmlDocument xml = new XmlDocument();
			xml.Load("http://demo.splendidcrm.com/Administration/Versions.xml" + (Sql.ToBoolean(Application["CONFIG.send_usage_info"]) ? Utils.UsageInfo(Application) : String.Empty));

			Version vSplendidVersion = new Version(Sql.ToString(Application["SplendidVersion"]));
			DataTable dt = XmlUtil.CreateDataTable(xml.DocumentElement, "Version", new string[] {"Build", "Date", "Description", "URL", "New"});
			foreach ( DataRow row in dt.Rows )
			{
				Version vBuild = new Version(Sql.ToString(row["Build"]));
				if ( vSplendidVersion < vBuild )
					row["New"] = "1";
			}
			return dt;
		}

		public static string UsageInfo(HttpApplicationState Application)
		{
			StringBuilder sb = new StringBuilder();
			//sb.Append("&Machine="  + HttpUtility.UrlEncode(System.Environment.MachineName                   ));
			//sb.Append("&Procs="    + HttpUtility.UrlEncode(System.Environment.ProcessorCount.ToString()     ));
			//sb.Append("&IP="       + HttpUtility.UrlEncode(Sql.ToString(Application["ServerIPAddress"     ])));
			sb.Append("?Server="   + HttpUtility.UrlEncode(Sql.ToString(Application["ServerName"          ])));
			sb.Append("&Splendid=" + HttpUtility.UrlEncode(Sql.ToString(Application["SplendidVersion"     ])));
			sb.Append("&Key="      + HttpUtility.UrlEncode(Sql.ToString(Application["CONFIG.unique_key"   ])));
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select count(*)    " + ControlChars.CrLf
					     + "  from vwUSERS     " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						sb.Append("&Users=" + Sql.ToString(cmd.ExecuteScalar()));
					}
					sSQL = "select count(*)    " + ControlChars.CrLf
					     + "  from vwUSERS     " + ControlChars.CrLf
					     + " where IS_ADMIN = 1" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						sb.Append("&Admins=" + Sql.ToString(cmd.ExecuteScalar()));
					}
					sSQL = "select count(*)    " + ControlChars.CrLf
					     + "  from vwUSERS     " + ControlChars.CrLf
					     + " where IS_GROUP = 1" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						sb.Append("&Groups=" + Sql.ToString(cmd.ExecuteScalar()));
					}
					/*
					// Remove redundant information. 
					sSQL = "select count(*)    " + ControlChars.CrLf
					     + "  from vwUSERS     " + ControlChars.CrLf
					     + " where (IS_ADMIN is null or IS_ADMIN = 0)" + ControlChars.CrLf
					     + "   and (IS_GROUP is null or IS_GROUP = 0)" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						sb.Append("&Registered=" + Sql.ToString(cmd.ExecuteScalar()));
					}
					*/
					// 01/14/2008 Paul.  SQL Server 2000 cannot count unique Guids. 
					sSQL = "select count(distinct cast(USER_ID as char(36)))" + ControlChars.CrLf
					     + "  from TRACKER                      " + ControlChars.CrLf
					     + " where DATE_ENTERED >= @DATE_ENTERED" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@DATE_ENTERED", DateTime.Today.AddMonths(-1));
						sb.Append("&Activity=" + Sql.ToString(cmd.ExecuteScalar()));
					}

					// 01/14/2008 Paul.  Put the OS Version and SQL Version at the end as they may get truncated. 
					// INETLOG only saves the first 255 of the query string. 
					sSQL = "select Version     " + ControlChars.CrLf
					     + "  from vwSqlVersion" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						string sDBVersion = Sql.ToString(cmd.ExecuteScalar());
						sDBVersion = sDBVersion.Replace("Microsoft ", "");
						sDBVersion = sDBVersion.Replace("Intel "    , "");
						sb.Append("&DB=" + HttpUtility.UrlEncode(sDBVersion));
					}
				}
			}
			catch //(Exception ex)
			{
			}
			string sOSVersion = System.Environment.OSVersion.ToString();
			sOSVersion = sOSVersion.Replace("Microsoft "  , "");
			sOSVersion = sOSVersion.Replace("Service Pack", "SP");
			sb.Append("&OS="      + HttpUtility.UrlEncode(sOSVersion));
			// 01/19/2008 Paul.  The application path seems useful, but will usually be /SplendidCRM. 
			sb.Append("&AppPath=" + HttpUtility.UrlEncode(Sql.ToString(Application["ApplicationPath"     ])));
			// 01/19/2008 Paul.  The second least is the .NET version because it will almost always be the current shipping version. 
			sb.Append("&System="  + HttpUtility.UrlEncode(System.Environment.Version.ToString()            ));
			// 01/19/2008 Paul.  The least important piece of information is the SugarVersion.  
			sb.Append("&Sugar="   + HttpUtility.UrlEncode(Sql.ToString(Application["CONFIG.sugar_version"])));
			return sb.ToString();
		}
	}
}
