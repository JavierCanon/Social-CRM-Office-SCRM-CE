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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.InboundEmail
{
	/// <summary>
	///		Summary description for Mailbox.
	/// </summary>
	public class Mailbox : SplendidControl
	{
		protected Guid            gID            ;
		protected DataTable       dtMain         ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected Label           lblError       ;
		protected StringBuilder   sbTrace        ;

		protected void Pop3Trace(string sText)
		{
			sbTrace.Append(sText + ControlChars.CrLf);
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				sbTrace = new StringBuilder();
				if ( e.CommandName == "Mailbox.CheckBounce" )
				{
					EmailUtils.CheckInbound(Application, gID, true);
				}
				if ( e.CommandName == "Mailbox.CheckMail" || e.CommandName == "Mailbox.CheckBounce" )
				{
					string sSERVER_URL     = Sql.ToString (ViewState["SERVER_URL"    ]);
					string sEMAIL_USER     = Sql.ToString (ViewState["EMAIL_USER"    ]);
					string sEMAIL_PASSWORD = Sql.ToString (ViewState["EMAIL_PASSWORD"]);
					int    nPORT           = Sql.ToInteger(ViewState["PORT"          ]);
					string sSERVICE        = Sql.ToString (ViewState["SERVICE"       ]);
					bool   bMAILBOX_SSL    = Sql.ToBoolean(ViewState["MAILBOX_SSL"   ]);

					// 01/08/2008 Paul.  Decrypt at the last minute to ensure that an unencrypted password is never sent to the browser. 
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					sEMAIL_PASSWORD = Security.DecryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);

					dtMain = new DataTable();
					dtMain.Columns.Add("From"        , typeof(System.String  ));
					dtMain.Columns.Add("Sender"      , typeof(System.String  ));
					dtMain.Columns.Add("ReplyTo"     , typeof(System.String  ));
					dtMain.Columns.Add("To"          , typeof(System.String  ));
					dtMain.Columns.Add("CC"          , typeof(System.String  ));
					dtMain.Columns.Add("Bcc"         , typeof(System.String  ));
					dtMain.Columns.Add("Subject"     , typeof(System.String  ));
					dtMain.Columns.Add("DeliveryDate", typeof(System.DateTime));
					dtMain.Columns.Add("Priority"    , typeof(System.String  ));
					dtMain.Columns.Add("Size"        , typeof(System.Int32   ));
					dtMain.Columns.Add("ContentID"   , typeof(System.String  ));
					dtMain.Columns.Add("MessageID"   , typeof(System.String  ));
					dtMain.Columns.Add("Headers"     , typeof(System.String  ));
					dtMain.Columns.Add("Body"        , typeof(System.String  ));

					Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
					try
					{
						pop.Trace += new Pop3.TraceHandler(this.Pop3Trace);
						pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
						pop.Connect();
						
						int nTotalEmails = 0;
						int mailboxSize  = 0;
						pop.GetMailboxStats(out nTotalEmails, out mailboxSize);

						List<int> arrEmailIds = new List<int>();
						pop.GetEmailIdList(out arrEmailIds);
						foreach ( int i in arrEmailIds )
						{
							int nEmailSize = pop.GetEmailSize(i);
							if ( nEmailSize < 1 * 1024 * 1024 )
							{
								Pop3.RxMailMessage mm = null;
#if DEBUG
								pop.IsCollectRawEmail = true;
#endif
								pop.GetHeaders(i, out mm);
								if ( mm == null )
								{
									sbTrace.Append("Email " + i.ToString() + " cannot be displayed." + ControlChars.CrLf);
								}
								else
								{
									DataRow row = dtMain.NewRow();
									dtMain.Rows.Add(row);
									row["From"        ] = Server.HtmlEncode(Sql.ToString(mm.From   ));
									row["Sender"      ] = Server.HtmlEncode(Sql.ToString(mm.Sender ));
									row["ReplyTo"     ] = Server.HtmlEncode(Sql.ToString(mm.ReplyTo));
									row["To"          ] = Server.HtmlEncode(Sql.ToString(mm.To     ));
									row["CC"          ] = Server.HtmlEncode(Sql.ToString(mm.CC     ));
									row["Bcc"         ] = Server.HtmlEncode(Sql.ToString(mm.Bcc    ));
									row["Subject"     ] = Server.HtmlEncode(mm.Subject);
									// 01/23/2008 Paul.  DateTime in the email is in universal time. 
									row["DeliveryDate"] = T10n.FromUniversalTime(mm.DeliveryDate);
									row["Priority"    ] = mm.Priority.ToString();
									row["Size"        ] = nEmailSize     ;
									row["ContentId"   ] = mm.ContentId   ;
									row["MessageId"   ] = mm.MessageId   ;
									row["Headers"     ] = "<pre>" + Server.HtmlEncode(mm.RawContent) + "</pre>";
									//row["Body"        ] = mm.Body        ;
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					finally
					{
						pop.Disconnect();
					}
					ViewState["Inbox"] = dtMain;
					vwMain = new DataView(dtMain);
					grdMain.DataSource = vwMain;
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
			finally
			{
#if DEBUG
				RegisterClientScriptBlock("Pop3Trace", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(sbTrace.ToString()) + "';</script>");
#endif
				}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						sSQL = "select *                    " + ControlChars.CrLf
						     + "  from vwINBOUND_EMAILS_Edit" + ControlChars.CrLf
						     + " where ID = @ID             " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							con.Open();

							if ( bDebug )
								RegisterClientScriptBlock("vwINBOUND_EMAILS_Edit", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									string sSERVER_URL     = Sql.ToString (rdr["SERVER_URL"    ]);
									string sEMAIL_USER     = Sql.ToString (rdr["EMAIL_USER"    ]);
									string sEMAIL_PASSWORD = Sql.ToString (rdr["EMAIL_PASSWORD"]);
									int    nPORT           = Sql.ToInteger(rdr["PORT"          ]);
									string sSERVICE        = Sql.ToString (rdr["SERVICE"       ]);
									bool   bMAILBOX_SSL    = Sql.ToBoolean(rdr["MAILBOX_SSL"   ]);
									
									ViewState["SERVER_URL"    ] = sSERVER_URL    ;
									ViewState["EMAIL_USER"    ] = sEMAIL_USER    ;
									ViewState["EMAIL_PASSWORD"] = sEMAIL_PASSWORD;
									ViewState["PORT"          ] = nPORT          ;
									ViewState["SERVICE"       ] = sSERVICE       ;
									ViewState["MAILBOX_SSL"   ] = bMAILBOX_SSL   ;
								}
							}
						}
					}
				}
				else
				{
					if ( ViewState["Inbox"] != null )
					{
						dtMain = ViewState["Inbox"] as DataTable;
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
