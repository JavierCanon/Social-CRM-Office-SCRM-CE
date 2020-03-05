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
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Xml;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.SyncSchema
{
	/// <summary>
	/// Summary description for SyncControl.
	/// </summary>
	public class SyncControl : SplendidControl
	{
		public CommandEventHandler Command ;

		protected XmlDocument GetXml()
		{
			XmlDocument xml = new XmlDocument();
			try
			{
				// 08/31/2007 Paul.  Use the parent to find the hidden control. 
				HtmlInputHidden txtXML = Parent.FindControl("txtXML") as HtmlInputHidden;
				if ( txtXML != null )
				{
					xml.LoadXml(Server.HtmlDecode(txtXML.Value));
				}
			}
			catch
			{
			}
			return xml;
		}

		protected void SetXml(XmlDocument xml)
		{
			try
			{
				// 08/31/2007 Paul.  Use the parent to find the hidden control. 
				HtmlInputHidden txtXML = Parent.FindControl("txtXML") as HtmlInputHidden;
				if ( txtXML != null )
				{
					txtXML.Value = Server.HtmlEncode(xml.OuterXml);
				}
			}
			catch
			{
			}
		}

		protected string LoadNames(XmlNode root, string sListName, string sNodeName, string sPROVIDER, string sCONNECTION, string sSQL)
		{
			XmlNode list = root.SelectSingleNode(sListName);
			if ( list == null )
			{
				list = root.OwnerDocument.CreateElement(sListName);
				root.AppendChild(list);
			}
			else
			{
				list.RemoveAll();
			}

			StringBuilder sb = new StringBuilder();
			DbProviderFactory dbf = DbProviderFactories.GetFactory(sPROVIDER, sCONNECTION);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							foreach(DataRow row in dt.Rows)
							{
								string sNAME = Sql.ToString(row["NAME"]);
								sNAME = sNAME.ToUpper();
								sb.Append(sNAME + "<br>" + ControlChars.CrLf);

								XmlElement nodeName = root.OwnerDocument.CreateElement(sNodeName);
								nodeName.InnerText = sNAME;
								list.AppendChild(nodeName);
							}
						}
					}
				}
			}
			SetXml(root.OwnerDocument);
			return sb.ToString();
		}

		protected void CompareNames(XmlNode nodeSource, XmlNode nodeDestination, string sListName, string sNodeName, ref StringBuilder sbSourceUnique, ref StringBuilder sbDestinationUnique)
		{
			XmlNodeList nlSource      = nodeSource     .SelectNodes(sListName + "/" + sNodeName);
			XmlNodeList nlDestination = nodeDestination.SelectNodes(sListName + "/" + sNodeName);
			foreach(XmlElement node in nlSource)
			{
				if ( nodeDestination.SelectSingleNode(sListName + "[" + sNodeName + "=\'" + node.InnerText + "\']") == null )
				{
					sbSourceUnique.Append(node.InnerText);
					sbSourceUnique.Append("<br>" + ControlChars.CrLf);
				}
			}

			foreach(XmlElement node in nlDestination)
			{
				if ( nodeSource.SelectSingleNode(sListName + "[" + sNodeName + "=\'" + node.InnerText + "\']") == null )
				{
					sbDestinationUnique.Append(node.InnerText);
					sbDestinationUnique.Append("<br>" + ControlChars.CrLf);
				}
			}
		}

		protected string GetTablesCommand(string sProvider)
		{
			string sSQL = String.Empty;
			if ( sProvider == "System.Data.SqlClient" )
			{
				sSQL = "select TABLE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.TABLES    " + ControlChars.CrLf
				     + " where TABLE_TYPE = 'BASE TABLE'    " + ControlChars.CrLf
				     + "   and TABLE_NAME not like '%_AUDIT'" + ControlChars.CrLf
				     + "   and TABLE_NAME != 'dtproperties' " + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "MySql.Data" )
			{
				sSQL = "select TABLE_NAME  as NAME                            " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.TABLES                      " + ControlChars.CrLf
				     + " where TABLE_TYPE = 'BASE TABLE'                      " + ControlChars.CrLf
				     + "   and TABLE_NAME not like '%_AUDIT'                  " + ControlChars.CrLf
				     + "   and TABLE_NAME not in ( 'columns_priv'             " + ControlChars.CrLf
				     + "                         , 'db'                       " + ControlChars.CrLf
				     + "                         , 'func'                     " + ControlChars.CrLf
				     + "                         , 'help_category'            " + ControlChars.CrLf
				     + "                         , 'help_keyword'             " + ControlChars.CrLf
				     + "                         , 'help_relation'            " + ControlChars.CrLf
				     + "                         , 'help_topic'               " + ControlChars.CrLf
				     + "                         , 'host'                     " + ControlChars.CrLf
				     + "                         , 'proc'                     " + ControlChars.CrLf
				     + "                         , 'procs_priv'               " + ControlChars.CrLf
				     + "                         , 'tables_priv'              " + ControlChars.CrLf
				     + "                         , 'time_zone'                " + ControlChars.CrLf
				     + "                         , 'time_zone_leap_second'    " + ControlChars.CrLf
				     + "                         , 'time_zone_name'           " + ControlChars.CrLf
				     + "                         , 'time_zone_transition'     " + ControlChars.CrLf
				     + "                         , 'time_zone_transition_type'" + ControlChars.CrLf
				     + "                         , 'user'                     " + ControlChars.CrLf
				     + "                         )                            " + ControlChars.CrLf
				     + " order by NAME                                        " + ControlChars.CrLf;
			}
			else if ( sProvider == "Oracle.DataAccess.Client" )
			{
				// 12/16/2005 Paul.  ALL_TABLES requires owner = USER filter. 
				sSQL = "select TABLE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from USER_TABLES                  " + ControlChars.CrLf
				     + " where TABLE_NAME not like '%_AUDIT'" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "IBM.Data.DB2" )
			{
				sSQL = "select TABLE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from SYSIBM.TABLES                " + ControlChars.CrLf
				     + " where TABLE_SCHEMA = current schema" + ControlChars.CrLf
				     + "   and TABLE_TYPE   = 'BASE TABLE'  " + ControlChars.CrLf
				     + "   and TABLE_NAME not like '%_AUDIT'" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "iAnywhere.Data.AsaClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'U'               " + ControlChars.CrLf
				     + "   and NAME not like '%_AUDIT'  " + ControlChars.CrLf
				     + "   and NAME <> 'sysquerymetrics'" + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			else if ( sProvider == "Sybase.Data.AseClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'U'               " + ControlChars.CrLf
				     + "   and NAME not like '%_AUDIT'  " + ControlChars.CrLf
				     + "   and NAME <> 'sysquerymetrics'" + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			return sSQL;
		}

		protected string GetColumnsCommand(string sProvider)
		{
			string sSQL = String.Empty;
			if ( sProvider == "System.Data.SqlClient" )
			{
				sSQL = "select INFORMATION_SCHEMA.COLUMNS.TABLE_NAME + '.' + INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME as NAME" + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.TABLE_NAME                                               " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME                                              " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.ORDINAL_POSITION                                         " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.IS_NULLABLE                                              " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.DATA_TYPE                                                " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.CHARACTER_MAXIMUM_LENGTH                                 " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.NUMERIC_PRECISION                                        " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.NUMERIC_SCALE                                            " + ControlChars.CrLf
				     + "  from      INFORMATION_SCHEMA.COLUMNS                                                     " + ControlChars.CrLf
				     + " inner join INFORMATION_SCHEMA.TABLES                                                      " + ControlChars.CrLf
				     + "         on INFORMATION_SCHEMA.TABLES.TABLE_NAME = INFORMATION_SCHEMA.COLUMNS.TABLE_NAME   " + ControlChars.CrLf
				     + " where INFORMATION_SCHEMA.TABLES.TABLE_TYPE = 'BASE TABLE'                                 " + ControlChars.CrLf
				     + "   and INFORMATION_SCHEMA.TABLES.TABLE_NAME not like '%_AUDIT'                             " + ControlChars.CrLf
				     + "   and INFORMATION_SCHEMA.TABLES.TABLE_NAME != 'dtproperties'                              " + ControlChars.CrLf
				     + " order by INFORMATION_SCHEMA.TABLES.TABLE_NAME, INFORMATION_SCHEMA.COLUMNS.ORDINAL_POSITION" + ControlChars.CrLf;
			}
			else if ( sProvider == "MySql.Data" )
			{
				sSQL = "select concat_ws(N'.', INFORMATION_SCHEMA.COLUMNS.TABLE_NAME, INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME) as NAME" + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.TABLE_NAME                                               " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME                                              " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.ORDINAL_POSITION                                         " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.IS_NULLABLE                                              " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.DATA_TYPE                                                " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.CHARACTER_MAXIMUM_LENGTH                                 " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.NUMERIC_PRECISION                                        " + ControlChars.CrLf
				     + "     , INFORMATION_SCHEMA.COLUMNS.NUMERIC_SCALE                                            " + ControlChars.CrLf
				     + "  from      INFORMATION_SCHEMA.COLUMNS                                                     " + ControlChars.CrLf
				     + " inner join INFORMATION_SCHEMA.TABLES                                                      " + ControlChars.CrLf
				     + "         on INFORMATION_SCHEMA.TABLES.TABLE_NAME = INFORMATION_SCHEMA.COLUMNS.TABLE_NAME   " + ControlChars.CrLf
				     + " where INFORMATION_SCHEMA.TABLES.TABLE_TYPE = 'BASE TABLE'                                 " + ControlChars.CrLf
				     + "   and INFORMATION_SCHEMA.TABLES.TABLE_NAME not like '%_AUDIT'                             " + ControlChars.CrLf
				     + "   and INFORMATION_SCHEMA.TABLES.TABLE_NAME not in ( 'columns_priv'                        " + ControlChars.CrLf
				     + "                                                   , 'db'                                  " + ControlChars.CrLf
				     + "                                                   , 'func'                                " + ControlChars.CrLf
				     + "                                                   , 'help_category'                       " + ControlChars.CrLf
				     + "                                                   , 'help_keyword'                        " + ControlChars.CrLf
				     + "                                                   , 'help_relation'                       " + ControlChars.CrLf
				     + "                                                   , 'help_topic'                          " + ControlChars.CrLf
				     + "                                                   , 'host'                                " + ControlChars.CrLf
				     + "                                                   , 'proc'                                " + ControlChars.CrLf
				     + "                                                   , 'procs_priv'                          " + ControlChars.CrLf
				     + "                                                   , 'tables_priv'                         " + ControlChars.CrLf
				     + "                                                   , 'time_zone'                           " + ControlChars.CrLf
				     + "                                                   , 'time_zone_leap_second'               " + ControlChars.CrLf
				     + "                                                   , 'time_zone_name'                      " + ControlChars.CrLf
				     + "                                                   , 'time_zone_transition'                " + ControlChars.CrLf
				     + "                                                   , 'time_zone_transition_type'           " + ControlChars.CrLf
				     + "                                                   , 'user'                                " + ControlChars.CrLf
				     + "                                                   )                                       " + ControlChars.CrLf
				     + " order by INFORMATION_SCHEMA.TABLES.TABLE_NAME, INFORMATION_SCHEMA.COLUMNS.ORDINAL_POSITION" + ControlChars.CrLf;
			}
			else if ( sProvider == "Oracle.DataAccess.Client" )
			{
				// 12/16/2005 Paul.  USER_TAB_COLUMNS requires owner = USER filter. 
				sSQL = "select USER_TAB_COLUMNS.TABLE_NAME || '.' || USER_TAB_COLUMNS.COLUMN_NAME as NAME" + ControlChars.CrLf
				     + "     , USER_TAB_COLUMNS.COLUMN_NAME                                              " + ControlChars.CrLf
				     + "     , USER_TAB_COLUMNS.COLUMN_ID                     as ORDINAL_POSITION        " + ControlChars.CrLf
				     + "     , (case USER_TAB_COLUMNS.NULLABLE when 'Y' then 1 else 0 end) as IS_NULLABLE" + ControlChars.CrLf
				     + "     , USER_TAB_COLUMNS.DATA_TYPE                                                " + ControlChars.CrLf
				     + "     , USER_TAB_COLUMNS.CHAR_LENGTH                   as CHARACTER_MAXIMUM_LENGTH" + ControlChars.CrLf
				     + "     , USER_TAB_COLUMNS.DATA_PRECISION                as NUMERIC_PRECISION       " + ControlChars.CrLf
				     + "     , USER_TAB_COLUMNS.DATA_SCALE                    as NUMERIC_SCALE           " + ControlChars.CrLf
				     + "  from      USER_TAB_COLUMNS                                                     " + ControlChars.CrLf
				     + " inner join USER_TABLES                                                          " + ControlChars.CrLf
				     + "         on USER_TABLES.TABLE_NAME = USER_TAB_COLUMNS.TABLE_NAME                 " + ControlChars.CrLf
				     + " where USER_TAB_COLUMNS.OWNER = USER                                             " + ControlChars.CrLf
				     + "   and USER_TABLES.TABLE_NAME not like '%_AUDIT'                                 " + ControlChars.CrLf
				     + " order by USER_TABLES.TABLE_NAME, USER_TAB_COLUMNS.COLUMN_ID                     " + ControlChars.CrLf;
			}
			else if ( sProvider == "IBM.Data.DB2" )
			{
				sSQL = "select SYSIBM.COLUMNS.TABLE_NAME || '.' || SYSIBM.COLUMNS.COLUMN_NAME as NAME" + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.TABLE_NAME                                             " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.COLUMN_NAME                                            " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.ORDINAL_POSITION                                       " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.IS_NULLABLE                                            " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.DATA_TYPE                                              " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.CHARACTER_MAXIMUM_LENGTH                               " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.NUMERIC_PRECISION                                      " + ControlChars.CrLf
				     + "     , SYSIBM.COLUMNS.NUMERIC_SCALE                                          " + ControlChars.CrLf
				     + "  from      SYSIBM.COLUMNS                                                   " + ControlChars.CrLf
				     + " inner join SYSIBM.TABLES                                                    " + ControlChars.CrLf
				     + "         on SYSIBM.TABLES.TABLE_NAME   = SYSIBM.COLUMNS.TABLE_NAME           " + ControlChars.CrLf
				     + "        and SYSIBM.TABLES.TABLE_SCHEMA = SYSIBM.COLUMNS.TABLE_SCHEMA         " + ControlChars.CrLf
				     + " where SYSIBM.COLUMNS.TABLE_SCHEMA = current schema                          " + ControlChars.CrLf
				     + "   and SYSIBM.TABLES.TABLE_TYPE = 'BASE TABLE'                               " + ControlChars.CrLf
				     + "   and SYSIBM.TABLES.TABLE_NAME not like '%_AUDIT'                           " + ControlChars.CrLf
				     + " order by SYSIBM.TABLES.TABLE_NAME, SYSIBM.COLUMNS.ORDINAL_POSITION          " + ControlChars.CrLf;
			}
			else if ( sProvider == "iAnywhere.Data.AsaClient" )
			{
				sSQL = "select sysobjects.name + '.' + syscolumns.name                as NAME                    " + ControlChars.CrLf
				     + "     , sysobjects.name                                        as TABLE_NAME              " + ControlChars.CrLf
				     + "     , syscolumns.name                                        as COLUMN_NAME             " + ControlChars.CrLf
				     + "     , syscolumns.colid                                       as ORDINAL_POSITION        " + ControlChars.CrLf
				     + "     , (case when syscolumns.status & 8 > 0 then 1 else 0 end) as IS_NULLABLE            " + ControlChars.CrLf
				     + "     , syscolumns.type                                        as DATA_TYPE               " + ControlChars.CrLf
				     + "     , syscolumns.length                                      as CHARACTER_MAXIMUM_LENGTH" + ControlChars.CrLf
				     + "     , syscolumns.prec                                        as NUMERIC_PRECISION       " + ControlChars.CrLf
				     + "     , syscolumns.scale                                       as NUMERIC_SCALE           " + ControlChars.CrLf
				     + "  from      sysobjects                                                                   " + ControlChars.CrLf
				     + " inner join syscolumns                                                                   " + ControlChars.CrLf
				     + "         on syscolumns.id = sysobjects.id                                                " + ControlChars.CrLf
				     + " where sysobjects.name <> N'sysquerymetrics'                                             " + ControlChars.CrLf
				     + "   and sysobjects.type = 'U'                                                             " + ControlChars.CrLf
				     + "   and sysobjects.name not like '%_AUDIT'                                                " + ControlChars.CrLf
				     + " order by sysobjects.name, syscolumns.colid                                              " + ControlChars.CrLf;
			}
			else if ( sProvider == "Sybase.Data.AseClient" )
			{
				sSQL = "select sysobjects.name + '.' + syscolumns.name                as NAME                    " + ControlChars.CrLf
				     + "     , sysobjects.name                                        as TABLE_NAME              " + ControlChars.CrLf
				     + "     , syscolumns.name                                        as COLUMN_NAME             " + ControlChars.CrLf
				     + "     , syscolumns.colid                                       as ORDINAL_POSITION        " + ControlChars.CrLf
				     + "     , (case when syscolumns.status & 8 > 0 then 1 else 0 end) as IS_NULLABLE            " + ControlChars.CrLf
				     + "     , syscolumns.type                                        as DATA_TYPE               " + ControlChars.CrLf
				     + "     , syscolumns.length                                      as CHARACTER_MAXIMUM_LENGTH" + ControlChars.CrLf
				     + "     , syscolumns.prec                                        as NUMERIC_PRECISION       " + ControlChars.CrLf
				     + "     , syscolumns.scale                                       as NUMERIC_SCALE           " + ControlChars.CrLf
				     + "  from      sysobjects                                                                   " + ControlChars.CrLf
				     + " inner join syscolumns                                                                   " + ControlChars.CrLf
				     + "         on syscolumns.id = sysobjects.id                                                " + ControlChars.CrLf
				     + " where sysobjects.name <> N'sysquerymetrics'                                             " + ControlChars.CrLf
				     + "   and sysobjects.type = 'U'                                                             " + ControlChars.CrLf
				     + "   and sysobjects.name not like '%_AUDIT'                                                " + ControlChars.CrLf
				     + " order by sysobjects.name, syscolumns.colid                                              " + ControlChars.CrLf;
			}
			return sSQL;
		}

		protected string GetViewsCommand(string sProvider)
		{
			string sSQL = String.Empty;
			if ( sProvider == "System.Data.SqlClient" )
			{
				sSQL = "select TABLE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.VIEWS     " + ControlChars.CrLf
				     + " where TABLE_NAME not in ('sysconstraints', 'syssegments')" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "MySql.Data" )
			{
				/*
				sSQL = "select TABLE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.VIEWS     " + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
				*/
				// 12/13/2005 Paul. MySQL 5.0 seems to have a bug where views are marked as invalid, but they still work. 
				// Use the tables schema to get to the views. 
				sSQL = "select TABLE_NAME  as NAME                     " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.TABLES               " + ControlChars.CrLf
				     + " where (TABLE_TYPE = 'VIEW' or TABLE_TYPE = '')" + ControlChars.CrLf
				     + "   and TABLE_NAME like 'vw%'                   " + ControlChars.CrLf
				     + " order by NAME                                 " + ControlChars.CrLf;
			}
			else if ( sProvider == "Oracle.DataAccess.Client" )
			{
				// 12/16/2005 Paul.  USER_VIEWS requires owner = USER filter. 
				sSQL = "select VIEW_NAME  as NAME           " + ControlChars.CrLf
				     + "  from USER_VIEWS                   " + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "IBM.Data.DB2" )
			{
				sSQL = "select TABLE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from SYSIBM.VIEWS                 " + ControlChars.CrLf
				     + " where TABLE_SCHEMA = current schema" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "iAnywhere.Data.AsaClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'V'               " + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			else if ( sProvider == "Sybase.Data.AseClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'V'               " + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			return sSQL;
		}

		protected string GetProceduresCommand(string sProvider)
		{
			string sSQL = String.Empty;
			if ( sProvider == "System.Data.SqlClient" )
			{
				sSQL = "select ROUTINE_NAME as NAME         " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.ROUTINES  " + ControlChars.CrLf
				     + " where INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE = 'PROCEDURE'  " + ControlChars.CrLf
				     + "   and INFORMATION_SCHEMA.ROUTINES.ROUTINE_NAME not like 'dt_%'" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "MySql.Data" )
			{
				sSQL = "select ROUTINE_NAME as NAME         " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.ROUTINES  " + ControlChars.CrLf
				     + " where INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE = 'PROCEDURE'" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "Oracle.DataAccess.Client" )
			{
				// 12/16/2005 Paul.  USER_PROCEDURES requires owner = USER filter. 
				sSQL = "select OBJECT_NAME as NAME          " + ControlChars.CrLf
				     + "  from USER_PROCEDURES              " + ControlChars.CrLf
				     + " where OBJECT_NAME like 'SP%'       " + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "IBM.Data.DB2" )
			{
				sSQL = "select ROUTINE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from SYSIBM.ROUTINES                " + ControlChars.CrLf
				     + " where ROUTINE_SCHEMA = current schema" + ControlChars.CrLf
				     + "   and ROUTINE_TYPE = 'PROCEDURE'     " + ControlChars.CrLf
				     + "   and ROUTINE_NAME like 'SP%'        " + ControlChars.CrLf
				     + " order by NAME                        " + ControlChars.CrLf;
			}
			else if ( sProvider == "iAnywhere.Data.AsaClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'P'               " + ControlChars.CrLf
				     + "   and NAME <> 'sp_hello'       " + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			else if ( sProvider == "Sybase.Data.AseClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'P'               " + ControlChars.CrLf
				     + "   and NAME <> 'sp_hello'       " + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			return sSQL;
		}

		protected string GetFunctionsCommand(string sProvider)
		{
			string sSQL = String.Empty;
			if ( sProvider == "System.Data.SqlClient" )
			{
				sSQL = "select ROUTINE_NAME as NAME         " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.ROUTINES  " + ControlChars.CrLf
				     + " where INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE = 'FUNCTION'" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "MySql.Data" )
			{
				sSQL = "select ROUTINE_NAME as NAME         " + ControlChars.CrLf
				     + "  from INFORMATION_SCHEMA.ROUTINES  " + ControlChars.CrLf
				     + " where INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE = 'FUNCTION'" + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "Oracle.DataAccess.Client" )
			{
				// 12/16/2005 Paul.  USER_PROCEDURES requires owner = USER filter. 
				sSQL = "select OBJECT_NAME as NAME          " + ControlChars.CrLf
				     + "  from USER_PROCEDURES              " + ControlChars.CrLf
				     + " where OBJECT_NAME not like 'SP%'   " + ControlChars.CrLf
				     + " order by NAME                      " + ControlChars.CrLf;
			}
			else if ( sProvider == "IBM.Data.DB2" )
			{
				sSQL = "select ROUTINE_NAME  as NAME          " + ControlChars.CrLf
				     + "  from SYSIBM.ROUTINES                " + ControlChars.CrLf
				     + " where ROUTINE_SCHEMA = current schema" + ControlChars.CrLf
				     + "   and ROUTINE_TYPE = 'FUNCTION'      " + ControlChars.CrLf
				     + " order by NAME                        " + ControlChars.CrLf;
			}
			else if ( sProvider == "iAnywhere.Data.AsaClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'F'               " + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			else if ( sProvider == "Sybase.Data.AseClient" )
			{
				sSQL = "select NAME                     " + ControlChars.CrLf
				     + "  from sysobjects               " + ControlChars.CrLf
				     + " where TYPE = 'F'               " + ControlChars.CrLf
				     + " order by NAME                  " + ControlChars.CrLf;
			}
			return sSQL;
		}

	}
}
