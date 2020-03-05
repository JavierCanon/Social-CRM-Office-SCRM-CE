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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.TeamNotices
{
	/// <summary>
	///		Summary description for MyTeamNotices.
	/// </summary>
	public class MyTeamNotices : SplendidControl
	{
		protected DataView vwTeamNotices;
		protected Repeater ctlRepeater ;

		private void Page_Load(object sender, System.EventArgs e)
		{
			bool bEnableTeamManagement = Crm.Config.enable_team_management();
			if ( !bEnableTeamManagement )
			{
				this.Visible = false;
				return;
			}
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                         " + ControlChars.CrLf
					     + "  from      vwTEAM_NOTICES_MyList" + ControlChars.CrLf
					     + " inner join vwTEAM_MEMBERSHIPS   " + ControlChars.CrLf
					     + "         on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID" + ControlChars.CrLf
					     + "        and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf
					     + " where @CURRENT_DATE between DATE_START and DATE_END" + ControlChars.CrLf
					     + " order by DATE_START             " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
						Sql.AddParameter(cmd, "@CURRENT_DATE", T10n.ToServerTime(DateTime.Now));

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwTeamNotices = dt.DefaultView;
								ctlRepeater.DataSource = vwTeamNotices ;
								if ( !IsPostBack )
								{
									ctlRepeater.DataBind();
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
