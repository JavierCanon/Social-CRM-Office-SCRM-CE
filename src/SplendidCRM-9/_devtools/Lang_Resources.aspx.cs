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
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for Lang_Resources.
	/// </summary>
	public class Lang_Resources : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			Response.Buffer = false;
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			Response.Write("<html><body><pre>\r\n");

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string strSpecifiedLang = Request["Lang"];
				con.Open();
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							for ( int i = 0 ; i < dt.Rows.Count && Response.IsClientConnected ; i++ )
							{
								string sLANG = Sql.ToString(dt.Rows[i]["Lang"]);
								Response.Write(sLANG + "\r\n");
								/* the following language packs have errors. 
								cn-ZH
								dk
								ge-CH
								ge-GE
								se
								sp-CO
								sp-VE
								tw-ZH
								*/
								if ( strSpecifiedLang == null || sLANG == strSpecifiedLang )
								{
									string sSQL ;
									sSQL = "select *                     " + ControlChars.CrLf
									     + "  from TERMINOLOGY           " + ControlChars.CrLf
									     + " where LANG = '" + sLANG + "'" + ControlChars.CrLf
									     + "   and LIST_NAME is null     " + ControlChars.CrLf
									     + " order by NAME               " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									using (DataTable dtLang = new DataTable() )
									{
										da.Fill(dtLang);
										XmlDocument docClean = new XmlDocument() ;
									
										docClean.Load(Server.MapPath(".") + "\\Resources\\Resource-clean.resx");
										for ( int j = 0 ; j < dtLang.Rows.Count && Response.IsClientConnected ; j++ )
										{
											//Response.Write(dtLang.Rows[j]["LANG"].ToString() + " " + dtLang.Rows[j]["NAME"].ToString() + "\r\n");
											XmlElement elmData  = docClean.CreateElement("data");
											XmlAttribute attName = (XmlAttribute) docClean.CreateNode(XmlNodeType.Attribute, "name", "");
											attName.Value= dtLang.Rows[j]["NAME"].ToString();
											elmData.Attributes.Append(attName);
											XmlElement elmValue   = docClean.CreateElement("value");
											XmlElement elmComment = docClean.CreateElement("comment");
											XmlAttribute attPreserve = (XmlAttribute) docClean.CreateNode(XmlNodeType.Attribute, "space", "xml");
											attPreserve.Value= "preserve";
											elmValue.Attributes.Append(attPreserve);
											elmComment.Attributes.Append((XmlAttribute) attPreserve.Clone());
											docClean.DocumentElement.AppendChild(elmData);
											elmData.AppendChild(elmValue );
											elmData.AppendChild(elmComment);
											elmValue  .InnerText = dtLang.Rows[j]["DISPLAY_NAME"].ToString();
											elmComment.InnerText = dtLang.Rows[j]["MODULE_NAME" ].ToString();
										}
										if ( dt.Rows[i]["Lang"].ToString() == "en-US" )
											docClean.Save(Server.MapPath(".") + "\\Resources\\Resource.resx");
										else
											docClean.Save(Server.MapPath(".") + "\\Resources\\Resource." + dt.Rows[i]["Lang"].ToString() + ".resx");
									}
								}
							}
						}
					}
				}
			}
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
