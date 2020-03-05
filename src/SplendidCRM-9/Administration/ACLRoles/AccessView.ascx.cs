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

namespace SplendidCRM.Administration.ACLRoles
{
	/// <summary>
	///		Summary description for AccessView.
	/// </summary>
	public class AccessView : SplendidControl
	{
		protected Guid          gID            ;
		protected DataView      vwMain         ;
		protected ACLGrid       grdACL         ;
		protected Label         lblError       ;
		protected Guid          gUSER_ID       ;

		public bool EnableACLEditing
		{
			get { return grdACL.EnableACLEditing; }
			set { grdACL.EnableACLEditing = value; }
		}

		public Guid USER_ID
		{
			get { return gUSER_ID; }
			set { gUSER_ID = value; }
		}

		// 04/25/2006 Paul.  FindControl needs to be executed on the DataGridItem.  I'm not sure why.
		public DropDownList FindACLControl(string sMODULE_NAME, string sACCESS_TYPE)
		{
			return grdACL.FindACLControl(sMODULE_NAME, sACCESS_TYPE);
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		public void BindGrid()
		{
			// 12/07/2006 Paul.  We need to be able to force the grid to be rebound when its data has changed. 
			Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				if ( !Sql.IsEmptyGuid(gUSER_ID) )
				{
					sSQL = "select MODULE_NAME          " + ControlChars.CrLf
					     + "     , DISPLAY_NAME         " + ControlChars.CrLf
					     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
					     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
					     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
					     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
					     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
					     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
					     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
					     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
					     + "  from vwACL_ACCESS_ByUser  " + ControlChars.CrLf
					     + " where USER_ID = @USER_ID   " + ControlChars.CrLf
					     + " order by MODULE_NAME       " + ControlChars.CrLf;
				}
				else if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
				{
					sSQL = "select MODULE_NAME          " + ControlChars.CrLf
					     + "     , DISPLAY_NAME         " + ControlChars.CrLf
					     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
					     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
					     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
					     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
					     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
					     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
					     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
					     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
					     + "  from vwACL_ACCESS_ByRole  " + ControlChars.CrLf
					     + " where ROLE_ID = @ROLE_ID   " + ControlChars.CrLf
					     + " order by MODULE_NAME       " + ControlChars.CrLf;
				}
				else
				{
					sSQL = "select MODULE_NAME          " + ControlChars.CrLf
					     + "     , DISPLAY_NAME         " + ControlChars.CrLf
					     + "     , ACLACCESS_ADMIN      " + ControlChars.CrLf
					     + "     , ACLACCESS_ACCESS     " + ControlChars.CrLf
					     + "     , ACLACCESS_VIEW       " + ControlChars.CrLf
					     + "     , ACLACCESS_LIST       " + ControlChars.CrLf
					     + "     , ACLACCESS_EDIT       " + ControlChars.CrLf
					     + "     , ACLACCESS_DELETE     " + ControlChars.CrLf
					     + "     , ACLACCESS_IMPORT     " + ControlChars.CrLf
					     + "     , ACLACCESS_EXPORT     " + ControlChars.CrLf
					     + "  from vwACL_ACCESS_ByModule" + ControlChars.CrLf
					     + " order by MODULE_NAME       " + ControlChars.CrLf;
				}
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					if ( !Sql.IsEmptyGuid(gUSER_ID) )
					{
						Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
						gID = Guid.Empty;
					}
					else if ( !Sql.IsEmptyGuid(gDuplicateID) )
					{
						Sql.AddParameter(cmd, "@ROLE_ID", gDuplicateID);
						gID = Guid.Empty;
					}
					else if ( !Sql.IsEmptyGuid(gID) )
					{
						Sql.AddParameter(cmd, "@ROLE_ID", gID);
					}

					if ( bDebug )
						RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							vwMain = dt.DefaultView;
							grdACL.DataSource = vwMain ;
							// 04/26/2006 Paul.  Normally, we would only bind if not a postback, 
							// but the ACL grid knows how to handle the postback state, so we must always bind. 
							grdACL.DataBind();
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				BindGrid();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
			// 05/03/2006 Paul.  Remove the page data binding as it clears the binding on UserRolesView.ascx. 
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
