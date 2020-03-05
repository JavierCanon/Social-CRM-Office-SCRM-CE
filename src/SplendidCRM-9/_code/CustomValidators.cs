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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SplendidCRM._controls;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for CustomValidators.
	/// </summary>
	public class RequiredFieldValidatorForCheckBoxLists : System.Web.UI.WebControls.BaseValidator 
	{
		private ListControl lst;

		public RequiredFieldValidatorForCheckBoxLists()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);

			if ( ctl != null )
			{
				lst = (ListControl) ctl;
				return (lst != null) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			return lst.SelectedIndex != -1;
		}
	}

	public class RequiredFieldValidatorForDropDownList : System.Web.UI.WebControls.BaseValidator 
	{
		private DropDownList lst;

		public RequiredFieldValidatorForDropDownList()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);

			if ( ctl != null )
			{
				lst = (DropDownList) ctl;
				return (lst != null) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			// 03/14/2006 Paul.  Use SelectedValue to determine if the dropdown is valid. 
			// Using a dropdown validator is not required because we only use the -- None -- first item when not required. 
			return !Sql.IsEmptyString(lst.SelectedValue);
		}
	}

	public class RequiredFieldValidatorForHiddenInputs : System.Web.UI.WebControls.BaseValidator 
	{
		// 12/03/2007 Paul.  The hidden field could be HtmlInputHidden or HiddenField. 
		private Control hid;

		public RequiredFieldValidatorForHiddenInputs()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);
			if ( ctl != null )
			{
				hid = ctl;
				return (hid.GetType() == typeof(System.Web.UI.HtmlControls.HtmlInputHidden) || hid.GetType() == typeof(System.Web.UI.WebControls.HiddenField)) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			if ( hid.GetType() == typeof(System.Web.UI.HtmlControls.HtmlInputHidden) )
				return !Sql.IsEmptyString((hid as HtmlInputHidden).Value) ;
			else if ( hid.GetType() == typeof(System.Web.UI.WebControls.HiddenField) )
				return !Sql.IsEmptyString((hid as HiddenField).Value) ;
			else
				return true;
		}
	}

	public class DateValidator : System.Web.UI.WebControls.BaseValidator 
	{
		private TextBox txt;

		public DateValidator()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);

			if ( ctl != null )
			{
				txt = (TextBox) ctl;
				return (txt != null) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			// 10/13/2005 Paul.  An empty string is treated as a valid date.  A separate RequiredFieldValidator is required to handle this condition. 
			return (txt.Text.Trim() == String.Empty) || Information.IsDate(txt.Text);
		}
	}

	public class TimeValidator : System.Web.UI.WebControls.BaseValidator 
	{
		private TextBox txt;

		public TimeValidator()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);

			if ( ctl != null )
			{
				txt = (TextBox) ctl;
				return (txt != null) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			// 03/03/2006 Paul.  An empty string is treated as a valid date.  A separate RequiredFieldValidator is required to handle this condition. 
			// 03/03/2006 Paul.  Validate with a prepended date so that it will fail if the user also supplies a date. 
			return (txt.Text.Trim() == String.Empty) || Information.IsDate(DateTime.Now.ToShortDateString() + " " + txt.Text);
		}
	}

	public class DatePickerValidator : System.Web.UI.WebControls.BaseValidator 
	{
		private DatePicker ctlDate;

		public DatePickerValidator()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);

			if ( ctl != null )
			{
				ctlDate = (DatePicker) ctl;
				return (ctlDate != null) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			// 03/03/2006 Paul.  An empty string is treated as a valid date.  A separate RequiredFieldValidator is required to handle this condition. 
			return (ctlDate.DateText.Trim() == String.Empty) || Information.IsDate(ctlDate.DateText);
		}
	}

	public class RequiredFieldValidatorForDatePicker : System.Web.UI.WebControls.BaseValidator 
	{
		private DatePicker ctlDate;

		public RequiredFieldValidatorForDatePicker()
		{
			base.EnableClientScript = false;
		}

		protected override bool ControlPropertiesValid()
		{
			Control ctl = FindControl(ControlToValidate);

			if ( ctl != null )
			{
				ctlDate = (DatePicker) ctl;
				return (ctlDate != null) ;
			}
			else 
				return false;  // raise exception
		}

		protected override bool EvaluateIsValid()
		{
			return !Sql.IsEmptyString(ctlDate.DateText) ;
		}
	}
}

