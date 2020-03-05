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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using FredCK.FCKeditorV2;

namespace SplendidCRM.Posts
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected HiddenField     THREAD_ID                    ;
		protected TextBox         txtTITLE                     ;
		protected FCKeditor       txtDESCRIPTION               ;
		protected RequiredFieldValidator reqTITLE;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			Guid gTHREAD_ID = Sql.ToGuid(THREAD_ID.Value);
			if ( e.CommandName == "Save" )
			{
				reqTITLE.Enabled = true;
				reqTITLE.Validate();
				if ( Page.IsValid )
				{
					try
					{
						SqlProcs.spPOSTS_Update(ref gID
							, gTHREAD_ID
							, txtTITLE.Text
							, txtDESCRIPTION.Value
							);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlEditButtons.ErrorText = ex.Message;
						return;
					}
					if ( !Sql.IsEmptyGuid(gTHREAD_ID) )
						Response.Redirect("~/Threads/view.aspx?ID=" + gTHREAD_ID.ToString());
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gTHREAD_ID) )
					Response.Redirect("~/Threads/view.aspx?ID=" + gTHREAD_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwPOSTS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["TITLE"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										THREAD_ID.Value = Sql.ToString(rdr["THREAD_ID"]);
										txtTITLE.Text        = Sql.ToString (rdr["TITLE"           ]);
										txtDESCRIPTION.Value = Sql.ToString (rdr["DESCRIPTION_HTML"]);
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't allow save 
										ctlEditButtons.DisableAll();
										ctlEditButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
						}
					}
					else
					{
						Guid gREPLY_ID  = Sql.ToGuid(Request["REPLY_ID" ]);
						Guid gTHREAD_ID = Sql.ToGuid(Request["THREAD_ID"]);
						THREAD_ID.Value = gTHREAD_ID.ToString();

						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							if ( !Sql.IsEmptyGuid(gTHREAD_ID) )
							{
								sSQL = "select *              " + ControlChars.CrLf
								     + "  from vwTHREADS_Edit " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
									Security.Filter(cmd, "Threads", "view");
									Sql.AppendParameter(cmd, gTHREAD_ID, "ID", false);
									con.Open();

									if ( bDebug )
										RegisterClientScriptBlock("vwTHREADS_Edit", Sql.ClientScriptBlock(cmd));

									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											ctlModuleHeader.Title = Sql.ToString(rdr["TITLE"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
											ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

											THREAD_ID.Value = gTHREAD_ID.ToString();
											txtTITLE.Text = L10n.Term("Posts.LBL_REPLY_PREFIX") + Sql.ToString (rdr["TITLE"]);
											if ( Sql.ToBoolean(Request["QUOTE"]) )
											{
												string sCREATED_BY    = Sql.ToString(rdr["CREATED_BY"      ]);
												string sORIGINAL_TEXT = Sql.ToString(rdr["DESCRIPTION_HTML"]);
												string sQUOTE_FORMAT  = L10n.Term("Posts.QUOTE_FORMAT");
												StringBuilder sb = new StringBuilder();
												sb.Append("<br />"           + ControlChars.CrLf);
												sb.Append("<br />"           + ControlChars.CrLf);
												sb.Append("<strong><em>" + String.Format(sQUOTE_FORMAT, sCREATED_BY) + "</em></strong> <br />" + ControlChars.CrLf);
												sb.Append("<table width=\"90%\" border=\"1\" cellspacing=\"0\" cellpadding=\"4\" style=\"FONT-STYLE: italic\" align=\"center\">" + ControlChars.CrLf);
												sb.Append("    <tr>"     + ControlChars.CrLf);
												sb.Append("        <td>");
												sb.Append(sORIGINAL_TEXT);
												sb.Append("</td>"            + ControlChars.CrLf);
												sb.Append("    </tr>"    + ControlChars.CrLf);
												sb.Append("</table>"         + ControlChars.CrLf);
												sb.Append("<br />"           + ControlChars.CrLf);
												sb.Append("<br />"           + ControlChars.CrLf);
												txtDESCRIPTION.Value  = sb.ToString();
											}
										}
									}
								}
							}
							else
							{
								sSQL = "select *              " + ControlChars.CrLf
								     + "  from vwPOSTS_Edit   " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
									Security.Filter(cmd, "Posts", "view");
									Sql.AppendParameter(cmd, gREPLY_ID, "ID", false);
									con.Open();

									if ( bDebug )
										RegisterClientScriptBlock("vwPOSTS_Edit", Sql.ClientScriptBlock(cmd));

									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											ctlModuleHeader.Title = Sql.ToString(rdr["TITLE"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
											ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

											THREAD_ID.Value = Sql.ToString(rdr["THREAD_ID"]);
											txtTITLE.Text = L10n.Term("Posts.LBL_REPLY_PREFIX") + Sql.ToString (rdr["TITLE"]);
											if ( Sql.ToBoolean(Request["QUOTE"]) )
											{
												string sCREATED_BY    = Sql.ToString(rdr["CREATED_BY"      ]);
												string sORIGINAL_TEXT = Sql.ToString(rdr["DESCRIPTION_HTML"]);
												string sQUOTE_FORMAT  = L10n.Term("Posts.QUOTE_FORMAT");
												StringBuilder sb = new StringBuilder();
												sb.Append("<br />"           + ControlChars.CrLf);
												sb.Append("<br />"           + ControlChars.CrLf);
												sb.Append("<strong><em>" + String.Format(sQUOTE_FORMAT, sCREATED_BY) + "</em></strong> <br />" + ControlChars.CrLf);
												sb.Append("<table width=\"90%\" border=\"1\" cellspacing=\"0\" cellpadding=\"4\" style=\"FONT-STYLE: italic\" align=\"center\">" + ControlChars.CrLf);
												sb.Append("    <tr>"     + ControlChars.CrLf);
												sb.Append("        <td>");
												sb.Append(sORIGINAL_TEXT);
												sb.Append("</td>"            + ControlChars.CrLf);
												sb.Append("    </tr>"    + ControlChars.CrLf);
												sb.Append("</table>"         + ControlChars.CrLf);
												sb.Append("<br />"           + ControlChars.CrLf);
												sb.Append("<br />"           + ControlChars.CrLf);
												txtDESCRIPTION.Value  = sb.ToString();
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlEditButtons.ErrorText = ex.Message;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Posts";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
