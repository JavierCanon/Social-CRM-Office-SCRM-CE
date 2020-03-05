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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Backups
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected Label        lblError       ;
		protected TextBox      NAME           ;
		protected RequiredFieldValidator NAME_REQUIRED;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				NAME.Text = NAME.Text.Trim();
				// 12/31/2007 Paul.  The NAME is not required.  If not provided, it will be generated. 
				//NAME_REQUIRED.Enabled = true;
				//NAME_REQUIRED.Validate();
				if ( Page.IsValid )
				{
					try
					{
						// 01/28/2008 Paul.  Cannot perform a backup or restore operation within a transaction. BACKUP DATABASE is terminating abnormally.
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sFILENAME = String.Empty;
							string sTYPE     = "FULL";
							//SqlProcs.spSqlBackupDatabase(ref sNAME, "FULL");
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.CommandText = "spSqlBackupDatabase";
								IDbDataParameter parFILENAME = Sql.AddParameter(cmd, "@FILENAME", sFILENAME  , 255);
								IDbDataParameter parTYPE     = Sql.AddParameter(cmd, "@TYPE"    , sTYPE      ,  20);
								parFILENAME.Direction = ParameterDirection.InputOutput;
								cmd.ExecuteNonQuery();
								sFILENAME = Sql.ToString(parFILENAME.Value);
							}
							lblError.Text = L10n.Term("Administration.LBL_DONE") + " " + sFILENAME;
							SplendidError.SystemMessage(Application, "Information", new StackTrace(true).GetFrame(0), "Database backup complete " + sFILENAME);
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
						return;
					}
				}
			}
			else if ( e.CommandName == "Back" )
			{
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Administration.LBL_MODULE_NAME"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			NAME_REQUIRED.DataBind();
			if ( !IsPostBack )
			{
				NAME.Text = "SplendidCRM_db_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".bak";
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
