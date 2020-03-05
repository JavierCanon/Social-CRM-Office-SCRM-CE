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
	///		Summary description for PipelineBySalesStage.
	/// </summary>
	public class PipelineBySalesStage : SplendidControl
	{
		protected _controls.ChartDatePicker ctlDATE_START ;
		protected _controls.ChartDatePicker ctlDATE_END   ;
		protected ListBox                   lstSALES_STAGE;
		protected ListBox                   lstUSERS      ;
		protected bool                       bShowEditDialog = false;

		protected string PipelineQueryString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CHART_LENGTH=10");
			sb.Append("&DATE_START=");
			// 07/09/2006 Paul.  The date is passed as TimeZone time, which is what the control value returns, so no conversion is necessary. 
			sb.Append(Server.UrlEncode(Sql.ToDateString(ctlDATE_START.Value)));
			sb.Append("&DATE_END=");
			sb.Append(Server.UrlEncode(Sql.ToDateString(ctlDATE_END.Value)));
			foreach(ListItem item in lstUSERS.Items)
			{
				if ( item.Selected )
				{
					sb.Append("&ASSIGNED_USER_ID=");
					sb.Append(Server.UrlEncode(item.Value));
				}
			}
			foreach(ListItem item in lstSALES_STAGE.Items)
			{
				if ( item.Selected )
				{
					sb.Append("&SALES_STAGE=");
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
				ctlDATE_START.Validate();
				ctlDATE_END  .Validate();
				if ( Page.IsValid )
				{
					ViewState["PipelineBySalesStageQueryString"] = PipelineQueryString();
				}
				// 01/19/2007 Paul.  Keep the edit dialog visible.
				bShowEditDialog = true;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				lstSALES_STAGE.DataSource = SplendidCache.List("sales_stage_dom");
				lstSALES_STAGE.DataBind();
				lstUSERS.DataSource = SplendidCache.ActiveUsers();
				lstUSERS.DataBind();
				// 09/14/2005 Paul.  Default to today, and all sales stages. 
				foreach(ListItem item in lstSALES_STAGE.Items)
				{
					item.Selected = true;
				}
				foreach(ListItem item in lstUSERS.Items)
				{
					item.Selected = true;
				}
				// 07/09/2006 Paul.  The date is passed as TimeZone time, so convert from server time. 
				ctlDATE_START.Value = T10n.FromServerTime(DateTime.Today);
				ctlDATE_END  .Value = T10n.FromServerTime(new DateTime(2100, 1, 1));
				// 09/15/2005 Paul.  Maintain the pipeline query string separately so that we can respond to specific submit requests 
				// and ignore all other control events on the page. 
				ViewState["PipelineBySalesStageQueryString"] = PipelineQueryString();
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
