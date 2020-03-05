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
 * Portions created by SplendidCRM Software are Copyright (C) 2006 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Text;
//using MySql.Data.MySqlClient;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for CsvDataReader.
	/// </summary>
	public class CsvDataReader
	{
		private DataTable m_tbl;

		public DataTable Table
		{
			get { return m_tbl; }
		}

		public CsvDataReader(Stream stm) : this(stm, ',')
		{
		}

		public CsvDataReader(Stream stm, char chFieldSeparator)
		{
			m_tbl = new DataTable();
			using ( TextReader reader = new StreamReader(stm) )
			{
				m_tbl.Columns.Add("Column001");

				string sLine = null;
				while ( (sLine = reader.ReadLine()) != null )
				{
					if ( sLine.Length == 0 )
						continue;

					DataRow row = m_tbl.NewRow();
					m_tbl.Rows.Add(row);
					
					int i = 0;
					int nMode = 0;
					int nField = 0;
					bool bContinueParsing = true;
					while ( bContinueParsing )
					{
						switch ( nMode )
						{
							case 0:  // Search for next entry. 
							{
								if ( chFieldSeparator == ControlChars.Tab )
								{
									// Don't skip the tab when it is used as a separator. 
									while ( Char.IsWhiteSpace(sLine[i]) && sLine[i] != ControlChars.Tab )
										i++;
								}
								else
								{
									while ( Char.IsWhiteSpace(sLine[i]) )
										i++;
								}
								nMode = 1;
								break;
							}
							case 1:  // Determine if field is quoted or unquoted. 
							{
								// first check if field is empty. 
								char chPunctuation = sLine[i];
								if ( chPunctuation == chFieldSeparator )
								{
									i++;
									nField++;
									if ( nField >= m_tbl.Columns.Count )
										m_tbl.Columns.Add("Column" + nField.ToString("000"));
									nMode = 0;
								}
								if ( chPunctuation == '\"' )
								{
									i++;
									// Field is quoted, so start reading until next quote. 
									nMode = 3;
								}
								else
								{
									// Field is unquoted, so start reading until next separator or end-of-line.
									nMode = 2;
								}
								break;
							}
							case 2:  // Extract unquoted field. 
							{
								nField++;
								if ( nField > m_tbl.Columns.Count )
									m_tbl.Columns.Add("Column" + nField.ToString("000"));
								
								int nFieldStart = i;
								// Field is unquoted, so start reading until next separator or end-of-line.
								while ( i < sLine.Length && sLine[i] != chFieldSeparator )
									i++;
								int nFieldEnd = i;
								
								string sField = sLine.Substring(nFieldStart, nFieldEnd-nFieldStart);
								row[nField-1] = sField;
								nMode = 0;
								i++;
								break;
							}
							case 3:  // Extract quoted field. 
							{
								nField++;
								if ( nField > m_tbl.Columns.Count )
									m_tbl.Columns.Add("Column" + nField.ToString("000"));
								
								bool bMultiline = false;
								StringBuilder sbField = new StringBuilder();
								do
								{
									int nFieldStart = i;
									// Field is quoted, so start reading until next quote.  Watch out for an escaped quote (two double quotes). 
									while ( ( i < sLine.Length && sLine[i] != '\"' ) || ( i + 1 < sLine.Length && sLine[i] == '\"' && sLine[i + 1] == '\"' ) )
									{
										if ( i + 1 < sLine.Length && sLine[i] == '\"' && sLine[i + 1] == '\"' )
											i++;
										i++;
									}
									int nFieldEnd = i;
									if ( sbField.Length > 0 )
										sbField.Append(ControlChars.CrLf);
									sbField.Append(sLine.Substring(nFieldStart, nFieldEnd - nFieldStart));
									
									// 08/23/2006 Paul.  If we are at the end of the line, then it must be a multi-line string. 
									bMultiline = (i == sLine.Length);
									if ( bMultiline )
									{
										sLine = reader.ReadLine();
										i = 0;
										if ( sLine == null )
											break;
									}
								}
								while ( bMultiline );

								if ( sLine != null )
								{
									// Skip all characters until we reach the separator or end-of-line. 
									while ( i < sLine.Length && sLine[i] != chFieldSeparator )
										i++;
								}
								
								string sField = sbField.ToString();
								sField = sField.Replace("\"\"", "\"");
								row[nField-1] = sField;
								nMode = 0;
								i++;
								break;
							}
							default:
								bContinueParsing = false;
								break;
						}
						if ( i >= sLine.Length )
							break;
					}
				}
			}
		}
	}
}
