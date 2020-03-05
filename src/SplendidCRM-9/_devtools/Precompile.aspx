<%@ Page language="c#" Codebehind="Precompile.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.Precompile" %>
<head visible="false" runat="server" />
<html>
<head>
	<title>Precompile</title>
<script type="text/javascript">
function RequestObject()
{
	var req = null;
	if ( window.XMLHttpRequest && !(window.ActiveXObject) )
	{
		// branch for native XMLHttpRequest object
		try
		{
			req = new XMLHttpRequest();
		}
		catch(e)
		{
			req = null;
		}
	}
	else if ( window.ActiveXObject )
	{
		// branch for IE/Windows ActiveX version
		try
		{
			req = new ActiveXObject("Msxml2.XMLHTTP");
		}
		catch(e)
		{
			try
			{
				req = new ActiveXObject("Microsoft.XMLHTTP");
			}
			catch(e)
			{
				req = null;
			}
		}
	}
	return req;
}

function loadXMLDoc(url)
{
	// http://developer.apple.com/internet/webcontent/xmlhttpreq.html
	var req = RequestObject();
	if ( req != null )
	{
		req.onreadystatechange = function()
		{
			var lblStatus = document.getElementById('<%= lblStatus.ClientID %>');
			var lnkTest   = document.getElementById('lnkTest');
			var divPrecompileOutput = document.getElementById('divPrecompileOutput');
			lblStatus.innerHTML = req.readyState;
			/*	readyState
				0 = uninitialized
				1 = loading
				2 = loaded
				3 = interactive
				4 = complete
			*/
			// only if req shows "loaded"
			switch ( req.readyState )
			{
				case 0:  lblStatus.innerHTML = 'uninitialized';  break;
				case 1:  lblStatus.innerHTML = 'loading'      ;  break;
				case 2:  lblStatus.innerHTML = 'loaded'       ;  break;
				case 3:  lblStatus.innerHTML = 'interactive'  ;  break;
				case 4:  lblStatus.innerHTML = 'complete'     ;  break;
			}
			if ( req.readyState == 4 )
			{
				// only if "OK"
				if ( req.status == 200 )
				{
					//divPrecompileOutput.innerHTML = req.responseText;
					divPrecompileOutput.innerHTML = url + ' successful';
					DoPrecompile();
				}
				else
				{
					lblStatus.innerHTML = 'There was a problem: ' + req.statusText;
					lnkTest.href = url;
					lnkTest.innerHTML = url.replace('/', '/ ');
					//lnkTest.onclick = 'javascript:window.open(\'' + url + '\', \'_new\', \'addressbar=yes,menubar=yes,scrollbars=yes,resizable=yes,top=0,width=580\');';

					divPrecompileOutput.innerHTML = req.responseText;
				}
			}
		}
		req.open("GET", url, true);
		req.send("");
	}
}

var nLastItem = 0;
var bContinuePrecompile = true;

function DoPrecompile()
{
	var lstFiles      = document.getElementById('<%= lstFiles.ClientID   %>');
	var lblRoot       = document.getElementById('<%= lblRoot.ClientID    %>');
	var lblCurrent    = document.getElementById('<%= lblCurrent.ClientID %>');
	var lblStatus     = document.getElementById('<%= lblStatus.ClientID  %>');
	var divScratchPad = document.getElementById('divScratchPad');
	if ( bContinuePrecompile && nLastItem < lstFiles.options.length )
	{
		lblCurrent.innerHTML = lstFiles.options[nLastItem].value.replace('/', '/ ');
		//divScratchPad.innerHTML = lblRoot.innerHTML + lblCurrent.innerHTML;
		lstFiles.options[nLastItem].selected = true;
		loadXMLDoc(lblRoot.innerHTML + lstFiles.options[nLastItem].value);
		nLastItem++;
	}
	else if ( !bContinuePrecompile )
	{
		lblStatus.innerHTML += '; Compile stopped';
	}
	else if ( nLastItem >= lstFiles.options.length )
	{
		lblStatus.innerHTML += '; End of list';
	}
}

function StopPrecompile()
{
	bContinuePrecompile = false;
}

function ContinuePrecompile()
{
	bContinuePrecompile = true;
	DoPrecompile();
}
</script>
</head>
<body onload="DoPrecompile();" style="margin: 0px 0px 0px 0px;">
<form runat="server">
<table width="100%" height="100%" border="1" cellpadding="0" cellspacing="0">
	<tr>
		<td width="10%" valign="top">
			<asp:Label ID="lblRoot"    runat="server" /><br />
			<asp:Label ID="lblCurrent" runat="server" /><br />
			<asp:Label ID="lblStatus"  runat="server" /><br />
			<a id="lnkTest" href="" target="PrecompileTest"></a><br />
			<button onclick='StopPrecompile();'>Cancel</button>
			<button onclick='ContinuePrecompile();'>Continue</button><br />
			<asp:ListBox ID="lstFiles" DataTextField="NAME" DataValueField="NAME" SelectionMode="Single" Width="200px" Height="600px" runat="server" />
			<asp:Label ID="lblErrors"  runat="server" /><br />
		</td>
		<td valign="top">
			<div id="divPrecompileOutput" />
		</td>
	</tr>
</table>
</form>
</body>
</html>
