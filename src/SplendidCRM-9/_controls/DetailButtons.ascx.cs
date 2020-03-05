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
using System.Web;
using System.Web.UI.WebControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for DetailButtons.
	/// </summary>
	public class DetailButtons : SplendidControl
	{
		protected Label  lblError    ;
		protected Button btnEdit     ;
		protected Button btnDuplicate;
		protected Button btnDelete   ;

		public CommandEventHandler Command;

		public void DisableAll()
		{
			btnEdit     .Enabled = false;
			btnDuplicate.Enabled = false;
			btnDelete   .Enabled = false;
		}

		public bool EnableEdit
		{
			get
			{
				return btnEdit.Enabled;
			}
			set
			{
				btnEdit.Enabled = value;
			}
		}

		public bool EnableDuplicate
		{
			get
			{
				return btnDuplicate.Enabled;
			}
			set
			{
				btnDuplicate.Enabled = value;
			}
		}

		public bool EnableDelete
		{
			get
			{
				return btnDelete.Enabled;
			}
			set
			{
				btnDelete.Enabled = value;
			}
		}

		public bool ShowEdit
		{
			get
			{
				return btnEdit.Visible;
			}
			set
			{
				btnEdit.Visible = value;
			}
		}

		public bool ShowDuplicate
		{
			get
			{
				return btnDuplicate.Visible;
			}
			set
			{
				btnDuplicate.Visible = value;
			}
		}

		public bool ShowDelete
		{
			get
			{
				return btnDelete.Visible;
			}
			set
			{
				btnDelete.Visible = value;
			}
		}

		public string ErrorText
		{
			get
			{
				return lblError.Text;
			}
			set
			{
				lblError.Text = value;
			}
		}

		// 04/27/2006 Paul.  This function should be virtual so that it could be 
		// over-ridden by LeadDetailButtons, or ProspectDetailButtons.
		public virtual void SetUserAccess(string sMODULE_NAME, Guid gASSIGNED_USER_ID)
		{
			// 05/22/2006 Paul.  Disable button if NOT Owner.
			int nACLACCESS_Delete = Security.GetUserAccess(sMODULE_NAME, "delete");
			if ( nACLACCESS_Delete == ACL_ACCESS.NONE || (nACLACCESS_Delete == ACL_ACCESS.OWNER && Security.USER_ID != gASSIGNED_USER_ID) )
			{
				btnDelete.Visible = false;
			}
			
			// 05/22/2006 Paul.  Disable button if NOT Owner.
			int nACLACCESS_Edit = Security.GetUserAccess(sMODULE_NAME, "edit");
			if ( nACLACCESS_Edit == ACL_ACCESS.NONE || (nACLACCESS_Edit == ACL_ACCESS.OWNER && Security.USER_ID != gASSIGNED_USER_ID) )
			{
				btnEdit.Visible      = false;
				btnDuplicate.Visible = false;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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
