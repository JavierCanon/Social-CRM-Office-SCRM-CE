using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for vcal_server.
	/// </summary>
	public class vcal_server : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 12/29/2005 Paul.  vCalendar support is not going to be easy.
			// Outlook will automatically use FrontPage extensions to place the file. 
			// When connecting to a Apache server, it will make HTTP GET/PUT requests. 
			SplendidError.SystemError(new StackTrace(true).GetFrame(0), "");
			Response.Write("");
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
