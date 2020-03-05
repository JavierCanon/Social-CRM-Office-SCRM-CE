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
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.ConfigureSettings
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected TextBox         txtNOTIFY_FROMNAME           ;
		protected TextBox         txtNOTIFY_FROMADDRESS        ;
		protected CheckBox        chkNOTIFY_ON                 ;
		protected CheckBox        chkNOTIFY_SEND_BY_DEFAULT    ;
		protected DropDownList    lstMAIL_SENDTYPE             ;
		protected TextBox         txtMAIL_SMTPSERVER           ;
		protected TextBox         txtMAIL_SMTPPORT             ;
		protected CheckBox        chkMAIL_SMTPAUTH_REQ         ;
		protected TextBox         txtMAIL_SMTPUSER             ;
		protected TextBox         txtMAIL_SMTPPASS             ;
		protected CheckBox        chkPORTAL_ON                 ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					try
					{
						SqlProcs.spCONFIG_Update("notify", "fromname"       , txtNOTIFY_FROMNAME       .Text);
						SqlProcs.spCONFIG_Update("notify", "fromaddress"    , txtNOTIFY_FROMADDRESS    .Text);
						SqlProcs.spCONFIG_Update("notify", "on"             , chkNOTIFY_ON             .Checked ? "1" : "0");
						SqlProcs.spCONFIG_Update("notify", "send_by_default", chkNOTIFY_SEND_BY_DEFAULT.Checked ? "1" : "0");
						SqlProcs.spCONFIG_Update("mail"  , "sendtype"       , lstMAIL_SENDTYPE         .SelectedValue      );
						SqlProcs.spCONFIG_Update("mail"  , "smtpserver"     , txtMAIL_SMTPSERVER       .Text);
						SqlProcs.spCONFIG_Update("mail"  , "smtpport"       , txtMAIL_SMTPPORT         .Text);
						SqlProcs.spCONFIG_Update("mail"  , "smtpauth_req"   , chkMAIL_SMTPAUTH_REQ     .Checked ? "1" : "0");
						SqlProcs.spCONFIG_Update("mail"  , "smtpuser"       , txtMAIL_SMTPUSER         .Text);
						SqlProcs.spCONFIG_Update("mail"  , "smtppass"       , txtMAIL_SMTPPASS         .Text);
						SqlProcs.spCONFIG_Update("portal", "on"             , chkPORTAL_ON             .Checked ? "1" : "0");
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlEditButtons.ErrorText = ex.Message;
						return;
					}
					Response.Redirect("../default.aspx");
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList.Administration"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				if ( !IsPostBack )
				{
					lstMAIL_SENDTYPE.DataSource = SplendidCache.List("notifymail_sendtype");
					lstMAIL_SENDTYPE.DataBind();

					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select *        " + ControlChars.CrLf
						     + "  from vwCONFIG " + ControlChars.CrLf
						     + " where CATEGORY_NAME in ( 'notify_fromname'       " + ControlChars.CrLf
						     + "                        , 'notify_fromaddress'    " + ControlChars.CrLf
						     + "                        , 'notify_on'             " + ControlChars.CrLf
						     + "                        , 'notify_send_by_default'" + ControlChars.CrLf
						     + "                        , 'mail_smtpserver'       " + ControlChars.CrLf
						     + "                        , 'mail_smtpport'         " + ControlChars.CrLf
						     + "                        , 'mail_smtpauth_req'     " + ControlChars.CrLf
						     + "                        , 'mail_smtpuser'         " + ControlChars.CrLf
						     + "                        , 'mail_smtppass'         " + ControlChars.CrLf
						     + "                        )" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							con.Open();

							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									string sCATEGORY_NAME = Sql.ToString(rdr["CATEGORY_NAME"]);
									switch ( sCATEGORY_NAME.ToUpper() )
									{
										case "NOTIFY_FROMNAME"       :  txtNOTIFY_FROMNAME       .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "NOTIFY_FROMADDRESS"    :  txtNOTIFY_FROMADDRESS    .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "NOTIFY_ON"             :  chkNOTIFY_ON             .Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "NOTIFY_SEND_BY_DEFAULT":  chkNOTIFY_SEND_BY_DEFAULT.Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "MAIL_SMTPSERVER"       :  txtMAIL_SMTPSERVER       .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "MAIL_SMTPPORT"         :  txtMAIL_SMTPPORT         .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "MAIL_SMTPAUTH_REQ"     :  chkMAIL_SMTPAUTH_REQ     .Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "MAIL_SMTPUSER"         :  txtMAIL_SMTPUSER         .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "MAIL_SMTPPASS"         :  txtMAIL_SMTPPASS         .Text    = Sql.ToString (rdr["VALUE"]);  break;
										case "PORTAL_ON"             :  chkPORTAL_ON             .Checked = Sql.ToBoolean(rdr["VALUE"]);  break;
										case "MAIL_SENDTYPE":
											try
											{
												lstMAIL_SENDTYPE.SelectedValue = Sql.ToString (rdr["VALUE"]);
											}
											catch(Exception ex)
											{
												SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
											}
											break;
									}
								}
							}
						}
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList.Administration") + " - " + ctlModuleHeader.Title);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlEditButtons.ErrorText = ex.Message;
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
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
