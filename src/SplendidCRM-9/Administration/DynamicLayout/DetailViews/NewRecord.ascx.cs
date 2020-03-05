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

namespace SplendidCRM.Administration.DynamicLayout.DetailViews
{
	/// <summary>
	///		Summary description for NewRecord.
	/// </summary>
	public class NewRecord : DynamicLayout.NewRecord
	{
		protected TextBox            txtDATA_FORMAT ;
		protected TextBox            txtURL_FIELD   ;
		protected TextBox            txtURL_FORMAT  ;
		protected TextBox            txtURL_TARGET  ;
		protected TextBox            txtCOLSPAN     ;
		protected HtmlGenericControl spnDATA        ;
		protected HtmlGenericControl spnDATA_FORMAT ;
		protected HtmlGenericControl spnURL         ;
		protected HtmlGenericControl spnLIST_NAME   ;

		public override void Clear()
		{
			base.Clear();
			txtDATA_FORMAT.Text  = String.Empty;
			txtURL_FIELD  .Text  = String.Empty;
			txtURL_FORMAT .Text  = String.Empty;
			txtURL_TARGET .Text  = String.Empty;
			txtCOLSPAN    .Text  = String.Empty;
		}

		public string DATA_FORMAT
		{
			get { return txtDATA_FORMAT.Text; }
			set { txtDATA_FORMAT.Text = value; }
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

		protected override void lstFIELD_TYPE_Changed(Object sender, EventArgs e)
		{
			switch ( lstFIELD_TYPE.SelectedValue )
			{
				case "String"   :  spnDATA.Visible = true ;   spnDATA_FORMAT.Visible = true ;  spnURL.Visible = false;  spnLIST_NAME.Visible = true ;  break;
				case "TextBox"  :  spnDATA.Visible = true ;   spnDATA_FORMAT.Visible = true ;  spnURL.Visible = false;  spnLIST_NAME.Visible = false;  break;
				case "HyperLink":  spnDATA.Visible = true ;   spnDATA_FORMAT.Visible = true ;  spnURL.Visible = true ;  spnLIST_NAME.Visible = false;  break;
				case "CheckBox" :  spnDATA.Visible = true ;   spnDATA_FORMAT.Visible = false;  spnURL.Visible = false;  spnLIST_NAME.Visible = false;  break;
				case "Button"   :  spnDATA.Visible = true ;   spnDATA_FORMAT.Visible = true ;  spnURL.Visible = false;  spnLIST_NAME.Visible = false;  break;
				case "Image"    :  spnDATA.Visible = true ;   spnDATA_FORMAT.Visible = false;  spnURL.Visible = false;  spnLIST_NAME.Visible = false;  break;
				case "Blank"    :  spnDATA.Visible = false;   spnDATA_FORMAT.Visible = false;  spnURL.Visible = false;  spnLIST_NAME.Visible = false;  break;
				case "Line"     :  spnDATA.Visible = false;   spnDATA_FORMAT.Visible = false;  spnURL.Visible = false;  spnLIST_NAME.Visible = false;  break;
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
