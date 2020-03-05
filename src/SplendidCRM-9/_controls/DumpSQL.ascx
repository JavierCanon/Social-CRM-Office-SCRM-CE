<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DumpSQL.ascx.cs" Inherits="SplendidCRM._controls.DumpSQL" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%
if ( bDebug )
	{
	%>
	<script type="text/javascript">
		if ( sDebugSQL != 'undefined' )
		{
			document.write('<pre>');
			document.write(sDebugSQL);
			document.write('</pre>');
		}
	</script>
	<%
	}
%>
