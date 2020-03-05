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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.SyncSchema
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : SplendidPage
	{
		protected PlaceHolder     plcSyncStep;
		protected XmlDocument     xml        ;
		protected HtmlInputHidden txtStep    ;
		protected HtmlInputHidden txtXML     ;

		protected void SetStep(string sStep)
		{
			SyncControl ctlStep = null;
			switch ( sStep )
			{
				case "SpecifyDatabases" :  ctlStep = (SyncControl) LoadControl("SpecifyDatabases.ascx" );  break;
				case "VerifyTables"     :  ctlStep = (SyncControl) LoadControl("VerifyTables.ascx"     );  break;
				case "VerifyColumns"    :  ctlStep = (SyncControl) LoadControl("VerifyColumns.ascx"    );  break;
				case "VerifyViews"      :  ctlStep = (SyncControl) LoadControl("VerifyViews.ascx"      );  break;
				case "VerifyProcedures" :  ctlStep = (SyncControl) LoadControl("VerifyProcedures.ascx" );  break;
				case "VerifyFunctions"  :  ctlStep = (SyncControl) LoadControl("VerifyFunctions.ascx"  );  break;
				case "Summary"          :  ctlStep = (SyncControl) LoadControl("SpecifyDatabases.ascx" );  break;
				default                 :  ctlStep = (SyncControl) LoadControl("SpecifyDatabases.ascx" );  break;
			}
			plcSyncStep.Controls.Clear();
			plcSyncStep.Controls.Add(ctlStep);
			ctlStep.Command = new CommandEventHandler(Page_Command);
			txtStep.Value = sStep;
			// 07/10/2006 Paul.  We can't bind here if SetStep() is to be called within InitializeComponent(). 
			//Page.DataBind();
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				XmlDocument xml = new XmlDocument();
				try
				{
					xml.LoadXml(Server.HtmlDecode(txtXML.Value));
				}
				catch
				{
				}
				string sStep = Sql.ToString(txtStep.Value);
				if ( e.CommandName == "Next" )
				{
					switch ( sStep )
					{
						case "Summary"          :  sStep = "VerifyTables"     ;  break;
						case ""                 :  sStep = "VerifyTables"     ;  break;
						case "SpecifyDatabases" :  sStep = "VerifyTables"     ;  break;
						case "VerifyTables"     :  sStep = "VerifyColumns"    ;  break;
						case "VerifyColumns"    :  sStep = "VerifyViews"      ;  break;
						case "VerifyViews"      :  sStep = "VerifyProcedures" ;  break;
						case "VerifyProcedures" :  sStep = "VerifyFunctions"  ;  break;
						case "VerifyFunctions"  :  sStep = "Summary"          ;  break;
					}
				}
				else if ( e.CommandName == "Previous" )
				{
					switch ( sStep )
					{
						case "Summary"          :  sStep = "SpecifyDatabases" ;  break;
						case ""                 :  sStep = "SpecifyDatabases" ;  break;
						case "VerifyTables"     :  sStep = "SpecifyDatabases" ;  break;
						case "VerifyColumns"    :  sStep = "VerifyTables"     ;  break;
						case "VerifyViews"      :  sStep = "VerifyColumns"    ;  break;
						case "VerifyProcedures" :  sStep = "VerifyViews"      ;  break;
						case "VerifyFunctions"  :  sStep = "VerifyProcedures" ;  break;
					}
				}
				XmlUtil.SetSingleNode(xml, "Step", sStep);
				txtXML.Value = Server.HtmlEncode(xml.OuterXml);
				SetStep(sStep);
				// 07/10/2006 Paul.  Move DataBind here. 
				Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 02/13/2007 Paul.  The Shortcuts are in the master page. 
			Utils.AdminShortcuts(this, true);
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
				Page.DataBind();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.IsAdminPage = true;
			this.Load += new System.EventHandler(this.Page_Load);
			string sStep = Sql.ToString(Request[txtStep.ClientID]);
			SetStep(sStep);
		}
		#endregion
	}
}
