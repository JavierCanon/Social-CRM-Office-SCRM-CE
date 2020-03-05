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
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;
using System.Text;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Import
{
	/// <summary>
	///		Summary description for ImportView.
	/// </summary>
	public class ImportView : SplendidControl
	{
		protected SplendidCRM._controls.ModuleHeader  ctlModuleHeader ;
		protected _controls.ImportButtons             ctlImportButtons;
		protected SplendidCRM._controls.ListHeader    ctlListHeader   ;
		protected PlaceHolder                         phDefaultsView  ;
		protected SplendidControl                     ctlDefaultsView ;

		protected Guid                   gID                          ;
		protected TextBox                txtNAME                      ;
		protected RequiredFieldValidator reqNAME                      ;

		protected RadioButton            radEXCEL                     ;
		protected RadioButton            radXML_SPREADSHEET           ;
		protected RadioButton            radXML                       ;
		protected RadioButton            radSALESFORCE                ;
		protected RadioButton            radACT_2005                  ;
		protected RadioButton            radCUSTOM_CSV                ;
		protected RadioButton            radCUSTOM_TAB                ;
		protected RadioButton            radCUSTOM_DELIMITED          ;
		protected TextBox                txtCUSTOM_DELIMITER_VAL      ;
		
		protected DataView               vwMain                       ;
		protected DataView               vwColumns                    ;
		protected SplendidGrid           grdMain                      ;
		protected DataView               vwMySaved                    ;
		protected SplendidGrid           grdMySaved                   ;

		protected XmlDocument            xml                          ;
		protected XmlDocument            xmlMapping                   ;
		protected string                 sImportModule                ;
		protected HtmlInputFile          fileIMPORT                   ;
		protected RequiredFieldValidator reqFILENAME                  ;
		protected CheckBox               chkHasHeader                 ;
		protected HtmlTable              tblImportMappings            ;
		protected StringBuilder          sbImport                     ;

		protected Label                  lblStatus                    ;
		protected Label                  lblSuccessCount              ;
		protected Label                  lblFailedCount               ;
		protected CheckBox               chkUseTransaction            ;

		protected HtmlInputHidden        txtACTIVE_TAB                ;
		protected bool                   bDuplicateFields = false;
		protected int                    nMAX_ERRORS = 200;

		public string Module
		{
			get { return sImportModule; }
			set { sImportModule = value; }
		}

		protected string SourceType()
		{
			string sSourceType = "";
			if      ( radEXCEL           .Checked ) sSourceType = "excel";
			else if ( radXML_SPREADSHEET .Checked ) sSourceType = "xmlspreadsheet";
			else if ( radXML             .Checked ) sSourceType = "xml";
			else if ( radSALESFORCE      .Checked ) sSourceType = "salesforce";
			else if ( radACT_2005        .Checked ) sSourceType = "act";
			else if ( radCUSTOM_CSV      .Checked ) sSourceType = "other";
			else if ( radCUSTOM_TAB      .Checked ) sSourceType = "other_tab";
			else if ( radCUSTOM_DELIMITED.Checked ) sSourceType = "custom_delimited";
			return sSourceType;
		}

		protected void SourceType(string sSOURCE)
		{
			switch ( sSOURCE )
			{
				case "excel"           :  radEXCEL           .Checked = true;  break;
				case "xmlspreadsheet"  :  radXML_SPREADSHEET .Checked = true;  break;
				case "xml"             :  radXML             .Checked = true;  break;
				case "salesforce"      :  radSALESFORCE      .Checked = true;  break;
				case "act"             :  radACT_2005        .Checked = true;  break;
				case "other"           :  radCUSTOM_CSV      .Checked = true;  break;
				case "other_tab"       :  radCUSTOM_TAB      .Checked = true;  break;
				case "custom_delimited":  radCUSTOM_DELIMITED.Checked = true;  break;
			}
		}

		protected void UpdateImportMappings(XmlDocument xml, bool bInitialize, bool bUpdateMappings)
		{
			Hashtable hashFieldMappings = new Hashtable();

			tblImportMappings.Rows.Clear();
			HtmlTableRow rowHeader = new HtmlTableRow();
			tblImportMappings.Rows.Add(rowHeader);
			HtmlTableCell cellField  = new HtmlTableCell();
			HtmlTableCell cellRowHdr = new HtmlTableCell();
			HtmlTableCell cellRow1   = new HtmlTableCell();
			HtmlTableCell cellRow2   = new HtmlTableCell();
			rowHeader.Cells.Add(cellField );
			if ( chkHasHeader.Checked || radXML.Checked )
				rowHeader.Cells.Add(cellRowHdr);
			rowHeader.Cells.Add(cellRow1  );
			rowHeader.Cells.Add(cellRow2  );
			cellField .Attributes.Add("class", "tabDetailViewDL");
			cellRowHdr.Attributes.Add("class", "tabDetailViewDL");
			cellRow1  .Attributes.Add("class", "tabDetailViewDL");
			cellRow2  .Attributes.Add("class", "tabDetailViewDL");
			cellField .Attributes.Add("style", "TEXT-ALIGN: left");
			cellRowHdr.Attributes.Add("style", "TEXT-ALIGN: left");
			cellRow1  .Attributes.Add("style", "TEXT-ALIGN: left");
			cellRow2  .Attributes.Add("style", "TEXT-ALIGN: left");
			Label lblField  = new Label();
			Label lblRowHdr = new Label();
			Label lblRow1   = new Label();
			Label lblRow2   = new Label();
			cellField .Controls.Add(lblField );
			cellRowHdr.Controls.Add(lblRowHdr);
			cellRow1  .Controls.Add(lblRow1  );
			cellRow2  .Controls.Add(lblRow2  );
			lblField .Font.Bold = true;
			lblRowHdr.Font.Bold = true;
			lblRow1  .Font.Bold = true;
			lblRow2  .Font.Bold = true;
			lblField .Text = L10n.Term("Import.LBL_DATABASE_FIELD");
			lblRowHdr.Text = L10n.Term("Import.LBL_HEADER_ROW"    );
			lblRow1  .Text = L10n.Term("Import.LBL_ROW"           ) + " 1";
			lblRow2  .Text = L10n.Term("Import.LBL_ROW"           ) + " 2";
			
			if ( xml.DocumentElement != null )
			{
				XmlNodeList nl = xml.DocumentElement.SelectNodes(sImportModule.ToLower());
				if ( nl.Count > 0 )
				{
					vwColumns.Sort = "DISPLAY_NAME";
					XmlNode nodeH = nl[0];
					XmlNode node1 = nl[0];
					XmlNode node2 = null;
					// 08/22/2006 Paul.  An XML Spreadsheet will have a header record, 
					// so don't assume that an XML file will use the tag names as the header. 
					if ( chkHasHeader.Checked )
					{
						if ( nl.Count > 1 )
							node1 = nl[1];
						if ( nl.Count > 2 )
							node2 = nl[2];
					}
					else
					{
						if ( nl.Count > 1 )
							node2 = nl[1];
					}
					bDuplicateFields = false;
					Hashtable hashSelectedFields = new Hashtable();
					for ( int i = 0 ; i < nodeH.ChildNodes.Count ; i++ )
					{
						rowHeader = new HtmlTableRow();
						tblImportMappings.Rows.Add(rowHeader);
						cellField  = new HtmlTableCell();
						cellRowHdr = new HtmlTableCell();
						cellRow1   = new HtmlTableCell();
						cellRow2   = new HtmlTableCell();
						rowHeader.Cells.Add(cellField );
						if ( chkHasHeader.Checked || radXML.Checked )
							rowHeader.Cells.Add(cellRowHdr);
						rowHeader.Cells.Add(cellRow1  );
						if ( node2 != null && i < node2.ChildNodes.Count )
							rowHeader.Cells.Add(cellRow2);
						cellField .Attributes.Add("class", "tabDetailViewDF");
						cellRowHdr.Attributes.Add("class", "tabDetailViewDF");
						cellRow1  .Attributes.Add("class", "tabDetailViewDF");
						cellRow2  .Attributes.Add("class", "tabDetailViewDF");
						DropDownList lstField  = new DropDownList();
						lblRowHdr = new Label();
						lblRow1   = new Label();
						lblRow2   = new Label();
						cellField .Controls.Add(lstField );
						cellRowHdr.Controls.Add(lblRowHdr);
						cellRow1  .Controls.Add(lblRow1  );
						cellRow2  .Controls.Add(lblRow2  );
						
						// 08/20/2006 Paul.  Clear any previous filters. 
						vwColumns.RowFilter = null;
						// 08/20/2006 Paul.  Don't use real column names as they may collide.
						lstField.ID             = "ImportField" + i.ToString("000");
						lstField.DataValueField = "NAME";
						lstField.DataTextField  = "DISPLAY_NAME";
						lstField.DataSource     = vwColumns;
						lstField.DataBind();
						lstField.Items.Insert(0, new ListItem(L10n.Term("Import.LBL_DONT_MAP"), String.Empty));
						try
						{
							if ( bInitialize )
							{
								if ( chkHasHeader.Checked )
								{
									// 08/22/2006 Paul.  If Has Header is checked, then always expect the body to contain the header names. 
									string sFieldName = nodeH.ChildNodes[i].InnerText.Trim();
									// 08/20/2006 Paul.  Use the DataView to locate matching fields so that we don't have to worry about case significance. 
									vwColumns.RowFilter = "NAME = '" + sFieldName.Replace("'", "''") + "' or DISPLAY_NAME = '" + sFieldName.Replace("'", "''") + "'";
									if ( vwColumns.Count == 1 )
									{
										hashFieldMappings.Add(i, Sql.ToString(vwColumns[0]["NAME"]));
										lstField.SelectedValue = Sql.ToString(vwColumns[0]["NAME"]);
									}
								}
								else if ( radXML.Checked )
								{
									// 08/22/2006 Paul.  If Has Header is not checked for XML, then use the tag ame as the field name. 
									string sFieldName = nodeH.ChildNodes[i].Name;
									// 08/20/2006 Paul.  Use the DataView to locate matching fields so that we don't have to worry about case significance. 
									vwColumns.RowFilter = "NAME = '" + sFieldName.Replace("'", "''") + "' or DISPLAY_NAME = '" + sFieldName.Replace("'", "''") + "'";
									if ( vwColumns.Count == 1 )
									{
										hashFieldMappings.Add(i, Sql.ToString(vwColumns[0]["NAME"]));
										lstField.SelectedValue = Sql.ToString(vwColumns[0]["NAME"]);
									}
								}
								else
									hashFieldMappings.Add(i, "ImportField" + i.ToString("000"));
							}
							else
							{
								// 08/20/2006 Paul.  Manually set the last value. 
								hashFieldMappings.Add(i, Sql.ToString(Request[lstField.UniqueID]));
								lstField.SelectedValue = Sql.ToString(Request[lstField.UniqueID]);
								if ( lstField.SelectedValue.Length > 0 )
								{
									if ( hashSelectedFields.ContainsKey(lstField.SelectedValue) )
									{
										bDuplicateFields = true;
									}
									else
									{
										hashSelectedFields.Add(lstField.SelectedValue, null);
									}
								}
							}
						}
						catch
						{
						}
						// XML data will use the node-name as the header. 
						if ( chkHasHeader.Checked )
						{
							// 08/22/2006 Paul.  If Has Header is checked, then always expect the body to contain the header names. 
							lblRowHdr.Text = nodeH.ChildNodes[i].InnerText;
						}
						else if ( radXML.Checked )
						{
							// 08/22/2006 Paul.  If Has Header is not checked for XML, then use the tag name as the field name. 
							lblRowHdr.Text = nodeH.ChildNodes[i].Name;
						}
						
						if ( node1 != null && i < node1.ChildNodes.Count )
							lblRow1.Text = node1.ChildNodes[i].InnerText;
						if ( node2 != null && i < node2.ChildNodes.Count )
							lblRow2.Text = node2.ChildNodes[i].InnerText;
					}
					if ( bDuplicateFields )
					{
						throw(new Exception(L10n.Term("Import.ERR_MULTIPLE")));
					}
					
					if ( bUpdateMappings )
					{
						XmlNodeList nlFields = xmlMapping.DocumentElement.SelectNodes("Fields/Field");
						foreach ( XmlNode xField in nlFields )
						{
							XmlUtil.SetSingleNode(xmlMapping, xField, "Mapping", String.Empty);
						}
						// 08/22/2006 Paul.  We should always use the header mappings instead of an index as nodes may move around. 
						XmlNode node = nl[0];
						for ( int j = 0; j < node.ChildNodes.Count; j++ )
						{
							XmlNode xField = xmlMapping.DocumentElement.SelectSingleNode("Fields/Field[@Name='" + hashFieldMappings[j] + "']");
							if ( xField != null )
							{
								XmlUtil.SetSingleNode(xmlMapping, xField, "Mapping", node.ChildNodes[j].Name);
							}
						}
					}
					else
					{
						// 10/12/2006 Paul.  If we are not updating the mappings, then we are setting the mappings. 
						XmlNodeList nlFields = xmlMapping.DocumentElement.SelectNodes("Fields/Field");
						foreach ( XmlNode xField in nlFields )
						{
							string sName    = xField.Attributes.GetNamedItem("Name").Value;
							string sMapping = XmlUtil.SelectSingleNode(xField, "Mapping");
							if ( !Sql.IsEmptyString(sMapping) )
							{
								DropDownList lstField = tblImportMappings.FindControl(sMapping) as DropDownList;
								if ( lstField != null )
								{
									try
									{
										lstField.SelectedValue = sName;
									}
									catch
									{
									}
								}
							}
						}
					}
				}
			}
		}

		protected void ValidateMappings()
		{
			switch ( sImportModule )
			{
				case "Accounts":
				{
					string sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						throw ( new Exception(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Accounts.LBL_LIST_ACCOUNT_NAME")) );
					break;
				}
				case "Contacts":
				{
					string sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='LAST_NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						throw ( new Exception(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Contacts.LBL_LIST_LAST_NAME")) );
					break;
				}
				case "Prospects":
				{
					string sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='LAST_NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						throw ( new Exception(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Prospects.LBL_LIST_LAST_NAME")) );
					break;
				}
				case "Opportunities":
				{
					StringBuilder sb = new StringBuilder();
					string sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						sb.Append(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Opportunities.LBL_LIST_NAME") + ControlChars.CrLf);
					// 11/02/2006 Paul.  Allow mapping of ACCOUNT_NAME or ACCOUNT_ID. 
					sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='ACCOUNT_NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
					{
						sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='ACCOUNT_ID']/Mapping");
						if ( Sql.IsEmptyString(sMapping) )
							sb.Append(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Opportunities.LBL_LIST_ACCOUNT_NAME") + ControlChars.CrLf);
					}
					sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='DATE_CLOSED']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						sb.Append(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Opportunities.LBL_LIST_DATE_CLOSED") + ControlChars.CrLf);
					sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='SALES_STAGE']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						sb.Append(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Opportunities.LBL_LIST_SALES_STAGE") + ControlChars.CrLf);
					if ( sb.Length > 0 )
						throw ( new Exception(sb.ToString()) );
					break;
				}
				case "Cases":
				{
					StringBuilder sb = new StringBuilder();
					string sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
						sb.Append(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Cases.LBL_LIST_NAME") + ControlChars.CrLf);
					// 11/02/2006 Paul.  Allow mapping of ACCOUNT_NAME or ACCOUNT_ID. 
					sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='ACCOUNT_NAME']/Mapping");
					if ( Sql.IsEmptyString(sMapping) )
					{
						sMapping = XmlUtil.SelectSingleNode(xmlMapping.DocumentElement, "Fields/Field[@Name='ACCOUNT_ID']/Mapping");
						if ( Sql.IsEmptyString(sMapping) )
							sb.Append(L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("Cases.LBL_LIST_ACCOUNT_NAME") + ControlChars.CrLf);
					}
					if ( sb.Length > 0 )
						throw ( new Exception(sb.ToString()) );
					break;
				}
			}
		}

		protected void GenerateImport(string sTempFileName, bool bPreview)
		{
			try
			{
				XmlDocument xmlImport = new XmlDocument();
				xmlImport.Load(Path.Combine(Path.GetTempPath(), sTempFileName));
				
				XmlNodeList nlRows = xmlImport.DocumentElement.SelectNodes(sImportModule.ToLower());
				if ( nlRows.Count == 0 )
					throw(new Exception(L10n.Term("Import.LBL_NOTHING")));
				
				// 08/20/2006 Paul.  Also map the header names to allow for a flexible XML. 
				StringDictionary hashHeaderMappings  = new StringDictionary();
				StringDictionary hashReverseMappings = new StringDictionary();
				Hashtable hashDefaultMappings = new Hashtable();
				XmlNodeList nlFields = xmlMapping.DocumentElement.SelectNodes("Fields/Field");
				foreach ( XmlNode xField in nlFields )
				{
					string sName    = xField.Attributes.GetNamedItem("Name").Value;
					string sMapping = XmlUtil.SelectSingleNode(xField, "Mapping");
					string sDefault = XmlUtil.SelectSingleNode(xField, "Default");
					if ( !Sql.IsEmptyString(sMapping) )
					{
						hashHeaderMappings.Add(sMapping, sName);
						hashReverseMappings.Add(sName, sMapping);
					}
					if ( !Sql.IsEmptyString(sDefault) )
					{
						hashDefaultMappings.Add(sName, sDefault);
					}
				}
				
				// 11/01/2006 Paul.  Use a hash for quick access to required fields. 
				Hashtable hashColumns = new Hashtable();
				foreach ( DataRowView row in vwColumns )
					hashColumns.Add(row["NAME"], row["DISPLAY_NAME"]);
				
				Hashtable hashRequiredFields = new Hashtable();
				DataTable dtRequiredFields = SplendidCache.EditViewFields(sImportModule + ".EditView");
				DataView dvRequiredFields = new DataView(dtRequiredFields);
				dvRequiredFields.RowFilter = "UI_REQUIRED = 1";
				foreach(DataRowView row in dvRequiredFields)
				{
					string sDATA_FIELD = Sql.ToString (row["DATA_FIELD"]);
					if (!Sql.IsEmptyString(sDATA_FIELD) )
					{
						if ( !hashRequiredFields.ContainsKey(sDATA_FIELD) )
							hashRequiredFields.Add(sDATA_FIELD, null);
					}
				}
				dvRequiredFields = null;
				dtRequiredFields = null;
				
				int nImported = 0;
				int nFailed   = 0;
				//int nSkipped  = 0;
				DataTable dtProcessed = new DataTable();
				dtProcessed.Columns.Add("IMPORT_ROW_STATUS", typeof(bool));
				dtProcessed.Columns.Add("IMPORT_ROW_NUMBER", typeof(Int32));
				dtProcessed.Columns.Add("IMPORT_ROW_ERROR"  );
				dtProcessed.Columns.Add("IMPORT_LAST_COLUMN");
				dtProcessed.Columns.Add("ID");  // 10/10/2006 Paul.  Every record will have an ID, either implied or specified. 
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Import Database Table: " + sImportModule);
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					Hashtable hashTeamNames = new Hashtable();
					if ( Crm.Config.enable_team_management() )
					{
						string sSQL;
						sSQL = "select ID          " + ControlChars.CrLf
						     + "     , NAME        " + ControlChars.CrLf
						     + "  from vwTEAMS_List" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									Guid   gTEAM_ID   = Sql.ToGuid  (rdr["ID"  ]);
									string sTEAM_NAME = Sql.ToString(rdr["NAME"]);
									sTEAM_NAME = sTEAM_NAME.Trim().ToUpper();
									if ( !Sql.IsEmptyString(sTEAM_NAME) )
										hashTeamNames.Add(sTEAM_NAME, gTEAM_ID);
								}
							}
						}
					}

					// 11/01/2006 Paul.  The transaction is optional, just make sure to always dispose it. 
					//using ( IDbTransaction trn = con.BeginTransaction() )
					{
						IDbTransaction trn = null;
						try
						{
							string sTABLE_NAME = Sql.ToString(Application["Modules." + sImportModule + ".TableName"]);
							if ( Sql.IsEmptyString(sTABLE_NAME) )
								sTABLE_NAME = sImportModule.ToUpper();
							
							IDbCommand cmdImport = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
							IDbCommand cmdImportCSTM = null;
							//IDbCommand cmdImportTeam = null;
							// 09/17/2007 Paul.  Only activate the custom field code if there are fields in the custom fields table. 
							vwColumns.RowFilter = "CustomField = 1";
							if ( vwColumns.Count > 0 )
							{
								vwColumns.Sort = "colid";
								cmdImportCSTM = con.CreateCommand();
								cmdImportCSTM.CommandType = CommandType.Text;
								cmdImportCSTM.CommandText = "update " + sTABLE_NAME + "_CSTM" + ControlChars.CrLf;
								int nFieldIndex = 0;
								foreach ( DataRowView row in vwColumns )
								{
									// 01/11/2006 Paul.  Uppercase looks better. 
									string sNAME   = Sql.ToString(row["ColumnName"]).ToUpper();
									string sCsType = Sql.ToString(row["ColumnType"]);
									// 01/13/2007 Paul.  We need to truncate any long strings to prevent SQL error. 
									// String or binary data would be truncated. The statement has been terminated. 
									int    nMAX_SIZE = Sql.ToInteger(row["Size"]);
									if ( nFieldIndex == 0 )
										cmdImportCSTM.CommandText += "   set ";
									else
										cmdImportCSTM.CommandText += "     , ";
									// 01/10/2006 Paul.  We can't use a StringBuilder because the Sql.AddParameter function
									// needs to be able to replace the @ with the appropriate database specific token. 
									cmdImportCSTM.CommandText += sNAME + " = @" + sNAME + ControlChars.CrLf;
									
									IDbDataParameter par = null;
									switch ( sCsType )
									{
										// 09/19/2007 Paul.  In order to leverage the existing AddParameter functions, we need to provide default values. 
										case "Guid"    :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, Guid.Empty             );  break;
										case "short"   :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, 0                      );  break;
										case "Int32"   :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, 0                      );  break;
										case "Int64"   :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, 0                      );  break;
										case "float"   :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, 0.0f                   );  break;
										case "decimal" :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, new Decimal()          );  break;
										case "bool"    :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, false                  );  break;
										case "DateTime":  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, DateTime.MinValue      );  break;
										default        :  par = Sql.AddParameter(cmdImportCSTM, "@" + sNAME, String.Empty, nMAX_SIZE);  break;
									}
									nFieldIndex++;
								}
								// 09/19/2007 Paul.  Exclude ID_C as it is expect and required. We don't want it to appear in the mapping table. 
								cmdImportCSTM.CommandText += " where ID_C = @ID_C" + ControlChars.CrLf;
								Sql.AddParameter(cmdImportCSTM, "@ID_C", Guid.Empty);
								cmdImportCSTM.ExecuteNonQuery();
							}
							vwColumns.RowFilter = "";
							/*
							if ( Crm.Config.enable_team_management() )
							{
								cmdImportTeam = con.CreateCommand();
								cmdImportTeam.CommandType = CommandType.Text;
								cmdImportTeam.CommandText  = "update " + sTABLE_NAME     + ControlChars.CrLf;
								cmdImportTeam.CommandText += "   set TEAM_ID = @TEAM_ID" + ControlChars.CrLf;
								cmdImportTeam.CommandText += " where ID      = @ID     " + ControlChars.CrLf;
								Sql.AddParameter(cmdImportTeam, "@TEAM_ID", Guid.Empty);
								Sql.AddParameter(cmdImportTeam, "@ID"     , Guid.Empty);
							}
							*/
							
							// 11/01/2006 Paul.  The transaction is optional, but on by default. 
							if ( chkUseTransaction.Checked || bPreview )
							{
								trn = con.BeginTransaction();
								cmdImport.Transaction = trn;
								if ( cmdImportCSTM != null )
									cmdImportCSTM.Transaction = trn;
								//if ( cmdImportTeam != null )
								//	cmdImportTeam.Transaction = trn;
							}
							int i = 0;
							if ( chkHasHeader.Checked )
								i++;
							for ( int iRowNumber = 1; i < nlRows.Count ; i++ )
							{
								XmlNode node = nlRows[i];
								int nEmptyColumns = 0;
								for ( int j = 0; j < node.ChildNodes.Count; j++ )
								{
									string sText = node.ChildNodes[j].InnerText;
									if ( sText == String.Empty )
										nEmptyColumns++;
								}
								// 09/04/2006 Paul.  If all columns are empty, then skip the row. 
								if ( nEmptyColumns == node.ChildNodes.Count )
									continue;
								DataRow row = dtProcessed.NewRow();
								row["IMPORT_ROW_NUMBER"] = iRowNumber ;
								iRowNumber++;
								dtProcessed.Rows.Add(row);
								try
								{
									if ( !Response.IsClientConnected )
									{
										break;
									}
									foreach(IDbDataParameter par in cmdImport.Parameters)
									{
										par.Value = DBNull.Value;
									}
									if ( cmdImportCSTM != null )
									{
										foreach(IDbDataParameter par in cmdImportCSTM.Parameters)
										{
											par.Value = DBNull.Value;
										}
									}
									/*
									if ( cmdImportTeam != null )
									{
										foreach(IDbDataParameter par in cmdImportTeam.Parameters)
										{
											par.Value = DBNull.Value;
										}
									}
									*/
									// 09/19/2007 Paul.  parID and parID_C are frequently used, so obtain outside the import loop. 
									IDbDataParameter parID   = Sql.FindParameter(cmdImport, "ID");
									IDbDataParameter parID_C = null;
									if ( cmdImportCSTM != null )
										parID_C = Sql.FindParameter(cmdImportCSTM, "ID_C");

									// 10/31/2006 Paul.  The modified user is always the person who imported the data. 
									// 11/01/2006 Paul.  The real problem with importing a contact is that the SYNC_CONTACT flag was null, and treated as 1. 
									// It still makes sense to set the modified id. 
									Sql.SetParameter(cmdImport, "@MODIFIED_USER_ID", Security.USER_ID);
									foreach(string sName in hashDefaultMappings.Keys)
									{
										string sDefault = Sql.ToString(hashDefaultMappings[sName]);
										if ( !dtProcessed.Columns.Contains(sName) )
										{
											dtProcessed.Columns.Add(sName);
										}
										row["IMPORT_ROW_STATUS" ] = true ;
										row["IMPORT_LAST_COLUMN"] = sName;
										row[sName] = sDefault;
										Sql.SetParameter(cmdImport, sName, sDefault);
										if ( cmdImportCSTM != null )
											Sql.SetParameter(cmdImportCSTM, sName, sDefault);
										//if ( cmdImportTeam != null && sName == "team_id" )
										//	Sql.SetParameter(cmdImportTeam, "@TEAM_ID", sDefault);
									}
									for ( int j = 0; j < node.ChildNodes.Count; j++ )
									{
										string sText = node.ChildNodes[j].InnerText;
										string sName = String.Empty;
										// 08/22/2006 Paul.  We should always use the header mappings instead of an index as nodes may move around. 
										sName = Sql.ToString(hashHeaderMappings[node.ChildNodes[j].Name]);
										// 09/08/2006 Paul.  There is no need to set the field if the value is empty. 
										if ( sName.Length > 0 && sText.Length > 0 )
										{
											sName = sName.ToUpper();
											// 08/20/2006 Paul.  Fix IDs. 
											// 09/30/2006 Paul.  CREATED_BY counts as an ID. 
											if ( sName == "ID" || sName.EndsWith("_ID") || sName == "CREATED_BY" )
											{
												// 09/30/2006 Paul.  IDs must be in upper case.  This is primarily for platforms that are case-significant. 
												// 10/05/2006 Paul.  We need to use upper case for SQL Server as well so that the SugarCRM user names are correctly replaced. 
												sText = sText.ToUpper();
												if ( sText.Length < 36 && sText.Length > 0 )
												{
													sText = "00000000-0000-0000-0000-000000000000".Substring(0, 36 - sText.Length) + sText;
													switch ( sText )
													{
														case "00000000-0000-0000-0000-000000JIM_ID":  sText = "00000000-0000-0000-0001-000000000000";  break;
														case "00000000-0000-0000-0000-000000MAX_ID":  sText = "00000000-0000-0000-0002-000000000000";  break;
														case "00000000-0000-0000-0000-00000WILL_ID":  sText = "00000000-0000-0000-0003-000000000000";  break;
														case "00000000-0000-0000-0000-0000CHRIS_ID":  sText = "00000000-0000-0000-0004-000000000000";  break;
														case "00000000-0000-0000-0000-0000SALLY_ID":  sText = "00000000-0000-0000-0005-000000000000";  break;
														case "00000000-0000-0000-0000-0000SARAH_ID":  sText = "00000000-0000-0000-0006-000000000000";  break;
														// 11/30/2006 Paul.  The following mappings will really only help when importing SugarCRM sample data. 
														case "00000000-0000-0000-0000-000000000001":  sText = "00000000-0000-0001-0000-000000000000";  break;
														case "00000000-0000-0000-0000-0PRIVATE.JIM":  sText = "00000000-0000-0001-0001-000000000000";  break;
														case "00000000-0000-0000-0000-0PRIVATE.MAX":  sText = "00000000-0000-0001-0002-000000000000";  break;
														case "00000000-0000-0000-0000-PRIVATE.WILL":  sText = "00000000-0000-0001-0003-000000000000";  break;
														case "00000000-0000-0000-0000PRIVATE.CHRIS":  sText = "00000000-0000-0001-0004-000000000000";  break;
														case "00000000-0000-0000-0000PRIVATE.SALLY":  sText = "00000000-0000-0001-0005-000000000000";  break;
														case "00000000-0000-0000-0000PRIVATE.SARAH":  sText = "00000000-0000-0001-0006-000000000000";  break;
														case "00000000-0000-0000-0000-00000000EAST":  sText = "00000000-0000-0001-0101-000000000000";  break;
														case "00000000-0000-0000-0000-00000000WEST":  sText = "00000000-0000-0001-0102-000000000000";  break;
														case "00000000-0000-0000-0000-0000000NORTH":  sText = "00000000-0000-0001-0103-000000000000";  break;
														case "00000000-0000-0000-0000-0000000SOUTH":  sText = "00000000-0000-0001-0104-000000000000";  break;
													}
												}
											}
											if ( !dtProcessed.Columns.Contains(sName) )
											{
												dtProcessed.Columns.Add(sName);
											}
											row["IMPORT_ROW_STATUS" ] = true ;
											row["IMPORT_LAST_COLUMN"] = sName;
											row[sName] = sText;
											Sql.SetParameter(cmdImport, sName, sText);
											if ( cmdImportCSTM != null )
												Sql.SetParameter(cmdImportCSTM, sName, sText);
										}
									}
									StringBuilder sbRequiredFieldErrors = new StringBuilder();
									foreach ( string sRequiredField in hashRequiredFields.Keys )
									{
										IDbDataParameter par = Sql.FindParameter(cmdImport, sRequiredField);
										if ( par == null && cmdImportCSTM != null )
											par = Sql.FindParameter(cmdImportCSTM, sRequiredField);
										if ( par != null )
										{
											if ( par.Value == DBNull.Value || par.Value.ToString() == String.Empty )
											{
												// 11/02/2006 Paul.  If ACCOUNT_ID is required, then also allow ACCOUNT_NAME. 
												if ( sRequiredField == "ACCOUNT_ID" && (sImportModule == "Cases " || sImportModule == "Opportunities") )
												{
													par = Sql.FindParameter(cmdImport, "ACCOUNT_NAME");
													if ( par != null )
													{
														if ( par.Value != DBNull.Value && par.Value.ToString() != String.Empty )
														{
															continue;
														}
													}
												}
												if ( sbRequiredFieldErrors.Length > 0 )
													sbRequiredFieldErrors.Append(", ");
												if ( hashColumns.ContainsKey(sRequiredField) )
													sbRequiredFieldErrors.Append(hashColumns[sRequiredField]);
												else
													sbRequiredFieldErrors.Append(sRequiredField);
											}
										}
									}
									if ( sbRequiredFieldErrors.Length > 0 )
									{
										row["IMPORT_ROW_STATUS"] = false;
										row["IMPORT_ROW_ERROR" ] = L10n.Term("Import.ERR_MISSING_REQUIRED_FIELDS") + " " + sbRequiredFieldErrors.ToString();
										nFailed++;
										// 10/31/2006 Paul.  Abort after 200 errors. 
										if ( nFailed >= nMAX_ERRORS )
										{
											ctlImportButtons.ErrorText += L10n.Term("Import.LBL_MAX_ERRORS");
											break;
										}
									}
									else
									{
										sbImport.Append(Sql.ExpandParameters(cmdImport));
										sbImport.Append(";");
										sbImport.Append(ControlChars.CrLf);
										cmdImport.ExecuteNonQuery();
										if ( parID != null )
										{
											row["ID"] = parID.Value;

											Guid gID = Sql.ToGuid(parID.Value);
											// 11/30/2006 Paul.  The TEAM_ID needs to be updated separately as it is not part of the typical update procedure. 
											/*
											if ( cmdImportTeam != null )
											{
												// 09/19/2007 Paul.  Allow team to be specified by ID or by NAME. 
												XmlNode xTEAM_ID   = null;
												XmlNode xTEAM_NAME = null;
												// 08/22/2006 Paul.  We should always use the header mappings instead of an index as nodes may move around. 
												if ( hashReverseMappings.ContainsKey("team_id") )
													xTEAM_ID   = node.SelectSingleNode(hashReverseMappings["team_id"  ]);
												if ( hashReverseMappings.ContainsKey("team_name") )
													xTEAM_NAME = node.SelectSingleNode(hashReverseMappings["team_name"]);
												if ( xTEAM_ID != null || xTEAM_NAME != null )
												{
													string sText = String.Empty;
													string sName = String.Empty;
													if ( xTEAM_ID != null )
													{
														sText = xTEAM_ID.InnerText;
														sName = "TEAM_ID";
													}
													else if ( xTEAM_NAME != null )
													{
														sText = xTEAM_NAME.InnerText;
														sName = "TEAM_NAME";
													}
													// 09/19/2007 Paul.  In the event that a field is updated, we will want to update the TEAM and allow it to be set to NULL.
													//if ( sText.Length > 0 )
													{
														Guid gTEAM_ID = Guid.Empty;
														if ( xTEAM_ID != null )
														{
															// 09/30/2006 Paul.  IDs must be in upper case.  This is primarily for platforms that are case-significant. 
															// 10/05/2006 Paul.  We need to use upper case for SQL Server as well so that the SugarCRM user names are correctly replaced. 
															sText = sText.ToUpper();
															if ( sText.Length < 36 && sText.Length > 0 )
															{
																sText = "00000000-0000-0000-0000-000000000000".Substring(0, 36 - sText.Length) + sText;
																switch ( sText )
																{
																	// 11/30/2006 Paul.  The following mappings will really only help when importing SugarCRM sample data. 
																	case "00000000-0000-0000-0000-000000000001":  sText = "00000000-0000-0001-0000-000000000000";  break;
																	case "00000000-0000-0000-0000-0PRIVATE.JIM":  sText = "00000000-0000-0001-0001-000000000000";  break;
																	case "00000000-0000-0000-0000-0PRIVATE.MAX":  sText = "00000000-0000-0001-0002-000000000000";  break;
																	case "00000000-0000-0000-0000-PRIVATE.WILL":  sText = "00000000-0000-0001-0003-000000000000";  break;
																	case "00000000-0000-0000-0000PRIVATE.CHRIS":  sText = "00000000-0000-0001-0004-000000000000";  break;
																	case "00000000-0000-0000-0000PRIVATE.SALLY":  sText = "00000000-0000-0001-0005-000000000000";  break;
																	case "00000000-0000-0000-0000PRIVATE.SARAH":  sText = "00000000-0000-0001-0006-000000000000";  break;
																	case "00000000-0000-0000-0000-00000000EAST":  sText = "00000000-0000-0001-0101-000000000000";  break;
																	case "00000000-0000-0000-0000-00000000WEST":  sText = "00000000-0000-0001-0102-000000000000";  break;
																	case "00000000-0000-0000-0000-0000000NORTH":  sText = "00000000-0000-0001-0103-000000000000";  break;
																	case "00000000-0000-0000-0000-0000000SOUTH":  sText = "00000000-0000-0001-0104-000000000000";  break;
																	// 11/30/2006 Paul.  We cannot import any other private teams because we do not have the ability
																	// to replace the new Guid value in all of the other records. 
																	default: sText = String.Empty;  break;
																}
															}
															gTEAM_ID = Sql.ToGuid(sText);
														}
														else if ( xTEAM_NAME != null )
														{
															string sTEAM_NAME = sText.Trim().ToUpper();
															if ( hashTeamNames.ContainsKey(sTEAM_NAME) )
															{
																gTEAM_ID = Sql.ToGuid(hashTeamNames[sTEAM_NAME]);
															}
														}
														if ( !dtProcessed.Columns.Contains(sName) )
														{
															dtProcessed.Columns.Add(sName);
														}
														row["IMPORT_ROW_STATUS" ] = true ;
														row["IMPORT_LAST_COLUMN"] = sName;
														row[sName] = sText;
														// 09/19/2007 Paul.  Use a separate command object to speed import. 
														// It also allows us to capture the SQL for the dump string. 
														//SplendidDynamic.UpdateTeam(trn, gID, sImportModule, gTEAM_ID);
														Sql.SetParameter(cmdImportTeam, "@ID"     , gID     );
														Sql.SetParameter(cmdImportTeam, "@TEAM_ID", gTEAM_ID);
														
														sbImport.Append(Sql.ExpandParameters(cmdImportTeam));
														sbImport.Append(";");
														sbImport.Append(ControlChars.CrLf);
														cmdImportTeam.ExecuteNonQuery();
													}
												}
											}
											*/
											if ( cmdImportCSTM != null && parID_C != null )
											{
												parID_C.Value = gID;
												sbImport.Append(Sql.ExpandParameters(cmdImportCSTM));
												sbImport.Append(";");
												sbImport.Append(ControlChars.CrLf);
												cmdImportCSTM.ExecuteNonQuery();
											}

										}
										nImported++;
										row["IMPORT_LAST_COLUMN"] = DBNull.Value;
									}
									Response.Write(" ");
								}
								catch(Exception ex)
								{
									row["IMPORT_ROW_STATUS"] = false;
									row["IMPORT_ROW_ERROR" ] = L10n.Term("Import.LBL_ERROR") + " " + Sql.ToString(row["IMPORT_LAST_COLUMN"]) + ". " + ex.Message;
									nFailed++;
									// 10/31/2006 Paul.  Abort after 200 errors. 
									if ( nFailed >= nMAX_ERRORS )
									{
										ctlImportButtons.ErrorText += L10n.Term("Import.LBL_MAX_ERRORS");
										break;
									}
								}
							}
							// 10/29/2006 Paul.  Save the processed table so that the result can be browsed. 
							string sProcessedFileID   = Guid.NewGuid().ToString();
							string sProcessedFileName = Security.USER_ID.ToString() + " " + Guid.NewGuid().ToString() + ".xml";
							DataSet dsProcessed = new DataSet();
							dsProcessed.Tables.Add(dtProcessed);
							dsProcessed.WriteXml(Path.Combine(Path.GetTempPath(), sProcessedFileName), XmlWriteMode.WriteSchema);
							Session["TempFile." + sProcessedFileID] = sProcessedFileName;
							ViewState["ProcessedFileID"] = sProcessedFileID;

							// 10/31/2006 Paul.  The transaction should rollback if it is not explicitly committed. 
							// Manually rolling back is causing a timeout. 
							//if ( bPreview || nFailed > 0 )
							//	trn.Rollback();
							//else
							if ( trn != null && !bPreview && nFailed == 0 )
							{
								trn.Commit();
							}
						}
						catch(Exception ex)
						{
							// 10/31/2006 Paul.  The transaction should rollback if it is not explicitly committed. 
							//if ( trn.Connection != null )
							//	trn.Rollback();
							// 10/31/2006 Paul.  Don't throw this exception.  We want to be able to display the failed count. 
							nFailed++;
							//throw(new Exception(ex.Message, ex.InnerException));
							ctlImportButtons.ErrorText += ex.Message;
						}
						finally
						{
							if ( trn != null )
								trn.Dispose();
						}
					}
				}
				if ( nFailed == 0 )
					lblStatus.Text = L10n.Term("Import.LBL_SUCCESS");
				else
					lblStatus.Text = L10n.Term("Import.LBL_FAIL"   );
				lblSuccessCount.Text = nImported.ToString() + " " + L10n.Term("Import.LBL_SUCCESSFULLY" );
				lblFailedCount.Text  = nFailed.ToString()   + " " + L10n.Term("Import.LBL_FAILED_IMPORT");

				grdMain.SortColumn = "IMPORT_ROW_STATUS, IMPORT_ROW_NUMBER";
				grdMain.SortOrder  = "asc" ;
				PreviewGrid(dtProcessed);
			}
			catch ( Exception ex )
			{
				ctlImportButtons.ErrorText += ex.Message;
			}
		}

		private void PreviewGrid(DataTable dtProcessed)
		{
			vwColumns.Sort = "DISPLAY_NAME";
			Hashtable hashColumns = new Hashtable();
			foreach ( DataRowView row in vwColumns )
				hashColumns.Add(row["NAME"], row["DISPLAY_NAME"]);

			// 10/31/2006 Paul.  Always reset columns before adding them. 
			grdMain.Columns.Clear();
			BoundColumn bnd = new BoundColumn();
			bnd.DataField  = "IMPORT_ROW_NUMBER";
			bnd.SortExpression = bnd.DataField;
			bnd.HeaderText = L10n.Term("Import.LBL_ROW");
			grdMain.Columns.Add(bnd);

			bnd = new BoundColumn();
			bnd.DataField  = "IMPORT_ROW_ERROR";
			bnd.SortExpression = bnd.DataField;
			bnd.HeaderText = L10n.Term("Import.LBL_ROW_STATUS");
			grdMain.Columns.Add(bnd);

			for ( int i = 4; i < dtProcessed.Columns.Count; i++ )
			{
				bnd = new BoundColumn();
				bnd.DataField = dtProcessed.Columns[i].ColumnName;
				bnd.SortExpression = bnd.DataField;
				if ( hashColumns.ContainsKey(bnd.DataField) )
					bnd.HeaderText = hashColumns[bnd.DataField] as string;
				else
					bnd.HeaderText = bnd.DataField;
				grdMain.Columns.Add(bnd);
			}
			
			grdMain.DataSource = new DataView(dtProcessed);
			grdMain.ApplySort();
			grdMain.DataBind();
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Import.Load" )
				{
					gID = Sql.ToGuid(e.CommandArgument);
					Response.Redirect(Request.Path + "?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Import.Delete" )
				{
					gID = Sql.ToGuid(e.CommandArgument);
					SqlProcs.spIMPORT_MAPS_Delete(gID);
					BindSaved();
					txtACTIVE_TAB.Value = "1";
				}
				else if ( e.CommandName == "Import.Save" )
				{
					reqNAME.Enabled = true;
					reqNAME.Validate();
					if ( Page.IsValid )
					{
						// 10/12/2006 Paul.  Save the sample data with the mappings. 
						XmlUtil.SetSingleNode(xmlMapping, "Sample", xml.OuterXml);
						SqlProcs.spIMPORT_MAPS_Update(ref gID, Security.USER_ID, txtNAME.Text, SourceType(), sImportModule, chkHasHeader.Checked, false, xmlMapping.OuterXml);
						XmlUtil.SetSingleNode(xmlMapping, "Sample", String.Empty);
						txtNAME.Text = String.Empty;
						BindSaved();
					}
					else
					{
						txtACTIVE_TAB.Value = "1";
					}
				}
				else if ( e.CommandName == "Import.Run" || e.CommandName == "Import.Preview" )
				{
					if ( Page.IsValid && !bDuplicateFields )
					{
						// 10/10/2006 Paul.  The temp file name is stored in the session so that it is impossible for a hacker to access. 
						string sTempFileID   = Sql.ToString(ViewState["TempFileID"]);
						string sTempFileName = Sql.ToString(Session["TempFile." + sTempFileID]);
						if ( Sql.IsEmptyString(sTempFileID) || Sql.IsEmptyString(sTempFileName) )
						{
							txtACTIVE_TAB.Value = "3";
							throw(new Exception(L10n.Term("Import.LBL_NOTHING")));
						}

						// 10/10/2006 Paul.  If there is a validation error, we want to display the mappings page. 
						// If there is no error, or if the error was during import, then show the results page. 
						txtACTIVE_TAB.Value = "4";
						ValidateMappings();
						txtACTIVE_TAB.Value = "5";
						GenerateImport(sTempFileName, e.CommandName == "Import.Preview");
					}
				}
				else if ( e.CommandName == "Import.Upload" )
				{
					reqFILENAME.Enabled = true;
					reqFILENAME.Validate();
					if ( Page.IsValid )
					{
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								
								xml = SplendidImport.ConvertStreamToXml(sImportModule, SourceType(), txtCUSTOM_DELIMITER_VAL.Text, pstIMPORT.InputStream);
								if ( xml.DocumentElement == null )
									throw(new Exception(L10n.Term("Import.LBL_NOTHING")));
								
								// 08/21/2006 Paul.  Don't move to next step if there is no data. 
								XmlNodeList nlRows = xml.DocumentElement.SelectNodes(sImportModule.ToLower());
								if ( nlRows.Count == 0 )
									throw(new Exception(L10n.Term("Import.LBL_NOTHING")));
								
								// 10/10/2006 Paul.  Don't store the file name in the ViewState because a hacker could find a way to access and alter it.
								// Storing the file name in the session and an ID in the view state should be sufficiently safe. 
								string sTempFileID   = Guid.NewGuid().ToString();
								string sTempFileName = Security.USER_ID.ToString() + " " + Guid.NewGuid().ToString() + " " + sFILENAME + ".xml";
								xml.Save(Path.Combine(Path.GetTempPath(), sTempFileName));
								Session["TempFile." + sTempFileID] = sTempFileName;
								ViewState["TempFileID"] = sTempFileID;
								
								// 10/10/2006 Paul.  We only need to save a small portion of the imported data as a sample. 
								// Trying to save too much data in ViewState can cause memory errors. 
								// 10/31/2006 Paul.  It is taking too long to reduce the size of a large XML file. 
								// Instead, extract the three rows and attach to a new XML document. 
								XmlDocument xmlSample = new XmlDocument();
								xmlSample.AppendChild(xmlSample.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
								xmlSample.AppendChild(xmlSample.CreateElement("xml"));
								// 10/31/2006 Paul.  Select only the nodes that apply.  We need to make sure to skip unrelated nodes. 
								for ( int i = 0; i < nlRows.Count && i < 3 ; i++ )
								{
									XmlNode node = nlRows[i];
									xmlSample.DocumentElement.AppendChild(xmlSample.ImportNode(node, true));
								}
								// 10/31/2006 Paul.  We are getting an OutOfMemoryException.  Try to free the large XML file. 
								xml = null;
								nlRows = null;
								xml = xmlSample;
								GC.Collect();
								ViewState["xmlSample"] = xml.OuterXml;

								bool bUpdateMapping = (Request["ID"] == null);
								UpdateImportMappings(xml, bUpdateMapping, bUpdateMapping);
								txtACTIVE_TAB.Value = "4";
							}
						}
					}
					if ( xml.DocumentElement == null )
						throw(new Exception(L10n.Term("Import.LBL_NOTHING")));
				}
				else if ( e.CommandName == "Cancel" )
				{
					string sRelativePath = Sql.ToString(Application["Modules." + sImportModule + ".RelativePath"]);
					if ( Sql.IsEmptyString(sRelativePath) )
						sRelativePath = "~/" + sImportModule + "/";
					Response.Redirect(sRelativePath);
				}
			}
			catch(Exception ex)
			{
				//SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlImportButtons.ErrorText += ex.Message;
				return;
			}
		}

		private string TableColumnName(string sModule, string sDISPLAY_NAME)
		{
			// 07/04/2006 Paul.  Some columns have global terms. 
			// 09/19/2007 Paul.  Include team fields in global terms. 
			if (  sDISPLAY_NAME == "DATE_ENTERED" 
			   || sDISPLAY_NAME == "DATE_MODIFIED"
			   || sDISPLAY_NAME == "ASSIGNED_TO"  
			   || sDISPLAY_NAME == "CREATED_BY"   
			   || sDISPLAY_NAME == "MODIFIED_BY"  
			   || sDISPLAY_NAME == "TEAM_ID"      
			   || sDISPLAY_NAME == "TEAM_NAME"    )
			{
				sDISPLAY_NAME = L10n.Term(".LBL_" + sDISPLAY_NAME).Replace(":", "");
			}
			else
			{
				// 07/04/2006 Paul.  Column names are aliased so that we don't have to redefine terms. 
				sDISPLAY_NAME = L10n.AliasedTerm(sModule + ".LBL_" + sDISPLAY_NAME).Replace(":", "");
			}
			return sDISPLAY_NAME;
		}

		private void InitMapping()
		{
			// 10/18/2006 Paul.  Initalize the fields. 
			XmlUtil.RemoveAllChildren(xmlMapping, "Fields");
			XmlNode xFields = xmlMapping.DocumentElement.SelectSingleNode("Fields");
			if ( xFields == null )
			{
				xFields = xmlMapping.CreateElement("Fields");
				xmlMapping.DocumentElement.AppendChild(xFields);
			}

			vwColumns.Sort = "colid";
			foreach ( DataRowView row in vwColumns )
			{
				XmlNode xField = xmlMapping.CreateElement("Field");
				xFields.AppendChild(xField);
				
				string sColumnName = Sql.ToString(row["Name"]);
				XmlUtil.SetSingleNodeAttribute(xmlMapping, xField, "Name", sColumnName);
				XmlUtil.SetSingleNode(xmlMapping, xField, "Type"   , Sql.ToString(row["ColumnType"]));
				XmlUtil.SetSingleNode(xmlMapping, xField, "Length" , Sql.ToString(row["Size"]));
				XmlUtil.SetSingleNode(xmlMapping, xField, "Default", String.Empty);
				XmlUtil.SetSingleNode(xmlMapping, xField, "Mapping", String.Empty);
			}
		}

		private void BindSaved()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *                                   " + ControlChars.CrLf
				     + "  from vwIMPORT_MAPS_List                  " + ControlChars.CrLf
				     + " where MODULE           = @MODULE          " + ControlChars.CrLf
				     + "   and ASSIGNED_USER_ID = @ASSIGNED_USER_ID" + ControlChars.CrLf
				     + " order by NAME                             " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@MODULE"          , sImportModule   );
					Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", Security.USER_ID);

					if ( bDebug )
						RegisterClientScriptBlock("vwIMPORT_MAPS_List", Sql.ClientScriptBlock(cmd));

					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							vwMySaved = new DataView(dt);
							grdMySaved.DataSource = vwMySaved ;
							grdMySaved.DataBind();
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Import.LBL_MODULE_NAME"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "import") >= 0);
			if ( !this.Visible )
				return;

			xml = new XmlDocument();
			xmlMapping = new XmlDocument();
			xmlMapping.AppendChild(xmlMapping.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
			xmlMapping.AppendChild(xmlMapping.CreateElement("Import"));

			sbImport = new StringBuilder();
			try
			{
				// 11/01/2006 Paul.  Max errors is now a config value. 
				nMAX_ERRORS = Sql.ToInteger(Application["CONFIG.import_max_errors"]);
				if ( nMAX_ERRORS <= 0 )
					nMAX_ERRORS = 200;

				// 07/02/2006 Paul.  The required fields need to be bound manually. 
				reqNAME    .ErrorMessage = L10n.Term(".ERR_REQUIRED_FIELD");
				reqFILENAME.ErrorMessage = L10n.Term(".ERR_REQUIRED_FIELD");
				// 12/17/2005 Paul.  Don't buffer so that the connection can be kept alive. 
				Response.BufferOutput = false;

				BindSaved();
				// 10/08/2006 Paul.  Columns table is used in multiple locations.  Make sure to load only once. 
				DataTable dtColumns = SplendidCache.ImportColumns(sImportModule).Copy();
				foreach ( DataRow row in dtColumns.Rows )
				{
					row["DISPLAY_NAME"] = TableColumnName(sImportModule, Sql.ToString(row["DISPLAY_NAME"]));
				}
				vwColumns = new DataView(dtColumns);

				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					radEXCEL.Checked = true;
					chkHasHeader.Checked = true;
					txtACTIVE_TAB.Value = "1";

					radEXCEL           .DataBind();
					radXML_SPREADSHEET .DataBind();
					radXML             .DataBind();
					radSALESFORCE      .DataBind();
					radACT_2005        .DataBind();
					radCUSTOM_CSV      .DataBind();
					radCUSTOM_TAB      .DataBind();
					radCUSTOM_DELIMITED.DataBind();

					radEXCEL           .Attributes.Add("onclick", "SelectSourceFormat()");
					radXML_SPREADSHEET .Attributes.Add("onclick", "SelectSourceFormat()");
					radXML             .Attributes.Add("onclick", "SelectSourceFormat()");
					radSALESFORCE      .Attributes.Add("onclick", "SelectSourceFormat()");
					radACT_2005        .Attributes.Add("onclick", "SelectSourceFormat()");
					radCUSTOM_CSV      .Attributes.Add("onclick", "SelectSourceFormat()");
					radCUSTOM_TAB      .Attributes.Add("onclick", "SelectSourceFormat()");
					radCUSTOM_DELIMITED.Attributes.Add("onclick", "SelectSourceFormat()");
					ctlListHeader.Title = L10n.Term("Import.LBL_LAST_IMPORTED") + " " + L10n.Term(".moduleList.", sImportModule);

					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL;
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwIMPORT_MAPS_Edit" + ControlChars.CrLf
							     + " where ID = @ID          " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("vwIMPORT_MAPS_Edit", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										SourceType(Sql.ToString(rdr["SOURCE"]));
										chkHasHeader.Checked = Sql.ToBoolean(rdr["HAS_HEADER"]);
										
										string sXmlMapping = Sql.ToString (rdr["CONTENT"]);
										ViewState["xmlMapping"] = sXmlMapping;
										xmlMapping.LoadXml(sXmlMapping);
										
										// 10/12/2006 Paul.  Extract the sample from the mapping. 
										xml = new XmlDocument();
										string sXmlSample = XmlUtil.SelectSingleNode(xmlMapping, "Sample");
										ViewState["xmlSample"] = sXmlSample;
										XmlUtil.SetSingleNode(xmlMapping, "Sample", String.Empty);
										if ( sXmlSample.Length > 0 )
											xml.LoadXml(sXmlSample);

										UpdateImportMappings(xml, false, false);
										txtACTIVE_TAB.Value = "3";
									}
								}
							}
						}
					}
					else
					{
						XmlUtil.SetSingleNodeAttribute(xmlMapping, xmlMapping.DocumentElement, "Name", String.Empty);
						XmlUtil.SetSingleNode(xmlMapping, "Module"    , sImportModule);
						XmlUtil.SetSingleNode(xmlMapping, "SourceType", SourceType() );
						XmlUtil.SetSingleNode(xmlMapping, "HasHeader" , chkHasHeader.Checked.ToString());
						InitMapping();
					}
				}
				else
				{
					string sXmlMapping = Sql.ToString(ViewState["xmlMapping"]);
					if ( sXmlMapping.Length > 0 )
						xmlMapping.LoadXml(sXmlMapping);
					
					XmlUtil.SetSingleNodeAttribute(xmlMapping, xmlMapping.DocumentElement, "Name", txtNAME.Text);
					XmlUtil.SetSingleNode(xmlMapping, "Module"    , sImportModule);
					XmlUtil.SetSingleNode(xmlMapping, "SourceType", SourceType() );
					XmlUtil.SetSingleNode(xmlMapping, "HasHeader" , chkHasHeader.Checked.ToString());

					// 10/10/2006 Paul.  This loop updates the default values. Field mappings are updated inside UpdateImportMappings(). 
					XmlNodeList nlFields = xmlMapping.DocumentElement.SelectNodes("Fields/Field");
					foreach ( XmlNode xField in nlFields )
					{
						string sFieldName = xField.Attributes.GetNamedItem("Name").Value;
						DynamicControl ctl = new DynamicControl(ctlDefaultsView, sFieldName);
						if ( ctl.Exists )
						{
							XmlUtil.SetSingleNode(xmlMapping, xField, "Default", ctl.Text);
						}
					}

					string sXmlSample = Sql.ToString(ViewState["xmlSample"]);
					if ( sXmlSample.Length > 0 )
					{
						xml.LoadXml(sXmlSample);
						UpdateImportMappings(xml, false, true);
					}
					
					string sProcessedFileID   = Sql.ToString(ViewState["ProcessedFileID"]);
					string sProcessedFileName = Sql.ToString(Session["TempFile." + sProcessedFileID]);
					string sProcessedPathName = Path.Combine(Path.GetTempPath(), sProcessedFileName);
					if ( File.Exists(sProcessedPathName) )
					{
						DataSet dsProcessed = new DataSet();
						dsProcessed.ReadXml(sProcessedPathName);
						if ( dsProcessed.Tables.Count == 1 )
						{
							PreviewGrid(dsProcessed.Tables[0]);
						}
					}
				}
			}
			catch ( Exception ex )
			{
				ctlImportButtons.ErrorText = ex.Message;
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			ViewState["xmlMapping"] = xmlMapping.OuterXml;
			ViewState["xmlSample" ] = xml.OuterXml;
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
			this.PreRender += new System.EventHandler(this.Page_PreRender);
			ctlImportButtons.Command = new CommandEventHandler(Page_Command);
			this.m_sMODULE = "Import";
			
			string sRelativePath = Sql.ToString(Application["Modules." + sImportModule + ".RelativePath"]);
			if ( Sql.IsEmptyString(sRelativePath) )
				sRelativePath = "~/" + sImportModule + "/";
			ctlDefaultsView = LoadControl(sRelativePath + "ImportDefaultsView.ascx") as SplendidControl;
			if ( ctlDefaultsView != null )
				phDefaultsView.Controls.Add(ctlDefaultsView);
		}
		#endregion
	}
}
