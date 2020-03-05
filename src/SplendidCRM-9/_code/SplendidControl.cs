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

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidControl.
	/// </summary>
	public class SplendidControl : System.Web.UI.UserControl
	{
		protected bool     bDebug = false;
		protected L10N     L10n;
		protected TimeZone T10n;
		protected Currency C10n;
		protected string   m_sMODULE;  // 04/27/2006 Paul.  Leave null so that we can get an error when not initialized. 

		public bool IsMobile
		{
			get
			{
				return (Page.Theme == "Mobile");
			}
		}

		public bool PrintView
		{
			get
			{
				SplendidPage oPage = Page as SplendidPage;
				if ( oPage != null )
					return oPage.PrintView;
				return false;
			}
			set
			{
				SplendidPage oPage = Page as SplendidPage;
				if ( oPage != null )
					oPage.PrintView = value;
			}
		}

		protected void SetMenu(string sMODULE)
		{
			// 01/20/2007 Paul.  Move code to SplendidPage. 
			SplendidPage oPage = Page as SplendidPage;
			if ( oPage != null )
				oPage.SetMenu(sMODULE);
		}

		public void SetPageTitle(string sTitle)
		{
			// 01/20/2007 Paul.  Wrap the page title function to minimized differences between Web1.2.
			Page.Title = sTitle;
		}

		protected void AppendDetailViewFields(string sDETAIL_NAME, HtmlTable tbl, IDataReader rdr)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendDetailViewFields(sDETAIL_NAME, tbl, rdr, GetL10n(), GetT10n(), null);
		}

		protected void AppendDetailViewFields(string sDETAIL_NAME, HtmlTable tbl, IDataReader rdr, CommandEventHandler Page_Command)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sDETAIL_NAME = sDETAIL_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendDetailViewFields(sDETAIL_NAME, tbl, rdr, GetL10n(), GetT10n(), Page_Command);
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
					try
					{
						Control ctl = LoadControl(sCONTROL_NAME + ".ascx");
						plc.Controls.Add(ctl);
					}
					catch(Exception ex)
					{
						Label lblError = new Label();
						// 06/09/2006 Paul.  Catch the error and display a message instead of crashing. 
						lblError.ID              = "lblDetailViewRelationshipsError";
						lblError.Text            = Utils.ExpandException(ex);
						lblError.ForeColor       = System.Drawing.Color.Red;
						lblError.EnableViewState = false;
						plc.Controls.Add(lblError);
					}
				}
			}
		}

		protected void AppendEditViewFields(string sEDIT_NAME, HtmlTable tbl, IDataReader rdr)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			// 12/02/2005 Paul.  AppendEditViewFields will get called inside InitializeComponent(), 
			// which occurs before OnInit(), so make sure to initialize L10n. 
			SplendidDynamic.AppendEditViewFields(sEDIT_NAME, tbl, rdr, GetL10n(), GetT10n());
		}

		protected void ValidateEditViewFields(string sEDIT_NAME)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sEDIT_NAME = sEDIT_NAME + (this.IsMobile ? ".Mobile" : "");
			SplendidDynamic.ValidateEditViewFields(sEDIT_NAME, this);
		}

		protected void AppendGridColumns(SplendidGrid grd, string sGRID_NAME)
		{
			// 11/17/2007 Paul.  Convert all view requests to a mobile request if appropriate.
			sGRID_NAME = sGRID_NAME + (this.IsMobile ? ".Mobile" : "");
			grd.AppendGridColumns(sGRID_NAME);
		}

		public TimeZone GetT10n()
		{
			// 08/30/2005 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( T10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				T10n = Context.Items["T10n"] as TimeZone;
				if ( T10n == null )
				{
					Guid   gTIMEZONE = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
					T10n = TimeZone.CreateTimeZone(gTIMEZONE);
				}
			}
			return T10n;
		}

		public L10N GetL10n()
		{
			// 08/30/2005 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( L10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				L10n = Context.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					string sCULTURE  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
					L10n = new L10N(sCULTURE);
				}
			}
			return L10n;
		}

		public Currency GetC10n()
		{
			// 05/09/2006 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( C10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				C10n = Context.Items["C10n"] as Currency;
				if ( C10n == null )
				{
					Guid gCURRENCY_ID = Sql.ToGuid(Session["USER_SETTINGS/CURRENCY"]);
					C10n = Currency.CreateCurrency(gCURRENCY_ID);
				}
			}
			return C10n;
		}

		protected void SetC10n(Guid gCURRENCY_ID)
		{
			C10n = Currency.CreateCurrency(gCURRENCY_ID);
			// 07/28/2006 Paul.  We cannot set the CurrencySymbol directly on Mono as it is read-only.  
			// Just clone the culture and modify the clone. 
			CultureInfo culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
			culture.NumberFormat.CurrencySymbol   = C10n.SYMBOL;
			Thread.CurrentThread.CurrentCulture   = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		protected void SetC10n(Guid gCURRENCY_ID, float fCONVERSION_RATE)
		{
			C10n = Currency.CreateCurrency(gCURRENCY_ID, fCONVERSION_RATE);
			// 07/28/2006 Paul.  We cannot set the CurrencySymbol directly on Mono as it is read-only.  
			// Just clone the culture and modify the clone. 
			CultureInfo culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
			culture.NumberFormat.CurrencySymbol   = C10n.SYMBOL;
			Thread.CurrentThread.CurrentCulture   = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		protected override void OnInit(EventArgs e)
		{
			// 11/27/2006 Paul.  We want to show the SQL on the Demo sites, so add a config variable to allow it. 
			bDebug = Sql.ToBoolean(Application["CONFIG.show_sql"]);
#if DEBUG
			bDebug = true;
#endif
			GetL10n();
			GetT10n();
			GetC10n();
			base.OnInit(e);
		}

		public void RegisterClientScriptBlock(string key, string script)
		{
			#pragma warning disable 618
			Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), key, script);
			#pragma warning restore 618
		}
	}
}
