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
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Export
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected SearchControl ctlSearch      ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Clear" )
				{
					ctlSearch.ClearForm();
					Server.Transfer("default.aspx");
				}
				else if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
				else if ( e.CommandName == "Export" )
				{
					string[] arrTABLES = Request.Form.GetValues("chkMain");
					if ( arrTABLES != null )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							
							int nErrors = 0 ;
							MemoryStream stm = new MemoryStream();
							XmlTextWriter xw = new XmlTextWriter(stm, Encoding.UTF8);
							xw.Formatting  = Formatting.Indented;
							xw.IndentChar  = ControlChars.Tab;
							xw.Indentation = 1;
							xw.WriteStartDocument();
							xw.WriteStartElement("splendidcrm");
							foreach ( string sTABLE_NAME in arrTABLES )
							{
								vwMain.RowFilter = "TABLE_NAME = '" + sTABLE_NAME + "'";
								if ( vwMain.Count > 0 )
								{
									try
									{
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = "select * from " + sTABLE_NAME;
											using ( IDataReader rdr = cmd.ExecuteReader() )
											{
												int nRecordCount = 0;
												while ( rdr.Read() )
												{
													nRecordCount++;
													xw.WriteStartElement(sTABLE_NAME.ToLower());
													for ( int nColumn = 0; nColumn < rdr.FieldCount; nColumn++ )
													{
														xw.WriteStartElement(rdr.GetName(nColumn).ToLower());
														if ( !rdr.IsDBNull(nColumn) )
														{
															switch ( rdr.GetFieldType(nColumn).FullName )
															{
																case "System.Boolean" :  xw.WriteString(rdr.GetBoolean (nColumn) ? "1" : "0");  break;
																case "System.Single"  :  xw.WriteString(rdr.GetDouble  (nColumn).ToString() );  break;
																case "System.Double"  :  xw.WriteString(rdr.GetDouble  (nColumn).ToString() );  break;
																case "System.Int16"   :  xw.WriteString(rdr.GetInt16   (nColumn).ToString() );  break;
																case "System.Int32"   :  xw.WriteString(rdr.GetInt32   (nColumn).ToString() );  break;
																case "System.Int64"   :  xw.WriteString(rdr.GetInt64   (nColumn).ToString() );  break;
																case "System.Decimal" :  xw.WriteString(rdr.GetDecimal (nColumn).ToString() );  break;
																case "System.DateTime":  xw.WriteString(rdr.GetDateTime(nColumn).ToUniversalTime().ToString(CalendarControl.SqlDateTimeFormat));  break;
																case "System.Guid"    :  xw.WriteString(rdr.GetGuid    (nColumn).ToString().ToUpper());  break;
																case "System.String"  :  xw.WriteString(rdr.GetString  (nColumn));  break;
																case "System.Byte[]"  :
																{
																	Byte[] buffer = rdr.GetValue(nColumn) as Byte[];
																	xw.WriteBase64(buffer, 0, buffer.Length);
																	break;
																}
																default:
																	throw(new Exception("Unsupported field type: " + rdr.GetFieldType(nColumn).FullName));
															}
														}
														xw.WriteEndElement();
													}
													xw.WriteEndElement();
												}
												vwMain[0]["TABLE_STATUS"] = String.Format(L10n.Term("Export.LBL_RECORDS"), nRecordCount);
											}
										}
									}
									catch(Exception ex)
									{
										nErrors++;
										vwMain[0]["TABLE_STATUS"] = ex.Message;
									}
								}
							}
							xw.WriteEndElement();
							xw.WriteEndDocument();
							xw.Flush();
							if ( nErrors == 0 )
							{
								Response.ContentType = "text/xml";
								Response.AddHeader("Content-Disposition", "attachment;filename=Export.xml");
								stm.WriteTo(Response.OutputStream);
								Response.End();
							}
							vwMain.RowFilter = null;
							grdMain.DataBind();
						}
					}
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("~/Administration/default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = Server.HtmlEncode(ex.Message);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.BufferOutput = true;
			SetPageTitle(L10n.Term(".LBL_EXPORT_DATABASE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select TABLE_NAME         " + ControlChars.CrLf
					     + "     , ' ' as TABLE_STATUS" + ControlChars.CrLf
					     + "  from vwSqlTables        " + ControlChars.CrLf
					     + " where 1 = 1              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						ctlSearch.SqlSearchClause(cmd);

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									// 12/14/2007 Paul.  Only set the default sort if it is not already set.  It may have been set by SearchView. 
									if ( String.IsNullOrEmpty(grdMain.SortColumn) )
									{
										grdMain.SortColumn = "TABLE_NAME";
										grdMain.SortOrder  = "asc" ;
									}
									grdMain.ApplySort();
									grdMain.DataBind();
								}
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
			ctlSearch.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
