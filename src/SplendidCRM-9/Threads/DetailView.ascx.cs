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

namespace SplendidCRM.Threads
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected SplendidCRM._controls.ModuleHeader  ctlModuleHeader ;
		protected _controls.ThreadButtons ctlDetailButtons;
		protected Posts        ctlPosts         ;
		protected ThreadView   ctlThreadView    ;
		protected HyperLink    lnkForum         ;

		protected Guid         gID              ;
		protected HtmlTable    tblMain          ;
		protected Label        txtTITLE         ;
		protected Label        txtCREATED_BY    ;
		protected Label        txtDATE_ENTERED  ;
		protected Label        txtMODIFIED_BY   ;
		protected Label        txtDATE_MODIFIED ;
		protected Literal      txtDESCRIPTION   ;
		protected HtmlTableRow trModified       ;
		protected PlaceHolder  plcSubPanel      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Reply" )
				{
					Response.Redirect("~/Posts/edit.aspx?THREAD_ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Quote" )
				{
					Response.Redirect("~/Posts/edit.aspx?QUOTE=1&THREAD_ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spTHREADS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDetailButtons.ErrorText = ex.Message;
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
				int nListView = Sql.ToInteger(Request["ListView"]);
				if ( !IsPostBack )
				{
					// 07/17/2007 Paul.  Default to ListView for admins and Threaded for everybody else. 
					if ( Security.IS_ADMIN && Sql.IsEmptyString(Request["ListView"]) )
					{
						nListView = 1;
						Response.Redirect("view.aspx?ID=" + gID.ToString() + "&ListView=" + nListView.ToString());
						return;
					}
				}
				ctlPosts     .Visible = nListView != 0;
				ctlThreadView.Visible = nListView == 0;

				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *             " + ControlChars.CrLf
							     + "  from vwTHREADS_Edit" + ControlChars.CrLf;
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
										ctlModuleHeader.Title = Sql.ToString(rdr["TITLE"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										SqlProcs.spTHREADS_IncrementViewCount(gID);
										
										txtTITLE        .Text = Sql.ToString(rdr["TITLE"           ]);
										txtCREATED_BY   .Text = Sql.ToString(rdr["CREATED_BY"      ]);
										txtDATE_ENTERED .Text = Sql.ToString(rdr["DATE_ENTERED"    ]);
										txtMODIFIED_BY  .Text = Sql.ToString(rdr["MODIFIED_BY"     ]);
										txtDATE_MODIFIED.Text = Sql.ToString(rdr["DATE_MODIFIED"   ]);
										txtDESCRIPTION  .Text = Sql.ToString(rdr["DESCRIPTION_HTML"]);
										if ( Sql.ToDateTime(rdr["DATE_ENTERED"]) != Sql.ToDateTime(rdr["DATE_MODIFIED"]) )
											trModified.Visible = true;

										Guid gCREATED_BY_ID = Sql.ToGuid(rdr["CREATED_BY_ID"]);
										ctlDetailButtons.ShowEdit   = Security.IS_ADMIN || gCREATED_BY_ID == Security.USER_ID;
										ctlDetailButtons.ShowDelete = ctlDetailButtons.ShowEdit;

										Guid gFORUM_ID = Sql.ToGuid(rdr["FORUM_ID"]);
										if ( !Sql.IsEmptyGuid(gFORUM_ID) )
										{
											lnkForum.NavigateUrl = "~/Forums/view.aspx?ID=" + gFORUM_ID.ToString();
											lnkForum.Visible = true;
										}
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't show its sub panel either. 
										plcSubPanel.Visible = false;
										ctlDetailButtons.DisableAll();
										ctlDetailButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
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
				ctlDetailButtons.ErrorText = ex.Message;
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
			ctlDetailButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Threads";
			SetMenu(m_sMODULE);
			this.AppendDetailViewRelationships(m_sMODULE + ".DetailView", plcSubPanel);
		}
		#endregion
	}
}
