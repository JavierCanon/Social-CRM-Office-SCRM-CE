<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TeamAssignedPopupScripts.ascx.cs" Inherits="SplendidCRM._controls.TeamAssignedPopupScripts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
	<script type="text/javascript">
	function ChangeUser(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, "ASSIGNED_USER_ID").ClientID %>').value = sPARENT_ID  ;
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, "ASSIGNED_TO"     ).ClientID %>').value = sPARENT_NAME;
	}
	function UserPopup()
	{
		return window.open('../Users/Popup.aspx','UserPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	function ChangeTeam(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, "TEAM_ID"  ).ClientID %>').value = sPARENT_ID  ;
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, "TEAM_NAME").ClientID %>').value = sPARENT_NAME;
	}
	function TeamPopup()
	{
		return window.open('../Administration/Teams/Popup.aspx','TeamPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
