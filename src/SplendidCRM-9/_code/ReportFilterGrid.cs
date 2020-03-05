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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Diagnostics;
using System.Xml;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	public class CreateItemTemplateReportFilterList : ITemplate
	{
		protected string       sDATA_FIELD;
		protected DropDownList lst        ;
		protected DataTable    dt         ;
		
		public CreateItemTemplateReportFilterList(string sDATA_FIELD)
		{
			this.sDATA_FIELD = sDATA_FIELD;
		}

		public void InstantiateIn(Control objContainer)
		{
			lst = new DropDownList();
			objContainer.Controls.Add(lst);
			lst.DataBinding += new EventHandler(lst_OnDataBinding);
		}
		private void lst_OnDataBinding(object sender, EventArgs e)
		{
			DataGridItem     objContainer = (DataGridItem) lst.NamingContainer;
			DataRowView      row = objContainer.DataItem as DataRowView;
			ReportFilterGrid grd = objContainer.Parent.Parent as ReportFilterGrid;
			if ( row != null )
			{
				// 04/25/2006 Paul.  We always need to translate the items, even during postback.
				// This is because we always build the DropDownList. 
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					string sID     = Sql.ToString(row["ID"         ]);
					string sMODULE = Sql.ToString(row["MODULE_NAME"]);
					lst.ID = sDATA_FIELD + "_" + sID;
					try
					{
						if ( sDATA_FIELD == "MODULE_NAME" )
						{
							XmlDocument xml = grd.Rdl;
							/*
							// 06/20/2006 Paul.  New RdlDocument handles custom properties. 
							string sRelationships = RdlUtil.GetCustomProperty(xml.DocumentElement, "Relationships");
							if ( !Sql.IsEmptyString(sRelationships) )
							{
								XmlDocument xmlRelationship = new XmlDocument();
								xmlRelationship.LoadXml(sRelationships);
								dt = XmlUtil.CreateDataTable(xmlRelationship.DocumentElement, "Relationship", new string[] {"MODULE_NAME", "DISPLAY_NAME"});
								lst.AutoPostBack = true;
								foreach ( DataRow rowRelationship in dt.Rows )
								{
									lst.Items.Add(new ListItem(Sql.ToString(rowRelationship["DISPLAY_NAME"]), Sql.ToString(rowRelationship["MODULE_NAME"])));
								}
							}
							*/
						}
						else if ( sDATA_FIELD == "DATA_FIELD" )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								string sSQL;
								sSQL = "select ColumnName as NAME              " + ControlChars.CrLf
								     + "     , ColumnName as DISPLAY_NAME      " + ControlChars.CrLf
								     + "  from vwSqlColumns                    " + ControlChars.CrLf
								     + " where ObjectName = @ObjectName        " + ControlChars.CrLf
								     + "   and ColumnName not in ('ID', 'ID_C')" + ControlChars.CrLf
								     + "   and ColumnName not like '%_ID'      " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									DropDownList lstMODULE_NAME = null;
									// 05/28/2006 Paul.  Not sure why, but grd.FindFilterControl() does not work. 
									foreach(DataGridItem itm in objContainer.Parent.Controls)
									{
										lstMODULE_NAME = itm.FindControl("MODULE_NAME" + "_" + sID) as DropDownList;
										if ( lstMODULE_NAME != null )
											break;
									}
									string sMODULE_NAME = lstMODULE_NAME.SelectedValue;
									string[] arrModule = sMODULE_NAME.Split(' ');
									string sModule     = arrModule[0];
									string sTableAlias = arrModule[0];
									if ( arrModule.Length > 1 )
										sTableAlias = arrModule[1].ToUpper();
									Sql.AddParameter(cmd, "@ObjectName", "vw" + sModule);

									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										( (IDbDataAdapter) da ).SelectCommand = cmd;
										dt = new DataTable();
										da.Fill(dt);
										foreach ( DataRow rowColumn in dt.Rows )
										{
											rowColumn["NAME"        ] = sMODULE + "." + Sql.ToString(rowColumn["NAME"]);
											rowColumn["DISPLAY_NAME"] = L10n.Term(sModule + ".LBL_" + Sql.ToString(rowColumn["DISPLAY_NAME"])).Replace(":", "");
										}
										DataView vwColumns = new DataView(dt);
										vwColumns.Sort = "DISPLAY_NAME";
										foreach ( DataRowView rowColumn in vwColumns )
										{
											lst.Items.Add(new ListItem(Sql.ToString(rowColumn["DISPLAY_NAME"]), Sql.ToString(rowColumn["NAME"])));
										}
									}
								}
							}
						}
					}
					catch
					{
					}
					try
					{
						// 04/25/2006 Paul.  Don't update values on postback, otherwise it will over-write modified values. 
						if ( !objContainer.Page.IsPostBack )
						{
							lst.SelectedValue = Sql.ToString(row[sDATA_FIELD]);
						}
					}
					catch
					{
					}
				}
				
				/*
				// 04/25/2006 Paul.  Make sure to translate the text.  
				// It cannot be translated in InstantiateIn() because the Page is not defined. 
				foreach(ListItem itm in lst.Items )
				{
					itm.Text = L10n.Term(itm.Text);
				}
				*/
			}
		}
	}

	public class CreateItemTemplateReportFilterText : ITemplate
	{
		protected string       sDATA_FIELD;
		protected TextBox      txt        ;
		
		public CreateItemTemplateReportFilterText(string sDATA_FIELD)
		{
			this.sDATA_FIELD = sDATA_FIELD;
		}

		public void InstantiateIn(Control objContainer)
		{
			txt = new TextBox();
			objContainer.Controls.Add(txt);
			txt.DataBinding += new EventHandler(txt_OnDataBinding);
		}
		private void txt_OnDataBinding(object sender, EventArgs e)
		{
			DataGridItem     objContainer = (DataGridItem) txt.NamingContainer;
			DataRowView      row = objContainer.DataItem as DataRowView;
			ReportFilterGrid grd = objContainer.Parent.Parent as ReportFilterGrid;
			
			if ( row != null )
			{
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					string sID     = Sql.ToString(row["ID"         ]);
					string sMODULE = Sql.ToString(row["MODULE_NAME"]);
					txt.ID = sDATA_FIELD + "_" + sID;
					try
					{
						// 04/25/2006 Paul.  Don't update values on postback, otherwise it will over-write modified values. 
						if ( !objContainer.Page.IsPostBack )
						{
							txt.Text = Sql.ToString(row[sDATA_FIELD]);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	/// <summary>
	/// Summary description for ACLGrid.
	/// </summary>
	public class ReportFilterGrid : SplendidGrid
	{
		protected XmlDocument xml;

		public ReportFilterGrid()
		{
			this.Init += new EventHandler(OnInit);
		}

		public XmlDocument Rdl
		{
			get { return xml; }
			set { xml = value; }
		}

		public DataBoundControl FindFilterControl(string sID, string sDATA_FIELD)
		{
			DataBoundControl ctl = null;
			foreach(DataGridItem itm in Items)
			{
				ctl = itm.FindControl(sDATA_FIELD + "_" + sID) as DataBoundControl;
				if ( ctl != null )
					break;
			}
			return ctl;
		}

		private void AppendFilterColumn(string sDATA_FIELD, string sTYPE)
		{
			TemplateColumn tpl = new TemplateColumn();
			tpl.ItemStyle.Width             = new Unit("12%");
			//tpl.ItemStyle.CssClass          = "tabDetailViewDF";
			tpl.ItemStyle.HorizontalAlign   = HorizontalAlign.NotSet;
			tpl.ItemStyle.VerticalAlign     = VerticalAlign.NotSet  ;
			tpl.ItemStyle.Wrap              = false;
			tpl.HeaderText = sDATA_FIELD;
			if ( sTYPE == "DropDownList" )
				tpl.ItemTemplate   = new CreateItemTemplateReportFilterList(sDATA_FIELD);
			else if ( sTYPE == "TextBox" )
				tpl.ItemTemplate   = new CreateItemTemplateReportFilterText(sDATA_FIELD);
			this.Columns.Add(tpl);
		}

		protected void OnInit(object sender, System.EventArgs e)
		{
			this.Width = new Unit();
			AppendFilterColumn("MODULE_NAME" , "DropDownList");
			AppendFilterColumn("DATA_FIELD"  , "DropDownList");
			AppendFilterColumn("OPERATOR"    , "DropDownList");
			AppendFilterColumn("SEARCH_FIELD", "TextBox"     );
		}

	}
}

