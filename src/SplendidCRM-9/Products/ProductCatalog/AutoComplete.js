// AutoComplete.js

function FormatCurrency(num)
{
	return formatNumber(num, num_grp_sep, dec_sep, 2, 2);
}

function GetCurrencyID(sCURRENCY_ID)
{
	// 03/15/2007 Paul.  The Price Level is required to lookup the item price.
	var sCURRENCY = '';
	var fldCURRENCY = document.getElementById(sCURRENCY_ID);
	if ( fldCURRENCY != null )
		sCURRENCY = fldCURRENCY.options[fldCURRENCY.selectedIndex].value;
	return sCURRENCY;
}

function ItemNameChanged(sCURRENCY_ID, fldNAME)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	var userContext = fldNAME.id.replace('NAME', '');
	
	var fldPREVIOUS_NAME = document.getElementById(userContext + 'PREVIOUS_NAME');
	if ( fldPREVIOUS_NAME.value != fldNAME.value )
	{
		if ( fldNAME.value.length > 0 )
			SplendidCRM.Products.ProductCatalog.AutoComplete.GetItemDetailsByName(GetCurrencyID(sCURRENCY_ID), fldNAME.value, ItemChanged_OnSucceededWithContext, ItemChanged_OnFailed, userContext);
	}
}

function ItemPartNumberChanged(sCURRENCY_ID, fldMFT_PART_NUM)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	var userContext = fldMFT_PART_NUM.id.replace('MFT_PART_NUM', '');

	var fldLastLineItemPartNumber = document.getElementById(userContext + 'PREVIOUS_MFT_PART_NUM');
	if ( fldLastLineItemPartNumber.value != fldMFT_PART_NUM.value )
	{
		if ( fldMFT_PART_NUM.value.length > 0 )
			SplendidCRM.Products.ProductCatalog.AutoComplete.GetItemDetailsByNumber(GetCurrencyID(sCURRENCY_ID), fldMFT_PART_NUM.value, ItemChanged_OnSucceededWithContext, ItemChanged_OnFailed, userContext);
	}
}

function ItemChanged_OnSucceededWithContext(result, userContext, methodName)
{
	if ( result != null )
	{
		var sID              = result.ID             ;
		var sNAME            = result.NAME           ;
		var sMFT_PART_NUM    = result.MFT_PART_NUM   ;
		var sVENDOR_PART_NUM = result.VENDOR_PART_NUM;
		var sTAX_CLASS       = result.TAX_CLASS      ;
		var dCOST_PRICE      = result.COST_PRICE     ;
		var dCOST_USDOLLAR   = result.COST_USDOLLAR  ;
		var dLIST_PRICE      = result.LIST_PRICE     ;
		var dLIST_USDOLLAR   = result.LIST_USDOLLAR  ;
		var dUNIT_PRICE      = result.UNIT_PRICE     ;
		var dUNIT_USDOLLAR   = result.UNIT_USDOLLAR  ;
		
		var fldAjaxErrors          = document.getElementById('AjaxErrors');
		var fldPRODUCT_TEMPLATE_ID = document.getElementById(userContext + 'PRODUCT_TEMPLATE_ID');
		var fldNAME                = document.getElementById(userContext + 'NAME'               );
		var fldMFT_PART_NUM        = document.getElementById(userContext + 'MFT_PART_NUM'       );
		var fldVENDOR_PART_NUM     = document.getElementById(userContext + 'VENDOR_PART_NUM'    );
		var fldTAX_CLASS           = document.getElementById(userContext + 'TAX_CLASS'          );
		var fldCOST_PRICE          = document.getElementById(userContext + 'COST_PRICE'         );
		var fldCOST_USDOLLAR       = document.getElementById(userContext + 'COST_USDOLLAR'      );
		var fldLIST_PRICE          = document.getElementById(userContext + 'LIST_PRICE'         );
		var fldLIST_USDOLLAR       = document.getElementById(userContext + 'LIST_USDOLLAR'      );
		var fldUNIT_PRICE          = document.getElementById(userContext + 'UNIT_PRICE'         );
		var fldUNIT_USDOLLAR       = document.getElementById(userContext + 'UNIT_USDOLLAR'      );
		if ( fldPRODUCT_TEMPLATE_ID != null ) fldPRODUCT_TEMPLATE_ID.value = sID             ;
		if ( fldNAME                != null ) fldNAME.value                = sNAME           ;
		if ( fldMFT_PART_NUM        != null ) fldMFT_PART_NUM.value        = sMFT_PART_NUM   ;
		if ( fldVENDOR_PART_NUM     != null ) fldVENDOR_PART_NUM.value     = sVENDOR_PART_NUM;
		if ( fldCOST_PRICE          != null ) fldCOST_PRICE.value          = FormatCurrency(dCOST_PRICE);
		if ( fldCOST_USDOLLAR       != null ) fldCOST_USDOLLAR.value       = dCOST_USDOLLAR  ;
		if ( fldLIST_PRICE          != null ) fldLIST_PRICE.value          = FormatCurrency(dLIST_PRICE);
		if ( fldLIST_USDOLLAR       != null ) fldLIST_USDOLLAR.value       = dLIST_USDOLLAR  ;
		if ( fldUNIT_PRICE          != null ) fldUNIT_PRICE.value          = FormatCurrency(dUNIT_PRICE);
		if ( fldUNIT_USDOLLAR       != null ) fldUNIT_USDOLLAR.value       = dUNIT_USDOLLAR  ;
		if ( fldTAX_CLASS           != null )
		{
			var lst = fldTAX_CLASS;
			if ( lst.options != null )
			{
				for ( i=0; i < lst.options.length ; i++ )
				{
					if ( lst.options[i].value == sTAX_CLASS )
					{
						lst.options[i].selected = true;
						break;
					}
				}
			}
		}

		var fldQUANTITY               = document.getElementById(userContext + 'QUANTITY'             );
		var fldEXTENDED_PRICE         = document.getElementById(userContext + 'EXTENDED_PRICE'       );
		var fldEXTENDED_USDOLLAR      = document.getElementById(userContext + 'EXTENDED_USDOLLAR'    );
		var fldPREVIOUS_MFT_PART_NUM  = document.getElementById(userContext + 'PREVIOUS_MFT_PART_NUM');
		var fldPREVIOUS_NAME          = document.getElementById(userContext + 'PREVIOUS_NAME'        );
		if ( fldPREVIOUS_MFT_PART_NUM  != null ) fldPREVIOUS_MFT_PART_NUM.value = sMFT_PART_NUM   ;
		if ( fldPREVIOUS_NAME          != null ) fldPREVIOUS_NAME.value         = sNAME           ;

		if ( fldQUANTITY != null && fldEXTENDED_PRICE != null )
		{
			var nQUANTITY = parseInt(fldQUANTITY.value);
			// 03/29/2007 Paul.  Initialize the quantity. 
			if ( isNaN(nQUANTITY) )
			{
				nQUANTITY = 1;
				fldQUANTITY.value = nQUANTITY;
			}
			if ( !isNaN(nQUANTITY) )
			{
				if ( !isNaN(dUNIT_PRICE) )
				{
					fldEXTENDED_PRICE.value    = FormatCurrency(nQUANTITY * dUNIT_PRICE);
				}
				if ( !isNaN(dUNIT_USDOLLAR) )
				{
					fldEXTENDED_USDOLLAR.value = nQUANTITY * dUNIT_USDOLLAR;
				}
			}
			fldQUANTITY.focus();
		}
	}
	else
	{
		alert('result from AutoComplete service is null');
	}
}
function ItemChanged_OnFailed(error, userContext)
{
	// Display the error.
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = 'Service Error: ' + error.get_message();
}

function ItemQuantityChanged(fldQUANTITY)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	
	var nQUANTITY = parseInt(fldQUANTITY.value);
	if ( !isNaN(nQUANTITY) )
	{
		var userContext       = fldQUANTITY.id.replace('QUANTITY', '');
		var fldUNIT_PRICE     = document.getElementById(userContext + 'UNIT_PRICE'    );
		var fldEXTENDED_PRICE = document.getElementById(userContext + 'EXTENDED_PRICE');
		if ( fldUNIT_PRICE != null && fldEXTENDED_PRICE != null )
		{
			var dUNIT_PRICE = parseFloat(unformatNumber(fldUNIT_PRICE.value, num_grp_sep, dec_sep));
			if ( !isNaN(dUNIT_PRICE) )
			{
				fldEXTENDED_PRICE.value = FormatCurrency(nQUANTITY * dUNIT_PRICE);
			}
			else
			{
				fldAjaxErrors.innerHTML = 'Unit Price is invalid';
			}
		}
	}
	else
	{
		fldAjaxErrors.innerHTML = 'Quantity is invalid';
	}
}

function ItemUnitPriceChanged(fldUNIT_PRICE)
{
	var fldAjaxErrors = document.getElementById('AjaxErrors');
	fldAjaxErrors.innerHTML = '';
	
	var dLIST_PRICE = unformatNumber(fldUNIT_PRICE.value, num_grp_sep, dec_sep);
	if ( !isNaN(dLIST_PRICE) )
	{
		fldUNIT_PRICE.value = FormatCurrency(dLIST_PRICE);
		var userContext       = fldUNIT_PRICE.id.replace('UNIT_PRICE', '');
		var fldQUANTITY       = document.getElementById(userContext + 'QUANTITY'      );
		var fldEXTENDED_PRICE = document.getElementById(userContext + 'EXTENDED_PRICE');
		if ( fldQUANTITY != null && fldEXTENDED_PRICE != null )
		{
			var nQUANTITY = parseInt(fldQUANTITY.value);
			if ( !isNaN(nQUANTITY) )
			{
				fldEXTENDED_PRICE.value = FormatCurrency(nQUANTITY * dLIST_PRICE);
			}
			else
			{
				fldAjaxErrors.innerHTML = 'Quantity is invalid';
			}
		}
	}
	else
	{
		fldAjaxErrors.innerHTML = 'Unit Price is invalid';
	}
}

if ( typeof(Sys) !== 'undefined' )
	Sys.Application.notifyScriptLoaded();

