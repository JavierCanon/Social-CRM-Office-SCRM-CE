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
using System.Web;
using System.Web.UI.WebControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for LastViewed.
	/// </summary>
	public class LastViewed : SplendidControl
	{
		protected DataView vwLastViewed;
		protected Repeater ctlRepeater ;

		public void Refresh()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwTRACKER_LastViewed" + ControlChars.CrLf
				     + " where USER_ID = @USER_ID  " + ControlChars.CrLf
				     + " order by DATE_ENTERED desc" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataSet ds = new DataSet() )
							{
								using ( DataTable dt = new DataTable("vwTRACKER") )
								{
									ds.Tables.Add(dt);
									// 08/16/2005 Paul.  Instead of TOP, use Fill to restrict the records. 
									int nHistoryMaxViewed = Sql.ToInteger(Application["CONFIG.history_max_viewed"]);
									if ( nHistoryMaxViewed == 0 )
										nHistoryMaxViewed = 10;
									// 10/18/2005 Paul.  Start record should be 0. 
									da.Fill(ds, 0, nHistoryMaxViewed, "vwTRACKER");
									
									// 08/17/2005 Paul.  Oracle is having a problem returning an integer column. 
									DataColumn colROW_NUMBER = dt.Columns.Add("ROW_NUMBER", Type.GetType("System.Int32"));
									int nRowNumber = 1;
									foreach(DataRow row in dt.Rows)
									{
										// 10/18/2005 Paul.  AccessKey must be in range of 1 to 9. 
										row["ROW_NUMBER"] = Math.Min(nRowNumber, 9);
										nRowNumber++;
									}
									vwLastViewed = dt.DefaultView;
									ctlRepeater.DataSource = vwLastViewed ;
									ctlRepeater.DataBind();
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 12/02/2005 Paul.  Always bind as the repeater does not save its state on postback. 
			Refresh();
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
