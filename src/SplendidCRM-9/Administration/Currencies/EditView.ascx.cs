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
using System.Globalization;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Currencies
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected Label           lblError                     ;
		protected Guid            gID                          ;
		protected TextBox         txtNAME                      ;
		protected TextBox         txtSYMBOL                    ;
		protected TextBox         txtISO4217                   ;
		protected TextBox         txtCONVERSION_RATE           ;
		protected DropDownList    lstSTATUS                    ;
		protected RequiredFieldValidator reqNAME           ;
		protected RequiredFieldValidator reqSYMBOL         ;
		protected RequiredFieldValidator reqISO4217        ;
		protected RequiredFieldValidator reqCONVERSION_RATE;

		protected _controls.ListHeader ctlListHeader ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "CURRENCIES";
					DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								float dCONVERSION_RATE = float.Parse(txtCONVERSION_RATE.Text, NumberStyles.AllowDecimalPoint);
								// 05/10/2006 Paul.  Sql.ToFloat() was having a problem when an alternate decimal point is used. 
								SqlProcs.spCURRENCIES_Update
									( ref gID
									, txtNAME.Text
									, txtSYMBOL.Text
									, txtISO4217.Text
									, dCONVERSION_RATE
									, lstSTATUS.SelectedValue
									, trn
									);
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								trn.Commit();
								// 04/20/2006 Paul.  Make sure to clear the cache. 
								Cache.Remove("vwCURRENCIES_LISTBOX");
								// 07/07/2007 Paul.  The currency in the application scope needs to be updated. 
								Currency C10n = new Currency
									( gID
									, txtNAME.Text
									, txtSYMBOL.Text
									, dCONVERSION_RATE
									);
								Application["CURRENCY." + C10n.ID.ToString()] = C10n;
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								lblError.Text = ex.Message;
								return;
							}
						}
					}
					Response.Redirect("default.aspx");
				}
			}
			else if ( e.CommandName == "Clear" )
			{
				Response.Redirect("default.aspx");
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Currencies.LBL_CURRENCY"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();  // 09/03/2005 Paul. DataBind is required in order for the RequiredFieldValidators to work. 
				// 07/02/2006 Paul.  The required fields need to be bound manually. 
				reqNAME.DataBind();
				reqSYMBOL.DataBind();
				reqISO4217.DataBind();
				reqCONVERSION_RATE.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					lstSTATUS.DataSource = SplendidCache.List("Currencies", "currency_status_dom");
					lstSTATUS.DataBind();
					
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                " + ControlChars.CrLf
							     + "  from vwCURRENCIES_Edit" + ControlChars.CrLf
							     + " where ID = @ID         " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										txtNAME          .Text  = Sql.ToString(rdr["NAME"         ]);
										ctlListHeader    .Title = L10n.Term("Currencies.LBL_CURRENCY") + ": " + txtNAME.Text;
										SetPageTitle(L10n.Term("Currencies.LBL_MODULE_NAME") + " - " + txtNAME.Text);
										txtSYMBOL.Text          = Sql.ToString(rdr["SYMBOL"         ]);
										txtISO4217.Text         = Sql.ToString(rdr["ISO4217"        ]);
										txtCONVERSION_RATE.Text = Sql.ToString(rdr["CONVERSION_RATE"]);
										try
										{
											lstSTATUS.SelectedValue = Sql.ToString(rdr["STATUS"]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
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
