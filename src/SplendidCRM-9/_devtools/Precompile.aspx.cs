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
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for Precompile.
	/// </summary>
	public class Precompile : System.Web.UI.Page
	{
		protected DataTable dtMain    ;
		protected Label     lblRoot   ;
		protected Label     lblCurrent;
		protected Label     lblStatus ;
		protected Label     lblErrors ;
		protected ListBox   lstFiles  ;

		bool GetHttp(string strPrecompileURL, out string strResult)
		{
			strResult = "" ;
			bool bGetHttp = false;
			HttpWebRequest  objRequest ;
			HttpWebResponse objResponse;
			try
			{
				objRequest = (HttpWebRequest) WebRequest.Create(strPrecompileURL + "?PrecompileOnly=1");
				objRequest.Headers.Add("accept-encoding", "gzip, deflate");
				objRequest.Headers.Add("cache-control", "no-cache");
				objRequest.KeepAlive = false;
				objRequest.AllowAutoRedirect = true;
				objRequest.Timeout = 120000;  //120 seconds
				//objRequest.Accept            = "*/*";
				//objRequest.ContentType       = "application/x-www-form-urlencoded";
				objRequest.Method = "GET";
				//objRequest.ContentLength     = 0;

				objResponse = (HttpWebResponse) objRequest.GetResponse();
				if ( objResponse != null )
				{
					if ( objResponse.StatusCode != HttpStatusCode.OK && objResponse.StatusCode != HttpStatusCode.Redirect )
						strResult = objResponse.StatusCode + " " + objResponse.StatusDescription;
					StreamReader readStream = new StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.UTF8);
					strResult += readStream.ReadToEnd();
					readStream.Close();
					if ( objResponse.StatusCode == HttpStatusCode.OK )
						bGetHttp = true;
				}
			}
			catch(WebException ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				strResult = ex.Message;
				//bContinue = false;
			}
			return bGetHttp;
		}

		void PrecompileDirectoryTree(string strDirectory, string strRootURL)
		{
			FileInfo objInfo ;

			string[] arrFiles = Directory.GetFiles(strDirectory);
			for ( int i = 0 ; i < arrFiles.Length ; i++ )
			{
				objInfo = new FileInfo(arrFiles[i]);
				if ( (String.Compare(objInfo.Name, "Precompile.aspx", true) != 0 ) && (String.Compare(objInfo.Extension, ".aspx", true) == 0 ) && Response.IsClientConnected )
				{
					DataRow row = dtMain.NewRow();
					row["NAME"] = strRootURL + objInfo.Name;
					dtMain.Rows.Add(row);
				}
			}

			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for ( int i = 0 ; i < arrDirectories.Length ; i++ )
			{
				objInfo = new FileInfo(arrDirectories[i]);
				// 08/29/2005 Paul.  Nothing in the _code folder should be PreCompiled. 
				// 01/18/2008 Paul.  _devtools should not be precompiled. 
				if ( (String.Compare(objInfo.Name, "_devtools", true) != 0) && (String.Compare(objInfo.Name, "_code", true) != 0) && (String.Compare(objInfo.Name, "_vti_cnf", true) != 0) && (String.Compare(objInfo.Name, "_sgbak", true) != 0) )
					PrecompileDirectoryTree(objInfo.FullName, strRootURL + objInfo.Name + "/");
			}
		}

		void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;

			dtMain = new DataTable();
			dtMain.Columns.Add("NAME", typeof(System.String));

			string sApplicationPath = Request.ApplicationPath;
			if ( !sApplicationPath.EndsWith("/") )
				sApplicationPath += "/";
			string[] arrFolders = Request.QueryString.GetValues("folder");
			if ( arrFolders == null || arrFolders.Length == 0 )
				PrecompileDirectoryTree(Server.MapPath(".."), "");
			else
			{
				for ( int i = 0 ; i < arrFolders.Length ; i++ )
				{
					PrecompileDirectoryTree(Server.MapPath("../" + arrFolders[i]), arrFolders[i] + "/");
				}
			}
			lblRoot.Text = Request.Url.Scheme + "://" + Request.Url.Host + sApplicationPath;
			lstFiles.DataSource = dtMain;
			lstFiles.DataBind();
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
