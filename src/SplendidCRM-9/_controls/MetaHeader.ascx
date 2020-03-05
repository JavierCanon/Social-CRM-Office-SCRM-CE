<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MetaHeader.ascx.cs" Inherits="SplendidCRM._controls.MetaHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script runat="server">
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
	<link rel="SHORTCUT ICON" href="<%= Application["imageURL"] %>SplendidCRM_Icon.ico">
	<script runat="server">
		// 03/07/2007 Paul.  Stylesheet files are now automatically added, so remove them from here. 
	</script>
	<script type="text/javascript" src="<%= Session["themeURL"]      %>menu.js"></script>
	<script type="text/javascript" src="<%= Application["scriptURL"] %>sugar_3.js"></script>
	<script type="text/javascript" src="<%= Application["scriptURL"] %>cookie.js"></script>
	<script type="text/javascript" src="<%= Application["scriptURL"] %>SplendidCRM.js"></script>
	<script type="text/javascript">
		// 05/07/2005 Paul.  Using startHighlight() causes document.getElementById to return null. 
		// 12/29/2005 Paul.  Demo went poorly because IE kept crashing.  It might be because of the highlighting. 
		//window.onload = startHighlight;
		
		// 11/14/2005 Paul.  Trap the ENTER key at the document level so that the default action can be cancelled. 
		// Use Utils.RegisterEnterKeyPress() to enable the ENTER key in any simulated sub-form. 
		document.onkeypress = function()
		{
			if ( (event.which ? event.which : event.keyCode) == 13 )
			{
				// 11/15/2005 Paul.  We need to allow the ENTER key for multi-line edit controls. 
				if ( event.srcElement.type == 'textarea' )
				{
					if ( event.srcElement.rows > 1 )
						return;
					// 11/19/2005 Paul.  The ENTER key should work on buttons and images, so only block if on a textbox. 
					event.returnValue = false;
					event.cancel = true;
				}
			}
		}
	</script>
