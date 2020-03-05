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
//using Microsoft.VisualBasic;
using SplendidCRM._controls;

namespace SplendidCRM.Administration.DynamicLayout.GridViews
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : DynamicLayoutView
	{
		protected NewRecord ctlNewRecord ;

		protected override string LayoutTableName()
		{
			return "GRIDVIEWS_FIELDS";
		}

		protected override string LayoutIndexName()
		{
			return "COLUMN_INDEX";
		}

		protected override string LayoutTypeName()
		{
			return "COLUMN_TYPE";
		}

		protected override string LayoutUpdateProcedure()
		{
			return "spGRIDVIEWS_COLUMNS_Update";
		}

		protected override string LayoutDeleteProcedure()
		{
			return "spGRIDVIEWS_COLUMNS_Delete";
		}

		protected override void GetLayoutFields(string sNAME)
		{
			dtFields = SplendidCache.GridViewColumns(sNAME).Copy();
		}

		protected override void GetModuleName(string sNAME, ref string sMODULE_NAME, ref string sVIEW_NAME)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select MODULE_NAME  " + ControlChars.CrLf
					     + "     , VIEW_NAME    " + ControlChars.CrLf
					     + "  from vwGRIDVIEWS  " + ControlChars.CrLf
					     + " where NAME = @NAME " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", sNAME);
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
								sVIEW_NAME   = Sql.ToString(rdr["VIEW_NAME"  ]);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlLayoutButtons.ErrorText = ex.Message;
			}
		}

		protected override void ClearCache(string sNAME)
		{
			SplendidCache.ClearGridView(sNAME);
		}

		protected override void LayoutView_Bind()
		{
			if ( dtFields != null )
			{
				tblMain.Rows.Clear();
				DataView dv = dtFields.DefaultView;
				dv.RowFilter = "DELETED = 0";
				dv.Sort      = LayoutIndexName();
				SplendidDynamic.AppendGridColumns(dv, tblMain, null, GetL10n(), GetT10n(), new CommandEventHandler(Page_Command));
			}
		}

		protected override void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Layout.Edit" )
				{
					if ( ctlNewRecord != null )
					{
						ctlNewRecord.Clear();
						int nFieldIndex = Sql.ToInteger(e.CommandArgument);
						DataView vwFields = new DataView(dtFields);
						vwFields.RowFilter = "DELETED = 0 and " + LayoutIndexName() + " = " + nFieldIndex.ToString();
						if ( vwFields.Count == 1 )
						{
							foreach(DataRowView row in vwFields)
							{
								ctlNewRecord.FIELD_ID                   = Sql.ToGuid   (row["ID"                        ]);
								ctlNewRecord.FIELD_INDEX                = Sql.ToInteger(row[LayoutIndexName()           ]);
								ctlNewRecord.FIELD_TYPE                 = Sql.ToString (row[LayoutTypeName()            ]);
								ctlNewRecord.DATA_FORMAT                = Sql.ToString (row["DATA_FORMAT"               ]);
								ctlNewRecord.DATA_LABEL                 = Sql.ToString (row["HEADER_TEXT"               ]);
								ctlNewRecord.DATA_FIELD                 = Sql.ToString (row["DATA_FIELD"                ]);
								ctlNewRecord.SORT_EXPRESSION            = Sql.ToString (row["SORT_EXPRESSION"           ]);
								ctlNewRecord.ITEMSTYLE_WIDTH            = Sql.ToString (row["ITEMSTYLE_WIDTH"           ]);
								ctlNewRecord.ITEMSTYLE_CSSCLASS         = Sql.ToString (row["ITEMSTYLE_CSSCLASS"        ]);
								ctlNewRecord.ITEMSTYLE_HORIZONTAL_ALIGN = Sql.ToString (row["ITEMSTYLE_HORIZONTAL_ALIGN"]);
								ctlNewRecord.ITEMSTYLE_VERTICAL_ALIGN   = Sql.ToString (row["ITEMSTYLE_VERTICAL_ALIGN"  ]);
								ctlNewRecord.ITEMSTYLE_WRAP             = Sql.ToBoolean(row["ITEMSTYLE_WRAP"            ]);
								ctlNewRecord.URL_FIELD                  = Sql.ToString (row["URL_FIELD"                 ]);
								ctlNewRecord.URL_FORMAT                 = Sql.ToString (row["URL_FORMAT"                ]);
								ctlNewRecord.URL_TARGET                 = Sql.ToString (row["URL_TARGET"                ]);
								ctlNewRecord.URL_MODULE                 = Sql.ToString (row["URL_MODULE"                ]);
								ctlNewRecord.URL_ASSIGNED_FIELD         = Sql.ToString (row["URL_ASSIGNED_FIELD"        ]);
								ctlNewRecord.LIST_NAME                  = Sql.ToString (row["LIST_NAME"                 ]);
								ctlNewRecord.Visible = true;
								break;
							}
						}
					}
				}
				else if ( e.CommandName == "NewRecord.Save" )
				{
					if ( ctlNewRecord != null )
					{
						DataView vwFields = new DataView(dtFields);
						vwFields.RowFilter = "DELETED = 0 and ID = '" + ctlNewRecord.FIELD_ID + "'";
						if ( vwFields.Count == 1 )
						{
							// 01/09/2006 Paul.  Make sure to use ToDBString to convert empty stings to NULL. 
							foreach(DataRowView row in vwFields)
							{
								row[LayoutTypeName()            ] = Sql.ToDBString (ctlNewRecord.FIELD_TYPE                );
								row["DATA_FORMAT"               ] = Sql.ToDBString (ctlNewRecord.DATA_FORMAT               );
								row["HEADER_TEXT"               ] = Sql.ToDBString (ctlNewRecord.DATA_LABEL                );
								row["DATA_FIELD"                ] = Sql.ToDBString (ctlNewRecord.DATA_FIELD                );
								row["SORT_EXPRESSION"           ] = Sql.ToDBString (ctlNewRecord.SORT_EXPRESSION           );
								row["ITEMSTYLE_WIDTH"           ] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_WIDTH           );
								row["ITEMSTYLE_CSSCLASS"        ] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_CSSCLASS        );
								row["ITEMSTYLE_HORIZONTAL_ALIGN"] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_HORIZONTAL_ALIGN);
								row["ITEMSTYLE_VERTICAL_ALIGN"  ] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_VERTICAL_ALIGN  );
								row["ITEMSTYLE_WRAP"            ] = Sql.ToDBBoolean(ctlNewRecord.ITEMSTYLE_WRAP            );
								row["URL_FIELD"                 ] = Sql.ToDBString (ctlNewRecord.URL_FIELD                 );
								row["URL_FORMAT"                ] = Sql.ToDBString (ctlNewRecord.URL_FORMAT                );
								row["URL_TARGET"                ] = Sql.ToDBString (ctlNewRecord.URL_TARGET                );
								row["URL_MODULE"                ] = Sql.ToDBString (ctlNewRecord.URL_MODULE                );
								row["URL_ASSIGNED_FIELD"        ] = Sql.ToDBString (ctlNewRecord.URL_ASSIGNED_FIELD        );
								row["LIST_NAME"                 ] = Sql.ToDBString (ctlNewRecord.LIST_NAME                 );
								break;
							}
						}
						else
						{
							// 01/08/2006 Paul.  If not found, then insert a new field. 
							if ( ctlNewRecord.FIELD_INDEX == -1 )
							{
								ctlNewRecord.FIELD_INDEX = DynamicTableNewFieldIndex();
							}
							else
							{
								// Make room for the new record. 
								DynamicTableInsert(ctlNewRecord.FIELD_INDEX);
							}
							// 01/09/2006 Paul.  Make sure to use ToDBString to convert empty stings to NULL. 
							DataRow row = dtFields.NewRow();
							dtFields.Rows.Add(row);
							row["ID"                        ] = Guid.NewGuid();
							row["DELETED"                   ] = 0;
							row["GRID_NAME"                 ] = Sql.ToDBString (ViewState["LAYOUT_VIEW_NAME"]);
							row[LayoutIndexName()           ] = Sql.ToDBInteger(ctlNewRecord.FIELD_INDEX               );
							row[LayoutTypeName()            ] = Sql.ToDBString (ctlNewRecord.FIELD_TYPE                );
							row["DATA_FORMAT"               ] = Sql.ToDBString (ctlNewRecord.DATA_FORMAT               );
							row["HEADER_TEXT"               ] = Sql.ToDBString (ctlNewRecord.DATA_LABEL                );
							row["DATA_FIELD"                ] = Sql.ToDBString (ctlNewRecord.DATA_FIELD                );
							row["SORT_EXPRESSION"           ] = Sql.ToDBString (ctlNewRecord.SORT_EXPRESSION           );
							row["ITEMSTYLE_WIDTH"           ] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_WIDTH           );
							row["ITEMSTYLE_CSSCLASS"        ] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_CSSCLASS        );
							row["ITEMSTYLE_HORIZONTAL_ALIGN"] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_HORIZONTAL_ALIGN);
							row["ITEMSTYLE_VERTICAL_ALIGN"  ] = Sql.ToDBString (ctlNewRecord.ITEMSTYLE_VERTICAL_ALIGN  );
							row["ITEMSTYLE_WRAP"            ] = Sql.ToDBBoolean(ctlNewRecord.ITEMSTYLE_WRAP            );
							row["URL_FIELD"                 ] = Sql.ToDBString (ctlNewRecord.URL_FIELD                 );
							row["URL_FORMAT"                ] = Sql.ToDBString (ctlNewRecord.URL_FORMAT                );
							row["URL_TARGET"                ] = Sql.ToDBString (ctlNewRecord.URL_TARGET                );
							row["URL_MODULE"                ] = Sql.ToDBString (ctlNewRecord.URL_MODULE                );
							row["URL_ASSIGNED_FIELD"        ] = Sql.ToDBString (ctlNewRecord.URL_ASSIGNED_FIELD        );
							row["LIST_NAME"                 ] = Sql.ToDBString (ctlNewRecord.LIST_NAME                 );
						}
						SaveFieldState();
						LayoutView_Bind();
						if ( ctlNewRecord != null )
							ctlNewRecord.Clear();
					}
				}
				else if ( e.CommandName == "Defaults" )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                         " + ControlChars.CrLf
						     + "  from vwGRIDVIEWS_COLUMNS       " + ControlChars.CrLf
						     + " where GRID_NAME = @GRID_NAME    " + ControlChars.CrLf
						     + "   and DEFAULT_VIEW = 1          " + ControlChars.CrLf
						     + " order by " + LayoutIndexName() + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@GRID_NAME", Sql.ToString(ViewState["LAYOUT_VIEW_NAME"]));
						
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								//dtFields = new DataTable();
								// 01/09/2006 Paul.  Mark existing records for deletion. 
								// This is so that the save operation can update only records that have changed. 
								foreach(DataRow row in dtFields.Rows)
									row["DELETED"] = 1;
								da.Fill(dtFields);
								// 01/09/2006 Paul.  We need to change the IDs for two reasons, one is to prevent updating the Default Values,
								// the second reason is that we need the row to get a Modified state.  Otherwise the update loop will skip it. 
								foreach(DataRow row in dtFields.Rows)
								{
									if ( Sql.ToInteger(row["DELETED"]) == 0 )
										row["ID"] = Guid.NewGuid();
								}
							}
						}
					}
					SaveFieldState();
					LayoutView_Bind();
					
					if ( ctlNewRecord != null )
						ctlNewRecord.Clear();
				}
				else
				{
					base.Page_Command(sender, e);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlLayoutButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("DynamicLayout.LBL_EDIT_VIEW_LAYOUT"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			// 02/08/2007 Paul.  The NewRecord control is now in the MasterPage. 
			ContentPlaceHolder plcSidebar = Page.Master.FindControl("cntSidebar") as ContentPlaceHolder;
			if ( plcSidebar != null )
			{
				ctlNewRecord = plcSidebar.FindControl("ctlNewRecord") as NewRecord;
				if ( ctlNewRecord != null )
				{
					ctlNewRecord.Command += new CommandEventHandler(Page_Command);
				}
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
			ctlLayoutButtons.Command += new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
