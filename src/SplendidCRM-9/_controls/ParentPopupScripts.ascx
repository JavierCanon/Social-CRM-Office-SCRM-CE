<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ParentPopupScripts.ascx.cs" Inherits="SplendidCRM._controls.ParentPopupScripts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
	<script type="text/javascript">
	function ChangeParent(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, sHiddenField).ClientID %>').value = sPARENT_ID  ;
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, sNameField  ).ClientID %>').value = sPARENT_NAME;
	}
	function ParentTypePopup(sPARENT_TYPE)
	{
		return window.open('<%= Application["rootURL"] %>' + sPARENT_TYPE + '/Popup.aspx',sPARENT_TYPE + 'Popup','width=600,height=400,resizable=1,scrollbars=1');
	}
	
	var ChangeAccount     = ChangeParent;
	var ChangeBug         = ChangeParent;
	var ChangeCase        = ChangeParent;
	var ChangeContact     = ChangeParent;
	var ChangeLead        = ChangeParent;
	var ChangeOpportunity = ChangeParent;
	var ChangeProject     = ChangeParent;
	var ChangeProjectTask = ChangeParent;
	var ChangeTask        = ChangeParent;
	var ChangeContract    = ChangeParent;
	// 07/04/2007 Paul.  Add support for a bunch of new parent modules. 
	var ChangeProspect    = ChangeParent;
	var ChangeCall        = ChangeParent;
	var ChangeMeeting     = ChangeParent;
	var ChangeProduct     = ChangeParent;
	var ChangeQuote       = ChangeParent;
	var ChangeOrder       = ChangeParent;
	var ChangeInvoice     = ChangeParent;

	function ParentPopup()
	{
		// 08/28/2006 Paul.  We need to be able to change the function pointers. So, don't assume that they are pointing to the expected function. 
		// We discovered this need when editing Notes.  A note has a parent and a Contact relationship. 
		ChangeAccount     = ChangeParent;
		ChangeBug         = ChangeParent;
		ChangeCase        = ChangeParent;
		ChangeContact     = ChangeParent;
		ChangeLead        = ChangeParent;
		ChangeOpportunity = ChangeParent;
		ChangeProject     = ChangeParent;
		ChangeProjectTask = ChangeParent;
		ChangeTask        = ChangeParent;
		ChangeContract    = ChangeParent;
		// 07/04/2007 Paul.  Add support for a bunch of new parent modules. 
		ChangeProspect    = ChangeParent;
		ChangeCall        = ChangeParent;
		ChangeMeeting     = ChangeParent;
		ChangeProduct     = ChangeParent;
		ChangeQuote       = ChangeParent;
		ChangeOrder       = ChangeParent;
		ChangeInvoice     = ChangeParent;
		
		var lst = document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, sListField).ClientID %>');
		var sPARENT_TYPE = lst.options[lst.options.selectedIndex].value;
		switch(sPARENT_TYPE)
		{
			case 'Accounts'     :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Bugs'         :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Cases'        :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Contacts'     :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Leads'        :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Opportunities':  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Project'      :  ParentTypePopup('Projects'    );  break;
			case 'ProjectTask'  :  ParentTypePopup('ProjectTasks');  break;
			case 'Tasks'        :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Contracts'    :  ParentTypePopup(sPARENT_TYPE  );  break;
			// 07/04/2007 Paul.  Add support for a bunch of new parent modules. 
			case 'Prospects'    :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Calls'        :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Meetings'     :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Products'     :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Quotes'       :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Orders'       :  ParentTypePopup(sPARENT_TYPE  );  break;
			case 'Invoices'     :  ParentTypePopup(sPARENT_TYPE  );  break;
			default:
				alert('Unknown type. Add ' + sPARENT_TYPE + ' to _controls/ParentPopupScripts.ascx');
				break;
		}
	}
	function ChangeParentType()
	{
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, sHiddenField).ClientID %>').value = '';
		document.getElementById('<%= new DynamicControl(this.Parent as SplendidControl, sNameField  ).ClientID %>').value = '';
	}
	</script>
