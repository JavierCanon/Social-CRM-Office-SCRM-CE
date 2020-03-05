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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Threads
{
	/// <summary>
	/// Summary description for PostView.
	/// </summary>
	public class PostView : SplendidControl
	{
		protected _controls.PostButtons ctlPostButtons;

		protected HiddenField  txtPOST_ID       ;
		protected Label        txtTITLE         ;
		protected Label        txtCREATED_BY    ;
		protected Label        txtDATE_ENTERED  ;
		protected Label        txtMODIFIED_BY   ;
		protected Label        txtDATE_MODIFIED ;
		protected Literal      txtDESCRIPTION   ;
		protected HtmlTableRow trModified       ;

		public Guid POST_ID
		{
			get { return Sql.ToGuid(txtPOST_ID.Value); }
			set { txtPOST_ID.Value = value.ToString(); }
		}

		public string TITLE
		{
			get { return txtTITLE.Text; }
			set { txtTITLE.Text = value; }
		}

		public string CREATED_BY
		{
			get { return txtCREATED_BY.Text; }
			set { txtCREATED_BY.Text = value; }
		}

		public string DATE_ENTERED
		{
			get { return txtDATE_ENTERED.Text; }
			set { txtDATE_ENTERED.Text = value; }
		}

		public string MODIFIED_BY
		{
			get { return txtMODIFIED_BY.Text; }
			set { txtMODIFIED_BY.Text = value; }
		}

		public string DATE_MODIFIED
		{
			get { return txtDATE_MODIFIED.Text; }
			set { txtDATE_MODIFIED.Text = value; }
		}

		public string DESCRIPTION
		{
			get { return txtDESCRIPTION.Text; }
			set { txtDESCRIPTION.Text = value; }
		}

		public bool Modified
		{
			get { return trModified.Visible; }
			set { trModified.Visible = value; }
		}

		public bool ShowEdit
		{
			get { return ctlPostButtons.ShowEdit; }
			set { ctlPostButtons.ShowEdit = value; }
		}

		public bool ShowDelete
		{
			get { return ctlPostButtons.ShowDelete; }
			set { ctlPostButtons.ShowDelete = value; }
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				Guid gPOST_ID = POST_ID;
				if ( e.CommandName == "Reply" )
				{
					Response.Redirect("~/Posts/edit.aspx?REPLY_ID=" + gPOST_ID.ToString());
				}
				else if ( e.CommandName == "Quote" )
				{
					Response.Redirect("~/Posts/edit.aspx?QUOTE=1&REPLY_ID=" + gPOST_ID.ToString());
				}
				else if ( e.CommandName == "Edit" )
				{
					Response.Redirect("~/Posts/edit.aspx?ID=" + gPOST_ID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spPOSTS_Delete(gPOST_ID);
					Guid gID = Sql.ToGuid(Request["ID"]);
					int nListView = Sql.ToInteger(Request["ListView"]);
					Response.Redirect("view.aspx?ID=" + gID.ToString() + "&ListView=" + nListView.ToString());
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlPostButtons.ErrorText = ex.Message;
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlPostButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Posts";
		}
		#endregion
	}
}
