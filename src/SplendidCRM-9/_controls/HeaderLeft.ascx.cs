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
using System.Web.UI.HtmlControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for HeaderLeft.
	/// </summary>
	public class HeaderLeft : SplendidControl
	{
		protected SplendidCRM.Themes.Sugar.HeaderLeft ctlHeaderLeft;
		protected Panel  pnlHeader;
		protected string sTitle;

		public string Title
		{
			get
			{
				if ( ctlHeaderLeft != null )
					return ctlHeaderLeft.Title;
				else
					return sTitle;
			}
			set
			{
				sTitle = value;
				if ( ctlHeaderLeft != null )
					ctlHeaderLeft.Title = value;
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
			string sTheme = Page.Theme;
			if ( String.IsNullOrEmpty(sTheme) )
				sTheme = "Sugar";
			string sHeaderLeftPath = "~/App_MasterPages/" + Page.Theme + "/HeaderLeft.ascx";
			if ( System.IO.File.Exists(MapPath(sHeaderLeftPath)) )
			{
				ctlHeaderLeft = LoadControl(sHeaderLeftPath) as SplendidCRM.Themes.Sugar.HeaderLeft;
				if ( ctlHeaderLeft != null )
				{
					ctlHeaderLeft.Title = sTitle;
					pnlHeader.Controls.Add(ctlHeaderLeft);
				}
			}
		}
		#endregion
	}
}
