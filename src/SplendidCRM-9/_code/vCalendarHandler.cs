using System;
using System.Web;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for vCalendarHandler.
	/// </summary>
	public class vCalendarHandler : IHttpHandler
	{
		public vCalendarHandler()
		{
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			SplendidError.SystemError(new StackTrace(true).GetFrame(0), context.Request.Path);
		}
	}
}
