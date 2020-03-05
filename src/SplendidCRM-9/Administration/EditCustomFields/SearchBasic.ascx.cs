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

namespace SplendidCRM.Administration.EditCustomFields
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected DropDownList lstMODULE_NAME;

		public string MODULE_NAME
		{
			get
			{
				return lstMODULE_NAME.SelectedValue;
			}
		}

		public override void ClearForm()
		{
			lstMODULE_NAME.SelectedIndex = 0;
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, lstMODULE_NAME, "CUSTOM_MODULE");
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !this.IsPostBack )
			{
				// 05/20/2007 Paul.  Make sure to copy the table before modifying the data, otherwise the changes will get applied to the cached table. 
				DataTable dtCustomEditModules = SplendidCache.CustomEditModules().Copy();
				foreach(DataRow row in dtCustomEditModules.Rows)
				{
					row["DISPLAY_NAME"] = L10n.Term(".moduleList." + row["DISPLAY_NAME"]);
				}
				lstMODULE_NAME.DataSource = dtCustomEditModules;
				lstMODULE_NAME.DataBind();
				// 01/05/2006 Paul.  Can't seem to set the selected value from ListView.ascx. 
				string sMODULE_NAME = Sql.ToString(Request["MODULE_NAME"]);
				try
				{
					lstMODULE_NAME.SelectedValue = sMODULE_NAME;
				}
				catch
				{
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
