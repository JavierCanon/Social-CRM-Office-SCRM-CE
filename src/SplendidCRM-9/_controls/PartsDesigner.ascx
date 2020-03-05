<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PartsDesigner.ascx.cs" Inherits="SplendidCRM._controls.PartsDesigner" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%= L10n.Term("WebParts.LBL_WEB_PARTS_MODE") %><br /><asp:DropDownList ID="lstWebPartMode" OnSelectedIndexChanged="lstWebPartMode_Changed" AutoPostBack="true" runat="server" /><br />
<asp:EditorZone ID="zPartsEditor" runat="server">
	<ZoneTemplate>
		<asp:PropertyGridEditorPart ID="edPropertyGrid" runat="server" />
		<asp:AppearanceEditorPart   ID="edAppearance"   runat="server" />
		<asp:BehaviorEditorPart     ID="edBehavior"     runat="server" />
		<asp:LayoutEditorPart       ID="edLayout"       runat="server" />
	</ZoneTemplate>
</asp:EditorZone>
<asp:CatalogZone ID="zCatalog" runat="server">
	<ZoneTemplate>
		<asp:PageCatalogPart ID="wpcPage" runat="server" />
	</ZoneTemplate>
</asp:CatalogZone>
