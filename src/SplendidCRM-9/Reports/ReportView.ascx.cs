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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using Microsoft.Reporting.WebForms;

namespace SplendidCRM.Reports
{
	/// <summary>
	///		Summary description for ReportView.
	/// </summary>
	public class ReportView : SplendidControl
	{
		protected Label        lblError ;
		protected ReportViewer rdlViewer;
		protected string       sReportSQL;
		protected HtmlGenericControl divReportView;

		public void ClearReport()
		{
			// 07/09/2006 Paul.  There is no way to to reset the ReportViewer. 
			// All we can do is hide the division. 
			divReportView.Visible = false;
		}

		public string ReportSQL
		{
			get { return sReportSQL; }
		}

		public void RunReport(string sRDL)
		{
			try
			{
				RdlDocument rdl = new RdlDocument();
				rdl.LoadRdl(sRDL);
				XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
				foreach ( XmlNode xReportParameter in nlReportParameters )
				{
					string sName = xReportParameter.Attributes.GetNamedItem("Name").Value;
					string sValue = Sql.ToString(Request[sName]);
					rdl.SetSingleNode(xReportParameter, "DefaultValue/Values/Value", sValue);
				}
				
				rdlViewer.ProcessingMode = ProcessingMode.Local;
				// 06/25/2006 Paul.  The data sources need to be cleared, otherwise the report will not refresh. 
				rdlViewer.LocalReport.DataSources.Clear();
				rdlViewer.LocalReport.DisplayName = rdl.SelectNodeAttribute(String.Empty, "Name");
				if ( Sql.IsEmptyString(rdlViewer.LocalReport.DisplayName) )
					rdlViewer.LocalReport.DisplayName = "Report";

				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					XmlNodeList nlDataSets = rdl.SelectNodesNS("DataSets/DataSet");
					foreach ( XmlNode xDataSet in nlDataSets )
					{
						DataTable dtReport = new DataTable();
						string sDataSetName = xDataSet.Attributes.GetNamedItem("Name").Value;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							rdl.BuildCommand(xDataSet, cmd);
							sReportSQL = Sql.ExpandParameters(cmd);

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								( (IDbDataAdapter) da ).SelectCommand = cmd;
								{
									da.Fill(dtReport);

									// 07/12/2006 Paul.  Every date cell needs to be localized. 
									foreach ( DataRow row in dtReport.Rows )
									{
										foreach ( DataColumn col in dtReport.Columns )
										{
											if ( col.DataType == typeof(System.DateTime) )
											{
												// 07/13/2006 Paul.  Don't try and translate a NULL. 
												if ( row[col.Ordinal] != DBNull.Value )
													row[col.Ordinal] = T10n.FromServerTime(row[col.Ordinal]);
											}
										}
									}
								}
							}
						}

						ReportDataSource rds = new ReportDataSource(sDataSetName, dtReport);
						rdlViewer.LocalReport.DataSources.Add(rds);
					}
				}
				/*
				http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=444154&SiteID=1
				Brian Hartman - MSFT  06 Jun 2006, 10:22 PM UTC
				LocalReport has a limitation that the report definition can't be changed once the report has been processed.  
				In winforms, you can use the ReportViewer.Reset() method to force the viewer to create a new instance of 
				LocalReport and workaround this issue.  But this method is currently not on the webforms version of report viewer.  
				We hope to add it in an upcoming service pack, but for now, 
				you must workaround this issue by creating a new instance of the ReportViewer. 
				*/
				/*
				// 07/09/2006 Paul.  Creating a new viewer solves the reset problem, but breaks ReportViewer pagination. 
				rdlViewer = new ReportViewer();
				rdlViewer.ID                  = "rdlViewer";
				rdlViewer.Font.Names          = new string[] { "Verdana" };
				rdlViewer.Font.Size           = new FontUnit("8pt");
				rdlViewer.Height              = new Unit("100%");
				rdlViewer.Width               = new Unit("100%");
				rdlViewer.AsyncRendering      = false;
				rdlViewer.SizeToReportContent = true;
				divReportView.Controls.Clear();
				divReportView.Controls.Add(rdlViewer);
				*/

				// 07/13/2006 Paul.  The ReportViewer is having a problem interpreting the date functions. 
				// To solve the problem, we should go through all the parameters and replace the date functions with values. 
				rdl.ReportViewerFixups();
				StringReader sr = new StringReader(rdl.OuterXml);
				rdlViewer.LocalReport.LoadReportDefinition(sr);
				// 06/25/2006 Paul.  Refresh did not work, clear the data sources instead. 
				//rdlViewer.LocalReport.Refresh();
				rdlViewer.DataBind();
			}
			catch ( Exception ex )
			{
				lblError.Text = Utils.ExpandException(ex);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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
