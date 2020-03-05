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

namespace SplendidCRM.Quotes
{
	/// <summary>
	///		Summary description for MassUpdate.
	/// </summary>
	public class MassUpdate : SplendidCRM.MassUpdate
	{
		protected Button          btnUpdate          ;
		protected Button          btnDelete          ;

		protected HiddenField     txtSHIPPING_ACCOUNT_ID  ;
		protected TextBox         txtSHIPPING_ACCOUNT_NAME;
		protected HiddenField     txtSHIPPING_CONTACT_ID  ;
		protected TextBox         txtSHIPPING_CONTACT_NAME;
		protected HiddenField     txtBILLING_ACCOUNT_ID   ;
		protected TextBox         txtBILLING_ACCOUNT_NAME ;
		protected HiddenField     txtBILLING_CONTACT_ID   ;
		protected TextBox         txtBILLING_CONTACT_NAME ;
		protected DropDownList    lstPAYMENT_TERMS        ;
		protected DropDownList    lstQUOTE_STAGE          ;
		protected SplendidCRM._controls.DatePicker ctlDATE_QUOTE_EXPECTED_CLOSED;
		protected SplendidCRM._controls.DatePicker ctlORIGINAL_PO_DATE          ;
		public    CommandEventHandler Command ;
		protected SplendidCRM._controls.TeamAssignedMassUpdate ctlTeamAssignedMassUpdate;

		public Guid ASSIGNED_USER_ID
		{
			get
			{
				return ctlTeamAssignedMassUpdate.ASSIGNED_USER;
			}
		}

		public Guid TEAM_ID
		{
			get
			{
				return ctlTeamAssignedMassUpdate.TEAM;
			}
		}


		public Guid SHIPPING_ACCOUNT_ID
		{
			get
			{
				return Sql.ToGuid(txtSHIPPING_ACCOUNT_ID.Value);
			}
		}

		public Guid SHIPPING_CONTACT_ID
		{
			get
			{
				return Sql.ToGuid(txtSHIPPING_CONTACT_ID.Value);
			}
		}

		public Guid BILLING_ACCOUNT_ID
		{
			get
			{
				return Sql.ToGuid(txtBILLING_ACCOUNT_ID.Value);
			}
		}

		public Guid BILLING_CONTACT_ID
		{
			get
			{
				return Sql.ToGuid(txtBILLING_CONTACT_ID.Value);
			}
		}

		public string PAYMENT_TERMS
		{
			get
			{
				return lstPAYMENT_TERMS.SelectedValue;
			}
		}

		public string QUOTE_STAGE
		{
			get
			{
				return lstQUOTE_STAGE.SelectedValue;
			}
		}

		public DateTime DATE_QUOTE_EXPECTED_CLOSED
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlDATE_QUOTE_EXPECTED_CLOSED.Value;
			}
		}

		public DateTime ORIGINAL_PO_DATE
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlORIGINAL_PO_DATE.Value;
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// Command is handled by the parent. 
			if ( Command != null )
				Command(this, e) ;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( !IsPostBack )
				{
					// 06/02/2006 Paul.  Buttons should be hidden if the user does not have access. 
					int nACLACCESS_Delete = Security.GetUserAccess(m_sMODULE, "delete");
					int nACLACCESS_Edit   = Security.GetUserAccess(m_sMODULE, "edit"  );
					btnDelete.Visible = (nACLACCESS_Delete >= 0);
					btnUpdate.Visible = (nACLACCESS_Edit   >= 0);

					lstPAYMENT_TERMS.DataSource = SplendidCache.List("payment_terms_dom");
					lstPAYMENT_TERMS.DataBind();
					lstPAYMENT_TERMS.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstQUOTE_STAGE  .DataSource = SplendidCache.List("quote_stage_dom");
					lstQUOTE_STAGE  .DataBind();
					lstQUOTE_STAGE  .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
