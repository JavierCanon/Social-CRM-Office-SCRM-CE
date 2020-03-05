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

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for ParentPopupScripts.
	/// </summary>
	public class ParentPopupScripts : SplendidControl
	{
		// 08/23/2006 Paul.  Specify default values for the fields. 
		protected string sListField   = "PARENT_TYPE";
		protected string sNameField   = "PARENT_NAME";
		protected string sHiddenField = "PARENT_ID"  ;

		public string ListField
		{
			get { return sListField; }
			set { sListField = value; }
		}

		public string NameField
		{
			get { return sNameField; }
			set { sNameField = value; }
		}

		public string HiddenField
		{
			get { return sHiddenField; }
			set { sHiddenField = value; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// ListField="lstPARENT_TYPE" NameField="txtPARENT_NAME" HiddenField="txtPARENT_ID" 
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
