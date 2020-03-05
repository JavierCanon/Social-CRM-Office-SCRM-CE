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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.SyncSchema
{
	/// <summary>
	///		Summary description for VerifyColumns.
	/// </summary>
	public class VerifyColumns : SyncControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected WizardButtons          ctlWizardButtons;

		protected Label   lblSOURCE_PROVIDER       ;
		protected Label   lblDESTINATION_PROVIDER  ;
		protected Label   lblSOURCE_CONNECTION     ;
		protected Label   lblDESTINATION_CONNECTION;
		protected Label   lblSourceError           ;
		protected Label   lblDestinationError      ;
		protected Literal litSOURCE_UNIQUE         ;
		protected Literal litDESTINATION_UNIQUE    ;
		protected Literal litSOURCE_LIST           ;
		protected Literal litDESTINATION_LIST      ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				if ( Command != null )
					Command(sender, e);
			}
			else if ( e.CommandName == "Previous" )
			{
				if ( Command != null )
					Command(sender, e);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				SetPageTitle(L10n.Term(".moduleList.Administration"));
				XmlDocument xml = GetXml();

				lblSOURCE_PROVIDER.Text        = XmlUtil.SelectSingleNode(xml, "Source/Provider"             );
				lblDESTINATION_PROVIDER.Text   = XmlUtil.SelectSingleNode(xml, "Destination/Provider"        );
				lblSOURCE_CONNECTION.Text      = XmlUtil.SelectSingleNode(xml, "Source/ConnectionString"     );
				lblDESTINATION_CONNECTION.Text = XmlUtil.SelectSingleNode(xml, "Destination/ConnectionString");

				XmlNode nodeSource      = xml.DocumentElement.SelectSingleNode("Source"     );
				XmlNode nodeDestination = xml.DocumentElement.SelectSingleNode("Destination");
				try
				{
					litSOURCE_LIST.Text = LoadNames(nodeSource, "Columns", "Column", lblSOURCE_PROVIDER.Text, lblSOURCE_CONNECTION.Text, GetColumnsCommand(lblSOURCE_PROVIDER.Text));
				}
				catch(Exception ex)
				{
					lblSourceError.Text = ex.Message;
				}
				try
				{
					litDESTINATION_LIST.Text = LoadNames(nodeDestination, "Columns", "Column", lblDESTINATION_PROVIDER.Text, lblDESTINATION_CONNECTION.Text, GetColumnsCommand(lblDESTINATION_PROVIDER.Text));
				}
				catch(Exception ex)
				{
					lblDestinationError.Text = ex.Message;
				}
				StringBuilder sbSourceUnique      = new StringBuilder();
				StringBuilder sbDestinationUnique = new StringBuilder();
				CompareNames(nodeSource, nodeDestination, "Columns", "Column", ref sbSourceUnique, ref sbDestinationUnique);
				litSOURCE_UNIQUE.Text = sbSourceUnique.ToString();
				litDESTINATION_UNIQUE.Text = sbDestinationUnique.ToString();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlWizardButtons.ErrorText = ex.Message;
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
			ctlWizardButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
