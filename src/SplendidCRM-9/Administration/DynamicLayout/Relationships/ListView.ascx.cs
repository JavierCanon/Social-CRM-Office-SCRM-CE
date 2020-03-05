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

namespace SplendidCRM.Administration.DynamicLayout.Relationships
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.SearchBasic            ctlSearch    ;
		protected SplendidCRM._controls.ListHeader ctlListHeader;

		protected DataView        vwMain       ;
		protected SplendidGrid    grdMain      ;
		protected Label           lblError     ;
		protected HtmlInputHidden txtINSERT    ;
		protected HtmlInputHidden txtKEY       ;
		protected HtmlInputHidden txtVALUE     ;
		protected HtmlInputHidden txtINDEX     ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				Guid gID = Sql.ToGuid(e.CommandArgument);
				if ( e.CommandName == "Relationships.MoveUp" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spDETAILVIEWS_RELATIONSHIPS_MoveUp(gID);
				}
				else if ( e.CommandName == "Relationships.MoveDown" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					// 09/08/2007 Paul.  The name is not MoveDown because Oracle will truncate to 30 characters
					// and we need to ensure there is no collision with MoveUp.
					SqlProcs.spDETAILVIEWS_RELATIONSHIPS_Down(gID);
				}
				else if ( e.CommandName == "Relationships.Disable" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spDETAILVIEWS_RELATIONSHIPS_Disable(gID);
				}
				else if ( e.CommandName == "Relationships.Enable" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spDETAILVIEWS_RELATIONSHIPS_Enable(gID);
				}
				// 01/04/2005 Paul.  If the list changes, reset the cached values. 
				SplendidCache.ClearDetailViewRelationships("vwMODULES_TabMenu");
				DETAILVIEWS_RELATIONSHIPS_BindData(true);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void DETAILVIEWS_RELATIONSHIPS_BindData(bool bBind)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                             " + ControlChars.CrLf
					     + "  from vwDETAILVIEWS_RELATIONSHIPS_La" + ControlChars.CrLf
					     + " where @DETAIL_NAME = DETAIL_NAME    " + ControlChars.CrLf
					     + " order by RELATIONSHIP_ENABLED, RELATIONSHIP_ORDER, MODULE_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@DETAIL_NAME", ctlSearch.NAME);

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
			SetPageTitle(L10n.Term("DynamicLayout.LBL_RELATIONSHIPS_LAYOUT"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				// 01/08/2006 Paul.  The viewstate is no longer disabled, so we can go back to using ctlSearch.NAME.
				string sNAME = ctlSearch.NAME;  //Sql.ToString(Request[ctlSearch.ListUniqueID]);
				ctlSearch.Visible = Sql.IsEmptyString(sNAME);
				ctlListHeader.Visible = !ctlSearch.Visible;
				ctlListHeader.Title = sNAME;

				if ( !Sql.IsEmptyString(sNAME) && sNAME != Sql.ToString(ViewState["LAYOUT_VIEW_NAME"]) )
				{
					// 01/08/2006 Paul.  We are having a problem with the ViewState not loading properly.
					// This problem only seems to occur when the NewRecord is visible and we try and load a different view.
					// The solution seems to be to hide the Search dialog so that the user must Cancel out of editing the current view.
					// This works very well to clear the ViewState because we GET the next page instead of POST to it. 
					
					SetPageTitle(sNAME);
					Page.DataBind();

					// Must bind in order for LinkButton to get the argument. 
					// ImageButton does not work no matter what I try. 
					DETAILVIEWS_RELATIONSHIPS_BindData(true);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
		}
		#endregion
	}
}
