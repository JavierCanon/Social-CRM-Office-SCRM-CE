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
//using iAnywhere.Data.AsaClient;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SQLAnywhereDataAdapter.
	/// 04/21/2006 Paul.  SQL Anywhere requires a boxed data adapter that inherits DbDataAdapter.
	/// </summary>
	public class SQLAnywhereDataAdapter : DbDataAdapter, IDbDataAdapter
	{
		protected Assembly       m_asmSqlClient      ;
		protected System.Type    m_typSqlDataAdapter ;
		protected IDbDataAdapter m_dbDataAdapter     ;
		private const string m_sAssemblyName    = "iAnywhere.Data.AsaClient";
		private const string m_sDataAdapterName = "iAnywhere.Data.AsaClient.AsaDataAdapter";

		public SQLAnywhereDataAdapter()
		{
			#pragma warning disable 618
			m_asmSqlClient      = Assembly.LoadWithPartialName(m_sAssemblyName);
			#pragma warning restore 618

			if ( m_asmSqlClient == null )
				throw(new Exception("Could not load " + m_sAssemblyName));
			m_typSqlDataAdapter = m_asmSqlClient.GetType(m_sDataAdapterName);
			
			ConstructorInfo info = m_typSqlDataAdapter.GetConstructor(new Type[0]); 
			m_dbDataAdapter = info.Invoke(null) as IDbDataAdapter; 
			if ( m_dbDataAdapter == null )
				throw(new Exception("Failed to invoke database adapter constructor."));
		}

		/*
		// 04/21/2006 Paul.  There does not need to be a dispose 
		// as neither m_asmSqlClient, nor m_typSqlDataAdapter have a dispose method. 
		private bool disposed = false;

		~SQLAnywhereDataAdapter()
		{
			Dispose(false);
		}
		
		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			if ( !disposed )
			{
				if ( disposing )
				{
					m_asmSqlClient      = null;
					m_typSqlDataAdapter = null;
				}
			}
			base.Dispose(disposing);
			disposed = true;
		}
		*/


		#region DbDataAdapter Abstract Members
		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow , IDbCommand command , StatementType statementType , DataTableMapping tableMapping)
		{
			// 04/24/2006 Paul.  I don't like seeing the unreachable code warning. 
			//throw new NotImplementedException();
			return null;
		}
		
		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow , IDbCommand command , StatementType statementType , DataTableMapping tableMapping)
		{
			// 04/24/2006 Paul.  I don't like seeing the unreachable code warning. 
			//throw new NotImplementedException();
			return null;
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			throw new NotImplementedException();
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IDbDataAdapter Members
		// 05/18/2006 Paul.  .NET 2.0 defines IDbDataAdapter overrides differently. 
		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get { return m_dbDataAdapter.UpdateCommand; }
			set { m_dbDataAdapter.UpdateCommand = value; }
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get { return m_dbDataAdapter.SelectCommand; }
			set { m_dbDataAdapter.SelectCommand = value; }
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get { return m_dbDataAdapter.DeleteCommand; }
			set { m_dbDataAdapter.DeleteCommand = value; }
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get { return m_dbDataAdapter.InsertCommand; }
			set { m_dbDataAdapter.InsertCommand = value; }
		}
		#endregion
	}
}
