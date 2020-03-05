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
using System.Text;
using System.Xml;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for Chooser.
	/// </summary>
	public class Chooser : SplendidControl
	{
		protected string          sChooserTitle    ;
		protected string          sLeftTitle       ;
		protected string          sRightTitle      ;
		public    HtmlInputHidden txtLeft          ;
		public    HtmlInputHidden txtRight         ;
		public    ListBox         lstLeft          ;
		public    ListBox         lstRight         ;
		protected TableCell       tdSpacerUpDown   ;
		protected TableCell       tdSpacerLeftRight;
		protected TableCell       tdMoveUpDown     ;
		protected TableCell       tdMoveLeftRight  ;

		public string ChooserTitle
		{
			get
			{
				return sChooserTitle;
			}
			set
			{
				sChooserTitle = value;
			}
		}

		public string LeftTitle
		{
			get
			{
				return sLeftTitle;
			}
			set
			{
				sLeftTitle = value;
			}
		}

		public string RightTitle
		{
			get
			{
				return sRightTitle;
			}
			set
			{
				sRightTitle = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return lstLeft.Enabled;
			}
			set
			{
				lstLeft .Enabled = value;
				lstRight.Enabled = value;
				tdSpacerUpDown.Visible    = value;
				tdSpacerLeftRight.Visible = value;
				tdMoveUpDown.Visible      = value;
				tdMoveLeftRight.Visible   = value;
			}
		}

		public ListBox LeftListBox
		{
			get
			{
				return lstLeft;
			}
		}

		public ListBox RightListBox
		{
			get
			{
				return lstRight;
			}
		}

		public DataTable LeftValuesTable
		{
			get
			{
				return ValuesTable(txtLeft.Value);
			}
		}

		public string LeftValues
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				DataTable dt = LeftValuesTable;
				if ( dt != null )
				{
					foreach ( DataRow row in dt.Rows )
					{
						if ( sb.Length > 0 )
							sb.Append(",");
						sb.Append(Sql.ToString(row["value"]));
					}
				}
				return sb.ToString();
			}
		}

		public DataTable RightValuesTable
		{
			get
			{
				return ValuesTable(txtRight.Value);
			}
		}

		public string RightValues
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				DataTable dt = RightValuesTable;
				if ( dt != null )
				{
					foreach ( DataRow row in dt.Rows )
					{
						if ( sb.Length > 0 )
							sb.Append(",");
						sb.Append(Sql.ToString(row["value"]));
					}
				}
				return sb.ToString();
			}
		}

		private DataTable ValuesTable(string sXml)
		{
			DataTable dt = null;
			try
			{
				if ( !Sql.IsEmptyString(sXml) )
				{
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(sXml);
					dt = XmlUtil.CreateDataTable(xml.DocumentElement, "list", new string[] {"text", "value"});
				}
			}
			catch
			{
			}
			return dt;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
