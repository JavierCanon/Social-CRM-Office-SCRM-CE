using System;
using System.IO;
using System.Data;
using System.Web.UI.WebControls.WebParts;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SplendidCRM
{
	public class SplendidPersonalizationProvider : PersonalizationProvider
	{
		protected string sApplicationName;

		public override string ApplicationName
		{
			get { return ApplicationName; }
			set { sApplicationName = value; }
		}

		/*
		public override void Initialize(string name, NameValueCollection config)
		{
			// Assign the provider a default name if it doesn't have one
			if ( String.IsNullOrEmpty(name) )
				name = "SplendidPersonalizationProvider";

			// Add a default "description" attribute to config if the
			// attribute doesn't exist or is empty
			if ( String.IsNullOrEmpty(config["description"]) )
			{
				config.Remove("description");
				config.Add("description", "Text file personalization provider");
			}

			// Call the base class's Initialize method
			base.Initialize(name, config);
		}
		*/

		public static void USER_PREFERENCES_Write(Guid gID, byte[] blob, IDbTransaction trn)
		{
			using ( MemoryStream stm = new MemoryStream(blob) )
			{
				const int BUFFER_LENGTH = 4*1024;
				byte[] binFILE_POINTER = new byte[16];
				
				SqlProcs.spUSER_PREFERENCES_InitPointer(gID, ref binFILE_POINTER, trn);
				using ( BinaryReader reader = new BinaryReader(stm) )
				{
					int nFILE_OFFSET = 0 ;
					byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					while ( binBYTES.Length > 0 )
					{
						SqlProcs.spUSER_PREFERENCES_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
						nFILE_OFFSET += binBYTES.Length;
						binBYTES = reader.ReadBytes(BUFFER_LENGTH);
					}
				}
			}
		}

		public static byte[] USER_PREFERENCES_Read(Guid gID, IDbConnection con)
		{
			using ( MemoryStream stm = new MemoryStream() )
			{
				using ( BinaryWriter writer = new BinaryWriter(stm) )
				{
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = "spUSER_PREFERENCES_ReadOffset";
						cmd.CommandType = CommandType.StoredProcedure;
						
						const int BUFFER_LENGTH = 4*1024;
						int idx  = 0;
						int size = 0;
						byte[] binData = new byte[BUFFER_LENGTH];
						IDbDataParameter parID          = Sql.AddParameter(cmd, "@ID"         , gID    );
						IDbDataParameter parFILE_OFFSET = Sql.AddParameter(cmd, "@FILE_OFFSET", idx    );
						IDbDataParameter parREAD_SIZE   = Sql.AddParameter(cmd, "@READ_SIZE"  , size   );
						IDbDataParameter parBYTES       = Sql.AddParameter(cmd, "@BYTES"      , binData);
						parBYTES.Direction = ParameterDirection.InputOutput;
						do
						{
							parID         .Value = gID          ;
							parFILE_OFFSET.Value = idx          ;
							parREAD_SIZE  .Value = BUFFER_LENGTH;
							size = 0;
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
				return stm.ToArray();
			}
		}

		protected override void LoadPersonalizationBlobs(WebPartManager webPartManager, string path, string userName, ref byte[] sharedDataBlob, ref byte[] userDataBlob)
		{
			sharedDataBlob = null;
			userDataBlob   = null;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL ;
					sSQL = "select ID                      " + ControlChars.CrLf
					     + "  from vwUSER_PREFERENCES      " + ControlChars.CrLf
					     + " where CATEGORY = @CATEGORY    " + ControlChars.CrLf
					     + "   and ASSIGNED_USER_ID is null" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@CATEGORY", path);
						Guid gID = Sql.ToGuid(cmd.ExecuteScalar());
						if ( !Sql.IsEmptyGuid(gID) )
						{
							sharedDataBlob = USER_PREFERENCES_Read(gID, con);
						}
					}
					// Load private state if userName holds a user name
					if ( !String.IsNullOrEmpty(userName) )
					{
						if ( userName.IndexOf('\\') >= 0 )
							userName = userName.Substring(userName.IndexOf('\\')+1);
						
						sSQL = "select ID                                      " + ControlChars.CrLf
						     + "  from vwUSER_PREFERENCES                      " + ControlChars.CrLf
						     + " where CATEGORY           = @CATEGORY          " + ControlChars.CrLf
						     + "   and ASSIGNED_USER_NAME = @ASSIGNED_USER_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@CATEGORY"          , path.ToLower()    );
							Sql.AddParameter(cmd, "@ASSIGNED_USER_NAME", userName.ToLower());
							Guid gID = Sql.ToGuid(cmd.ExecuteScalar());
							if ( !Sql.IsEmptyGuid(gID) )
							{
								userDataBlob = USER_PREFERENCES_Read(gID, con);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		protected override void ResetPersonalizationBlob(WebPartManager webPartManager, string path, string userName)
		{
			if ( userName != null )
			{
				if ( userName.IndexOf('\\') >= 0 )
					userName = userName.Substring(userName.IndexOf('\\')+1);
			}
			try
			{
				SqlProcs.spUSER_PREFERENCES_DeleteByUser( userName, path);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		protected override void SavePersonalizationBlob(WebPartManager webPartManager, string path, string userName, byte[] dataBlob)
		{
			if ( userName != null )
			{
				if ( userName.IndexOf('\\') >= 0 )
					userName = userName.Substring(userName.IndexOf('\\')+1);
				userName = userName.ToLower();
			}
			path = path.ToLower();
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = con.BeginTransaction() )
					{
						try
						{
							Guid gID = Guid.Empty;
							SqlProcs.spUSER_PREFERENCES_InsertByUser(ref gID, userName, path, trn);
							USER_PREFERENCES_Write(gID, dataBlob, trn);
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception(ex.Message, ex.InnerException));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public override PersonalizationStateInfoCollection FindState(PersonalizationScope scope, PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotSupportedException();
		}

		public override int GetCountOfState(PersonalizationScope scope, PersonalizationStateQuery query)
		{
			throw new NotSupportedException();
		}

		public override int ResetState(PersonalizationScope scope, string[] paths, string[] usernames)
		{
			// 01/25/2007 Paul.  Strip the domain from the user names. 
			for ( int i=0; i < usernames.Length; i++ )
			{
				string sUserName = usernames[i];
				if ( sUserName.IndexOf('\\') >= 0 )
					usernames[i] = sUserName.Substring(sUserName.IndexOf('\\')+1);
				// 01/25/2007 Paul.  Convert to lowercase to support Oracle. 
				usernames[i] = sUserName.ToLower();
			}
			// 01/25/2007 Paul.  Convert to lowercase to support Oracle. 
			for ( int i=0; i < paths.Length; i++ )
			{
				paths[i] = paths[i].ToLower();
			}
			
			int nResetCount = 0;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = con.BeginTransaction() )
					{
						try
						{
							string sSQL ;
							sSQL = "select ID                " + ControlChars.CrLf
							     + "  from vwUSER_PREFERENCES" + ControlChars.CrLf
							     + " where 1 = 1             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, paths, "CATEGORY");
								if ( scope == PersonalizationScope.User )
									Sql.AppendParameter(cmd, usernames, "ASSIGNED_USER_NAME");
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									while ( rdr.Read() )
									{
										Guid gID = Sql.ToGuid(rdr["ID"]);
										SqlProcs.spUSER_PREFERENCES_Delete(gID, trn);
										nResetCount++;
									}
								}
							}
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception(ex.Message, ex.InnerException));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return nResetCount;
		}

		public override int ResetUserState(string path, DateTime userInactiveSinceDate)
		{
			int nResetCount = 0;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = con.BeginTransaction() )
					{
						try
						{
							string sSQL ;
							sSQL = "select ID                            " + ControlChars.CrLf
							     + "  from vwUSER_PREFERENCES            " + ControlChars.CrLf
							     + " where CATEGORY      = @CATEGORY     " + ControlChars.CrLf
							     + "   and DATE_MODIFIED < @DATE_MODIFIED" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.Transaction = trn;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@CATEGORY"     , path.ToLower());
								Sql.AddParameter(cmd, "@DATE_MODIFIED", userInactiveSinceDate);
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									while ( rdr.Read() )
									{
										Guid gID = Sql.ToGuid(rdr["ID"]);
										SqlProcs.spUSER_PREFERENCES_Delete(gID, trn);
										nResetCount++;
									}
								}
							}
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception(ex.Message, ex.InnerException));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return nResetCount;
		}
	}
}

