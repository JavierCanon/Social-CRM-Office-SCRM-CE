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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Web;
using System.Web.UI.WebControls;

namespace SplendidCRM.Emails._controls
{
	/// <summary>
	///		Summary description for InboundButtons.
	/// </summary>
	public class InboundButtons : SplendidControl
	{
		protected Label  lblError    ;
		protected Button btnForward  ;
		protected Button btnReply    ;
		protected Button btnDelete   ;
		protected Button btnShowRaw  ;
		protected Button btnHideRaw  ;

		public CommandEventHandler Command;

		public void DisableAll()
		{
			btnForward.Enabled = false;
			btnReply  .Enabled = false;
			btnDelete .Enabled = false;
			btnShowRaw.Enabled = false;
			btnHideRaw.Enabled = false;
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

		public bool ShowForward
		{
			get
			{
				return btnForward.Visible;
			}
			set
			{
				btnForward.Visible = value;
			}
		}

		public bool ShowReply
		{
			get
			{
				return btnReply.Visible;
			}
			set
			{
				btnReply.Visible = value;
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

		public bool ShowShowRaw
		{
			get
			{
				return btnShowRaw.Visible;
			}
			set
			{
				btnShowRaw.Visible = value;
			}
		}

		public bool ShowHideRaw
		{
			get
			{
				return btnHideRaw.Visible;
			}
			set
			{
				btnHideRaw.Visible = value;
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
