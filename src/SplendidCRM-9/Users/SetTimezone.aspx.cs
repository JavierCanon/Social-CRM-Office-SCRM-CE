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
using System.Data.Common;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Users
{
	/// <summary>
	/// Summary description for SetTimezone.
	/// </summary>
	public class SetTimezone : SplendidPage
	{
		protected Label        lblError   ;
		protected DropDownList lstTIMEZONE;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					string sUSER_PREFERENCES = Sql.ToString(Session["USER_PREFERENCES"]);
					if ( Sql.IsEmptyString(sUSER_PREFERENCES) )
						sUSER_PREFERENCES = "<xml></xml>";
					
					XmlDocument xml = SplendidInit.InitUserPreferences(sUSER_PREFERENCES);
					XmlUtil.SetSingleNode(xml, "timezone", lstTIMEZONE.SelectedValue);
					Session["USER_SETTINGS/TIMEZONE"] = lstTIMEZONE.SelectedValue;
					Session["USER_SETTINGS/TIMEZONE/ORIGINAL"] = lstTIMEZONE.SelectedValue;
					
					SqlProcs.spUSERS_PreferencesUpdate(Security.USER_ID, xml.OuterXml);
					Session["USER_PREFERENCES"] = xml.OuterXml;
				}
				catch(Exception ex)
				{
					lblError.Text = ex.Message;
					return;
				}
				string sDefaultModule = Sql.ToString(Application["CONFIG.default_module"]);
				if ( sDefaultModule.StartsWith("~") )
					Response.Redirect(sDefaultModule);
				else if ( !Sql.IsEmptyString(sDefaultModule) )
					Response.Redirect("~/" + sDefaultModule + "/");
				else
					Response.Redirect("~/Home/");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				Page.DataBind();
				lstTIMEZONE.DataSource = SplendidCache.TimezonesListbox();
				try
				{
					lstTIMEZONE.SelectedValue = SplendidDefaults.TimeZone().ToLower();
					lstTIMEZONE.DataBind();
				}
				catch //(Exception ex)
				{
					//lblError.Text = ex.Message;
				}
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
		}
		#endregion
	}
}
