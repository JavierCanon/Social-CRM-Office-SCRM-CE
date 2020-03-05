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
using FredCK.FCKeditorV2;

namespace SplendidCRM.Help
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected string          sNAME                        ;
		protected string          sMODULE                      ;
		protected FCKeditor       txtDISPLAY_TEXT              ;

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
								SqlProcs.spTERMINOLOGY_HELP_Update(ref gID, sNAME, L10n.NAME, sMODULE, txtDISPLAY_TEXT.Value);
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
					Response.Redirect("view.aspx?NAME=" + sNAME + "&MODULE=" + sMODULE);
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("view.aspx?NAME=" + sNAME + "&MODULE=" + sMODULE);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Utils.SetPageTitle(Page, L10n.Term(".moduleList." + m_sMODULE));
			// 10/25/2006 Paul.  There is a config flag to disable the wiki entirely. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0) && Sql.ToBoolean(Application["CONFIG.enable_help_wiki"]);
			if ( !this.Visible )
				return;

			try
			{
				gID     = Sql.ToGuid  (Request["ID"    ]);
				sNAME   = Sql.ToString(Request["NAME"  ]);
				sMODULE = Sql.ToString(Request["MODULE"]);
				if ( !Sql.IsEmptyGuid(gID) )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select *                 " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY_HELP" + ControlChars.CrLf
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
									sNAME   = Sql.ToString(rdr["NAME"       ]);
									sMODULE = Sql.ToString(rdr["MODULE_NAME"]);
									Utils.SetPageTitle(Page, L10n.Term(".moduleList." + sMODULE) + " - " + L10n.Term(".LNK_HELP"));

									if ( !IsPostBack )
									{
										txtDISPLAY_TEXT.Value = Sql.ToString(rdr["DISPLAY_TEXT"]);
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
			m_sMODULE = "Help";
		}
		#endregion
	}
}
