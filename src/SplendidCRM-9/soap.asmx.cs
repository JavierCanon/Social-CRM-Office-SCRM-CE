using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for sugarsoap.
	/// 02/17/2006 Paul.  Change class name to sugarsoap to match the namespace used by SugarCRM. 
	/// 02/18/2006 Paul.  Must use the same SugarCRM namespace in order for SugarMail to consume our services.
	/// 02/18/2006 Paul.  The correct way to change the name is to use the Name property of WebService.
	/// 02/18/2006 Paul.  Must specify [SoapRpcService] in order to be compatible with SugarCRM. 
	/// 02/18/2006 Paul.  Methods must be marked with [SoapRpcMethod] in order to be compatible with SugarCRM. 
	/// </summary>
	[SoapRpcService]
	[WebService(Namespace="http://www.sugarcrm.com/sugarcrm", Name="sugarsoap", Description="SugarCRM web services implemented in C#")]
	public class soap : System.Web.Services.WebService
	{
		public soap()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}


		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		
		#endregion

		#region Data Structures
		[Serializable]
		public class contact_detail
		{
			public string        email_address;
			public string        name1        ;
			public string        name2        ;
			public string        association  ;
			public string        id           ;
			public string        msi_id       ;
			public string        type         ;

			public contact_detail()
			{
				email_address = String.Empty;
				name1         = String.Empty;
				name2         = String.Empty;
				association   = String.Empty;
				id            = String.Empty;
				msi_id        = String.Empty;
				type          = String.Empty;
			}
		}

		[Serializable]
		public class document_revision
		{
			public string        id           ;
			public string        document_name;
			public string        revision     ;
			public string        filename     ;
			public string        file         ;

			public document_revision()
			{
				id            = String.Empty;
				document_name = String.Empty;
				revision      = String.Empty;
				filename      = String.Empty;
				file          = String.Empty;
			}
		}

		[Serializable]
		public class error_value
		{
			public string        number       ;
			public string        name         ;
			public string        description  ;

			public error_value()
			{
				number      = "0";
				name        = "No Error";
				description = "No Error";
			}

			public error_value(string number, string name, string description)
			{
				this.number       = number      ;
				this.name         = name        ;
				this.description  = description ;
			}
		}

		[Serializable]
		public class set_relationship_list_result
		{
			public int           created      ;
			public int           failed       ;
			public error_value   error        ;

			public set_relationship_list_result()
			{
				created = 0;
				failed  = 0;
				error   = new error_value();
			}
		}

		[Serializable]
		public class set_relationship_value
		{
			public string        module1      ;
			public string        module1_id   ;
			public string        module2      ;
			public string        module2_id   ;

			public set_relationship_value()
			{
				module1    = String.Empty;
				module1_id = String.Empty;
				module2    = String.Empty;
				module2_id = String.Empty;
			}
		}

		[Serializable]
		public class id_mod
		{
			public string        id           ;
			public string        date_modified;
			public int           deleted      ;

			public id_mod()
			{
				id            = String.Empty;
				date_modified = String.Empty;
				deleted       = 0;
			}
			public id_mod(string id, string date_modified, int deleted)
			{
				this.id            = id           ;
				this.date_modified = date_modified;
				this.deleted       = deleted      ;
			}
		}

		[Serializable]
		public class get_relationships_result
		{
			public id_mod[]      ids          ;
			public error_value   error        ;

			public get_relationships_result()
			{
				ids   = new id_mod[0];
				error = new error_value();
			}
		}

		/*
		<return xsi:type="tns:get_entry_list_result_encoded">
			<result_count xsi:type="xsd:int">0</result_count>
			<next_offset xsi:type="xsd:int">0</next_offset>
			<total_count xsi:type="xsd:int">0</total_count>
			<field_list xsi:type="SOAP-ENC:Array" SOAP-ENC:arrayType="xsd:string[0]"></field_list>
			<entry_list xsi:type="xsd:string">PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48aXRlbXM+PC9pdGVtcz4=</entry_list>
			<error xsi:type="tns:error_value">
				<number xsi:type="xsd:string">0</number>
				<name xsi:type="xsd:string">No Error</name>
				<description xsi:type="xsd:string">No Error</description>
			</error>
		</return>
		*/
		// 06/19/2007 Paul.  Starting with version 4.2, SugarCRM uses a function that optimizes syncing. 
		[Serializable]
		public class get_entry_list_result_encoded
		{
			public int           result_count ;
			public int           next_offset  ;
			public int           total_count  ;
			public string[]      field_list   ;
			public string        entry_list   ;  // Defaults to base64 encoded XML, but can also be PHP encoded. 
			public error_value   error        ;

			public get_entry_list_result_encoded()
			{
				result_count = 0;
				next_offset  = 0;
				total_count  = 0;
				field_list   = new string[0];
				entry_list   = String.Empty;
				error        = new error_value();
			}
		}

		[Serializable]
		public class module_list
		{
			public string[]      modules      ;
			public error_value   error        ;

			public module_list()
			{
				modules = new string[0];
				error   = new error_value();
			}
		}

		[Serializable]
		public class name_value
		{
			public string        name         ;
			public string        value        ;

			public name_value()
			{
				name  = String.Empty;
				value = String.Empty;
			}

			public name_value(string name, string value)
			{
				this.name  = name;
				this.value = value;
			}
		}

		[Serializable]
		public class field
		{
			public string        name         ;
			public string        type         ;
			public string        label        ;
			public int           required     ;
			public name_value[]  options      ;

			public field()
			{
				name     = String.Empty;
				type     = String.Empty;
				label    = String.Empty;
				required = 0;
				options  = new name_value[0];
			}

			public field(string name, string type, string label, int required)
			{
				this.name     = name    ;
				this.type     = type    ;
				this.label    = label   ;
				this.required = required;
				options       = new name_value[0];
			}
		}

		[Serializable]
		public class module_fields
		{
			public string        module_name  ;
			public field[]       module_fields1;
			public error_value   error        ;

			public module_fields()
			{
				module_name    = String.Empty;
				module_fields1 = new field[0];
				error          = new error_value();
			}
		}

		[Serializable]
		public class note_attachment
		{
			public string        id           ;
			public string        filename     ;
			public string        file         ;

			public note_attachment()
			{
				id       = String.Empty;
				filename = String.Empty;
				file     = String.Empty;
			}
		}

		[Serializable]
		public class return_note_attachment
		{
			public note_attachment note_attachment;
			public error_value     error          ;

			public return_note_attachment()
			{
				note_attachment = new note_attachment();
				error           = new error_value();
			}
		}

		[Serializable]
		public class set_entries_result
		{
			public string[]      ids          ;
			public error_value   error        ;

			public set_entries_result()
			{
				ids   = new string[0];
				error = new error_value();
			}
		}

		[Serializable]
		public class entry_value
		{
			public string        id           ;
			public string        module_name  ;
			public name_value[]  name_value_list;

			public entry_value()
			{
				id              = String.Empty;
				module_name     = String.Empty;
				name_value_list = new name_value[0];
			}
			public entry_value(string id, string module_name, string name, string value)
			{
				this.id                 = id;
				this.module_name        = module_name ;
				this.name_value_list    = new name_value[1];
				this.name_value_list[0] = new name_value(name, value);
			}
		}

		[Serializable]
		public class get_entry_result
		{
			public field[]       field_list   ;
			public entry_value[] entry_list   ;
			public error_value   error        ;

			public get_entry_result()
			{
				field_list = new field      [0];
				entry_list = new entry_value[0];
				error      = new error_value();
			}
		}

		[Serializable]
		public class get_entry_list_result
		{
			public int           result_count ;
			public int           next_offset  ;
			public field[]       field_list   ;
			public entry_value[] entry_list   ;
			public error_value   error        ;

			public get_entry_list_result()
			{
				result_count = 0;
				next_offset  = 0;
				field_list   = new field      [0];
				entry_list   = new entry_value[0];
				error        = new error_value();
			}
		}

		[Serializable]
		public class set_entry_result
		{
			public string        id           ;
			public error_value   error        ;

			public set_entry_result()
			{
				id    = String.Empty;
				error = new error_value();
			}
		}

		[Serializable]
		public class user_auth
		{
			public string        user_name    ;
			public string        password     ;
			public string        version      ;

			public user_auth()
			{
				user_name     = String.Empty;
				password      = String.Empty;
				version       = String.Empty;
			}
		}

		[Serializable]
		public class user_detail
		{
			public string        email_address;
			public string        user_name    ;
			public string        first_name   ;
			public string        last_name    ;
			public string        department   ;
			public string        id           ;
			public string        title        ;

			public user_detail()
			{
				email_address = String.Empty;
				user_name     = String.Empty;
				first_name    = String.Empty;
				last_name     = String.Empty;
				department    = String.Empty;
				id            = String.Empty;
				title         = String.Empty;
			}
		}
		#endregion

		// 12/29/2005 Paul.  Application will be started on first service call. 
		// 02/18/2006 Paul.  Methods must be marked with [SoapRpcMethod] in order to be compatible with SugarCRM. 
		#region System Information
		[WebMethod]
		[SoapRpcMethod]
		public string get_server_version()
		{
			return Sql.ToString(HttpContext.Current.Application["CONFIG.sugar_version"]);
		}

		[WebMethod]
		[SoapRpcMethod]
		public int is_loopback()
		{
			if ( HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] == HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"] )
				return 1;
			return 0;
		}

		[WebMethod]
		[SoapRpcMethod]
		public string test(string s)
		{
			return s;
		}

		[WebMethod]
		[SoapRpcMethod]
		public string get_server_time()
		{
			DateTime dtNow = DateTime.Now;
			return dtNow.ToString("G");
		}

		[WebMethod]
		[SoapRpcMethod]
		public string get_gmt_time()
		{
			DateTime dtNow = DateTime.Now;
			return dtNow.ToUniversalTime().ToString("u");
		}
		#endregion

		/*
		'no_error'               =>array('number'=>0 , 'name'=>'No Error', 'description'=>'No Error'),
		'invalid_login'          =>array('number'=>10 , 'name'=>'Invalid Login', 'description'=>'Login attempt failed please check the username and password'),
		'invalid_session'        =>array('number'=>11 , 'name'=>'Invalid Session ID', 'description'=>'The session ID is invalid'),
		'no_portal'              =>array('number'=>12 , 'name'=>'Invalid Portal Client', 'description'=>'Portal Client does not have authorized access'),
		'no_module'              =>array('number'=>20 , 'name'=>'Module Does Not Exist', 'description'=>'This module is not available on this server'),
		'no_file'                =>array('number'=>21 , 'name'=>'File Does Not Exist', 'description'=>'The desired file does not exist on the server'),
		'no_module_support'      =>array('number'=>30 , 'name'=>'Module Not Supported', 'description'=>'This module does not support this feature'),
		'no_relationship_support'=>array('number'=>31 , 'name'=>'Relationship Not Supported', 'description'=>'This module does not support this relationship'),
		'no_access'              =>array('number'=>40 , 'name'=>'Access Denied', 'description'=>'You do not have access'),
		'duplicates'             =>array('number'=>50 , 'name'=>'Duplicate Records', 'description'=>'Duplicate records have been found. Please be more specific.'),
		'no_records'             =>array('number'=>51 , 'name'=>'No Records', 'description'=>'No records were found.'),
		*/

		#region Session
		public static DateTime DefaultCacheExpiration()
		{
			return DateTime.Now.AddDays(1);
		}

		private Guid GetSessionUserID(string session)
		{
			System.Web.Caching.Cache Cache = HttpContext.Current.Cache;

			Guid gUSER_ID = Sql.ToGuid(Cache.Get("soap.session.user." + session));
			if ( gUSER_ID == Guid.Empty )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SOAP User login.");
				throw(new Exception("The session ID is invalid.  " + session));
			}
			// 02/17/2006 Paul.  We do need to set the USER_ID in the session as the stored procedures use the session variable. 
			if ( Session == null )
				throw(new Exception("HttpContext.Current.Session is null"));
			HttpContext.Current.Session["USER_ID"] = gUSER_ID;

			// 09/01/2006 Paul.  On every SOAP request, we need to update the cache expiration.
			// This should only be a minor impact on performance, but it will allow the user to stay connected indefinitely
			// when the Outlook Plug-in is set to auto-sync. 
			Guid gSessionID = Sql.ToGuid(session);
			string user_name   = Sql.ToString(Cache.Get("soap.user.username." + gUSER_ID.ToString()));
			string sCurrencyID = Sql.ToString(Cache.Get("soap.user.currency." + gUSER_ID.ToString()));
			string sTimeZone   = Sql.ToString(Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			Cache.Remove("soap.session.user."     + gSessionID.ToString());
			//Cache.Remove("soap.username.session." + user_name.ToLower()  );
			Cache.Remove("soap.user.username."    + gUSER_ID.ToString()  );
			Cache.Remove("soap.user.currency."    + gUSER_ID.ToString()  );
			Cache.Remove("soap.user.timezone."    + gUSER_ID.ToString()  );

			// 03/07/2007 Paul.  Use a single expiration value. 
			DateTime dtExpiration = DefaultCacheExpiration();
			Cache.Insert("soap.session.user."     + gSessionID.ToString(), gUSER_ID           , null, dtExpiration, Cache.NoSlidingExpiration);
			//Cache.Insert("soap.username.session." + user_name.ToLower()  , gSessionID         , null, dtExpiration, Cache.NoSlidingExpiration);
			Cache.Insert("soap.user.username."    + gUSER_ID.ToString()  , user_name.ToLower(), null, dtExpiration, Cache.NoSlidingExpiration);
			Cache.Insert("soap.user.currency."    + gUSER_ID.ToString()  , sCurrencyID        , null, dtExpiration, Cache.NoSlidingExpiration);
			Cache.Insert("soap.user.timezone."    + gUSER_ID.ToString()  , sTimeZone          , null, dtExpiration, Cache.NoSlidingExpiration);
			Cache.Insert("soap.user.expiration."  + gUSER_ID.ToString()  , dtExpiration       , null, dtExpiration, Cache.NoSlidingExpiration);
			return gUSER_ID;
		}

		private bool IsAdmin(Guid gUSER_ID)
		{
			bool bIS_ADMIN = false;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select IS_ADMIN" + ControlChars.CrLf
				     + "  from vwUSERS " + ControlChars.CrLf
				     + " where ID = @ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gUSER_ID);
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							bIS_ADMIN = Sql.ToBoolean(rdr["IS_ADMIN"]);
						}
					}
				}
			}
			return bIS_ADMIN;
		}

		// 03/12/2007 Paul.  A number of customers are having a problem with the plug-in timing out. 
		// The problem could be that the username is blank for windows authentication. 
		public Guid LoginUser(ref string sUSER_NAME, string sPASSWORD)
		{
			Guid gUSER_ID = Guid.Empty;
			string sNTLM = String.Empty;
			if ( Security.IsWindowsAuthentication() )
			{
				string[] arrUserName = HttpContext.Current.User.Identity.Name.Split('\\');
				string sUSER_DOMAIN = arrUserName[0];
				sUSER_NAME = arrUserName[1];
				// 09/07/2006 Paul.  Provide an indication that we are using NTLM. 
				sNTLM = " (NTLM " + sUSER_DOMAIN + ")";
			}
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				// 05/23/2006 Paul.  Use vwUSERS_Login so that USER_HASH can be removed from vwUSERS to prevent its use in reports. 
				sSQL = "select ID                    " + ControlChars.CrLf
				     + "     , USER_NAME             " + ControlChars.CrLf
				     + "     , FULL_NAME             " + ControlChars.CrLf
				     + "     , IS_ADMIN              " + ControlChars.CrLf
				     + "     , STATUS                " + ControlChars.CrLf
				     + "     , PORTAL_ONLY           " + ControlChars.CrLf
				     + "  from vwUSERS_Login         " + ControlChars.CrLf
				     + " where lower(USER_NAME) = @USER_NAME" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME);
					if ( !Security.IsWindowsAuthentication() )
					{
						if ( !Sql.IsEmptyString(sPASSWORD) )
						{
							cmd.CommandText += "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@USER_HASH", sPASSWORD.ToLower());
						}
						else
						{
							// 11/19/2005 Paul.  Handle the special case of the password stored as NULL or empty string. 
							cmd.CommandText += "   and (USER_HASH = '' or USER_HASH is null)" + ControlChars.CrLf;
						}
					}
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							gUSER_ID = Sql.ToGuid(rdr["ID"]);
							// 09/07/2006 Paul.  Include the user name in the message. 
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SOAP User login for " + sUSER_NAME + sNTLM);
						}
						else
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SECURITY: failed attempted login for " + sUSER_NAME + sNTLM + " using SOAP api");
						}
					}
				}
			}
			if ( gUSER_ID == Guid.Empty )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Invalid username and/or password for " + sUSER_NAME + sNTLM);
				throw(new Exception("Invalid username and/or password for " + sUSER_NAME + sNTLM));
			}
			// 02/16/2006 Paul.  We do need to set the USER_ID in the session as the stored procedures use the session variable. 
			if ( HttpContext.Current.Session == null )
				throw(new Exception("HttpContext.Current.Session is null"));
			HttpContext.Current.Session["USER_ID"] = gUSER_ID;
			return gUSER_ID;
		}
		
		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		// 02/18/2006 Paul.  The return attribute does not tag the output with [return: System.Xml.Serialization.SoapElementAttribute("return")]. 
		// [return: XmlElement("return")]
		public string create_session(string user_name, string password)
		{
			// 06/04/2007 Paul.  Move logic to a new function that can return the Session ID.  It will be needed in other functions. 
			// 06/05/2007 Paul.  The SplendidCRM create_session function just validates the users, but does not create a session. 
			// This is because it does is no good to create a session if we are unable to access the SessionID.
			Guid gUSER_ID = LoginUser(ref user_name, password);
			// LoginUser will throw an exception if the user is invalid.  Let this exception bubble-up the chain and return to the calling service. 
			// 12/29/2005 Paul. SugarCRM returns Success instead of the SessionID.  The login function will return the Session ID. Very strange.
			return "Success";
		}

		private Guid CreateSession(string user_name, string password)
		{
			System.Web.Caching.Cache Cache = HttpContext.Current.Cache;
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);
			// 12/29/2005 Paul.  If the user is valid, then try and locate an existing session. 
			
			// 06/04/2007 Paul.  Always create a new session. This is to allow multiple active sessions. 
			Guid gSessionID = Guid.Empty; // Sql.ToGuid(Cache.Get("soap.username.session." + user_name.ToLower()));
			//if ( gSessionID == Guid.Empty )
			{
				gSessionID = Guid.NewGuid();
				// 03/07/2007 Paul.  Use a single expiration value. 
				DateTime dtExpiration = DefaultCacheExpiration();
				Cache.Insert("soap.session.user."      + gSessionID.ToString(), gUSER_ID           , null, dtExpiration, Cache.NoSlidingExpiration);
				//Cache.Insert("soap.username.session."  + user_name.ToLower()  , gSessionID         , null, dtExpiration, Cache.NoSlidingExpiration);
				Cache.Insert("soap.user.username."     + gUSER_ID.ToString()  , user_name.ToLower(), null, dtExpiration, Cache.NoSlidingExpiration);
				Cache.Insert("soap.user.expiration."   + gUSER_ID.ToString()  , dtExpiration       , null, dtExpiration, Cache.NoSlidingExpiration);
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL ;
					sSQL = "select *           " + ControlChars.CrLf
					     + "  from vwUSERS_Edit" + ControlChars.CrLf
					     + " where ID = @ID    " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gUSER_ID);
						con.Open();
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								string sUSER_PREFERENCES = Sql.ToString(rdr["USER_PREFERENCES"]);
								if ( !Sql.IsEmptyString(sUSER_PREFERENCES) )
								{
									XmlDocument xml = SplendidInit.InitUserPreferences(sUSER_PREFERENCES);
									try
									{
										Cache.Insert("soap.user.timezone." + gUSER_ID.ToString(), XmlUtil.SelectSingleNode(xml, "timezone"), null, dtExpiration, Cache.NoSlidingExpiration);
									}
									catch
									{
										SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Invalid USER_SETTINGS/TIMEZONE: " + XmlUtil.SelectSingleNode(xml, "timezone"));
									}
									try
									{
										Cache.Insert("soap.user.currency." + gUSER_ID.ToString(), XmlUtil.SelectSingleNode(xml, "currency_id")  , null, dtExpiration, Cache.NoSlidingExpiration);
									}
									catch
									{
										SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Invalid USER_SETTINGS/CURRENCY: " + XmlUtil.SelectSingleNode(xml, "currency_id"));
									}
								}
							}
						}
					}
					string sTimeZone   = Sql.ToString(Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
					string sCurrencyID = Sql.ToString(Cache.Get("soap.user.currency." + gUSER_ID.ToString()));
					if ( Sql.IsEmptyString(sCurrencyID) )
					{
						// 09/01/2006 Paul.  Use system default currency if no user value is provided. 
						sCurrencyID = SplendidDefaults.CurrencyID();
						Cache.Insert("soap.user.currency." + gUSER_ID.ToString(), sCurrencyID, null, dtExpiration, Cache.NoSlidingExpiration);
					}
					if ( Sql.IsEmptyString(sTimeZone) )
					{
						// 09/01/2006 Paul.  Use system default timezone if no user value is provided. 
						sTimeZone = SplendidDefaults.TimeZone();
						Cache.Insert("soap.user.timezone." + gUSER_ID.ToString(), sTimeZone  , null, dtExpiration, Cache.NoSlidingExpiration);
					}
				}
			}
			return gSessionID;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public set_entry_result login(user_auth user_auth, string application_name)
		{
			// 03/12/2007 Paul.  If we are using NTLM, then the user_name will be blank. 
			// This could be one of the reasons why some sessions were dying.  
			if ( Security.IsWindowsAuthentication() )
			{
				string[] arrUserName = HttpContext.Current.User.Identity.Name.Split('\\');
				string sUSER_DOMAIN = arrUserName[0];
				user_auth.user_name = arrUserName[1];
			}
			// 12/29/2005 Paul.  create_session returns "Suceess".  We need a separate operation to get the SessionID.
			set_entry_result result = new set_entry_result();
			// 06/04/2007 Paul.  Use new function that returns the Session ID. 
			result.id = CreateSession(user_auth.user_name, user_auth.password).ToString();
			//result.id = Sql.ToString(HttpContext.Current.Cache.Get("soap.username.session." + user_auth.user_name.ToLower()));
			return result;
		}

		[WebMethod]
		[SoapRpcMethod]
		public string end_session(string user_name)
		{
			// 06/04/2007 Paul.  end_session does nothing.  The cached session will eventually expire. 
			// This was a poor design by SugarCRM. 
			/*
			System.Web.Caching.Cache Cache = HttpContext.Current.Cache;
			Guid gSessionID = Sql.ToGuid(Cache.Get("soap.username.session." + user_name.ToLower()));
			if ( gSessionID != Guid.Empty )
			{
				Guid gUSER_ID = Sql.ToGuid(Cache.Get("soap.session.user." + gSessionID.ToString()));
				Cache.Remove("soap.session.user."     + gSessionID.ToString());
				Cache.Remove("soap.username.session." + user_name.ToLower()  );
				// 09/01/2006 Paul.  Remove all cached entries for this user. 
				Cache.Remove("soap.user.username."    + gUSER_ID.ToString()  );
				Cache.Remove("soap.user.currency."    + gUSER_ID.ToString()  );
				Cache.Remove("soap.user.timezone."    + gUSER_ID.ToString()  );
			}
			*/
			return "Success";
		}

		[WebMethod]
		[SoapRpcMethod]
		public int seamless_login(string session)
		{
			Guid gUSER_ID = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.session.user." + session));
			if ( gUSER_ID == Guid.Empty )
			{
				return 0;
			}
			return 1;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public error_value logout(string session)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			error_value results = new error_value();
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string get_user_id(string session)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			return gUSER_ID.ToString();
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string get_user_team_id(string session)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			// 12/29/2005 Paul.  Lets not throw an exception so that the Outlook plugin will not fail if it calls this function. 
			/*
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			*/
			return String.Empty;
		}
		#endregion

		#region UserName/Password-required functions

		#region Creation methods
		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string create_contact(string user_name, string password, string first_name, string last_name, string email_address)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			int nACLACCESS = Security.GetUserAccess("Contacts", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			Guid gID = Guid.Empty;
			SqlProcs.spCONTACTS_New(ref gID, first_name, last_name, String.Empty, email_address);
			return "1";
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string create_lead(string user_name, string password, string first_name, string last_name, string email_address)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			int nACLACCESS = Security.GetUserAccess("Leads", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			Guid gID = Guid.Empty;
			SqlProcs.spLEADS_New(ref gID, first_name, last_name, String.Empty, email_address);
			return "1";
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string create_account(string user_name, string password, string name, string phone, string website)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			int nACLACCESS = Security.GetUserAccess("Accounts", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			Guid gID = Guid.Empty;
			SqlProcs.spACCOUNTS_New(ref gID, name, phone, website);
			return "1";
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string create_opportunity(string user_name, string password, string name, string amount)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			int nACLACCESS = Security.GetUserAccess("Opportunities", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			Guid gID = Guid.Empty;
			SqlProcs.spOPPORTUNITIES_New(ref gID, Guid.Empty, name, Sql.ToDecimal(amount), Guid.Empty, DateTime.MinValue, String.Empty);
			return "1";
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string create_case(string user_name, string password, string name)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			int nACLACCESS = Security.GetUserAccess("Cases", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			Guid gID = Guid.Empty;
			SqlProcs.spCASES_New(ref gID, name, String.Empty, Guid.Empty);
			return "1";
		}
		#endregion

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public contact_detail[] contact_by_email(string user_name, string password, string email_address)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			int nACLACCESS = Security.GetUserAccess("Contacts", "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			contact_detail[] results = new contact_detail[0];
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select *                      " + ControlChars.CrLf
				     + "  from vwSOAP_Contact_By_Email" + ControlChars.CrLf
				     + " where 1 = 0                  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 12/29/2005 Paul.  Allow multiple email addresses, separated by a semicolon. 
					email_address = email_address.Replace(" ", "");
					string[] aAddresses = email_address.Split(';');
					// 02/20/2006 Paul.  Need to use the IN clause. 
					Sql.AppendParameter(cmd, aAddresses, "EMAIL1", true);
					Sql.AppendParameter(cmd, aAddresses, "EMAIL2", true);
					if ( nACLACCESS == ACL_ACCESS.OWNER )
					{
						Sql.AppendParameter(cmd, gUSER_ID, "ASSIGNED_USER_ID");
					}
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 02/20/2006 Paul.  First initialize the array. 
									results = new contact_detail[dt.Rows.Count];
									for ( int i=0; i < dt.Rows.Count ; i++ )
									{
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										results[i] = new contact_detail();
										results[i].email_address = Sql.ToString(dt.Rows[i]["EMAIL_ADDRESS"]);
										results[i].name1         = Sql.ToString(dt.Rows[i]["NAME1"        ]);
										results[i].name2         = Sql.ToString(dt.Rows[i]["NAME2"        ]);
										results[i].association   = Sql.ToString(dt.Rows[i]["ASSOCIATION"  ]);
										results[i].id            = Sql.ToString(dt.Rows[i]["ID"           ]);
										results[i].type          = Sql.ToString(dt.Rows[i]["TYPE"         ]);
										results[i].msi_id        = (i+1).ToString();
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed contact_by_email", ex));
					}
				}
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public user_detail[] user_list(string user_name, string password)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			if ( !IsAdmin(gUSER_ID) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			user_detail[] results = new user_detail[0];
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select *               " + ControlChars.CrLf
				     + "  from vwSOAP_User_List" + ControlChars.CrLf
				     + " where 1 = 1           " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 02/20/2006 Paul.  First initialize the array. 
									results = new user_detail[dt.Rows.Count];
									for ( int i=0; i < dt.Rows.Count ; i++ )
									{
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										results[i] = new user_detail();
										results[i].email_address = Sql.ToString(dt.Rows[i]["EMAIL_ADDRESS"]);
										results[i].user_name     = Sql.ToString(dt.Rows[i]["USER_NAME"    ]);
										results[i].first_name    = Sql.ToString(dt.Rows[i]["FIRST_NAME"   ]);
										results[i].last_name     = Sql.ToString(dt.Rows[i]["LAST_NAME"    ]);
										results[i].department    = Sql.ToString(dt.Rows[i]["DEPARTMENT"   ]);
										results[i].id            = Sql.ToString(dt.Rows[i]["ID"           ]);
										results[i].title         = Sql.ToString(dt.Rows[i]["TITLE"        ]);
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed user_list", ex));
					}
				}
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public contact_detail[] search(string user_name, string password, string name)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);

			contact_detail[] results = new contact_detail[0];
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				int nACLACCESS = 0;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					StringBuilder sb = new StringBuilder();
					// 12/29/2005 Paul.  Names are normally separated by a semicolon.
					// Since we are using our StringBuilder, convert the semicolon to an OR clause. 
					name = name.Replace(";", " or ");
					sSQL = "select ID                     as ID           " + ControlChars.CrLf
					     + "     , FIRST_NAME             as NAME1        " + ControlChars.CrLf
					     + "     , LAST_NAME              as NAME2        " + ControlChars.CrLf
					     + "     , ACCOUNT_NAME           as ASSOCIATION  " + ControlChars.CrLf
					     + "     , N'Contact'             as TYPE         " + ControlChars.CrLf
					     + "     , EMAIL1                 as EMAIL_ADDRESS" + ControlChars.CrLf
					     + "  from vwCONTACTS_List                        " + ControlChars.CrLf
					     + " where 1 = 1                                  " + ControlChars.CrLf
					     +  Contacts.SearchContacts.UnifiedSearch(name, cmd);
					nACLACCESS = Security.GetUserAccess("Contacts", "list");
					if ( nACLACCESS < 0 )
						sSQL += sSQL + "   and 1 = 0" + ControlChars.CrLf;
					else if ( nACLACCESS == ACL_ACCESS.OWNER )
						sSQL += sSQL + "   and ASSIGNED_USER_ID = '" + gUSER_ID.ToString() + "'" + ControlChars.CrLf;
					sb.Append(sSQL);

					// 05/23/2006 Paul.  Add space after the query to prevent UNION ALL from touching a previous field or keyword. 
					sSQL = " union all                                    " + ControlChars.CrLf
					     + "select ID                     as ID           " + ControlChars.CrLf
					     + "     , FIRST_NAME             as NAME1        " + ControlChars.CrLf
					     + "     , LAST_NAME              as NAME2        " + ControlChars.CrLf
					     + "     , ACCOUNT_NAME           as ASSOCIATION  " + ControlChars.CrLf
					     + "     , N'Lead'                as TYPE         " + ControlChars.CrLf
					     + "     , EMAIL1                 as EMAIL_ADDRESS" + ControlChars.CrLf
					     + "  from vwLEADS_List                           " + ControlChars.CrLf
					     + " where 1 = 1                                  " + ControlChars.CrLf
					     +  Leads.SearchLeads.UnifiedSearch(name, cmd);
					nACLACCESS = Security.GetUserAccess("Leads", "list");
					if ( nACLACCESS < 0 )
						sSQL += sSQL + "   and 1 = 0" + ControlChars.CrLf;
					else if ( nACLACCESS == ACL_ACCESS.OWNER )
						sSQL += sSQL + "   and ASSIGNED_USER_ID = '" + gUSER_ID.ToString() + "'" + ControlChars.CrLf;
					sb.Append(sSQL);

					// 05/23/2006 Paul.  Add space after the query to prevent UNION ALL from touching a previous field or keyword. 
					sSQL = " union all                                    " + ControlChars.CrLf
					     + "select ID                     as ID           " + ControlChars.CrLf
					     + "     , N''                    as NAME1        " + ControlChars.CrLf
					     + "     , NAME                   as NAME2        " + ControlChars.CrLf
					     + "     , BILLING_ADDRESS_CITY   as ASSOCIATION  " + ControlChars.CrLf
					     + "     , N'Account'             as TYPE         " + ControlChars.CrLf
					     + "     , EMAIL1                 as EMAIL_ADDRESS" + ControlChars.CrLf
					     + "  from vwACCOUNTS_List                        " + ControlChars.CrLf
					     + " where 1 = 1                                  " + ControlChars.CrLf
					     +  Accounts.SearchAccounts.UnifiedSearch(name, cmd);
					nACLACCESS = Security.GetUserAccess("Accounts", "list");
					if ( nACLACCESS < 0 )
						sSQL += sSQL + "   and 1 = 0" + ControlChars.CrLf;
					else if ( nACLACCESS == ACL_ACCESS.OWNER )
						sSQL += sSQL + "   and ASSIGNED_USER_ID = '" + gUSER_ID.ToString() + "'" + ControlChars.CrLf;
					sb.Append(sSQL);

					// 05/23/2006 Paul.  Add space after the query to prevent UNION ALL from touching a previous field or keyword. 
					sSQL = " union all                                    " + ControlChars.CrLf
					     + "select ID                     as ID           " + ControlChars.CrLf
					     + "     , N''                    as NAME1        " + ControlChars.CrLf
					     + "     , NAME                   as NAME2        " + ControlChars.CrLf
					     + "     , ACCOUNT_NAME           as ASSOCIATION  " + ControlChars.CrLf
					     + "     , N'Case'                as TYPE         " + ControlChars.CrLf
					     + "     , N''                    as EMAIL_ADDRESS" + ControlChars.CrLf
					     + "  from vwCASES_List                           " + ControlChars.CrLf
					     + " where 1 = 1                                  " + ControlChars.CrLf
					     +  Cases.SearchCases.UnifiedSearch(name, cmd);
					nACLACCESS = Security.GetUserAccess("Cases", "list");
					if ( nACLACCESS < 0 )
						sSQL += sSQL + "   and 1 = 0" + ControlChars.CrLf;
					else if ( nACLACCESS == ACL_ACCESS.OWNER )
						sSQL += sSQL + "   and ASSIGNED_USER_ID = '" + gUSER_ID.ToString() + "'" + ControlChars.CrLf;
					sb.Append(sSQL);

					// 05/23/2006 Paul.  Add space after the query to prevent UNION ALL from touching a previous field or keyword. 
					sSQL = " union all                                    " + ControlChars.CrLf
					     + "select ID                     as ID           " + ControlChars.CrLf
					     + "     , N''                    as NAME1        " + ControlChars.CrLf
					     + "     , NAME                   as NAME2        " + ControlChars.CrLf
					     + "     , ACCOUNT_NAME           as ASSOCIATION  " + ControlChars.CrLf
					     + "     , N'Opportunity'         as TYPE         " + ControlChars.CrLf
					     + "     , N''                    as EMAIL_ADDRESS" + ControlChars.CrLf
					     + "  from vwOPPORTUNITIES_List                   " + ControlChars.CrLf
					     + " where 1 = 1                                  " + ControlChars.CrLf
					     +  Opportunities.SearchOpportunities.UnifiedSearch(name, cmd);
					nACLACCESS = Security.GetUserAccess("Opportunities", "list");
					if ( nACLACCESS < 0 )
						sSQL += sSQL + "   and 1 = 0" + ControlChars.CrLf;
					else if ( nACLACCESS == ACL_ACCESS.OWNER )
						sSQL += sSQL + "   and ASSIGNED_USER_ID = '" + gUSER_ID.ToString() + "'" + ControlChars.CrLf;
					sb.Append(sSQL);

					// 06/01/2006 Paul.  The string builder contains the full query. 
					cmd.CommandText = sb.ToString();
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 02/20/2006 Paul.  First initialize the array. 
									results = new contact_detail[dt.Rows.Count];
									for ( int i=0; i < dt.Rows.Count ; i++ )
									{
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										results[i] = new contact_detail();
										results[i].email_address = Sql.ToString(dt.Rows[i]["EMAIL_ADDRESS"]);
										results[i].name1         = Sql.ToString(dt.Rows[i]["NAME1"        ]);
										results[i].name2         = Sql.ToString(dt.Rows[i]["NAME2"        ]);
										results[i].association   = Sql.ToString(dt.Rows[i]["ASSOCIATION"  ]);
										results[i].id            = Sql.ToString(dt.Rows[i]["ID"           ]);
										results[i].type          = Sql.ToString(dt.Rows[i]["TYPE"         ]);
										results[i].msi_id        = (i+1).ToString();
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed search()", ex));
					}
				}
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public string track_email(string user_name, string password, string parent_id, string contact_ids, DateTime date_sent, string email_subject, string email_body)
		{
			// 03/12/2007 Paul.  If using NTLM, then user_name will be updated with value from Identity object. 
			Guid gUSER_ID = LoginUser(ref user_name, password);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return String.Empty;
		}
		#endregion

		#region Session-required functions
		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public get_entry_list_result get_entry_list(string session, string module_name, string query, string order_by, int offset, string[] select_fields, int max_results, int deleted)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			Guid gTIMEZONE = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

			if ( offset < 0 )
				throw(new Exception("offset must be a non-negative number"));
			if ( max_results <= 0 )
				throw(new Exception("max_results must be a postive number"));

			string sTABLE_NAME = VerifyModuleName(module_name);
			query       = query.ToUpper();
			order_by    = order_by.ToUpper();
			query    = query   .Replace(sTABLE_NAME + ".", String.Empty);
			order_by = order_by.Replace(sTABLE_NAME + ".", String.Empty);
			
			int nACLACCESS = Security.GetUserAccess(module_name, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			get_entry_list_result results = new get_entry_list_result();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select *" + ControlChars.CrLf
				     + "  from " + sTABLE_NAME + ControlChars.CrLf
				     + " where DELETED = @DELETED" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					if ( !Sql.IsEmptyString(query) )
					{
						// 02/16/2006 Paul.  As much as I dislike the idea of allowing a query string, 
						// I don't have the time to parse this.
						// 03/08/2006 Paul.  Prepend the AND clause. 
						sSQL += "   and " + query + ControlChars.CrLf;
					}
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@DELETED", Math.Min(deleted, 1));
					if ( nACLACCESS == ACL_ACCESS.OWNER )
					{
						// 09/01/2006 Paul.  Notes do not have an ASSIGNED_USER_ID. 
						if ( sTABLE_NAME != "NOTES" )
							Sql.AppendParameter(cmd, gUSER_ID, "ASSIGNED_USER_ID");
					}
					try
					{
						CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 02/16/2006 Paul.  Don't sort in the database as it provides a hacker another attempt at SQL-Injection. 
									// Bad sort values will just throw an exception here. 
									DataView dv = new DataView(dt);
									dv.Sort = order_by;

									results.result_count = Math.Min(dt.Rows.Count - offset, max_results);
									results.next_offset  = offset + results.result_count;
									
									// 02/20/2006 Paul.  First initialize the array. 
									results.field_list = new field      [select_fields.Length];
									results.entry_list = new entry_value[results.result_count];
									for ( int i=0; i < select_fields.Length; i++ )
									{
										string sColumnName = select_fields[i];
										DataColumn col = dt.Columns[sColumnName];
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										// 02/16/2006 Paul.  We don't have a mapping for the labels, so just return the column name. 
										// varchar, bool, datetime, int, text, blob
										results.field_list[i] = new field(sColumnName.ToLower(), col.DataType.ToString(), sColumnName, 0);
									}
									
									// 02/16/2006 Paul.  SugarCRM 3.5.1 returns all fields even though only a few were requested.  We will do the same. 
									int j = 0;
									foreach ( DataRowView row in dv )
									{
										if ( j >= offset && j < offset + results.result_count )
										{
											int nItem = j - offset;
											// 02/20/2006 Paul.  Then initialize each element in the array. 
											results.entry_list[nItem] = new entry_value();
											results.entry_list[nItem].id              = Sql.ToGuid(row["ID"]).ToString();
											results.entry_list[nItem].module_name     = module_name;
											// 02/20/2006 Paul.  First initialize the array. 
											results.entry_list[nItem].name_value_list = new name_value[dt.Columns.Count];
											int nColumn = 0;
											foreach ( DataColumn col in dt.Columns )
											{
												// 02/20/2006 Paul.  Then initialize each element in the array. 
												// 08/17/2006 Paul.  We need to convert all dates to UniversalTime. 
												if ( Information.IsDate(row[col.ColumnName]) )
												{
													// 08/17/2006 Paul.  The time on the server and the time in the database are both considered ServerTime. 
													DateTime dtServerTime = Sql.ToDateTime(row[col.ColumnName]);
													// 08/17/2006 Paul.  We need a special function to convert to UniversalTime because it might already be in UniversalTime, based on m_bGMTStorage flag. 
													DateTime dtUniversalTime = T10n.ToUniversalTimeFromServerTime(dtServerTime);
													results.entry_list[nItem].name_value_list[nColumn] = new name_value(col.ColumnName.ToLower(), dtUniversalTime.ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat));
												}
												else
												{
													results.entry_list[nItem].name_value_list[nColumn] = new name_value(col.ColumnName.ToLower(), Sql.ToString(row[col.ColumnName]));
												}
												nColumn++;
											}
										}
										j++;
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed get_entry_list", ex));
					}
				}
			}
			return results;
		}

		private string VerifyModuleName(string sMODULE_NAME)
		{
			string sTABLE_NAME = String.Empty;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select *                         " + ControlChars.CrLf
				     + "  from vwMODULES                 " + ControlChars.CrLf
				     + " where MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME);
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							sTABLE_NAME = Sql.ToString(rdr["TABLE_NAME"]);
						}
						else
						{
							throw(new Exception("This module is not available on this server"));
						}
					}
				}
			}
			return sTABLE_NAME;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public get_entry_result get_entry(string session, string module_name, string id, string[] select_fields)
		{
			Guid gUSER_ID = GetSessionUserID(session);

			string sTABLE_NAME = VerifyModuleName(module_name);
			int nACLACCESS = Security.GetUserAccess(module_name, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			get_entry_result results = new get_entry_result();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select *" + ControlChars.CrLf
				     + "  from " + sTABLE_NAME + ControlChars.CrLf
				     + " where ID = @ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", id);
					if ( nACLACCESS == ACL_ACCESS.OWNER )
					{
						// 09/01/2006 Paul.  Notes do not have an ASSIGNED_USER_ID. 
						if ( sTABLE_NAME != "NOTES" )
							Sql.AppendParameter(cmd, gUSER_ID, "ASSIGNED_USER_ID");
					}
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 02/20/2006 Paul.  First initialize the array. 
									results.field_list = new field      [select_fields.Length];
									results.entry_list = new entry_value[select_fields.Length];
									DataRow row = dt.Rows[0];
									for ( int i=0; i < select_fields.Length; i++ )
									{
										string sColumnName = select_fields[i];
										DataColumn col = dt.Columns[sColumnName];
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										// varchar, bool, datetime, int, text, blob
										results.field_list[i] = new field(sColumnName.ToLower(), col.DataType.ToString(), sColumnName, 0);
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										results.entry_list[i] = new entry_value(id, module_name, sColumnName, Sql.ToString(row[sColumnName]));
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed contact_by_email", ex));
					}
				}
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public get_entry_result get_entries(string session, string module_name, string[] ids, string[] select_fields)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			Guid gTIMEZONE = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

			string sTABLE_NAME = VerifyModuleName(module_name);
			int nACLACCESS = Security.GetUserAccess(module_name, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			get_entry_result results = new get_entry_result();
			// 02/19/2006 Paul.  Exit early if nothing to get.  We need to prevent fetching all recods. 
			if ( ids.Length == 0 )
				return results;

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				// 02/16/2006 Paul.  Convert the table name to a VIEW.  We can do this because we don't want deleted records. 
				// 02/18/2006 Paul.  Use the Edit view as it will include description, content, etc. 
				sSQL = "select *" + ControlChars.CrLf
				     + "  from vw" + sTABLE_NAME + "_Edit" + ControlChars.CrLf
				     + " where 1 = 1" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 02/19/2006 Paul.  Need to filter by the IDs povided. 
					Sql.AppendParameter(cmd, ids, "ID");
					if ( nACLACCESS == ACL_ACCESS.OWNER )
					{
						// 09/01/2006 Paul.  Notes do not have an ASSIGNED_USER_ID. 
						if ( sTABLE_NAME != "NOTES" )
							Sql.AppendParameter(cmd, gUSER_ID, "ASSIGNED_USER_ID");
					}
					try
					{
						CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 02/20/2006 Paul.  First initialize the array. 
									results.field_list = new field      [select_fields.Length];
									results.entry_list = new entry_value[dt.Rows.Count];
									for ( int i=0; i < select_fields.Length; i++ )
									{
										string sColumnName = select_fields[i];
										DataColumn col = dt.Columns[sColumnName];
										// 02/21/2006 Paul.  Column may not exist.  For example, we don't return a MEETINGS.TIME_START. 
										if ( col != null )
										{
											// 02/20/2006 Paul.  Then initialize each element in the array. 
											// 02/16/2006 Paul.  We don't have a mapping for the labels, so just return the column name. 
											// varchar, bool, datetime, int, text, blob
											results.field_list[i] = new field(sColumnName.ToLower(), col.DataType.ToString(), sColumnName, 0);
										}
									}
									
									// 02/16/2006 Paul.  SugarCRM 3.5.1 returns all fields even though only a few were requested.  We will do the same. 
									int nItem = 0;
									foreach ( DataRow row in dt.Rows )
									{
										// 02/20/2006 Paul.  Then initialize each element in the array. 
										results.entry_list[nItem] = new entry_value();
										results.entry_list[nItem].id              = Sql.ToGuid(row["ID"]).ToString();
										results.entry_list[nItem].module_name     = module_name;
										// 02/20/2006 Paul.  First initialize the array. 
										results.entry_list[nItem].name_value_list = new name_value[dt.Columns.Count];
										int nColumn = 0;
										foreach ( DataColumn col in dt.Columns )
										{
											// 02/20/2006 Paul.  Then initialize each element in the array. 
											// 08/17/2006 Paul.  We need to convert all dates to UniversalTime. 
											if ( Information.IsDate(row[col.ColumnName]) )
											{
												// 08/17/2006 Paul.  The time on the server and the time in the database are both considered ServerTime. 
												DateTime dtServerTime = Sql.ToDateTime(row[col.ColumnName]);
												// 08/17/2006 Paul.  We need a special function to convert to UniversalTime because it might already be in UniversalTime, based on m_bGMTStorage flag. 
												DateTime dtUniversalTime = T10n.ToUniversalTimeFromServerTime(dtServerTime);
												results.entry_list[nItem].name_value_list[nColumn] = new name_value(col.ColumnName.ToLower(), dtUniversalTime.ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat));
											}
											else
											{
												results.entry_list[nItem].name_value_list[nColumn] = new name_value(col.ColumnName.ToLower(), Sql.ToString(row[col.ColumnName]));
											}
											nColumn++;
										}
										nItem++;
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed get_entries", ex));
					}
				}
			}
			return results;
		}

		private bool DeleteEntry(name_value[] name_value_list)
		{
			bool bDelete = false;
			for ( int j = 0; j < name_value_list.Length; j++ )
			{
				if ( String.Compare(name_value_list[j].name, "deleted", true) == 0 )
				{
					if ( name_value_list[j].value == "1" )
						bDelete = true;
				}
			}
			return bDelete;
		}

		private string EntryDateTime(name_value[] name_value_list, string sDateField, string sTimeField)
		{
			string sDateTime = String.Empty;
			string sDate     = String.Empty;
			string sTime     = String.Empty;
			for ( int j = 0; j < name_value_list.Length; j++ )
			{
				if ( String.Compare(name_value_list[j].name, sDateField, true) == 0 )
					sDate = name_value_list[j].value;
				if ( String.Compare(name_value_list[j].name, sTimeField, true) == 0 )
					sTime = name_value_list[j].value;
			}
			sDateTime = sDate + " " + sTime;
			return sDateTime;
		}

		private void InitializeParameters(IDbConnection con, string sTABLE_NAME, Guid gID, IDbCommand cmdUpdate)
		{
			String sSQL = String.Empty;
			sSQL = "select *" + ControlChars.CrLf
			     + "  from vw" + sTABLE_NAME + "_Edit" + ControlChars.CrLf
			     + " where ID = @ID" + ControlChars.CrLf;
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				cmd.CommandText = sSQL;
				Sql.AddParameter(cmd, "@ID", gID);
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								DataRow row = dt.Rows[0];
								foreach ( DataColumn col in dt.Columns )
								{
									IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
									if ( par != null )
									{
										par.Value = row[col.ColumnName];
									}
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
		}

		private Guid FindID(name_value[] name_value_list)
		{
			Guid gID = Guid.Empty;
			for ( int j = 0; j < name_value_list.Length; j++ )
			{
				if ( String.Compare(name_value_list[j].name, "id", true) == 0 )
				{
					gID = Sql.ToGuid(name_value_list[j].value);
					break;
				}
			}
			return gID;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public set_entry_result set_entry(string session, string module_name, name_value[] name_value_list)
		{
			Guid gUSER_ID  = GetSessionUserID(session);
			Guid gTIMEZONE = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

			string sTABLE_NAME = VerifyModuleName(module_name);
			int nACLACCESS = Security.GetUserAccess(module_name, "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			set_entry_result results = new set_entry_result();

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				// 02/21/2006 Paul.  Delete operations come in as set_entry with deleted = 1. 
				if ( DeleteEntry(name_value_list) )
				{
					IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Delete");
					// 10/10/2006 Paul.  Use IDbDataParameter to be consistent. 
					foreach(IDbDataParameter par in cmdDelete.Parameters)
					{
						par.Value = DBNull.Value;
					}
					Sql.SetParameter(cmdDelete, "@MODIFIED_USER_ID", gUSER_ID.ToString());
					Guid gID = FindID(name_value_list);
					if ( gID != Guid.Empty )
					{
						Sql.SetParameter(cmdDelete, "@ID", gID.ToString());
						cmdDelete.ExecuteNonQuery();
					}
				}
				else
				{
					IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
					IDbDataParameter parID = Sql.FindParameter(cmdUpdate, "@ID");
					// 10/10/2006 Paul.  Use IDbDataParameter to be consistent. 
					foreach(IDbDataParameter par in cmdUpdate.Parameters)
					{
						par.Value = DBNull.Value;
					}
					// 08/31/2006 Paul.  We need to initialize the values of any fields not provided. 
					// The stored procedure always updates all fields, so we need to make sure not to clear fields that are not provided. 
					// This problem was first noticed when the Outlook Plug-in kept clearing the ASSIGNED_USER_ID field. 
					Guid gID = FindID(name_value_list);
					if ( gID != Guid.Empty )
					{
						// 08/31/2006 Paul.  If the ID is not found, then this must be a new 
						InitializeParameters(con, sTABLE_NAME, gID, cmdUpdate);
					}
					Sql.SetParameter(cmdUpdate, "@MODIFIED_USER_ID", gUSER_ID.ToString());

					for ( int j = 0; j < name_value_list.Length; j++ )
					{
						// 04/04/2006 Paul.  DATE_START & TIME_START need to be combined into DATE_TIME. 
						if ( name_value_list[j].name.ToUpper() == "TIME_START" )
						{
							// 04/04/2006 Paul.  Modules that have a TIME_START field are MEETINGS, CALLS, TASKS, EMAILS, EMAIL_MARKETING, PROJECT_TASK
							string sDateTime = EntryDateTime(name_value_list, "DATE_START", "TIME_START");
							if ( sTABLE_NAME == "TASKS" || sTABLE_NAME == "PROJECT_TASK" )
							{
								Sql.SetParameter(cmdUpdate, "@DATE_TIME_START", T10n.ToServerTimeFromUniversalTime(sDateTime));
							}
							else
							{
								Sql.SetParameter(cmdUpdate, "@DATE_TIME", T10n.ToServerTimeFromUniversalTime(sDateTime));
							}
						}
						// 04/04/2006 Paul.  DATE_DUE & TIME_DUE need to be combined into DATE_TIME_DUE. 
						else if ( name_value_list[j].name.ToUpper() == "TIME_DUE" )
						{
							// 04/04/2006 Paul.  Modules that have a TIME_DUE field are TASKS, PROJECT_TASK
							string sDateTime = EntryDateTime(name_value_list, "DATE_DUE", "TIME_DUE");
							Sql.SetParameter(cmdUpdate, "@DATE_TIME_DUE", T10n.ToServerTimeFromUniversalTime(sDateTime));
						}
						else
						{
							Sql.SetParameter(cmdUpdate, "@" + name_value_list[j].name, name_value_list[j].value);
						}
					}
					cmdUpdate.ExecuteNonQuery();

					if ( parID != null )
					{
						results.id = Sql.ToString(parID.Value);
					}
				}
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public set_entries_result set_entries(string session, string module_name, name_value[][] name_value_lists)
		{
			Guid gUSER_ID  = GetSessionUserID(session);
			Guid gTIMEZONE = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

			string sTABLE_NAME = VerifyModuleName(module_name);
			int nACLACCESS = Security.GetUserAccess(module_name, "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			set_entries_result results = new set_entries_result();
			results.ids = new string[name_value_lists.Length];

			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
				IDbDataParameter parID = Sql.FindParameter(cmdUpdate, "@ID");
				
				for ( int i=0; i < name_value_lists.Length ; i++ )
				{
					name_value[] name_value_list = name_value_lists[i];
					// 10/10/2006 Paul.  Use IDbDataParameter to be consistent. 
					foreach(IDbDataParameter par in cmdUpdate.Parameters)
					{
						par.Value = DBNull.Value;
					}
					// 08/31/2006 Paul.  We need to initialize the values of any fields not provided. 
					// The stored procedure always updates all fields, so we need to make sure not to clear fields that are not provided. 
					// This problem was first noticed when the Outlook Plug-in kept clearing the ASSIGNED_USER_ID field. 
					Guid gID = FindID(name_value_list);
					if ( gID != Guid.Empty )
					{
						// 08/31/2006 Paul.  If the ID is not found, then this must be a new 
						InitializeParameters(con, sTABLE_NAME, gID, cmdUpdate);
					}
					Sql.SetParameter(cmdUpdate, "@MODIFIED_USER_ID", gUSER_ID.ToString());
					for ( int j = 0; j < name_value_lists[i].Length; j++ )
					{
						// 04/04/2006 Paul.  DATE_START & TIME_START need to be combined into DATE_TIME. 
						if ( name_value_list[j].name.ToUpper() == "TIME_START" )
						{
							// MEETINGS, CALLS, TASKS, EMAILS, EMAIL_MARKETING, PROJECT_TASK
							string sDateTime = EntryDateTime(name_value_list, "DATE_START", "TIME_START");
							if ( sTABLE_NAME == "TASKS" || sTABLE_NAME == "PROJECT_TASK" )
							{
								Sql.SetParameter(cmdUpdate, "@DATE_TIME_START", T10n.ToServerTimeFromUniversalTime(sDateTime));
							}
							else
							{
								Sql.SetParameter(cmdUpdate, "@DATE_TIME", T10n.ToServerTimeFromUniversalTime(sDateTime));
							}
						}
						// 04/04/2006 Paul.  DATE_DUE & TIME_DUE need to be combined into DATE_TIME_DUE. 
						else if ( name_value_list[j].name.ToUpper() == "TIME_DUE" )
						{
							// TASKS, PROJECT_TASK
							string sDateTime = EntryDateTime(name_value_list, "DATE_DUE", "TIME_DUE");
							Sql.SetParameter(cmdUpdate, "@DATE_TIME_DUE", T10n.ToServerTimeFromUniversalTime(sDateTime));
						}
						else
						{
							Sql.SetParameter(cmdUpdate, "@" + name_value_list[j].name, name_value_list[j].value);
						}
					}
					cmdUpdate.ExecuteNonQuery();

					if ( parID != null )
					{
						results.ids[i] = Sql.ToString(parID.Value);
					}
				}
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public set_entry_result set_note_attachment(string session, note_attachment note)
		{
			Guid   gUSER_ID        = GetSessionUserID(session);
			Guid   gNOTE_ID        = Sql.ToGuid(note.id);
			string sFILENAME       = Path.GetFileName (note.filename);
			string sFILE_EXT       = Path.GetExtension(sFILENAME);
			string sFILE_MIME_TYPE = "application/octet-stream";
			
			int nACLACCESS = Security.GetUserAccess("Notes", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			set_entry_result result = new set_entry_result();
			byte[] byData = Convert.FromBase64String(note.file);
			// 02/20/2006 Paul.  Try and reduce the memory requirements by releasing the original data as soon as possible. 
			note.file = null;
			using ( MemoryStream stm = new System.IO.MemoryStream(byData) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					Guid gASSIGNED_USER_ID = Guid.Empty;

					/*
					// 09/01/2006 Paul.  Notes do not have an ASSIGNED_USER_ID. 
					string sSQL = String.Empty;
					sSQL = "select *           " + ControlChars.CrLf
					     + "  from vwNOTES_Edit" + ControlChars.CrLf
					     + " where ID = @ID    " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gNOTE_ID);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							if ( rdr.Read() )
							{
								gASSIGNED_USER_ID = Sql.ToGuid(rdr["ASSIGNED_USER_ID"]);
							}
						}
					}
					*/
					if ( nACLACCESS != ACL_ACCESS.OWNER || (nACLACCESS == ACL_ACCESS.OWNER  && gASSIGNED_USER_ID == gUSER_ID) )
					{
						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								Guid gAttachmentID = Guid.Empty;
								SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gAttachmentID, gNOTE_ID, note.filename, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
								SplendidCRM.Notes.EditView.LoadFile(gAttachmentID, stm, trn);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								throw ( new Exception(ex.Message) );
							}
						}
					}
				}
			}
			byData = null;
			return result;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public return_note_attachment get_note_attachment(string session, string id)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return null;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public error_value relate_note_to_module(string session, string note_id, string module_name, string module_id)
		{
			Guid   gUSER_ID     = GetSessionUserID(session);
			Guid   gNOTE_ID     = Sql.ToGuid(note_id);
			string sPARENT_TYPE = module_name;
			Guid   gPARENT_ID   = Guid.Empty;
			Guid   gCONTACT_ID  = Guid.Empty;
			if ( String.Compare(sPARENT_TYPE, "Contacts", true) == 0 )
				gCONTACT_ID = Sql.ToGuid(module_id);
			else
				gPARENT_ID = Sql.ToGuid(module_id);

			int nACLACCESS = Security.GetUserAccess("Notes", "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			error_value results = new error_value();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select *           " + ControlChars.CrLf
				     + "  from vwNOTES_Edit" + ControlChars.CrLf
				     + " where ID = @ID    " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gNOTE_ID);
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						if ( rdr.Read() )
						{
							string sNAME             = Sql.ToString(rdr["NAME"            ]);
							string sDESCRIPTION      = Sql.ToString(rdr["DESCRIPTION"     ]);
							// 09/01/2006 Paul.  Notes do not have an ASSIGNED_USER_ID. 
							Guid   gASSIGNED_USER_ID = Guid.Empty;  // Sql.ToGuid  (rdr["ASSIGNED_USER_ID"]);
							if ( nACLACCESS != ACL_ACCESS.OWNER || (nACLACCESS == ACL_ACCESS.OWNER  && gASSIGNED_USER_ID == gUSER_ID) )
							{
								SqlProcs.spNOTES_Update
									( ref gNOTE_ID
									, sNAME
									, sPARENT_TYPE
									, gPARENT_ID
									, gCONTACT_ID
									, sDESCRIPTION
									, Sql.ToGuid(rdr["TEAM_ID"])
									);
							}
						}
					}
				}
			}

			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public get_entry_result get_related_notes(string session, string module_name, string module_id, string[] select_fields)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return null;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public module_fields get_module_fields(string session, string module_name)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return null;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public module_list get_available_modules(string session)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return null;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public error_value update_portal_user(string session, string portal_name, name_value[] name_value_list)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return null;
		}

		// 02/16/2006 Paul.  The query string is expected to be in a very specific format. 
		// string sDateQuery = "date_start > '" + dtStartDate.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss") + "' AND date_start < '" + dtEndDate.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss") + "'";
		private static void ParseDateRange(string sQuery, string sField, TimeZone T10n, ref DateTime dtBeginDate, ref DateTime dtEndDate)
		{
			dtBeginDate = DateTime.MinValue;
			dtEndDate   = DateTime.MinValue;
			sQuery = sQuery.ToUpper();
			// 02/16/ 2006 Paul.  Remove excess whitespace. 
			Regex r = new Regex(@"[^\w]+");
			sQuery = r.Replace(sQuery, " ");
			// 03/19/2006 Paul.  Use VB split as the C# split will split on each character. 
			string[] aQuery = Strings.Split(sQuery, " AND ", -1, CompareMethod.Text);
			foreach ( string s in aQuery )
			{
				if ( s.StartsWith("DATE_START > ") )
				{
					string sDate = s.Substring("DATE_START > ".Length);
					sDate = sDate.Replace("\'", "");
					dtBeginDate = DateTime.Parse(sDate);
					// 04/04/2006 Paul.  Make sure to convert to server time. 
					dtBeginDate = T10n.ToServerTimeFromUniversalTime(dtBeginDate);
				}
				else if ( s.StartsWith("DATE_START < ") )
				{
					string sDate = s.Substring("DATE_START < ".Length);
					sDate = sDate.Replace("\'", "");
					dtEndDate = DateTime.Parse(sDate);
					// 04/04/2006 Paul.  Make sure to convert to server time. 
					dtEndDate = T10n.ToServerTimeFromUniversalTime(dtEndDate);
				}
			}
		}

		/*
		<tns:sync_get_modified_relationships>
			<session xsi:type="xsd:string">e7b36ceb478224f55fec23ed2febc673</session>
			<module_name xsi:type="xsd:string">Users</module_name>
			<related_module xsi:type="xsd:string">Contacts</related_module>
			<from_date xsi:type="xsd:string">2007-06-20 01:19:54</from_date>
			<to_date xsi:type="xsd:string">2007-06-20 01:21:59</to_date>
			<offset xsi:type="xsd:int">0</offset>
			<max_results xsi:type="xsd:int">3000</max_results>
			<deleted xsi:type="xsd:int">0</deleted>
			<module_id xsi:type="xsd:string">1</module_id>
			<select_fields href="#id1" />
			<ids href="#id2" />
			<relationship_name xsi:type="xsd:string">contacts_users</relationship_name>
			<deletion_date xsi:type="xsd:string">2007-06-20 01:19:52</deletion_date>
			<php_serialize xsi:type="xsd:int">0</php_serialize>
		</tns:sync_get_modified_relationships>
		<soapenc:Array id="id1" soapenc:arrayType="xsd:string[6]">
			<Item>id</Item>
			<Item>date_modified</Item>
			<Item>deleted</Item>
			<Item>first_name</Item>
			<Item>last_name</Item>
			<Item>rt.deleted synced</Item>
		</soapenc:Array>
		<soapenc:Array id="id2" soapenc:arrayType="xsd:string[0]" />
		*/
		// 06/19/2007 Paul.  Starting with version 4.2, SugarCRM uses a function that optimizes syncing. 
		// 06/19/2007 Paul.  Starting with version 4.5.1d, SugarCRM no longer allows get_relationships() to get user/contact relationships. 
		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public get_entry_list_result_encoded sync_get_modified_relationships(string session, string module_name, string related_module, string from_date, string to_date, int offset, int max_results, int deleted, string module_id, string[] select_fields, string[] ids, string relationship_name, string deletion_date, int php_serialize)
		{
			Guid gUSER_ID  = GetSessionUserID(session);
			Guid gTIMEZONE = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

			string sTABLE_NAME = VerifyModuleName(module_name   );
			int nACLACCESS = Security.GetUserAccess(module_name, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			sTABLE_NAME = VerifyModuleName(related_module);
			nACLACCESS = Security.GetUserAccess(related_module, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			get_entry_list_result_encoded results = new get_entry_list_result_encoded();
			results.error.name        = "not supported";
			results.error.number      = "-1";
			results.error.description = "sync_get_modified_relationships is not supported at this time";
			return results;
		}


		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public get_relationships_result get_relationships(string session, string module_name, string module_id, string related_module, string related_module_query, int deleted)
		{
			Guid gUSER_ID  = GetSessionUserID(session);
			Guid gTIMEZONE = Sql.ToGuid(HttpContext.Current.Cache.Get("soap.user.timezone." + gUSER_ID.ToString()));
			TimeZone T10n = TimeZone.CreateTimeZone(gTIMEZONE);

			string sTABLE_NAME = VerifyModuleName(module_name   );
			int nACLACCESS = Security.GetUserAccess(module_name, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			sTABLE_NAME = VerifyModuleName(related_module);
			nACLACCESS = Security.GetUserAccess(related_module, "list");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			get_relationships_result results = new get_relationships_result();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				// 02/16/2006 Paul.  Providing a way to directly access tables is a hacker's dream.  
				// We will not do that here.  We will require that all relationships be defined in a SQL view. 
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					switch ( module_name )
					{
						case "Contacts":
						{
							switch ( related_module )
							{
								case "Calls":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwCONTACTS_CALLS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								case "Meetings":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwCONTACTS_MEETINGS_Soap" + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								case "Users":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwCONTACTS_USERS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										throw(new Exception(String.Format("A related_module_query is not allowed at this time.", module_name, related_module)));
									}
									break;
								}
								default:
								{
									throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", module_name, related_module)));
								}
							}
							break;
						}
						case "Users":
						{
							switch ( related_module )
							{
								case "Calls":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwUSERS_CALLS_Soap      " + ControlChars.CrLf
									     + " where PRIMARY_ID  = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								case "Meetings":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwUSERS_MEETINGS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID  = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								case "Contacts":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwUSERS_CONTACTS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										throw(new Exception(String.Format("A related_module_query is not allowed at this time.", module_name, related_module)));
									}
									break;
								}
								default:
								{
									throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", module_name, related_module)));
								}
							}
							break;
						}
						case "Meetings":
						{
							switch ( related_module )
							{
								case "Contacts":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwMEETINGS_CONTACTS_Soap" + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								case "Users":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwMEETINGS_USERS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								default:
								{
									throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", module_name, related_module)));
								}
							}
							break;
						}
						case "Calls":
						{
							switch ( related_module )
							{
								case "Contacts":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwCALLS_CONTACTS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								case "Users":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwCALLS_USERS_Soap      " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										DateTime dtBeginDate = DateTime.MinValue;
										DateTime dtEndDate   = DateTime.MinValue;
										ParseDateRange(related_module_query, "DATE_START", T10n, ref dtBeginDate, ref dtEndDate);
										if ( dtBeginDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START > @BEGIN_DATE" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@BEGIN_DATE", dtBeginDate);
										}
										if ( dtEndDate != DateTime.MinValue )
										{
											cmd.CommandText += "   and DATE_START < @END_DATE  " + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@END_DATE"  , dtEndDate  );
										}
									}
									break;
								}
								default:
								{
									throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", module_name, related_module)));
								}
							}
							break;
						}
						case  "Accounts":
						{
							switch ( related_module )
							{
								case "Contacts":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwACCOUNTS_CONTACTS_Soap" + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										throw(new Exception(String.Format("A related_module_query is not allowed at this time.", module_name, related_module)));
									}
									break;
								}
								case "Users":
								{
									sSQL = "select *                       " + ControlChars.CrLf
									     + "  from vwACCOUNTS_USERS_Soap   " + ControlChars.CrLf
									     + " where PRIMARY_ID = @PRIMARY_ID" + ControlChars.CrLf
									     + "   and DELETED    = @DELETED   " + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@PRIMARY_ID", module_id           );
									Sql.AddParameter(cmd, "@DELETED"   , Math.Min(deleted, 1));
									if ( !Sql.IsEmptyString(related_module_query) )
									{
										throw(new Exception(String.Format("A related_module_query is not allowed at this time.", module_name, related_module)));
									}
									break;
								}
								default:
								{
									throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", module_name, related_module)));
								}
							}
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", module_name, related_module)));
						}
					}

					try
					{
						CultureInfo ciEnglish = CultureInfo.CreateSpecificCulture("en-US");
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									results.ids = new id_mod[dt.Rows.Count];
									int i = 0;
									foreach ( DataRow row in dt.Rows )
									{
										results.ids[i] = new id_mod();
										results.ids[i].id            = Sql.ToString  (row["RELATED_ID"   ]);
										results.ids[i].deleted       = Sql.ToInteger (row["DELETED"      ]);
										// 06/13/2006 Paul.  Italian has a problem with the time separator.  Use the value from the culture from CalendarControl.SqlDateTimeFormat. 
										// 06/14/2006 Paul.  The Italian problem was that it was using the culture separator, but DataView only supports the en-US format. 
										// 08/17/2006 Paul.  The time on the server and the time in the database are both considered ServerTime. 
										DateTime dtDATE_MODIFIED_ServerTime = Sql.ToDateTime(row["DATE_MODIFIED"]);
										// 08/17/2006 Paul.  We need a special function to convert to UniversalTime because it might already be in UniversalTime, based on m_bGMTStorage flag. 
										DateTime dtDATE_MODIFIED_UniversalTime = T10n.ToUniversalTimeFromServerTime(dtDATE_MODIFIED_ServerTime);
										results.ids[i].date_modified = dtDATE_MODIFIED_UniversalTime.ToString(CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat);
										i++;
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						throw(new Exception("SOAP: Failed get_relationships", ex));
					}
				}
			}
			return results;
		}

		private void SetRelationship(string sMODULE1, string sMODULE1_ID, string sMODULE2, string sMODULE2_ID)
		{
			switch ( sMODULE1 )
			{
				case "Contacts":
				{
					Guid gCONTACT_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Calls":
						{
							Guid gCALL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCALLS_CONTACTS_Update(gCALL_ID, gCONTACT_ID, false, String.Empty);
							break;
						}
						case "Meetings":
						{
							Guid gMEETING_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spMEETINGS_CONTACTS_Update(gMEETING_ID, gCONTACT_ID, false, String.Empty);
							break;
						}
						case "Users":
						{
							Guid gUSER_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not defined. 
							// 05/14/2007 Paul.  The SugarCRM plug-in technique for unsyncing a contact is to send NULL as the USER_ID. 
							if ( Sql.IsEmptyGuid(gUSER_ID) )
								SqlProcs.spCONTACTS_USERS_Delete(gCONTACT_ID, Security.USER_ID);
							else
								SqlProcs.spCONTACTS_USERS_Update(gCONTACT_ID, gUSER_ID);
							break;
						}
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spEMAILS_CONTACTS_Update(gEMAIL_ID, gCONTACT_ID);
							break;
						}
						// 08/17/2006 Paul.  New relationships. 
						case "Accounts":
						{
							Guid gACCOUNT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spACCOUNTS_CONTACTS_Update(gACCOUNT_ID, gCONTACT_ID);
							break;
						}
						case "Bugs":
						{
							Guid gBUG_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCONTACTS_BUGS_Update(gBUG_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Cases":
						{
							Guid gCASE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCONTACTS_CASES_Update(gCASE_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Contract":
						{
							Guid gCONTRACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCONTRACTS_CONTACTS_Update(gCONTRACT_ID, gCONTACT_ID);
							break;
						}
						case "Opportunities":
						{
							Guid gOPPORTUNITY_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spOPPORTUNITIES_CONTACTS_Update(gOPPORTUNITY_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Project":
						{
							Guid gPROJECT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE1, gCONTACT_ID);
							break;
						}
						case "Quotes":
						{
							Guid gQUOTE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spQUOTES_CONTACTS_Update(gQUOTE_ID, gCONTACT_ID, String.Empty);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("A relationship between {0} and {1} has not been defined.", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case "Users":
				{
					Guid gUSER_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Calls":
						{
							Guid gCALL_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spCALLS_USERS_Update(gCALL_ID, gUSER_ID, false, String.Empty);
							break;
						}
						case "Meetings":
						{
							Guid gMEETING_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spMEETINGS_USERS_Update(gMEETING_ID, gUSER_ID, false, String.Empty);
							break;
						}
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spCONTACTS_USERS_Update(gCONTACT_ID, gUSER_ID);
							break;
						}
						// 08/17/2006 Paul.  New relationships. 
						case "Emails":
						{
							Guid gEMAILS_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_USERS_Update(gEMAILS_ID, gUSER_ID);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case "Meetings":
				{
					Guid gMEETING_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spMEETINGS_CONTACTS_Update(gMEETING_ID, gCONTACT_ID, false, String.Empty);
							break;
						}
						case "Users":
						{
							Guid gUSER_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spMEETINGS_USERS_Update(gMEETING_ID, gUSER_ID, false, String.Empty);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case "Calls":
				{
					Guid gCALL_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spCALLS_CONTACTS_Update(gCALL_ID, gCONTACT_ID, false, String.Empty);
							break;
						}
						case "Users":
						{
							Guid gUSER_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spCALLS_USERS_Update(gCALL_ID, gUSER_ID, false, String.Empty);
							break;
						}
						// 08/17/2006 Paul.  New relationships. 
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case  "Accounts":
				{
					Guid gACCOUNT_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spACCOUNTS_CONTACTS_Update(gACCOUNT_ID, gCONTACT_ID);
							break;
						}
						// 08/17/2006 Paul.  Relationship not defined.
						/*
						case "Users":
						{
							Guid gUSER_ID = Sql.ToGuid(sMODULE2_ID);
							break;
						}
						*/
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spEMAILS_ACCOUNTS_Update(gEMAIL_ID, gACCOUNT_ID);
							break;
						}
						// 08/17/2006 Paul.  New relationships. 
						case "Bugs":
						{
							Guid gBUG_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spACCOUNTS_BUGS_Update(gACCOUNT_ID, gBUG_ID);
							break;
						}
						case "Opportunities":
						{
							Guid gOPPORTUNITY_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spACCOUNTS_OPPORTUNITIES_Update(gACCOUNT_ID, gOPPORTUNITY_ID);
							break;
						}
						case "Project":
						{
							Guid gPROJECT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE1, gACCOUNT_ID);
							break;
						}
						case "Quotes":
						{
							Guid gQUOTE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spQUOTES_ACCOUNTS_Update(gQUOTE_ID, gACCOUNT_ID, String.Empty);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case  "Leads":
				{
					Guid gLEAD_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						// 08/17/2006 Paul.  Relationship is not defined. 
						/*
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							break;
						}
						case "Users":
						{
							Guid gUSER_ID = Sql.ToGuid(sMODULE2_ID);
							break;
						}
						*/
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							// 08/17/2006 Paul.  Relationship not previously created. 
							SqlProcs.spEMAILS_LEADS_Update(gEMAIL_ID, gLEAD_ID);
							break;
						}
						// 08/17/2006 Paul.  New relationships. 
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case  "Tasks":
				{
					Guid gTASK_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						// 08/17/2006 Paul.  New relationships. 
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_TASKS_Update(gEMAIL_ID, gTASK_ID);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case  "Opportunities":
				{
					Guid gOPPORTUNITY_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						// 08/17/2006 Paul.  New relationships. 
						case "Accounts":
						{
							Guid gACCOUNT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spACCOUNTS_OPPORTUNITIES_Update(gACCOUNT_ID, gOPPORTUNITY_ID);
							break;
						}
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spOPPORTUNITIES_CONTACTS_Update(gOPPORTUNITY_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Contracts":
						{
							Guid gCONTRACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCONTRACTS_OPPORTUNITIES_Update(gCONTRACT_ID, gOPPORTUNITY_ID);
							break;
						}
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_OPPORTUNITIES_Update(gEMAIL_ID, gOPPORTUNITY_ID);
							break;
						}
						case "Project":
						{
							Guid gPROJECT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE1, gOPPORTUNITY_ID);
							break;
						}
						case "Quotes":
						{
							Guid gQUOTE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spQUOTES_OPPORTUNITIES_Update(gQUOTE_ID, gOPPORTUNITY_ID);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case  "Project":
				{
					Guid gPROJECT_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						// 08/17/2006 Paul.  New relationships. 
						case "Accounts":
						{
							Guid gACCOUNT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE2, gACCOUNT_ID);
							break;
						}
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE2, gCONTACT_ID);
							break;
						}
						case "Opportunities":
						{
							Guid gOPPORTUNITY_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE2, gOPPORTUNITY_ID);
							break;
						}
						case "Quotes":
						{
							Guid gQUOTE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE2, gQUOTE_ID);
							break;
						}
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_PROJECTS_Update(gEMAIL_ID, gPROJECT_ID);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				case "Emails":
				{
					Guid gEMAIL_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						// 08/17/2006 Paul.  New relationships. 
						case "Accounts":
						{
							Guid gACCOUNT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_ACCOUNTS_Update(gEMAIL_ID, gACCOUNT_ID);
							break;
						}
						case "Bugs":
						{
							Guid gBUG_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_BUGS_Update(gEMAIL_ID, gBUG_ID);
							break;
						}
						case "Cases":
						{
							Guid gCASE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_CASES_Update(gEMAIL_ID, gCASE_ID);
							break;
						}
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_CONTACTS_Update(gEMAIL_ID, gCONTACT_ID);
							break;
						}
						case "Opportunities":
						{
							Guid gOPPORTUNITY_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_OPPORTUNITIES_Update(gEMAIL_ID, gOPPORTUNITY_ID);
							break;
						}
						case "Project":
						{
							Guid gPROJECT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_PROJECTS_Update(gEMAIL_ID, gPROJECT_ID);
							break;
						}
						case "Quotes":
						{
							Guid gQUOTE_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_QUOTES_Update(gEMAIL_ID, gQUOTE_ID);
							break;
						}
						case "Tasks":
						{
							Guid gTASK_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_TASKS_Update(gEMAIL_ID, gTASK_ID);
							break;
						}
						case "Users":
						{
							Guid gUSER_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_USERS_Update(gEMAIL_ID, gUSER_ID);
							break;
						}
						default:
						{
							throw(new Exception(String.Format("A relationship between {0} and {1} has not been defined.", sMODULE1, sMODULE2)));
						}
					}
					break;
				}
				// 12/07/2007 Paul.  The plug-in treats the bug as the parent when archiving an email. 
				case "Bugs":
				{
					Guid gBUG_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Accounts":
						{
							Guid gACCOUNT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spACCOUNTS_BUGS_Update(gACCOUNT_ID, gBUG_ID);
							break;
						}
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCONTACTS_BUGS_Update(gBUG_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_BUGS_Update(gEMAIL_ID, gBUG_ID);
							break;
						}
					}
					break;
				}
				// 12/07/2007 Paul.  The plug-in treats the case as the parent when archiving an email. 
				case "Cases":
				{
					Guid gCASE_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Contacts":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spCONTACTS_CASES_Update(gCASE_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_CASES_Update(gEMAIL_ID, gCASE_ID);
							break;
						}
					}
					break;
				}
				// 12/07/2007 Paul.  The plug-in treats the quote as the parent when archiving an email. 
				case "Quotes":
				{
					Guid gQUOTE_ID = Sql.ToGuid(sMODULE1_ID);
					switch ( sMODULE2 )
					{
						case "Accounts":
						{
							Guid gACCOUNT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spQUOTES_ACCOUNTS_Update(gQUOTE_ID, gACCOUNT_ID, String.Empty);
							break;
						}
						case "Contact":
						{
							Guid gCONTACT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spQUOTES_CONTACTS_Update(gQUOTE_ID, gCONTACT_ID, String.Empty);
							break;
						}
						case "Emails":
						{
							Guid gEMAIL_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spEMAILS_QUOTES_Update(gEMAIL_ID, gQUOTE_ID);
							break;
						}
						case "Opportunities":
						{
							Guid gOPPORTUNITY_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spQUOTES_OPPORTUNITIES_Update(gQUOTE_ID, gOPPORTUNITY_ID);
							break;
						}
						case "Project":
						{
							Guid gPROJECT_ID = Sql.ToGuid(sMODULE2_ID);
							SqlProcs.spPROJECT_RELATION_Update(gPROJECT_ID, sMODULE2, gQUOTE_ID);
							break;
						}
					}
					break;
				}
				default:
				{
					throw(new Exception(String.Format("Relationship between {0} and {1} is not defined", sMODULE1, sMODULE2)));
				}
			}
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public error_value set_relationship(string session, set_relationship_value set_relationship_value)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			
			// 02/16/2006 Paul.  Don't need to verify the modules as it will be done inside SetRelationship();
			//VerifyModuleName(set_relationship_value.module1);
			//VerifyModuleName(set_relationship_value.module2);
			int nACLACCESS = Security.GetUserAccess(set_relationship_value.module1, "edit");
			if ( nACLACCESS < 0 )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			error_value results = new error_value();
			SetRelationship(set_relationship_value.module1, set_relationship_value.module1_id, set_relationship_value.module2, set_relationship_value.module2_id);
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public set_relationship_list_result set_relationships(string session, set_relationship_value[] set_relationship_list)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			
			set_relationship_list_result results = new set_relationship_list_result();
			results.created = 0;
			results.failed  = 0;
			for ( int i=0; i < set_relationship_list.Length; i ++ )
			{
				int nACLACCESS = Security.GetUserAccess(set_relationship_list[i].module1, "edit");
				if ( nACLACCESS < 0 )
				{
					L10N L10n = new L10N("en-US");
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
				SetRelationship(set_relationship_list[i].module1, set_relationship_list[i].module1_id, set_relationship_list[i].module2, set_relationship_list[i].module2_id);
				results.created++ ;
			}
			return results;
		}

		[WebMethod(EnableSession=true)]
		[SoapRpcMethod]
		public set_entry_result set_document_revision(string session, document_revision note)
		{
			Guid gUSER_ID = GetSessionUserID(session);
			if ( gUSER_ID != Guid.Empty )
			{
				throw(new Exception("Method not implemented."));
			}
			return null;
		}
		#endregion
	}
}
