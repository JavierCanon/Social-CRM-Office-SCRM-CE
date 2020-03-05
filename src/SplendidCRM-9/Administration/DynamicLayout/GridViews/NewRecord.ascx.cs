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

namespace SplendidCRM.Administration.DynamicLayout.GridViews
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : DynamicLayout.NewRecord
	{
		protected DropDownList               lstDATA_FORMAT               ;
		protected TextBox                    txtITEMSTYLE_WIDTH           ;
		protected TextBox                    txtITEMSTYLE_CSSCLASS        ;
		protected DropDownList               lstITEMSTYLE_HORIZONTAL_ALIGN;
		protected DropDownList               lstITEMSTYLE_VERTICAL_ALIGN  ;
		protected CheckBox                   chkITEMSTYLE_WRAP            ;
		protected TextBox                    txtURL_FIELD                 ;
		protected TextBox                    txtURL_FORMAT                ;
		protected TextBox                    txtURL_TARGET                ;
		protected TextBox                    txtURL_MODULE                ;
		protected TextBox                    txtURL_ASSIGNED_FIELD        ;
		protected HtmlGenericControl         spnURL                       ;

		public override void Clear()
		{
			base.Clear();
			txtDATA_FIELD        .Text  = String.Empty;
			txtITEMSTYLE_WIDTH   .Text  = String.Empty;
			txtITEMSTYLE_CSSCLASS.Text  = String.Empty;
			txtURL_FIELD         .Text  = String.Empty;
			txtURL_FORMAT        .Text  = String.Empty;
			txtURL_TARGET        .Text  = String.Empty;
			txtURL_MODULE        .Text  = String.Empty;
			txtURL_ASSIGNED_FIELD.Text  = String.Empty;
			lstDATA_FORMAT               .SelectedIndex = 0;
			lstITEMSTYLE_HORIZONTAL_ALIGN.SelectedIndex = 0;
			lstITEMSTYLE_VERTICAL_ALIGN  .SelectedIndex = 0;
			chkITEMSTYLE_WRAP.Checked = false;
		}

		public string DATA_FORMAT
		{
			get
			{
				return lstDATA_FORMAT.SelectedValue;
			}
			set
			{
				try
				{
					lstDATA_FORMAT.SelectedValue = value;
					lstFIELD_TYPE_Changed(null, null);
				}
				catch
				{
				}
			}
		}

		public string ITEMSTYLE_WIDTH
		{
			get { return txtITEMSTYLE_WIDTH.Text; }
			set { txtITEMSTYLE_WIDTH.Text = value; }
		}

		public string ITEMSTYLE_CSSCLASS
		{
			get { return txtITEMSTYLE_CSSCLASS.Text; }
			set { txtITEMSTYLE_CSSCLASS.Text = value; }
		}

		public string ITEMSTYLE_HORIZONTAL_ALIGN
		{
			get
			{
				return lstITEMSTYLE_HORIZONTAL_ALIGN.SelectedValue;
			}
			set
			{
				try
				{
					lstITEMSTYLE_HORIZONTAL_ALIGN.SelectedValue = value;
				}
				catch
				{
				}
			}
		}

		public string ITEMSTYLE_VERTICAL_ALIGN
		{
			get
			{
				return lstITEMSTYLE_VERTICAL_ALIGN.SelectedValue;
			}
			set
			{
				try
				{
					lstITEMSTYLE_VERTICAL_ALIGN.SelectedValue = value;
				}
				catch
				{
				}
			}
		}

		public bool ITEMSTYLE_WRAP
		{
			get { return chkITEMSTYLE_WRAP.Checked; }
			set { chkITEMSTYLE_WRAP.Checked = value; }
		}

		public string URL_FIELD
		{
			get { return txtURL_FIELD.Text; }
			set { txtURL_FIELD.Text = value; }
		}

		public string URL_FORMAT
		{
			get { return txtURL_FORMAT.Text; }
			set { txtURL_FORMAT.Text = value; }
		}

		public string URL_TARGET
		{
			get { return txtURL_TARGET.Text; }
			set { txtURL_TARGET.Text = value; }
		}

		public string URL_MODULE
		{
			get { return txtURL_MODULE.Text; }
			set { txtURL_MODULE.Text = value; }
		}

		public string URL_ASSIGNED_FIELD
		{
			get { return txtURL_ASSIGNED_FIELD.Text; }
			set { txtURL_ASSIGNED_FIELD.Text = value; }
		}

		protected override void lstFIELD_TYPE_Changed(Object sender, EventArgs e)
		{
			spnURL.Visible = lstFIELD_TYPE.SelectedValue == "HyperLinkColumn" || lstDATA_FORMAT.SelectedValue == "HyperLink";
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
