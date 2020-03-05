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

namespace SplendidCRM.Administration.DynamicLayout.EditViews
{
	/// <summary>
	///		Summary description for NewRecord.
	/// </summary>
	public class NewRecord : DynamicLayout.NewRecord
	{
		protected CheckBox           chkDATA_REQUIRED    ;  // All except blank
		protected CheckBox           chkUI_REQUIRED      ;  // All except blank
		protected TextBox            txtDISPLAY_FIELD    ;  // ChangeButton
		protected TextBox            txtONCLICK_SCRIPT   ;  // ChangeButton
		protected TextBox            txtFORMAT_SCRIPT    ;  // None
		protected TextBox            txtFORMAT_TAB_INDEX ;  // All except blank
		protected TextBox            txtFORMAT_MAX_LENGTH;  // TextBox, Password, File
		protected TextBox            txtFORMAT_SIZE      ;  // TextBox, Password, File
		protected TextBox            txtFORMAT_ROWS      ;  // TextBox
		protected TextBox            txtFORMAT_COLUMNS   ;  // TextBox
		protected TextBox            txtCOLSPAN          ;  // All except blank
		protected TextBox            txtROWSPAN          ;  // AddressButtons

		protected HtmlGenericControl spnDATA             ;  // All except blank
		protected HtmlGenericControl spnREQUIRED         ;  // All except blank
		protected HtmlGenericControl spnCHANGE           ;  // ChangeButton only
		protected HtmlGenericControl spnTEXT             ;  // TextBox, Password, File
		protected HtmlGenericControl spnLIST_NAME        ;  // ListBox only
		protected HtmlGenericControl spnGENERAL          ;  // All except blank

		public override void Clear()
		{
			base.Clear();
			txtDISPLAY_FIELD    .Text    = String.Empty;
			txtONCLICK_SCRIPT   .Text    = String.Empty;
			txtFORMAT_SCRIPT    .Text    = String.Empty;
			txtFORMAT_TAB_INDEX .Text    = String.Empty;
			txtFORMAT_MAX_LENGTH.Text    = String.Empty;
			txtFORMAT_SIZE      .Text    = String.Empty;
			txtFORMAT_ROWS      .Text    = String.Empty;
			txtFORMAT_COLUMNS   .Text    = String.Empty;
			txtCOLSPAN          .Text    = String.Empty;
			txtROWSPAN          .Text    = String.Empty;
			chkDATA_REQUIRED    .Checked = false;
			chkUI_REQUIRED      .Checked = false;
		}

		public string DISPLAY_FIELD
		{
			get { return txtDISPLAY_FIELD.Text; }
			set { txtDISPLAY_FIELD.Text = value; }
		}

		public string ONCLICK_SCRIPT
		{
			get { return txtONCLICK_SCRIPT.Text; }
			set { txtONCLICK_SCRIPT.Text = value; }
		}

		public string FORMAT_SCRIPT
		{
			get { return txtFORMAT_SCRIPT.Text; }
			set { txtFORMAT_SCRIPT.Text = value; }
		}

		public int FORMAT_TAB_INDEX
		{
			get { return Sql.ToInteger(txtFORMAT_TAB_INDEX.Text); }
			set
			{
				if ( value > 0 )
					txtFORMAT_TAB_INDEX.Text = value.ToString();
				else
					txtFORMAT_TAB_INDEX.Text = String.Empty;
			}
		}

		public int FORMAT_MAX_LENGTH
		{
			get { return Sql.ToInteger(txtFORMAT_MAX_LENGTH.Text); }
			set
			{
				if ( value > 0 )
					txtFORMAT_MAX_LENGTH.Text = value.ToString();
				else
					txtFORMAT_MAX_LENGTH.Text = String.Empty;
			}
		}

		public int FORMAT_SIZE
		{
			get { return Sql.ToInteger(txtFORMAT_SIZE.Text); }
			set
			{
				if ( value > 0 )
					txtFORMAT_SIZE.Text = value.ToString();
				else
					txtFORMAT_SIZE.Text = String.Empty;
			}
		}

		public int FORMAT_ROWS
		{
			get { return Sql.ToInteger(txtFORMAT_ROWS.Text); }
			set
			{
				if ( value > 0 )
					txtFORMAT_ROWS.Text = value.ToString();
				else
					txtFORMAT_ROWS.Text = String.Empty;
			}
		}

		public int FORMAT_COLUMNS
		{
			get { return Sql.ToInteger(txtFORMAT_COLUMNS.Text); }
			set
			{
				if ( value > 0 )
					txtFORMAT_COLUMNS.Text = value.ToString();
				else
					txtFORMAT_COLUMNS.Text = String.Empty;
			}
		}

		public int COLSPAN
		{
			get { return Sql.ToInteger(txtCOLSPAN.Text); }
			set
			{
				if ( value > 0 )
					txtCOLSPAN.Text = value.ToString();
				else
					txtCOLSPAN.Text = String.Empty;
			}
		}

		public int ROWSPAN
		{
			get { return Sql.ToInteger(txtROWSPAN.Text); }
			set
			{
				if ( value > 0 )
					txtROWSPAN.Text = value.ToString();
				else
					txtROWSPAN.Text = String.Empty;
			}
		}

		public bool DATA_REQUIRED
		{
			get { return chkDATA_REQUIRED.Checked; }
			set { chkDATA_REQUIRED.Checked = value; }
		}

		public bool UI_REQUIRED
		{
			get { return chkUI_REQUIRED.Checked; }
			set { chkUI_REQUIRED.Checked = value; }
		}

		protected override void lstFIELD_TYPE_Changed(Object sender, EventArgs e)
		{
			switch ( lstFIELD_TYPE.SelectedValue )
			{
				case "TextBox"       :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = true ;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "Label"         :  spnDATA.Visible = true ;  spnREQUIRED.Visible = false;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "ListBox"       :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = true ;  spnGENERAL.Visible = true ;  break;
				case "CheckBox"      :  spnDATA.Visible = true ;  spnREQUIRED.Visible = false;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "ChangeButton"  :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = true ;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "DatePicker"    :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "DateRange"     :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "DateTimeEdit"  :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "DateTimePicker":  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "Image"         :  spnDATA.Visible = true ;  spnREQUIRED.Visible = false;  spnCHANGE.Visible = false;  spnTEXT.Visible = true ;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "File"          :  spnDATA.Visible = true ;  spnREQUIRED.Visible = false;  spnCHANGE.Visible = false;  spnTEXT.Visible = true ;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "Password"      :  spnDATA.Visible = true ;  spnREQUIRED.Visible = true ;  spnCHANGE.Visible = false;  spnTEXT.Visible = true ;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "AddressButtons":  spnDATA.Visible = true ;  spnREQUIRED.Visible = false;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = true ;  break;
				case "Blank"         :  spnDATA.Visible = false;  spnREQUIRED.Visible = false;  spnCHANGE.Visible = false;  spnTEXT.Visible = false;  spnLIST_NAME.Visible = false;  spnGENERAL.Visible = false;  break;
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
