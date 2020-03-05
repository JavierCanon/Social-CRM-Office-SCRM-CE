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
using System.Xml;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools.DataDictionary
{
	/// <summary>
	/// Summary description for GridViews.
	/// </summary>
	public class GridViews : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/24/2006 Paul.  Only a developer/administrator should see this. 
			// 01/27/2006 Paul.  Just require admin. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							string sSQL;
							string sNAME = Sql.ToString(Request.QueryString["NAME"]) ;
							if ( Sql.IsEmptyString(sNAME) )
							{
								sSQL = "select *          " + ControlChars.CrLf
								     + "  from vwGRIDVIEWS" + ControlChars.CrLf
								     + " order by NAME    " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									Response.Write("<html><body><h1>GridViews</h1>");
									while ( rdr.Read() )
									{
										Response.Write("<a href=\"GridViews.aspx?NAME=" + rdr.GetString(rdr.GetOrdinal("NAME")) + "\">" + rdr.GetString(rdr.GetOrdinal("NAME")) + "</a><br>" + ControlChars.CrLf);
									}
									Response.Write("</body></html>");
								}
							}
							else
							{
								Response.ContentType = "text/xml";
								Response.AddHeader("Content-Disposition", "attachment;filename=" + sNAME + ".Mapping.xml");

								XmlDocument xml = new XmlDocument();
								xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
								xml.AppendChild(xml.CreateElement("SplendidTest.Dictionary"));
								XmlAttribute aName = xml.CreateAttribute("Name");
								aName.Value = sNAME + ".Mapping.xml";
								xml.DocumentElement.Attributes.Append(aName);

								sSQL = "select DATA_FIELD               " + ControlChars.CrLf
								     + "     , URL_FIELD                " + ControlChars.CrLf
								     + "  from vwGRIDVIEWS_COLUMNS      " + ControlChars.CrLf
								     + " where GRID_NAME    = @GRID_NAME" + ControlChars.CrLf
								     + "   and DEFAULT_VIEW = 0         " + ControlChars.CrLf
								     + "   and DATA_FIELD is not null   " + ControlChars.CrLf
								     + " order by COLUMN_INDEX          " + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@GRID_NAME", sNAME);
								
								using ( SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader() )
								{
									while ( rdr.Read() )
									{
										string sDATA_FIELD = rdr.GetString(rdr.GetOrdinal("DATA_FIELD"));
										Utils.AppendDictionaryEntry(xml, sDATA_FIELD, "ctlListView_" + sDATA_FIELD);

										if ( !rdr.IsDBNull(rdr.GetOrdinal("URL_FIELD")) )
										{
											string sURL_FIELD = rdr.GetString(rdr.GetOrdinal("URL_FIELD"));
											if ( sDATA_FIELD != sURL_FIELD )
												Utils.AppendDictionaryEntry(xml, sURL_FIELD, "ctlListView_" + sURL_FIELD);
										}
									}
								}
								StringBuilder sb = new StringBuilder();
								if ( xml != null && xml.DocumentElement != null)
									XmlUtil.Dump(ref sb, "", xml.DocumentElement);
								Response.Write(sb.ToString());
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message + ControlChars.CrLf);
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
