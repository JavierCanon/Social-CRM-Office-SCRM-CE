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
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Activities
{
	/// <summary>
	///		Summary description for MyActivities.
	/// </summary>
	public class MyActivities : SplendidControl
	{
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected DropDownList  lstTHROUGH     ;
		protected Label         txtTHROUGH     ;
		protected string        sDetailView    ;

		public string DetailView
		{
			get { return sDetailView; }
			set { sDetailView = value; }
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				Guid gID = Sql.ToGuid(e.CommandArgument);
				switch ( e.CommandName )
				{
					case "Activity.Accept"   :  SqlProcs.spACTIVITIES_UpdateStatus(gID, Security.USER_ID, "Accept"   );  break;
					case "Activity.Tentative":  SqlProcs.spACTIVITIES_UpdateStatus(gID, Security.USER_ID, "Tentative");  break;
					case "Activity.Decline"  :  SqlProcs.spACTIVITIES_UpdateStatus(gID, Security.USER_ID, "Decline"  );  break;
				}
				// 08/31/2006 Paul.  Instead of redirecting, which we prefer, we are going to bind again
				// so that the THROUGH dropdown will not get reset. 
				Bind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 09/09/2006 Paul.  Visibility is already controlled by the ASPX page, 
			// but since this control is used on the home page, we need to apply the module specific rules. 
			// 11/05/2007 Paul.  Don't show panel if it was manually hidden. 
			this.Visible = this.Visible && (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			// 09/09/2007 Paul.  We are having trouble dynamically adding user controls to the WebPartZone. 
			// Instead, control visibility manually here.  This approach as the added benefit of hiding the 
			// control even if the WebPartManager has moved it to an alternate zone. 
			if ( this.Visible && !Sql.IsEmptyString(sDetailView) )
			{
				// 01/17/2008 Paul.  We need to use the sDetailView property and not the hard-coded view name. 
				DataView vwFields = new DataView(SplendidCache.DetailViewRelationships(sDetailView));
				vwFields.RowFilter = "CONTROL_NAME = '~/Activities/MyActivities'";
				this.Visible = vwFields.Count > 0;
			}
			if ( !this.Visible )
				return;

			if ( !IsPostBack )
			{
				lstTHROUGH.DataSource =  SplendidCache.List("appointment_filter_dom");
				lstTHROUGH.DataBind();
			}
			Bind();
		}

		private void Bind()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				// 04/04/2006 Paul.  Start with today in ZoneTime and not ServerTime. 
				DateTime dtZONE_NOW   = T10n.FromUniversalTime(DateTime.Now.ToUniversalTime());
				DateTime dtZONE_TODAY = new DateTime(dtZONE_NOW.Year, dtZONE_NOW.Month, dtZONE_NOW.Day);
				DateTime dtDATE_START = dtZONE_TODAY;
				switch ( lstTHROUGH.SelectedValue )
				{
					case "today"          :  dtDATE_START = dtZONE_TODAY;  break;
					case "tomorrow"       :  dtDATE_START = dtDATE_START.AddDays(1);  break;
					case "this Saturday"  :  dtDATE_START = dtDATE_START.AddDays(DayOfWeek.Saturday-dtDATE_START.DayOfWeek);  break;
					case "next Saturday"  :  dtDATE_START = dtDATE_START.AddDays(DayOfWeek.Saturday-dtDATE_START.DayOfWeek).AddDays(7);  break;
					case "last this_month":  dtDATE_START = new DateTime(dtZONE_TODAY.Year, dtZONE_TODAY.Month, DateTime.DaysInMonth(dtZONE_TODAY.Year, dtZONE_TODAY.Month));  break;
					case "last next_month":  dtDATE_START = new DateTime(dtZONE_TODAY.Year, dtZONE_TODAY.Month, DateTime.DaysInMonth(dtZONE_TODAY.Year, dtZONE_TODAY.Month)).AddMonths(1);  break;
				}
				
				// 04/04/2006 Paul.  Now that we are using ZoneTime, we don't need to convert it to server time when displaying the date. 
				txtTHROUGH.Text = "(" + Sql.ToDateString(dtDATE_START) + ")";
				sSQL = "select *                  " + ControlChars.CrLf
				     + "  from vwACTIVITIES_MyList" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
					Security.Filter(cmd, m_sMODULE, "list");
					Sql.AppendParameter(cmd, Security.USER_ID, "ASSIGNED_USER_ID", false);
					cmd.CommandText += "   and DATE_START < @DATE_START" + ControlChars.CrLf;
					cmd.CommandText += " order by DATE_START asc       " + ControlChars.CrLf;
					// 04/04/2006 Paul.  DATE_START is not including all records for today. 
					// 04/04/2006 Paul.  Instead of using DATE_START <= @DATE_START, change to DATE_START < @DATE_START and increase the start date to tomorrow. 
					// 04/04/2006 Paul.  Here we do need to convert it to ServerTime because that is all that the database understands. 
					Sql.AddParameter(cmd, "@DATE_START"      , T10n.ToServerTime(dtDATE_START.AddDays(1)));

					if ( bDebug )
						RegisterClientScriptBlock("vwACTIVITIES_MyList", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									//grdMain.SortColumn = "DATE_START";
									//grdMain.SortOrder  = "desc" ;
								}
								// 09/15/2005 Paul. We must always bind, otherwise a Dashboard refresh will display the grid with empty rows. 
								grdMain.DataBind();
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
			// 11/25/2006 Paul.  At some point we will need to stop using vwACTIVITIES_MyList
			// and apply security to Calls and Meetings separtely.  For now, just treat all activities as Calls. 
			m_sMODULE = "Calls";
			this.AppendGridColumns(grdMain, "Activities.MyActivities");
		}
		#endregion
	}
}
