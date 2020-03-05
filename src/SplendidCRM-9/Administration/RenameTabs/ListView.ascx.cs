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

namespace SplendidCRM.Administration.RenameTabs
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected DataView        vwMain       ;
		protected SplendidGrid    grdMain      ;
		protected Label           lblError     ;
		protected SearchBasic     ctlSearch    ;
		protected HtmlInputHidden txtRENAME    ;
		protected HtmlInputHidden txtKEY       ;
		protected HtmlInputHidden txtVALUE     ;
		protected bool            bEnableAdd   ;

		protected _controls.ListHeader ctlListHeader ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
		}

		private void TERMINOLOGY_BindData(bool bBind)
		{
			bEnableAdd = true;
			ctlListHeader.Visible = true;
			
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                   " + ControlChars.CrLf
					     + "  from vwMODULES_RenameTabs" + ControlChars.CrLf
					     + " where 1 = 1               " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 11/19/2005 Paul.  The language must be initialized before the search clause is applied. 
						if ( !IsPostBack )
							ctlSearch.LANGUAGE = L10n.NAME;
						ctlSearch.SqlSearchClause(cmd);

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								
								// 12/14/2007 Paul.  Only set the default sort if it is not already set.  It may have been set by SearchView. 
								if ( String.IsNullOrEmpty(grdMain.SortColumn) )
								{
									grdMain.SortColumn = "TAB_ORDER";
									grdMain.SortOrder  = "asc" ;
								}
								grdMain.ApplySort();
								if ( bBind )
									grdMain.DataBind();
							}
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

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Administration.LBL_RENAME_TABS"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			// 09/08/2005 Paul. An empty key is valid, so use a separate INSERT field. 
			if ( txtRENAME.Value == "1" )
			{
				try
				{
					SqlProcs.spMODULES_TAB_Rename(Guid.Empty, txtKEY.Value, ctlSearch.LANGUAGE, txtVALUE.Value);
					SplendidCache.ClearList(ctlSearch.LANGUAGE, "moduleList");
					// 01/17/2006 Paul.  Also need to clear the TabMenu. 
					SplendidCache.ClearTabMenu();
					// 04/20/2006 Paul.  Also clear the term for the list. 
					L10N.SetTerm(ctlSearch.LANGUAGE, String.Empty, "moduleList", txtKEY.Value, txtVALUE.Value);
					txtRENAME.Value = "";
					// 09/09/2005 Paul.  Transfer so that viewstate will be reset completely. 
					// 01/04/2005 Paul.  Redirecting to default.aspx will loose the language setting.  Just rebind. 
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					lblError.Text = ex.Message;
				}
			}
			// Must bind in order for LinkButton to get the argument. 
			// ImageButton does not work no matter what I try. 
			TERMINOLOGY_BindData(true);
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
			// We have to load the control in here, otherwise the control will not initialized before the Page_Load above. 
			ctlSearch.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
