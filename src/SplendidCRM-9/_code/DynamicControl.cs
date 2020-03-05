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
using System.Xml;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for DynamicControl.
	/// </summary>
	public class DynamicControl
	{
		protected string          sNAME     ;
		protected SplendidControl ctlPARENT ;
		protected DataRow         rowCurrent;

		public bool Exists
		{
			get
			{
				Control ctl = ctlPARENT.FindControl(sNAME);
				return (ctl != null);
			}
		}

		public string Type
		{
			get
			{
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
					return ctl.GetType().Name;
				return String.Empty;
			}
		}

		public string ClientID
		{
			get
			{
				string sClientID = ctlPARENT.ID + ":" + sNAME;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					sClientID = ctl.ClientID;
				}
				return sClientID;
			}
		}

		public string Text
		{
			get
			{
				string sVALUE = String.Empty;
				Control ctl = ctlPARENT.FindControl(sNAME);
				// 01/10/2008 Paul.  Simplify by using the IS clause instead of GetType(). 
				if ( ctl != null )
				{
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					if ( ctl is TextBox )
					{
						TextBox txt = ctl as TextBox;
						// 01/29/2008 Paul.  Lets trim the text everwhere.  The only place that trimming is not wanted is in the terminology editor. 
						sVALUE = txt.Text.Trim();
					}
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					else if ( ctl is Label )
					{
						Label lbl = ctl as Label;
						sVALUE = lbl.Text;
					}
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					else if ( ctl is DropDownList )
					{
						DropDownList lst = ctl as DropDownList;
						sVALUE = lst.SelectedValue;
					}
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					else if ( ctl is HtmlInputHidden )
					{
						HtmlInputHidden txt = ctl as HtmlInputHidden;
						sVALUE = txt.Value;
					}
					// 05/28/2007 Paul.  .NET 2.0 has a new control for the hidden field and we are starting to use it in the Payments module. 
					else if ( ctl is HiddenField )
					{
						HiddenField txt = ctl as HiddenField;
						sVALUE = txt.Value;
					}
					// 09/05/2006 Paul.  DetailViews place the literal in a span. 
					else if ( ctl is HtmlGenericControl )
					{
						HtmlGenericControl spn = ctl as HtmlGenericControl;
						if ( spn.Controls.Count > 0 )
						{
							if ( spn.Controls[0] is Literal )
							{
								Literal txt = spn.Controls[0] as Literal;
								sVALUE = txt.Text;
							}
						}
						else
						{
							sVALUE = spn.InnerText;
						}
					}
					// 12/30/2007 Paul.  A customer needed the ability to save and restore the multiple selection. 
					else if ( ctl is ListBox )
					{
						ListBox lst = ctl as ListBox;
						if ( lst.SelectionMode == ListSelectionMode.Multiple )
						{
							XmlDocument xml = new XmlDocument();
							// 12/30/2007 Paul.  The XML declaration is important as it will be used to determine if the XML is valid during rendering. 
							xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
							xml.AppendChild(xml.CreateElement("Values"));
							int nSelected = 0;
							foreach(ListItem item in lst.Items)
							{
								if ( item.Selected )
									nSelected++;
							}
							if ( nSelected > 0 )
							{
								foreach(ListItem item in lst.Items)
								{
									if ( item.Selected )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										xValue.InnerText = item.Value;
									}
								}
							}
							sVALUE = xml.OuterXml;
						}
						else
						{
							sVALUE = lst.SelectedValue;
						}
					}
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						sVALUE = Sql.ToString(rowCurrent[sNAME]);
				}
				return sVALUE;
			}
			set
			{
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					if ( ctl is TextBox )
					{
						TextBox txt = ctl as TextBox;
						txt.Text = value;
					}
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					else if ( ctl is Label )
					{
						Label lbl = ctl as Label;
						lbl.Text = value;
					}
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					else if ( ctl is DropDownList )
					{
						DropDownList lst = ctl as DropDownList;
						try
						{
							lst.SelectedValue = value;
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
					}
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					else if ( ctl is HtmlInputHidden )
					{
						HtmlInputHidden txt = ctl as HtmlInputHidden;
						txt.Value = value;
					}
					// 06/05/2007 Paul.  .NET 2.0 has a new control for the hidden field and we are starting to use it in the Payments module. 
					else if ( ctl is HiddenField )
					{
						HiddenField txt = ctl as HiddenField;
						txt.Value = value;
					}
					// 07/24/2006 Paul.  Allow the text of a literal to be set. 
					else if ( ctl is Literal )
					{
						Literal txt = ctl as Literal;
						txt.Text = value;
					}
					// 09/05/2006 Paul.  DetailViews place the literal in a span. 
					else if ( ctl is HtmlGenericControl )
					{
						HtmlGenericControl spn = ctl as HtmlGenericControl;
						if ( spn.Controls.Count > 0 )
						{
							if ( spn.Controls[0] is Literal )
							{
								Literal txt = spn.Controls[0] as Literal;
								txt.Text = value;
							}
						}
						else
						{
							spn.InnerText = value;
						}
					}
					// 12/30/2007 Paul.  A customer needed the ability to save and restore the multiple selection. 
					else if ( ctl is ListBox )
					{
						ListBox lst = ctl as ListBox;
						try
						{
							// 12/30/2007 Paul.  Require the XML declaration in the data before trying to treat as XML. 
							string sVALUE = value;
							if ( lst.SelectionMode == ListSelectionMode.Multiple && sVALUE.StartsWith("<?xml") )
							{
								XmlDocument xml = new XmlDocument();
								xml.LoadXml(sVALUE);
								XmlNodeList nlValues = xml.DocumentElement.SelectNodes("Value");
								foreach ( XmlNode xValue in nlValues )
								{
									foreach ( ListItem item in lst.Items )
									{
										if ( item.Value == xValue.InnerText )
											item.Selected = true;
									}
								}
							}
							else
							{
								lst.SelectedValue = sVALUE;
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
			}
		}

		public string SelectedValue
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = value;
			}
		}
		
		public Guid ID
		{
			get
			{
				// 12/03/2005 Paul.  Don't catch the Guid conversion error as this should not happen. 
				Guid gVALUE = Guid.Empty;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					string sVALUE = this.Text;
					if ( !Sql.IsEmptyString(sVALUE) )
						gVALUE = Sql.ToGuid(sVALUE);
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						gVALUE = Sql.ToGuid(rowCurrent[sNAME]);
				}
				return gVALUE;
			}
			set
			{
				this.Text = value.ToString();
			}
		}

		public int IntegerValue
		{
			get
			{
				// 12/03/2005 Paul.  Don't catch the Integer conversion error as this should not happen. 
				int nVALUE = 0;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					string sVALUE = this.Text;
					if ( !Sql.IsEmptyString(sVALUE) )
						nVALUE = Sql.ToInteger(sVALUE);
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						nVALUE = Sql.ToInteger(rowCurrent[sNAME]);
				}
				return nVALUE;
			}
			set
			{
				this.Text = value.ToString();
			}
		}

		public Decimal DecimalValue
		{
			get
			{
				// 12/03/2005 Paul.  Don't catch the Decimal conversion error as this should not happen. 
				Decimal dVALUE = Decimal.Zero;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					string sVALUE = this.Text;
					if ( !Sql.IsEmptyString(sVALUE) )
						dVALUE = Sql.ToDecimal(sVALUE);
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						dVALUE = Sql.ToDecimal(rowCurrent[sNAME]);
				}
				return dVALUE;
			}
			set
			{
				this.Text = value.ToString();
			}
		}

		public float FloatValue
		{
			get
			{
				// 12/03/2005 Paul.  Don't catch the float conversion error as this should not happen. 
				float fVALUE = 0;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					string sVALUE = this.Text;
					if ( !Sql.IsEmptyString(sVALUE) )
						fVALUE = Sql.ToFloat(sVALUE);
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						fVALUE = Sql.ToFloat(rowCurrent[sNAME]);
				}
				return fVALUE;
			}
			set
			{
				this.Text = value.ToString();
			}
		}

		public bool Checked
		{
			get
			{
				bool bVALUE = false;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					if ( ctl is CheckBox )
					{
						CheckBox chk = ctl as CheckBox;
						bVALUE = chk.Checked;
					}
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						bVALUE = Sql.ToBoolean(rowCurrent[sNAME]);
				}
				return bVALUE;
			}
			set
			{
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					if ( ctl is CheckBox )
					{
						CheckBox chk = ctl as CheckBox;
						chk.Checked = value;
					}
				}
			}
		}

		public DateTime DateValue
		{
			get
			{
				DateTime dtVALUE = DateTime.MinValue;
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					TimeZone T10n = ctlPARENT.GetT10n();
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					if ( ctl is TextBox )
					{
						TextBox txt = ctl as TextBox;
						dtVALUE = T10n.ToServerTime(txt.Text);
					}
					// 03/10/2006 Paul.  User controls end in "_ascx".  Compare to the base type. 
					else if ( ctl.GetType().BaseType == typeof(_controls.DatePicker) )
					{
						_controls.DatePicker dt = ctl as _controls.DatePicker;
						dtVALUE = T10n.ToServerTime(dt.Value);
					}
					// 03/10/2006 Paul.  User controls end in "_ascx".  Compare to the base type. 
					else if ( ctl.GetType().BaseType == typeof(_controls.DateTimePicker) )
					{
						_controls.DateTimePicker dt = ctl as _controls.DateTimePicker;
						dtVALUE = T10n.ToServerTime(dt.Value);
					}
					// 03/10/2006 Paul.  User controls end in "_ascx".  Compare to the base type. 
					else if ( ctl.GetType().BaseType == typeof(_controls.DateTimeEdit) )
					{
						_controls.DateTimeEdit dt = ctl as _controls.DateTimeEdit;
						dtVALUE = T10n.ToServerTime(dt.Value);
					}
				}
				else if ( rowCurrent != null )
				{
					// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
					if ( rowCurrent.Table.Columns.Contains(sNAME) )
						dtVALUE = Sql.ToDateTime(rowCurrent[sNAME]);
				}
				return dtVALUE;
			}
			set
			{
				Control ctl = ctlPARENT.FindControl(sNAME);
				if ( ctl != null )
				{
					TimeZone T10n = ctlPARENT.GetT10n();
					// 03/10/2006 Paul.  Compare to the type as .NET 2.0 returns the name in lowercase. We don't want to have this problem again. 
					if ( ctl is TextBox )
					{
						TextBox txt = ctl as TextBox;
						txt.Text = T10n.FromServerTime(value).ToString();
					}
					// 03/10/2006 Paul.  User controls end in "_ascx".  Compare to the base type. 
					else if ( ctl.GetType().BaseType == typeof(_controls.DatePicker) )
					{
						_controls.DatePicker dt = ctl as _controls.DatePicker;
						dt.Value = T10n.FromServerTime(value);
					}
					// 03/10/2006 Paul.  User controls end in "_ascx".  Compare to the base type. 
					else if ( ctl.GetType().BaseType == typeof(_controls.DateTimePicker) )
					{
						_controls.DateTimePicker dt = ctl as _controls.DateTimePicker;
						dt.Value = T10n.FromServerTime(value);
					}
					// 03/10/2006 Paul.  User controls end in "_ascx".  Compare to the base type. 
					else if ( ctl.GetType().BaseType == typeof(_controls.DateTimeEdit) )
					{
						_controls.DateTimeEdit dt = ctl as _controls.DateTimeEdit;
						dt.Value = T10n.FromServerTime(value);
					}
				}
			}
		}

		public override string ToString()
		{
			return this.Text;
		}
		
		public DynamicControl(SplendidControl ctlPARENT, string sNAME)
		{
			this.ctlPARENT  = ctlPARENT ;
			this.sNAME      = sNAME     ;
			this.rowCurrent = null      ;
		}
		
		// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
		public DynamicControl(SplendidControl ctlPARENT, DataRow rowCurrent, string sNAME)
		{
			this.ctlPARENT  = ctlPARENT ;
			this.sNAME      = sNAME     ;
			this.rowCurrent = rowCurrent;
		}

	}
}
