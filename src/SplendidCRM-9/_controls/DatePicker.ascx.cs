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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for DatePicker.
	/// </summary>
	public class DatePicker : SplendidControl
	{
		protected TextBox  txtDATE      ;
		protected Label    lblDateFormat;
		protected System.Web.UI.WebControls.Image    imgCalendar  ;
		protected RequiredFieldValidator     reqDATE;
		// 08/31/2006 Paul.  We cannot use a regular expression validator because there are just too many date formats.
		protected DateValidator              valDATE;

		public DateTime Value
		{
			get
			{
				// 07/09/2006 Paul.  Dates are no longer converted inside this control. 
				return Sql.ToDateTime(txtDATE.Text);
			}
			set
			{
				txtDATE.Text = Sql.ToDateString(value);
			}
		}

		public string DateText
		{
			get
			{
				return txtDATE.Text;
			}
			set
			{
				txtDATE.Text = value;
			}
		}

		public string DateClientID
		{
			get
			{
				return txtDATE.ClientID;
			}
		}

		public short TabIndex
		{
			get
			{
				return txtDATE.TabIndex;
			}
			set
			{
				txtDATE.TabIndex = value;
			}
		}

		public bool EnableDateFormat
		{
			get
			{
				return lblDateFormat.Visible;
			}
			set
			{
				lblDateFormat.Visible = value;
			}
		}

		// 04/05/2006 Paul.  Need a way to clear the date. 
		public void Clear()
		{
			txtDATE.Text = String.Empty;
		}

		public void Validate()
		{
			reqDATE.Enabled = true;
			valDATE.Enabled = true;
			// 11/07/2005 Paul.  Not sure why rglDATE is not available. 
			//rglDATE.Enabled = true;
			// 04/15/2006 Paul.  The error message is not binding properly.  Just assign here as a quick solution. 
			// 06/09/2006 Paul.  Now that we have solved the data binding issues, we can let the binding fill the message. 
			//rglDATE.ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			// 08/31/2006 Paul.  Enable and perform date validation. 
			reqDATE.Validate();
			valDATE.Validate();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/09/2006 Paul.  Always set the message as this control does not remember its state. 
			reqDATE.ErrorMessage = L10n.Term(".ERR_REQUIRED_FIELD");
			// 08/31/2006 Paul.  Need to bind the text. 
			valDATE.ErrorMessage = L10n.Term(".ERR_INVALID_DATE");
			if ( !IsPostBack )
			{
				// 06/29/2006 Paul.  The image needs to be manually bound in Administration/ProductTemplates/EditView.ascx
				imgCalendar.DataBind();
				// 07/05/2006 Paul.  Need to bind the label manually. 
				// 07/06/2005 Paul.  lblDateFormat is not defined in ChartDatePicker, so we must test if lblDateFormat exists. 
				if ( lblDateFormat != null )
					lblDateFormat.DataBind();
				//this.DataBind();
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
