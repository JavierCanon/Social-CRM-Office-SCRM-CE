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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Threads
{
	/// <summary>
	/// Summary description for ThreadView.
	/// </summary>
	public class ThreadView : SplendidControl
	{
		protected PlaceHolder  plcPosts;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Guid gID = Sql.ToGuid(Request["ID"]);
				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				//if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwTHREADS_POSTS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;

								// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
								Security.Filter(cmd, m_sMODULE, "list");
								cmd.CommandText += "   and THREAD_ID = @THREAD_ID" + ControlChars.CrLf;
								cmd.CommandText += " order by DATE_ENTERED asc   " + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@THREAD_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwTHREADS_POSTS", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);

										foreach ( DataRow rdr in dt.Rows )
										{
											PostView ctlPost = LoadControl("PostView.ascx") as PostView;
											plcPosts.Controls.Add(ctlPost);

											ctlPost.POST_ID       = Sql.ToGuid  (rdr["ID"              ]);
											ctlPost.TITLE         = Sql.ToString(rdr["TITLE"           ]);
											ctlPost.CREATED_BY    = Sql.ToString(rdr["CREATED_BY"      ]);
											ctlPost.DATE_ENTERED  = Sql.ToString(rdr["DATE_ENTERED"    ]);
											ctlPost.MODIFIED_BY   = Sql.ToString(rdr["MODIFIED_BY"     ]);
											ctlPost.DATE_MODIFIED = Sql.ToString(rdr["DATE_MODIFIED"   ]);
											ctlPost.DESCRIPTION   = Sql.ToString(rdr["DESCRIPTION_HTML"]);
											if ( Sql.ToDateTime(rdr["DATE_ENTERED"]) != Sql.ToDateTime(rdr["DATE_MODIFIED"]) )
												ctlPost.Modified = true;

											Guid gCREATED_BY_ID = Sql.ToGuid(rdr["CREATED_BY_ID"]);
											ctlPost.ShowEdit   = Security.IS_ADMIN || gCREATED_BY_ID == Security.USER_ID;
											ctlPost.ShowDelete = Security.IS_ADMIN || gCREATED_BY_ID == Security.USER_ID;
										}
									}
								}
							}
						}
					}
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Posts";
		}
		#endregion
	}
}
