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
using System.Collections;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Shortcuts
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID             ;
		protected DropDownList    MODULE_NAME     ;
		protected TextBox         DISPLAY_NAME    ;
		protected TextBox         RELATIVE_PATH   ;
		protected TextBox         IMAGE_NAME      ;
		protected CheckBox        SHORTCUT_ENABLED;
		protected TextBox         SHORTCUT_ORDER  ;
		protected DropDownList    SHORTCUT_MODULE ;
		protected DropDownList    SHORTCUT_ACLTYPE;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								SqlProcs.spSHORTCUTS_Update(ref gID, MODULE_NAME.SelectedValue, DISPLAY_NAME.Text, RELATIVE_PATH.Text, IMAGE_NAME.Text, SHORTCUT_ENABLED.Checked, Sql.ToInteger(SHORTCUT_ORDER.Text), SHORTCUT_MODULE.SelectedValue, SHORTCUT_ACLTYPE.SelectedValue);
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
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					MODULE_NAME.DataSource = SplendidCache.Modules();
					MODULE_NAME.DataBind();
					SHORTCUT_MODULE.DataSource = SplendidCache.Modules();
					SHORTCUT_MODULE.DataBind();
					SHORTCUT_MODULE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					SHORTCUT_ACLTYPE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwSHORTCUTS_Edit" + ControlChars.CrLf
							     + " where 1 = 1           " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["DISPLAY_NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
										
										try
										{
											MODULE_NAME.SelectedValue = Sql.ToString(rdr["MODULE_NAME"]);
										}
										catch
										{
										}
										DISPLAY_NAME    .Text = Sql.ToString(rdr["DISPLAY_NAME" ]);
										RELATIVE_PATH   .Text = Sql.ToString(rdr["RELATIVE_PATH"]);
										IMAGE_NAME      .Text = Sql.ToString(rdr["IMAGE_NAME"   ]);
										SHORTCUT_ENABLED.Checked = Sql.ToBoolean(rdr["SHORTCUT_ENABLED"]);
										if ( Sql.ToInteger(rdr["SHORTCUT_ORDER"]) > 0 )
											SHORTCUT_ORDER.Text = Sql.ToString(rdr["SHORTCUT_ORDER"]);
										try
										{
											SHORTCUT_MODULE.SelectedValue = Sql.ToString(rdr["SHORTCUT_MODULE"]);
										}
										catch
										{
										}
										try
										{
											SHORTCUT_ACLTYPE.SelectedValue = Sql.ToString(rdr["SHORTCUT_ACLTYPE"]);
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
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Shortcuts";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
