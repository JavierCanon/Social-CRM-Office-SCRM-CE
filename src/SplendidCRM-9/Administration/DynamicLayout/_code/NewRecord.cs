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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.DynamicLayout
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : SplendidControl
	{
		public CommandEventHandler Command ;

		protected string             sMODULE_NAME      ;
		protected string             sVIEW_NAME        ;

		protected Label              lblError          ;
		protected HiddenField        txtFIELD_ID       ;
		protected Label              txtFIELD_INDEX    ;
		protected DropDownList       lstFIELD_TYPE     ;
		protected TextBox            txtDATA_LABEL     ;
		protected DropDownList       lstDATA_LABEL     ;
		protected TextBox            txtDATA_FIELD     ;
		protected DropDownList       lstDATA_FIELD     ;
		protected DropDownList       lstLIST_NAME      ;
		protected CheckBox           chkFREE_FORM_LABEL;
		protected CheckBox           chkFREE_FORM_DATA ;
		protected DropDownList       lstSORT_EXPRESSION;

		//protected RequiredFieldValidator     reqNAME        ;

		public virtual void Clear()
		{
			txtFIELD_ID   .Value = String.Empty;
			txtFIELD_INDEX.Text  = String.Empty;
			txtDATA_LABEL .Text  = String.Empty;
			txtDATA_FIELD .Text  = String.Empty;
			chkFREE_FORM_LABEL.Checked = false;
			chkFREE_FORM_DATA .Checked = false;
			lstFIELD_TYPE.SelectedIndex = 0;
			lstDATA_LABEL.SelectedIndex = 0;
			lstDATA_FIELD.SelectedIndex = 0;
			lstLIST_NAME .SelectedIndex = 0;
			if ( lstSORT_EXPRESSION != null )
				lstSORT_EXPRESSION.SelectedIndex = 0;
			this.Visible = false;
		}

		public string MODULE_NAME
		{
			get
			{
				return sMODULE_NAME;
			}
			set
			{
				sMODULE_NAME = value;
				lstDATA_LABEL_Bind();
			}
		}

		public string VIEW_NAME
		{
			get
			{
				return sVIEW_NAME;
			}
			set
			{
				sVIEW_NAME = value;
				lstDATA_FIELD_Bind();
			}
		}

		public string FIELD_TYPE
		{
			get
			{
				return lstFIELD_TYPE.SelectedValue;
			}
			set
			{
				try
				{
					lstFIELD_TYPE.SelectedValue = value;
					lstFIELD_TYPE_Changed(null, null);
				}
				catch
				{
				}
			}
		}

		public Guid FIELD_ID
		{
			get { return Sql.ToGuid(txtFIELD_ID.Value); }
			set { txtFIELD_ID.Value = value.ToString(); }
		}

		public int FIELD_INDEX
		{
			get
			{
				if ( txtFIELD_INDEX.Text == String.Empty )
					return -1;
				else
					return Sql.ToInteger(txtFIELD_INDEX.Text);
			}
			set
			{
				if ( value == -1 )
					txtFIELD_INDEX.Text = String.Empty;
				else
					txtFIELD_INDEX.Text = value.ToString();
			}
		}

		public string DATA_LABEL
		{
			get
			{
				if ( chkFREE_FORM_LABEL.Checked )
					return txtDATA_LABEL.Text;
				else
					return lstDATA_LABEL.SelectedValue;
			}
			set
			{
				try
				{
					// 01/10/2006 Paul.  Always try and select the label from the list. 
					txtDATA_LABEL.Text = value;
					lstDATA_LABEL.SelectedValue = value;
					chkFREE_FORM_LABEL.Checked = false;
				}
				catch
				{
					// 01/10/2006 Paul.  If value does not exist, then go to free form. 
					txtDATA_LABEL.Text = value;
					chkFREE_FORM_LABEL.Checked = true;
				}
				chkFREE_FORM_LABEL_CheckedChanged(null, null);
			}
		}

		public string DATA_FIELD
		{
			get
			{
				if ( chkFREE_FORM_DATA.Checked )
					return txtDATA_FIELD.Text;
				else
					return lstDATA_FIELD.SelectedValue;
			}
			set
			{
				if ( value.IndexOf(" ") >= 0 || value.IndexOf(".") >= 0 )
				{
					txtDATA_FIELD.Text = value;
					chkFREE_FORM_DATA.Checked = true;
				}
				else
				{
					try
					{
						txtDATA_FIELD.Text = value;
						lstDATA_FIELD.SelectedValue = value;
						chkFREE_FORM_DATA.Checked = false;
					}
					catch
					{
						txtDATA_FIELD.Text = value;
						chkFREE_FORM_DATA.Checked = true;
					}
				}
				chkFREE_FORM_DATA_CheckedChanged(null, null);
			}
		}

		public string LIST_NAME
		{
			get
			{
				return lstLIST_NAME.SelectedValue;
			}
			set
			{
				try
				{
					lstLIST_NAME.SelectedValue = value;
				}
				catch
				{
				}
			}
		}

		public string SORT_EXPRESSION
		{
			get
			{
				return lstSORT_EXPRESSION.SelectedValue;
			}
			set
			{
				try
				{
					lstSORT_EXPRESSION.SelectedValue = value;
				}
				catch
				{
				}
			}
		}

		protected virtual void lstFIELD_TYPE_Changed(Object sender, EventArgs e)
		{
		}

		protected void chkFREE_FORM_LABEL_CheckedChanged(Object sender, EventArgs e)
		{
			if ( !chkFREE_FORM_LABEL.Checked )
			{
				// 01/10/2006 Paul.  Validate the ability to turn off free form.
				txtDATA_LABEL.Text = txtDATA_LABEL.Text.Trim();
				try
				{
					lstDATA_LABEL.SelectedValue = txtDATA_LABEL.Text;
				}
				catch
				{
					// 01/10/2006 Paul.  If there is an error, then go back to free form. 
					chkFREE_FORM_LABEL.Checked = true;
				}
			}
			txtDATA_LABEL.Visible =  chkFREE_FORM_LABEL.Checked;
			lstDATA_LABEL.Visible = !chkFREE_FORM_LABEL.Checked;
		}

		protected void chkFREE_FORM_DATA_CheckedChanged(Object sender, EventArgs e)
		{
			if ( !chkFREE_FORM_DATA.Checked )
			{
				// 01/10/2006 Paul.  Validate the ability to turn off free form.
				txtDATA_FIELD.Text = txtDATA_FIELD.Text.Trim();
				if ( txtDATA_FIELD.Text.IndexOf(" ") >= 0 || txtDATA_FIELD.Text.IndexOf(".") >= 0 )
				{
					chkFREE_FORM_DATA.Checked = true;
				}
				else
				{
					try
					{
						lstDATA_FIELD.SelectedValue = txtDATA_FIELD.Text;
					}
					catch
					{
						// 01/10/2006 Paul.  If there is an error, then go back to free form. 
						chkFREE_FORM_DATA.Checked = true;
					}
				}
			}
			txtDATA_FIELD.Visible =  chkFREE_FORM_DATA.Checked;
			lstDATA_FIELD.Visible = !chkFREE_FORM_DATA.Checked;
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "NewRecord.Save" || e.CommandName == "NewRecord.Cancel" )
			{
				//reqNAME.Enabled = true;
				//reqNAME.Validate();
				if ( Page.IsValid )
				{
					Guid gID = Guid.Empty;
					try
					{
						if ( Command != null )
							Command(this, e) ;
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
			}
		}

		protected void lstDATA_LABEL_Bind()
		{
			DataTable dt = null;
			if ( !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME != Sql.ToString(ViewState["LAST_MODULE_NAME"]) )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                      " + ControlChars.CrLf
						     + "     , DISPLAY_NAME              " + ControlChars.CrLf
						     + "     , MODULE_NAME               " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY_LayoutLabel " + ControlChars.CrLf
						     + " where MODULE_NAME is null       " + ControlChars.CrLf
						     + "    or MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf
						     + " order by NAME                   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME);
						
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								dt.Rows.InsertAt(dt.NewRow(), 0);
								//ViewState["vwTERMINOLOGY_Labels"] = dt;
								ViewState["LAST_MODULE_NAME"] = sMODULE_NAME;
								lstDATA_LABEL.DataSource = dt;
								lstDATA_LABEL.DataBind();
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
		}

		protected void lstDATA_FIELD_Bind()
		{
			DataTable dt = null;
			if ( !Sql.IsEmptyString(sVIEW_NAME) && sVIEW_NAME != Sql.ToString(ViewState["LAST_VIEW_NAME"]) )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @ObjectName" + ControlChars.CrLf
						     + " order by ColumnName           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ObjectName", sVIEW_NAME);
						
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								dt.Rows.InsertAt(dt.NewRow(), 0);
								ViewState["vwSqlColumns_Fields"] = dt;
								ViewState["LAST_VIEW_NAME"     ] = sVIEW_NAME;
								lstDATA_FIELD.DataSource = dt;
								lstDATA_FIELD.DataBind();
								if ( lstSORT_EXPRESSION != null )
								{
									lstSORT_EXPRESSION.DataSource = dt;
									lstSORT_EXPRESSION.DataBind();
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
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !this.IsPostBack || lstLIST_NAME.Items.Count == 0 )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select LIST_NAME             " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY_PickList" + ControlChars.CrLf
						     + " order by LIST_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								DataTable dt = new DataTable();
								da.Fill(dt);
								// 01/07/2006 Paul.  Having a problem with manual list inserts getting lost.  Try modifying the table. 
								// 07/15/2007 Paul.  There are a bunch more cached lists that need to be added. 
								// 08/17/2007 Paul.  We forgot to include the blank string.  
								// Without the blank string, all data fields would need to be associated with a list, which is a major problem. 
								string[] arrCachedLists = new string[]
									{ ""
									, "AssignedUser"
									, "Currencies"
									, "Release"
									, "Manufacturers"
									, "Shippers"
									, "ProductTypes"
									, "ProductCategories"
									, "ContractTypes"
									, "ForumTopics"
									};
								for ( int i = 0; i < arrCachedLists.Length; i++ )
								{
									DataRow row = dt.NewRow();
									row["LIST_NAME"] = arrCachedLists[i];
									dt.Rows.InsertAt(row, i);
								}
								lstLIST_NAME.DataSource = dt.DefaultView;
								lstLIST_NAME.DataBind();
							}
						}
					}
					lstFIELD_TYPE_Changed(null, null);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					lblError.Text = ex.Message;
				}
			}
			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//this.DataBind();  // Need to bind so that Text of the Button gets updated. 
			//reqNAME.ErrorMessage = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + " " + L10n.Term("DynamicLayout.LBL_DATA_NAME") + "<br>";
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
