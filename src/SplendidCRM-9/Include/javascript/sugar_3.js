/**
 * Javascript file for Sugar
 *
 * The contents of this file are subject to the SugarCRM Professional End User
 * License Agreement ("License") which can be viewed at
 * http://www.sugarcrm.com/crm/products/sugar-professional-eula.html
 * By installing or using this file, You have unconditionally agreed to the
 * terms and conditions of the License, and You may not use this file except in
 * compliance with the License.  Under the terms of the license, You shall not,
 * among other things: 1) sublicense, resell, rent, lease, redistribute, assign
 * or otherwise transfer Your rights to the Software, and 2) use the Software
 * for timesharing or service bureau purposes such as hosting the Software for
 * commercial gain and/or for the benefit of a third party.  Use of the Software
 * may be subject to applicable fees and any use of the Software without first
 * paying applicable fees is strictly prohibited.  You do not have the right to
 * remove SugarCRM copyrights from the source code or user interface.
 *
 * All copies of the Covered Code must include on each user interface screen:
 *  (i) the "Powered by SugarCRM" logo and
 *  (ii) the SugarCRM copyright notice
 * in the same form as they appear in the distribution.  See full license for
 * requirements.
 *
 * Your Warranty, Limitations of liability and Indemnity are expressly stated
 * in the License.  Please refer to the License for the specific language
 * governing these rights and limitations under the License.  Portions created
 * by SugarCRM are Copyright (C) 2004-2006 SugarCRM, Inc.; All Rights Reserved.
 */



/**
 * Namespace for Sugar Objects
 */
var SUGAR = function() {
    return {
    	/**
    	 * Namespace for Homepage
    	 */
    	sugarHome: {},
    	/**
    	 * Namespace for Subpanel Utils
    	 */
    	subpanelUtils: {},
    	/**
    	 * AJAX status class 
    	 */ 
    	ajaxStatusClass: {},
    	/**
    	 * Tab selector utils
    	 */ 
    	tabChooser: {},
    	/**
    	 * General namespace for Sugar utils
    	 */
    	util: {},
    	savedViews: {},
    	/**
    	 * Dashlet utils
    	 */
    	dashlets: {},
    	unifiedSearchAdvanced: {},

    	themes: {},

    	searchForm: {},
    	language: {},
    	Studio:{},
    	contextMenu: {}
    }
}();


/**
 * DHTML date validation script. Courtesy of SmartWebby.com (http://www.smartwebby.com/dhtml/)
 * Portions created by SugarCRM are Copyright (C) SugarCRM, Inc.
 * All Rights Reserved.
 * Contributor(s): ______________________________________..
 */
// Declaring valid date character, minimum year and maximum year
var dtCh= "-";
var minYear=1900;
var maxYear=2100;
var nameIndex = 0;
var typeIndex = 1;
var requiredIndex = 2;
var msgIndex = 3;
var jstypeIndex = 5;
var minIndex = 10;
var maxIndex = 11;
var compareToIndex = 7;
var allowblank = 8;
var validate = new Array();
var maxHours = 24;
var requiredTxt = 'Missing Required Field:'
var invalidTxt = 'Invalid Value:'
var secondsSinceLoad = 0;
var inputsWithErrors = new Array();
var lastSubmitTime = 0;
var alertList = new Array();
var oldStartsWith = '';

function isSupportedIE() {
	var userAgent = navigator.userAgent.toLowerCase() ;

	// IE Check supports ActiveX controls
	if (userAgent.indexOf("msie") != -1 && userAgent.indexOf("mac") == -1 && userAgent.indexOf("opera") == -1) {
		var version = navigator.appVersion.match(/MSIE (.\..)/)[1] ;
		if(version >= 5.5 ) {
			return true;
		} else {
			return false;
		}
	}
}
var isIE = isSupportedIE();

// escapes regular expression characters
RegExp.escape = function(text) { // http://simon.incutio.com/archive/2006/01/20/escape
  if (!arguments.callee.sRE) {
    var specials = ['/', '.', '*', '+', '?', '|','(', ')', '[', ']', '{', '}', '\\'];
    arguments.callee.sRE = new RegExp('(\\' + specials.join('|\\') + ')', 'g');
  }
  return text.replace(arguments.callee.sRE, '\\$1');
}

function addAlert(type, name,subtitle, description,time, redirect) {
	var addIndex = alertList.length;
	alertList[addIndex]= new Array();
	alertList[addIndex]['name'] = name;
	alertList[addIndex]['type'] = type;
	alertList[addIndex]['subtitle'] = subtitle;
	alertList[addIndex]['description'] = description.replace(/<br>/gi, "\n").replace(/&amp;/gi,'&').replace(/&lt;/gi,'<').replace(/&gt;/gi,'>').replace(/&#039;/gi,'\'').replace(/&quot;/gi,'"');
	alertList[addIndex]['time'] = time;
	alertList[addIndex]['done'] = 0;
	alertList[addIndex]['redirect'] = redirect;
} 
function checkAlerts() {
	secondsSinceLoad += 1;
	var mj = 0;
	var alertmsg = '';
	for(mj = 0 ; mj < alertList.length; mj++) {
		if(alertList[mj]['done'] == 0) {
			if(alertList[mj]['time'] < secondsSinceLoad && alertList[mj]['time'] > -1 ) {
				alertmsg = alertList[mj]['type'] + ":" + alertList[mj]['name'] + "\n" +alertList[mj]['subtitle']+ "\n"+ alertList[mj]['description'] + "\n\n";
				alertList[mj]['done'] = 1;
				if(alertList[mj]['redirect'] == '') {
					alert(alertmsg);
				} 
				else if(confirm(alertmsg)) {
					window.location = alertList[mj]['redirect'];
				}
			}
		}
	}
	
	setTimeout("checkAlerts()", 1000);
}

function toggleDisplay(id) {		
	if(this.document.getElementById(id).style.display == 'none') {
		this.document.getElementById(id).style.display = '';
		if(this.document.getElementById(id+"link") != undefined) {
			this.document.getElementById(id+"link").style.display = 'none';
		}
	}
	else {
		this.document.getElementById(id).style.display = 'none'
		if(this.document.getElementById(id+"link") != undefined) {
			this.document.getElementById(id+"link").style.display = '';
		}
	}
}

function checkAll(form, field, value) {
	for (i = 0; i < form.elements.length; i++) {
		if(form.elements[i].name == field)
			form.elements[i].checked = value;
	}
}

function replaceAll(text, src, rep) {
	offset = text.toLowerCase().indexOf(src.toLowerCase());
	while(offset != -1) {
		text = text.substring(0, offset) + rep + text.substring(offset + src.length ,text.length);
		offset = text.indexOf( src, offset + rep.length + 1);
	}
	return text;
}

function addForm(formname) {
	validate[formname] = new Array();
}

function addToValidate(formname, name, type,required, msg) {
	if(typeof validate[formname] == 'undefined') {
		addForm(formname);
	}
	validate[formname][validate[formname].length] = new Array(name, type,required, msg);	
}

function addToValidateRange(formname, name, type,required,  msg,min,max) {
	addToValidate(formname, name, type,required,  msg);
	validate[formname][validate[formname].length - 1][jstypeIndex] = 'range'
	validate[formname][validate[formname].length - 1][minIndex] = min;
	validate[formname][validate[formname].length - 1][maxIndex] = max;
}

function addToValidateDateBefore(formname, name, type, required, msg, compareTo) {
	addToValidate(formname, name, type,required,  msg);
	validate[formname][validate[formname].length - 1][jstypeIndex] = 'isbefore'
	validate[formname][validate[formname].length - 1][compareToIndex] = compareTo;
}

function addToValidateDateBeforeAllowBlank(formname, name, type, required, msg, compareTo, allowBlank) {
	addToValidate(formname, name, type,required,  msg);
	validate[formname][validate[formname].length - 1][jstypeIndex] = 'isbefore'
	validate[formname][validate[formname].length - 1][compareToIndex] = compareTo;
	validate[formname][validate[formname].length - 1][allowblank] = allowBlank;
}

function addToValidateBinaryDependency(formname, name, type, required, msg, compareTo) {
	addToValidate(formname, name, type, required, msg);
	validate[formname][validate[formname].length - 1][jstypeIndex] = 'binarydep';
	validate[formname][validate[formname].length - 1][compareToIndex] = compareTo;
}

function addToValidateComparison(formname, name, type, required, msg, compareTo) {
	addToValidate(formname, name, type, required, msg);
	validate[formname][validate[formname].length - 1][jstypeIndex] = 'comparison';
	validate[formname][validate[formname].length - 1][compareToIndex] = compareTo;
}

function removeFromValidate(formname, name) {
	for(i = 0; i < validate[formname].length; i++){
		if(validate[formname][i][nameIndex] == name){
			validate[formname].splice(i, 1);
		}
	}
}

function toDecimal(original) {
	temp = Math.round(original*100)/100;
	if((original * 100) % 100 == 0)
		return temp + '.00';
	if((original * 10) % 10 == 0)
		return temp + '0';
	return temp
}

function isInteger(s) {
	if(typeof num_grp_sep != 'undefined' && typeof dec_sep != 'undefined')
		s = unformatNumber(s, num_grp_sep, dec_sep).toString();

	var i;
    for (i = 0; i < s.length; i++){
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    // All characters are numbers.
    return true;
}

function isNumeric(s) {
  if(!/^-*[0-9\.]+$/.test(s)) {
   		return false
   }
   else {
		return true;
   }
}

function stripCharsInBag(s, bag) {
	var i;
    var returnString = "";
    // Search through string's characters one by one.
    // If character is not in bag, append to returnString.
    for (i = 0; i < s.length; i++){
        var c = s.charAt(i);
        if (bag.indexOf(c) == -1) returnString += c;
    }
    return returnString;
}

function daysInFebruary(year) {
	// February has 29 days in any year evenly divisible by four,
    // EXCEPT for centurial years which are not also divisible by 400.
    return (((year % 4 == 0) && ( (!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28 );
}

function DaysArray(n) {
	for (var i = 1; i <= n; i++) {
		this[i] = 31
		if (i==4 || i==6 || i==9 || i==11) {this[i] = 30}
		if (i==2) {this[i] = 29}
   }
   return this
}

var date_reg_positions = {'Y': 1,'m': 2,'d': 3};
var date_reg_format = '([0-9]{4})-([0-9]{1,2})-([0-9]{1,2})'
function isDate(dtStr) {
	if(dtStr.length== 0) {
		return true;
	}
	myregexp = new RegExp(date_reg_format)
	if(!myregexp.test(dtStr))
		return false

return true
}

function getDateObject(dtStr) {
	if(dtStr.length== 0) {
		return true;
	}

	myregexp = new RegExp(date_reg_format)

	if(myregexp.exec(dtStr)) var dt = myregexp.exec(dtStr)
	else return false;

	var yr = dt[date_reg_positions['Y']];
	var mh = dt[date_reg_positions['m']];
	var dy = dt[date_reg_positions['d']];
	var date1 = new Date();
	date1.setFullYear(yr); // xxxx 4 char year
	date1.setMonth(mh-1); // 0-11 Bug 4048: javascript Date obj months are 0-index
	date1.setDate(dy); // 1-31
	return date1;
}

function isBefore(value1, value2) {
	var d1 = getDateObject(value1);
	var d2 = getDateObject(value2);
	
	return d2 >= d1;
}

function isValidEmail(emailStr) {
	if(emailStr.length== 0) {
		return true;
	}
	
	// cn: bug 7128, a period at the end of the string mangles checks.
	var lastChar = emailStr.charAt(emailStr.length - 1);
	if(!lastChar.match(/[a-z]/i)) {
		return false;
	}
	
	if(!/^\w+(['\.\-\+]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/.test(emailStr))
		return false
	return true	
}

function isValidPhone(phoneStr) {
	if(phoneStr.length== 0) {
		return true;
	}
	if(!/^[0-9\-\(\)]+$/.test(phoneStr))
		return false
	return true	
}
function isFloat(floatStr) {
	if(floatStr.length== 0) {
		return true;
	}
	floatStr = unformatNumber(floatStr, num_grp_sep, dec_sep).toString();

	if(!/^[0-9\.]+$/.test(floatStr))
		return false
	return true	
}
function isDBName(str) {
	
	if(str.length== 0) {
		return true;
	}
	if(!/^[a-zA-Z\_0-9]+$/.test(str))
		return false
	return true	
}
var time_reg_format = "[0-9]{1,2}\:[0-9]{2}";
function isTime(timeStr) {
	time_reg_format = time_reg_format.replace('([ap]m)', '');
	time_reg_format = time_reg_format.replace('([AP]M)', '');
	if(timeStr.length== 0){
		return true;
	}
	//we now support multiple time formats
	myregexp = new RegExp(time_reg_format)
	if(!myregexp.test(timeStr))
		return false

	return true
}

function inRange(value, min, max) {
	return value >= min && value <= max;
}

function bothExist(item1, item2) {
	if(typeof item1 == 'undefined') { return false; }
	if(typeof item2 == 'undefined') { return false; }
	if((item1 == '' && item2 != '') || (item1 != '' && item2 == '') ) { return false; }
	return true;
}

function trim(s) {
	if(typeof(s) == 'undefined')  
		return s;
	while (s.substring(0,1) == " ") {
		s = s.substring(1, s.length);
	}
	while (s.substring(s.length-1, s.length) == ' ') {
		s = s.substring(0,s.length-1);
	}

	return s;
}


function check_form(formname) {
	if (typeof(siw) != 'undefined' && siw 
		&& typeof(siw.selectingSomething) != 'undefined' && siw.selectingSomething)
			return false;
	return validate_form(formname, '');	
}

function add_error_style(formname, input, txt) {
	inputHandle = eval("document." + formname + "['" + input + "']");
	style = get_current_bgcolor(inputHandle);
		
	if(inputHandle.parentNode.innerHTML.search(txt) == -1) {
		errorTextNode = document.createElement('span');
		errorTextNode.className = 'required';
		errorTextNode.innerHTML = '<br />' + txt;
		inputHandle.parentNode.appendChild(errorTextNode);		
	}

	inputHandle.style.backgroundColor = "#FF0000";
	inputsWithErrors.push(inputHandle);

	for(wp = 1; wp <= 10; wp++) {
		window.setTimeout('fade_error_style(style, ' + wp * 10 + ')', 1000 + (wp * 50));
	}
}

function get_current_bgcolor(input) {
	if(input.currentStyle) {// ie 
		style = input.currentStyle.backgroundColor;
		return style.substring(1,7);
	}
	else {// moz
		style = '';
		styleRGB = document.defaultView.getComputedStyle(input, '').getPropertyValue("background-color");
		comma = styleRGB.indexOf(',');
		style += dec2hex(styleRGB.substring(4, comma));
		commaPrevious = comma;
		comma = styleRGB.indexOf(',', commaPrevious+1);
		style += dec2hex(styleRGB.substring(commaPrevious+2, comma));
		style += dec2hex(styleRGB.substring(comma+2, styleRGB.lastIndexOf(')')));
		return style;
	}
}

function hex2dec(hex){return(parseInt(hex,16));}
var hexDigit=new Array("0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F");
function dec2hex(dec){return(hexDigit[dec>>4]+hexDigit[dec&15]);}

function fade_error_style(normalStyle, percent) {	
	errorStyle = 'c60c30';
	var r1 = hex2dec(errorStyle.slice(0,2));
	var g1 = hex2dec(errorStyle.slice(2,4));
	var b1 = hex2dec(errorStyle.slice(4,6));
	
	var r2 = hex2dec(normalStyle.slice(0,2));
	var g2 = hex2dec(normalStyle.slice(2,4));
	var b2 = hex2dec(normalStyle.slice(4,6));


	var pc = percent / 100;

	r= Math.floor(r1+(pc*(r2-r1)) + .5);
	g= Math.floor(g1+(pc*(g2-g1)) + .5);
	b= Math.floor(b1+(pc*(b2-b1)) + .5);

	for(var wp = 0; wp < inputsWithErrors.length; wp++) {
		inputsWithErrors[wp].style.backgroundColor = "#" + dec2hex(r) + dec2hex(g) + dec2hex(b);
	}
}


function validate_form(formname, startsWith){
    requiredTxt = SUGAR.language.get('app_strings', 'ERR_MISSING_REQUIRED_FIELDS');
    invalidTxt = SUGAR.language.get('app_strings', 'ERR_INVALID_VALUE');

	if ( typeof (formname) == 'undefined')
	{
		return false;
	}
	if ( typeof (validate[formname]) == 'undefined')
	{
		return true;
	}

	var form = "document." + formname;
	var isError = false;
	var errorMsg = "";
	
	var _date = new Date();
	if(_date.getTime() < (lastSubmitTime + 2000) && startsWith == oldStartsWith) { // ignore submits for the next 2 seconds
		return false;
	}
	lastSubmitTime = _date.getTime();
	oldStartsWith = startsWith;
	
	for(var wp = 0; wp < inputsWithErrors.length; wp++) {
		if(typeof(inputsWithErrors[wp]) !='undefined' && typeof inputsWithErrors[wp].parentNode != 'undefined')
			inputsWithErrors[wp].parentNode.removeChild(inputsWithErrors[wp].parentNode.lastChild);
	} // remove previous error messages
	
	inputsWithErrors = new Array();
	for(var i = 0; i < validate[formname].length; i++){
			if(validate[formname][i][nameIndex].indexOf(startsWith) == 0){
				if(typeof eval(form + "['" + validate[formname][i][nameIndex] + "']" ) != 'undefined'){
					var bail = false;
					if(validate[formname][i][requiredIndex]){
						if(typeof eval(form + "['" + validate[formname][i][nameIndex] + "']") == 'undefined' || trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")) == ""){
							add_error_style(formname, validate[formname][i][nameIndex], requiredTxt +' ' + validate[formname][i][msgIndex]);
							isError = true;
						}
					}
					if(!bail){
						switch(validate[formname][i][typeIndex]){
						case 'email':
							if(!isValidEmail(trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")))){
								isError = true;
								add_error_style(formname, validate[formname][i][nameIndex], invalidTxt + " " +	validate[formname][i][msgIndex]);
							}
							 break;
						case 'time':
							if( !isTime(trim(eval(form+"['" + validate[formname][i][nameIndex] + "']" + ".value")))){
								isError = true;
								add_error_style(formname, validate[formname][i][nameIndex], invalidTxt + " " +	validate[formname][i][msgIndex]);
							} break;
						case 'date': if(!isDate(trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")))){
								isError = true;
								add_error_style(formname, validate[formname][i][nameIndex], invalidTxt + " " +	validate[formname][i][msgIndex]);
							}  break;
						case 'alpha': 
							break;
						case 'DBName':
							if(!isDBName(trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")))){
								isError = true;
								add_error_style(formname, validate[formname][i][nameIndex], invalidTxt + " " +	validate[formname][i][msgIndex]);
							}
							break;
						case 'alphanumeric': 
							break;
						case 'int':
							if(!isInteger(trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")))){
								isError = true;
								add_error_style(formname, validate[formname][i][nameIndex], invalidTxt + " " +	validate[formname][i][msgIndex]);
							}
							break;
						case 'float':
							if(!isFloat(trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")))){
								isError = true;
								add_error_style(formname, validate[formname][i][nameIndex], invalidTxt + " " +	validate[formname][i][msgIndex]);
							} 
							break;
						}

						if(typeof validate[formname][i][jstypeIndex]  != 'undefined'/* && !isError*/){

							switch(validate[formname][i][jstypeIndex]){
							case 'range':
								if(!inRange(trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value")), validate[formname][i][minIndex], validate[formname][i][maxIndex])){
									isError = true;
									add_error_style(formname, validate[formname][i][nameIndex], validate[formname][i][msgIndex] + " value " + eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value") + " is not within the valid range (" +validate[formname][i][minIndex] + " - " + validate[formname][i][maxIndex] +  ") ");
								}
							break;
							case 'isbefore':
								compareTo = form + "." + validate[formname][i][compareToIndex];
								if(	typeof compareTo != 'undefined'){
									if(trim(eval(compareTo + '.value')) != '' || (validate[formname][i][allowblank] != 'true') ) {
										date2 = trim(eval(compareTo + '.value'));
										date1 = trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value"));
	
										if(trim(date1).length != 0 && !isBefore(date1,date2)){ 		
											isError = true;
											add_error_style(formname, validate[formname][i][nameIndex], validate[formname][i][msgIndex] + "(" + date1 + ") is not before " + date2);
										}										
									}
								}
							break;
							case 'binarydep':
								compareTo = form + "." + validate[formname][i][compareToIndex];
								if( typeof compareTo != 'undefined') {
									item1 = trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value"));
									item2 = trim(eval(compareTo + '.value'));
									if(!bothExist(item1, item2)) {
										isError = true;
										add_error_style(formname, validate[formname][i][nameIndex], validate[formname][i][msgIndex]);
									}
								}
							break;
							case 'comparison':
								compareTo = form + "." + validate[formname][i][compareToIndex];
								if( typeof compareTo != 'undefined') {
									item1 = trim(eval(form + "['" + validate[formname][i][nameIndex] + "']" + ".value"));
									item2 = trim(eval(compareTo + '.value'));
									if(!bothExist(item1, item2) || item1 != item2) {
										isError = true;
										add_error_style(formname, validate[formname][i][nameIndex], validate[formname][i][msgIndex]);
									}
								}
							break;
							}
						}
					}
				}
			}
		}
	if (isError == true) {
		var nw, ne, sw, se;
		if (self.pageYOffset) // all except Explorer
		{
			nwX = self.pageXOffset;
			seX = self.innerWidth;
			nwY = self.pageYOffset;
			seY = self.innerHeight;
		}
		else if (document.documentElement && document.documentElement.scrollTop) // Explorer 6 Strict
		{
			nwX = document.documentElement.scrollLeft;
			seX = document.documentElement.clientWidth;
			nwY = document.documentElement.scrollTop; 
			seY = document.documentElement.clientHeight;
		}
		else if (document.body) // all other Explorers
		{
			nwX = document.body.scrollLeft;
			seX = document.body.clientWidth;
			nwY = document.body.scrollTop;
			seY = document.body.clientHeight;
		}

		var inView = true; // is there an error within viewport of browser
		for(var wp = 0; wp < inputsWithErrors.length; wp++) {
			var elementCoor = findElementPos(inputsWithErrors[wp]); 
			if(!(elementCoor.x >= nwX && elementCoor.y >= nwY &&
				elementCoor.x <= seX && elementCoor.y <= seY)) { // if input is not within viewport
					inView = false;
					scrollToTop = elementCoor.y - 75;
					scrollToLeft = elementCoor.x - 75;
			}
			else { // on first input within viewport, don't scroll
				break;
			}
		}
		
		if(!inView) window.scrollTo(scrollToTop, scrollToLeft);

		return false;
	}
	
	return true;
	
}


/**
 * This array is used to remember mark status of rows in browse mode
 */
var marked_row = new Array;


/**
 * Sets/unsets the pointer and marker in browse mode
 *
 * @param   object    the table row
 * @param   interger  the row number
 * @param   string    the action calling this script (over, out or click)
 * @param   string    the default background color
 * @param   string    the color to use for mouseover
 * @param   string    the color to use for marking a row
 *
 * @return  boolean  whether pointer is set or not
 */
function setPointer(theRow, theRowNum, theAction, theDefaultColor, thePointerColor, theMarkColor) {
    var theCells = null;

    // 1. Pointer and mark feature are disabled or the browser can't get the
    //    row -> exits
    if ((thePointerColor == '' && theMarkColor == '')
        || typeof(theRow.style) == 'undefined') {
        return false;
    }

    // 2. Gets the current row and exits if the browser can't get it
    if (typeof(document.getElementsByTagName) != 'undefined') {
        theCells = theRow.getElementsByTagName('td');
    }
    else if (typeof(theRow.cells) != 'undefined') {
        theCells = theRow.cells;
    }
    else {
        return false;
    }

    // 3. Gets the current color...
    var rowCellsCnt  = theCells.length;
    var domDetect    = null;
    var currentColor = null;
    var newColor     = null;
    // 3.1 ... with DOM compatible browsers except Opera that does not return
    //         valid values with "getAttribute"
    if (typeof(window.opera) == 'undefined'
        && typeof(theCells[0].getAttribute) != 'undefined') {
        currentColor = theCells[0].getAttribute('bgcolor');
        domDetect    = true;
    }
    // 3.2 ... with other browsers
    else {
        currentColor = theCells[0].style.backgroundColor;
        domDetect    = false;
    } // end 3

    // 4. Defines the new color
    // 4.1 Current color is the default one
    if (currentColor == ''
        || currentColor.toLowerCase() == theDefaultColor.toLowerCase()) {
        if (theAction == 'over' && thePointerColor != '') {
            newColor              = thePointerColor;
        }
        else if (theAction == 'click' && theMarkColor != '') {
            newColor              = theMarkColor;
            marked_row[theRowNum] = true;
        }
    }
    // 4.1.2 Current color is the pointer one
    else if (currentColor.toLowerCase() == thePointerColor.toLowerCase()
             && (typeof(marked_row[theRowNum]) == 'undefined' || !marked_row[theRowNum])) {
        if (theAction == 'out') {
            newColor              = theDefaultColor;
        }
        else if (theAction == 'click' && theMarkColor != '') {
            newColor              = theMarkColor;
            marked_row[theRowNum] = true;
        }
    }
    // 4.1.3 Current color is the marker one
    else if (currentColor.toLowerCase() == theMarkColor.toLowerCase()) {
        if (theAction == 'click') {
            newColor              = (thePointerColor != '')
                                  ? thePointerColor
                                  : theDefaultColor;
            marked_row[theRowNum] = (typeof(marked_row[theRowNum]) == 'undefined' || !marked_row[theRowNum])
                                  ? true
                                  : null;
        }
    } // end 4

    // 5. Sets the new color...
    if (newColor) {
        var c = null;
        // 5.1 ... with DOM compatible browsers except Opera
        if (domDetect) {
            for (c = 0; c < rowCellsCnt; c++) {
                theCells[c].setAttribute('bgcolor', newColor, 0);
            } // end for
        }
        // 5.2 ... with other browsers
        else {
            for (c = 0; c < rowCellsCnt; c++) {
                theCells[c].style.backgroundColor = newColor;
            }
        }
    } // end 5

    return true;
} // end of the 'setPointer()' function


/**
  * listbox redirection
  */
function goToUrl(selObj, goToLocation) {
    eval("document.location.href = '" + goToLocation + "pos=" + selObj.options[selObj.selectedIndex].value + "'");
}



var json_objects = new Object();

function getXMLHTTPinstance() {
	var xmlhttp = false;
	var userAgent = navigator.userAgent.toLowerCase() ;

	// IE Check supports ActiveX controls
	if (userAgent.indexOf("msie") != -1 && userAgent.indexOf("mac") == -1 && userAgent.indexOf("opera") == -1) {
		var version = navigator.appVersion.match(/MSIE (.\..)/)[1] ;
		if(version >= 5.5 ) {
			try {
				xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
			}
			catch (e) {
				try {
					xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
				}
				catch (E) {
					xmlhttp = false;
				}
			}
		}
	}
	
	if (!xmlhttp && typeof XMLHttpRequest!='undefined') {
		xmlhttp = new XMLHttpRequest();
	}
	return xmlhttp;
}

// NOW LOAD THE OBJECT..
var global_xmlhttp = getXMLHTTPinstance();

function http_fetch_sync(url,post_data) {
	global_xmlhttp = getXMLHTTPinstance();
	var method = 'GET';

	if(typeof(post_data) != 'undefined') method = 'POST';
	try {
		global_xmlhttp.open(method, url,false);
	}
	catch(e) {
		alert('message:'+e.message+":url:"+url);
	}
	if(method == 'POST') {
		global_xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
	}

	global_xmlhttp.send(post_data);
	
	var args = {"responseText" : global_xmlhttp.responseText,
				"responseXML" : global_xmlhttp.responseXML,
				"request_id" : request_id};
	return args;

}
// this is a GET unless post_data is defined

function http_fetch_async(url,callback,request_id,post_data) {
	var method = 'GET';
	if(typeof(post_data) != 'undefined') {
		method = 'POST';
	}
	
	try {
		global_xmlhttp.open(method, url,true);
	}
	catch(e) {
		alert('message:'+e.message+":url:"+url);
	}
	if(method == 'POST') {
		global_xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
	}
	global_xmlhttp.onreadystatechange = function() {                                                
		if(global_xmlhttp.readyState==4) {
			if(global_xmlhttp.status == 200) {
				var args = {"responseText" : global_xmlhttp.responseText,
							"responseXML" : global_xmlhttp.responseXML,
							"request_id" : request_id };
				callback.call(document,args);
			}
			else {
				alert("There was a problem retrieving the XML data:\n" + global_xmlhttp.statusText);
			}
		}
	}
	global_xmlhttp.send(post_data);
}

function call_json_method(module,action,vars,variable_name,callback) {
	global_xmlhttp.open("GET", "json.php?module="+module+"&action="+action+"&"+vars,true);
	global_xmlhttp.onreadystatechange=
	function() {
		if(global_xmlhttp.readyState==4) {
			if(global_xmlhttp.status == 200) {
				try {
					eval("json_objects['"+variable_name+"'] =" + global_xmlhttp.responseText);
			    } 
			    catch(exception) {
			         alert("ERROR:"+exception+", returned from server:"+global_xmlhttp.responseText);
				}
				var args = {responseText:global_xmlhttp.responseText,responseXML:global_xmlhttp.responseXML};
				callback.call(document,args);
			} 
			else {
				alert("There was a problem retrieving the XML data:\n" +
				global_xmlhttp.statusText);
			}
		}
	}
	global_xmlhttp.send(null);
}

function insert_at_cursor(field, value) {
 //ie:
	if (document.selection) {
		field.focus();
		sel = document.selection.createRange();
		sel.text = value;
	}
 //mozilla:
	else if(field.selectionStart || field.selectionStart == '0') {
		var start_pos = field.selectionStart;
		var end_pos = field.selectionEnd;
		field.value = field.value.substring(0, start_pos) + value + field.value.substring(end_pos, field.value.length);
	} 
	else {
		field.value += value;
	}
}

function checkParentType(type,button) {
	if(button == null) {
		return;
	}
	if(typeof(disabledModules[type]) != 'undefined') {
		button.disabled='disabled';
	}
	else {
		button.disabled = false;
	}
}

function parseDate(input, format) {
	date = input.value;
	format = format.replace(/%/g, '');
	sep = format.charAt(1);
	yAt = format.indexOf('Y')
	// 1-1-06 or 1-12-06 or 1-1-2006 or 1-12-2006
	if(date.match(/^\d{1,2}[\/-]\d{1,2}[\/-]\d{2,4}$/) && yAt == 4) {
		if(date.match(/^\d{1}[\/-].*$/)) date = '0' + date;
		if(date.match(/^\d{2}[\/-]\d{1}[\/-].*$/)) date = date.substring(0,3) + '0' + date.substring(3,date.length);
		if(date.match(/^\d{2}[\/-]\d{2}[\/-]\d{2}$/)) date = date.substring(0,6) + '20' + date.substring(6,date.length);
	}
	// 06-11-1 or 06-1-1
	else if(date.match(/^\d{2,4}[\/-]\d{1,2}[\/-]\d{1,2}$/)) {
		if(date.match(/^\d{2}[\/-].*$/)) date = '20' + date;
		if(date.match(/^\d{4}[\/-]\d{1}[\/-].*$/)) date = date.substring(0,5) + '0' + date.substring(5,date.length);		
		if(date.match(/^\d{4}[\/-]\d{2}[\/-]\d{1}$/)) date = date.substring(0,8) + '0' + date.substring(8,date.length);		
	}
	else if(date.match(/^\d{4,8}$/)) { // digits only
		digits = 0;
		if(date.match(/^\d{8}$/)) digits = 8;// match for 8 digits
		else if(date.match(/\d{6}/)) digits = 6;// match for 5 digits
		else if(date.match(/\d{4}/)) digits = 4;// match for 5 digits
		else if(date.match(/\d{5}/)) digits = 5;// match for 5 digits
		
		switch(yAt) {
			case 0:
				switch(digits) {
					case 4: date = '20' + date.substring(0,2) + sep + '0' + date.substring(2, 3) + sep + '0' + date.substring(3,4); break;
					case 5: date = '20' + date.substring(0,2) + sep + date.substring(2, 4) + sep + '0' + date.substring(4,5); break;
					case 6: date = '20' + date.substring(0,2) + sep + date.substring(2, 4) + sep + date.substring(4,6); break;				
					case 8: date = date.substring(0,4) + sep + date.substring(4, 6) + sep + date.substring(6,8); break;
				}
				break;
			case 2:
				switch(digits) {
					case 4: date = '0' + date.substring(0,1) + sep + '20' + date.substring(1, 3) + sep + '0' + date.substring(3,4); break;
					case 5: date = date.substring(0,2) + sep + '20' + date.substring(2, 4) + sep + '0' + date.substring(4,5); break;
					case 6: date = date.substring(0,2) + sep + '20' + date.substring(2, 4) + sep + date.substring(4,6); break;				
					case 8: date = date.substring(0,2) + sep + date.substring(2, 6) + sep + date.substring(6,8); break;
				}
			case 4:
				switch(digits) {
					case 4: date = '0' + date.substring(0,1) + sep + '0' + date.substring(1, 2) + sep + '20' + date.substring(2,4); break;
					case 5: date = '0' + date.substring(0,1) + sep + date.substring(1, 3) + sep + '20' + date.substring(3,5); break;
					case 6: date = date.substring(0,2) + sep + date.substring(2, 4) + sep + '20' + date.substring(4,6); break;				
					case 8: date = date.substring(0,2) + sep + date.substring(2, 4) + sep + date.substring(4,8); break;
				}
				break;
		}
	}
	date = date.replace(/[\/-]/g, sep);
	input.value = date;
}

// find obj's position
function findElementPos(obj) {
    var x = 0;
    var y = 0;
    if (obj.offsetParent) {
      while (obj.offsetParent) {
        x += obj.offsetLeft;
        y += obj.offsetTop;
        obj = obj.offsetParent;
      }
    }//if offsetParent exists
    else if (obj.x && obj.y) {
      y += obj.y
      x += obj.x
    }
	return new coordinate(x, y);
}//findElementPos


// get dimensions of the browser window
function getClientDim() {
	var nwX, nwY, seX, seY;
	if (self.pageYOffset) // all except Explorer
	{
	  nwX = self.pageXOffset;
	  seX = self.innerWidth + nwX;
	  nwY = self.pageYOffset;
	  seY = self.innerHeight + nwY;
	}
	else if (document.documentElement && document.documentElement.scrollTop) // Explorer 6 Strict
	{
	  nwX = document.documentElement.scrollLeft;
	  seX = document.documentElement.clientWidth + nwX;
	  nwY = document.documentElement.scrollTop;
	  seY = document.documentElement.clientHeight + nwY;
	}
	else if (document.body) // all other Explorers
	{
	  nwX = document.body.scrollLeft;
	  seX = document.body.clientWidth + nwX;
	  nwY = document.body.scrollTop;
	  seY = document.body.clientHeight + nwY;
	}
	return {'nw' : new coordinate(nwX, nwY), 'se' : new coordinate(seX, seY)};
} 

/**
* stop propagation on events
**/
function freezeEvent(e) {
	if(e) {
	  if (e.preventDefault) e.preventDefault();
	  e.returnValue = false;
	  e.cancelBubble = true;
	  if (e.stopPropagation) e.stopPropagation();
	  return false;
	}
}


/**
 * coordinate class
 **/
function coordinate(_x, _y) {
  var x = _x;
  var y = _y;
  this.add = add;
  this.sub = sub;
  this.x = x;
  this.y = y;

  function add(rh) {
    return new position(this.x + rh.x, this.y + rh.y);
  }

  function sub(rh) {
    return new position(this.x + rh.x, this.y + rh.y);
  }
}

// sends theForm via AJAX and fills in the theDiv
function sendAndRetrieve(theForm, theDiv, loadingStr) {
	function success(data) {
		document.getElementById(theDiv).innerHTML = data.responseText;
		ajaxStatus.hideStatus();
	}
	if(typeof loadingStr == 'undefined') SUGAR.language.get('app_strings', 'LBL_LOADING');
	ajaxStatus.showStatus(loadingStr);
	YAHOO.util.Connect.setForm(theForm); 
	var cObj = YAHOO.util.Connect.asyncRequest('POST', 'index.php', {success: success, failure: success});
	return false;
}

// -- start sugarListView class
// js functions used for ListView
function sugarListView() {
}

sugarListView.update_count = function(count, add) {
	if(typeof document.MassUpdate != 'undefined') {
		the_form = document.MassUpdate;
		for(wp = 0; wp < the_form.elements.length; wp++) {
			if(typeof the_form.elements[wp].name != 'undefined' && the_form.elements[wp].name == 'selectCount[]') {
				if(add)	the_form.elements[wp].value = parseInt(the_form.elements[wp].value) + count;
				else the_form.elements[wp].value = count;
			}	
		}
	}
}

sugarListView.prototype.send_form = function(select, currentModule, action, no_record_txt,action_module,return_info) {

	sugarListView.get_checks();
	// create new form to post (can't access action property of MassUpdate form due to action input) 
	var newForm = document.createElement('form');
	newForm.method = 'post';
	newForm.action = action;
	newForm.name = 'newForm';
	newForm.id = 'newForm';
	
	var uidTa = document.createElement('textarea');
	uidTa.name = 'uid';
	uidTa.style.display = 'none';
	if(select) { // use selected items	
		uidTa.value = document.MassUpdate.uid.value;
	}
	else { // use current page
		inputs = document.MassUpdate.elements;
		ar = new Array();
		for(i = 0; i < inputs.length; i++) {
			if(inputs[i].name == 'mass[]')
				ar.push(inputs[i].value);
		}
		uidTa.value = ar.join(',');
	}
	
	if(uidTa.value == '') { 
		alert(no_record_txt); 
		return false;
	}
	
	newForm.appendChild(uidTa);
	
	var moduleInput = document.createElement('input');
	moduleInput.name = 'module';
	moduleInput.type = 'hidden';
	moduleInput.value = currentModule;	
	newForm.appendChild(moduleInput);
	
	var actionInput = document.createElement('input');
	actionInput.name = 'action';
	actionInput.type = 'hidden';
	actionInput.value = 'index';	
	newForm.appendChild(actionInput);
	
	if (typeof action_module != 'undefined' && action_module!= '') {
		var actionModule = document.createElement('input');
		actionModule.name = 'action_module';
		actionModule.type = 'hidden';
		actionModule.value = action_module;	
		newForm.appendChild(actionModule);
	}
	//return_info must follow this pattern."&return_module=Accounts&return_action=index"
	if (typeof return_info!= 'undefined' && return_info != '') {
		var params= return_info.split('&');
		if (params.length > 0) {
			for (var i=0;i< params.length;i++) {
				if (params[i].length > 0) {
					var param_nv=params[i].split('='); 
					if (param_nv.length==2){
						returnModule = document.createElement('input');
						returnModule.name = param_nv[0];
						returnModule.type = 'hidden';
						returnModule.value = param_nv[1];	
						newForm.appendChild(returnModule);
					}				
				}
			}
		}
	}

	document.MassUpdate.parentNode.appendChild(newForm);

	newForm.submit();

	return false;
}
//return a count of checked row.
sugarListView.get_checks_count = function() {
	ar = new Array();

	// build associated array of uids, associated array ensures uniqueness	
	inputs = document.MassUpdate.elements;
	for(i = 0; i < inputs.length; i++) {
		if(inputs[i].name == 'mass[]') 
			ar[inputs[i].value]	= (inputs[i].checked) ? 1 : 0; // 0 of it is unchecked
	}
	
	// build regular array of uids
	uids = new Array(); 
	for(i in ar) { 
		if(ar[i] == 1) uids.push(i);
	}
	
	return uids.length;
}

// saves the checks on the current page into the uid textarea
sugarListView.get_checks = function() {
	ar = new Array();

	if(document.MassUpdate.uid.value != '') {
		oldUids = document.MassUpdate.uid.value.split(',');
		for(uid in oldUids) ar[oldUids[uid]] = 1;	
	}
	
	// build associated array of uids, associated array ensures uniqueness	
	inputs = document.MassUpdate.elements;
	for(i = 0; i < inputs.length; i++) {
		if(inputs[i].name == 'mass[]') 
			ar[inputs[i].value]	= (inputs[i].checked) ? 1 : 0; // 0 of it is unchecked
	}
	
	// build regular array of uids
	uids = new Array(); 
	for(i in ar) { 
		if(ar[i] == 1) uids.push(i);
	}
	
	document.MassUpdate.uid.value = uids.join(',');

	if(uids.length == 0) return false; // return false if no checks to get 
	return true; // there are saved checks
}

sugarListView.prototype.save_checks = function(offset, moduleString) {
	checks = sugarListView.get_checks();
	eval('document.MassUpdate.' + moduleString + '.value = offset');
	if(typeof document.MassUpdate.massupdate != 'undefined') document.MassUpdate.massupdate.value = 'false';
	if(checks) document.MassUpdate.submit();
	return !checks;
}

sugarListView.prototype.check_item = function(cb, form) {
	if(cb.checked) sugarListView.update_count(1, true);
	else sugarListView.update_count(-1, true);
}

sugarListView.prototype.check_all = function(form, field, value) {
	// count number of items
	count = 0;
	
	for (i = 0; i < form.elements.length; i++) {
		if(form.elements[i].name == field) {
			if(form.elements[i].checked != value) count++;
			form.elements[i].checked = value;
		}
	}

	if(value) sugarListView.update_count(count, true);
	else sugarListView.update_count(-1 * count, true);
}
sugarListView.check_all = sugarListView.prototype.check_all;

sugarListView.prototype.check_boxes = function() {
	var inputsCount = 0;	
	var checkedCount = 0;
	var existing_onload = window.onload;
	var theForm = document.MassUpdate;

	if(typeof theForm.uid.value != 'undefined' && theForm.uid.value != "") {
		inputs_array = theForm.elements;
		checked_items = theForm.uid.value.split(",");
		for(wp = 0 ; wp < inputs_array.length; wp++) {
			if(inputs_array[wp].name == "mass[]") {
				inputsCount++;				
				for(i in checked_items) {
					if(inputs_array[wp].value == checked_items[i]) {
						checkedCount++;
						inputs_array[wp].checked = true;
					}
				}
			}
		}
		sugarListView.update_count(checked_items.length);
	}
	else {
		sugarListView.update_count(0)
	}
	if(checkedCount > 0 && checkedCount == inputsCount) 
		document.MassUpdate.massall.checked = true;
}

sugarListView.prototype.send_mass_update = function(mode, no_record_txt, del) {
	formValid = check_form('MassUpdate');
	if(!formValid) return false;
	
	var ar = new Array();
	if(del == 1) {
		var deleteInput = document.createElement('input');
		deleteInput.name = 'Delete';
		deleteInput.type = 'hidden';
		deleteInput.value = true;	
		document.MassUpdate.appendChild(deleteInput);
	}
	
	switch(mode) {
		case 'page': 
			document.MassUpdate.uid.value = '';
			for(wp = 0; wp < document.MassUpdate.elements.length; wp++) {
				if(typeof document.MassUpdate.elements[wp].name != 'undefined' 
					&& document.MassUpdate.elements[wp].name == 'mass[]') {
							ar.push(document.MassUpdate.elements[wp].value);
				}
			}			
			document.MassUpdate.uid.value = ar.join(',');
			if(document.MassUpdate.uid.value == '') {
				alert(no_record_txt);
				return false;
			}
			break;
		case 'selected':
			for(wp = 0; wp < document.MassUpdate.elements.length; wp++) {
				if(typeof document.MassUpdate.elements[wp].name != 'undefined' 
					&& document.MassUpdate.elements[wp].name == 'mass[]' 
						&& document.MassUpdate.elements[wp].checked) {
							ar.push(document.MassUpdate.elements[wp].value);
				}
			}			
			if(document.MassUpdate.uid.value != '') document.MassUpdate.uid.value += ',';
			document.MassUpdate.uid.value += ar.join(',');
			if(document.MassUpdate.uid.value == '') {
				alert(no_record_txt);
				return false;
			}
			break;
		case 'entire': 
			var entireInput = document.createElement('input');
			entireInput.name = 'entire';
			entireInput.type = 'hidden';
			entireInput.value = 'index';	
			document.MassUpdate.appendChild(entireInput);
			confirm(no_record_txt);
			break;	
	}
				
	document.MassUpdate.submit();
	return false;
}


sugarListView.prototype.clear_all = function() {
	document.MassUpdate.uid.value = '';
	sugarListView.check_all(document.MassUpdate, 'mass[]', false);
	document.MassUpdate.massall.checked = false;
	sugarListView.update_count(0);
}

sListView = new sugarListView();
// -- end sugarListView class

// format and unformat numbers
function unformatNumber(n, num_grp_sep, dec_sep) {
	if(typeof num_grp_sep == 'undefined' || typeof dec_sep == 'undefined') return n;
	n = n.toString();
	if(n.length > 0) {
		n = n.replace(new RegExp(RegExp.escape(num_grp_sep), 'g'), '').replace(new RegExp(RegExp.escape(dec_sep)), '.');		
		return parseFloat(n);
	}
	return '';
}

// round parameter can be negative for decimal, precision has to be postive
function formatNumber(n, num_grp_sep, dec_sep, round, precision) {
  if(typeof num_grp_sep == 'undefined' || typeof dec_sep == 'undefined') return n;
  n = n.toString();
  if(n.split) n = n.split('.');
  else return n;

  if(n.length > 2) return n.join('.'); // that's not a num!
  // round
  if(typeof round != 'undefined') {
    if(round > 0 && n.length > 1) { // round to decimal
      n[1] = parseFloat('0.' + n[1]);
      n[1] = Math.round(n[1] * Math.pow(10, round)) / Math.pow(10, round);
      n[1] = n[1].toString().split('.')[1];
    }
    if(round <= 0) { // round to whole number
      n[0] = Math.round(parseInt(n[0]) * Math.pow(10, round)) / Math.pow(10, round);
      n[1] = '';
    }
  }

  if(typeof precision != 'undefined' && precision >= 0) {
    if(n.length > 1 && typeof n[1] != 'undefined') n[1] = n[1].substring(0, precision); // cut off precision 
	else n[1] = '';
    if(n[1].length < precision) {
      for(var wp = n[1].length; wp < precision; wp++) n[1] += '0';
    }
  }

  regex = /(\d+)(\d{3})/;
  while(regex.test(n[0])) n[0] = n[0].replace(regex, '$1' + num_grp_sep + '$2');
  return n[0] + (n.length > 1 && n[1] != '' ? dec_sep + n[1] : '');
}

// --- begin ajax status class
SUGAR.ajaxStatusClass = function() {};
SUGAR.ajaxStatusClass.prototype.statusDiv = null;
SUGAR.ajaxStatusClass.prototype.oldOnScroll = null;
SUGAR.ajaxStatusClass.prototype.shown = false; // state of the status window

// reposition the status div, top and centered
SUGAR.ajaxStatusClass.prototype.positionStatus = function() {
	this.statusDiv.style.top = document.body.scrollTop + 8 + 'px';
	statusDivRegion = YAHOO.util.Dom.getRegion(this.statusDiv);
	statusDivWidth = statusDivRegion.right - statusDivRegion.left;
	this.statusDiv.style.left = YAHOO.util.Dom.getViewportWidth() / 2 - statusDivWidth / 2 + 'px';
}

// private func, create the status div
SUGAR.ajaxStatusClass.prototype.createStatus = function(text) {
	statusDiv = document.createElement('div');
	statusDiv.className = 'dataLabel';
	statusDiv.style.background = '#ffffff';
	statusDiv.style.color = '#c60c30';
	statusDiv.style.position = 'absolute';
	
	statusDiv.style.opacity = .8;
	statusDiv.style.filter = 'alpha(opacity=80)';
	statusDiv.id = 'ajaxStatusDiv';
	document.body.appendChild(statusDiv);
	this.statusDiv = document.getElementById('ajaxStatusDiv');
}

// public - show the status div with text
SUGAR.ajaxStatusClass.prototype.showStatus = function(text) {
	if(!this.statusDiv) {
		this.createStatus(text);	
	}
	else {
		this.statusDiv.style.display = '';
	}
	this.statusDiv.innerHTML = '&nbsp;<b>' + text + '</b>&nbsp;';
	this.positionStatus();
	if(!this.shown) {
		this.shown = true;
		this.statusDiv.style.display = '';
		if(window.onscroll) this.oldOnScroll = window.onscroll; // save onScroll
		window.onscroll = this.positionStatus;
	}
}

// public - hide it
SUGAR.ajaxStatusClass.prototype.hideStatus = function(text) {
	if(!this.shown) return;
	this.shown = false;
	if(this.oldOnScroll) window.onscroll = this.oldOnScroll;
	else window.onscroll = '';
	this.statusDiv.style.display = 'none';
}

SUGAR.ajaxStatusClass.prototype.flashStatus = function(text, time){
	this.showStatus(text);
	window.setTimeout('ajaxStatus.hideStatus();', time);
}


var ajaxStatus = new SUGAR.ajaxStatusClass();
// --- end ajax status class

/**
 * Unified Search Advanced - for global search
 */
SUGAR.unifiedSearchAdvanced = function() {
	var usa_div;
	var usa_img;
	var usa_open;
	var usa_content;
	var anim_open;
	var anim_close;
	
	return {
		init: function() {
			SUGAR.unifiedSearchAdvanced.usa_div = document.getElementById('unified_search_advanced_div'); 
			SUGAR.unifiedSearchAdvanced.usa_img = document.getElementById('unified_search_advanced_img');
			
			if(!SUGAR.unifiedSearchAdvanced.usa_div || !SUGAR.unifiedSearchAdvanced.usa_img) return;
			
			SUGAR.unifiedSearchAdvanced.anim_open = new YAHOO.util.Anim('unified_search_advanced_div', { height: {to: 300} } );
			SUGAR.unifiedSearchAdvanced.anim_open.duration = 0.75;
			SUGAR.unifiedSearchAdvanced.anim_close = new YAHOO.util.Anim('unified_search_advanced_div', { height: {to: 1} } );
			SUGAR.unifiedSearchAdvanced.anim_close.duration = 0.75;
			SUGAR.unifiedSearchAdvanced.anim_close.onComplete.subscribe(function() {SUGAR.unifiedSearchAdvanced.usa_div.style.display = 'none'});
		   
			SUGAR.unifiedSearchAdvanced.usa_img._x = YAHOO.util.Dom.getX(SUGAR.unifiedSearchAdvanced.usa_img);
			SUGAR.unifiedSearchAdvanced.usa_img._y = YAHOO.util.Dom.getY(SUGAR.unifiedSearchAdvanced.usa_img);
		
			
			SUGAR.unifiedSearchAdvanced.usa_open = false;
			SUGAR.unifiedSearchAdvanced.usa_content = null;
	
		   YAHOO.util.Event.addListener('unified_search_advanced_img', 'click', SUGAR.unifiedSearchAdvanced.get_content);
		},
		
		get_content: function(e) {
	   		if(SUGAR.unifiedSearchAdvanced.usa_content == null) { 
		   		ajaxStatus.showStatus(SUGAR.language.get('app_strings', 'LBL_LOADING'));
				var cObj = YAHOO.util.Connect.asyncRequest('GET','index.php?to_pdf=1&module=Home&action=UnifiedSearch&usa_form=true', 
														  {success: SUGAR.unifiedSearchAdvanced.animate, failure: SUGAR.unifiedSearchAdvanced.animate}, null);
			}
			else SUGAR.unifiedSearchAdvanced.animate();
	    },
	    
		animate: function(data) {
			ajaxStatus.hideStatus();
			
			if(data) {
				SUGAR.unifiedSearchAdvanced.usa_content = data.responseText;
				SUGAR.unifiedSearchAdvanced.usa_div.innerHTML = SUGAR.unifiedSearchAdvanced.usa_content;
			}
			if(SUGAR.unifiedSearchAdvanced.usa_open) {
				document.UnifiedSearch.advanced.value = 'false';
				SUGAR.unifiedSearchAdvanced.anim_close.animate();
			}
			else {
				document.UnifiedSearch.advanced.value = 'true';
				SUGAR.unifiedSearchAdvanced.usa_div.style.display = '';
				YAHOO.util.Dom.setX(SUGAR.unifiedSearchAdvanced.usa_div, SUGAR.unifiedSearchAdvanced.usa_img._x - 90);
				YAHOO.util.Dom.setY(SUGAR.unifiedSearchAdvanced.usa_div, SUGAR.unifiedSearchAdvanced.usa_img._y + 15);
				SUGAR.unifiedSearchAdvanced.anim_open.animate();
			}
	      	SUGAR.unifiedSearchAdvanced.usa_open = !SUGAR.unifiedSearchAdvanced.usa_open;
	      	
			return false;
		},
		
		checkUsaAdvanced: function() {
			if(document.UnifiedSearch.advanced.value == 'true') {
				document.UnifiedSearchAdvanced.query_string.value = document.UnifiedSearch.query_string.value;
				document.UnifiedSearchAdvanced.submit();
				return false;
			}
			return true;
		}
};
}();
if(typeof YAHOO != 'undefined') YAHOO.util.Event.addListener(window, 'load', SUGAR.unifiedSearchAdvanced.init);


/**
 * General Sugar Utils
 */ 
SUGAR.util = function () {
	var additionalDetailsCache;
	var additionalDetailsCalls;
	var additionalDetailsRpcCall;
	
	return {
	    evalScript:function(text){
	        objRegex = /<\s*script[^>]*>((.|\s|\v|\0)*?)<\s*\/script\s*>/igm;
            result =  objRegex.exec(text)
              
            while(result){
                  try{
	                  eval(result[1]);
	              } 
	              catch(e) {

                  }
                  result =  objRegex.exec(text)
			}
	    },
		/**
		 * Gets the sidebar object
		 * @return object pointer to the sidebar element
		 */
		getLeftColObj: function() {
			leftColObj = document.getElementById('leftCol');
			while(leftColObj.nodeName != 'TABLE') {
				leftColObj = leftColObj.firstChild;
			}
			leftColTable = leftColObj;
			leftColTd = leftColTable.getElementsByTagName('td')[0];
			leftColTdRegion = YAHOO.util.Dom.getRegion(leftColTd);
			leftColTd.style.width = (leftColTdRegion.right - leftColTdRegion.left) + 'px';
			
			return leftColTd;
		},
		/** 
		 * Fills the shortcut menu placeholders w/ actual content
		 * Call this on load event
		 *
		 * @param shortcutContent Array array of content to fill in
		 */
		fillShortcuts: function(e, shortcutContent) {
	    	spans = document.getElementById('leftCol').getElementsByTagName('span');
			hideCol = document.getElementById('HideMenu').getElementsByTagName('span');
			w = spans.length + 1;
			for(i in hideCol) {
				spans[w] = hideCol[i];
				w++;
			}
		    for(je in shortcutContent) {
		    	for(wp in spans) {
		    		if(typeof spans[wp].innerHTML != 'undefined' && spans[wp].innerHTML == ('wp_shortcut_fill_' + je)) {
		    			if(typeof spans[wp].parentNode.parentNode == 'object') {
		    				if(typeof spans[wp].parentNode.parentNode.onclick != 'undefined') {
		    					spans[wp].parentNode.parentNode.onclick = null;
		    				}
		    				/* If the wp_shortcut span is contained by an A tag, replace the A with a DIV. */
		    				if(spans[wp].parentNode.tagName == 'A' && !isIE) {
		    					var newDiv = document.createElement('DIV');
		    					var parentAnchor = spans[wp].parentNode;
		    					
		    					spans[wp].parentNode.parentNode.style.display = 'none';
		    					
		    					/* Copy styles over to the new container div */
		    					if(window.getComputedStyle) {
			    					var parentStyle = window.getComputedStyle(parentAnchor, '');
			    					for(var styleName in parentStyle) {
				    					if(typeof parentStyle[styleName] != 'function'
	   			    				    && styleName != 'display'
	   			    				    && styleName != 'borderWidth'
				    				    && styleName != 'visibility') {
				    				    	try {
						    					newDiv.style[styleName] = parentStyle[styleName];
						    				} catch(e) {
						    					/* Catches .length and .parentRule, and others */
						    				}
					    				}
				    				}
				    			}
			    				
			    				/* Replace the A with the DIV */
		    					newDiv.appendChild(spans[wp]);
		    					parentAnchor.parentNode.replaceChild(newDiv, parentAnchor);
		    					
		    					spans[wp].parentNode.parentNode.style.display = '';
		    				}
		    			}
			            spans[wp].innerHTML = shortcutContent[je]; // fill w/ content
			            if(spans[wp].style) spans[wp].style.display = '';
		    		}
		    	}
			}
		},
		/** 
		 * Make an AJAX request.
		 *
		 * @param	url				string	resource to load
		 * @param	theDiv			string	id of element to insert loaded data into
		 * @param	postForm		string	if set, a POST request will be made to resource specified by url using the form named by postForm
		 * @param	callback		string	name of function to invoke after HTTP response is recieved
		 * @param	callbackParam	any		parameter to pass to callback when invoked
		 * @param	appendMode		bool	if true, HTTP response will be appended to the contents of theDiv, or else contents will be overriten.
		 */
	    retrieveAndFill: function(url, theDiv, postForm, callback, callbackParam, appendMode) {
			if(typeof theDiv == 'string') {
				try {
					theDiv = document.getElementById(theDiv);
				}
		        catch(e) {
					return;
				}
			}
			
			var success = function(data) {
				if (typeof theDiv != 'undefined' && theDiv != null)
				{
					try {
						if (typeof appendMode != 'undefined' && appendMode)
						{
							theDiv.innerHTML += data.responseText;
						}
						else
						{
							theDiv.innerHTML = data.responseText;
						}
					}
					catch (e) {
						return;
					}
				}
				if (typeof callback != 'undefined' && callback != null) callback(callbackParam);
		  	}

			if(typeof postForm == 'undefined' || postForm == null) {
				var cObj = YAHOO.util.Connect.asyncRequest('GET', url, {success: success, failure: success});
			}
			else {
				YAHOO.util.Connect.setForm(postForm); 
				var cObj = YAHOO.util.Connect.asyncRequest('POST', url, {success: success, failure: success});
			}
		},
		checkMaxLength: function() { // modified from http://www.quirksmode.org/dom/maxlength.html
			var maxLength = this.getAttribute('maxlength');
			var currentLength = this.value.length;
			if (currentLength > maxLength) {
				this.value = this.value.substring(0, maxLength);
			}
			// not innerHTML
		},
		/**
		 * Adds maxlength attribute to textareas
		 */
		setMaxLength: function() { // modified from http://www.quirksmode.org/dom/maxlength.html
			var x = document.getElementsByTagName('textarea');
			for (var i=0;i<x.length;i++) {
				if (x[i].getAttribute('maxlength')) {		
					x[i].onkeyup = x[i].onchange = SUGAR.util.checkMaxLength;
					x[i].onkeyup();
				}
			}
		},

		/**
		 * Retrieves additional details dynamically
		 */		
		getAdditionalDetails: function(bean, id, spanId) {
			go = function() {
				oReturn = function(body, caption, width, theme) {
					return overlib(body, CAPTION, caption, STICKY, MOUSEOFF, 1000, WIDTH, width, CLOSETEXT, ('<img border=0 src=themes/' + theme + '/images/close_inline.gif>'), CLOSETITLE, 'Click to Close', CLOSECLICK, FGCLASS, 'olFgClass', CGCLASS, 'olCgClass', BGCLASS, 'olBgClass', TEXTFONTCLASS, 'olFontClass', CAPTIONFONTCLASS, 'olCapFontClass', CLOSEFONTCLASS, 'olCloseFontClass', REF, spanId, REFC, 'LL', REFX, 13);
				}
				
				success = function(data) {
					eval(data.responseText);

					SUGAR.util.additionalDetailsCache[spanId] = new Array();
					SUGAR.util.additionalDetailsCache[spanId]['body'] = result['body'];
					SUGAR.util.additionalDetailsCache[spanId]['caption'] = result['caption'];
					SUGAR.util.additionalDetailsCache[spanId]['width'] = result['width'];
					SUGAR.util.additionalDetailsCache[spanId]['theme'] = result['theme'];
					ajaxStatus.hideStatus();
					return oReturn(SUGAR.util.additionalDetailsCache[spanId]['body'], SUGAR.util.additionalDetailsCache[spanId]['caption'], SUGAR.util.additionalDetailsCache[spanId]['width'], SUGAR.util.additionalDetailsCache[spanId]['theme']);
				}
	
				if(typeof SUGAR.util.additionalDetailsCache[spanId] != 'undefined') 
					return oReturn(SUGAR.util.additionalDetailsCache[spanId]['body'], SUGAR.util.additionalDetailsCache[spanId]['caption'], SUGAR.util.additionalDetailsCache[spanId]['width'], SUGAR.util.additionalDetailsCache[spanId]['theme']);
				
				if(typeof SUGAR.util.additionalDetailsCalls[spanId] != 'undefined') // call already in progress
					return;
				ajaxStatus.showStatus(SUGAR.language.get('app_strings', 'LBL_LOADING'));
				url = 'index.php?to_pdf=1&module=Home&action=AdditionalDetailsRetrieve&bean=' + bean + '&id=' + id;
				SUGAR.util.additionalDetailsCalls[spanId] = YAHOO.util.Connect.asyncRequest('GET', url, {success: success, failure: success});
				
				return false;
			}
			SUGAR.util.additionalDetailsRpcCall = window.setTimeout('go()', 250);
		},
		clearAdditionalDetailsCall: function() {
			if(typeof SUGAR.util.additionalDetailsRpcCall == 'number') window.clearTimeout(SUGAR.util.additionalDetailsRpcCall);
		}
	};
}(); // end util
SUGAR.util.additionalDetailsCache = new Array();
SUGAR.util.additionalDetailsCalls = new Array();
if(typeof YAHOO != 'undefined') YAHOO.util.Event.addListener(window, 'load', SUGAR.util.setMaxLength); // allow textareas to obey maxlength attrib

SUGAR.savedViews = function() {
	var selectedOrderBy;
	var selectedSortOrder;
	var displayColumns;
	var hideTabs;
	var columnsMeta; // meta data for the display columns
	
	return {
		setChooser: function() {

			var displayColumnsDef = new Array();
			var hideTabsDef = new Array();
	
		    var left_td = document.getElementById('display_tabs_td');	
		    if(typeof left_td == 'undefined' || left_td == null) return; // abort!
		    var right_td = document.getElementById('hide_tabs_td');			
	
		    var displayTabs = left_td.getElementsByTagName('select')[0];
		    var hideTabs = right_td.getElementsByTagName('select')[0];
			
			for(i = 0; i < displayTabs.options.length; i++) {
				displayColumnsDef.push(displayTabs.options[i].value);
			}
			
			if(typeof hideTabs != 'undefined') {
				for(i = 0; i < hideTabs.options.length; i++) {
			         hideTabsDef.push(hideTabs.options[i].value);
				}
			}

			document.getElementById('displayColumnsDef').value = displayColumnsDef.join('|');
			document.getElementById('hideTabsDef').value = hideTabsDef.join('|');
		},
		
		select: function(saved_search_select) {
			for(var wp = 0; wp < document.search_form.saved_search_select.options.length; wp++) {
				if(typeof document.search_form.saved_search_select.options[wp].value != 'undefined' &&
					document.search_form.saved_search_select.options[wp].value == saved_search_select) {
						document.search_form.saved_search_select.selectedIndex = wp;
						document.search_form.ss_delete.style.display = '';
						document.search_form.ss_update.style.display = '';
				}
			}
		},
		saved_search_action: function(action, delete_lang) {
			if(action == 'delete') {
				if(!confirm(delete_lang)) return;
			}
			if(action == 'save') {
				if(document.search_form.saved_search_name.value.replace(/^\s*|\s*$/g, '') == '') {
					alert(SUGAR.language.get('app_strings', 'LBL_SAVED_SEARCH_ERROR'));
					return;
				}
			}
			
			// This check is needed for the Activities module (Calls/Meetings/Tasks).
			if (document.search_form.saved_search_action)
			{
				document.search_form.saved_search_action.value = action;
				document.search_form.search_module.value = document.search_form.module.value;
				document.search_form.module.value = 'SavedSearch';
			}
			document.search_form.submit();
		},
		shortcut_select: function(selectBox, module) {
			document.location.href = 'index.php?module=SavedSearch&search_module=' + module + '&action=index&saved_search_select=' + selectBox.options[selectBox.selectedIndex].value;
		},
		handleForm: function() {
			SUGAR.tabChooser.movementCallback = function(left_side, right_side) {
				while(document.getElementById('orderBySelect').childNodes.length != 0) { // clear out order by options
					document.getElementById('orderBySelect').removeChild(document.getElementById('orderBySelect').lastChild);
				}
				
				var selectedIndex = 0;
				var nodeCount = -1; // need this because the counter i also includes "undefined" nodes
									// which was breaking Calls and Meetings
								
				for(i in left_side.childNodes) { // fill in order by options
					if(typeof left_side.childNodes[i].nodeName != 'undefined' &&
						left_side.childNodes[i].nodeName.toLowerCase() == 'option' &&
						typeof SUGAR.savedViews.columnsMeta[left_side.childNodes[i].value] != 'undefined' && // check if column is sortable
						typeof SUGAR.savedViews.columnsMeta[left_side.childNodes[i].value]['sortable'] == 'undefined' &&
						SUGAR.savedViews.columnsMeta[left_side.childNodes[i].value]['sortable'] != false) {
							nodeCount++;
							optionNode = document.createElement('option');
							optionNode.value = left_side.childNodes[i].value;
							optionNode.innerHTML = left_side.childNodes[i].innerHTML;
							document.getElementById('orderBySelect').appendChild(optionNode);
							if(optionNode.value == SUGAR.savedViews.selectedOrderBy) 
								selectedIndex = nodeCount;
					}
				}
				// Firefox needs this to be set after all the option nodes are created.
				document.getElementById('orderBySelect').selectedIndex = selectedIndex;
			};
			SUGAR.tabChooser.movementCallback(document.getElementById('display_tabs_td').getElementsByTagName('select')[0]);
			
			// This check is needed for the Activities module (Calls/Meetings/Tasks).
			if (document.search_form.orderBy)
				document.search_form.orderBy.options.value = SUGAR.savedViews.selectedOrderBy;

			// handle direction
			if(SUGAR.savedViews.selectedSortOrder == 'DESC') document.getElementById('sort_order_desc_radio').checked = true;
			else document.getElementById('sort_order_asc_radio').checked = true;
		}
	};
}();

SUGAR.searchForm = function() {
	var url;
	
	return {
		// searchForm tab selector util
		searchFormSelect: function(view, previousView) {
			var module = view.split('|')[0];
			var theView = view.split('|')[1];
			// retrieve form
			var handleDisplay = function() { // hide other divs
				document.search_form.searchFormTab.value = theView;
				
				document.getElementById(module + 'basic_searchSearchForm').style.display = 'none';
				document.getElementById(module + 'advanced_searchSearchForm').style.display = 'none';
				document.getElementById(module + 'saved_viewsSearchForm').style.display = 'none';
				
				document.getElementById(module + theView + 'SearchForm').style.display = '';
				switch(theView) {
					case 'saved_views':  // handle all the javascript for saved view tab
						SUGAR.savedViews.handleForm();
						break;
					case 'basic_search': // copy advanced search to basic
						if(previousView == '' || previousView.split('|')[1] == 'advanced_search') {
							for(num in document.search_form.elements) {
								if(document.search_form.elements[num]) {
									el = document.search_form.elements[num];
									if(typeof el.type != 'undefined' && typeof el.name != 'undefined' && !el.name.match(/^(.*)_basic$/)) {
										basic_input_name = el.name + '_basic'; // add _basic
										if(typeof document.search_form[basic_input_name] != 'undefined')  // if basic input of same name exists
											SUGAR.searchForm.copyElement(basic_input_name, el);
									}
								}
							}
						}
						break;
					case 'advanced_search': // copy basic search elements to advanced search
						if(previousView == '' || previousView.split('|')[1] == 'basic_search') {
							for(num in document.search_form.elements) {
								if(document.search_form.elements[num]) {
									el = document.search_form.elements[num];
									if(typeof el.type != 'undefined' && typeof el.name != 'undefined' && el.name.match(/^(.*)_basic$/)) {
										advanced_input_name = el.name.match(/^(.*)_basic$/)[1]; // strip _basic
										if(typeof document.search_form[advanced_input_name] != 'undefined')  // if advanced input of same name exists
											SUGAR.searchForm.copyElement(advanced_input_name, el);
									}
								}
							}
						}
						break;
				}
			}
			
			// if tab is not cached
			if(document.getElementById(module + theView + 'SearchForm').innerHTML == '') { 
				ajaxStatus.showStatus(SUGAR.language.get('app_strings', 'LBL_LOADING'));
				var success = function(data) {
					document.getElementById(module + theView + 'SearchForm').innerHTML = data.responseText;
					
					SUGAR.util.evalScript(data.responseText);
					// pass script variables to global scope
					if(theView == 'saved_views') {
						if(typeof columnsMeta != 'undefined') SUGAR.savedViews.columnsMeta = columnsMeta; 
						if(typeof selectedOrderBy != 'undefined') SUGAR.savedViews.selectedOrderBy = selectedOrderBy;
						if(typeof selectedSortOrder != 'undefined') SUGAR.savedViews.selectedSortOrder = selectedSortOrder;
					}

					handleDisplay();					
					ajaxStatus.hideStatus();
				}
				url = 	'index.php?module=' + module + '&action=ListView&search_form_only=true&to_pdf=true&search_form_view=' + theView;
				if(theView == 'saved_views') // handle the tab chooser
					url += '&displayColumns=' + SUGAR.savedViews.displayColumns + '&hideTabs=' + SUGAR.savedViews.hideTabs + '&orderBy=' + SUGAR.savedViews.selectedOrderBy + '&sortOrder=' + SUGAR.savedViews.selectedSortOrder;
					
				var cObj = YAHOO.util.Connect.asyncRequest('GET', url, {success: success, failure: success});
			}
			else { // that form already retrieved
				handleDisplay();
			}
		},
		
		// copies one input to another
		copyElement: function(inputName, copyFromElement) {
			switch(copyFromElement.type) {
				case 'select-one':
				case 'text':
					document.search_form[inputName].value = copyFromElement.value;
					break;
			}
		},
		
		clear_form: function(form) {
			var newLoc = 'index.php?action=' + form.action.value + '&module=' + form.module.value + '&query=true&clear_query=true';
			if(typeof(form.searchFormTab) != 'undefined'){
				newLoc += '&searchFormTab=' + form.searchFormTab.value;
			}
			document.location.href = newLoc;
		} 
	};
}();
// Code for the column/tab chooser used on homepage and in admin section
SUGAR.tabChooser = function () {
	var	object_refs = new Array();
	return {
			/* Describe certain transfers as invalid */
			frozenOptions: [],
			
			movementCallback: function(left_side, right_side) {},
			orderCallback: function(left_side, right_side) {},
			
			freezeOptions: function(left_name, right_name, target) {
				if(!SUGAR.tabChooser.frozenOptions) { SUGAR.tabChooser.frozenOptions = []; }
				if(!SUGAR.tabChooser.frozenOptions[left_name]) { SUGAR.tabChooser.frozenOptions[left_name] = []; }
				if(!SUGAR.tabChooser.frozenOptions[left_name][right_name]) { SUGAR.tabChooser.frozenOptions[left_name][right_name] = []; }
				if(typeof target == 'array') {
					for(var i in target) {
						SUGAR.tabChooser.frozenOptions[left_name][right_name][target[i]] = true;
					}
				} else {
					SUGAR.tabChooser.frozenOptions[left_name][right_name][target] = true;
				}
			},
			
			buildSelectHTML: function(info) {
				var text = "<select";
		
		        if(typeof (info['select']['size']) != 'undefined') {
		                text +=" size=\""+ info['select']['size'] +"\"";
		        }
		
		        if(typeof (info['select']['name']) != 'undefined') {
		                text +=" name=\""+ info['select']['name'] +"\"";
		        }
		
		        if(typeof (info['select']['style']) != 'undefined') {
		                text +=" style=\""+ info['select']['style'] +"\"";
		        }
		
		        if(typeof (info['select']['onchange']) != 'undefined') {
		                text +=" onChange=\""+ info['select']['onchange'] +"\"";
		        }
		
		        if(typeof (info['select']['multiple']) != 'undefined') {
		                text +=" multiple";
		        }
		        text +=">";
		
		        for(i=0; i<info['options'].length;i++) {
		                option = info['options'][i];
		                text += "<option value=\""+option['value']+"\" ";
		                if ( typeof (option['selected']) != 'undefined' && option['selected']== true) {
		                        text += "SELECTED";
		                }
		                text += ">"+option['text']+"</option>";
		        }
		        text += "</select>";
		        return text;
			},
			
			left_to_right: function(left_name, right_name, left_size, right_size) {
			    var left_td = document.getElementById(left_name+'_td');
			    var right_td = document.getElementById(right_name+'_td');
			
			    var display_columns_ref = left_td.getElementsByTagName('select')[0];
			    var hidden_columns_ref = right_td.getElementsByTagName('select')[0];
			    
			    var selected_left = new Array();
			    var notselected_left = new Array();
			    var notselected_right = new Array();
			
			    var left_array = new Array();
			    
			    var frozen_options = SUGAR.tabChooser.frozenOptions;
			    frozen_options = frozen_options && frozen_options[left_name] && frozen_options[left_name][right_name]?frozen_options[left_name][right_name]:[];
				
			        // determine which options are selected in left 
			    for (i=0; i < display_columns_ref.options.length; i++)
			    {
			        if ( display_columns_ref.options[i].selected == true && !frozen_options[display_columns_ref.options[i].value]) 
			        {
			            selected_left[selected_left.length] = {text: display_columns_ref.options[i].text, value: display_columns_ref.options[i].value};
			        }
			        else
			        {
			            notselected_left[notselected_left.length] = {text: display_columns_ref.options[i].text, value: display_columns_ref.options[i].value};
			        }
			        
			    }
			
			    for (i=0; i < hidden_columns_ref.options.length; i++)
			    {
			        notselected_right[notselected_right.length] = {text:hidden_columns_ref.options[i].text, value:hidden_columns_ref.options[i].value};
			        
			    }
			
			    var left_select_html_info = new Object();
			    var left_options = new Array();
			    var left_select = new Object();
			
			    left_select['name'] = left_name+'[]';
			    left_select['id'] = left_name;
			    left_select['size'] = left_size;
			    left_select['multiple'] = 'true';
			
			    var right_select_html_info = new Object();
			    var right_options = new Array();
			    var right_select = new Object();
			
			    right_select['name'] = right_name+'[]';
			    right_select['id'] = right_name;
			    right_select['size'] = right_size;
			    right_select['multiple'] = 'true';
			
			    for (i = 0; i < notselected_right.length; i++) {
			        right_options[right_options.length] = notselected_right[i];    
			    }
			
			    for (i = 0; i < selected_left.length; i++) {
			        right_options[right_options.length] = selected_left[i];    
			    }
			    for (i = 0; i < notselected_left.length; i++) {
			        left_options[left_options.length] = notselected_left[i];    
			    }
			    left_select_html_info['options'] = left_options;
			    left_select_html_info['select'] = left_select;
			    right_select_html_info['options'] = right_options;
			    right_select_html_info['select'] = right_select;
			    right_select_html_info['style'] = 'background: lightgrey';
			
			    var left_html = this.buildSelectHTML(left_select_html_info);
			    var right_html = this.buildSelectHTML(right_select_html_info);
			
			    left_td.innerHTML = left_html;
			    right_td.innerHTML = right_html;

				object_refs[left_name] = left_td.getElementsByTagName('select')[0];
				object_refs[right_name] = right_td.getElementsByTagName('select')[0];
				
				this.movementCallback(object_refs[left_name], object_refs[right_name]);
				
			    return false;
			},


			right_to_left: function(left_name, right_name, left_size, right_size, max_left) {
			    var left_td = document.getElementById(left_name+'_td');
			    var right_td = document.getElementById(right_name+'_td');
			    
			    var display_columns_ref = left_td.getElementsByTagName('select')[0];
			    var hidden_columns_ref = right_td.getElementsByTagName('select')[0];
			    
			    var selected_right = new Array();
			    var notselected_right = new Array();
			    var notselected_left = new Array();
			    
			    var frozen_options = SUGAR.tabChooser.frozenOptions;
			    frozen_options = SUGAR.tabChooser.frozenOptions && SUGAR.tabChooser.frozenOptions[right_name] && SUGAR.tabChooser.frozenOptions[right_name][left_name]?SUGAR.tabChooser.frozenOptions[right_name][left_name]:[];
			
			    for (i=0; i < hidden_columns_ref.options.length; i++)
			    {
			        if (hidden_columns_ref.options[i].selected == true && !frozen_options[hidden_columns_ref.options[i].value]) 
			        {
			            selected_right[selected_right.length] = {text:hidden_columns_ref.options[i].text, value:hidden_columns_ref.options[i].value};
			        }
			        else
			        {
			            notselected_right[notselected_right.length] = {text:hidden_columns_ref.options[i].text, value:hidden_columns_ref.options[i].value};
			        }
			        
			    }
			    
			    if(max_left != '' && (display_columns_ref.length + selected_right.length) > max_left) {
			    	alert('Maximum of ' + max_left + ' columns can be displayed.');
					return;
			    }
			    
			    for (i=0; i < display_columns_ref.options.length; i++)
			    {
			        notselected_left[notselected_left.length] = {text:display_columns_ref.options[i].text, value:display_columns_ref.options[i].value};
			        
			    }
			
			    var left_select_html_info = new Object();
			    var left_options = new Array();
			    var left_select = new Object();
			
			    left_select['name'] = left_name+'[]';
			    left_select['id'] = left_name;
			    left_select['multiple'] = 'true';
			    left_select['size'] = left_size;
			
			    var right_select_html_info = new Object();
			    var right_options = new Array();
			    var right_select = new Object();
			
			    right_select['name'] = right_name+ '[]';
			    right_select['id'] = right_name;
			    right_select['multiple'] = 'true';
			    right_select['size'] = right_size;
			
			    for (i = 0; i < notselected_left.length; i++) {
			        left_options[left_options.length] = notselected_left[i];    
			    }
			
			    for (i = 0; i < selected_right.length; i++) {
			        left_options[left_options.length] = selected_right[i];    
			    }
			    for (i = 0; i < notselected_right.length; i++) {
			        right_options[right_options.length] = notselected_right[i];    
			    }
			    left_select_html_info['options'] = left_options;
			    left_select_html_info['select'] = left_select;
			    right_select_html_info['options'] = right_options;
			    right_select_html_info['select'] = right_select;
			    right_select_html_info['style'] = 'background: lightgrey';
			
			    var left_html = this.buildSelectHTML(left_select_html_info);
			    var right_html = this.buildSelectHTML(right_select_html_info);
			
			    left_td.innerHTML = left_html;
			    right_td.innerHTML = right_html;

				object_refs[left_name] = left_td.getElementsByTagName('select')[0];
				object_refs[right_name] = right_td.getElementsByTagName('select')[0];
				
				this.movementCallback(object_refs[left_name], object_refs[right_name]);
				
			    return false;
			},

			up: function(name, left_name, right_name) {
			    var left_td = document.getElementById(left_name+'_td');
			    var right_td = document.getElementById(right_name+'_td');			
			    var td = document.getElementById(name+'_td');
			    var obj = td.getElementsByTagName('select')[0];
			    obj = (typeof obj == "string") ? document.getElementById(obj) : obj;
			    if (obj.tagName.toLowerCase() != "select" && obj.length < 2)
			        return false;
			    var sel = new Array();
			
			    for (i=0; i<obj.length; i++) {
			        if (obj[i].selected == true) {
			            sel[sel.length] = i;
			        }
			    }
			    for (i in sel) {
			        if (sel[i] != 0 && !obj[sel[i]-1].selected) {
			            var tmp = new Array(obj[sel[i]-1].text, obj[sel[i]-1].value);
			            obj[sel[i]-1].text = obj[sel[i]].text;
			            obj[sel[i]-1].value = obj[sel[i]].value;
			            obj[sel[i]].text = tmp[0];
			            obj[sel[i]].value = tmp[1];
			            obj[sel[i]-1].selected = true;
			            obj[sel[i]].selected = false;
			        }
			    }
			    
				object_refs[left_name] = left_td.getElementsByTagName('select')[0];
				object_refs[right_name] = right_td.getElementsByTagName('select')[0];

				this.orderCallback(object_refs[left_name], object_refs[right_name]);

			    return false;
			},
			
			down: function(name, left_name, right_name) {
			   	var left_td = document.getElementById(left_name+'_td');
			    var right_td = document.getElementById(right_name+'_td');				
			    var td = document.getElementById(name+'_td');
			    var obj = td.getElementsByTagName('select')[0];
			    if (obj.tagName.toLowerCase() != "select" && obj.length < 2)
			        return false;
			    var sel = new Array();
			    for (i=obj.length-1; i>-1; i--) {
			        if (obj[i].selected == true) {
			            sel[sel.length] = i;
			        }
			    }
			    for (i in sel) {
			        if (sel[i] != obj.length-1 && !obj[sel[i]+1].selected) {
			            var tmp = new Array(obj[sel[i]+1].text, obj[sel[i]+1].value);
			            obj[sel[i]+1].text = obj[sel[i]].text;
			            obj[sel[i]+1].value = obj[sel[i]].value;
			            obj[sel[i]].text = tmp[0];
			            obj[sel[i]].value = tmp[1];
			            obj[sel[i]+1].selected = true;
			            obj[sel[i]].selected = false;
			        }
			    }
			    
				object_refs[left_name] = left_td.getElementsByTagName('select')[0];
				object_refs[right_name] = right_td.getElementsByTagName('select')[0];
				
				this.orderCallback(object_refs[left_name], object_refs[right_name]);

			    return false;
			}
		};
}(); // end tabChooser

SUGAR.language = function() {
	var languages = new Array();
	
	return {
		setLanguage: function(module, data) {
			languages[module] = data;
		},
		
		get: function(module, str) {
			if(typeof languages[module] == 'undefined' || typeof languages[module][str] == 'undefined') return 'undefined';
			return languages[module][str];
		}
	};
}();

SUGAR.contextMenu = function() {
	return {
		objects: new Object(),
		objectTypes: new Object(),
		/**
		 * Registers a new object for the context menu.
		 * objectType - name of the type
		 * id - element id
		 * metaData - metaData to pass to the action function
		 **/
		registerObject: function(objectType, id, metaData) {
			SUGAR.contextMenu.objects[id] = new Object();
            SUGAR.contextMenu.objects[id] = {'objectType' : objectType, 'metaData' : metaData};
		},
		/**
		 * Registers a new object type
		 * name - name of the type
		 * menuItems - array of menu items
		 **/
		registerObjectType: function(name, menuItems) {
			SUGAR.contextMenu.objectTypes[name] = new Object();
			SUGAR.contextMenu.objectTypes[name] = {'menuItems' : menuItems, 'objects' : new Array()};
		},
		/**
		 * Determines which menu item was clicked
		 **/ 
		getListItemFromEventTarget: function(p_oNode) {
            var oLI;
            if(p_oNode.tagName == "LI") {
	            oLI = p_oNode;
            }
            else {
	            do {
	                if(p_oNode.tagName == "LI") {
	                    oLI = p_oNode;                            
	                    break;
	                }
	        
	            } while((p_oNode = p_oNode.parentNode));
  	        }
            return oLI;
         },
         /**
          * handles movement within context menu
          **/ 
         onContextMenuMove: function() {
            var oNode = this.contextEventTarget;
            var bDisabled = (oNode.tagName == "UL");
            var i = this.getItemGroups()[0].length - 1;
            do {
                this.getItem(i).cfg.setProperty("disabled", bDisabled);
            }
            while(i--);
        },
        /**
         * handles clicks on a context menu ITEM
         **/
		onContextMenuItemClick: function(p_sType, p_aArguments, p_oItem) {
            var oLI = SUGAR.contextMenu.getListItemFromEventTarget(this.parent.contextEventTarget);
            id = this.parent.contextEventTarget.parentNode.id; // id of the target
            funct = eval(SUGAR.contextMenu.objectTypes[SUGAR.contextMenu.objects[id]['objectType']]['menuItems'][this.index]['action']);
            funct(this.parent.contextEventTarget, SUGAR.contextMenu.objects[id]['metaData']);
		},
		/**
		 * Initializes all context menus registered
		 **/ 
		init: function() {
			for(var i in SUGAR.contextMenu.objects) { // make a variable called objects in objectTypes containg references to all triggers
                if(typeof SUGAR.contextMenu.objectTypes[SUGAR.contextMenu.objects[i]['objectType']]['objects'] == 'undefined') 
                    SUGAR.contextMenu.objectTypes[SUGAR.contextMenu.objects[i]['objectType']]['objects'] = new Array(); 
				SUGAR.contextMenu.objectTypes[SUGAR.contextMenu.objects[i]['objectType']]['objects'].push(document.getElementById(i));
			}
            // register the menus
			for(var i in SUGAR.contextMenu.objectTypes) {		
	            var oContextMenu = new YAHOO.widget.ContextMenu(i, {'trigger': SUGAR.contextMenu.objectTypes[i]['objects']});
				var aMainMenuItems = SUGAR.contextMenu.objectTypes[i]['menuItems'];
	            var nMainMenuItems = aMainMenuItems.length;
	            var oMenuItem;
	            for(var j = 0; j < nMainMenuItems; j++) {
	                oMenuItem = new YAHOO.widget.ContextMenuItem(aMainMenuItems[j].text, { helptext: aMainMenuItems[j].helptext });
	                oMenuItem.clickEvent.subscribe(SUGAR.contextMenu.onContextMenuItemClick, oMenuItem, true);
	                oContextMenu.addItem(oMenuItem);
	            }
	            //  Add a "move" event handler to the context menu 
	            oContextMenu.moveEvent.subscribe(SUGAR.contextMenu.onContextMenuMove, oContextMenu, true);
	            // Add a "keydown" event handler to the context menu
	            oContextMenu.keyDownEvent.subscribe(SUGAR.contextMenu.onContextMenuItemClick, oContextMenu, true);
	            // Render the context menu
	            oContextMenu.render(document.body);
	        }
		}
	};
}();

SUGAR.contextMenu.actions = function() {
	return {
		/**
		 * redirects to a new note with the clicked on object as the target
		 **/
		createNote: function(itemClicked, metaData) {
			loc = 'index.php?module=Notes&action=EditView';
			for(i in metaData) {
				if(i == 'notes_parent_type') loc += '&parent_type=' + metaData[i];
				else if(i != 'module' && i != 'parent_type') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * redirects to a new note with the clicked on object as the target
		 **/
		scheduleMeeting: function(itemClicked, metaData) {
			loc = 'index.php?module=Meetings&action=EditView';
			for(i in metaData) {
				if(i != 'module') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * redirects to a new note with the clicked on object as the target
		 **/
		scheduleCall: function(itemClicked, metaData) {
			loc = 'index.php?module=Calls&action=EditView';
			for(i in metaData) {
				if(i != 'module') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * redirects to a new contact with the clicked on object as the target
		 **/
		createContact: function(itemClicked, metaData) {
			loc = 'index.php?module=Contacts&action=EditView';
			for(i in metaData) {
				if(i != 'module') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * redirects to a new task with the clicked on object as the target
		 **/
		createTask: function(itemClicked, metaData) {
			loc = 'index.php?module=Tasks&action=EditView';
			for(i in metaData) {
				if(i != 'module') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * redirects to a new opportunity with the clicked on object as the target
		 **/
		createOpportunity: function(itemClicked, metaData) {
			loc = 'index.php?module=Opportunities&action=EditView';
			for(i in metaData) {
				if(i != 'module') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * redirects to a new opportunity with the clicked on object as the target
		 **/
		createCase: function(itemClicked, metaData) {
			loc = 'index.php?module=Cases&action=EditView';
			for(i in metaData) {
				if(i != 'module') loc += '&' + i + '=' + metaData[i];
			}
			document.location = loc;
		},
		/**
		 * handles add to favorites menu selection
		 **/ 
		addToFavorites: function(itemClicked, metaData) {
			success = function(data) {
				console.log(data.responseText);
			}
			var cObj = YAHOO.util.Connect.asyncRequest('GET', 'index.php?to_pdf=true&module=Home&action=AddToFavorites&target_id=' + metaData['id'] + '&target_module=' + metaData['module'], {success: success, failure: success});
			
		}
	};
}();
//if(typeof YAHOO != 'undefined') YAHOO.util.Event.addListener(window, 'load', SUGAR.contextMenu.init);
