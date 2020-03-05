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

namespace SplendidCRM.Campaigns._controls
{
	/// <summary>
	///		Summary description for CampaignButtons.
	/// </summary>
	public class CampaignButtons : SplendidCRM._controls.DetailButtons
	{
		protected Button    btnSendTest    ;
		protected Button    btnSendEmails  ;
		protected Button    btnMailMerge   ;
		protected Button    btnDeleteTest  ;
		protected HyperLink lnkViewTrack   ;
		protected HyperLink lnkViewDetails ;
		protected HyperLink lnkViewROI     ;


		public bool ShowSendTest
		{
			get { return btnSendTest.Visible; }
			set { btnSendTest.Visible = value; }
		}

		public bool ShowSendEmails
		{
			get { return btnSendEmails.Visible; }
			set { btnSendEmails.Visible = value; }
		}

		public bool ShowMailMerge
		{
			get { return btnMailMerge.Visible; }
			set { btnMailMerge.Visible = value; }
		}

		public bool ShowDeleteTest
		{
			get { return btnDeleteTest.Visible; }
			set
			{
				btnDeleteTest.Visible = value;
				if ( btnDeleteTest.Visible )
				{
					btnEdit      .Visible = false;
					btnDuplicate .Visible = false;
					btnDelete    .Visible = false;
					btnSendTest  .Visible = false;
					btnSendEmails.Visible = false;
					btnMailMerge .Visible = false;
				}
			}
		}

		public bool ShowViewTrack
		{
			get { return lnkViewTrack.Visible; }
			set { lnkViewTrack.Visible = value; }
		}

		public bool ShowViewDetails
		{
			get { return lnkViewDetails.Visible; }
			set { lnkViewDetails.Visible = value; }
		}

		public bool ShowViewROI
		{
			get { return lnkViewROI.Visible; }
			set { lnkViewROI.Visible = value; }
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
