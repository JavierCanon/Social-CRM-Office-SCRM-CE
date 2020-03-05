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
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.SyncSchema
{
	/// <summary>
	///		Summary description for SpecifyDatabases.
	/// </summary>
	public class SpecifyDatabases : SyncControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected WizardButtons          ctlWizardButtons;

		protected DropDownList    lstSOURCE_PROVIDER           ;
		protected DropDownList    lstDESTINATION_PROVIDER      ;
		protected TextBox         txtSOURCE_CONNECTION         ;
		protected TextBox         txtDESTINATION_CONNECTION    ;
		protected Label           lblSourceError               ;
		protected Label           lblDestinationError          ;


		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Next" )
			{
				if ( Page.IsValid )
				{
					XmlDocument xml = GetXml();
					if ( xml.DocumentElement != null )
					{
						if( lstSOURCE_PROVIDER.SelectedValue != XmlUtil.SelectSingleNode(xml, "Source/Provider") )
						{
							XmlNode node = xml.DocumentElement.SelectSingleNode("Source");
							if ( node != null )
								xml.DocumentElement.RemoveChild(node);
						}
						if( lstDESTINATION_PROVIDER.SelectedValue != XmlUtil.SelectSingleNode(xml, "Destination/Provider") )
						{
							XmlNode node = xml.DocumentElement.SelectSingleNode("Destination");
							if ( node != null )
								xml.DocumentElement.RemoveChild(node);
						}
					}
					XmlUtil.SetSingleNode(xml, "Source/Provider"             , lstSOURCE_PROVIDER.SelectedValue     );
					XmlUtil.SetSingleNode(xml, "Destination/Provider"        , lstDESTINATION_PROVIDER.SelectedValue);
					XmlUtil.SetSingleNode(xml, "Source/ConnectionString"     , txtSOURCE_CONNECTION.Text            );
					XmlUtil.SetSingleNode(xml, "Destination/ConnectionString", txtDESTINATION_CONNECTION.Text       );
					SetXml(xml);
					Response.Cookies["SyncSchema"]["Source.Provider"             ] = XmlUtil.ToBase64String(lstSOURCE_PROVIDER.SelectedValue     );
					Response.Cookies["SyncSchema"]["Destination.Provider"        ] = XmlUtil.ToBase64String(lstDESTINATION_PROVIDER.SelectedValue);
					Response.Cookies["SyncSchema"]["Source.ConnectionString"     ] = XmlUtil.ToBase64String(txtSOURCE_CONNECTION.Text            );
					Response.Cookies["SyncSchema"]["Destination.ConnectionString"] = XmlUtil.ToBase64String(txtDESTINATION_CONNECTION.Text       );
					Response.Cookies["SyncSchema"].Expires = new DateTime(2010, 1, 1);

					bool bValidSource      = false;
					bool bValidDestination = false;
					try
					{
						if ( txtSOURCE_CONNECTION.Text.Length > 0 )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory(lstSOURCE_PROVIDER.SelectedValue, txtSOURCE_CONNECTION.Text);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								lblSourceError.Text = "Connection successful.";
								bValidSource = true;
							}
						}
					}
					catch(Exception ex)
					{
						lblSourceError.Text = ex.Message;
					}
					try
					{
						if ( txtDESTINATION_CONNECTION.Text.Length > 0 )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory(lstDESTINATION_PROVIDER.SelectedValue, txtDESTINATION_CONNECTION.Text);
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								lblDestinationError.Text = "Connection successful.";
								bValidDestination = true;
							}
						}
					}
					catch(Exception ex)
					{
						lblDestinationError.Text = ex.Message;
					}
					if ( bValidSource && bValidDestination )
					{
						if ( Command != null )
							Command(sender, e);
					}
				}
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
				if ( !IsPostBack )
				{
					ctlWizardButtons.EnablePrevious = false;
					XmlDocument xml = new XmlDocument();
					xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
					xml.AppendChild(xml.CreateElement("SyncSchema"));

					if ( Request.Cookies["SyncSchema"] != null )
					{
						try
						{
							XmlUtil.SetSingleNode(xml, "Source/Provider"        , XmlUtil.FromBase64String(Request.Cookies["SyncSchema"]["Source.Provider"        ]));
							XmlUtil.SetSingleNode(xml, "Source/ConnectionString", XmlUtil.FromBase64String(Request.Cookies["SyncSchema"]["Source.ConnectionString"]));
							lstSOURCE_PROVIDER.SelectedValue      = XmlUtil.SelectSingleNode(xml, "Source/Provider");
						}
						catch
						{
						}
						try
						{
							XmlUtil.SetSingleNode(xml, "Destination/Provider"        , XmlUtil.FromBase64String(Request.Cookies["SyncSchema"]["Destination.Provider"        ]));
							XmlUtil.SetSingleNode(xml, "Destination/ConnectionString", XmlUtil.FromBase64String(Request.Cookies["SyncSchema"]["Destination.ConnectionString"]));
							lstDESTINATION_PROVIDER.SelectedValue = XmlUtil.SelectSingleNode(xml, "Destination/Provider");
						}
						catch
						{
						}
						txtSOURCE_CONNECTION.Text      = XmlUtil.SelectSingleNode(xml, "Source/ConnectionString"     );
						txtDESTINATION_CONNECTION.Text = XmlUtil.SelectSingleNode(xml, "Destination/ConnectionString");
					}
					SetXml(xml);
				}
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
