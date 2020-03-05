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
using System.Web;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for XmlUtil.
	/// </summary>
	public class XmlUtil
	{
		public static DataTable CreateDataTable(XmlNode parent, string sTableName, string sPrimaryKey, string[] asColumns)
		{
			DataTable dt = new DataTable(sTableName);
			dt.Columns.Add(sPrimaryKey);
			foreach(string sColumn in asColumns)
			{
				dt.Columns.Add(sColumn);
			}
			if ( parent != null )
			{
				XmlNodeList nl = parent.SelectNodes(sTableName);
				if ( nl != null )
				{
					foreach(XmlNode node in nl)
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						try
						{
							row[sPrimaryKey] = node.Attributes.GetNamedItem(sPrimaryKey).Value;
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
						foreach(string sColumn in asColumns)
						{
							row[sColumn] = XmlUtil.SelectSingleNode(node, sColumn);
						}
					}
				}
			}
			return dt;
		}

		public static DataTable CreateDataTable(XmlNode parent, string sTableName, string[] asColumns)
		{
			DataTable dt = new DataTable(sTableName);
			foreach(string sColumn in asColumns)
			{
				dt.Columns.Add(sColumn);
			}
			if ( parent != null )
			{
				XmlNodeList nl = parent.SelectNodes(sTableName);
				if ( nl != null )
				{
					foreach(XmlNode node in nl)
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						foreach(string sColumn in asColumns)
						{
							row[sColumn] = XmlUtil.SelectSingleNode(node, sColumn);
						}
					}
				}
			}
			return dt;
		}

		public static void RemoveAllChildren(XmlDocument xml, string sNode)
		{
			try
			{
				XmlNode node   = null;
				XmlNode parent = xml.DocumentElement;
				string[] aNode = sNode.Split('/');
				foreach ( string sNodePart in aNode )
				{
					node = parent.SelectSingleNode(sNodePart);
					if ( node == null )
					{
						return ;
					}
					parent = node;
				}
				if ( node != null )
				{
					node.RemoveAll();
				}
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static string SelectSingleNode(XmlDocument xml, string sNode)
		{
			try
			{
				if ( xml.DocumentElement != null )
				{
					XmlNode node = xml.DocumentElement.SelectSingleNode(sNode);
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public static string GetNamedItem(XmlNode xNode, string sAttribute)
		{
			string sValue = String.Empty;
			XmlNode xValue = xNode.Attributes.GetNamedItem(sAttribute);
			if ( xValue != null )
				sValue = xValue.Value;
			return sValue;
		}

		public static string SelectSingleNode(XmlDocument xml, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				if ( xml.DocumentElement != null )
				{
					// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
					if ( sNode.IndexOf(':') < 0 )
						sNode = "defaultns:" + sNode;
					XmlNode node = xml.DocumentElement.SelectSingleNode(sNode, nsmgr);
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public static string SelectSingleNode(XmlDocument xml, string sNode, string sDefault)
		{
			try
			{
				if ( xml.DocumentElement != null )
				{
					XmlNode node = xml.DocumentElement.SelectSingleNode(sNode);
					if ( node != null )
					{
						if ( !Sql.IsEmptyString(node.InnerText) )
							return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return sDefault;
		}

		public static string SelectSingleNode(XmlNode node, string sNode)
		{
			try
			{
				if ( node != null )
				{
					node = node.SelectSingleNode(sNode);
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public static string SelectSingleNode(XmlNode parent, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				if ( parent != null )
				{
					XmlNode node = null;
					// 10/24/2007 Paul.  We need to support multiple tags. 
					string[] aNode = sNode.Split('/');
					int i = 0;
					for ( i=0; i < aNode.Length; i++ )
					{
						string sNodeNS = aNode[i];
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							return null;
						}
						parent = node;
					}
					if ( node != null )
					{
						return node.InnerText;
					}
				}
			}
			catch(Exception /* ex */)
			{
			}
			return String.Empty;
		}

		public static XmlNode SelectNode(XmlDocument xml, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					int i = 0;
					for ( i=0; i < aNode.Length; i++ )
					{
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						string sNodeNS = aNode[i];
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							return null;
						}
						parent = node;
					}
					if ( i == aNode.Length )
						return parent;
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return null;
		}

		public static XmlNode SelectNode(XmlNode parent, string sNode, XmlNamespaceManager nsmgr)
		{
			try
			{
				XmlNode node = null;
				string[] aNode = sNode.Split('/');
				int i = 0;
				for ( i=0; i < aNode.Length; i++ )
				{
					// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
					string sNodeNS = aNode[i];
					if ( sNodeNS.IndexOf(':') < 0 )
						sNodeNS = "defaultns:" + sNodeNS;
					node = parent.SelectSingleNode(sNodeNS, nsmgr);
					if ( node == null )
					{
						return null;
					}
					parent = node;
				}
				if ( i == aNode.Length )
					return node;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return null;
		}

		public static void SetSingleNode(XmlDocument xml, string sNode, string sValue)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						node = parent.SelectSingleNode(sNodePart);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public static void SetSingleNode(XmlDocument xml, string sNode, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		public static void SetSingleNodeAttribute(XmlDocument xml, string sNode, string sAttribute, string sValue)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						node = parent.SelectSingleNode(sNodePart);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				node.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void SetSingleNodeAttribute(XmlDocument xml, string sNode, string sAttribute, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					XmlNode parent = xml.DocumentElement;
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				node.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void SetSingleNodeAttribute(XmlDocument xml, XmlNode parent, string sAttribute, string sValue)
		{
			try
			{
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				parent.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void SetSingleNodeAttribute(XmlDocument xml, XmlNode parent, string sAttribute, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlAttribute attr = xml.CreateAttribute(sAttribute);
				attr.Value = sValue;
				parent.Attributes.SetNamedItem(attr);
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void SetSingleNode(XmlDocument xml, XmlNode parent, string sNode, string sValue)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode);
				if ( node == null )
				{
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						node = parent.SelectSingleNode(sNodePart);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void SetSingleNode(XmlDocument xml, XmlNode parent, string sNode, string sValue, XmlNamespaceManager nsmgr, string sNamespaceURI)
		{
			try
			{
				XmlNode node = xml.SelectSingleNode(sNode, nsmgr);
				if ( node == null )
				{
					string[] aNode = sNode.Split('/');
					foreach ( string sNodePart in aNode )
					{
						string sNodeNS = sNodePart;
						// 06/20/2006 Paul.  The default namespace cannot be selected, so create an alias and reference the alias. 
						if ( sNodeNS.IndexOf(':') < 0 )
							sNodeNS = "defaultns:" + sNodeNS;
						node = parent.SelectSingleNode(sNodeNS, nsmgr);
						if ( node == null )
						{
							node = xml.CreateElement(sNodePart, sNamespaceURI);
							parent.AppendChild(node);
						}
						parent = node;
					}
				}
				node.InnerText = sValue;
			}
			catch(Exception /* ex */)
			{
			}
		}

		public static void Dump(ref StringBuilder sb, string sIndent, XmlNode parent)
		{
			sb.Append(sIndent + "<" + parent.Name);
			if ( parent.Attributes != null )
			{
				foreach ( XmlAttribute attr in parent.Attributes )
				{
					sb.Append(" "  + attr.Name  + "=");
					sb.Append("\"" + attr.Value + "\""); // TODO: encode the value.  
				}
			}
			if ( parent.HasChildNodes )
			{
				if ( parent.ChildNodes.Count == 1 && parent.ChildNodes[0].NodeType == XmlNodeType.Text )
				{
					XmlNode child = parent.ChildNodes[0];
					if ( child.Value != String.Empty )
					{
						// 10/12/2006 Paul.  Reduce the XML dump. 
						if ( child.Value.IndexOf(' ') > 0 )
						{
							sb.Append(">" + ControlChars.CrLf);
							sb.Append(sIndent + "    " + child.Value + ControlChars.CrLf);
							sb.Append(sIndent + "</" + parent.Name + ">" + ControlChars.CrLf);
						}
						else
						{
							sb.Append(">" + child.Value + "</" + parent.Name + ">" + ControlChars.CrLf);
						}
					}
					else
					{
						sb.Append(" />" + ControlChars.CrLf);
					}
				}
				else
				{
					sb.Append(">" + ControlChars.CrLf);
					foreach ( XmlNode child in parent.ChildNodes )
					{
						if ( child.NodeType == XmlNodeType.Text )
						{
							if ( child.Value != String.Empty )
							{
								sb.Append(sIndent + "    " + child.Value + ControlChars.CrLf);
							}
						}
						else
						{
							Dump(ref sb, sIndent + "    ", child);
						}
					}
					sb.Append(sIndent + "</" + parent.Name + ">" + ControlChars.CrLf);
				}
			}
			else
			{
				sb.Append(" />" + ControlChars.CrLf);
			}
		}

		public static void Dump(XmlDocument xml)
		{
			StringBuilder sb = new StringBuilder();
			if ( xml != null && xml.DocumentElement != null)
				Dump(ref sb, "", xml.DocumentElement);
			string sDump = HttpContext.Current.Server.HtmlEncode(sb.ToString());
			HttpContext.Current.Response.Write("<pre><font face='courier new'>");
			HttpContext.Current.Response.Write(sDump);
			HttpContext.Current.Response.Write("</font></pre>");
		}

		public static string BaseTypeXPath(object o)
		{
			return o.GetType().BaseType.ToString().Replace(".", "/");
		}

		private static string PHPString(MemoryStream mem)
		{
			string sSize   = String.Empty;
			string sString = String.Empty;
			int nMode = 0;
			int nChar = mem.ReadByte();
			while ( nChar != -1 )
			{
				char ch = Convert.ToChar(nChar);
				switch ( nMode )
				{
					case 0:  // Looking for ':'
						if ( ch == ':' )
							nMode = 1;
						break;
					case 1:  // Looking for a number
						if ( Char.IsDigit(ch) )
							sSize += ch;
						else if ( ch == ':' )
							nMode = 2;
						break;
					case 2: // Read string
					{
						int nSize = Int32.Parse(sSize);
						for ( int i = 0 ; i < (nSize+2) && nChar != -1 ; i++ )
						{
							if ( !(ch == '\"' && (i == 0 || i == nSize + 1)) )
								sString += ch;
							nChar = mem.ReadByte();
							if ( nChar != -1 )
								ch = Convert.ToChar(nChar);
						}
						if ( nChar != -1 && ch == ';' )
							return sString;
						nMode = 3;
						break;
					}
					case 3: // Expecting ';'
						if ( ch == ';' )
							return sString;
						break;
				}
				nChar = mem.ReadByte();
			}
			return sString;
		}

		private static string PHPInteger(MemoryStream mem)
		{
			string sNumber = String.Empty;
			int nMode = 0;
			int nChar = mem.ReadByte();
			while ( nChar != -1 )
			{
				char ch = Convert.ToChar(nChar);
				switch ( nMode )
				{
					case 0:  // Looking for ':'
						if ( ch == ':' )
							nMode = 1;
						break;
					case 1:  // Looking for a number
						if ( Char.IsDigit(ch) )
							sNumber += ch;
						else if ( ch == ';' )
						{
							return sNumber;
						}
						break;
				}
				nChar = mem.ReadByte();
			}
			return sNumber;
		}

		private static void PHPArray(XmlDocument xml, XmlElement parent, MemoryStream mem)
		{
			string sSize = String.Empty;
			string sNAME  = String.Empty;
			string sVALUE = String.Empty;
			int nChar = mem.ReadByte();
			// Skip past size and get to the begging of the array. 
			while ( nChar != -1 && Convert.ToChar(nChar) != '{' )
			{
				nChar = mem.ReadByte();
			}
			if ( nChar == -1 )
				return ;

			int nMode = 0;
			nChar = mem.ReadByte();
			while ( nChar != -1 )
			{
				char ch = Convert.ToChar(nChar);
				switch ( nMode )
				{
					case 0:  // Looking for "s" at the start of the variable. 
						if ( ch == 's' )
						{
							sNAME = PHPString(mem);
							nMode = 1;
						}
						else if ( ch == 'i' )
						{
							sNAME = PHPInteger(mem);
							XmlAttribute attr = xml.CreateAttribute("index_array");
							attr.Value = "true";
							parent.Attributes.SetNamedItem(attr);
							nMode = 2;
						}
						else if ( ch == '}' )
						{
							// End of the array was reached. 
							return;
						}
						break;
					case 1: // Read variable data type
						if ( ch == 's' )
						{
							sVALUE = PHPString(mem);
							XmlUtil.SetSingleNode(xml, parent, sNAME, sVALUE);
							nMode = 0;
						}
						else if ( ch == 'i' )
						{
							sVALUE = PHPInteger(mem);
							XmlUtil.SetSingleNode(xml, parent, sNAME, sVALUE);
							nMode = 0;
						}
						else if ( ch == 'a' )
						{
							XmlElement node = xml.CreateElement(sNAME);
							parent.AppendChild(node);
							PHPArray(xml, node, mem);
							nMode = 0;
						}
						break;
					case 2: // Index array values. 
						if ( ch == 's' )
						{
							sVALUE = PHPString(mem);
							XmlUtil.SetSingleNode(xml, parent, "index_" + sNAME, sVALUE);
							nMode = 0;
						}
						else if ( ch == 'i' )
						{
							sVALUE = PHPInteger(mem);
							XmlUtil.SetSingleNode(xml, parent, "index_" + sNAME, sVALUE);
							nMode = 0;
						}
						break;
				}
				nChar = mem.ReadByte();
			}
		}

		public static string ConvertFromPHP(string sPHP)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
			xml.AppendChild(xml.CreateElement("USER_PREFERENCE"));
			try
			{
				byte[] abyPHP = Convert.FromBase64String(sPHP);
				StringBuilder sb = new StringBuilder();
				foreach(char by in abyPHP)
					sb.Append(by);
				MemoryStream mem = new MemoryStream(abyPHP);

				string sSize = String.Empty;
				int nChar = mem.ReadByte();
				while ( nChar != -1 )
				{
					char ch = Convert.ToChar(nChar);
					if ( ch == 'a' )
						PHPArray(xml, xml.DocumentElement, mem);
					else if ( ch == 's' )
						PHPString(mem);
					else if ( ch == 'i' )
						PHPInteger(mem);
					nChar = mem.ReadByte();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return xml.OuterXml;
		}

		public static string ConvertToPHP(XmlElement parent)
		{
			StringBuilder sb = new StringBuilder();
			if ( parent.ChildNodes.Count > 1 )
			{
				sb.Append("s:" + parent.Name.Length.ToString() + ":\"" + parent.Name + "\";");
				sb.Append("a:" + parent.ChildNodes.Count.ToString() + "{");
				if ( Sql.ToBoolean(parent.GetAttribute("index_array")) )
				{
					int i = 0;
					foreach(XmlElement node in parent.ChildNodes)
					{
						sb.Append("i:" + i.ToString() + ";");
						sb.Append("s:" + node.InnerText.Length.ToString() + ":\"" + node.InnerText + "\";");
						i++;
					}
				}
				else
				{
					foreach(XmlElement node in parent.ChildNodes)
					{
						sb.Append(ConvertToPHP(node));
					}
				}
				sb.Append("}");
			}
			else
			{
				sb.Append("s:" + parent.Name.Length.ToString() + ":\"" + parent.Name + "\";");
				sb.Append("s:" + parent.InnerText.Length.ToString() + ":\"" + parent.InnerText + "\";");
			}
			return ToBase64String(sb.ToString());
		}

		public static string ToBase64String(string s)
		{
			byte[] aby = UTF8Encoding.UTF8.GetBytes(s);
			return Convert.ToBase64String(aby);
		}

		public static string FromBase64String(string s)
		{
			byte[] aby = Convert.FromBase64String(s);
			return UTF8Encoding.UTF8.GetString(aby);
		}
	}
}
