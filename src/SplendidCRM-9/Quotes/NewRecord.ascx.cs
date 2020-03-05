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

namespace SplendidCRM.Quotes
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		protected Label                      lblError          ;
		protected TextBox                    txtNAME           ;
		protected TextBox                    txtACCOUNT_NAME   ;
		protected HiddenField                txtACCOUNT_ID     ;
		protected DropDownList               lstQUOTE_STAGE    ;
		protected RequiredFieldValidator     reqNAME           ;
		protected RequiredFieldValidator     reqAMOUNT         ;
		protected RequiredFieldValidatorForHiddenInputs reqACCOUNT_ID     ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord" )
			{
				reqNAME       .Enabled = true;
				reqACCOUNT_ID .Enabled = true;
				reqNAME       .Validate();
				reqACCOUNT_ID .Validate();
				if ( Page.IsValid )
				{
					Guid gID = Guid.Empty;
					try
					{
						SqlProcs.spQUOTES_New(ref gID, Sql.ToGuid(txtACCOUNT_ID.Value), txtNAME.Text, lstQUOTE_STAGE.SelectedValue);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					if ( !Sql.IsEmptyGuid(gID) )
						Response.Redirect("~/Quotes/view.aspx?ID=" + gID.ToString());
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Bind so that the text of the validators get updated. 
			reqNAME       .ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Quotes.LBL_LIST_NAME"        ) + "<br>";
			reqACCOUNT_ID .ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Quotes.LBL_LIST_ACCOUNT_NAME") + "<br>";
			if ( !IsPostBack )
			{
				lstQUOTE_STAGE.DataSource = SplendidCache.List("quote_stage_dom");
				lstQUOTE_STAGE.DataBind();
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
			m_sMODULE = "Quotes";
		}
		#endregion
	}
}
