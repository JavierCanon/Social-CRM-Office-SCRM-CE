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

namespace SplendidCRM.Administration.Dropdown
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected DropDownList lstDROPDOWN_OPTIONS;
		protected DropDownList lstLANGUAGE_OPTIONS;

		public string DROPDOWN
		{
			get
			{
				return lstDROPDOWN_OPTIONS.SelectedValue;
			}
			set
			{
				if ( lstDROPDOWN_OPTIONS.DataSource == null )
				{
					lstDROPDOWN_OPTIONS.DataSource = SplendidCache.TerminologyPickLists();
					lstDROPDOWN_OPTIONS.DataBind();
				}
				lstDROPDOWN_OPTIONS.SelectedValue = value;
			}
		}

		public string LANGUAGE
		{
			get
			{
				return lstLANGUAGE_OPTIONS.SelectedValue;
			}
			set
			{
				if ( lstLANGUAGE_OPTIONS.DataSource == null )
				{
					lstLANGUAGE_OPTIONS.DataSource = SplendidCache.Languages();
					lstLANGUAGE_OPTIONS.DataBind();
				}
				Utils.SetValue(lstLANGUAGE_OPTIONS, L10N.NormalizeCulture(value));
			}
		}

		public override void ClearForm()
		{
			lstDROPDOWN_OPTIONS.SelectedIndex = 0;
			lstLANGUAGE_OPTIONS.SelectedValue = L10N.NormalizeCulture("en-US");
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, lstDROPDOWN_OPTIONS.SelectedValue, 50, Sql.SqlFilterMode.Exact, "LIST_NAME");
			Sql.AppendParameter(cmd, lstLANGUAGE_OPTIONS.SelectedValue, 10, Sql.SqlFilterMode.Exact, "LANG"     );
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				if ( lstDROPDOWN_OPTIONS.DataSource == null )
				{
					lstDROPDOWN_OPTIONS.DataSource = SplendidCache.TerminologyPickLists();
					lstDROPDOWN_OPTIONS.DataBind();
				}
				if ( lstLANGUAGE_OPTIONS.DataSource == null )
				{
					lstLANGUAGE_OPTIONS.DataSource = SplendidCache.Languages();
					lstLANGUAGE_OPTIONS.DataBind();
					Utils.SetValue(lstLANGUAGE_OPTIONS, L10N.NormalizeCulture(L10n.NAME));
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
		}
		#endregion
	}
}
