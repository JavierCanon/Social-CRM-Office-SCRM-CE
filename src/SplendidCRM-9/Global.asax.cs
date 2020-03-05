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
using System.Web;
using System.Threading;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SplendidCRM 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Timer tSchedulerManager = null;

		public void InitSchedulerManager()
		{
			if ( tSchedulerManager == null )
			{
				tSchedulerManager = new Timer(SchedulerUtils.OnTimer, this, new TimeSpan(0, 1, 0), new TimeSpan(0, 5, 0));
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "The Scheduler Manager timer has been activated.");
			}
			else
			{
				tSchedulerManager.Change(new TimeSpan(0, 1, 0), new TimeSpan(0, 5, 0));
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "The Scheduler Manager timer has been updated.");
			}
		}

		public Global()
		{
			InitializeComponent();
		}

		protected void Application_OnError(Object sender, EventArgs e)
		{
			//SplendidInit.Application_OnError();
		}
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			SplendidInit.InitApp();
			InitSchedulerManager();
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
			SplendidInit.InitSession();
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			// 12/29/2005 Paul.  vCalendar support is not going to be easy.
			// Outlook will automatically use FrontPage extensions to place the file. 
			// When connecting to a Apache server, it will make HTTP GET/PUT requests. 
			/*
			string sPath = HttpContext.Current.Request.Path.ToLower();
			Regex regex = new Regex("/vcal_server/(\\w+)", RegexOptions.IgnoreCase);
			MatchCollection matches = regex.Matches(sPath);
			//if ( sPath.IndexOf("/vcal_server/") >= 0 )
			if ( matches.Count > 0 )
			{
				//sPath = sPath.Replace("/vcal_server/", "/vcal_server.aspx?");
				sPath = "~/vcal_server.aspx?" + matches[0].Groups[1].ToString();
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), sPath);
				HttpContext.Current.RewritePath(sPath);
			}
			*/
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AcquireRequestState(Object sender, EventArgs e)
		{
			// 03/04/2007 Paul.  The Session will be NULL during web service calls. 
			// We noticed this problem when AJAX failed in ScriptResource.axd. 
			if ( HttpContext.Current.Session != null )
			{
				// 02/28/2007 Paul.  Although Application_AuthenticateRequest might seem like the best place for this code,
				// we have to wait until the Session variables have been initialized to determine if the user has been authenticated. 
				if ( !Sql.IsEmptyString(HttpContext.Current.Session["USER_NAME"]) )
				{
					// 02/28/2007 Paul.  WebParts requires a valid User identity.  
					// We must store the USER_NAME as this will be the lookup key when updating preferences. 
					if ( !HttpContext.Current.User.Identity.IsAuthenticated )
						HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(Security.USER_NAME, "Forms"), null);
				}
			}
		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{
			// 10/29/2006 Paul.  Delete temp files. 
			foreach ( string sKey in Session.Keys )
			{
				if ( sKey.StartsWith("TempFile.") )
				{
					string sTempFileName = Sql.ToString(Session[sKey]);
					string sTempPathName = Path.Combine(Path.GetTempPath(), sTempFileName);
					if ( File.Exists(sTempPathName) )
					{
						try
						{
							File.Delete(sTempPathName);
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Could not delete temp file: " + sTempPathName + ControlChars.CrLf + ex.Message);
						}
					}
				}
			}
		}

		protected void Application_End(Object sender, EventArgs e)
		{
			if ( tSchedulerManager != null )
				tSchedulerManager.Dispose();
		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

