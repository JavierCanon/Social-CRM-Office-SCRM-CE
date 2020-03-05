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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Terminology.Import
{
	/// <summary>
	///		Summary description for LanguagePacks.
	/// </summary>
	public class LanguagePacks : SplendidControl
	{
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected Label           lblError       ;

		public CommandEventHandler Command ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e) ;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				DataTable dt = Cache.Get("PublicSugarCRMLanguagePacks.xml") as DataTable;
				if ( dt == null )
				{
					XmlDocument xml = new XmlDocument();
					if ( !IsPostBack )
					{
						try
						{
#if DEBUG
							xml.Load(Server.MapPath("PublicSugarCRMLanguagePacks.xml"));
#else
							xml.Load("http://demo.splendidcrm.com/Administration/Terminology/Import/PublicSugarCRMLanguagePacks.xml");
#endif
						}
						catch
						{
							xml.Load(Server.MapPath("PublicSugarCRMLanguagePacks.xml"));
						}
					}
					dt = XmlUtil.CreateDataTable(xml.DocumentElement, "LanguagePack", new string[] {"Name", "Date", "Description", "URL"});
					Cache.Insert("PublicSugarCRMLanguagePacks.xml", dt, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
				}

				vwMain = new DataView(dt);
				vwMain.RowFilter = "URL > ''";
				vwMain.Sort      = "Name";
				grdMain.DataSource = vwMain ;
				if ( !IsPostBack )
				{
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
			m_sMODULE = "Terminology";
		}
		#endregion
	}
}
