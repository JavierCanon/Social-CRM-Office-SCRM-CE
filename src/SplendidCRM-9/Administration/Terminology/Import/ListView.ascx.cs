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
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;
using System.Net;
//using Microsoft.VisualBasic;
using ICSharpCode.SharpZipLib.Zip;
//using ICSharpCode.SharpZipLib.BZip2;
//using ICSharpCode.SharpZipLib.Zip.Compression;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
//using ICSharpCode.SharpZipLib.GZip;


namespace SplendidCRM.Administration.Terminology.Import
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected Label         lblError       ;
		protected HtmlInputFile fileIMPORT     ;
		protected CheckBox      chkTruncate    ;
		protected CheckBox      chkForceUTF8   ;
		protected Literal       lblImportErrors;
		protected LanguagePacks ctlLanguagePacks;

		protected RequiredFieldValidator reqFILENAME    ;

		private bool bContinue = true;

		void ProcessDirectory(string strDirectory)
		{
			FileInfo objInfo ;
			if ( !bContinue )
				return;

			string[] arrFiles = Directory.GetFiles(strDirectory);
			for ( int i = 0 ; i < arrFiles.Length ; i++ )
			{
				objInfo = new FileInfo(arrFiles[i]);
				Response.Write(objInfo.FullName + "<br>" + ControlChars.CrLf);
			}
			
			string[] arrDirectories = Directory.GetDirectories(strDirectory);
			for ( int i = 0 ; i < arrDirectories.Length ; i++ )
			{
				objInfo = new FileInfo(arrDirectories[i]);
				ProcessDirectory(objInfo.FullName);
			}
		}

		protected void ImportFromStream(Stream stm)
		{
			// http://msdn.microsoft.com/msdnmag/issues/03/06/ZipCompression/default.aspx
			// http://community.sharpdevelop.net/forums/738/ShowPost.aspx
			// The #ZipLib is licensed under a modified GPL. This modification grants you the right to use the compiled  .DLL in closed source applications. 
			// Modifcations to the library however fall under the provisions of the GPL.
			Hashtable hashLanguages = new Hashtable();
			using ( ZipInputStream stmZip = new ZipInputStream(stm) )
			{
				ZipEntry theEntry = null;
				while ( (theEntry = stmZip.GetNextEntry()) != null )
				{
					string sFileName = Path.GetFileName(theEntry.Name);
					if ( sFileName != String.Empty )
					{
						Response.Write(theEntry.Name + "<br>" + ControlChars.CrLf);
						if ( theEntry.Name.EndsWith(".lang.php") )
						{
							string sLang = LanguagePackImport.GetLanguage(theEntry.Name);
							// 11/13/2006 Paul.  SugarCRM still has not fixed their German language pack. Convert ge-GE to de-DE.
							if ( String.Compare(sLang, "ge-GE", true) == 0 )
								sLang = "de-DE";
							// 08/22/2007 Paul.  Only insert the language record once. 
							if ( !hashLanguages.ContainsKey(sLang) )
							{
								CultureInfo culture = new CultureInfo(sLang);
								if ( culture == null )
									throw(new Exception("Unknown language: " + sLang));
								SqlProcs.spLANGUAGES_InsertOnly(sLang, culture.LCID, true, culture.NativeName, culture.DisplayName);
								if ( chkTruncate.Checked )
								{
									SqlProcs.spTERMINOLOGY_DeleteAll(sLang);
									hashLanguages.Add(sLang, String.Empty);
								}
							}
							LanguagePackImport.InsertTerms(theEntry.Name, stmZip, chkForceUTF8.Checked);
						}
					}
				}
				// 01/12/2006 Paul.  Update internal cache. 
				SplendidInit.InitTerminology();
				// 01/13/2006 Paul.  Clear the language cache. 
				SplendidCache.ClearLanguages();
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				reqFILENAME.Enabled = true;
				reqFILENAME.Validate();
				if ( Page.IsValid )
				{
					Response.Write("<div id=\"divImportList\">" + ControlChars.CrLf);
					try
					{
						// 07/03/2007 Paul.  Increase timeout to support slower machines. 
						Server.ScriptTimeout = 600;
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								//string sLocalFile = Path.Combine(Path.GetTempPath(), sFILENAME);
								//pstIMPORT.SaveAs(sLocalFile);
								//ProcessDirectory(sLocalFile + "\\SugarRus\\manifest.php");
								if ( sFILE_MIME_TYPE == "application/x-zip-compressed" )
								{
									ImportFromStream(pstIMPORT.InputStream);
									lblError.Text = "Import Complete";
								}
								else
								{
									throw(new Exception("ZIP is the only supported format at this time."));
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
					finally
					{
						Response.Write("</div>" + ControlChars.CrLf);
						RegisterClientScriptBlock("HideImportList", "<script type=\"text/javascript\">document.getElementById('divImportList').style.display='none';</script>");
					}
				}
			}
			else if ( e.CommandName == "LanguagePack.Import" )
			{
				Response.Write("<div id=\"divImportList\">" + ControlChars.CrLf);
				try
				{
					// 07/03/2007 Paul.  Increase timeout to support slower machines. 
					Server.ScriptTimeout = 600;
					string sURL = e.CommandArgument.ToString();
					if ( sURL.Length > 0 )
					{
						HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create(sURL);
						objRequest.Headers.Add("cache-control", "no-cache");
						objRequest.KeepAlive         = false;
						objRequest.AllowAutoRedirect = true;
						objRequest.Timeout           = 120000;  //120 seconds
						objRequest.Method            = "GET";

						HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse();
						if ( objResponse != null )
						{
							if ( objResponse.StatusCode != HttpStatusCode.OK && objResponse.StatusCode != HttpStatusCode.Found )
							{
								lblError.Text = objResponse.StatusCode + " " + objResponse.StatusDescription;
							}
							else
							{
								string sFILE_MIME_TYPE = objResponse.ContentType;
								if ( sFILE_MIME_TYPE == "application/zip" )
								{
									ImportFromStream(objResponse.GetResponseStream());
									lblError.Text = "Import Complete";
								}
								else
								{
									throw(new Exception("ZIP is the only supported format at this time."));
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					lblError.Text = ex.Message;
				}
				finally
				{
					Response.Write("</div>" + ControlChars.CrLf);
					RegisterClientScriptBlock("HideImportList", "<script type=\"text/javascript\">document.getElementById('divImportList').style.display='none';</script>");
				}
			}
			else if ( e.CommandName == "Back" )
			{
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Administration.LBL_MODULE_NAME"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();
			// 12/17/2005 Paul.  Don't buffer so that the connection can be kept alive. 
			Response.BufferOutput = false;
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
			ctlLanguagePacks.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
