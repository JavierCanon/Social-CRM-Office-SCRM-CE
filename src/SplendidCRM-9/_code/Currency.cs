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
using System.Web;
using Microsoft.Win32;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for Currency.
	/// </summary>
	public class Currency
	{
		protected Guid   m_gID             ;
		protected string m_sNAME           ;
		protected string m_sSYMBOL         ;
		protected float  m_fCONVERSION_RATE;
		protected bool   m_bUSDollars      ;
		
		protected static Guid m_gUSDollar  = new Guid("E340202E-6291-4071-B327-A34CB4DF239B");
		
		public Guid ID
		{
			get
			{
				return m_gID;
			}
		}

		public string NAME
		{
			get
			{
				return m_sNAME;
			}
		}

		public string SYMBOL
		{
			get
			{
				return m_sSYMBOL;
			}
		}

		public float CONVERSION_RATE
		{
			get
			{
				return m_fCONVERSION_RATE;
			}
		}

		public static Currency CreateCurrency(Guid gCURRENCY_ID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			Currency C10n = Application["CURRENCY." + gCURRENCY_ID.ToString()] as SplendidCRM.Currency;
			if ( C10n == null )
			{
				// 05/09/2006 Paul. First try and use the default from CONFIG. 
				gCURRENCY_ID = Sql.ToGuid(Application["CONFIG.default_currency"]);
				C10n = Application["CURRENCY." + gCURRENCY_ID.ToString()] as SplendidCRM.Currency;
				if ( C10n == null )
				{
					// Default to USD if default not specified. 
					gCURRENCY_ID = m_gUSDollar;
					C10n = Application["CURRENCY." + gCURRENCY_ID.ToString()] as SplendidCRM.Currency;
				}
				// If currency is still null, then create a blank zone. 
				if ( C10n == null )
				{
					C10n = new Currency();
					Application["CURRENCY." + gCURRENCY_ID.ToString()] = C10n;
				}
			}
			return C10n;
		}

		public static Currency CreateCurrency(Guid gCURRENCY_ID, float fCONVERSION_RATE)
		{
			Currency C10n = CreateCurrency(gCURRENCY_ID);
			// 03/31/2007 Paul.  Create a new currency object so that we can override the rate 
			// without overriding the global value. 
			if ( fCONVERSION_RATE == 0.0 )
				fCONVERSION_RATE = 1.0F;
			return new Currency(C10n.ID, C10n.NAME, C10n.SYMBOL, fCONVERSION_RATE);
		}

		public Currency()
		{
			m_gID              = m_gUSDollar;
			m_sNAME            = "U.S. Dollar";
			m_sSYMBOL          = "USD";
			m_fCONVERSION_RATE = 1.0f;
			m_bUSDollars       = true;
		}
		
		public Currency
			( Guid   gID             
			, string sNAME           
			, string sSYMBOL         
			, float  fCONVERSION_RATE
			)
		{
			m_gID              = gID             ;
			m_sNAME            = sNAME           ;
			m_sSYMBOL          = sSYMBOL         ;
			m_fCONVERSION_RATE = fCONVERSION_RATE;
			m_bUSDollars       = (m_gID == m_gUSDollar);
		}

		public float ToCurrency(float f)
		{
			// 05/10/2006 Paul.  Short-circuit the math if USD. 
			// This is more to prevent bugs than to speed calculations. 
			if ( m_bUSDollars )
				return f;
			return f * m_fCONVERSION_RATE;
		}

		public float FromCurrency(float f)
		{
			// 05/10/2006 Paul.  Short-circuit the math if USD. 
			// This is more to prevent bugs than to speed calculations. 
			if ( m_bUSDollars )
				return f;
			return f / m_fCONVERSION_RATE;
		}

		// 03/30/2007 Paul.  Decimal is the main format for currencies. 
		public Decimal ToCurrency(Decimal d)
		{
			if ( m_bUSDollars )
				return d;
			return Convert.ToDecimal(Convert.ToDouble(d) * m_fCONVERSION_RATE);
		}

		public Decimal FromCurrency(Decimal d)
		{
			// 05/10/2006 Paul.  Short-circuit the math if USD. 
			// This is more to prevent bugs than to speed calculations. 
			// 04/18/2007 Paul.  Protect against divide by zero. 
			if ( m_bUSDollars || m_fCONVERSION_RATE == 0.0 )
				return d;
			return Convert.ToDecimal(Convert.ToDouble(d) / m_fCONVERSION_RATE);
		}
	}
}
