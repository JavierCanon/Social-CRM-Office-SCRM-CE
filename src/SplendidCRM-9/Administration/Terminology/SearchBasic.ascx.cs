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
using System.Web.UI.WebControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.Terminology
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected _controls.SearchButtons ctlSearchButtons;

		protected TextBox      txtNAME            ;
		protected TextBox      txtDISPLAY_NAME    ;
		protected DropDownList lstLANGUAGE        ;
		protected DropDownList lstMODULE_NAME     ;
		protected DropDownList lstLIST_NAME       ;
		protected CheckBox     chkGLOBAL_TERMS    ;
		protected CheckBox     chkINCLUDE_LISTS   ;

		public bool GLOBAL_TERMS
		{
			get { return chkGLOBAL_TERMS.Checked; }
			set { chkGLOBAL_TERMS.Checked = value; }
		}

		public bool INCLUDE_LISTS
		{
			get { return chkINCLUDE_LISTS.Checked; }
			set { chkINCLUDE_LISTS.Checked = value; }
		}

		/*
		public string LANGUAGE
		{
			get
			{
				return lstLANGUAGE.SelectedValue;
			}
			set
			{
				if ( lstLANGUAGE.DataSource == null )
				{
					lstLANGUAGE.DataSource = SplendidCache.Languages();
					lstLANGUAGE.DataBind();
					lstLANGUAGE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				}
				Utils.SetValue(lstLANGUAGE, L10N.NormalizeCulture(value));
			}
		}
		*/

		public override void ClearForm()
		{
			txtNAME        .Text = String.Empty;
			txtDISPLAY_NAME.Text = String.Empty;
			lstLANGUAGE    .SelectedIndex = 0;
			lstMODULE_NAME .SelectedIndex = 0;
			lstLIST_NAME   .SelectedIndex = 0;
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, txtNAME        .Text         ,   50, Sql.SqlFilterMode.StartsWith, "NAME"        );
			Sql.AppendParameter(cmd, txtDISPLAY_NAME.Text         , 2000, Sql.SqlFilterMode.StartsWith, "DISPLAY_NAME");
			Sql.AppendParameter(cmd, lstLANGUAGE    .SelectedValue,   10, Sql.SqlFilterMode.Exact     , "LANG"        );
			Sql.AppendParameter(cmd, lstMODULE_NAME .SelectedValue,   20, Sql.SqlFilterMode.Exact     , "MODULE_NAME" );
			Sql.AppendParameter(cmd, lstLIST_NAME   .SelectedValue,   50, Sql.SqlFilterMode.Exact     , "LIST_NAME"   );
		}

		protected void chkGLOBAL_TERMS_CheckedChanged(Object sender, EventArgs e)
		{
			if ( Command != null )
				Command(this, new CommandEventArgs("Search", String.Empty)) ;
		}
		
		protected void chkINCLUDE_LISTS_CheckedChanged(Object sender, EventArgs e)
		{
			if ( Command != null )
				Command(this, new CommandEventArgs("Search", String.Empty)) ;
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				lstLANGUAGE.DataSource = SplendidCache.Languages();
				lstLANGUAGE.DataBind();
				lstLANGUAGE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				Utils.SetValue(lstLANGUAGE, L10N.NormalizeCulture(L10n.NAME));

				lstMODULE_NAME.DataSource = SplendidCache.Modules();
				lstMODULE_NAME.DataBind();
				lstMODULE_NAME.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));

				lstLIST_NAME  .DataSource = SplendidCache.TerminologyPickLists();
				lstLIST_NAME  .DataBind();
				lstLIST_NAME  .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
			}
			if ( !chkINCLUDE_LISTS.Checked )
				lstLIST_NAME.SelectedIndex = 0;
			lstLIST_NAME.Enabled = chkINCLUDE_LISTS.Checked;
			if ( chkGLOBAL_TERMS.Checked )
				lstMODULE_NAME.SelectedIndex = 0;
			lstMODULE_NAME.Enabled = !chkGLOBAL_TERMS.Checked;
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
			ctlSearchButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
