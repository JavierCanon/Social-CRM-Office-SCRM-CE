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
using System.Diagnostics;
using System.Xml;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Import
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected Label         lblError       ;
		protected HtmlInputFile fileIMPORT     ;
		protected CheckBox      chkTruncate    ;
		protected Literal       lblImportErrors;
		protected RequiredFieldValidator reqFILENAME;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				if ( Page.IsValid )
				{
					try
					{
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								if ( sFILE_MIME_TYPE == "text/xml" )
								{
									using ( MemoryStream mstm = new MemoryStream() )
									{
										using ( BinaryWriter mwtr = new BinaryWriter(mstm) )
										{
											using ( BinaryReader reader = new BinaryReader(pstIMPORT.InputStream) )
											{
												byte[] binBYTES = reader.ReadBytes(8*1024);
												while ( binBYTES.Length > 0 )
												{
													for(int i=0; i < binBYTES.Length; i++ )
													{
														// MySQL dump seems to dump binary 0 & 1 for byte values. 
														if ( binBYTES[i] == 0 )
															mstm.WriteByte(Convert.ToByte('0'));
														else if ( binBYTES[i] == 1 )
															mstm.WriteByte(Convert.ToByte('1'));
														else
															mstm.WriteByte(binBYTES[i]);
													}
													binBYTES = reader.ReadBytes(8*1024);
												}
											}
											mwtr.Flush();
											mstm.Seek(0, SeekOrigin.Begin);
											XmlDocument xml = new XmlDocument();
											xml.Load(mstm);
											try
											{
												// 09/30/2006 Paul.  Clear any previous error. 
												lblImportErrors.Text = "";
												SplendidImport.Import(xml, null, chkTruncate.Checked);
											}
											catch(Exception ex)
											{
												lblImportErrors.Text = ex.Message;
											}
										}
									}
								}
								else
								{
									throw(new Exception("XML is the only supported format at this time."));
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
						return;
					}
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

			// 07/02/2006 Paul.  The required fields need to be bound manually. 
			reqFILENAME.DataBind();
			// 12/17/2005 Paul.  Don't buffer so that the connection can be kept alive. 
			Response.BufferOutput = false;
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
		}
		#endregion
	}
}
