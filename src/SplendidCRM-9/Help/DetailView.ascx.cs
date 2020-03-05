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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Help
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected _controls.DetailButtons ctlDetailButtons;
		protected Literal lblDISPLAY_TEXT;

		protected Guid        gID              ;
		protected string      sPageTitle       ;
		protected string      sNAME            ;
		protected string      sMODULE          ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString() + "&NAME=" + sNAME + "&MODULE=" + sMODULE);
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
			// 10/25/2006 Paul.  There is a config flag to disable the wiki entirely. 
			ctlDetailButtons.ShowEdit = Sql.ToBoolean(Application["CONFIG.enable_help_wiki"]);

			sNAME   = Sql.ToString(Request["NAME"  ]);
			sMODULE = Sql.ToString(Request["MODULE"]);
			sPageTitle = L10n.Term(".moduleList." + sMODULE) + " - " + L10n.Term(".LNK_HELP");
			Utils.SetPageTitle(Page, sPageTitle);
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL ;
					sSQL = "select *                         " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY_HELP        " + ControlChars.CrLf
					     + " where LANG        = @LANG       " + ControlChars.CrLf
					     + "   and MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf
					     + "   and NAME        = @NAME       " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LANG"       , L10n.NAME);
						Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE  );
						Sql.AddParameter(cmd, "@NAME"       , sNAME    );
						con.Open();

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								gID     = Sql.ToGuid  (rdr["ID"         ]);
								sNAME   = Sql.ToString(rdr["NAME"       ]);
								sMODULE = Sql.ToString(rdr["MODULE_NAME"]);
								sPageTitle = L10n.Term(".moduleList." + sMODULE) + " - " + L10n.Term(".LNK_HELP");
								Utils.SetPageTitle(Page, sPageTitle);
								lblDISPLAY_TEXT.Text = Sql.ToString(rdr["DISPLAY_TEXT"]);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
				Page.DataBind();
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
			m_sMODULE = "Help";
		}
		#endregion
	}
}
