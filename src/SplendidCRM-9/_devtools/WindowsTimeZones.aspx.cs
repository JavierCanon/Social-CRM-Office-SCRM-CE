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
using System.Diagnostics;
//using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace SplendidCRM._devtools
{
	/// <summary>
	/// Summary description for WindowsTimeZones.
	/// http://www.codeproject.com/dotnet/WorldClock.asp?df=100&forumid=126704&exp=0&select=981883
	/// </summary>
	public class WindowsTimeZones : System.Web.UI.Page
	{
		[StructLayout( LayoutKind.Sequential )]
		private struct SYSTEMTIME
		{
			public UInt16 wYear        ;
			public UInt16 wMonth       ;
			public UInt16 wDayOfWeek   ;
			public UInt16 wDay         ;
			public UInt16 wHour        ;
			public UInt16 wMinute      ;
			public UInt16 wSecond      ;
			public UInt16 wMilliseconds;
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct TZI
		{
			public int        nBias         ;
			public int        nStandardBias ;
			public int        nDaylightBias ;
			public SYSTEMTIME dtStandardDate;
			public SYSTEMTIME dtDaylightDate;
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN || Request.ServerVariables["SERVER_NAME"] != "localhost" )
				return;
			StringBuilder sbSQL = new StringBuilder();
			RegistryKey keyTimeZones = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones");
			if ( keyTimeZones != null )
			{
				Response.Write("<table border=1 cellspacing=0 cellpadding=4>" + ControlChars.CrLf);
				Response.Write("	<tr>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Name</th>" + ControlChars.CrLf);
				Response.Write("		<th>Display  Name</th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Name</th>" + ControlChars.CrLf);

				Response.Write("		<th>Bias         </th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Bias</th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Bias</th>" + ControlChars.CrLf);

				Response.Write("		<th>Standard Date       </th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Year       </th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Month      </th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Day        </th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Day Of Week</th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Hour       </th>" + ControlChars.CrLf);
				Response.Write("		<th>Standard Minute     </th>" + ControlChars.CrLf);

				Response.Write("		<th>Daylight Date       </th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Year       </th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Month      </th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Day        </th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Day Of Week</th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Hour       </th>" + ControlChars.CrLf);
				Response.Write("		<th>Daylight Minute     </th>" + ControlChars.CrLf);
				Response.Write("	</tr>" + ControlChars.CrLf);

				foreach ( string sTimeZone in keyTimeZones.GetSubKeyNames() )
				{
					RegistryKey keyTimeZone = keyTimeZones.OpenSubKey(sTimeZone);
					string sStandardName = keyTimeZone.GetValue("Std"    ).ToString();
					string sDisplayName  = keyTimeZone.GetValue("Display").ToString();
					string sDaylightName = keyTimeZone.GetValue("Dlt"    ).ToString();
					byte[] byTZI         = (byte[]) keyTimeZone.GetValue("TZI");

					TZI tzi ;
					GCHandle h = GCHandle.Alloc(byTZI, GCHandleType.Pinned);
					try
					{
						tzi = (TZI) Marshal.PtrToStructure( h.AddrOfPinnedObject(), typeof(TZI) );
					}
					finally
					{
						h.Free();
					}

					sbSQL.Append("exec dbo.spTIMEZONES_UpdateByName null");
					sbSQL.Append(", '" + sDisplayName .Replace("'", "''") + "'" + Strings.Space(61-sDisplayName.Length));
					sbSQL.Append(", '" + sStandardName.Replace("'", "''") + "'" + Strings.Space(31-sStandardName.Length));
					sbSQL.Append(", ''");
					sbSQL.Append(", '" + sDaylightName.Replace("'", "''") + "'" + Strings.Space(31-sDaylightName.Length));
					sbSQL.Append(", ''");
					sbSQL.Append(", " + tzi.nBias                     );
					sbSQL.Append(", " + tzi.nStandardBias             );
					sbSQL.Append(", " + tzi.nDaylightBias             );
					sbSQL.Append(", " + tzi.dtStandardDate.wYear      );
					sbSQL.Append(", " + tzi.dtStandardDate.wMonth     );
					sbSQL.Append(", " + tzi.dtStandardDate.wDay       );  // Week
					sbSQL.Append(", " + tzi.dtStandardDate.wDayOfWeek );
					sbSQL.Append(", " + tzi.dtStandardDate.wHour      );
					sbSQL.Append(", " + tzi.dtStandardDate.wMinute    );
					sbSQL.Append(", " + tzi.dtDaylightDate.wYear      );
					sbSQL.Append(", " + tzi.dtDaylightDate.wMonth     );
					sbSQL.Append(", " + tzi.dtDaylightDate.wDay       );  // Week
					sbSQL.Append(", " + tzi.dtDaylightDate.wDayOfWeek );
					sbSQL.Append(", " + tzi.dtDaylightDate.wHour      );
					sbSQL.Append(", " + tzi.dtDaylightDate.wMinute    );
					sbSQL.Append(ControlChars.CrLf);

					Response.Write("	<tr>" + ControlChars.CrLf);
					Response.Write("		<td>" + sDisplayName       + "</td>" + ControlChars.CrLf);
					Response.Write("		<td>" + sStandardName      + "</td>" + ControlChars.CrLf);
					Response.Write("		<td>" + sDaylightName      + "</td>" + ControlChars.CrLf);
					Response.Write("		<td>" + tzi.nBias          + "</td>" + ControlChars.CrLf);
					Response.Write("		<td>" + tzi.nStandardBias  + "</td>" + ControlChars.CrLf);
					Response.Write("		<td>" + tzi.nDaylightBias  + "</td>" + ControlChars.CrLf);
					int nThisYear = DateTime.Today.Year;
					if ( tzi.dtStandardDate.wMonth > 0 )
					{
						DateTime dtStandardDate = new DateTime(nThisYear, tzi.dtStandardDate.wMonth, 1, tzi.dtStandardDate.wHour, tzi.dtStandardDate.wMinute, tzi.dtStandardDate.wSecond, tzi.dtStandardDate.wMilliseconds);
						dtStandardDate = dtStandardDate.AddDays(7 - (dtStandardDate.DayOfWeek - DayOfWeek.Sunday));  // First Sunday in the month. 
						dtStandardDate = dtStandardDate.AddDays(7 * (tzi.dtStandardDate.wDay - 1));  // Last Sunday, but might overflow.  5 means last Sunday. 
						while ( dtStandardDate.Month != tzi.dtStandardDate.wMonth )
							dtStandardDate = dtStandardDate.AddDays(-7);  // In case of overflow, subtract a week until the month matches. 
						Response.Write("		<td>" + dtStandardDate.ToString()     + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtStandardDate.wYear      + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtStandardDate.wMonth     + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtStandardDate.wDay       + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtStandardDate.wDayOfWeek + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtStandardDate.wHour      + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtStandardDate.wMinute    + "</td>" + ControlChars.CrLf);
					}
					else
					{
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
					}
					// Day is actually the week. 
					// Day of Week is typically 0 for Sunday, but Egypt has a 3. 
					if ( tzi.dtDaylightDate.wMonth > 0 )
					{
						DateTime dtDaylightDate = new DateTime(nThisYear, tzi.dtDaylightDate.wMonth, 1, tzi.dtDaylightDate.wHour, tzi.dtDaylightDate.wMinute, tzi.dtDaylightDate.wSecond, tzi.dtDaylightDate.wMilliseconds);
						dtDaylightDate = dtDaylightDate.AddDays(7 - (dtDaylightDate.DayOfWeek - DayOfWeek.Sunday));  // First Sunday in the month. 
						dtDaylightDate = dtDaylightDate.AddDays(7 * (tzi.dtDaylightDate.wDay - 1));  // Last Sunday, but might overflow.  5 means last Sunday. 
						while ( dtDaylightDate.Month != tzi.dtDaylightDate.wMonth )
							dtDaylightDate = dtDaylightDate.AddDays(-7);  // In case of overflow, subtract a week until the month matches. 
						Response.Write("		<td>" + dtDaylightDate.ToString()     + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtDaylightDate.wYear      + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtDaylightDate.wMonth     + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtDaylightDate.wDay       + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtDaylightDate.wDayOfWeek + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtDaylightDate.wHour      + "</td>" + ControlChars.CrLf);
						Response.Write("		<td>" + tzi.dtDaylightDate.wMinute    + "</td>" + ControlChars.CrLf);
					}
					else
					{
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
						Response.Write("		<td>&nbsp;</td>");
					}
					Response.Write("	</tr>" + ControlChars.CrLf);
				}
				sbSQL.Append(ControlChars.CrLf);
				Response.Write("</table>" + ControlChars.CrLf);
				Response.Write("<pre>");
				Response.Write(sbSQL.ToString());
				Response.Write("</pre>");
			}
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
