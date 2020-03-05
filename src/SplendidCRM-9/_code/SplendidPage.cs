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
using System.Threading;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SplendidCRM._controls;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidPage.
	/// </summary>
	public class SplendidPage : System.Web.UI.Page
	{
		protected bool     bDebug = false;
		// 08/29/2005 Paul.  Only store the absolute minimum amount of data.  
		// This means remove the data that is accessable from the Security object. 
		// The security data is not accessed frequently enough to justify initialization in every user control. 
		// High frequency objects are L10N and TimeZone. 
		protected string   m_sCULTURE    ;
		protected string   m_sDATEFORMAT ;
		protected string   m_sTIMEFORMAT ;
		protected Guid     m_gTIMEZONE   ;
		protected bool     m_bPrintView  = false;
		protected bool     m_bIsAdminPage = false;

		// L10n is an abbreviation for Localization (between the L & n are 10 characters. 
		protected L10N     L10n          ;  // 08/28/2005 Paul.  Keep old L10n name, and rename the object to simplify updated approach. 
		protected TimeZone T10n          ;
		protected Currency C10n          ;

		public SplendidPage()
		{
			this.PreInit += new EventHandler(Page_PreInit);
		}

		public bool PrintView
		{
			get
			{
				return m_bPrintView;
			}
			set
			{
				m_bPrintView = value;
			}
		}

		public bool IsAdminPage
		{
			get
			{
				return m_bIsAdminPage;
			}
			set
			{
				m_bIsAdminPage = value;
			}
		}

		public void SetMenu(string sMODULE)
		{
			// 01/20/2007 Paul.  Move the menu code to a fuction so that will only get called in EditView, DetailView and ListView controls. 
			// 01/19/2007 Paul.  If a MasterPage is in use, then we need to set the ActiveTab. 
			if ( !String.IsNullOrEmpty(sMODULE) )
			{
				if ( Master != null )
				{
					SplendidCRM.Themes.Sugar.TabMenu ctlTabMenu = Master.FindControl("ctlTabMenu") as SplendidCRM.Themes.Sugar.TabMenu;
					if ( ctlTabMenu != null )
					{
						// 01/20/2007 Paul.  Only set the first control as each 
						// SplendidControl on page will pass through this code. 
						if ( String.IsNullOrEmpty(ctlTabMenu.ActiveTab) )
							ctlTabMenu.ActiveTab = sMODULE;
					}
				}
			}
		}

		public void SetPageTitle(string sTitle)
		{
			// 01/20/2007 Paul.  Wrap the page title function to minimized differences between Web1.2.
			Page.Title = sTitle;
		}

		public L10N GetL10n()
		{
			// 08/30/2005 Paul.  Move the L10N creation to this get function so that the first control 
			// that gets created will cause the creation of L10N.  The UserControls get the OnInit event before the Page onInit event. 
			if ( L10n == null )
			{
				m_sCULTURE     = Sql.ToString (Session["USER_SETTINGS/CULTURE"   ]);
				m_sDATEFORMAT  = Sql.ToString (Session["USER_SETTINGS/DATEFORMAT"]);
				m_sTIMEFORMAT  = Sql.ToString (Session["USER_SETTINGS/TIMEFORMAT"]);

				// 05/09/2006 Paul.  Initialize the numeric separators. 
				string sGROUP_SEPARATOR   = Sql.ToString(Session["USER_SETTINGS/GROUP_SEPARATOR"  ]);
				string sDECIMAL_SEPARATOR = Sql.ToString(Session["USER_SETTINGS/DECIMAL_SEPARATOR"]);

				L10n = new L10N(m_sCULTURE);
				// 08/05/2006 Paul.  We cannot set the CurrencyDecimalSeparator directly on Mono as it is read-only.  
				// Hold off setting the CurrentCulture until we have updated all the settings. 
				CultureInfo culture = CultureInfo.CreateSpecificCulture(L10n.NAME);
				culture.DateTimeFormat.ShortDatePattern = m_sDATEFORMAT;
				culture.DateTimeFormat.ShortTimePattern = m_sTIMEFORMAT;
				// 06/03/2006 Paul.  Setting the separators is causing some users a problem.  It may be because the strings were empty. 
				if ( !Sql.IsEmptyString(sGROUP_SEPARATOR  ) ) culture.NumberFormat.CurrencyGroupSeparator   = sGROUP_SEPARATOR  ;
				if ( !Sql.IsEmptyString(sDECIMAL_SEPARATOR) ) culture.NumberFormat.CurrencyDecimalSeparator = sDECIMAL_SEPARATOR;
				if ( !Sql.IsEmptyString(sGROUP_SEPARATOR  ) ) culture.NumberFormat.NumberGroupSeparator     = sGROUP_SEPARATOR  ;
				if ( !Sql.IsEmptyString(sDECIMAL_SEPARATOR) ) culture.NumberFormat.NumberDecimalSeparator   = sDECIMAL_SEPARATOR;

				// 08/30/2005 Paul. We don't need the long time pattern because we simply do not use it. 
				//Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern  = m_sTIMEFORMAT;
				//Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
				//08/05/2006 Paul.  Apply the modified cultures. 
				Thread.CurrentThread.CurrentCulture   = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
			}
			return L10n;
		}

		public TimeZone GetT10n()
		{
			// 08/30/2005 Paul.  Move the TimeZone creation to this get function so that the first control 
			// that gets created will cause the creation of TimeZone.  The UserControls get the OnInit event before the Page onInit event. 
			if ( T10n == null )
			{
				m_gTIMEZONE = Sql.ToGuid(Session["USER_SETTINGS/TIMEZONE"]);
				T10n = TimeZone.CreateTimeZone(m_gTIMEZONE);
				if ( T10n.ID != m_gTIMEZONE )
				{
					// 08/30/2005 Paul. If we are using a default, then update the session so that future controls will be quicker. 
					m_gTIMEZONE = T10n.ID ;
					Session["USER_SETTINGS/TIMEZONE"] = m_gTIMEZONE.ToString() ;
				}
			}
			return T10n;
		}

		public Currency GetC10n()
		{
			if ( C10n == null )
			{
				Guid gCURRENCY_ID = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
				C10n = Currency.CreateCurrency(gCURRENCY_ID);
				if ( C10n.ID != gCURRENCY_ID )
				{
					// 05/09/2006 Paul. If we are using a default, then update the session so that future controls will be quicker. 
					gCURRENCY_ID = C10n.ID;
					Session["USER_SETTINGS/CURRENCY"] = gCURRENCY_ID.ToString();
					
				}
				// 03/30/2007 Paul.  Always set the currency symbol.  It is not retained between page requests. 
				// 07/28/2006 Paul.  We cannot set the CurrencySymbol directly on Mono as it is read-only.  
				// Just clone the culture and modify the clone. 
				CultureInfo culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
				culture.NumberFormat.CurrencySymbol   = C10n.SYMBOL;
				Thread.CurrentThread.CurrentCulture   = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
			}
			return C10n;
		}

		// 11/19/2005 Paul.  Default to expiring everything. 
		virtual protected bool AuthenticationRequired()
		{
			return true;
		}

		public bool IsMobile
		{
			get
			{
				return (this.Theme == "Mobile");
			}
		}

		protected void AppendDetailViewRelationships(string sDETAIL_NAME, PlaceHolder plc)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			DataTable dtFields = SplendidCache.DetailViewRelationships(sDETAIL_NAME);
			foreach(DataRow row in dtFields.Rows)
			{
				string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
				string sCONTROL_NAME = Sql.ToString(row["CONTROL_NAME"]);
				// 04/27/2006 Paul.  Only add the control if the user has access. 
				if ( Security.GetUserAccess(sMODULE_NAME, "list") >= 0 )
				{
					Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
					plc.Controls.Add(ctl);
				}
			}
		}

		protected void AppendGridColumns(SplendidGrid grd, string sGRID_NAME)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sGRID_NAME = sGRID_NAME + (this.IsMobile ? ".Mobile" : "");
			grd.AppendGridColumns(sGRID_NAME);
		}

		protected override void OnInit(EventArgs e)
		{
			if ( Request["PrecompileOnly"] == "1" )
				Response.End();
			if ( Sql.IsEmptyString(Application["imageURL"]) )
			{
				SplendidInit.InitSession();
			}
			if ( AuthenticationRequired() )
			{
				// 11/17/2007 Paul.  New function to determine if user is authenticated. 
				if ( !Security.IsAuthenticated() )
					Response.Redirect("~/Users/Login.aspx");
			}
			// 11/27/2006 Paul.  We want to show the SQL on the Demo sites, so add a config variable to allow it. 
			bDebug = Sql.ToBoolean(Application["CONFIG.show_sql"]);
#if DEBUG
			bDebug = true;
#endif
			
			// 08/30/2005 Paul.  Apply the new culture at the page level so that it is only applied once. 
			GetL10n();
			GetT10n();
			GetC10n();
			// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
			// This is so that we don't need to require that the page inherits from SplendidPage. 
			// A port to DNN prompted this approach. 
			Context.Items["L10n"] = GetL10n();
			Context.Items["T10n"] = GetT10n();
			Context.Items["C10n"] = GetC10n();
			base.OnInit(e);
		}
		
		protected void Page_PreInit(object sender, EventArgs e)
		{
			//if ( Request["PrintView"] == "true" )
			//	this.MasterPageFile = "~/PrintView.master";
			string sTheme = Sql.ToString(Session["USER_SETTINGS/THEME"]);
			if ( String.IsNullOrEmpty(sTheme) )
				sTheme = "Sugar";
			this.Theme = sTheme;
			if ( !String.IsNullOrEmpty(this.MasterPageFile) )
			{
				if ( !this.MasterPageFile.Contains("/App_MasterPages/") )
				{
					string sFileName = System.IO.Path.GetFileName(this.MasterPageFile);
					this.MasterPageFile = "~/App_MasterPages/" + sTheme + "/" + sFileName;
				}
			}
		}
	}
}
