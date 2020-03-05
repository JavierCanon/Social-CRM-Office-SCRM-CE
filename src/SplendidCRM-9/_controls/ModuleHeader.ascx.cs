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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for ModuleHeader.
	/// </summary>
	public class ModuleHeader : SplendidControl
	{
		protected SplendidCRM.Themes.Sugar.ModuleHeader ctlModuleHeader;
		protected Panel     pnlHeader;
		protected string    sModule   = String.Empty;
		protected string    sTitle    = String.Empty;
		protected string    sHelpName = String.Empty;
		protected string    sTitleText= String.Empty;
		protected bool      bEnableModuleLabel = true;
		protected bool      bEnablePrint       = false;
		protected bool      bEnableHelp        = false;

		public string Module
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.Module;
				else
					return sModule;
			}
			set
			{
				sModule = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.Module = value;
			}
		}

		public string Title
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.Title;
				else
					return sTitle;
			}
			set
			{
				sTitle = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.Title = value;
			}
		}

		public string HelpName
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.HelpName;
				else
					return sHelpName;
			}
			set
			{
				sHelpName = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.HelpName = value;
			}
		}

		public string TitleText
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.TitleText;
				else
					return sTitleText;
			}
			set
			{
				sTitleText = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.TitleText = value;
			}
		}

		public bool EnableModuleLabel
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnableModuleLabel;
				else
					return bEnableModuleLabel;
			}
			set
			{
				bEnableModuleLabel = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnableModuleLabel = value;
			}
		}

		public bool EnablePrint
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnablePrint;
				else
					return bEnablePrint;
			}
			set
			{
				bEnablePrint = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnablePrint = value;
			}
		}

		public bool EnableHelp
		{
			get
			{
				if ( ctlModuleHeader != null )
					return ctlModuleHeader.EnableHelp;
				else
					return bEnableHelp;
			}
			set
			{
				bEnableHelp = value;
				if ( ctlModuleHeader != null )
					ctlModuleHeader.EnableHelp = value;
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
			string sTheme = Page.Theme;
			if ( String.IsNullOrEmpty(sTheme) )
				sTheme = "Sugar";
			string sModuleHeaderPath = "~/App_MasterPages/" + Page.Theme + "/ModuleHeader.ascx";
			if ( System.IO.File.Exists(MapPath(sModuleHeaderPath)) )
			{
				ctlModuleHeader = LoadControl(sModuleHeaderPath) as SplendidCRM.Themes.Sugar.ModuleHeader;
				if ( ctlModuleHeader != null )
				{
					ctlModuleHeader.Module            = sModule           ;
					ctlModuleHeader.Title             = sTitle            ;
					ctlModuleHeader.HelpName          = sHelpName         ;
					ctlModuleHeader.TitleText         = sTitleText        ;
					ctlModuleHeader.EnableModuleLabel = bEnableModuleLabel;
					ctlModuleHeader.EnablePrint       = bEnablePrint      ;
					ctlModuleHeader.EnableHelp        = bEnableHelp       ;
					pnlHeader.Controls.Add(ctlModuleHeader);
				}
			}
		}
		#endregion
	}
}
