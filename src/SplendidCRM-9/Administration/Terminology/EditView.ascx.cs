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

namespace SplendidCRM.Administration.Terminology
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid                       gID                ;
		protected TextBox                    txtNAME            ;
		protected TextBox                    txtDISPLAY_NAME    ;
		protected DropDownList               lstLANGUAGE        ;
		protected DropDownList               lstMODULE_NAME     ;
		protected DropDownList               lstLIST_NAME       ;
		protected TextBox                    txtLIST_ORDER      ;
		protected RequiredFieldValidator     reqNAME            ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					reqNAME.Enabled = true;
					reqNAME.Validate();
					if ( Page.IsValid )
					{
						Guid gID = Guid.Empty;
						try
						{
							SqlProcs.spTERMINOLOGY_Update(txtNAME.Text, lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, lstLIST_NAME.SelectedValue, Sql.ToInteger(txtLIST_ORDER.Text), txtDISPLAY_NAME.Text);
							// 01/16/2006 Paul.  Update language cache. 
							if ( Sql.IsEmptyString(lstLIST_NAME.SelectedValue) )
								L10N.SetTerm(lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, txtNAME.Text, txtDISPLAY_NAME.Text);
							else
								L10N.SetTerm(lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, lstLIST_NAME.SelectedValue, txtNAME.Text, txtDISPLAY_NAME.Text);
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlEditButtons.ErrorText = ex.Message;
							return;
						}
					}
					Response.Redirect("default.aspx");
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList.Terminology"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					lstLANGUAGE.DataSource = SplendidCache.Languages();
					lstLANGUAGE.DataBind();

					lstMODULE_NAME.DataSource = SplendidCache.Modules();
					lstMODULE_NAME.DataBind();
					lstMODULE_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

					lstLIST_NAME  .DataSource = SplendidCache.TerminologyPickLists();
					lstLIST_NAME  .DataBind();
					lstLIST_NAME  .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwTERMINOLOGY_Edit" + ControlChars.CrLf
							     + " where ID = @ID          " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList.Terminology") + " - " + ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										// 01/20/2006 Paul.  Don't allow the name to be changed.  Require a new term. 
										txtNAME.ReadOnly = true;

										txtNAME        .Text = Sql.ToString(rdr["NAME"        ]);
										txtDISPLAY_NAME.Text = Sql.ToString(rdr["DISPLAY_NAME"]);
										if ( Sql.ToInteger(rdr["LIST_ORDER"]) > 0 )
											txtLIST_ORDER.Text = Sql.ToString(rdr["LIST_ORDER"]);
										try
										{
											lstLANGUAGE.SelectedValue = L10N.NormalizeCulture(Sql.ToString(rdr["LANG"]));
										}
										catch
										{
										}
										string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
										try
										{
											lstMODULE_NAME.SelectedValue = sMODULE_NAME;
										}
										catch
										{
											// 01/12/2006 Paul.  If module does not exist in our table, then add it to prevent data loss. 
											lstMODULE_NAME.Items.Add(sMODULE_NAME);
											lstMODULE_NAME.SelectedValue = sMODULE_NAME;
										}
										try
										{
											lstLIST_NAME.SelectedValue = Sql.ToString(rdr["LIST_NAME"]);
										}
										catch
										{
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
					SetPageTitle(L10n.Term(".moduleList.Administration") + " - " + ctlModuleHeader.Title);
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
		}
		#endregion
	}
}
