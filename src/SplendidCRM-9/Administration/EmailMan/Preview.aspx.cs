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
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.EmailMan
{
	/// <summary>
	/// Summary description for Preview.
	/// </summary>
	public class Preview : SplendidPage
	{
		protected _controls.ModuleHeader  ctlModuleHeader;
		protected Guid          gID              ;
		protected Label         lblError         ;
		protected Label         txtSEND_DATE_TIME;
		protected Label         txtFROM          ;
		protected Label         txtTO            ;
		protected Label         txtSUBJECT       ;
		protected Label         txtBODY_HTML     ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Delete" )
				{
					SqlProcs.spEMAILMAN_Delete(gID);
					// 12/20/2007 Paul.  Use RegisterStartupScript so that the rest of the page is rendered before the code is run. 
					Page.ClientScript.RegisterStartupScript(System.Type.GetType("System.String"), "UpdateParent", "<script type=\"text/javascript\">UpdateParent();</script>");
				}
				else if ( e.CommandName == "Send" )
				{
					EmailUtils.SendQueued(Application, gID, Guid.Empty, true);
					Page.ClientScript.RegisterStartupScript(System.Type.GetType("System.String"), "UpdateParent", "<script type=\"text/javascript\">UpdateParent();</script>");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("EmailMan.LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				Page.DataBind();
				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							// 01/12/2008 Paul.  Preview is different in that it does not filter on queue date. 
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwEMAILMAN_Preview" + ControlChars.CrLf
							     + " where 1 = 1             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
								{
									#pragma warning disable 618
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "SQLCode", Sql.ClientScriptBlock(cmd));
									#pragma warning restore 618
								}

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["EMAIL_MARKETING_NAME"]) + " - " + Sql.ToString(rdr["RECIPIENT_NAME"]);
										SetPageTitle(ctlModuleHeader.Title);
										
										string sRECIPIENT_NAME  = Sql.ToString(rdr["RECIPIENT_NAME" ]);
										string sRECIPIENT_EMAIL = Sql.ToString(rdr["RECIPIENT_EMAIL"]);
										string sSUBJECT         = Sql.ToString(rdr["SUBJECT"        ]);
										string sBODY_HTML       = Sql.ToString(rdr["BODY_HTML"      ]);
										string sRELATED_TYPE    = Sql.ToString(rdr["RELATED_TYPE"   ]);
										Guid   gRELATED_ID      = Sql.ToGuid  (rdr["RELATED_ID"     ]);
										string sCAMPAIGN_NAME   = Sql.ToString(rdr["CAMPAIGN_NAME"  ]);
										Guid   gCAMPAIGN_ID     = Sql.ToGuid  (rdr["CAMPAIGN_ID"    ]);
										Guid   gMARKETING_ID    = Sql.ToGuid  (rdr["MARKETING_ID"   ]);
										string sFROM_ADDR       = Sql.ToString(rdr["EMAIL_MARKETING_FROM_ADDR"]);
										string sFROM_NAME       = Sql.ToString(rdr["EMAIL_MARKETING_FROM_NAME"]);

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
											lblError.Text = L10n.Term("Campaigns.LBL_LOG_ENTRIES_INVALID_EMAIL_TITLE");
										}
										
										DataTable dtRelated = Crm.Modules.Parent(sRELATED_TYPE, gRELATED_ID);
										if ( dtRelated.Rows.Count > 0 )
										{
											// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
											sSUBJECT   = EmailUtils.FillEmail(sSUBJECT  , "contact", dtRelated.Rows[0]);
											sBODY_HTML = EmailUtils.FillEmail(sBODY_HTML, "contact", dtRelated.Rows[0]);
										}

										Guid   gTARGET_TRACKER_KEY = Guid.NewGuid();
										string sSiteURL            = Utils.MassEmailerSiteURL(Application);
										DataTable dtTrackers = EmailUtils.CampaignTrackers(gCAMPAIGN_ID);
										sBODY_HTML = EmailUtils.FillTrackers(sBODY_HTML, dtTrackers, sSiteURL, gTARGET_TRACKER_KEY, L10n);

										txtSEND_DATE_TIME.Text = Sql.ToDateTime(rdr["SEND_DATE_TIME"]).ToString();
										txtFROM          .Text = sFROM_NAME      + " &lt;" + sFROM_ADDR       + "&gt;";
										txtTO            .Text = sRECIPIENT_NAME + " &lt;" + sRECIPIENT_EMAIL + "&gt;";
										txtSUBJECT       .Text = sSUBJECT  ;
										txtBODY_HTML     .Text = sBODY_HTML;
									}
								}
							}
						}
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
