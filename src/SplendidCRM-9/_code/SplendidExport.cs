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
using System.Text;
using System.Xml;
using System.Web;
using System.Collections;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SplendidExport.
	/// </summary>
	public class SplendidExport
	{
		public static void Export(DataView vw, string sModuleName, string sExportFormat, string sExportRange, int nCurrentPage, int nPageSize, string[] arrID)
		{
			int nStartRecord = 0;
			int nEndRecord   = vw.Count;
			switch ( sExportRange )
			{
				case "Page":
					nStartRecord = nCurrentPage * nPageSize;
					nEndRecord   = Math.Min(nStartRecord + nPageSize, vw.Count);
					break;
				case "Selected":
				{
					// 10/17/2006 Paul.  There must be one selected record to continue. 
					if ( arrID == null )
					{
						L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
						throw(new Exception(L10n.Term(".LBL_LISTVIEW_NO_SELECTED")));
					}
					StringBuilder sbIDs = new StringBuilder();
					int nCount = 0;
					foreach(string item in arrID)
					{
						if ( nCount > 0 )
							sbIDs.Append(" or ");
						sbIDs.Append("ID = \'" + item.Replace("\'", "\'\'") + "\'" + ControlChars.CrLf);
						nCount++;
					}
					//vw.RowFilter = "ID in (" + sbIDs.ToString() + ")";
					// 11/03/2006 Paul.  A filter might already exist, so make sure to maintain the existing filter. 
					if ( vw.RowFilter.Length > 0 )
						vw.RowFilter = " and (" + sbIDs.ToString() + ")";
					else
						vw.RowFilter = sbIDs.ToString();
					nEndRecord = vw.Count;
					break;
				}
			}
			
			HttpResponse Response = HttpContext.Current.Response;
			StringBuilder sb = new StringBuilder();
			switch ( sExportFormat )
			{
				case "csv"  :
					Response.ContentType = "text/csv";
					Response.AddHeader("Content-Disposition", "attachment;filename=" + sModuleName + ".csv");
					ExportDelimited(Response.OutputStream, vw, sModuleName.ToLower(), nStartRecord, nEndRecord, ',' );
					Response.End();
					break;
				case "tab"  :
					Response.ContentType = "text/txt";
					Response.AddHeader("Content-Disposition", "attachment;filename=" + sModuleName + ".txt");
					ExportDelimited(Response.OutputStream, vw, sModuleName.ToLower(), nStartRecord, nEndRecord, '\t');
					Response.End();
					break;
				case "xml"  :
					Response.ContentType = "text/xml";
					Response.AddHeader("Content-Disposition", "attachment;filename=" + sModuleName + ".xml");
					ExportXml(Response.OutputStream, vw, sModuleName.ToLower(), nStartRecord, nEndRecord);
					Response.End();
					break;
				//case "Excel":
				default     :
					Response.ContentType = "application/vnd.ms-excel";
					Response.AddHeader("Content-Disposition", "attachment;filename=" + sModuleName + ".xlb");
					ExportExcel(Response.OutputStream, vw, sModuleName.ToLower(), nStartRecord, nEndRecord);
					Response.End();
					break;
			}
			//vw.RowFilter = null;
		}

		private static void ExportExcel(Stream stm, DataView vw, string sModuleName, int nStartRecord, int nEndRecord)
		{
			XmlTextWriter xw = new XmlTextWriter(stm, Encoding.UTF8);
			xw.Formatting  = Formatting.Indented;
			xw.IndentChar  = ControlChars.Tab;
			xw.Indentation = 1;
			xw.WriteStartDocument();
			xw.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");

			xw.WriteStartElement("Workbook");
				xw.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
				xw.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
				xw.WriteAttributeString("xmlns:x", "urn:schemas-microsoft-com:office:excel");
				xw.WriteAttributeString("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");
				xw.WriteAttributeString("xmlns:html", "http://www.w3.org/TR/REC-html40");

			xw.WriteStartElement("DocumentProperties");
				xw.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:office");
				xw.WriteStartElement("Author");
					xw.WriteString(Security.FULL_NAME);
				xw.WriteEndElement();
				xw.WriteStartElement("Created");
					xw.WriteString(DateTime.Now.ToUniversalTime().ToString("s"));
				xw.WriteEndElement();
				xw.WriteStartElement("Version");
					xw.WriteString("11.6568");
				xw.WriteEndElement();
			xw.WriteEndElement();
			xw.WriteStartElement("ExcelWorkbook");
				xw.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");
				xw.WriteStartElement("WindowHeight");
					xw.WriteString("15465");
				xw.WriteEndElement();
				xw.WriteStartElement("WindowWidth");
					xw.WriteString("23820");
				xw.WriteEndElement();
				xw.WriteStartElement("WindowTopX");
					xw.WriteString("120");
				xw.WriteEndElement();
				xw.WriteStartElement("WindowTopY");
					xw.WriteString("75");
				xw.WriteEndElement();
				xw.WriteStartElement("ProtectStructure");
					xw.WriteString("False");
				xw.WriteEndElement();
				xw.WriteStartElement("ProtectWindows");
					xw.WriteString("False");
				xw.WriteEndElement();
			xw.WriteEndElement();

			xw.WriteStartElement("Styles");
				xw.WriteStartElement("Style");
					xw.WriteAttributeString("ss:ID", "Default");
					xw.WriteAttributeString("ss:Name", "Normal");
					xw.WriteStartElement("Alignment");
						xw.WriteAttributeString("ss:Vertical", "Bottom");
					xw.WriteEndElement();
					xw.WriteStartElement("Borders");
					xw.WriteEndElement();
					xw.WriteStartElement("Font");
					xw.WriteEndElement();
					xw.WriteStartElement("Interior");
					xw.WriteEndElement();
					xw.WriteStartElement("NumberFormat");
					xw.WriteEndElement();
					xw.WriteStartElement("Protection");
					xw.WriteEndElement();
				xw.WriteEndElement();
				xw.WriteStartElement("Style");
					xw.WriteAttributeString("ss:ID", "s21");
					xw.WriteStartElement("NumberFormat");
						xw.WriteAttributeString("ss:Format", "General Date");
					xw.WriteEndElement();
				xw.WriteEndElement();
			xw.WriteEndElement();

			DataTable tbl = vw.Table;
			xw.WriteStartElement("Worksheet");
				xw.WriteAttributeString("ss:Name", sModuleName);
			xw.WriteStartElement("Table");
				xw.WriteAttributeString("ss:ExpandedColumnCount", tbl.Columns.Count.ToString());
				xw.WriteAttributeString("ss:FullColumns"        , tbl.Columns.Count.ToString());
				// 11/03/2006 Paul.  Add one row for the header. 
				xw.WriteAttributeString("ss:ExpandedRowCount"   , (nEndRecord - nStartRecord + 1).ToString());

			xw.WriteStartElement("Row");
			for ( int nColumn = 0; nColumn < tbl.Columns.Count; nColumn++ )
			{
				DataColumn col = tbl.Columns[nColumn];
				xw.WriteStartElement("Cell");
				xw.WriteStartElement("Data");
				xw.WriteAttributeString("ss:Type", "String");
				xw.WriteString(col.ColumnName.ToLower());
				xw.WriteEndElement();
				xw.WriteEndElement();
			}
			xw.WriteEndElement();
			for ( int i = nStartRecord; i < nEndRecord; i++ )
			{
				xw.WriteStartElement("Row");
				DataRowView row = vw[i];
				for ( int nColumn = 0; nColumn < tbl.Columns.Count; nColumn++ )
				{
					DataColumn col = tbl.Columns[nColumn];
					xw.WriteStartElement("Cell");
					// 11/03/2006 Paul.  The style must be set in order for a date to be displayed properly. 
					if ( col.DataType.FullName == "System.DateTime" && row[nColumn] != DBNull.Value )
						xw.WriteAttributeString("ss:StyleID", "s21");
					xw.WriteStartElement("Data");
					if ( row[nColumn] != DBNull.Value )
					{
						switch ( col.DataType.FullName )
						{
							case "System.Boolean" :
								xw.WriteAttributeString("ss:Type", "String");
								xw.WriteString(Sql.ToBoolean (row[nColumn]) ? "1" : "0");
								break;
							case "System.Single"  :
								xw.WriteAttributeString("ss:Type", "Number");
								xw.WriteString(Sql.ToDouble  (row[nColumn]).ToString() );
								break;
							case "System.Double"  :
								xw.WriteAttributeString("ss:Type", "Number");
								xw.WriteString(Sql.ToDouble  (row[nColumn]).ToString() );
								break;
							case "System.Int16"   :
								xw.WriteAttributeString("ss:Type", "Number");
								xw.WriteString(Sql.ToInteger (row[nColumn]).ToString() );
								break;
							case "System.Int32"   :
								xw.WriteAttributeString("ss:Type", "Number");
								xw.WriteString(Sql.ToInteger (row[nColumn]).ToString() );
								break;
							case "System.Int64"   :
								xw.WriteAttributeString("ss:Type", "Number");
								xw.WriteString(Sql.ToLong    (row[nColumn]).ToString() );
								break;
							case "System.Decimal" :
								xw.WriteAttributeString("ss:Type", "Number");
								xw.WriteString(Sql.ToDecimal (row[nColumn]).ToString() );
								break;
							case "System.DateTime":
								xw.WriteAttributeString("ss:Type", "DateTime");
								xw.WriteString(Sql.ToDateTime(row[nColumn]).ToUniversalTime().ToString("s"));
								break;
							case "System.Guid"    :
								xw.WriteAttributeString("ss:Type", "String");
								xw.WriteString(Sql.ToGuid    (row[nColumn]).ToString().ToUpper());
								break;
							case "System.String"  :
								xw.WriteAttributeString("ss:Type", "String");
								xw.WriteString(Sql.ToString  (row[nColumn]));
								break;
							case "System.Byte[]"  :
							{
								xw.WriteAttributeString("ss:Type", "String");
								byte[] buffer = Sql.ToByteArray((System.Array) row[nColumn]);
								xw.WriteBase64(buffer, 0, buffer.Length);
								break;
							}
							default:
								//	throw(new Exception("Unsupported field type: " + rdr.GetFieldType(nColumn).FullName));
								// 11/03/2006 Paul.  We need to write the type even for empty cells. 
								xw.WriteAttributeString("ss:Type", "String");
								break;
						}
					}
					else
					{
						// 11/03/2006 Paul.  We need to write the type even for empty cells. 
						xw.WriteAttributeString("ss:Type", "String");
					}
					xw.WriteEndElement();
					xw.WriteEndElement();
				}
				xw.WriteEndElement();
			}
			xw.WriteEndElement();  // Table
			xw.WriteStartElement("WorksheetOptions");
				xw.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");
				xw.WriteStartElement("Selected");
				xw.WriteEndElement();
				xw.WriteStartElement("ProtectObjects");
					xw.WriteString("False");
				xw.WriteEndElement();
				xw.WriteStartElement("ProtectScenarios");
					xw.WriteString("False");
				xw.WriteEndElement();
			xw.WriteEndElement();  // WorksheetOptions
			xw.WriteEndElement();  // Worksheet
			xw.WriteEndElement();  // Workbook
			xw.WriteEndDocument();
			xw.Flush();
		}

		private static void ExportXml(Stream stm, DataView vw, string sModuleName, int nStartRecord, int nEndRecord)
		{
			XmlTextWriter xw = new XmlTextWriter(stm, Encoding.UTF8);
			xw.Formatting  = Formatting.Indented;
			xw.IndentChar  = ControlChars.Tab;
			xw.Indentation = 1;
			xw.WriteStartDocument();
			xw.WriteStartElement("splendidcrm");

			DataTable tbl = vw.Table;
			for ( int i = nStartRecord; i < nEndRecord; i++ )
			{
				xw.WriteStartElement(sModuleName);
				DataRowView row = vw[i];
				for ( int nColumn = 0; nColumn < tbl.Columns.Count; nColumn++ )
				{
					DataColumn col = tbl.Columns[nColumn];
					xw.WriteStartElement(col.ColumnName.ToLower());
					if ( row[nColumn] != DBNull.Value )
					{
						switch ( col.DataType.FullName )
						{
							case "System.Boolean" :  xw.WriteString(Sql.ToBoolean (row[nColumn]) ? "1" : "0");  break;
							case "System.Single"  :  xw.WriteString(Sql.ToDouble  (row[nColumn]).ToString() );  break;
							case "System.Double"  :  xw.WriteString(Sql.ToDouble  (row[nColumn]).ToString() );  break;
							case "System.Int16"   :  xw.WriteString(Sql.ToInteger (row[nColumn]).ToString() );  break;
							case "System.Int32"   :  xw.WriteString(Sql.ToInteger (row[nColumn]).ToString() );  break;
							case "System.Int64"   :  xw.WriteString(Sql.ToLong    (row[nColumn]).ToString() );  break;
							case "System.Decimal" :  xw.WriteString(Sql.ToDecimal (row[nColumn]).ToString() );  break;
							case "System.DateTime":  xw.WriteString(Sql.ToDateTime(row[nColumn]).ToUniversalTime().ToString(CalendarControl.SqlDateTimeFormat));  break;
							case "System.Guid"    :  xw.WriteString(Sql.ToGuid    (row[nColumn]).ToString().ToUpper());  break;
							case "System.String"  :  xw.WriteString(Sql.ToString  (row[nColumn]));  break;
							case "System.Byte[]"  :
							{
								byte[] buffer = Sql.ToByteArray((System.Array) row[nColumn]);
								xw.WriteBase64(buffer, 0, buffer.Length);
								break;
							}
							//default:
							//	throw(new Exception("Unsupported field type: " + rdr.GetFieldType(nColumn).FullName));
						}
					}
					xw.WriteEndElement();
				}
				xw.WriteEndElement();
			}
			xw.WriteEndElement();
			xw.WriteEndDocument();
			xw.Flush();
		}

		private static void ExportDelimited(Stream stm, DataView vw, string sModuleName, int nStartRecord, int nEndRecord, char chDelimiter)
		{
			StreamWriter wt = new StreamWriter(stm);
			DataTable tbl = vw.Table;
			for ( int nColumn = 0; nColumn < tbl.Columns.Count; nColumn++ )
			{
				if ( nColumn > 0 )
					wt.Write(chDelimiter);
				DataColumn col = tbl.Columns[nColumn];
				wt.Write(col.ColumnName.ToLower());
			}
			wt.WriteLine("");

			for ( int i = nStartRecord; i < nEndRecord; i++ )
			{
				DataRowView row = vw[i];
				for ( int nColumn = 0; nColumn < tbl.Columns.Count; nColumn++ )
				{
					if ( nColumn > 0 )
						wt.Write(chDelimiter);
					DataColumn col = tbl.Columns[nColumn];
					if ( row[nColumn] != DBNull.Value )
					{
						string sValue = String.Empty;
						switch ( col.DataType.FullName )
						{
							case "System.Boolean" :  sValue = Sql.ToBoolean (row[nColumn]) ? "1" : "0";  break;
							case "System.Single"  :  sValue = Sql.ToDouble  (row[nColumn]).ToString() ;  break;
							case "System.Double"  :  sValue = Sql.ToDouble  (row[nColumn]).ToString() ;  break;
							case "System.Int16"   :  sValue = Sql.ToInteger (row[nColumn]).ToString() ;  break;
							case "System.Int32"   :  sValue = Sql.ToInteger (row[nColumn]).ToString() ;  break;
							case "System.Int64"   :  sValue = Sql.ToLong    (row[nColumn]).ToString() ;  break;
							case "System.Decimal" :  sValue = Sql.ToDecimal (row[nColumn]).ToString() ;  break;
							case "System.DateTime":  sValue = Sql.ToDateTime(row[nColumn]).ToUniversalTime().ToString(CalendarControl.SqlDateTimeFormat);  break;
							case "System.Guid"    :  sValue = Sql.ToGuid    (row[nColumn]).ToString().ToUpper();  break;
							case "System.String"  :  sValue = Sql.ToString  (row[nColumn]);  break;
							case "System.Byte[]"  :
							{
								byte[] buffer = Sql.ToByteArray((System.Array) row[0]);
								sValue = Convert.ToBase64String(buffer, 0, buffer.Length);
								break;
							}
							//default:
							//	throw(new Exception("Unsupported field type: " + rdr.GetFieldType(nColumn).FullName));
						}
						if( sValue.IndexOf(chDelimiter) >= 0 || sValue.IndexOf('\"') >= 0 )
							sValue = "\"" + sValue.Replace("\"", "\"\"") + "\"";
						wt.Write(sValue);
					}
				}
				wt.WriteLine("");
			}
			wt.Flush();
		}
	}
}
