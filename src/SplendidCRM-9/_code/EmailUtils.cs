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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	public class EmailUtils
	{
		private static bool bInsideSendQueue    = false;
		private static bool bInsideCheckInbound = false;

		// Cross-Site Scripting (XSS) filter. 
		// http://you.gotfoo.org/howto-anti-xss-w-aspnet-and-c/
		public static string XssFilter(string sHTML, string sXSSTags)
		{
			RegexOptions options = RegexOptions.IgnoreCase;
			string nojavascript = ("([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([\\`\\\'\\\\\"]*)[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x0" + "0-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*");
			Regex regex = new Regex(nojavascript, options);
			string sResult = regex.Replace(sHTML, "");
			if ( !Sql.IsEmptyString(sXSSTags) )
			{
				string unwantedTags = "</*(" + sXSSTags + ")[^>]*>"; 
				regex = new Regex(unwantedTags, options);
				sResult = regex.Replace(sResult, "");
			}
			return sResult;
		}

		public static MailAddress SplitMailAddress(string sFullAddress)
		{
			string sName    = String.Empty;
			string sAddress = String.Empty;
			int nStartAddress = sFullAddress.IndexOf('<');
			if ( nStartAddress > 0 )
			{
				sName = sFullAddress.Substring(0, nStartAddress-1);
				sName = sName.Trim();
				sAddress = sFullAddress.Substring(nStartAddress+1);
				int nEndAddress = sAddress.IndexOf('>');
				if ( nEndAddress >= 0 )
					sAddress = sAddress.Substring(0, nEndAddress);
			}
			else
			{
				sAddress = sFullAddress;
			}
			if ( sName != String.Empty )
				return new MailAddress(sAddress, sName);
			else
				return new MailAddress(sAddress);
		}

		public static string FillEmail(string sTEMPLATE_BODY, string sMODULE, DataRow row)
		{
			string sEMAIL_BODY = sTEMPLATE_BODY;
			if ( row != null )
			{
				sMODULE = sMODULE.ToLower();
				foreach ( DataColumn col in row.Table.Columns )
				{
					if ( row[col.ColumnName] == DBNull.Value )
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + col.ColumnName.ToLower(), String.Empty);
					else
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + col.ColumnName.ToLower(), row[col.ColumnName].ToString());
				}
			}
			return sEMAIL_BODY;
		}

		public static string FillEmail(string sTEMPLATE_BODY, string sMODULE, DataRowView row)
		{
			string sEMAIL_BODY = sTEMPLATE_BODY;
			if ( row != null )
			{
				sMODULE = sMODULE.ToLower();
				foreach ( DataColumn col in row.Row.Table.Columns )
				{
					if ( row[col.ColumnName] == DBNull.Value )
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + col.ColumnName.ToLower(), String.Empty);
					else
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + col.ColumnName.ToLower(), row[col.ColumnName].ToString());
				}
			}
			return sEMAIL_BODY;
		}

		public static DataTable CampaignTrackers(Guid gID)
		{
			DataTable dt = new DataTable();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select ID                        " + ControlChars.CrLf
				     + "     , TRACKER_NAME              " + ControlChars.CrLf
				     + "     , TRACKER_URL               " + ControlChars.CrLf
				     + "     , IS_OPTOUT                 " + ControlChars.CrLf
				     + "  from vwCAMPAIGNS_CAMPAIGN_TRKRS" + ControlChars.CrLf
				     + " where CAMPAIGN_ID = @CAMPAIGN_ID" + ControlChars.CrLf
				     + " order by DATE_ENTERED asc       " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@CAMPAIGN_ID", gID);
		
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
					}
				}
			}
			return dt;
		}

		public static DataTable EmailTemplateAttachments(Guid gID)
		{
			DataTable dt = new DataTable();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *                            " + ControlChars.CrLf
				     + "  from vwEMAIL_TEMPLATES_Attachments" + ControlChars.CrLf
				     + " where EMAIL_TEMPLATE_ID = @EMAIL_TEMPLATE_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_TEMPLATE_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
					}
				}
			}
			return dt;
		}

		public static string FillTrackers(string sBODY_HTML, DataTable dtTrackers, string sSiteURL, Guid gTARGET_TRACKER_KEY, L10N L10n)
		{
			bool bHAS_OPTOUT_LINKS = false;
			foreach ( DataRow row in dtTrackers.Rows )
			{
				Guid   gTRACKER_ID   = Sql.ToGuid   (row["ID"          ]);
				string sTRACKER_NAME = Sql.ToString (row["TRACKER_NAME"]);
				string sTRACKER_URL  = Sql.ToString (row["TRACKER_URL" ]);
				bool   bIS_OPTOUT    = Sql.ToBoolean(row["IS_OPTOUT"   ]);
				string sTrackerPath = String.Empty;
				if ( bIS_OPTOUT )
				{
					bHAS_OPTOUT_LINKS = true;
					sTrackerPath += "RemoveMe.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString();
				}
				else
				{
					sTrackerPath += "campaign_trackerv2.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString();
					sTrackerPath += "&track=" + gTRACKER_ID.ToString();
				}
				sBODY_HTML = sBODY_HTML.Replace("{" + sTRACKER_NAME + "}", sSiteURL + sTrackerPath);
			}

			if ( !bHAS_OPTOUT_LINKS )
				sBODY_HTML += "<br><font size='2'>" + HttpUtility.HtmlEncode(L10n.Term("EmailMan.TXT_REMOVE_ME")) + "<a href='" + sSiteURL + "RemoveMe.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString() + "'>" + HttpUtility.HtmlEncode(L10n.Term("EmailMan.TXT_REMOVE_ME_CLICK")) + "</a></font>";
			sBODY_HTML += "<br><img height='1' width='1' src='" + sSiteURL + "image.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString() + "'>";
			return sBODY_HTML;
		}

		public static void SendEmail(Guid gID, string sFromName, string sFromAddress, ref int nEmailsSent)
		{
			MailMessage mail = new MailMessage();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL ;
				bool bReadyToSend = false;
				DataTable dtContacts  = new DataTable();
				DataTable dtLeads     = new DataTable();
				DataTable dtProspects = new DataTable();
				// 10/05/2007 Paul.  The vwEMAILS_CONTACTS view handles the join and returns all vwCONTACTS data. 
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_CONTACTS   " + ControlChars.CrLf
				     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtContacts);
					}
				}
				// 10/05/2007 Paul.  The vwEMAILS_LEADS view handles the join and returns all vwLEADS data. 
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_LEADS      " + ControlChars.CrLf
				     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtLeads);
					}
				}
				// 10/05/2007 Paul.  The vwEMAILS_PROSPECTS view handles the join and returns all vwPROSPECTS data. 
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_PROSPECTS  " + ControlChars.CrLf
				     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtProspects);
					}
				}
				DataView vwContacts  = new DataView(dtContacts );
				DataView vwLeads     = new DataView(dtLeads    );
				DataView vwProspects = new DataView(dtProspects);
				string[] arrTo = new string[] {};
				
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_ReadyToSend" + ControlChars.CrLf
				     + " where ID = @ID            " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gID);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							bReadyToSend = true;
							string sFrom        = Sql.ToString(rdr["FROM_ADDR"       ]);
							string sTo          = Sql.ToString(rdr["TO_ADDRS"        ]);
							string sCC          = Sql.ToString(rdr["CC_ADDRS"        ]);
							string sBcc         = Sql.ToString(rdr["BCC_ADDRS"       ]);
							string sSubject     = Sql.ToString(rdr["NAME"            ]);
							string sBody        = Sql.ToString(rdr["DESCRIPTION"     ]);
							string sBodyHtml    = Sql.ToString(rdr["DESCRIPTION_HTML"]);
							string sPARENT_TYPE = Sql.ToString(rdr["PARENT_TYPE"     ]);
							Guid   gPARENT_ID   = Sql.ToGuid  (rdr["PARENT_ID"       ]);

							// 12/19/2006 Paul.  Fill the email with parent data. 
							if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								DataTable dtParent = Crm.Modules.Parent(sPARENT_TYPE, gPARENT_ID);
								if ( dtParent.Rows.Count > 0 )
								{
									string sFillPrefix = String.Empty;
									switch ( sPARENT_TYPE )
									{
										case "Accounts" :  sFillPrefix = "account";  break;
										case "Contacts" :  sFillPrefix = "contact";  break;
										case "Leads"    :  sFillPrefix = "contact";  break;
										case "Prospects":  sFillPrefix = "contact";  break;
										default:
											sFillPrefix = sPARENT_TYPE;
											if ( sFillPrefix.EndsWith("s") )
												sFillPrefix = sFillPrefix.Substring(0, sFillPrefix.Length-1);
											break;
									}
									// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
									sSubject  = EmailUtils.FillEmail(sSubject , sFillPrefix, dtParent.Rows[0]);
									sBodyHtml = EmailUtils.FillEmail(sBodyHtml, sFillPrefix, dtParent.Rows[0]);
								}
							}

							if ( Sql.IsEmptyString(sFrom) && !Sql.IsEmptyString(sFromAddress) )
								mail.From = new MailAddress(sFromAddress, sFromName);
							else if ( !Sql.IsEmptyString(sFrom) )
								mail.From = EmailUtils.SplitMailAddress(sFrom);
							
							// 12/19/2006 Paul.  We are going to send each email in the TO field as a separate email. 
							arrTo = sTo.Split(';');
							/*
							foreach ( string sAddress in arrTo )
							{
								if ( sAddress.Trim() != String.Empty )
									mail.To.Add(SEmailUtils.plitMailAddress(sAddress));
							}
							*/
							string[] arrAddresses = sCC.Split(';');
							foreach ( string sAddress in arrAddresses )
							{
								if ( sAddress.Trim() != String.Empty )
									mail.CC.Add(EmailUtils.SplitMailAddress(sAddress));
							}
							arrAddresses = sBcc.Split(';');
							foreach ( string sAddress in arrAddresses )
							{
								if ( sAddress.Trim() != String.Empty )
									mail.Bcc.Add(EmailUtils.SplitMailAddress(sAddress));
							}
							mail.Subject     = sSubject       ;
							if ( !Sql.IsEmptyString(sBodyHtml) )
							{
								mail.Body         = sBodyHtml;
								// 08/24/2006 Paul.  Set the encoding to UTF8. 
								mail.BodyEncoding = System.Text.Encoding.UTF8;
								mail.IsBodyHtml   = true;
							}
							else
							{
								mail.Body       = sBody    ;
							}
							mail.Headers.Add("X-SplendidCRM-ID", gID.ToString());
						}
						else
						{
							throw(new Exception("SendEmail: Email is not ready to send, " + gID.ToString()));
						}
					}
				}

				if ( bReadyToSend )
				{
					// 07/30/2006 Paul.  .NET 2.0 now supports sending mail from a stream, remove the directory stuff. 
					using ( DataTable dtAttachments = new DataTable() )
					{
						sSQL = "select *                   " + ControlChars.CrLf
						     + "  from vwEMAILS_Attachments" + ControlChars.CrLf
						     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@EMAIL_ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dtAttachments);
							}
						}
						
						try
						{
							if ( dtAttachments.Rows.Count > 0 )
							{
								foreach(DataRow row in dtAttachments.Rows)
								{
									string sFILENAME           = Sql.ToString(row["FILENAME"          ]);
									string sFILE_MIME_TYPE     = Sql.ToString(row["FILE_MIME_TYPE"    ]);
									Guid   gNOTE_ATTACHMENT_ID = Sql.ToGuid  (row["NOTE_ATTACHMENT_ID"]);

									// 07/30/2006 Paul.  We cannot close the streams until the message is sent. 
									MemoryStream mem = new MemoryStream();
									BinaryWriter writer = new BinaryWriter(mem);
									Notes.Attachment.WriteStream(gNOTE_ATTACHMENT_ID, con, writer);
									writer.Flush();
									mem.Seek(0, SeekOrigin.Begin);
									Attachment att = new Attachment(mem, sFILENAME, sFILE_MIME_TYPE);
									mail.Attachments.Add(att);
								}
							}
							HttpApplicationState Application = HttpContext.Current.Application;
							// 04/17/2006 Paul.  Use config value for SMTP server. 
							// 12/21/2006 Paul.  Allow the use of SMTP servers that require authentication. 
							//string sFromName     = Sql.ToString (Application["CONFIG.fromname"    ]);
							//string sFromAddress  = Sql.ToString (Application["CONFIG.fromaddress" ]);
							string sSmtpServer   = Sql.ToString (Application["CONFIG.smtpserver"  ]);
							int    nSmtpPort     = Sql.ToInteger(Application["CONFIG.smtpport"    ]);
							bool   bSmtpAuthReq  = Sql.ToBoolean(Application["CONFIG.smtpauth_req"]);
							bool   bSmtpSSL      = Sql.ToBoolean(Application["CONFIG.smtpssl"     ]);
							string sSmtpUser     = Sql.ToString (Application["CONFIG.smtpuser"    ]);
							string sSmtpPassword = Sql.ToString (Application["CONFIG.smtppass"    ]);
							// 01/12/2008 Paul.  We must decrypt the password before using it. 
							if ( !Sql.IsEmptyString(sSmtpPassword) )
							{
								Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
								Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
								sSmtpPassword = Security.DecryptPassword(sSmtpPassword, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							}
							if ( Sql.IsEmptyString(sSmtpServer) )
								sSmtpServer = "127.0.0.1";
							if ( nSmtpPort == 0 )
								nSmtpPort = 25;
							
							SmtpClient client = new SmtpClient(sSmtpServer, nSmtpPort);
							client.Timeout = 60 * 1000;
							// 01/12/2008 Paul.  Use SMTP SSL flag to support Gmail. 
							if ( bSmtpSSL )
								client.EnableSsl = true;
							if ( bSmtpAuthReq )
								client.Credentials = new NetworkCredential(sSmtpUser, sSmtpPassword);
							else
								client.UseDefaultCredentials = true;
							
							if ( arrTo.Length > 0 )
							{
								string sSubject = mail.Subject;
								string sBody    = mail.Body   ;
								foreach ( string sAddress in arrTo )
								{
									if ( sAddress.Trim() != String.Empty )
									{
										MailAddress addr = EmailUtils.SplitMailAddress(sAddress);
										mail.To.Clear();
										mail.To.Add(addr);
										// 12/19/2006 Paul.  The address can be in any one of three tables.  
										// Try and filter on the minimum number of tables for performance reasons. 
										vwContacts.RowFilter = "EMAIL1 = '" + addr.Address + "'";
										if ( vwContacts.Count > 0 )
										{
											// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
											mail.Subject = EmailUtils.FillEmail(sSubject, "contact", vwContacts[0]);
											mail.Body    = EmailUtils.FillEmail(sBody   , "contact", vwContacts[0]);
										}
										else
										{
											vwLeads.RowFilter = "EMAIL1 = '" + addr.Address + "'";
											if ( vwLeads.Count > 0 )
											{
												mail.Subject = EmailUtils.FillEmail(sSubject, "contact", vwLeads[0]);
												mail.Body    = EmailUtils.FillEmail(sBody   , "contact", vwLeads[0]);
											}
											else
											{
												vwProspects.RowFilter = "EMAIL1 = '" + addr.Address + "'";
												if ( vwProspects.Count > 0 )
												{
													mail.Subject = EmailUtils.FillEmail(sSubject, "contact", vwProspects[0]);
													mail.Body    = EmailUtils.FillEmail(sBody   , "contact", vwProspects[0]);
												}
												else
												{
													// 12/19/2006 Paul.  The email might be free-form, so just send unfilled text. 
													mail.Subject = sSubject;
													mail.Body    = sBody   ;
												}
											}
										}
										client.Send(mail);
										nEmailsSent++;
										// 12/19/2006 Paul.  Clear the CC and BCC after first send so that they only get one email. 
										mail.CC.Clear();
										mail.Bcc.Clear();
									}
								}
							}
							else if ( mail.CC.Count > 0 || mail.Bcc.Count > 0 )
							{
								// 12/19/2006 Paul.  Still send the email even if there are no TO addresses. 
								client.Send(mail);
								nEmailsSent++;
							}
							else
								throw(new Exception("SendEmail: No addresses"));
						}
						finally
						{
							// 07/30/2006 Paul.  Close the streams after the message is sent. 
							foreach ( Attachment att in mail.Attachments )
							{
								if ( att.ContentStream != null )
									att.ContentStream.Close();
							}
						}
					}
				}
			}
		}

		public static void SendQueued(HttpApplicationState Application, Guid gID, Guid gCAMPAIGN_ID, bool bSendNow)
		{
			if ( !bInsideSendQueue )
			{
				bInsideSendQueue = true;
				Hashtable hashTrackers    = new Hashtable();
				Hashtable hashAttachments = new Hashtable();
				Hashtable hashNoteStreams = new Hashtable();
				try
				{
					//SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), "SendQueued Begin");

					// 04/17/2006 Paul.  Use config value for SMTP server. 
					// 12/21/2006 Paul.  Allow the use of SMTP servers that require authentication. 
					string sFromName     = Sql.ToString (Application["CONFIG.fromname"    ]);
					string sFromAddress  = Sql.ToString (Application["CONFIG.fromaddress" ]);
					string sSmtpServer   = Sql.ToString (Application["CONFIG.smtpserver"  ]);
					int    nSmtpPort     = Sql.ToInteger(Application["CONFIG.smtpport"    ]);
					bool   bSmtpAuthReq  = Sql.ToBoolean(Application["CONFIG.smtpauth_req"]);
					bool   bSmtpSSL      = Sql.ToBoolean(Application["CONFIG.smtpssl"     ]);
					string sSmtpUser     = Sql.ToString (Application["CONFIG.smtpuser"    ]);
					string sSmtpPassword = Sql.ToString (Application["CONFIG.smtppass"    ]);
					// 01/12/2008 Paul.  We must decrypt the password before using it. 
					if ( !Sql.IsEmptyString(sSmtpPassword) )
					{
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						sSmtpPassword = Security.DecryptPassword(sSmtpPassword, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
					}
					int    nEmailsPerRun = Sql.ToInteger(Application["CONFIG.massemailer_campaign_emails_per_run"]);
					if ( Sql.IsEmptyString(sSmtpServer) )
						sSmtpServer = "127.0.0.1";
					if ( nSmtpPort == 0 )
						nSmtpPort = 25;
					if ( nEmailsPerRun == 0 )
						nEmailsPerRun = 500;
					
					SmtpClient client = new SmtpClient(sSmtpServer, nSmtpPort);
					client.Timeout = 60 * 1000;
					// 01/12/2008 Paul.  Use SMTP SSL flag to support Gmail. 
					if ( bSmtpSSL )
						client.EnableSsl = true;
					if ( bSmtpAuthReq )
						client.Credentials = new NetworkCredential(sSmtpUser, sSmtpPassword);
					else
						client.UseDefaultCredentials = true;

					L10N L10n = new L10N(SplendidDefaults.Culture(Application));
					string sSiteURL = Utils.MassEmailerSiteURL(Application);
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						// 01/12/2008 Paul.  Preview is different in that it does not filter on queue date. 
						if ( !Sql.IsEmptyGuid(gID) )
						{
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwEMAILMAN_Preview" + ControlChars.CrLf
							     + " where 1 = 1             " + ControlChars.CrLf;
						}
						else
						{
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwEMAILMAN_Send" + ControlChars.CrLf
							     + " where 1 = 1          " + ControlChars.CrLf;
						}
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							if ( !Sql.IsEmptyGuid(gID) )
								Sql.AppendParameter(cmd, gID, "ID", false);
							// 01/12/2008 Paul.  Allow filtering by campaign for the Sent Test. 
							else if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
								Sql.AppendParameter(cmd, gCAMPAIGN_ID, "CAMPAIGN_ID", false);
							else if ( !bSendNow )
								Sql.AppendParameter(cmd, DateTime.Now, DateTime.MinValue, "SEND_DATE_TIME");
							// 12/20/2007 Paul.  Set the order so that it is predictable. 
							cmd.CommandText += " order by CAMPAIGN_ID, MARKETING_ID, LIST_ID, EMAIL_TEMPLATE_ID, RECIPIENT_EMAIL";
							if ( !bSendNow && nEmailsPerRun > 0 )
							{
								Sql.LimitResults(cmd, nEmailsPerRun);
							}

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
										SplendidError.SystemMessage(Application, "Warning", new StackTrace(true).GetFrame(0), "Processing " + dt.Rows.Count.ToString() + " emails");
									foreach ( DataRow row in dt.Rows )
									{
										gID = Sql.ToGuid(row["ID"]);
										string sRECIPIENT_NAME    = Sql.ToString(row["RECIPIENT_NAME"   ]);
										string sRECIPIENT_EMAIL   = Sql.ToString(row["RECIPIENT_EMAIL"  ]);
										string sSUBJECT           = Sql.ToString(row["SUBJECT"          ]);
										string sBODY_HTML         = Sql.ToString(row["BODY_HTML"        ]);
										string sRELATED_TYPE      = Sql.ToString(row["RELATED_TYPE"     ]);
										Guid   gRELATED_ID        = Sql.ToGuid  (row["RELATED_ID"       ]);
										string sCAMPAIGN_NAME     = Sql.ToString(row["CAMPAIGN_NAME"    ]);
										       gCAMPAIGN_ID       = Sql.ToGuid  (row["CAMPAIGN_ID"      ]);
										//Guid   gMARKETING_ID      = Sql.ToGuid  (row["MARKETING_ID"     ]);
										//Guid   gLIST_ID           = Sql.ToGuid  (row["LIST_ID"          ]);
										// 12/20/2007 Paul.  We will need the email template to get any attachments. 
										Guid   gEMAIL_TEMPLATE_ID = Sql.ToGuid  (row["EMAIL_TEMPLATE_ID"]);
										string sFROM_ADDR         = Sql.ToString(row["EMAIL_MARKETING_FROM_ADDR"  ]);
										string sFROM_NAME         = Sql.ToString(row["EMAIL_MARKETING_FROM_NAME"  ]);
										string sRETURN_PATH       = Sql.ToString(row["EMAIL_MARKETING_RETURN_PATH"]);
										// 01/20/2008 Paul.  If the from address is not provided by the email campaign, then use the default settings. 
										if ( Sql.IsEmptyString(sFROM_ADDR) )
										{
											sFROM_ADDR = sFromAddress;
											if ( Sql.IsEmptyString(sFROM_NAME) )
												sFROM_NAME = sFromName;
										}
										// 12/20/2007 Paul.  Try and capture invalid emails. 
										MailMessage mail = new MailMessage();
										try
										{
											// 12/27/2007 Paul.  If the From address is invalid, then that should generate a send error, not an invalid email error. 
											// 01/12/2008 Paul.  Populate ReplyTo and Sender using the same values as From. 
											if ( !Sql.IsEmptyString(sFROM_NAME) )
											{
												mail.From    = new MailAddress(sFROM_ADDR, sFROM_NAME);
												mail.Sender  = new MailAddress(sFROM_ADDR, sFROM_NAME);
												mail.ReplyTo = new MailAddress(sFROM_ADDR, sFROM_NAME);
											}
											else
											{
												mail.From    = new MailAddress(sFROM_ADDR);
												mail.Sender  = new MailAddress(sFROM_ADDR);
												mail.ReplyTo = new MailAddress(sFROM_ADDR);
											}
										}
										catch(Exception ex)
										{
											using ( IDbTransaction trn = con.BeginTransaction() )
											{
												try
												{
													SqlProcs.spEMAILMAN_SendFailed(gID, "send error", true, trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
											continue;
										}
										try
										{
											/*
											http://www.regexlib.com/
											Expression :  ^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$
											Description:  The most complete email validation routine I could come up with. It verifies that: - Only letters, numbers and email acceptable symbols (+, _, -, .) are allowed - No two different symbols may follow each other - Cannot begin with a symbol - Ending domain ...
											Matches    :  [g_s+gav@com.com], [gav@gav.com], [jim@jim.c.dc.ca]
											Non-Matches:  [gs_.gs@com.com], [gav@gav.c], [jim@--c.ca]
											*/
											Regex r = new Regex(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");
											if ( !r.Match(sRECIPIENT_EMAIL).Success )
											{
												using ( IDbTransaction trn = con.BeginTransaction() )
												{
													try
													{
														SqlProcs.spEMAILMAN_SendFailed(gID, "invalid email", true, trn);
														trn.Commit();
													}
													catch(Exception ex)
													{
														trn.Rollback();
														throw(new Exception(ex.Message, ex.InnerException));
													}
												}
												continue;
											}
											if ( sRECIPIENT_NAME != String.Empty )
												mail.To.Add(new MailAddress(sRECIPIENT_EMAIL, sRECIPIENT_NAME));
											else
												mail.To.Add(new MailAddress(sRECIPIENT_EMAIL));
										}
										catch(Exception ex)
										{
											using ( IDbTransaction trn = con.BeginTransaction() )
											{
												try
												{
													SqlProcs.spEMAILMAN_SendFailed(gID, "invalid email", true, trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
											continue;
										}
										try
										{
											DataTable dtRelated = Crm.Modules.Parent(Application, sRELATED_TYPE, gRELATED_ID);
											if ( dtRelated.Rows.Count > 0 )
											{
												// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
												sSUBJECT   = EmailUtils.FillEmail(sSUBJECT  , "contact", dtRelated.Rows[0]);
												sBODY_HTML = EmailUtils.FillEmail(sBODY_HTML, "contact", dtRelated.Rows[0]);
											}

											// 12/20/2007 Paul.  We don't watch to cache the trackers for any period of time, just for the particular campaign run. 
											DataTable dtTrackers = hashTrackers[gCAMPAIGN_ID] as DataTable;
											if ( dtTrackers == null )
											{
												dtTrackers = CampaignTrackers(gCAMPAIGN_ID);
												hashTrackers.Add(gCAMPAIGN_ID, dtTrackers);
											}
											DataTable dtAttachments = hashAttachments[gEMAIL_TEMPLATE_ID] as DataTable;
											if ( dtAttachments == null )
											{
												dtAttachments = EmailTemplateAttachments(gEMAIL_TEMPLATE_ID);
												hashAttachments.Add(gEMAIL_TEMPLATE_ID, dtAttachments);
											}

											Guid gTARGET_TRACKER_KEY = Guid.NewGuid();
											sBODY_HTML = EmailUtils.FillTrackers(sBODY_HTML, dtTrackers, sSiteURL, gTARGET_TRACKER_KEY, L10n);

											if ( dtAttachments.Rows.Count > 0 )
											{
												foreach(DataRow rowAttachment in dtAttachments.Rows)
												{
													string sFILENAME           = Sql.ToString(rowAttachment["FILENAME"          ]);
													string sFILE_MIME_TYPE     = Sql.ToString(rowAttachment["FILE_MIME_TYPE"    ]);
													Guid   gNOTE_ATTACHMENT_ID = Sql.ToGuid  (rowAttachment["NOTE_ATTACHMENT_ID"]);
													
													MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
													if ( mem == null )
													{
														// 07/30/2006 Paul.  We cannot close the streams until the message is sent. 
														mem = new MemoryStream();
														BinaryWriter writer = new BinaryWriter(mem);
														Notes.Attachment.WriteStream(gNOTE_ATTACHMENT_ID, con, writer);
														writer.Flush();
														mem.Seek(0, SeekOrigin.Begin);
														hashNoteStreams.Add(gNOTE_ATTACHMENT_ID, mem);
													}
													
													Attachment att = new Attachment(mem, sFILENAME, sFILE_MIME_TYPE);
													mail.Attachments.Add(att);
												}
											}

											mail.Subject      = sSUBJECT;
											mail.Body         = sBODY_HTML;
											mail.BodyEncoding = System.Text.Encoding.UTF8;
											mail.IsBodyHtml   = true;
											mail.Headers.Add("X-SplendidCRM-ID", gTARGET_TRACKER_KEY.ToString());
											mail.Headers.Add("X-Mailer", "SplendidCRM");
											if ( !Sql.IsEmptyString(sRETURN_PATH) )
											{
												// 12/21/2007 Paul.  Return-Path may not work with Exchange Server any more.
												mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
												mail.Headers.Add("Return-Path", sRETURN_PATH);
											}
											client.Send(mail);

											using ( IDbTransaction trn = con.BeginTransaction() )
											{
												try
												{
													Guid gEMAIL_ID = Guid.Empty;
													// 01/13/2008 Paul.  The email manager is also being used for AutoReplies, so the campaign might not exist. 
													if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
													{
														// 01/13/2008 Paul.  Since the Plug-in saves body in DESCRIPTION, we need to continue to use it as the primary source of data. 
														SqlProcs.spEMAILS_CampaignRef(ref gEMAIL_ID, sCAMPAIGN_NAME + ": " + sSUBJECT, sRELATED_TYPE, gRELATED_ID, sBODY_HTML, String.Empty, sFROM_ADDR, sFROM_NAME, sRECIPIENT_NAME + " <" + sRECIPIENT_EMAIL + ">", gRELATED_ID.ToString(), sRECIPIENT_NAME, sRECIPIENT_EMAIL, "campaign", "sent", sRELATED_TYPE, gRELATED_ID, trn);
													}
													SqlProcs.spEMAILMAN_SendSuccessful(gID, gTARGET_TRACKER_KEY, gEMAIL_ID, trn);
													trn.Commit();
												}
												catch(Exception ex)
												{
													trn.Rollback();
													throw(new Exception(ex.Message, ex.InnerException));
												}
											}
										}
										catch(Exception ex)
										{
											using ( IDbTransaction trn = con.BeginTransaction() )
											{
												try
												{
													SqlProcs.spEMAILMAN_SendFailed(gID, "send error", false, trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
										}
										
										// 12/20/2007 Paul.  We need to protect against caching too much. 
										// If the total cached size is > 100M, then close all streams and clear the cache. 
										long lTotalCachedSize = 0;
										foreach ( Guid gNOTE_ATTACHMENT_ID in hashNoteStreams.Keys )
										{
											MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
											if ( mem != null )
												lTotalCachedSize += mem.Length;
										}
										// 12/20/2007 Paul.  In an attempt to be efficient, if we are only caching one big file, then don't flush it. 
										if ( hashNoteStreams.Count > 1 && lTotalCachedSize > 100 * 1024 * 1024 )
										{
											foreach ( Guid gNOTE_ATTACHMENT_ID in hashNoteStreams.Keys )
											{
												MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
												if ( mem != null )
													mem.Close();
											}
											hashNoteStreams.Clear();
										}
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					// 12/20/2007 Paul.  We should close the streams manually to help with garbage collection. 
					foreach ( Guid gNOTE_ATTACHMENT_ID in hashNoteStreams.Keys )
					{
						MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
						if ( mem != null )
							mem.Close();
					}
					hashTrackers   .Clear();
					hashAttachments.Clear();
					hashNoteStreams.Clear();
					bInsideSendQueue = false;
				}
			}
		}

		public static string L10n_Term(HttpApplicationState Application, string sEntryName)
		{
			// 01/11/2008 Paul.  Protect against uninitialized variables. 
			if ( String.IsNullOrEmpty(sEntryName) )
				return String.Empty;

			// 01/13/2008 Paul.  Lookup the default culture. 
			string NAME = SplendidDefaults.Culture(Application);
			object oDisplayName = Application[NAME + "." + sEntryName];
			if ( oDisplayName == null )
			{
				// 01/11/2007 Paul.  Default to English if term not found. 
				// There are just too many untranslated terms when importing a SugarCRM Language Pack. 
				oDisplayName = Application["en-US." + sEntryName];
				if ( oDisplayName == null )
				{
					return sEntryName;
				}
			}
			return oDisplayName.ToString();
		}

		private static string[] arrTrackers = new string[] { "/RemoveMe.aspx?identifier=", "/campaign_trackerv2.aspx?identifier=", "/image.aspx?identifier=" };

		public static Guid FindTargetTrackerKey(Pop3.RxMailMessage mm)
		{
			Guid gTARGET_TRACKER_KEY = Sql.ToGuid(mm.Headers["x-splendidcrm-id"]);
			if ( Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
			{
				// 01/13/2008 Paul.  Now look for a RemoveMe tracker, or any of the other expected trackers. 
				if ( mm.Body != null )
				{
					foreach ( string sTracker in arrTrackers )
					{
						int nStartTracker = mm.Body.IndexOf(sTracker);
						if ( nStartTracker > 0 )
						{
							nStartTracker += sTracker.Length;
							gTARGET_TRACKER_KEY = Sql.ToGuid(mm.Body.Substring(nStartTracker, 36));
							if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
								return gTARGET_TRACKER_KEY;
						}
					}
				}
				foreach ( AlternateView vw in mm.AlternateViews )
				{
					if ( vw.TransferEncoding == TransferEncoding.SevenBit || vw.TransferEncoding == TransferEncoding.Base64 )
					{
						Encoding enc = new System.Text.ASCIIEncoding();
						switch ( vw.ContentType.CharSet )
						{
							case "UTF-8" :  enc = new System.Text.UTF8Encoding   ();  break;
							case "UTF-16":  enc = new System.Text.UnicodeEncoding();  break;
							case "UTF-32":  enc = new System.Text.UTF32Encoding  ();  break;
						}
						StreamReader rdr = new StreamReader(vw.ContentStream, enc);
						string sBody = rdr.ReadToEnd();
						foreach ( string sTracker in arrTrackers )
						{
							int nStartTracker = sBody.IndexOf(sTracker);
							if ( nStartTracker > 0 )
							{
								nStartTracker += sTracker.Length;
								gTARGET_TRACKER_KEY = Sql.ToGuid(sBody.Substring(nStartTracker, 36));
								if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
									return gTARGET_TRACKER_KEY;
							}
						}
					}
				}
				// 01/20/2008 Paul.  In a bounce, the server messages will be stored in entities. 
				foreach ( Pop3.RxMailMessage ent in mm.Entities )
				{
					// text/plain
					// message/delivery-status
					// message/rfc822
					if ( ent.ContentType.MediaType == "text/plain" || ent.ContentType.MediaType == "message/rfc822" )
					{
						gTARGET_TRACKER_KEY = FindTargetTrackerKey(ent);
						if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
							return gTARGET_TRACKER_KEY;
					}
				}
			}
			return gTARGET_TRACKER_KEY;
		}

		public static void ImportInboundEmail(HttpApplicationState Application, IDbConnection con, Pop3.RxMailMessage mm, Guid gMAILBOX_ID, string sINTENT, Guid gGROUP_ID)
		{
			using ( IDbTransaction trn = con.BeginTransaction() )
			{
				try
				{
					string sEMAIL_TYPE = "inbound";
					string sSTATUS     = "unread";

					StringBuilder sbTO_ADDRS        = new StringBuilder();
					StringBuilder sbTO_ADDRS_NAMES  = new StringBuilder();
					StringBuilder sbTO_ADDRS_EMAILS = new StringBuilder();
					if ( mm.To != null )
					{
						foreach ( MailAddress addr in mm.To )
						{
							// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
							sbTO_ADDRS.Append((sbTO_ADDRS.Length > 0) ? "; " : String.Empty);
							sbTO_ADDRS.Append(addr.ToString());

							sbTO_ADDRS_NAMES.Append((sbTO_ADDRS_NAMES.Length > 0) ? "; " : String.Empty);
							sbTO_ADDRS_NAMES.Append(!Sql.IsEmptyString(addr.DisplayName) ? addr.DisplayName : addr.Address);

							sbTO_ADDRS_EMAILS.Append((sbTO_ADDRS_EMAILS.Length > 0) ? "; " : String.Empty);
							sbTO_ADDRS_EMAILS.Append(addr.Address);
						}
					}

					StringBuilder sbCC_ADDRS        = new StringBuilder();
					StringBuilder sbCC_ADDRS_NAMES  = new StringBuilder();
					StringBuilder sbCC_ADDRS_EMAILS = new StringBuilder();
					if ( mm.CC != null )
					{
						foreach ( MailAddress addr in mm.CC )
						{
							// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
							sbCC_ADDRS.Append((sbCC_ADDRS.Length > 0) ? "; " : String.Empty);
							sbCC_ADDRS.Append(addr.ToString());

							sbCC_ADDRS_NAMES.Append((sbCC_ADDRS_NAMES.Length > 0) ? "; " : String.Empty);
							sbCC_ADDRS_NAMES.Append(!Sql.IsEmptyString(addr.DisplayName) ? addr.DisplayName : addr.Address);

							sbCC_ADDRS_EMAILS.Append((sbCC_ADDRS_EMAILS.Length > 0) ? "; " : String.Empty);
							sbCC_ADDRS_EMAILS.Append(addr.Address);
						}
					}

					StringBuilder sbBCC_ADDRS        = new StringBuilder();
					StringBuilder sbBCC_ADDRS_NAMES  = new StringBuilder();
					StringBuilder sbBCC_ADDRS_EMAILS = new StringBuilder();
					if ( mm.Bcc != null )
					{
						foreach ( MailAddress addr in mm.Bcc )
						{
							// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
							sbBCC_ADDRS.Append((sbBCC_ADDRS.Length > 0) ? "; " : String.Empty);
							sbBCC_ADDRS.Append(addr.ToString());

							sbBCC_ADDRS_NAMES.Append((sbBCC_ADDRS_NAMES.Length > 0) ? "; " : String.Empty);
							sbBCC_ADDRS_NAMES.Append(!Sql.IsEmptyString(addr.DisplayName) ? addr.DisplayName : addr.Address);

							sbBCC_ADDRS_EMAILS.Append((sbBCC_ADDRS_EMAILS.Length > 0) ? "; " : String.Empty);
							sbBCC_ADDRS_EMAILS.Append(addr.Address);
						}
					}

					Guid gID = Guid.Empty;
					// 01/13/2008 Paul.  First look for our special header. 
					// Our special header will only exist if the email is a bounce. 
					Guid gTARGET_TRACKER_KEY = Guid.Empty;
					// 01/13/2008 Paul.  The header will always be in lower case. 
					gTARGET_TRACKER_KEY = FindTargetTrackerKey(mm);
					// 01/20/2008 Paul.  mm.DeliveredTo can be NULL. 
					// 01/20/2008 Paul.  Filter the XSS tags before inserting the email. 
					// 01/23/2008 Paul.  DateTime in the email is in universal time. 
					string sSAFE_BODY = EmailUtils.XssFilter(mm.Body, Sql.ToString(Application["CONFIG.email_xss"]));
					SqlProcs.spEMAILS_InsertInbound
						( ref gID
						, gGROUP_ID
						, mm.Subject
						, mm.DeliveryDate.ToLocalTime()
						, sSAFE_BODY
						, String.Empty
						, ((mm.From != null) ? mm.From.Address    : String.Empty)
						, ((mm.From != null) ? mm.From.ToString() : String.Empty)
						, sbTO_ADDRS.ToString()
						, sbCC_ADDRS.ToString()
						, sbBCC_ADDRS.ToString()
						, sbTO_ADDRS_NAMES  .ToString()
						, sbTO_ADDRS_EMAILS .ToString()
						, sbCC_ADDRS_NAMES  .ToString()
						, sbCC_ADDRS_EMAILS .ToString()
						, sbBCC_ADDRS_NAMES .ToString()
						, sbBCC_ADDRS_EMAILS.ToString()
						, sEMAIL_TYPE
						, sSTATUS
						, mm.MessageId + ((mm.DeliveredTo != null && mm.DeliveredTo.Address != null) ? mm.DeliveredTo.Address : String.Empty)
						, ((mm.ReplyTo != null) ? mm.ReplyTo.ToString() : String.Empty)
						, ((mm.ReplyTo != null) ? mm.ReplyTo.Address    : String.Empty)
						, sINTENT
						, gMAILBOX_ID
						, gTARGET_TRACKER_KEY
						, mm.RawContent
						, trn
						);
					
					// 01/20/2008 Paul.  In a bounce, the server messages will be stored in entities. 
					foreach ( Pop3.RxMailMessage ent in mm.Entities )
					{
						// text/plain
						// message/delivery-status
						// message/rfc822
						// 01/20/2008 Paul.  Most server status reports will not have a subject, so use the first 300 characters, but take out the CRLF. 
						// 01/21/2008 Paul.  Substring will throw an exception if request exceeds length. 
						if ( Sql.IsEmptyString(ent.Subject) && !Sql.IsEmptyString(ent.Body) )
							ent.Subject = ent.Body.Substring(0, Math.Min(ent.Body.Length, 300)).Replace("\r\n", " ");
						Guid gNOTE_ID = Guid.Empty;
						SqlProcs.spNOTES_Update
							( ref gNOTE_ID
							, mm.ContentType.MediaType + ": " + ent.Subject
							, "Emails"   // Parent Type
							, gID        // Parent ID
							, Guid.Empty
							, ent.Body
							, Guid.Empty // TEAM_ID
							, trn
							);

					}
					foreach ( Attachment att in mm.Attachments )
					{
						// 01/13/2008 Paul.  We may need to convert the encoding to UTF8. 
						// att.NameEncoding;
						string sFILENAME       = Path.GetFileName (att.Name);
						string sFILE_EXT       = Path.GetExtension(sFILENAME);
						string sFILE_MIME_TYPE = att.ContentType.MediaType;
					
						Guid gNOTE_ID = Guid.Empty;
						SqlProcs.spNOTES_Update
							( ref gNOTE_ID
							, L10n_Term(Application, "Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
							, "Emails"   // Parent Type
							, gID        // Parent ID
							, Guid.Empty
							, att.ContentId
							, Guid.Empty        // TEAM_ID
							, trn
							);

						Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
						// 01/20/2006 Paul.  Must include in transaction
						SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, att.Name, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
						Notes.EditView.LoadFile(gNOTE_ATTACHMENT_ID, att.ContentStream, trn);
					}
					trn.Commit();
				}
				catch(Exception ex)
				{
					trn.Rollback();
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
			}
		}

		public static void CheckInbound(HttpApplicationState Application, Guid gID, bool bBounce)
		{
			if ( !bInsideCheckInbound )
			{
				bInsideCheckInbound = true;
				try
				{
					bool bEMAIL_INBOUND_SAVE_RAW = Sql.ToBoolean(Application["CONFIG.email_inbound_save_raw"]);
					Guid gINBOUND_EMAIL_KEY      = Sql.ToGuid   (Application["CONFIG.InboundEmailKey"       ]);
					Guid gINBOUND_EMAIL_IV       = Sql.ToGuid   (Application["CONFIG.InboundEmailIV"        ]);
					DataView vwINBOUND_EMAILS_Inbound = null;
					if ( bBounce )
						vwINBOUND_EMAILS_Inbound = new DataView(SplendidCache.InboundEmailBounce());
					else
						vwINBOUND_EMAILS_Inbound = new DataView(SplendidCache.InboundEmailMonitored());

					if ( !Sql.IsEmptyGuid(gID) )
						vwINBOUND_EMAILS_Inbound.RowFilter = "ID = '" + gID.ToString() + "'";
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select count(*)                " + ControlChars.CrLf
						     + "  from vwEMAILS                " + ControlChars.CrLf
						     + " where MESSAGE_ID = @MESSAGE_ID" + ControlChars.CrLf;
						using ( IDbCommand cmdExistingEmails = con.CreateCommand() )
						{
							cmdExistingEmails.CommandText = sSQL;
							IDbDataParameter parMESSAGE_ID = Sql.AddParameter(cmdExistingEmails, "@MESSAGE_ID", String.Empty, 1000);
							foreach ( DataRowView rowInbound in vwINBOUND_EMAILS_Inbound )
							{
								// 01/13/2008 Paul.  The MAILBOX_ID is the ID for the INBOUND_EMAIL record. 
								Guid   gMAILBOX_ID     = Sql.ToGuid   (rowInbound["ID"            ]);
								Guid   gGROUP_ID       = Sql.ToGuid   (rowInbound["GROUP_ID"      ]);
								string sMAILBOX_TYPE   = Sql.ToString (rowInbound["MAILBOX_TYPE"  ]);
								string sSERVER_URL     = Sql.ToString (rowInbound["SERVER_URL"    ]);
								string sEMAIL_USER     = Sql.ToString (rowInbound["EMAIL_USER"    ]);
								string sEMAIL_PASSWORD = Sql.ToString (rowInbound["EMAIL_PASSWORD"]);
								int    nPORT           = Sql.ToInteger(rowInbound["PORT"          ]);
								string sSERVICE        = Sql.ToString (rowInbound["SERVICE"       ]);
								bool   bMAILBOX_SSL    = Sql.ToBoolean(rowInbound["MAILBOX_SSL"   ]);
								bool   bMARK_READ      = Sql.ToBoolean(rowInbound["MARK_READ"     ]);
								bool   bONLY_SINCE     = Sql.ToBoolean(rowInbound["ONLY_SINCE"    ]);

								// 01/08/2008 Paul.  Decrypt at the last minute to ensure that an unencrypted password is never sent to the browser. 
								sEMAIL_PASSWORD = Security.DecryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);

								Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
								try
								{
									pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
									pop.Connect();
									
									int nTotalEmails = 0;
									int mailboxSize  = 0;
									pop.GetMailboxStats(out nTotalEmails, out mailboxSize);

									List<int> arrEmailIds = new List<int>();
									pop.GetEmailIdList(out arrEmailIds);
									foreach ( int i in arrEmailIds )
									{
										Pop3.RxMailMessage mm = null;
										try
										{
											pop.IsCollectRawEmail = bEMAIL_INBOUND_SAVE_RAW;
											pop.GetHeaders(i, out mm);
											if ( mm != null )
											{
												// 01/13/2008 Paul.  Bounce processing only applies if sent by the mailer daemon. 
												// 01/13/2008 Paul.  MS Exchange Server uses postmaster. 
												bool bMailerDaemon = mm.From.Address.IndexOf("mailer-daemon@") >= 0 || mm.From.Address.IndexOf("postmaster@") >= 0;
												if ( (bBounce && bMailerDaemon) || (!bBounce && !bMailerDaemon) )
												{
													// 01/12/2008 Paul.  Lookup the message to see if we need to import it. 
													// SugarCRM: The uniqueness of a given email message is determined by a concatenationof 2 values, 
													// SugarCRM: the messageID and the delivered-to field.  This allows multiple To: and B/CC: destination 
													// SugarCRM: addresses to be imported by Sugar without violating the true duplicate-email issues.
													// 01/20/2008 Paul.  mm.DeliveredTo can be NULL. 
													parMESSAGE_ID.Value = mm.MessageId + ((mm.DeliveredTo != null && mm.DeliveredTo.Address != null) ? mm.DeliveredTo.Address : String.Empty);
													if ( Sql.ToInteger(cmdExistingEmails.ExecuteScalar()) == 0 )
													{
														mm = null;
														pop.GetEmail(i, out mm);
														// 01/13/2008 Paul.  Pull POP3 logic out of import function so that it can be reused by IMAP4 driver. 
														ImportInboundEmail(Application, con, mm, gMAILBOX_ID, sMAILBOX_TYPE, gGROUP_ID);
														if ( !bMARK_READ )
															pop.DeleteEmail(i);
													}
												}
											}
										}
										finally
										{
											// 01/13/2008 Paul.  We may need to be more efficient about garbage cleanup as an email can contain a large attachment. 
											mm = null;
										}
									}
								}
								finally
								{
									pop.Disconnect();
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideCheckInbound = false;
				}
			}
		}

		public static void CheckBounced(HttpApplicationState Application, Guid gID)
		{
			CheckInbound(Application, gID, true);
		}

		public static void CheckMonitored(HttpApplicationState Application, Guid gID)
		{
			CheckInbound(Application, gID, false);
		}
	}
}
