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

namespace SplendidCRM.Administration.EditCustomFields
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected string        sMODULE_NAME   ;
		protected SearchBasic          ctlSearch   ;
		protected _controls.ListHeader ctlListTitle;
		protected NewRecord            ctlNewRecord;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					if ( ctlNewRecord != null )
					{
						ctlNewRecord.Clear();
					}
				}
				else if ( e.CommandName == "NewRecord" )
				{
					FIELDS_META_DATA_Bind();
				}
				else if ( e.CommandName == "EditCustomFields.Delete" )
				{
					Guid gID = Sql.ToGuid(e.CommandArgument);

					// 07/18/2006 Paul.  Manually create the command so that we can increase the timeout. 
					// 07/18/2006 Paul.  Keep the original procedure call so that we will get a compiler error if something changes. 
					bool bIncreaseTimeout = true;
					if ( !bIncreaseTimeout )
					{
						SqlProcs.spFIELDS_META_DATA_Delete(gID);
					}
					else
					{
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
										cmd.CommandText = "spFIELDS_META_DATA_Delete";
										// 07/18/2006 Paul.  Tripple the default timeout.  The operation was timing-out on QA machines and on the demo server. 
										// 02/03/2007 Paul.  Increase timeout to 5 minutes.  It should not take that long, but some users are reporting a timeout. 
										cmd.CommandTimeout = 5*60;
										IDbDataParameter parID               = Sql.AddParameter(cmd, "@ID"              , gID                );
										IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID",  Security.USER_ID  );
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
					FIELDS_META_DATA_Bind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void FIELDS_META_DATA_Bind()
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                             " + ControlChars.CrLf
					     + "  from vwFIELDS_META_DATA_List       " + ControlChars.CrLf
					     + " where 1 = 1                         " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						if ( IsPostBack )
							ctlSearch.SqlSearchClause(cmd);
						else
							Sql.AppendParameter(cmd, sMODULE_NAME, "CUSTOM_MODULE");
						cmd.CommandText += " order by NAME" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 10/06/2005 Paul.  Convert the term here so that sorting will apply. 
								foreach(DataRow row in dt.Rows)
								{
									row["REQUIRED_OPTION"] = L10n.Term(Sql.ToString(row["REQUIRED_OPTION"]));
								}
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								// 01/06/2006 Paul.  Always bind the table, otherwise the table events will not fire. 
								grdMain.DataBind();
								// 01/05/2006 Paul.  Need to rebind this control so that the grid headers get translated. 
								// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
								//this.DataBind();
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
			ctlListTitle.Visible = grdMain.Visible;
			ctlListTitle.Title = L10n.Term("EditCustomFields.LBL_CUSTOM_FIELDS") + ": " + L10n.Term(".moduleList." + sMODULE_NAME);
			if ( ctlNewRecord != null )
			{
				ctlNewRecord.MODULE_NAME = sMODULE_NAME;
				ctlNewRecord.Command = new CommandEventHandler(Page_Command);
			}
			// 01/04/2006 Paul. EnableNewRecord is not working, but I don't know why.
			// As an alternative, hide a div tag in NewRecord.ascx. 
			if ( ctlNewRecord != null )
			{
				ctlNewRecord.Visible = grdMain.Visible;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("EditCustomFields.LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
			if ( !this.Visible )
				return;

			// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
			//Page.DataBind();
			// 02/08/2007 Paul.  The NewRecord control is now in the MasterPage. 
			ContentPlaceHolder plcSidebar = Page.Master.FindControl("cntSidebar") as ContentPlaceHolder;
			if ( plcSidebar != null )
			{
				ctlNewRecord = plcSidebar.FindControl("ctlNewRecord") as NewRecord;
			}
			if ( IsPostBack )
			{
				sMODULE_NAME = ctlSearch.MODULE_NAME;
			}
			else
			{
				sMODULE_NAME = Sql.ToString(Request["MODULE_NAME"]);
				// 01/05/2006 Paul.  Fix Form Action so that Query String parameters will not continue to get passed around. 
				if ( !Sql.IsEmptyString(sMODULE_NAME) )
					RegisterClientScriptBlock("frmRedirect", "<script type=\"text/javascript\">document.forms[0].action='default.aspx';</script>");
			}

			grdMain.Visible = IsPostBack || !Sql.IsEmptyString(sMODULE_NAME);
			FIELDS_META_DATA_Bind();
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
			// We have to load the control in here, otherwise the control will not initialized before the Page_Load above. 
			ctlSearch.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
