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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.Terminology
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		protected Label                      lblError           ;
		protected TextBox                    txtNAME            ;
		protected TextBox                    txtDISPLAY_NAME    ;
		protected DropDownList               lstLANGUAGE        ;
		protected DropDownList               lstMODULE_NAME     ;
		protected DropDownList               lstLIST_NAME       ;
		protected TextBox                    txtLIST_ORDER      ;
		protected RequiredFieldValidator     reqNAME            ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord" )
			{
				reqNAME.Enabled = true;
				reqNAME.Validate();
				if ( Page.IsValid )
				{
					Guid gID = Guid.Empty;
					try
					{
						SqlProcs.spTERMINOLOGY_InsertOnly(txtNAME.Text, lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, lstLIST_NAME.SelectedValue, Sql.ToInteger(txtLIST_ORDER.Text), txtDISPLAY_NAME.Text);
						// 01/16/2006 Paul.  Update language cache. 
						if ( Sql.IsEmptyString(lstLIST_NAME.SelectedValue) )
							L10N.SetTerm(lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, txtNAME.Text, txtDISPLAY_NAME.Text);
						else
							L10N.SetTerm(lstLANGUAGE.SelectedValue, lstMODULE_NAME.SelectedValue, lstLIST_NAME.SelectedValue, txtNAME.Text, txtDISPLAY_NAME.Text);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					if ( !Sql.IsEmptyGuid(gID) )
						Response.Redirect("default.aspx");
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Need to bind so that Text of the Button gets updated. 
			reqNAME.ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Terminology.LBL_LIST_NAME") + "<br>";
			if ( !IsPostBack )
			{
				// 01/12/2006 Paul.  Language cannot be null. 
				lstLANGUAGE.DataSource = SplendidCache.Languages();
				lstLANGUAGE.DataBind();

				DataTable dtModules = SplendidCache.Modules().Copy();
				dtModules.Rows.InsertAt(dtModules.NewRow(), 0);
				lstMODULE_NAME.DataSource = dtModules;
				lstMODULE_NAME.DataBind();
				// 01/12/2006 Paul.  Insert is failing, but I don't know why.  
				// It might be because the NewRecord control is loaded using LoadControl. 
				// Very odd as the Search Control is not having a problem inserting a value. 
				//lstMODULE_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

				DataTable dtPickLists = SplendidCache.TerminologyPickLists().Copy();
				dtPickLists.Rows.InsertAt(dtPickLists.NewRow(), 0);
				lstLIST_NAME.DataSource = dtPickLists;
				lstLIST_NAME.DataBind();
				//lstLIST_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

				try
				{
					// 01/12/2006 Paul.  Set default value to current language. 
					// 01/12/2006 Paul.  This is not working.  Use client-side script to select the default. 
					lstLANGUAGE.SelectedValue = L10N.NormalizeCulture(L10n.NAME);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
				//this.DataBind();
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
