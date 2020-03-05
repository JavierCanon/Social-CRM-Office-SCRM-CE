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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Emails
{
	/// <summary>
	/// Summary description for InboundView.
	/// </summary>
	public class InboundView : SplendidControl
	{
		protected SplendidCRM._controls.ModuleHeader ctlModuleHeader  ;
		protected Emails._controls.InboundButtons    ctlInboundButtons;

		protected Guid        gID              ;
		protected HtmlTable   tblMain          ;
		protected PlaceHolder plcSubPanel      ;
		protected Repeater    ctlAttachments   ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Forward" )
				{
					Response.Redirect("edit.aspx?type=forward&DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Reply" )
				{
					Response.Redirect("edit.aspx?type=reply&DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Reply All" )
				{
					Response.Redirect("edit.aspx?type=replyall&DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spEMAILS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "ShowRaw" )
				{
				}
				else if ( e.CommandName == "HideRaw" )
				{
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlInboundButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *            " + ControlChars.CrLf
							     + "  from vwEMAILS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "view");
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										
										this.AppendDetailViewFields(m_sMODULE + ".DetailView", tblMain, rdr);
										
										// 11/17/2005 Paul.  Archived emails allow editing of the Date & Time Sent. 
										string sEMAIL_TYPE = Sql.ToString(rdr["TYPE"]).ToLower();
										ctlModuleHeader.EnableModuleLabel = false;
										switch ( sEMAIL_TYPE )
										{
											case "archived":
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_ARCHIVED_MODULE_NAME") + ":" + ctlModuleHeader.Title;
												Response.Redirect("view.aspx?ID=" + gID.ToString());
												break;
											case "inbound":
												// 06/28/2007 Paul.  Inbound emails should not automatically go to edit mode. 
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_INBOUND_TITLE") + ":" + ctlModuleHeader.Title;
												break;
											case "out":
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + ":" + ctlModuleHeader.Title;
												Response.Redirect("view.aspx?ID=" + gID.ToString());
												break;
											case "sent":
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + ":" + ctlModuleHeader.Title;
												Response.Redirect("view.aspx?ID=" + gID.ToString());
												break;
											case "campaign":
												// 01/13/2008 Paul.  Campaign emails should be treated the same as outbound emails. 
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + ":" + ctlModuleHeader.Title;
												Response.Redirect("view.aspx?ID=" + gID.ToString());
												break;
											default:
												sEMAIL_TYPE = "draft";
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_COMPOSE_MODULE_NAME" ) + ":" + ctlModuleHeader.Title;
												// 01/21/2006 Paul.  Draft messages go directly to edit mode. 
												Response.Redirect("edit.aspx?ID=" + gID.ToString());
												break;
										}
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't show its sub panel either. 
										plcSubPanel.Visible = false;
										ctlInboundButtons.DisableAll();
										ctlInboundButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
							sSQL = "select *                   " + ControlChars.CrLf
							     + "  from vwEMAILS_Attachments" + ControlChars.CrLf
							     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@EMAIL_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwEMAILS_Attachments", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										ctlAttachments.DataSource = dt.DefaultView;
										ctlAttachments.DataBind();
									}
								}
							}
						}
					}
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlInboundButtons.ErrorText = ex.Message;
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
			ctlInboundButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Emails";
			// 02/13/2007 Paul.  Emails should highlight the Activities menu. 
			// 05/26/2007 Paul.  We are display the emails tab, so we must highlight the tab. 
			SetMenu(m_sMODULE);
			this.AppendDetailViewRelationships(m_sMODULE + ".DetailView", plcSubPanel);
		}
		#endregion
	}
}
