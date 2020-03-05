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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.EditCustomFields
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected Label                      lblError           ;
		protected Guid                       gID                ;
		protected TableRow                   trDROPDOWN_LIST    ;
		protected DataView                   vwDROPDOWN_LIST    ;
		protected DataView                   vwPICK_LIST_VALUES ;
		protected DataGrid                   grdPICK_LIST_VALUES;
		protected TextBox                    txtNAME            ;
		protected TextBox                    txtLABEL           ;
		protected DropDownList               lstDATA_TYPE       ;
		protected TableRow                   trMAX_SIZE         ;
		protected TextBox                    txtMAX_SIZE        ;
		protected CheckBox                   chkREQUIRED        ;
		protected TextBox                    txtDEFAULT_VALUE   ;
		protected CheckBox                   chkAUDITED         ;
		protected DropDownList               lstDROPDOWN_LIST   ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			string sMODULE_NAME = Sql.ToString(ViewState["MODULE_NAME"]);
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					try
					{
						// 04/24/2006 Paul.  Upgrade to SugarCRM 4.2 Schema. 
						// 04/24/2006 Paul.  We dont support MassUpdate at this time. 

						// 07/18/2006 Paul.  Manually create the command so that we can increase the timeout. 
						// 07/18/2006 Paul.  Keep the original procedure call so that we will get a compiler error if something changes. 
						bool bIncreaseTimeout = true;
						if ( !bIncreaseTimeout )
						{
							SqlProcs.spFIELDS_META_DATA_Update(gID, Sql.ToInteger(txtMAX_SIZE.Text), chkREQUIRED.Checked, chkAUDITED.Checked, txtDEFAULT_VALUE.Text, lstDROPDOWN_LIST.SelectedValue, false);
						}
						else
						{
							Int32  nMAX_SIZE      = Sql.ToInteger(txtMAX_SIZE.Text);
							bool   bREQUIRED      = chkREQUIRED.Checked            ;
							bool   bAUDITED       = chkAUDITED.Checked             ;
							string sDEFAULT_VALUE = txtDEFAULT_VALUE.Text          ;
							string sDROPDOWN_LIST = lstDROPDOWN_LIST.SelectedValue ;
							bool   bMASS_UPDATE   = false                          ;
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								using ( IDbTransaction trn = con.BeginTransaction() )
								{
									try
									{
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.Transaction = trn;
											cmd.CommandType = CommandType.StoredProcedure;
											cmd.CommandText = "spFIELDS_META_DATA_Update";
											// 01/06/2006 Paul.  Tripple the default timeout.  The operation was timing-out on QA machines and on the demo server. 
											// 02/03/2007 Paul.  Increase timeout to 5 minutes.  It should not take that long, but some users are reporting a timeout. 
											cmd.CommandTimeout = 5*60;
											IDbDataParameter parID               = Sql.AddParameter(cmd, "@ID"              , gID                );
											IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID",  Security.USER_ID  );
											IDbDataParameter parMAX_SIZE         = Sql.AddParameter(cmd, "@MAX_SIZE"        , nMAX_SIZE          );
											IDbDataParameter parREQUIRED         = Sql.AddParameter(cmd, "@REQUIRED"        , bREQUIRED          );
											IDbDataParameter parAUDITED          = Sql.AddParameter(cmd, "@AUDITED"         , bAUDITED           );
											IDbDataParameter parDEFAULT_VALUE    = Sql.AddParameter(cmd, "@DEFAULT_VALUE"   , sDEFAULT_VALUE     , 255);
											// 01/10/2007 Paul.  DROPDOWN_LIST was added as it can be modified. 
											IDbDataParameter parDROPDOWN_LIST    = Sql.AddParameter(cmd, "@DROPDOWN_LIST"   , sDROPDOWN_LIST     ,  50);
											IDbDataParameter parMASS_UPDATE      = Sql.AddParameter(cmd, "@MASS_UPDATE"     , bMASS_UPDATE       );
											cmd.ExecuteNonQuery();
										}
										trn.Commit();
									}
									catch(Exception ex)
									{
										trn.Rollback();
										throw(new Exception(ex.Message, ex.InnerException));
									}
								}
							}
						}
						// 01/10/2006 Paul.  Clear the cache. 
						SplendidCache.ClearFieldsMetaData(sMODULE_NAME);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
						return;
					}
					Response.Redirect("default.aspx?MODULE_NAME=" + sMODULE_NAME);
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("default.aspx?MODULE_NAME=" + sMODULE_NAME);
			}
		}

		protected void lstDROPDOWN_LIST_Changed(Object sender, EventArgs e)
		{
			try
			{
				if ( trDROPDOWN_LIST.Visible )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                         " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY                " + ControlChars.CrLf
						     + " where lower(LIST_NAME) = @LIST_NAME" + ControlChars.CrLf
						     + "   and lower(LANG     ) = @LANG     " + ControlChars.CrLf
						     + " order by LIST_ORDER                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@LIST_NAME", lstDROPDOWN_LIST.SelectedValue.ToLower());
							Sql.AddParameter(cmd, "@LANG"     , L10n.NAME.ToLower());
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								DataTable dt = new DataTable();
								da.Fill(dt);
								vwPICK_LIST_VALUES = dt.DefaultView;
								grdPICK_LIST_VALUES.DataSource = vwPICK_LIST_VALUES ;
								grdPICK_LIST_VALUES.DataBind();
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

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList.EditCustomFields"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();

				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					// 01/10/2007 Paul.  Use cached data to populate the pick list. 
					lstDROPDOWN_LIST.DataSource = SplendidCache.TerminologyPickLists();
					lstDROPDOWN_LIST.DataBind();
					lstDROPDOWN_LIST_Changed(null, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                      " + ControlChars.CrLf
							     + "  from vwFIELDS_META_DATA_Edit" + ControlChars.CrLf
							     + " where ID = @ID               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ViewState["MODULE_NAME"] = Sql.ToString(rdr["CUSTOM_MODULE"   ]);
										txtNAME         .Text    = Sql.ToString(rdr["NAME"            ]);
										SetPageTitle(L10n.Term("EditCustomFields.LBL_MODULE_NAME") + " - " + txtNAME.Text);
										txtLABEL        .Text    = Sql.ToString (rdr["LABEL"          ]);
										txtMAX_SIZE     .Text    = Sql.ToString (rdr["MAX_SIZE"       ]);
										txtDEFAULT_VALUE.Text    = Sql.ToString (rdr["DEFAULT_VALUE"  ]);
										chkAUDITED      .Checked = Sql.ToBoolean(rdr["AUDITED"        ]);
										chkREQUIRED     .Checked = (Sql.ToString(rdr["REQUIRED_OPTION"]) == "required") ? true : false;
										try
										{
											lstDATA_TYPE.SelectedValue = Sql.ToString(rdr["DATA_TYPE"]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
										}
										try
										{
											if ( lstDATA_TYPE.SelectedValue == "enum" )
												lstDROPDOWN_LIST.SelectedValue = Sql.ToString(rdr["EXT1"]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
										}
										trDROPDOWN_LIST.Visible = (lstDATA_TYPE.SelectedValue == "enum"   );
										trMAX_SIZE      .Visible = (lstDATA_TYPE.SelectedValue == "varchar");
										lstDROPDOWN_LIST_Changed(null, null);
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
