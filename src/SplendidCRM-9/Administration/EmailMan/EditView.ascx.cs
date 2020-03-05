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
using System.Globalization;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.EmailMan
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.EditButtons  ctlEditButtons ;

		protected TextBox     EMAILS_PER_RUN       ;
		protected RadioButton SITE_LOCATION_DEFAULT;
		protected RadioButton SITE_LOCATION_CUSTOM ;
		protected TextBox     SITE_LOCATION        ;

		protected RequiredFieldValidator reqEMAILS_PER_RUN;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				reqEMAILS_PER_RUN.Enabled = true;
				reqEMAILS_PER_RUN.Validate();
				if ( Page.IsValid )
				{
					try
					{
						int nEMAILS_PER_RUN = Sql.ToInteger(EMAILS_PER_RUN.Text);
						Application["CONFIG.massemailer_campaign_emails_per_run"        ] = (nEMAILS_PER_RUN > 0)        ? nEMAILS_PER_RUN.ToString() : String.Empty;
						Application["CONFIG.massemailer_tracking_entities_location_type"] = SITE_LOCATION_CUSTOM.Checked ? "2"                        : String.Empty;
						Application["CONFIG.massemailer_tracking_entities_location"     ] = SITE_LOCATION_CUSTOM.Checked ? SITE_LOCATION.Text         : String.Empty;
						SqlProcs.spCONFIG_Update("mail", "massemailer_campaign_emails_per_run"        , Sql.ToString(Application["CONFIG.massemailer_campaign_emails_per_run"        ]));
						SqlProcs.spCONFIG_Update("mail", "massemailer_tracking_entities_location_type", Sql.ToString(Application["CONFIG.massemailer_tracking_entities_location_type"]));
						SqlProcs.spCONFIG_Update("mail", "massemailer_tracking_entities_location"     , Sql.ToString(Application["CONFIG.massemailer_tracking_entities_location"     ]));
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
			SetPageTitle(L10n.Term("EmailMan.LBL_CAMPAIGN_EMAIL_SETTINGS"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				if ( !IsPostBack )
				{
					EMAILS_PER_RUN.Text = Sql.ToString(Application["CONFIG.massemailer_campaign_emails_per_run"]);
					if ( Sql.ToString(Application["CONFIG.massemailer_tracking_entities_location_type"]) == "2" )
					{
						SITE_LOCATION_DEFAULT.Checked = false;
						SITE_LOCATION_CUSTOM .Checked = true ;
						SITE_LOCATION.Text = Sql.ToString(Application["CONFIG.massemailer_tracking_entities_location"]);
					}
					else
					{
						SITE_LOCATION_DEFAULT.Checked = true ;
						SITE_LOCATION_CUSTOM .Checked = false;
					}
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
			// 05/20/2007 Paul.  The m_sMODULE field must be set in order to allow default export handling. 
			m_sMODULE = "EmailMan";
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
