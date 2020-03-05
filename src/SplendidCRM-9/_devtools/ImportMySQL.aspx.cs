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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for ImportMySQL.
	/// </summary>
	public class ImportMySQL : System.Web.UI.Page
	{
		protected HtmlInputFile fileUNC;

		protected void Page_ItemCommand(Object sender, CommandEventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			// 12/22/2007 Paul.  Allow an admin to import data. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;
			if ( e.CommandName == "Upload" )
			{
				if ( fileUNC.PostedFile == null || Sql.IsEmptyString(fileUNC.PostedFile.FileName) )
				{
					throw(new Exception("File was not provided"));
				}
			
				Response.ContentType = "Text/SQL";
				Response.AddHeader("Content-Disposition", "attachment;filename=ImportMySQL.sql");
				Response.Write("set nocount on" + ControlChars.CrLf);
				Response.Write("GO" + ControlChars.CrLf);
				
				HttpPostedFile pstFile  = fileUNC.PostedFile;
				XmlDocument xml = new XmlDocument();
				xml.Load(pstFile.FileName);
				foreach(XmlNode node in xml.DocumentElement.ChildNodes)
				{
					if ( node.NodeType == XmlNodeType.Element )
					{
						string sTableName = node.Name.ToUpper();
						StringBuilder  sbUpdate       = new StringBuilder();
						StringBuilder  sbInsertColumn = new StringBuilder();
						StringBuilder  sbInsertValues = new StringBuilder();
						sbUpdate.Append("	update " + sTableName + ControlChars.CrLf);
						sbInsertColumn.Append("	insert into " + sTableName);
						sbInsertValues.Append("	            " + Strings.Space(Math.Max(0, node.Name.Length - 6)) + "values");
						string sColumnLeader = "(";
						string sUpdateLeader = "	   set ";
						string sPrimaryKeyName  = String.Empty;
						string sPrimaryKeyValue = String.Empty;
						foreach(XmlNode nodeColumn in node.ChildNodes)
						{
							if ( node.NodeType == XmlNodeType.Element )
							{
								string sColumnName  = nodeColumn.Name.ToUpper();
								string sColumnValue = nodeColumn.InnerText.Replace("'", "''");
								if ( sColumnName == "ROLES_MODULES" && sColumnName == "MODULE_ID" )
									sColumnName = "MODULE";
								sbUpdate.Append(sUpdateLeader);
								sbInsertColumn.Append(sColumnLeader);
								sbInsertColumn.Append(nodeColumn.Name.ToUpper());
								sbInsertValues.Append(sColumnLeader);
								if ( (sColumnName == "ID" || sColumnName.EndsWith("_ID") || sColumnName.EndsWith("_BY")) && (nodeColumn.InnerText.Length > 0 && nodeColumn.InnerText.Length < 12) )
								{
									sColumnValue = "00000000-0000-0000-0000-";  // 42b109076e06
									// 07/31/2006 Paul.  Stop using VisualBasic library to increase compatibility with Mono. 
									sColumnValue += new string('0', 12 - nodeColumn.InnerText.Length);
									sColumnValue += nodeColumn.InnerText;
								}
								if ( sColumnName == "DO_NOT_CALL" || sColumnName == "EMAIL_OPT_OUT" || sColumnName == "DATE_DUE_FLAG" || sColumnName == "DATE_START_FLAG" || sColumnName == "IS_ADMIN" )
								{
									if ( sColumnValue == "off" )
										sColumnValue = "0";
									else if ( sColumnValue == "on" )
										sColumnValue = "1";
								}
								if ( sColumnValue.Length == 0 )
								{
									sbUpdate.Append(sColumnName + " = null");
									sbInsertValues.Append("null");
								}
								else
								{
									sbUpdate.Append(sColumnName + " = ");
									if ( sColumnName.StartsWith("AMOUNT") )
									{
										sbUpdate.Append(sColumnValue);
										sbInsertValues.Append(sColumnValue);
									}
									else
									{
										sbUpdate.Append("'");
										sbUpdate.Append(sColumnValue);
										sbUpdate.Append("'");
										sbInsertValues.Append("'");
										sbInsertValues.Append(sColumnValue);
										sbInsertValues.Append("'");
									}
								}
								if ( sColumnName == "ID" )
								{
									sPrimaryKeyName  = sColumnName ;
									sPrimaryKeyValue = sColumnValue;
								}
								sColumnLeader = ", ";
								sUpdateLeader = ControlChars.CrLf + "	     , ";
							}
						}
						sbUpdate.Append(ControlChars.CrLf + "	 where " + sPrimaryKeyName + " = '" + sPrimaryKeyValue + "'" + ControlChars.CrLf);
						sbInsertColumn.Append(")" + ControlChars.CrLf);
						sbInsertValues.Append(")" + ControlChars.CrLf);
						if ( sPrimaryKeyName == String.Empty )
						{
							// Don't import CONFIG table. 
							//Response.Write(sbInsertColumn.ToString());
							//Response.Write(sbInsertValues.ToString());
						}
						else
						{
							if ( sTableName == "BUGS" || sTableName == "CASES" || sTableName == "CAMPAIGNS" || sTableName == "PROSPECTS" )
								Response.Write("set identity_insert " + sTableName + " on" + ControlChars.CrLf);
							Response.Write("if not exists(select * from " + node.Name.ToUpper() + " where " + sPrimaryKeyName + " = '" + sPrimaryKeyValue + "')" + ControlChars.CrLf);
							Response.Write(sbInsertColumn.ToString());
							Response.Write(sbInsertValues.ToString());
							Response.Write("else" + ControlChars.CrLf);
							Response.Write(sbUpdate.ToString());
							if ( sTableName == "BUGS" || sTableName == "CASES" || sTableName == "CAMPAIGNS" || sTableName == "PROSPECTS" )
								Response.Write("set identity_insert " + sTableName + " off" + ControlChars.CrLf);
							Response.Write("GO" + ControlChars.CrLf);
						}
					}
				}
				Response.End();
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
