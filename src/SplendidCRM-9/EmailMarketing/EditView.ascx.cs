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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.EmailMarketing
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;
		protected HiddenField     CAMPAIGN_ID                  ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				// 01/16/2006 Paul.  Enable validator before validating page. 
				this.ValidateEditViewFields(m_sMODULE + ".EditView");
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "EMAIL_MARKETING";
					DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
						DataRow   rowCurrent = null;
						DataTable dtCurrent  = new DataTable();
						if ( !Sql.IsEmptyGuid(gID) )
						{
							string sSQL ;
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwEMAIL_MARKETING_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtCurrent);
									if ( dtCurrent.Rows.Count > 0 )
									{
										rowCurrent = dtCurrent.Rows[0];
									}
									else
									{
										// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
										// It is possible that the record exists, but that ACL rules prevent it from being selected. 
										gID = Guid.Empty;
									}
								}
							}
						}

						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
								SqlProcs.spEMAIL_MARKETING_Update(ref gID
									, new DynamicControl(this, rowCurrent, "NAME"              ).Text
									, new DynamicControl(this, rowCurrent, "FROM_ADDR"         ).Text
									, new DynamicControl(this, rowCurrent, "FROM_NAME"         ).Text
									, new DynamicControl(this, rowCurrent, "DATE_START"        ).DateValue
									, new DynamicControl(this, rowCurrent, "TEMPLATE_ID"       ).ID
									, new DynamicControl(this, rowCurrent, "CAMPAIGN_ID"       ).ID
									, new DynamicControl(this, rowCurrent, "INBOUND_EMAIL_ID"  ).ID
									, new DynamicControl(this, rowCurrent, "STATUS"            ).SelectedValue
									, new DynamicControl(this, rowCurrent, "ALL_PROSPECT_LISTS").Checked
									, trn
									);
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								ctlEditButtons.ErrorText = ex.Message;
								return;
							}
						}
					}
					Guid gCAMPAIGN_ID = Sql.ToGuid(Request["CAMPAIGN_ID"]);
					if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
						Response.Redirect("~/Campaigns/view.aspx?ID=" + gCAMPAIGN_ID.ToString());
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Guid gCAMPAIGN_ID = Sql.ToGuid(Request["CAMPAIGN_ID"]);
				if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
					Response.Redirect("~/Campaigns/view.aspx?ID=" + gCAMPAIGN_ID.ToString());
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
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwEMAIL_MARKETING_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AppendParameter(cmd, gDuplicateID, "ID", false);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AppendParameter(cmd, gID, "ID", false);
								}
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
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										Guid gCAMPAIGN_ID = Sql.ToGuid(rdr["CAMPAIGN_ID"]);
										CAMPAIGN_ID.Value = gCAMPAIGN_ID.ToString();
										this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, rdr);
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
						this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);

						Guid gCAMPAIGN_ID = Sql.ToGuid(Request["CAMPAIGN_ID"]);
						CAMPAIGN_ID.Value = gCAMPAIGN_ID.ToString();
					}
					HtmlInputButton btnChange = FindControl("TEMPLATE_ID_btnChange") as HtmlInputButton;
					if ( btnChange != null )
					{
						HtmlInputButton btnEdit = new HtmlInputButton("button");
						btnEdit.ID = "TEMPLATE_ID_btnEdit";
						btnEdit.Attributes.Add("class"  , "button");
						btnEdit.Attributes.Add("onclick", "EmailTemplateEdit();");
						btnEdit.Value = "  " + L10n.Term(".LBL_EDIT_BUTTON_LABEL") + "  ";
						btnChange.Parent.Controls.Add(btnEdit);
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
			m_sMODULE = "EmailMarketing";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
				HtmlInputButton btnChange = FindControl("TEMPLATE_ID_btnChange") as HtmlInputButton;
				if ( btnChange != null )
				{
					HtmlInputButton btnEdit = new HtmlInputButton("button");
					btnEdit.ID = "TEMPLATE_ID_btnEdit";
					btnEdit.Attributes.Add("class"  , "button");
					btnEdit.Attributes.Add("onclick", "EmailTemplateEdit();");
					btnEdit.Value = "  " + L10n.Term(".LBL_EDIT_BUTTON_LABEL") + "  ";
					btnChange.Parent.Controls.Add(btnEdit);
				}
			}
		}
		#endregion
	}
}
