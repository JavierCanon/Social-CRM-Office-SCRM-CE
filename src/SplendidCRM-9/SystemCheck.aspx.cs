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
using System.Web;
using System.Diagnostics;
using System.Reflection;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SystemCheck.
	/// </summary>
	public class SystemCheck : System.Web.UI.Page
	{
		protected string sBuildNumber;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			sBuildNumber = asm.GetName().Version.ToString();
			
			// 01/20/2006 Paul.  Expire immediately. 
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			try
			{
				// 11/20/2005 Paul.  ASP.NET 2.0 has a namespace conflict, so we need the full name for the SplendidCRM factory. 
				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
				}
			}
			catch(Exception ex)
			{
				//SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// 06/21/2007 Paul.  Display inner exception if exists. 
				if ( ex.InnerException != null )
					Response.Write(ex.InnerException.Message + "<br>");
				Response.Write(ex.Message + "<br>");
			}
			
			try
			{
				// 08/17/2006 Paul.  A customer reported a problem with a view missing columns.  
				// Provide a way to recompile the views. 
				if ( Request.QueryString["Recompile"] == "1" || Request.QueryString["Reload"] == "1" || Sql.IsEmptyString(Application["imageURL"]) )
				{
					// 12/20/2005 Paul.  Require admin rights to reload. 
					if ( SplendidCRM.Security.IS_ADMIN )
					{
						if ( Request.QueryString["Recompile"] == "1" )
						{
							Utils.RefreshAllViews();
						}
						SplendidInit.InitApp();
						// 11/17/2007 Paul.  New function to determine if user is authenticated. 
						if ( Security.IsAuthenticated() )
							SplendidInit.LoadUserPreferences(Security.USER_ID, Sql.ToString(Session["USER_SETTINGS/THEME"]), Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
					}
					else
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), "You must be an administrator to reload the application.");
						Response.Write("You must be an administrator to reload the application." + "<br>");
					}
				}
			}
			catch(Exception ex)
			{
				//SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message + "<br>");
			}
			Page.DataBind();
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
