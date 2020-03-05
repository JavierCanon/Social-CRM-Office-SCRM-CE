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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2006 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.IO;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	public class RdlDocument : XmlDocument
	{
		// "http://schemas.microsoft.com/sqlserver/reporting/2003/10/reportdefinition"
		// 06/20/2006 Paul.  Use the 2005 spec as it has better support for custom properties. 
		public static string sDefaultNamespace  = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";
		public static string sDesignerNamespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

		private XmlNamespaceManager nsmgr;

		public void LoadRdl(string rdl)
		{
			base.LoadXml(rdl);
			nsmgr = new XmlNamespaceManager(this.NameTable);
			// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
			nsmgr.AddNamespace("defaultns", sDefaultNamespace );
			// 06/22/2006 Paul.  The designer namespace must be manually added. 
			nsmgr.AddNamespace("rd", sDesignerNamespace);
		}

		public XmlNode SelectNode(string sNode)
		{
			return XmlUtil.SelectNode(this, sNode, nsmgr);
		}

		public string SelectNodeValue(string sNode)
		{
			string sValue = String.Empty;
			XmlNode xValue = XmlUtil.SelectNode(this, sNode, nsmgr);
			if ( xValue != null )
				sValue = xValue.InnerText;
			return sValue;
		}

		public string SelectNodeAttribute(string sNode, string sAttribute)
		{
			string sValue = String.Empty;
			XmlNode xNode = null;
			if ( sNode == String.Empty )
				xNode = this.DocumentElement;
			else
				xNode = XmlUtil.SelectNode(this, sNode, nsmgr);
			if ( xNode != null )
			{
				if ( xNode.Attributes != null )
				{
					XmlNode xValue = xNode.Attributes.GetNamedItem(sAttribute);
					if ( xValue != null )
						sValue = xValue.Value;
				}
			}
			return sValue;
		}

		public string SelectNodeValue(XmlNode parent, string sNode)
		{
			return XmlUtil.SelectSingleNode(parent, sNode, nsmgr);
		}

		public XmlNodeList SelectNodesNS(string sXPath)
		{
			string[] arrXPath = sXPath.Split('/');
			for ( int i = 0; i < arrXPath.Length; i++ )
			{
				// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
				if ( arrXPath[i].IndexOf(':') < 0 )
					arrXPath[i] = "defaultns:" + arrXPath[i];
			}
			sXPath = String.Join("/", arrXPath);
			return this.DocumentElement.SelectNodes(sXPath, nsmgr);
		}

		public XmlNodeList SelectNodesNS(XmlNode parent, string sXPath)
		{
			string[] arrXPath = sXPath.Split('/');
			for ( int i = 0; i < arrXPath.Length; i++ )
			{
				// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
				if ( arrXPath[i].IndexOf(':') < 0 )
					arrXPath[i] = "defaultns:" + arrXPath[i];
			}
			sXPath = String.Join("/", arrXPath);
			return parent.SelectNodes(sXPath, nsmgr);
		}

		public void SetSingleNode(string sNode, string sValue)
		{
			XmlUtil.SetSingleNode(this, sNode, sValue, nsmgr, sDefaultNamespace);
		}

		public void SetSingleNode(XmlNode parent, string sNode, string sValue)
		{
			XmlUtil.SetSingleNode(this, parent, sNode, sValue, nsmgr, sDefaultNamespace);
		}

		public void SetSingleNodeAttribute(string sNode, string sAttribute, string sValue)
		{
			XmlUtil.SetSingleNodeAttribute(this, sNode, sAttribute, sValue, nsmgr, sDefaultNamespace);
		}

		public void SetSingleNodeAttribute(XmlNode parent, string sAttribute, string sValue)
		{
			XmlUtil.SetSingleNodeAttribute(this, parent, sAttribute, sValue, nsmgr, sDefaultNamespace);
		}

		public RdlDocument() : base()
		{
		}

		public RdlDocument(string sNAME, string sAUTHOR) : base()
		{
			this.AppendChild(this.CreateXmlDeclaration("1.0", "UTF-8", null));
			//this.AppendChild(this.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));

			this.AppendChild(this.CreateElement("Report", sDefaultNamespace));
			XmlUtil.SetSingleNodeAttribute(this, this.DocumentElement, "Name", sNAME);
			// 06/20/2006 Paul.  Add the RD namespace manually. 
			XmlUtil.SetSingleNodeAttribute(this, this.DocumentElement, "xmlns:rd", sDesignerNamespace);

			nsmgr = new XmlNamespaceManager(this.NameTable);
			nsmgr.AddNamespace(""  , sDefaultNamespace );
			nsmgr.AddNamespace("rd", sDesignerNamespace);
			// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
			nsmgr.AddNamespace("defaultns", sDefaultNamespace );

			// 06/18/2006 Paul.  The report definition element 'Report' is empty at line 14, position 5054. It is missing a mandatory child element of type 'Width'. 
			// Change PageWidth to Width, and remove namespace. 
			SetSingleNode("Width"       , "11in"      );
			SetSingleNode("PageWidth"   , "11in"      );
			SetSingleNode("PageHeight"  , "8.5in"     );
			SetSingleNode("LeftMargin"  , ".5in"      );
			SetSingleNode("RightMargin" , ".5in"      );
			SetSingleNode("TopMargin"   , ".5in"      );
			SetSingleNode("BottomMargin", ".5in"      );
			//SetSingleNode("Language"    , "en-US"     );
			SetSingleNode("Description" , String.Empty);
			SetSingleNode("Author"      , sAUTHOR     );

			string sDataProvider = String.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				if ( Sql.IsSQLServer(con) )
					sDataProvider = "SQL";
				else if ( Sql.IsOracle(con) )
					sDataProvider = "Oracle";
				else if ( Sql.IsDB2(con) )
					sDataProvider = "DB2";
				else if ( Sql.IsMySQL(con) )
					sDataProvider = "MySQL";
				else if ( Sql.IsSybase(con) )
					sDataProvider = "Sybase";
				else if ( Sql.IsSqlAnywhere(con) )
					sDataProvider = "SQL Anywhere";
				
			}

			SetSingleNode         ("CustomProperties", String.Empty);

			SetSingleNode         ("DataSources/DataSource", String.Empty);
			SetSingleNodeAttribute("DataSources/DataSource", "Name", "dataSource1");
			SetSingleNode         ("DataSources/DataSource/ConnectionProperties/DataProvider" , sDataProvider);
			SetSingleNode         ("DataSources/DataSource/ConnectionProperties/ConnectString", String.Empty );
			
			SetSingleNode         ("DataSets/DataSet", String.Empty);
			SetSingleNodeAttribute("DataSets/DataSet", "Name", "dataSet");
			SetSingleNode         ("DataSets/DataSet/Query/DataSourceName", "dataSource1");
			SetSingleNode         ("DataSets/DataSet/Query/CommandText"   , String.Empty );
			SetSingleNode         ("DataSets/DataSet/Fields"              , String.Empty );
			
			SetSingleNode         ("Body/ReportItems/Table", String.Empty);
			SetSingleNodeAttribute("Body/ReportItems/Table", "Name", "table1");
			SetSingleNode         ("Body/ReportItems/Table/DataSetName"                          , "dataSet"   );
			SetSingleNode         ("Body/ReportItems/Table/Header/RepeatOnNewPage"               , "true"      );
			
			// 06/21/2006 Paul.  TableRow requires a Height element. 
			SetSingleNode         ("Body/ReportItems/Table/Header/TableRows/TableRow/Height"     , "0.21in"    );
			SetSingleNode         ("Body/ReportItems/Table/Details/TableRows/TableRow/Height"    , "0.21in"    );

			SetSingleNode         ("Body/ReportItems/Table/Header/TableRows/TableRow/TableCells" , String.Empty);
			SetSingleNode         ("Body/ReportItems/Table/Details/TableRows/TableRow/TableCells", String.Empty);
			// 06/21/2006 Paul.  Table requires a TableColumns element. 
			SetSingleNode         ("Body/ReportItems/Table/TableColumns"                         , String.Empty);
			// 06/21/2006 Paul.  Body requires a Height element. 
			SetSingleNode         ("Body/Height"                                                 , "8.5in"      );
			
			// 06/21/2006 Paul.  If PageFooter is included, then it must have a Height element. 
			//SetSingleNode         ("PageFooter", String.Empty);
			//SetSingleNode         ("PageHeader", String.Empty);
		}

		public XmlDocument GetCustomProperty(string sName)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateElement(sName));
			
			try
			{
				XmlNode xValue = this.DocumentElement.SelectSingleNode("defaultns:CustomProperties/defaultns:CustomProperty[defaultns:Name=\'crm:" + sName + "\']/defaultns:Value", nsmgr);
				if ( xValue != null )
				{
					string sValue = xValue.InnerText;
					if ( !Sql.IsEmptyString(sValue) )
					{
						xml.LoadXml(sValue);
					}
				}
			}
			catch
			{
			}
			return xml;
		}

		public string GetCustomPropertyValue(string sName)
		{
			string sValue = String.Empty;
			try
			{
				XmlNode xValue = this.DocumentElement.SelectSingleNode("defaultns:CustomProperties/defaultns:CustomProperty[defaultns:Name=\'crm:" + sName + "\']/defaultns:Value", nsmgr);
				if ( xValue != null )
				{
					sValue = xValue.InnerText;
				}
			}
			catch
			{
			}
			return sValue;
		}

		public void SetCustomProperty(string sName, string sValue)
		{
			// "CustomProperties"
			XmlNode xCustomProperties = this.DocumentElement.SelectSingleNode("defaultns:CustomProperties", nsmgr);
			if ( xCustomProperties == null )
			{
				xCustomProperties = this.CreateElement("CustomProperties", sDefaultNamespace);
				this.DocumentElement.AppendChild(xCustomProperties);
			}
			// 05/27/2006 Paul.  All SplendidCRM properties should start with "crm". 
			XmlNode xCustomProperty = xCustomProperties.SelectSingleNode("defaultns:CustomProperty[defaultns:Name=\'crm:" + sName + "\']", nsmgr);
			if ( xCustomProperty == null )
			{
				xCustomProperty = this.CreateElement("CustomProperty", sDefaultNamespace);
				xCustomProperties.AppendChild(xCustomProperty);

				XmlNode xName  = this.CreateElement("Name" , sDefaultNamespace);
				XmlNode xValue = this.CreateElement("Value", sDefaultNamespace);
				xCustomProperty.AppendChild(xName );
				xCustomProperty.AppendChild(xValue);
				xName.InnerText  = "crm:" + sName;
				xValue.InnerText = sValue;
			}
			else
			{
				XmlNode xValue = xCustomProperty.SelectSingleNode("defaultns:Value", nsmgr);
				if ( xValue == null )
				{
					xValue = this.CreateElement("Value", sDefaultNamespace);
					xCustomProperty.AppendChild(xValue);
				}
				xValue.InnerText = sValue;
			}
		}

		public DataTable CreateDataTable()
		{
			DataTable dt = new DataTable("ReportItems");
			dt.Columns.Add("text" );
			dt.Columns.Add("value");
			if ( this.DocumentElement != null )
			{
				XmlNodeList nlHeaderValues = this.SelectNodesNS("Body/ReportItems/Table/Header/TableRows/TableRow/TableCells/TableCell/ReportItems/Textbox/Value" );
				XmlNodeList nlDetailValues = this.SelectNodesNS("Body/ReportItems/Table/Details/TableRows/TableRow/TableCells/TableCell/ReportItems/Textbox/Value");
				for ( int iDetail = 0; iDetail < nlDetailValues.Count; iDetail++ )
				{
					if ( iDetail < nlHeaderValues.Count )
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						try
						{
							string sHeader = nlHeaderValues[iDetail].InnerText;
							string sDetail = nlDetailValues[iDetail].InnerText;
							if ( sDetail.StartsWith("=Fields!") && sDetail.EndsWith(".Value") )
							{
								sDetail = sDetail.Substring("=Fields!".Length, sDetail.Length - "=Fields!".Length - ".Value".Length);
								// 06/18/2006 Paul.  Now translate the field name to the column name. 
								XmlNode xField = this.SelectNode("DataSets/DataSet/Fields/Field[@Name='" + sDetail + "']/DataField");
								if ( xField != null )
									sDetail = xField.InnerText;
							}
							row["text" ] = sHeader;
							row["value"] = sDetail;
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
			}
			return dt;
		}

		public static string RdlName(string sName)
		{
			sName = Regex.Replace(sName, @"[\[\]]" , "");
			sName = Regex.Replace(sName, @"[\.\: ]", "__");
			return sName;
		}

		// 07/12/2006 Paul.  The textbox name needs to be derived from the field name.
		// There are just too many pitfalls of attempting to derive the textbox from the display name.
		// We would have to escape all characters except alpha-numerics. 
		// When we use the field name as the base, then we only have to escape a few characters. 
		public void CreateTextboxValue(XmlNode parent, string sTextboxName, string sValue, bool bField)
		{
			XmlNode xTableCell   = this.CreateElement("TableCell"  , sDefaultNamespace);
			XmlNode xReportItems = this.CreateElement("ReportItems", sDefaultNamespace);
			XmlNode xTextbox     = this.CreateElement("Textbox"    , sDefaultNamespace);
			//XmlNode xWidth       = this.CreateElement("Width"      , sDefaultNamespace);
			XmlNode xValue       = this.CreateElement("Value"      , sDefaultNamespace);
			parent.AppendChild(xTableCell);
			xTableCell.AppendChild(xReportItems);
			xReportItems.AppendChild(xTextbox);
			xTextbox.AppendChild(xValue);
			if ( bField )
			{
				// 06/21/2006 Paul.  Textbox requires a Name attribute. 
				this.SetSingleNodeAttribute(xTextbox, "Name", RdlName(sTextboxName) + "__Value");
				// 06/18/2006 Paul.  Field names must be CLS-compliant identifiers. Use two underscores to distinguish betwen typical use of underscore. 
				xValue.InnerText = "=Fields!" + RdlName(sValue) + ".Value";
				this.SetSingleNode(xTextbox, "Style/PaddingLeft"  , "2pt");
				this.SetSingleNode(xTextbox, "Style/PaddingRight" , "2pt");
				this.SetSingleNode(xTextbox, "Style/PaddingBottom", "2pt");
				this.SetSingleNode(xTextbox, "Style/PaddingTop"   , "2pt");
				this.SetSingleNode(xTextbox, "CanGrow"            , "true");
				//this.SetSingleNode(xTextbox, "rd:DefaultName"     , sValue);
			}
			else
			{
				// 06/21/2006 Paul.  Textbox requires a Name attribute. 
				this.SetSingleNodeAttribute(xTextbox, "Name", RdlName(sTextboxName) + "__Header");
				//xTextbox.AppendChild(xWidth);
				//xWidth.InnerText = "1in";
				xValue.InnerText = sValue;
				this.SetSingleNode(xTextbox, "Style/PaddingLeft"  , "2pt");
				this.SetSingleNode(xTextbox, "Style/PaddingRight" , "2pt");
				this.SetSingleNode(xTextbox, "Style/PaddingBottom", "2pt");
				this.SetSingleNode(xTextbox, "Style/PaddingTop"   , "2pt");
				this.SetSingleNode(xTextbox, "Style/FontWeight"   , "900");
				this.SetSingleNode(xTextbox, "CanGrow"            , "true");
				//this.SetSingleNode(xTextbox, "rd:DefaultName"     , sValue);
				this.SetSingleNode(xTextbox, "Style/BorderWidth/Bottom", "2pt"  );
				this.SetSingleNode(xTextbox, "Style/BorderColor/Bottom", "Black");
				this.SetSingleNode(xTextbox, "Style/BorderStyle/Bottom", "Solid");
			}
		}

		public void CreateField(XmlNode parent, string sFieldName)
		{
			XmlNode xField     = this.CreateElement("Field"    , sDefaultNamespace);
			XmlNode xDataField = this.CreateElement("DataField", sDefaultNamespace);
			parent.AppendChild(xField);
			xField.AppendChild(xDataField);
			xDataField.InnerText = sFieldName;
			
			XmlAttribute attr = this.CreateAttribute("Name");
			// 06/18/2006 Paul.  Field names must be CLS-compliant identifiers. Use two underscores to distinguish betwen typical use of underscore. 
			attr.Value = RdlName(sFieldName);
			xField.Attributes.SetNamedItem(attr);
		}

		public void CreateField(XmlNode parent, string sFieldName, string sFieldType)
		{
			XmlNode xField     = this.CreateElement("Field"      , sDefaultNamespace );
			XmlNode xDataField = this.CreateElement("DataField"  , sDefaultNamespace );
			XmlNode xTypeName  = this.CreateElement("rd:TypeName", sDesignerNamespace);
			parent.AppendChild(xField);
			xField.AppendChild(xDataField);
			xField.AppendChild(xTypeName);
			xDataField.InnerText = sFieldName;
			xTypeName.InnerText  = sFieldType;
			
			XmlAttribute attr = this.CreateAttribute("Name");
			// 06/18/2006 Paul.  Field names must be CLS-compliant identifiers. Use two underscores to distinguish betwen typical use of underscore. 
			attr.Value = RdlName(sFieldName);
			xField.Attributes.SetNamedItem(attr);
		}

		public void RemoveField(string sFieldName)
		{
			XmlNode xHeaderCells  = this.SelectNode("Body/ReportItems/Table/Header/TableRows/TableRow/TableCells" );
			XmlNode xDetailsCells = this.SelectNode("Body/ReportItems/Table/Details/TableRows/TableRow/TableCells");
			XmlNode xTableColumns = this.SelectNode("Body/ReportItems/Table/TableColumns");
			for ( int i = 0; i < xDetailsCells.ChildNodes.Count; i++ )
			{
				XmlNode xTableCell = xDetailsCells.ChildNodes[i];
				if ( xTableCell.SelectSingleNode("defaultns:ReportItems/defaultns:Textbox[defaultns:Value=\"=Fields!" + RdlName(sFieldName) + ".Value\"]", nsmgr) != null )
				{
					if ( i < xHeaderCells .ChildNodes.Count ) xHeaderCells .RemoveChild(xHeaderCells .ChildNodes[i]);
					if ( i < xDetailsCells.ChildNodes.Count ) xDetailsCells.RemoveChild(xDetailsCells.ChildNodes[i]);
					if ( i < xTableColumns.ChildNodes.Count ) xTableColumns.RemoveChild(xTableColumns.ChildNodes[i]);
					break;
				}
			}
		}

		public void UpdateDataTable(DataTable dtDisplayColumns)
		{
			SetSingleNode("Body/ReportItems/Table/Header/TableRows/TableRow/TableCells" , String.Empty);
			SetSingleNode("Body/ReportItems/Table/Details/TableRows/TableRow/TableCells", String.Empty);
			SetSingleNode("Body/ReportItems/Table/TableColumns" , String.Empty);
			//SetSingleNode("DataSets/DataSet/Fields", String.Empty);
			
			XmlNode xHeaderCells  = this.SelectNode("Body/ReportItems/Table/Header/TableRows/TableRow/TableCells" );
			XmlNode xDetailsCells = this.SelectNode("Body/ReportItems/Table/Details/TableRows/TableRow/TableCells");
			XmlNode xTableColumns = this.SelectNode("Body/ReportItems/Table/TableColumns");
			//XmlNode xFields       = this.SelectNode("DataSets/DataSet/Fields");
			xHeaderCells .RemoveAll();
			xDetailsCells.RemoveAll();
			xTableColumns.RemoveAll();
			//xFields.RemoveAll();
			if ( dtDisplayColumns != null )
			{
				string sWidth = SelectNodeValue("Width");
				sWidth = sWidth.Replace("in", "");
				double dWidth = Sql.ToDouble(sWidth);
				if ( dWidth == 0.0 )
					dWidth = 15.0;
				if ( dtDisplayColumns.Rows.Count > 0 )
				{
					dWidth = dWidth / dtDisplayColumns.Rows.Count;
					// 11/24/2006 Paul.  The ReportViewer only understands the en-US decimal format. 
					CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
					sWidth = dWidth.ToString("#.##", ciEnglish.NumberFormat) + "in";
				}
				foreach ( DataRow row in dtDisplayColumns.Rows )
				{
					string sFieldLabel = Sql.ToString(row["Label"]);
					string sFieldName  = Sql.ToString(row["Field"]);
					CreateTextboxValue(xHeaderCells , sFieldName , sFieldLabel, false);
					CreateTextboxValue(xDetailsCells, sFieldName , sFieldName , true );
					//CreateField(xFields, sFieldName);
					
					XmlNode xTableColumn = this.CreateElement("TableColumn", sDefaultNamespace);
					XmlNode xWidth       = this.CreateElement("Width"      , sDefaultNamespace);
					xTableColumns.AppendChild(xTableColumn);
					xTableColumn.AppendChild(xWidth);
					xWidth.InnerText = sWidth;
				}
			}
		}

		private string RdlValue(string sValue)
		{
			if ( sValue.StartsWith("=") )
			{
				sValue = sValue.Substring(1);
				if ( sValue.StartsWith("\"") && sValue.EndsWith("\"") )
				{
					sValue = sValue.Substring(1, sValue.Length - 2);
					sValue = sValue.Replace("\"\"", "\"");
				}
			}
			return sValue;
		}

		public static string RdlParameterName(string sDATA_FIELD, int nParameterIndex, bool bSecondary)
		{
			//
			return "@" + RdlDocument.RdlName(sDATA_FIELD) + "__" + nParameterIndex.ToString("00") + (bSecondary ? "B" : "A");
		}

		public static string RdlFieldFromParameter(string sName)
		{
			// 07/13/2006 Paul.  The field name now always starts with @ and ends with __00A, with the last two digits being the parameter index. 
			if ( sName.StartsWith("@") )
			{
				if ( sName.EndsWith("A") || sName.EndsWith("B") )
					return sName.Substring(1, sName.Length - 6);
			}
			return sName;
		}

		public void ReportViewerFixups()
		{
			// 07/13/2006 Paul.  The ReportViewer does not know how to translate the date functions. 
			// These functions should work fine on the report server, so we are just going to work-around the ReportViewer problems. 
			XmlNodeList nlQueryParameters = this.SelectNodesNS("DataSets/DataSet/Query/QueryParameters/QueryParameter");
			foreach ( XmlNode xQueryParameter in nlQueryParameters )
			{
				string sName      = xQueryParameter.Attributes.GetNamedItem("Name").Value;
				string sValue     = this.SelectNodeValue(xQueryParameter, "Value");
				// 07/13/2006 Paul.  The field name now always starts with @ and ends with __00A, with the last two digits being the parameter index. 
				string sFieldName = RdlFieldFromParameter(sName);
				string sTypeName = this.SelectNodeValue("DataSets/DataSet/Fields/Field[@Name='" + sFieldName + "']/rd:TypeName");
				sValue = RdlValue(sValue);
				switch ( sTypeName )
				{
					case "System.DateTime":
					{
						switch ( sValue.ToUpper() )
						{
							case "TODAY()"                           :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today                    .ToShortDateString());  break;
							case "DATEADD(DAY, -1, TODAY())"         :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddDays( -1)       .ToShortDateString());  break;
							case "DATEADD(DAY, 1, TODAY())"          :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddDays(  1)       .ToShortDateString());  break;
							case "MONTH(TODAY())"                    :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.Month              .ToString()         );  break;
							case "MONTH(DATEADD(MONTH, -1, TODAY()))":  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddMonths(-1).Month.ToString()         );  break;
							case "MONTH(DATEADD(MONTH, 1, TODAY()))" :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddMonths( 1).Month.ToString()         );  break;
							case "YEAR(DATEADD(MONTH, -1, TODAY()))" :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddMonths(-1).Year .ToString()         );  break;
							case "YEAR(DATEADD(MONTH, 1, TODAY()))"  :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddMonths( 1).Year .ToString()         );  break;
							case "YEAR(TODAY())"                     :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.Year               .ToString()         );  break;
							case "YEAR(DATEADD(YEAR, -1, TODAY()))"  :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddYears(-1).Year  .ToString()         );  break;
							case "YEAR(DATEADD(YEAR, 1, TODAY()))"   :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddYears( 1).Year  .ToString()         );  break;
							case "DATEADD(DAY, -7, TODAY())"         :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddDays( -7)       .ToShortDateString());  break;
							case "DATEADD(DAY, 7, TODAY())"          :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddDays(  7)       .ToShortDateString());  break;
							case "DATEADD(DAY, -30, TODAY())"        :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddDays(-30)       .ToShortDateString());  break;
							case "DATEADD(DAY, 30, TODAY())"         :  this.SetSingleNode(xQueryParameter, "Value", DateTime.Today.AddDays( 30)       .ToShortDateString());  break;
						}
						break;
					}
				}
			}
		}

		public void BuildCommand(XmlNode xDataSet, IDbCommand cmd)
		{
			cmd.CommandText = XmlUtil.SelectSingleNode(xDataSet, "Query/CommandText", nsmgr);
			// 07/12/2006 Paul.  If not SQL Server and not Sybase, then we must remove the DBO. 
			if ( !Sql.IsSQLServer(cmd) && !Sql.IsSybase(cmd) )
			{
				cmd.CommandText = cmd.CommandText.Replace(" dbo.fnDateOnly(", " fnDateOnly(");
			}
			XmlNodeList nlQueryParameters = this.SelectNodesNS(xDataSet, "Query/QueryParameters/QueryParameter");
			foreach ( XmlNode xQueryParameter in nlQueryParameters )
			{
				string sName      = xQueryParameter.Attributes.GetNamedItem("Name").Value;
				string sValue     = this.SelectNodeValue(xQueryParameter, "Value");
				// 07/13/2006 Paul.  The field name now always starts with @ and ends with __00A, with the last two digits being the parameter index. 
				string sFieldName = RdlFieldFromParameter(sName);

				string  sTypeName = String.Empty;
				XmlNode xTypeName = XmlUtil.SelectNode(xDataSet, "Fields/Field[@Name='" + sFieldName + "']/rd:TypeName", nsmgr);
				if ( xTypeName != null )
					sTypeName = xTypeName.InnerText;

				sValue = RdlValue(sValue);
				if ( sValue.StartsWith("Parameters!") && sValue.EndsWith(".Value") )
				{
					string sParameterName = sValue.Substring(11, sValue.Length-11-6);
					XmlNode xReportParameterValue = this.SelectNode("ReportParameters/ReportParameter[@Name='" + sParameterName + "']/DefaultValue/Values/Value");
					if ( xReportParameterValue != null )
					{
						sValue = xReportParameterValue.InnerText;
						// 10/24/2007 Paul.  The ReportViewer will complain if the query parameter does not have a value. 
						this.SetSingleNode(xQueryParameter, "Value", sValue);
					}
				}
				switch ( sTypeName )
				{
					case "System.Guid"    :  Sql.AddParameter(cmd, sName, Sql.ToGuid    (sValue));  break;
					case "System.Boolean" :  Sql.AddParameter(cmd, sName, Sql.ToBoolean (sValue));  break;
					case "System.Double"  :  Sql.AddParameter(cmd, sName, Sql.ToFloat   (sValue));  break;
					case "System.Decimal" :  Sql.AddParameter(cmd, sName, Sql.ToDecimal (sValue));  break;
					case "System.Int16"   :  Sql.AddParameter(cmd, sName, Sql.ToInteger (sValue));  break;
					case "System.Int32"   :  Sql.AddParameter(cmd, sName, Sql.ToInteger (sValue));  break;
					case "System.Int64"   :  Sql.AddParameter(cmd, sName, Sql.ToInteger (sValue));  break;
					case "System.DateTime":
					{
						switch ( sValue.ToUpper() )
						{
							case "TODAY()"                           :  Sql.AddParameter(cmd, sName, DateTime.Today)             ;  break;
							case "DATEADD(DAY, -1, TODAY())"         :  Sql.AddParameter(cmd, sName, DateTime.Today.AddDays( -1));  break;
							case "DATEADD(DAY, 1, TODAY())"          :  Sql.AddParameter(cmd, sName, DateTime.Today.AddDays(  1));  break;
							case "MONTH(TODAY())"                    :  Sql.AddParameter(cmd, sName, DateTime.Today.Month)       ;  break;
							case "MONTH(DATEADD(MONTH, -1, TODAY()))":  Sql.AddParameter(cmd, sName, DateTime.Today.AddMonths(-1).Month);  break;
							case "MONTH(DATEADD(MONTH, 1, TODAY()))" :  Sql.AddParameter(cmd, sName, DateTime.Today.AddMonths( 1).Month);  break;
							case "YEAR(DATEADD(MONTH, -1, TODAY()))" :  Sql.AddParameter(cmd, sName, DateTime.Today.AddMonths(-1).Year );  break;
							case "YEAR(DATEADD(MONTH, 1, TODAY()))"  :  Sql.AddParameter(cmd, sName, DateTime.Today.AddMonths( 1).Year );  break;
							case "YEAR(TODAY())"                     :  Sql.AddParameter(cmd, sName, DateTime.Today.Year)               ;  break;
							case "YEAR(DATEADD(YEAR, -1, TODAY()))"  :  Sql.AddParameter(cmd, sName, DateTime.Today.AddYears(-1).Year)  ;  break;
							case "YEAR(DATEADD(YEAR, 1, TODAY()))"   :  Sql.AddParameter(cmd, sName, DateTime.Today.AddYears( 1).Year)  ;  break;
							case "DATEADD(DAY, -7, TODAY())"         :  Sql.AddParameter(cmd, sName, DateTime.Today.AddDays( -7));  break;
							case "DATEADD(DAY, 7, TODAY())"          :  Sql.AddParameter(cmd, sName, DateTime.Today.AddDays(  7));  break;
							case "DATEADD(DAY, -30, TODAY())"        :  Sql.AddParameter(cmd, sName, DateTime.Today.AddDays(-30));  break;
							case "DATEADD(DAY, 30, TODAY())"         :  Sql.AddParameter(cmd, sName, DateTime.Today.AddDays( 30));  break;
							default                                  :  Sql.AddParameter(cmd, sName, Sql.ToDateTime(sValue))     ;  break;
						}
						break;
					}
					case "System.String"  :  Sql.AddParameter(cmd, sName, sValue);  break;
					default               :  Sql.AddParameter(cmd, sName, sValue);  break;
				}
			}
			// 04/17/2007 Paul.  @MEMBERSHIP_USER_ID is a special replacement field that we use to filter results based on ACL rules. 
			cmd.CommandText = cmd.CommandText.Replace("@MEMBERSHIP_USER_ID", "'" + Security.USER_ID.ToString() + "'");
		}

		public void AddQueryParameter(XmlNode xQueryParameters, string sPARAMETER_NAME, string sDATA_TYPE, string sVALUE)
		{
			XmlNode xQueryParameter = this.CreateElement("QueryParameter", sDefaultNamespace);
			xQueryParameters.AppendChild(xQueryParameter);
			SetSingleNodeAttribute(xQueryParameter, "Name", sPARAMETER_NAME);
			if ( sDATA_TYPE == "string" || sDATA_TYPE == "enum" )
				SetSingleNode(xQueryParameter, "Value", "=\"" + sVALUE.Replace("\"", "\\\"") + "\"");
			else
				SetSingleNode(xQueryParameter, "Value", "=" + sVALUE);
		}

		public void AddQueryParameter(XmlNode xQueryParameters, string sPARAMETER_NAME, string sDATA_TYPE, string sVALUE1, string sVALUE2)
		{
			XmlNode xQueryParameter = this.CreateElement("QueryParameter", sDefaultNamespace);
			xQueryParameters.AppendChild(xQueryParameter);
			SetSingleNodeAttribute(xQueryParameter, "Name", sPARAMETER_NAME + "1");
			if ( sDATA_TYPE == "string" || sDATA_TYPE == "enum" )
				SetSingleNode(xQueryParameter, "Value", "=\"" + sVALUE1.Replace("\"", "\\\"") + "\"");
			else
				SetSingleNode(xQueryParameter, "Value", "=" + sVALUE1);

			xQueryParameter = this.CreateElement("QueryParameter", sDefaultNamespace);
			xQueryParameters.AppendChild(xQueryParameter);
			SetSingleNodeAttribute(xQueryParameter, "Name", sPARAMETER_NAME + "2");
			if ( sDATA_TYPE == "string" || sDATA_TYPE == "enum" )
				SetSingleNode(xQueryParameter, "Value", "=\"" + sVALUE2.Replace("\"", "\\\"") + "\"");
			else
				SetSingleNode(xQueryParameter, "Value", "=" + sVALUE2);
		}

	}


	/// <summary>
	/// Summary description for RdlUtil.
	/// </summary>
	public class RdlUtil
	{
		/*
		public static string GetFieldsAsString(XmlDocument xml)
		{
			StringBuilder sb = new StringBuilder();
			if ( xml.DocumentElement != null )
			{
				XmlNodeList nlDataFields = xml.DocumentElement.SelectNodes("DataSets/DataSet/Fields/Field/DataField");
				foreach ( XmlNode xDataField in nlDataFields )
				{
					if ( sb.Length > 0 )
						sb.Append(", ");
					sb.Append("'");
					sb.Append(xDataField.InnerText);
					sb.Append("'");
				}
			}
			return sb.ToString();
		}
		*/
	}
}
