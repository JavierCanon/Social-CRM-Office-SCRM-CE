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

namespace SplendidCRM.Themes.Sugar
{
	/// <summary>
	///		Summary description for ModuleHeader.
	/// </summary>
	public class ModuleHeader : SplendidControl
	{
		protected string    sModule   = String.Empty;
		protected string    sTitle    = String.Empty;
		protected string    sHelpName = String.Empty;
		protected bool      bEnableModuleLabel = true;
		protected bool      bEnablePrint;
		protected bool      bEnableHelp ;
		protected Label     lblTitle    ;
		protected HyperLink lnkHelpImage;
		protected HyperLink lnkHelpText ;

		public string Module
		{
			get
			{
				return sModule;
			}
			set
			{
				sModule = value;
			}
		}

		public string Title
		{
			get
			{
				return sTitle;
			}
			set
			{
				sTitle = value;
			}
		}

		public string HelpName
		{
			get
			{
				return sHelpName;
			}
			set
			{
				sHelpName = value;
			}
		}

		// 09/03/2006 Paul.  Import needs to update the text directly. 
		public string TitleText
		{
			get
			{
				if ( lblTitle != null )
					return lblTitle.Text;
				else
					return String.Empty;
			}
			set
			{
				if ( lblTitle != null )
					lblTitle.Text = value;
			}
		}

		public bool EnableModuleLabel
		{
			get
			{
				return bEnableModuleLabel;
			}
			set
			{
				bEnableModuleLabel = value;
			}
		}

		public bool EnablePrint
		{
			get
			{
				return bEnablePrint;
			}
			set
			{
				bEnablePrint = value;
			}
		}

		public bool EnableHelp
		{
			get
			{
				return bEnableHelp;
			}
			set
			{
				bEnableHelp = value;
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Print" )
			{
				PrintView = true;
				// 06/09/2006 Paul.  This is an exception to the new binding rule.  We want to rebind to apply the PrintView change. 
				Page.DataBind();
			}
			else if ( e.CommandName == "PrintOff" )
			{
				PrintView = false;
				// 06/09/2006 Paul.  This is an exception to the new binding rule.  We want to rebind to apply the PrintView change. 
				Page.DataBind();
			}
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( lblTitle != null )
				lblTitle.Text = (bEnableModuleLabel ? L10n.Term(".moduleList." + sModule) + ": " : "") + L10n.Term(sTitle);
			if ( bEnableHelp )
			{
				if ( !Sql.IsEmptyString(sHelpName) )
				{
					if ( lnkHelpImage != null )
						lnkHelpImage.NavigateUrl = "~/Help/view.aspx?MODULE=" + sModule + "&NAME=" + sHelpName;
					if ( lnkHelpText != null )
					{
						lnkHelpText .NavigateUrl = lnkHelpImage.NavigateUrl;
						// 10/25/2006 Paul.  There is a config flag to disable the wiki entirely. 
						if ( (SplendidCRM.Security.GetUserAccess("Help", "edit") >= 0) && Sql.ToBoolean(Application["CONFIG.enable_help_wiki"]) )
							lnkHelpText .Text = L10n.Term(".LNK_HELP_WIKI");
						else
							lnkHelpText .Text = L10n.Term(".LNK_HELP");
					}
				}
				else
				{
					bEnableHelp = false;
				}
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
