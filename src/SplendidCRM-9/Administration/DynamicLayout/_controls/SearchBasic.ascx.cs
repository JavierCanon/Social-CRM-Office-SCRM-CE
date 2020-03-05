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
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.DynamicLayout._controls
{
	/// <summary>
	///		Summary description for SearchBasic.
	/// </summary>
	public class SearchBasic : SearchControl
	{
		protected DropDownList lstLAYOUT_VIEWS;
		protected string sViewTableName;
		protected string sViewFieldName;

		public string NAME
		{
			get
			{
				return lstLAYOUT_VIEWS.SelectedValue;
			}
		}

		public string ViewTableName
		{
			get { return sViewTableName; }
			set { sViewTableName = value; }
		}

		public string ViewFieldName
		{
			get { return sViewFieldName; }
			set { sViewFieldName = value; }
		}

		public override void SqlSearchClause(IDbCommand cmd)
		{
			Sql.AppendParameter(cmd, lstLAYOUT_VIEWS, sViewFieldName);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 01/06/2006 Paul.  Try disabling viewstate of DetailView to prevent viewstate error. 
			if ( !this.IsPostBack || !Parent.EnableViewState )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                   " + ControlChars.CrLf
					     + "  from " + sViewTableName    + ControlChars.CrLf
					     + " order by DISPLAY_NAME     " + ControlChars.CrLf;
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
									lstLAYOUT_VIEWS.DataSource = dt;
									lstLAYOUT_VIEWS.DataBind();
									lstLAYOUT_VIEWS.Items.Insert(0, String.Empty);

									// 01/08/2006 Paul.  The viewstate is no longer disabled, so this is not necessary. 
									/*
									try
									{
										// 01/06/2006 Paul.  If viewstate has been disabled, then recall the submitted value. 
										if ( !Parent.EnableViewState )
										{
											string sNAME = Sql.ToString(Request[ListUniqueID]);
											lstLAYOUT_VIEWS.SelectedValue = sNAME;
										}
									}
									catch
									{
									}
									*/
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
