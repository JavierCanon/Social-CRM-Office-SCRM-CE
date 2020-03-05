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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Diagnostics;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for PartsDesigner.
	/// </summary>
	public class PartsDesigner : SplendidControl
	{
		protected DropDownList           lstWebPartMode ;
		protected CatalogZone            zCatalog       ;
		protected EditorZone             zPartsEditor   ;
		protected PageCatalogPart        wpcPage        ;
		protected PropertyGridEditorPart edPropertyGrid ;
		protected AppearanceEditorPart   edAppearance   ;
		protected BehaviorEditorPart     edBehavior     ;
		protected LayoutEditorPart       edLayout       ;

		protected void lstWebPartMode_Changed(Object sender, EventArgs e)
		{
			WebPartManager wpm = WebPartManager.GetCurrentWebPartManager(Page);
			wpm.DisplayMode = wpm.SupportedDisplayModes[lstWebPartMode.SelectedValue];
		}

		protected void WebPartManager_DisplayModeChanged(Object sender, WebPartDisplayModeEventArgs e)
		{
			WebPartManager wpm = WebPartManager.GetCurrentWebPartManager(Page);
			try
			{
				lstWebPartMode.SelectedValue = wpm.DisplayMode.Name;
			}
			catch
			{
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				// 01/11/2008 Paul.  The WebPartsZone has been removed from all areas except the Home and the Dashboard. 
				// Hide the designer control in all other areas. 
				this.Visible = (Page.AppRelativeVirtualPath == "~/Home/Default.aspx" || Page.AppRelativeVirtualPath == "~/Dashboard/Default.aspx");

				try
				{
					WebPartManager wpm = WebPartManager.GetCurrentWebPartManager(Page);
					foreach ( WebPartDisplayMode mode in wpm.SupportedDisplayModes )
					{
						//if ( mode.IsEnabled(wpm) )
						{
							// 09/01/2007 Pierre.  Make sure that the ListItem value is set to the original mode.Name,
							// otherwise we will throw an exception when changing modes. 
							lstWebPartMode.Items.Add(new ListItem(L10n.Term(".web_parts_mode." + mode.Name), mode.Name));
						}
					}
					zCatalog.HeaderText           = L10n.Term("WebParts.LBL_CATALOG_ZONE"       );
					zCatalog.InstructionText      = L10n.Term("WebParts.LBL_CATALOG_INSTRUCTION");
					zCatalog.AddVerb        .Text = L10n.Term("WebParts.LBL_CATALOG_ADD"        );
					zCatalog.CloseVerb      .Text = L10n.Term("WebParts.LBL_CATALOG_CLOSE"      );
					zCatalog.HeaderCloseVerb.Text = L10n.Term("WebParts.LBL_CATALOG_CLOSE"      );
					zCatalog.SelectTargetZoneText = L10n.Term("WebParts.LBL_CATALOG_ADD_TO"     );
					zPartsEditor.HeaderText       = L10n.Term("WebParts.LBL_EDIT0R_ZONE"        );
					zPartsEditor.InstructionText  = L10n.Term("WebParts.LBL_EDIT0R_INSTRUCTION" );
					zPartsEditor.ApplyVerb  .Text = L10n.Term("WebParts.LBL_EDIT0R_APPLY"       );
					zPartsEditor.OKVerb     .Text = L10n.Term("WebParts.LBL_EDIT0R_OK"          );
					zPartsEditor.CancelVerb .Text = L10n.Term("WebParts.LBL_EDIT0R_CANCEL"      );

					// 01/11/2008 Paul.  wpcPage.Title is throwing an exception. It may be because we stopped using parts. 
					wpcPage.Title                 = L10n.Term("WebParts.LBL_CATALOG_PAGE_CATALOG");
					// 01/24/2007 Paul.  The editor controls do not provide a way to customize the terminology. 
					edPropertyGrid.Title = L10n.Term("WebParts.LBL_PROPERTY_TITLE"  );
					edAppearance  .Title = L10n.Term("WebParts.LBL_APPEARANCE_TITLE");
					edBehavior    .Title = L10n.Term("WebParts.LBL_BEHAVIOR_TITLE"  );
					edLayout      .Title = L10n.Term("WebParts.LBL_LAYOUT_TITLE"    );
					//edLayout.GroupingText 
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
			WebPartManager wpm = WebPartManager.GetCurrentWebPartManager(Page);
			wpm.DisplayModeChanged += new WebPartDisplayModeEventHandler(this.WebPartManager_DisplayModeChanged);
		}
		#endregion
	}
}
