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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Opportunities
{
	/// <summary>
	///		Summary description for MyPipeline.
	/// </summary>
	public class MyPipeline : SplendidControl
	{
		protected _controls.ChartDatePicker ctlDATE_START ;
		protected _controls.ChartDatePicker ctlDATE_END   ;
		protected ListBox                   lstSALES_STAGE;
		// 08/31/2006 Paul.  Provide an additional error message that will appear even when the edit box is not visible. 
		protected DatePickerValidator       valDATE_START ;
		protected DatePickerValidator       valDATE_END   ;
		protected bool                      bShowEditDialog = false;
		protected string                    sDetailView   ;

		public string DetailView
		{
			get { return sDetailView; }
			set { sDetailView = value; }
		}

		protected string PipelineQueryString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("DATE_START=");
			// 07/09/2006 Paul.  The date is passed as TimeZone time, which is what the control value returns, so no conversion is necessary. 
			sb.Append(Server.UrlEncode(Sql.ToDateString(ctlDATE_START.Value)));
			sb.Append("&DATE_END=");
			sb.Append(Server.UrlEncode(Sql.ToDateString(ctlDATE_END.Value)));
			// 09/16/2005 Paul.  Since this is MyPipeline, specify current user. 
			sb.Append("&ASSIGNED_USER_ID=");
			sb.Append(Server.UrlEncode(Security.USER_ID.ToString()));
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
				valDATE_START.Enabled = true;
				valDATE_END  .Enabled = true;
				ctlDATE_START.Validate();
				ctlDATE_END  .Validate();
				valDATE_START.Validate();
				valDATE_END .Validate();
				if ( Page.IsValid )
				{
					ViewState["MyPipelineQueryString"] = PipelineQueryString();
				}
				// 01/19/2007 Paul.  Keep the edit dialog visible.
				bShowEditDialog = true;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 11/05/2007 Paul.  Don't show panel if it was manually hidden. 
			this.Visible = this.Visible && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			// 09/09/2007 Paul.  We are having trouble dynamically adding user controls to the WebPartZone. 
			// Instead, control visibility manually here.  This approach as the added benefit of hiding the 
			// control even if the WebPartManager has moved it to an alternate zone. 
			if ( this.Visible && this.Visible && !Sql.IsEmptyString(sDetailView) )
			{
				// 01/17/2008 Paul.  We need to use the sDetailView property and not the hard-coded view name. 
				DataView vwFields = new DataView(SplendidCache.DetailViewRelationships(sDetailView));
				vwFields.RowFilter = "CONTROL_NAME = '~/Opportunities/MyPipeline'";
				this.Visible = vwFields.Count > 0;
			}
			if ( !this.Visible )
				return;

			valDATE_START.ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			valDATE_END  .ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			if ( !IsPostBack )
			{
				lstSALES_STAGE.DataSource = SplendidCache.List("sales_stage_dom");
				lstSALES_STAGE.DataBind();
				// 09/14/2005 Paul.  Default to today, and all sales stages. 
				foreach(ListItem item in lstSALES_STAGE.Items)
				{
					item.Selected = true;
				}
				// 07/09/2006 Paul.  The date is passed in TimeZone time, so convert from server time. 
				ctlDATE_START.Value = T10n.FromServerTime(DateTime.Today);
				ctlDATE_END  .Value = T10n.FromServerTime(new DateTime(2100, 1, 1));
				// 09/15/2005 Paul.  Maintain the pipeline query string separately so that we can respond to specific submit requests 
				// and ignore all other control events on the page. 
				ViewState["MyPipelineQueryString"] = PipelineQueryString();
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
			m_sMODULE = "Opportunities";
		}
		#endregion
	}
}
