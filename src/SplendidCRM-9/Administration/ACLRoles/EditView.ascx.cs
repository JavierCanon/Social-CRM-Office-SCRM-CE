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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.ACLRoles
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;
		protected AccessView ctlAccessView;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + ".EditView");
					if ( Page.IsValid )
					{
						DataTable dtACLACCESS = null;
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME          " + ControlChars.CrLf
							     + "     , DISPLAY_NAME         " + ControlChars.CrLf
							     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
							     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
							     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
							     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
							     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
							     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
							     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
							     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
							     + "  from vwACL_ACCESS_ByModule" + ControlChars.CrLf
							     + " order by MODULE_NAME       " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dtACLACCESS = new DataTable();
									da.Fill(dtACLACCESS);
								}
							}

							string sCUSTOM_MODULE = "ACL_ROLES";
							DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
							using ( IDbTransaction trn = con.BeginTransaction() )
							{
								try
								{
									SqlProcs.spACL_ROLES_Update
										( ref gID
										, new DynamicControl(this, "NAME"       ).Text
										, new DynamicControl(this, "DESCRIPTION").Text
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
									
									foreach(DataRow row in dtACLACCESS.Rows)
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										// 04/25/2006 Paul.  FindControl needs to be executed on the DataGridItem.  I'm not sure why.
										DropDownList lstAccess = lstAccess = ctlAccessView.FindACLControl(sMODULE_NAME, "access");
										DropDownList lstView   = ctlAccessView.FindACLControl(sMODULE_NAME, "view"  );
										DropDownList lstList   = ctlAccessView.FindACLControl(sMODULE_NAME, "list"  );
										DropDownList lstEdit   = ctlAccessView.FindACLControl(sMODULE_NAME, "edit"  );
										DropDownList lstDelete = ctlAccessView.FindACLControl(sMODULE_NAME, "delete");
										DropDownList lstImport = ctlAccessView.FindACLControl(sMODULE_NAME, "import");
										DropDownList lstExport = ctlAccessView.FindACLControl(sMODULE_NAME, "export");
										Guid gActionAccessID = Guid.Empty;
										Guid gActionViewID   = Guid.Empty;
										Guid gActionListID   = Guid.Empty;
										Guid gActionEditID   = Guid.Empty;
										Guid gActionDeleteID = Guid.Empty;
										Guid gActionImportID = Guid.Empty;
										Guid gActionExportID = Guid.Empty;
										if ( lstAccess != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionAccessID, gID, "access", sMODULE_NAME, Sql.ToInteger(lstAccess.SelectedValue), trn);
										if ( lstView   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionViewID  , gID, "view"  , sMODULE_NAME, Sql.ToInteger(lstView  .SelectedValue), trn);
										if ( lstList   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionListID  , gID, "list"  , sMODULE_NAME, Sql.ToInteger(lstList  .SelectedValue), trn);
										if ( lstEdit   != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionEditID  , gID, "edit"  , sMODULE_NAME, Sql.ToInteger(lstEdit  .SelectedValue), trn);
										if ( lstDelete != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionDeleteID, gID, "delete", sMODULE_NAME, Sql.ToInteger(lstDelete.SelectedValue), trn);
										if ( lstImport != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionImportID, gID, "import", sMODULE_NAME, Sql.ToInteger(lstImport.SelectedValue), trn);
										if ( lstExport != null ) SqlProcs.spACL_ROLES_ACTIONS_Update(ref gActionExportID, gID, "export", sMODULE_NAME, Sql.ToInteger(lstExport.SelectedValue), trn);
										//break;
									}
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
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlEditButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("ACLRoles.LBL_ROLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
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
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwACL_ROLES_Edit" + ControlChars.CrLf
							     + " where ID = @ID        " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term("ACLRoles.LBL_ROLE") + " - " + ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, rdr);
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term("ACLRoles.LBL_ROLE") + " - " + ctlModuleHeader.Title);
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
			m_sMODULE = "ACLRoles";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
			}
		}
		#endregion
	}
}
