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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using SplendidCRM._controls;

namespace SplendidCRM.Calendar
{
	/// <summary>
	///		Summary description for NewRecord.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		protected Label                      lblError          ;
		protected RadioButton                radScheduleCall   ;
		protected RadioButton                radScheduleMeeting;
		protected TextBox                    txtNAME           ;
		protected Label                      lblDATEFORMAT     ;
		protected Label                      lblTIMEFORMAT     ;
		protected DatePicker                 ctlDATE_START     ;
		protected TextBox                    txtTIME_START     ;
		protected RequiredFieldValidator     reqNAME           ;
		protected RequiredFieldValidator     reqTIME_START     ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord" )
			{
				reqNAME      .Enabled = true;
				reqTIME_START.Enabled = true;
				reqNAME      .Validate();
				reqTIME_START.Validate();
				if ( Page.IsValid )
				{
					Guid gID = Guid.Empty;
					try
					{
						// 02/28/2006 Paul.  The easiest way to parse the two separate date/time fields is to combine the text. 
						DateTime dtDATE_START = T10n.ToServerTime(Sql.ToDateTime(ctlDATE_START.DateText + " " + txtTIME_START.Text));
						if ( radScheduleCall.Checked )
							SqlProcs.spCALLS_New(ref gID, txtNAME.Text, dtDATE_START);
						else
							SqlProcs.spMEETINGS_New(ref gID, txtNAME.Text, dtDATE_START);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					if ( !Sql.IsEmptyGuid(gID) )
					{
						if ( radScheduleCall.Checked )
							Response.Redirect("~/Calls/view.aspx?ID=" + gID.ToString());
						else
							Response.Redirect("~/Meetings/view.aspx?ID=" + gID.ToString());
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			radScheduleCall   .Visible = (SplendidCRM.Security.GetUserAccess("Calls"   , "edit") >= 0);
			radScheduleMeeting.Visible = (SplendidCRM.Security.GetUserAccess("Meetings", "edit") >= 0);

			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			this.Visible = radScheduleCall.Visible || radScheduleMeeting.Visible;
			if ( !this.Visible )
				return;

			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Need to bind so that Text of the Button gets updated. 
			reqNAME      .ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Calls.LBL_LIST_SUBJECT") + "<br>";
			reqTIME_START.ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Calls.LBL_LIST_TIME"   ) + "<br>";
			if ( !IsPostBack )
			{
				DateTime dt1100PM = DateTime.Today.AddHours(23);
				lblDATEFORMAT.Text = "(" + Session["USER_SETTINGS/DATEFORMAT"] + ")";
				lblTIMEFORMAT.Text = "(" + dt1100PM.ToShortTimeString() + ")";

				DateTime dtNow = T10n.FromServerTime(DateTime.Now);
				ctlDATE_START.Value = dtNow;
				txtTIME_START.Text  = Sql.ToTimeString(dtNow);
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
