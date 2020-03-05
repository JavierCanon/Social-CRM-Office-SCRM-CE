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

namespace SplendidCRM.Campaigns
{
	/// <summary>
	/// Summary description for Preview.
	/// </summary>
	public class Preview : SplendidPage
	{
		protected SplendidCRM._controls.SearchView   ctlSearchView  ;

		protected Guid          gID           ;
		protected DataView      vwMain        ;
		protected SplendidGrid  grdMain       ;
		protected Label         lblError      ;
		protected Button        btnProduction ;
		protected Button        btnTest       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
				else if ( e.CommandName == "Preview.Production" )
				{
					ViewState["TEST"] = false;
					CAMPAIGNS_BindData(true);
				}
				else if ( e.CommandName == "Preview.Test" )
				{
					ViewState["TEST"] = true;
					CAMPAIGNS_BindData(true);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void CAMPAIGNS_BindData(bool bBind)
		{
			bool bTEST = Sql.ToBoolean(ViewState["TEST"]);
			btnProduction.Enabled =  bTEST;
			btnTest      .Enabled = !bTEST;

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *               " + ControlChars.CrLf
				     + "  from vwCAMPAIGNS_Send" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
					Security.Filter(cmd, "Campaigns", "list");
					Sql.AppendParameter(cmd, gID  , "CAMPAIGN_ID", false);
					// 09/09/2007 Paul.  AppendParameter is ignoring false values.  Apply the filter manually. 
					cmd.CommandText += "   and TEST = @TEST" + ControlChars.CrLf;
					Sql.AddParameter(cmd, "@TEST" , bTEST);
					ctlSearchView.SqlSearchClause(cmd);

					if ( bDebug )
						Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "SQLCode", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( bBind )
								{
									// 12/14/2007 Paul.  Only set the default sort if it is not already set.  It may have been set by SearchView. 
									if ( String.IsNullOrEmpty(grdMain.SortColumn) )
									{
										grdMain.SortColumn = "RELATED_NAME";
										grdMain.SortOrder  = "asc" ;
									}
									grdMain.ApplySort();
									grdMain.DataBind();
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
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			SetPageTitle(L10n.Term("Campaigns.LBL_LIST_FORM_TITLE"));
			if ( !IsPostBack )
			{
				ViewState["TEST"] = false;
				CAMPAIGNS_BindData(true);
				Page.DataBind();
			}
			else
			{
				CAMPAIGNS_BindData(false);
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
			ctlSearchView.Command = new CommandEventHandler(Page_Command);
			// 07/26/2007 Paul.  Use the new PopupView so that the view is customizable. 
			this.AppendGridColumns(grdMain, "Campaigns.PreviewView");
		}
		#endregion
	}
}
