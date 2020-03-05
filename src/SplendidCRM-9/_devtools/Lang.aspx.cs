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
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for Lang.
	/// </summary>
	public class Lang : System.Web.UI.Page
	{
		void PrecompileDirectoryTree(string strDirectory)
		{
			if ( !Directory.Exists(strDirectory) )
				return;
			FileInfo objInfo;
			string[] arrFiles = Directory.GetFiles(strDirectory);
			for (int i = 0; i < arrFiles.Length; i++)
			{
				objInfo = new FileInfo(arrFiles[i]);
				if ( Response.IsClientConnected )
				{
					if ( objInfo.FullName.EndsWith(".lang.php") )
					{
						LanguagePackImport.InsertTerms(objInfo.FullName, false);
						Response.Write(objInfo.FullName + ControlChars.CrLf);
					}
					else if ( objInfo.FullName.EndsWith(".html") && objInfo.FullName.IndexOf(".help.") >= 0 )
					{
						LanguagePackImport.InsertHelp(objInfo.FullName, false);
						Response.Write(objInfo.FullName + ControlChars.CrLf);
					}
				}
			}

			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for (int i = 0; i < arrDirectories.Length; i++)
			{
				objInfo = new FileInfo(arrDirectories[i]);
				if (objInfo.Name != "_vti_cnf")
					PrecompileDirectoryTree(objInfo.FullName);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			Response.Buffer = false;
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			Response.ContentEncoding = System.Text.Encoding.UTF8;
			Response.Charset = "UTF-8";
			Response.Write("<html><body><pre>\r\n");

			PrecompileDirectoryTree(Server.MapPath("../LanguagePacks"));
			Response.Write("</pre></body></html>\r\n");
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
