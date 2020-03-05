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

namespace SplendidCRM.Orders._controls
{
	/// <summary>
	///		Summary description for OrderDetailButtons.
	/// </summary>
	
	// 02/27/2006 Paul.  Can't name the file DetailButtons even though it is a separate namespace. 
	// CS1595: '_ASP.DetailButtons_ascx' is defined in multiple places; 
	public class OrderDetailButtons : SplendidCRM._controls.DetailButtons
	{
		protected Button btnConvert;

		public bool EnableConvert
		{
			get
			{
				return btnConvert.Enabled;
			}
			set
			{
				btnConvert.Enabled = value;
			}
		}

		public bool ShowConvert
		{
			get
			{
				return btnConvert.Visible;
			}
			set
			{
				btnConvert.Visible = value;
			}
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
