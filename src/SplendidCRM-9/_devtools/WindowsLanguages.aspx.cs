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
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Diagnostics;
//using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for WindowsTimeZones.
	/// http://www.codeproject.com/dotnet/WorldClock.asp?df=100&forumid=126704&exp=0&select=981883
	/// </summary>
	public class WindowsLanguages : System.Web.UI.Page
	{
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			StringBuilder sbSQL = new StringBuilder();
			Response.Write("<table border=1 cellspacing=0 cellpadding=4>" + ControlChars.CrLf);
			Response.Write("	<tr>" + ControlChars.CrLf);
			Response.Write("		<th>Name        </th>" + ControlChars.CrLf);
			Response.Write("		<th>LCID        </th>" + ControlChars.CrLf);
			Response.Write("		<th>Native Name</th>" + ControlChars.CrLf);
			Response.Write("		<th>Display Name</th>" + ControlChars.CrLf);
			Response.Write("	</tr>" + ControlChars.CrLf);

			CultureInfo[] aCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			foreach ( CultureInfo culture in aCultures)
			{
				Response.Write("	<tr>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.Name        + "</td>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.LCID        + "</td>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.NativeName  + "</td>" + ControlChars.CrLf);
				Response.Write("		<td>" + culture.DisplayName + "</td>" + ControlChars.CrLf);
				Response.Write("	</tr>" + ControlChars.CrLf);

				sbSQL.Append("--exec dbo.spLANGUAGES_InsertOnly null ");
				sbSQL.Append(", '" + culture.Name + "'" + Strings.Space(10-culture.Name.Length));
				sbSQL.Append(", " + Strings.Space(5-culture.LCID.ToString().Length) + culture.LCID );
				sbSQL.Append(", 0" );
				sbSQL.Append(", '" + culture.NativeName .Replace("'", "''") + "'");
				sbSQL.Append(", '" + culture.DisplayName.Replace("'", "''") + "'");
				sbSQL.Append(ControlChars.CrLf);

			}
			Response.Write("</table>" + ControlChars.CrLf);
			Response.Write("<pre>");
			Response.Write(sbSQL.ToString());
			Response.Write("</pre>");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
