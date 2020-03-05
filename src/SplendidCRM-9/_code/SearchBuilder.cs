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
using System.Data;
using System.Text;
using System.Collections;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SearchBuilder.
	/// </summary>
	public class SearchBuilder
	{
		private   bool     m_bFeatureOR     = true  ;  //OR keyword
		private   bool     m_bFeatureAND    = true  ;  //AND keyword
		private   bool     m_bFeatureNOT    = true  ;  //NOT keyword
		private   bool     m_bFeatureMIMUS  = true  ;  //- symbol
		private   bool     m_bFeaturePLUS   = true  ;  //+ symbol
		private   bool     m_bFeatureSTAR   = true  ;  //* symbol
		private   bool     m_bFeatureSINGLE = false ;  //Single letter filtering.  01/17/2005 Paul.  Don't do this. 
		private   bool     m_bFeatureLESS   = true  ;  // < or > symbol
		private   bool     m_bFeatureEXCL   = true  ;  //! symbol
		private   bool     m_bFeatureEQUAL  = true  ;  //= sumbol

		protected string   m_sTermOR        = "OR"  ;  //UCase(LmsConfigTermInternal("OR" ))
		protected string   m_sTermAND       = "AND" ;  //UCase(LmsConfigTermInternal("AND"))
		protected string   m_sTermNOT       = "NOT" ;  //UCase(LmsConfigTermInternal("NOT"))

		protected string   m_sInput         = String.Empty;
		protected string[] m_arrTokens      = new String[] {};

		private   IDbCommand m_cmd = null;

		// 07/16/2006 Paul.  We need to know the database platform in order to build the like escape clause properly. 
		public SearchBuilder(string str, IDbCommand cmd)
		{
			m_cmd = cmd;
			ParseInput(str);
		}

		public string[] Tokens
		{
			get
			{
				return m_arrTokens;
			}
		}

		protected bool IsEmptyString(string str)
		{
			if ( str == null || str == String.Empty )
			{
				return true;
			}
			return false;
		}

		protected string EscapeSql(string str)
		{
			return str.Replace("\'", "\'\'");
		}

		protected string EscapeLike(string str)
		{
			string sEscaped = EscapeSql(str);
			// 07/16/2006 Paul.  SQL Server, Oracle and DB2 all support the ESCAPE clause. 
			// MySQL works, but requires ESCAPE '\\'. 
			sEscaped = sEscaped.Replace(@"\", @"\\");
			sEscaped = sEscaped.Replace("%" , @"\%");
			sEscaped = sEscaped.Replace("_" , @"\_");
			return sEscaped;
		}

		protected void ParseInput(string sInput)
		{
			m_sInput  = sInput;
			m_arrTokens = new String[] {};
			if ( !IsEmptyString(sInput) )
			{
				sInput = sInput.ToUpper();
		
				char          chPreviousChar = ' ';
				char[]        arrText        = sInput.ToCharArray();
				bool          bInsideQuotes  = false;
				ArrayList     arrTokens      = new ArrayList();
				StringBuilder sbToken        = new StringBuilder();
				for ( int i = 0 ; i < arrText.Length ; i++ )
				{
					char ch = arrText[i];
					if ( ch == '\"' )
					{
						if ( bInsideQuotes )
						{
							//Add empty quoted strings so that -"" or +"" can be filtered later. 
							//They are filtered later so that the - or + will not be applied to the following token. 
							arrTokens.Add(sbToken.ToString());
							sbToken = new StringBuilder();
						}
						else
						{
							//If starting a quoted token, then add existing token. 
							if ( sbToken.Length > 0 )
								arrTokens.Add(sbToken.ToString());
							sbToken = new StringBuilder();
							sbToken.Append("\"") ; //Place quote as first character as a flag. 
						}
						bInsideQuotes = !bInsideQuotes;
					}
					else if ( bInsideQuotes )
						sbToken.Append(ch);
					//The -/+ should be preceded by a space; so says the Google documentation.
					else if ( m_bFeatureMIMUS && ch == '-' && chPreviousChar == ' ' )
					{
						if ( sbToken.Length > 0 )
							arrTokens.Add(sbToken.ToString());
						sbToken = new StringBuilder();
						arrTokens.Add(ch.ToString());
					}
					else if ( m_bFeaturePLUS && ch == '+' && chPreviousChar == ' ' )
					{
						if ( sbToken.Length > 0 )
							arrTokens.Add(sbToken.ToString());
						sbToken = new StringBuilder();
						arrTokens.Add(ch.ToString());
					}
					else if ( m_bFeatureLESS && ch == '<' )
					{
						if ( sbToken.Length > 0 )
							arrTokens.Add(sbToken.ToString());
						sbToken = new StringBuilder();
						arrTokens.Add(ch.ToString());
					}
					else if ( m_bFeatureLESS && ch == '>' )
					{
						if ( sbToken.Length > 0 )
							arrTokens.Add(sbToken.ToString());
						sbToken = new StringBuilder();
						arrTokens.Add(ch.ToString());
					}
					else if ( m_bFeatureEXCL && ch == '!' )
					{
						if ( sbToken.Length > 0 )
							arrTokens.Add(sbToken.ToString());
						sbToken = new StringBuilder();
						arrTokens.Add(ch.ToString());
					}
					else if ( m_bFeatureEQUAL && ch == '=' )
					{
						if ( sbToken.Length > 0 )
							arrTokens.Add(sbToken.ToString());
						sbToken = new StringBuilder();
						arrTokens.Add(ch.ToString());
					}
					else if ( ch == ControlChars.Cr || ch == ControlChars.Lf || ch == ControlChars.Tab || ch == ' ' || ch == ',' || ch == ';' )
					{
						//CR, LF, TAB, SPACE, COMMA and SEMICOLON can all be used as token separators. 
						if ( sbToken.Length > 0 )
						{
							string strToken = sbToken.ToString();
							arrTokens.Add(strToken);
						}
						sbToken = new StringBuilder();
					}
					else
					{
						sbToken.Append(ch);
					}
					chPreviousChar = ch;
				}
				if ( sbToken.Length > 0 )
					arrTokens.Add(sbToken.ToString());
				m_arrTokens = (string[]) arrTokens.ToArray(Type.GetType("System.String"));
			}
		}

		//Can't do an upper() on NTEXT.  SQL Server complains. 
		public string BuildQuery(string sCondition, string sField, string sInput)
		{
			return BuildQuery(sCondition, sField, sInput, true);
		}

		public string BuildQuery(string sCondition, string sField, string sInput, bool bTextField)
		{
			ParseInput(sInput);
			return BuildQuery(sCondition, sField, bTextField);
		}

		//To determine if the string is valid, build a query and return true if a result is built.
		//The goal is to avoid user queries like "" AND "" where nothing is provided in the quotes. 
		//BuildQuery will properly return nothing, but there are times when we need to know this before we build. 
		public bool IsValidQuery()
		{
			string str = BuildQuery("  ", "xxxx", true);
			return !IsEmptyString(str);
		}

		public string BuildQuery(string sCondition, string sField)
		{
			return BuildQuery(sCondition, sField, true);
		}

		public string BuildQuery(string sCondition, string sField, bool bTextField)
		{
			StringBuilder sbSqlQuery  = new StringBuilder();
			int           nNotFlag    = 0;
			bool          bOrFlag     = false;
			int           nQueryLine  = 0;
			int           nLessFlag   = 0;
			bool          bEqualFlag  = false;
			sbSqlQuery.Append(sCondition);
			sbSqlQuery.Append("(");

			bool bIsOracle = false;
			bool bIsDB2    = false;
			bool bIsMySQL  = false;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					bIsOracle = Sql.IsOracle(cmd);
					bIsDB2    = Sql.IsDB2   (cmd);
					bIsMySQL  = Sql.IsMySQL (cmd);
				}
			}
			// 07/16/2006 Paul.  Now that we know the database platform, only use the upper clause if Oracle or DB2. 
			if ( bIsOracle || bIsDB2 )
			{
				//Can't do an upper() on NTEXT.  SQL Server complains. 
				// 01/16/2008 Paul.  We don't use the bTextField anymore, and I could not find any places where we searched an NTEXT field anyway. 
				//if ( !bTextField )
					sField = "upper(" + sField + ")";
			}

			foreach ( string sThisToken in m_arrTokens )
			{
				if      ( m_bFeatureMIMUS && sThisToken == "-" )
					nNotFlag = 1;
				else if ( m_bFeaturePLUS  && sThisToken == "+" )
					nNotFlag = 2;
				else if ( m_bFeatureOR    && (sThisToken == m_sTermOR  || sThisToken == "OR" ) )
					bOrFlag = true;
				else if ( m_bFeatureAND   && (sThisToken == m_sTermAND || sThisToken == "AND") )
					bOrFlag = false;
				else if ( m_bFeatureNOT   && (sThisToken == m_sTermNOT || sThisToken == "NOT") )
					nNotFlag = 1;
				// 12/17/2007 Paul.  Add support for <, <=, > and >=. 
				else if ( m_bFeatureLESS  && sThisToken == "<" )
					nLessFlag = 1;
				else if ( m_bFeatureLESS  && sThisToken == ">" )
					nLessFlag = -1;
				// 12/17/2007 Paul.  Add support for ! and !=.  They are both the same. 
				else if ( m_bFeatureEXCL  && sThisToken == "!" )
					nNotFlag = 1;
				else if ( m_bFeatureEQUAL && sThisToken == "=" )
					bEqualFlag = true;
				else
				{
					if ( sThisToken.Length > 0 )
					{
						//Google ignores single digit and single letters unless + or - is used. 
						if ( !m_bFeatureSINGLE || nNotFlag > 0 || sThisToken.Length > 1 || !Char.IsLetterOrDigit(sThisToken[0]) )
						{
							//Ignore quoted strings that contain nothing after the quote. 
							if ( sThisToken[0] != '\"' || sThisToken.Length > 1 )
							{
								//Add spaces to the line to align the fields.
								if ( nQueryLine > 0 )
								{
									sbSqlQuery.Append(Strings.Space(sCondition.Length));
									if ( bOrFlag )
										sbSqlQuery.Append("    or ");
									else
										sbSqlQuery.Append("   and ");
								}
								else
									sbSqlQuery.Append("      ");
								if ( nNotFlag == 1 )
									sbSqlQuery.Append("not ");
								else
									sbSqlQuery.Append("    ");
								
								//Remove the double quote flag from a quoted string. 
								string sToken = sThisToken;
								if ( sToken[0] == '\"' )
									sToken = sThisToken.Substring(1);
								
								// 12/17/2007 Paul.  Wildcards will have a higher priority over greater than or less than. 
								if ( m_bFeatureSTAR && sToken.IndexOf('*') >= 0 )
								{
									//Escape to prevent use of % as a wild-card.
									sToken = EscapeLike(sToken);
									// 07/16/2006 Paul.  SQL Server, Oracle and DB2 all support the ESCAPE '\' clause. 
									if ( bIsMySQL)
										sToken = sToken.Replace("\\", "\\\\");
									sToken = sToken.Replace("*", "%");
									sbSqlQuery.Append(sField + " like '" + sToken + "'");
									// 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
									if ( bIsMySQL )
										sbSqlQuery.Append(" escape '\\\\'" + ControlChars.CrLf);
									else
										sbSqlQuery.Append(" escape '\\'" + ControlChars.CrLf);
								}
								else
								{
									if ( nLessFlag == 0 )
									{
										if ( bEqualFlag )
										{
											// 12/17/2007 Paul.  Single quotes need to be manually escaped. 
											sToken = EscapeSql(sToken);
											sbSqlQuery.Append(sField + " = '" + sToken + "'");
										}
										else
										{
											//Escape to prevent use of % as a wild-card.
											sToken = EscapeLike(sToken);
											// 07/16/2006 Paul.  SQL Server, Oracle and DB2 all support the ESCAPE '\' clause. 
											if ( bIsMySQL)
												sToken = sToken.Replace("\\", "\\\\");
											sbSqlQuery.Append(sField + " like '%" + sToken + "%'");
											// 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											if ( bIsMySQL )
												sbSqlQuery.Append(" escape '\\\\'" + ControlChars.CrLf);
											else
												sbSqlQuery.Append(" escape '\\'" + ControlChars.CrLf);
										}
									}
									else
									{
										// 12/17/2007 Paul.  Single quotes need to be manually escaped. 
										sToken = EscapeSql(sToken);
										if ( nLessFlag == 1 )
										{
											if ( bEqualFlag )
												sbSqlQuery.Append(sField + " <= '" + sToken + "'");
											else
												sbSqlQuery.Append(sField + " < '" + sToken + "'");
										}
										else if ( nLessFlag == -1 )
										{
											if ( bEqualFlag )
												sbSqlQuery.Append(sField + " >= '" + sToken + "'");
											else
												sbSqlQuery.Append(sField + " > '" + sToken + "'");
										}
									}
								}
								
								nQueryLine += 1;
							}
						}
					}
					nNotFlag = 0;
					bOrFlag  = false;
					bEqualFlag = false;
					nLessFlag = 0;
				}
			}
			if ( nQueryLine > 0 )
			{
				sbSqlQuery.Append(Strings.Space(sCondition.Length) + ")" + ControlChars.CrLf);
				return sbSqlQuery.ToString();
			}
			return String.Empty;
		}
	}
}

