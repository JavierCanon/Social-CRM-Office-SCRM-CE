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
using System.Net;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Feeds
{
	/// <summary>
	///		Summary description for FeedSummaryView.
	/// </summary>
	public class FeedSummaryView : SplendidControl
	{
		protected Guid      gID             ;
		protected Label     lblError        ;
		protected string    sChannelTitle   ;
		protected string    sChannelLink    ;
		protected string    sLastBuildDate  ;
		protected Label     lblLastBuildDate;
		protected string    sURL            ;
		protected Repeater  rpFeed          ;
		protected DataTable dtChannel       ;
		protected DataTable dtItems         ;

		public Guid FEED_ID
		{
			get
			{
				return gID;
			}
			set
			{
				gID = value;
			}
		}

		public string URL
		{
			get
			{
				return sURL;
			}
			set
			{
				sURL = value;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "MoveUp" )
				{
				}
				else if ( e.CommandName == "MoveDown" )
				{
				}
				else if ( e.CommandName == "Delete" )
				{
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// 12/06/2005 Paul.  Can't use the DataSet reader because it returns the following error:
				// The same table (description) cannot be the child table in two nested relations, caused by News.com feed. 
				XmlDocument xml = new XmlDocument();
				xml.Load(sURL);
				sChannelTitle  = XmlUtil.SelectSingleNode(xml, "channel/title"        );
				sChannelLink   = XmlUtil.SelectSingleNode(xml, "channel/link"         );
				sLastBuildDate = XmlUtil.SelectSingleNode(xml, "channel/lastBuildDate");
				if ( !Sql.IsEmptyString(sLastBuildDate) )
				{
					sLastBuildDate = L10n.Term("Feeds.LBL_LAST_UPDATED") + ": " + sLastBuildDate;
				}
				lblLastBuildDate.Text = sLastBuildDate;

				dtItems = new DataTable();
				DataColumn colTitle       = new DataColumn("title"      , Type.GetType("System.String"));
				DataColumn colLink        = new DataColumn("link"       , Type.GetType("System.String"));
				DataColumn colDescription = new DataColumn("description", Type.GetType("System.String"));
				DataColumn colCategory    = new DataColumn("category"   , Type.GetType("System.String"));
				DataColumn colPubDate     = new DataColumn("pubDate"    , Type.GetType("System.String"));
				dtItems.Columns.Add(colTitle      );
				dtItems.Columns.Add(colLink       );
				dtItems.Columns.Add(colDescription);
				dtItems.Columns.Add(colCategory   );
				dtItems.Columns.Add(colPubDate    );
				try
				{
					XmlNodeList nl = xml.DocumentElement.SelectNodes("channel/item");
					int nRows = 0;
					foreach(XmlNode item in nl)
					{
						DataRow row = dtItems.NewRow();
						dtItems.Rows.Add(row);
						row["title"      ] = XmlUtil.SelectSingleNode(item, "title"      );
						row["link"       ] = XmlUtil.SelectSingleNode(item, "link"       );
						row["description"] = XmlUtil.SelectSingleNode(item, "description");
						row["category"   ] = XmlUtil.SelectSingleNode(item, "category"   );
						row["pubDate"    ] = XmlUtil.SelectSingleNode(item, "pubDate"    );
						nRows++;
						if ( nRows == 5 )
							break;
					}
					rpFeed.DataSource = dtItems;
					rpFeed.DataBind();
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// Ignore errors for now. 
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
			}
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//Page.DataBind();
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
