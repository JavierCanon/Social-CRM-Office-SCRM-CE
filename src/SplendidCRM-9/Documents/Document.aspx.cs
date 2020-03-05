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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Documents
{
	/// <summary>
	/// Summary description for Document.
	/// </summary>
	public class Document : SplendidPage
	{
		public static void WriteStream(Guid gID, IDbConnection con, BinaryWriter writer)
		{
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				cmd.CommandText = "spDOCUMENTS_CONTENT_ReadOffset";
				cmd.CommandType = CommandType.StoredProcedure;
				
				const int BUFFER_LENGTH = 4*1024;
				int idx  = 0;
				int size = 0;
				byte[] binData = new byte[BUFFER_LENGTH];  // 10/20/2005 Paul.  This allocation is only used to set the parameter size. 
				IDbDataParameter parID          = Sql.AddParameter(cmd, "@ID"         , gID    );
				IDbDataParameter parFILE_OFFSET = Sql.AddParameter(cmd, "@FILE_OFFSET", idx    );
				// 01/21/2006 Paul.  Field was renamed to READ_SIZE. 
				IDbDataParameter parREAD_SIZE   = Sql.AddParameter(cmd, "@READ_SIZE"  , size   );
				IDbDataParameter parBYTES       = Sql.AddParameter(cmd, "@BYTES"      , binData);
				parBYTES.Direction = ParameterDirection.InputOutput;
				do
				{
					parID         .Value = gID          ;
					parFILE_OFFSET.Value = idx          ;
					parREAD_SIZE  .Value = BUFFER_LENGTH;
					size = 0;
					// 08/14/2005 Paul.  Oracle returns the bytes in a field.
					// SQL Server can only return the bytes in a resultset. 
					// 10/20/2005 Paul.  MySQL works returning bytes in an output parameter. 
					// 02/05/2006 Paul.  DB2 returns bytse in a field. 
					if ( Sql.IsOracle(cmd) || Sql.IsDB2(cmd) ) // || Sql.IsMySQL(cmd) )
					{
						cmd.ExecuteNonQuery();
						binData = Sql.ToByteArray(parBYTES);
						if ( binData != null )
						{
							size = binData.Length;
							writer.Write(binData);
							idx += size;
						}
					}
					else
					{
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								// 10/20/2005 Paul.  MySQL works returning a record set, but it cannot be cast to a byte array. 
								// binData = (byte[]) rdr[0];
								binData = Sql.ToByteArray((System.Array) rdr[0]);
								if ( binData != null )
								{
									size = binData.Length;
									writer.Write(binData);
									idx += size;
								}
							}
						}
					}
				}
				while ( size == BUFFER_LENGTH );
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Guid gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							Guid gRevisionID = Guid.Empty;
							sSQL = "select *                   " + ControlChars.CrLf
							     + "  from vwDOCUMENT_REVISIONS" + ControlChars.CrLf
							     + " where ID = @ID            " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										Response.ContentType = Sql.ToString(rdr["FILE_MIME_TYPE"]);
										string sFileName = Path.GetFileName(Sql.ToString(rdr["FILENAME"]));
										Response.AddHeader("Content-Disposition", "attachment;filename=" + sFileName);
									}
								}
							}
							using ( BinaryWriter writer = new BinaryWriter(Response.OutputStream) )
							{
								WriteStream(gID, con, writer);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
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
