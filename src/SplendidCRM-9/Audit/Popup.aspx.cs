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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Audit
{
	/// <summary>
	/// Summary description for Popup.
	/// </summary>
	public class Popup : SplendidPage
	{
		protected Guid          gID            ;
		protected string        sModule        ;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblTitle       ;
		protected Label         lblError       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private DataTable BuildChangesTable(DataTable dtAudit)
		{
			DataTable dtChanges = new DataTable();
			DataColumn colFIELD_NAME   = new DataColumn("FIELD_NAME"  , typeof(System.String  ));
			DataColumn colBEFORE_VALUE = new DataColumn("BEFORE_VALUE", typeof(System.String  ));
			DataColumn colAFTER_VALUE  = new DataColumn("AFTER_VALUE" , typeof(System.String  ));
			DataColumn colCREATED_BY   = new DataColumn("CREATED_BY"  , typeof(System.String  ));
			DataColumn colDATE_CREATED = new DataColumn("DATE_CREATED", typeof(System.DateTime));
			dtChanges.Columns.Add(colFIELD_NAME  );
			dtChanges.Columns.Add(colBEFORE_VALUE);
			dtChanges.Columns.Add(colAFTER_VALUE );
			dtChanges.Columns.Add(colCREATED_BY  );
			dtChanges.Columns.Add(colDATE_CREATED);
			if ( dtAudit.Rows.Count > 0 )
			{
				StringDictionary dict = new StringDictionary();
				dict.Add("AUDIT_ACTION"      , String.Empty);
				dict.Add("AUDIT_DATE"        , String.Empty);
				dict.Add("AUDIT_COLUMNS"     , String.Empty);
				dict.Add("CSTM_AUDIT_COLUMNS", String.Empty);
				dict.Add("ID"                , String.Empty);
				dict.Add("ID_C"              , String.Empty);
				dict.Add("DELETED"           , String.Empty);
				dict.Add("CREATED_BY"        , String.Empty);
				dict.Add("DATE_ENTERED"      , String.Empty);
				dict.Add("MODIFIED_USER_ID"  , String.Empty);
				dict.Add("DATE_MODIFIED"     , String.Empty);

				DataRow rowLast = dtAudit.Rows[0];
				for ( int i = 1; i < dtAudit.Rows.Count; i++ )
				{
					DataRow row = dtAudit.Rows[i];
					foreach ( DataColumn col in row.Table.Columns )
					{
						if ( !dict.ContainsKey(col.ColumnName) )
						{
							if ( Sql.ToString(rowLast[col.ColumnName]) != Sql.ToString(row[col.ColumnName]) )
							{
								DataRow rowChange = dtChanges.NewRow();
								dtChanges.Rows.Add(rowChange);
								rowChange["FIELD_NAME"  ] = col.ColumnName;
								rowChange["CREATED_BY"  ] = SplendidCache.AssignedUser(Sql.ToGuid(row["MODIFIED_USER_ID"]));
								rowChange["DATE_CREATED"] = row["AUDIT_DATE"];
								rowChange["BEFORE_VALUE"] = rowLast[col.ColumnName];
								rowChange["AFTER_VALUE" ] = row    [col.ColumnName];

							}
						}
					}
					rowLast = row;
				}
			}
			return dtChanges;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				gID     = Sql.ToGuid  (Request["ID"    ]);
				sModule = Sql.ToString(Request["Module"]);
				string sTableName = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
				if ( !Sql.IsEmptyGuid(gID) && !Sql.IsEmptyString(sModule) && !Sql.IsEmptyString(sTableName) )
				{
					// 12/30/2007 Paul.  The first query should be used just to determine if access is allowed. 
					bool bAccessAllowed = false;
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vw" + sTableName + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sModule, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									bAccessAllowed = true;
									string sNAME = String.Empty;
									try
									{
										// 12/30/2007 Paul.  The name field might not be called NAME.
										// For now, just ignore the issue. 
										sNAME = Sql.ToString(rdr["NAME"]);
									}
									catch
									{
									}
									lblTitle.Text = L10n.Term(".moduleList." + sModule) + ": " + sNAME;
									SetPageTitle(L10n.Term(".moduleList." + sModule) + " - " + sNAME);
								}
							}
						}
						if ( bAccessAllowed )
						{
							StringBuilder sb = new StringBuilder();
							DataTable dtTableColumns  = new DataTable();
							DataTable dtCustomColumns = new DataTable();
							sSQL = "select ColumnName              " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @ObjectName" + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ObjectName", sTableName);
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtTableColumns);
								}
							}
							sSQL = "select ColumnName              " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @ObjectName" + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ObjectName", sTableName + "_CSTM");
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtCustomColumns);
								}
							}
							string sAuditName       = sTableName + "_AUDIT";
							string sCustomAuditName = sTableName + "_CSTM_AUDIT";
							sb.Append("select " + sAuditName       + ".AUDIT_ACTION  as AUDIT_ACTION      " + ControlChars.CrLf);
							sb.Append("     , " + sAuditName       + ".AUDIT_DATE    as AUDIT_DATE        " + ControlChars.CrLf);
							sb.Append("     , " + sAuditName       + ".AUDIT_COLUMNS as AUDIT_COLUMNS     " + ControlChars.CrLf);
							sb.Append("     , " + sCustomAuditName + ".AUDIT_COLUMNS as CSTM_AUDIT_COLUMNS" + ControlChars.CrLf);
							foreach ( DataRow row in dtTableColumns.Rows )
							{
								sb.Append("     , " + sAuditName + "." + Sql.ToString(row["ColumnName"]) + ControlChars.CrLf);
							}
							foreach ( DataRow row in dtCustomColumns.Rows )
							{
								sb.Append("     , " + sCustomAuditName + "." + Sql.ToString(row["ColumnName"]) + ControlChars.CrLf);
							}
							sb.Append("  from            " + sAuditName + ControlChars.CrLf);
							sb.Append("  left outer join " + sCustomAuditName + ControlChars.CrLf);
							sb.Append("               on " + sCustomAuditName + ".ID_C        = " + sAuditName + ".ID         " + ControlChars.CrLf);
							sb.Append("              and " + sCustomAuditName + ".AUDIT_TOKEN = " + sAuditName + ".AUDIT_TOKEN" + ControlChars.CrLf);
							sb.Append(" where " + sAuditName + ".ID = @ID" + ControlChars.CrLf);
							sb.Append(" order by " + sAuditName + ".AUDIT_VERSION asc" + ControlChars.CrLf);
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sb.ToString();
								Sql.AddParameter(cmd, "@ID", gID);

								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "SQLCode", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										DataTable dtChanges = BuildChangesTable(dt);
										vwMain = new DataView(dtChanges);
										vwMain.Sort = "DATE_CREATED desc, FIELD_NAME asc";
										grdMain.DataSource = vwMain ;
										if ( !IsPostBack )
										{
											grdMain.DataBind();
										}
									}
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
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
				Page.DataBind();
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
