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
using System.Xml;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Feeds
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		protected Label                      lblError;
		protected TextBox                    txtURL  ;
		protected RequiredFieldValidator     reqURL  ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord" )
			{
				reqURL.Enabled = true;
				reqURL.Validate();
				if ( Page.IsValid )
				{
					try
					{
						// 03/13/2007 Paul.  Pull the XML processing outside the database transaction.  
						// It is not a good idea to lock the database while waiting for the XML file to download. 
						// 07/15/2006 Paul.  Require HTTP protocol to prevent user from trying to access the file system. 
						if ( !txtURL.Text.ToLower().StartsWith("http://") && !txtURL.Text.ToLower().StartsWith("https://") )
							throw(new Exception("Invalid URL."));
						// 12/06/2005 Paul.  Can't use the DataSet reader because it returns the following error:
						// The same table (description) cannot be the child table in two nested relations, caused by News.com feed. 
						XmlDocument xml = new XmlDocument();
						xml.Load(txtURL.Text);

						Guid gID = Guid.Empty;
						// 11/22/2006 Paul.  Use a transaction because we added team management. 
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							using ( IDbTransaction trn = con.BeginTransaction() )
							{
								try
								{
									string sTITLE       = XmlUtil.SelectSingleNode(xml, "channel/title"      );
									string sDESCRIPTION = XmlUtil.SelectSingleNode(xml, "channel/description");
									// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
									SqlProcs.spFEEDS_Update
										( ref gID
										, Security.USER_ID
										, sTITLE
										, sDESCRIPTION
										, txtURL.Text
										, Security.TEAM_ID
										, trn
										);
									// 01/31/2007 Paul.  The transaction was not being committed. 
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									throw(new Exception(ex.Message));
								}
							}
						}
						if ( !Sql.IsEmptyGuid(gID) )
							Response.Redirect("~/Feeds/view.aspx?ID=" + gID.ToString());
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Need to bind so that Text of the Button gets updated. 
			reqURL.ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Feeds.LBL_RSS_URL") + "<br>";
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
			m_sMODULE = "Feeds";
		}
		#endregion
	}
}
