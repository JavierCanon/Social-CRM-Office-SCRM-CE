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
using System.Reflection;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for DbProviderFactory.
	/// </summary>
	public class DbProviderFactory
	{
		protected string      m_sConnectionString ;
		protected Assembly    m_asmSqlClient      ;
		protected System.Type m_typSqlConnection  ;
		protected System.Type m_typSqlCommand     ;
		protected System.Type m_typSqlDataAdapter ;
		protected System.Type m_typSqlParameter   ;
		protected System.Type m_typSqlBuilder     ;

		public DbProviderFactory(string sConnectionString, string sAssemblyName, string sConnectionName, string sCommandName, string sDataAdapterName, string sParameterName, string sBuilderName)
		{
			m_sConnectionString = sConnectionString;
			#pragma warning disable 618
			m_asmSqlClient      = Assembly.LoadWithPartialName(sAssemblyName);
			#pragma warning restore 618

			// 03/06/2006 Paul.  Provide better error message if assembly cannot be loaded. 
			if ( m_asmSqlClient == null )
				throw(new Exception("Could not load " + sAssemblyName));
			m_typSqlConnection  = m_asmSqlClient.GetType(sConnectionName );
			m_typSqlCommand     = m_asmSqlClient.GetType(sCommandName    );
			// 04/21/2006 Paul.  SQL Anywhere requires a boxed data adapter that inherits DbDataAdapter.
			if ( sDataAdapterName.StartsWith("SplendidCRM.") )
				m_typSqlDataAdapter = Type.GetType(sDataAdapterName);
			else
				m_typSqlDataAdapter = m_asmSqlClient.GetType(sDataAdapterName);
			m_typSqlParameter   = m_asmSqlClient.GetType(sParameterName  );
			// 08/03/2006 Paul.  Mono does not like the CommandBuilder. 
			//m_typSqlBuilder     = m_asmSqlClient.GetType(sBuilderName    );
		}

		public IDbConnection CreateConnection()
		{
			Type[] types = new Type[1];
			types[0] = Type.GetType("System.String");
			ConstructorInfo info = m_typSqlConnection.GetConstructor(types); 
			object[] parameters = new object[1];
			parameters[0] = m_sConnectionString;
			IDbConnection con = info.Invoke(parameters) as IDbConnection; 
			// 04/21/2006 Paul.  Throw exception if NULL.  
			if ( con == null )
				throw(new Exception("Failed to invoke database connection constructor."));
			return con;
			//return new SqlConnection(sConnectionString);
		}

		public IDbCommand CreateCommand()
		{
			ConstructorInfo info = m_typSqlCommand.GetConstructor(new Type[0]); 
			IDbCommand cmd = info.Invoke(null) as IDbCommand; 
			// 04/21/2006 Paul.  Throw exception if NULL.  
			if ( cmd == null )
				throw(new Exception("Failed to invoke database command constructor."));
			return cmd;
			//return new SqlCommand();
		}

		public DbDataAdapter CreateDataAdapter()
		{
			ConstructorInfo info = m_typSqlDataAdapter.GetConstructor(new Type[0]); 
			DbDataAdapter da = info.Invoke(null) as DbDataAdapter; 
			// 04/21/2006 Paul.  Throw exception if NULL.  SQL Anywhere is having a problem. 
			if ( da == null )
				throw(new Exception("Failed to invoke database adapter constructor."));
			return da;
			//return new SqlDataAdapter();
		}

		public IDbDataParameter CreateParameter()
		{
			ConstructorInfo info = m_typSqlParameter.GetConstructor(new Type[0]); 
			IDbDataParameter par = info.Invoke(null) as IDbDataParameter; 
			// 04/21/2006 Paul.  Throw exception if NULL.  
			if ( par == null )
				throw(new Exception("Failed to invoke database parameter constructor."));
			return par;
			//return new SqlParameter();
		}

		public void DeriveParameters(IDbCommand cmd)
		{
			object[] parameters = new object[1];
			parameters[0] = cmd;
			//m_typSqlBuilder.InvokeMember("DeriveParameters", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, parameters);
		}
	}
}
