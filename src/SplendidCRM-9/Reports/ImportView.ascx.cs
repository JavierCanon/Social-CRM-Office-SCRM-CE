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
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Reports
{
	/// <summary>
	///		Summary description for ImportView.
	/// </summary>
	public class ImportView : SplendidControl
	{
		protected _controls.ImportButtons ctlImportButtons;

		protected TextBox                txtNAME                 ;
		protected DropDownList           lstMODULE               ;
		protected DropDownList           lstREPORT_TYPE          ;
		protected TextBox                txtASSIGNED_TO          ;
		protected HtmlInputHidden        txtASSIGNED_USER_ID     ;
		protected HtmlInputFile          fileIMPORT              ;
		protected RequiredFieldValidator reqNAME                 ;
		protected RequiredFieldValidator reqFILENAME             ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Import" )
				{
					reqNAME.Enabled = true;
					reqNAME.Validate();
					reqFILENAME.Enabled = true;
					reqFILENAME.Validate();
					if ( Page.IsValid )
					{
						HttpPostedFile pstIMPORT = fileIMPORT.PostedFile;
						if ( pstIMPORT != null )
						{
							if ( pstIMPORT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstIMPORT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstIMPORT.ContentType;
								
								RdlDocument rdl = new RdlDocument();
								rdl.Load(pstIMPORT.InputStream);
								rdl.SetSingleNodeAttribute(rdl.DocumentElement, "Name", txtNAME.Text);
								// 10/22/2007 Paul.  Use the Assigned User ID field when saving the record. 
								Guid gID = Guid.Empty;
								SqlProcs.spREPORTS_Update(ref gID, Sql.ToGuid(txtASSIGNED_USER_ID.Value), txtNAME.Text, lstMODULE.SelectedValue, lstREPORT_TYPE.SelectedValue, rdl.OuterXml);
							}
						}
						Response.Redirect("default.aspx");
					}
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlImportButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;
			
			reqNAME.DataBind();
			reqFILENAME.DataBind();
			try
			{
				if ( !IsPostBack )
				{
					txtASSIGNED_TO.Text       = Security.USER_NAME;
					txtASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
					ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					lstMODULE.DataSource = SplendidCache.ReportingModules();
					lstMODULE.DataBind();
					lstREPORT_TYPE.DataSource = SplendidCache.List("dom_report_types");
					lstREPORT_TYPE.DataBind();
					try
					{
						lstREPORT_TYPE.SelectedValue = "Freeform";
					}
					catch
					{
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlImportButtons.ErrorText = ex.Message;
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
			ctlImportButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Reports";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
