var sDebugSQL = '';

startHighlight = function()
{
	if (document.all && document.getElementById)
	{
		navRoot = document.getElementById('ctlListView_grdMain');  // 'grdMain'
		
		// Get a reference to the TBODY element 
		if ( navRoot != null )
		{
			tbody = navRoot.childNodes[0];
			
			for (i = 2; i < tbody.childNodes.length; i++)
			{
				node = tbody.childNodes[i];
				if (node.nodeName == 'TR')
				{
					node.onmouseover=function()
					{
						this.className += '_hilite';
					}
						
					node.onmouseout=function()
					{
						this.className = this.className.replace('_hilite', '');
					}
				}
			}
		}
	}
}

var ChangeDate = null;

function CalendarPopup(ctlDate, clientX, clientY)
{
	clientX = window.screenLeft + parseInt(clientX);
	clientY = window.screenTop  + parseInt(clientY);
	if ( clientX < 0 )
		clientX = 0;
	if ( clientY < 0 )
		clientY = 0;
	return window.open('../Calendar/Popup.aspx?Date=' + ctlDate.value,'CalendarPopup','width=193,height=155,resizable=1,scrollbars=0,left=' + clientX + ',top=' + clientY);
	/*
	var sFeatures = 'dialogHeight:215px ;dialogWidth:253px;resizable:yes;center:yes;status:no;help:no;scroll:no;';
	var sUrl = '<%= Application["rootURL"] %>Calendar/CalendarPopup.aspx?Date=' + ctlDate.value;
	var lookupItems = window.showModalDialog(sUrl, null, sFeatures);
	if ( lookupItems != null )
	{
		ctlDate.value = lookupItems;
	}
	*/
}

function SelectOption(sID, sValue)
{
	var lst = document.forms[0][sID];
	if ( lst != null )
	{
		if ( lst.options != null )
		{
			for ( i=0; i < lst.options.length ; i++ )
			{
				if ( lst.options[i].value == sValue )
				{
					lst.options[i].selected = true;
					break;
				}
			}
		}
	}
}
