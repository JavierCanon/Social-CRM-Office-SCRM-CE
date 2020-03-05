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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Dashboard
{
	/// <summary>
	///		Summary description for OppByLeadSource.
	/// </summary>
	public class OppByLeadSource : SplendidControl
	{
		protected ListBox                   lstLEAD_SOURCE;
		protected ListBox                   lstUSERS      ;
		protected bool                       bShowEditDialog = false;

		protected string PipelineQueryString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CHART_LENGTH=10");
			foreach(ListItem item in lstUSERS.Items)
			{
				if ( item.Selected )
				{
					sb.Append("&ASSIGNED_USER_ID=");
					sb.Append(Server.UrlEncode(item.Value));
				}
			}
			foreach(ListItem item in lstLEAD_SOURCE.Items)
			{
				if ( item.Selected )
				{
					sb.Append("&LEAD_SOURCE=");
					sb.Append(Server.UrlEncode(item.Value));
				}
			}
			// 09/15/2005 Paul.  The hBarS flash will append a "?0.12341234" timestamp to the URL. 
			// Use a bogus parameter to separate the timestamp from the last sales stage. 
			sb.Append("&TIME_STAMP=");
			return sb.ToString();
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Submit" )
			{
				if ( Page.IsValid )
				{
					ViewState["OppByLeadSourceByOutcomeQueryString"] = PipelineQueryString();
				}
				// 01/19/2007 Paul.  Keep the edit dialog visible.
				bShowEditDialog = true;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				lstLEAD_SOURCE.DataSource = SplendidCache.List("lead_source_dom");
				lstLEAD_SOURCE.DataBind();
				lstLEAD_SOURCE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				lstUSERS.DataSource = SplendidCache.ActiveUsers();
				lstUSERS.DataBind();
				// 09/14/2005 Paul.  Default to today, and all leads. 
				foreach(ListItem item in lstLEAD_SOURCE.Items)
				{
					item.Selected = true;
				}
				foreach(ListItem item in lstUSERS.Items)
				{
					item.Selected = true;
				}
				// 09/15/2005 Paul.  Maintain the pipeline query string separately so that we can respond to specific submit requests 
				// and ignore all other control events on the page. 
				ViewState["OppByLeadSourceByOutcomeQueryString"] = PipelineQueryString();
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
