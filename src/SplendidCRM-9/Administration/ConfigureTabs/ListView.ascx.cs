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

namespace SplendidCRM.Administration.ConfigureTabs
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected DataView        vwMain       ;
		protected SplendidGrid    grdMain      ;
		protected Label           lblError     ;
		protected HtmlInputHidden txtINSERT    ;
		protected HtmlInputHidden txtKEY       ;
		protected HtmlInputHidden txtVALUE     ;
		protected HtmlInputHidden txtINDEX     ;
		protected bool            bEnableAdd   ;

		protected _controls.ListHeader ctlListHeader ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				Guid gID = Sql.ToGuid(e.CommandArgument);
				if ( e.CommandName == "ConfigureTabs.MoveUp" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_TAB_ORDER_MoveUp(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.MoveDown" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_TAB_ORDER_MoveDown(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.Hide" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_TAB_Hide(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.Show" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_TAB_Show(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.HideMobile" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_TAB_HideMobile(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.ShowMobile" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_TAB_ShowMobile(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.Disable" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_Disable(gID);
				}
				else if ( e.CommandName == "ConfigureTabs.Enable" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spMODULES_Enable(gID);
				}
				// 01/04/2005 Paul.  If the list changes, reset the cached values. 
				Cache.Remove("vwMODULES_TabMenu");
				// 06/03/2006 Paul.  The tab menu is now user-specific, but we will only clear the current user. 
				SplendidCache.ClearTabMenu();
				TERMINOLOGY_BindData(true);
				// 08/21/2007 Paul.  The easiest way to rebuild the current menu is to refresh the page. 
				// 10/02/2007 Paul.  Use an AJAX UpdatePanel to reduce whole page repaints. 
				//Response.Redirect("default.aspx");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
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
					// 08/22/2007 Paul.  Admin modules cannot be displayed as a tab. 
					sSQL = "select *                        " + ControlChars.CrLf
					     + "  from vwMODULES_CONFIGURE_TABS " + ControlChars.CrLf
					     + " order by MODULE_ENABLED, TAB_ENABLED, TAB_ORDER, MODULE_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;

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
			SetPageTitle(L10n.Term("Administration.LBL_CONFIGURE_TABS"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			// Must bind in order for LinkButton to get the argument. 
			// ImageButton does not work no matter what I try. 
			TERMINOLOGY_BindData(true);
			// 01/04/2006 Paul.  DataBind seems to be required, otherwise the table header will not get translated. 
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//Page.DataBind();
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
		}
		#endregion
	}
}
