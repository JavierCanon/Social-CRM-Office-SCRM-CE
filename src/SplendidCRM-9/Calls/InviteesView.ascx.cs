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

namespace SplendidCRM.Calls
{
	/// <summary>
	///		Summary description for InviteesView.
	/// </summary>
	public class InviteesView : SplendidControl
	{
		protected DataView           vwMain         ;
		protected SplendidGrid       grdMain        ;
		protected Label              lblError       ;
		protected HtmlGenericControl divInvitees    ;
		protected SearchInvitees     ctlSearch      ;
		protected string[]           arrINVITEES    ;

		public CommandEventHandler Command ;

		public string[] INVITEES
		{
			get
			{
				return arrINVITEES;
			}
			set
			{
				arrINVITEES = value;
			}
		}

		public bool IsExistingInvitee(string sINVITEE_ID)
		{
			if ( arrINVITEES != null )
			{
				foreach(string s in arrINVITEES)
				{
					if ( s == sINVITEE_ID )
						return true;
				}
			}
			return false;
		}
		
		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					if ( Command != null )
						Command(this, e) ;
					BindInvitees();
					ViewState["InviteesSearch"] = true;
				}
				else if ( e.CommandName == "Invitees.Add" )
				{
					if ( Command != null )
						Command(this, e) ;
					BindInvitees();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void BindInvitees()
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                                                               " + ControlChars.CrLf
					     + "  from vwACTIVITIES_Invitees                                           " + ControlChars.CrLf
					     + " where (INVITEE_TYPE = 'Users' or ASSIGNED_USER_ID = @ASSIGNED_USER_ID)" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", Security.USER_ID);
						ctlSearch.SqlSearchClause(cmd);
						cmd.CommandText += " order by INVITEE_TYPE desc, LAST_NAME asc, FIRST_NAME asc" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock("vwACTIVITIES_Invitees", Sql.ClientScriptBlock(cmd));

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
									grdMain.DataBind();
									divInvitees.Visible = true;
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
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( Sql.ToBoolean(ViewState["InviteesSearch"]) )
				BindInvitees();
			//if ( !IsPostBack )
			{
				// 02/21/2006 Paul.  Don't DataBind, otherwise it will cause the DropDownLists to loose their selected value. 
				// grdMain.DataBind() does not work.  Force the data bind by using L10nTranslate(). 
				grdMain.L10nTranslate();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Contact is required by the ASP.NET Web Form Designer.
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
			ctlSearch.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
