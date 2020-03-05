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

namespace SplendidCRM.Administration.Dropdown
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
				if ( e.CommandName == "Select" )
				{
					TERMINOLOGY_BindData(true);
				}
				else if ( e.CommandName == "Dropdown.MoveUp" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spTERMINOLOGY_LIST_MoveUp(gID);
					// 09/08/2005 Paul.  If the list changes, reset the cached values. 
					SplendidCache.ClearList(ctlSearch.LANGUAGE, ctlSearch.DROPDOWN);
					//TERMINOLOGY_BindData(true);
					Response.Redirect("default.aspx?Dropdown=" + ctlSearch.DROPDOWN);
				}
				else if ( e.CommandName == "Dropdown.MoveDown" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spTERMINOLOGY_LIST_MoveDown(gID);
					// 09/08/2005 Paul.  If the list changes, reset the cached values. 
					SplendidCache.ClearList(ctlSearch.LANGUAGE, ctlSearch.DROPDOWN);
					//TERMINOLOGY_BindData(true);
					Response.Redirect("default.aspx?Dropdown=" + ctlSearch.DROPDOWN);
				}
				else if ( e.CommandName == "Dropdown.Delete" )
				{
					if ( Sql.IsEmptyGuid(gID) )
						throw(new Exception("Unspecified argument"));
					SqlProcs.spTERMINOLOGY_LIST_Delete(gID);
					// 09/08/2005 Paul.  If the list changes, reset the cached values. 
					SplendidCache.ClearList(ctlSearch.LANGUAGE, ctlSearch.DROPDOWN);
					//TERMINOLOGY_BindData(true);
					Response.Redirect("default.aspx?Dropdown=" + ctlSearch.DROPDOWN);
				}
				else
				{
					TERMINOLOGY_BindData(true);
				}
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
					sSQL = "select *                 " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY_List" + ControlChars.CrLf
					     + " where 1 = 1             " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 01/16/2006 Paul.  New lists should go directly to list editing. 
						string sDROPDOWN = Sql.ToString(Request.QueryString["DROPDOWN"]);
						// 11/19/2005 Paul.  The language must be initialized before the search clause is applied. 
						if ( !IsPostBack )
						{
							// 01/05/2006 Paul.  Fix Form Action so that Query String parameters will not continue to get passed around. 
							if ( !Sql.IsEmptyString(sDROPDOWN) )
							{
								ctlSearch.DROPDOWN = sDROPDOWN;
								RegisterClientScriptBlock("frmRedirect", "<script type=\"text/javascript\">document.forms[0].action='default.aspx';</script>");
							}
							ctlSearch.LANGUAGE = L10n.NAME;
						}
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
									grdMain.SortColumn = "LIST_ORDER";
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
			SetPageTitle(L10n.Term("Dropdown.LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				// 09/08/2005 Paul. An empty key is valid, so use a separate INSERT field. 
				if ( txtINSERT.Value == "1" )
				{
					try
					{
						Guid gID = Guid.Empty;
						SqlProcs.spTERMINOLOGY_LIST_Insert(ref gID, txtKEY.Value, ctlSearch.LANGUAGE, String.Empty, ctlSearch.DROPDOWN, Sql.ToInteger(txtINDEX.Value), txtVALUE.Value);
						// 01/16/2006 Paul.  Update cache. 
						L10N.SetTerm(ctlSearch.LANGUAGE, String.Empty, ctlSearch.DROPDOWN, txtKEY.Value, txtVALUE.Value);
						// 07/25/2005 Paul.  In addition to updating the term, we must update the cached list. 
						SplendidCache.ClearList(ctlSearch.LANGUAGE, ctlSearch.DROPDOWN);
						txtINSERT.Value = "";
						// 09/09/2005 Paul.  Transfer so that viewstate will be reset completely. 
						Response.Redirect("default.aspx?Dropdown=" + ctlSearch.DROPDOWN);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
				if ( !IsPostBack )
				{
					string sDROPDOWN = Sql.ToString(Request["Dropdown"]);
					if ( !Sql.IsEmptyString(sDROPDOWN) )
					{
						ctlSearch.DROPDOWN = sDROPDOWN;
						TERMINOLOGY_BindData(true);
					}
				}
				else
				{
					// Must bind in order for LinkButton to get the argument. 
					// ImageButton does not work no matter what I try. 
					TERMINOLOGY_BindData(true);
				}
			}
			catch(Exception ex)
			{
				// 01/20/2006 Paul.  Need to catch all errors.  Saw a dropdown error when creating a new dropdown. 
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
			// We have to load the control in here, otherwise the control will not initialized before the Page_Load above. 
			ctlSearch.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
