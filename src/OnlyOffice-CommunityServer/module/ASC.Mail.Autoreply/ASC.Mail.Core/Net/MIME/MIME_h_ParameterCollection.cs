/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


namespace ASC.Mail.Net.MIME
{
    #region usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web;

    #endregion

    /// <summary>
    /// Represents MIME header field parameters collection.
    /// </summary>
    public class MIME_h_ParameterCollection : IEnumerable
    {
        #region Members

        private readonly MIME_h m_pOwner;
        private readonly Dictionary<string, MIME_h_Parameter> m_pParameters;
        private bool m_IsModified;

        #endregion

        #region Properties

        /// <summary>
        /// Gets if this header field parameters are modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public bool IsModified
        {
            get
            {
                if (m_IsModified)
                {
                    return true;
                }
                else
                {
                    foreach (MIME_h_Parameter parameter in ToArray())
                    {
                        if (parameter.IsModified)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Gets owner MIME header field.
        /// </summary>
        public MIME_h Owner
        {
            get { return m_pOwner; }
        }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return m_pParameters.Count; }
        }

        /// <summary>
        /// Gets or sets specified header field parameter value. Value null means not specified.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <returns>Returns specified header field value or null if specified parameter doesn't exist.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
        public string this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                MIME_h_Parameter retVal = null;
                if (m_pParameters.TryGetValue(name, out retVal))
                {
                    return retVal.Value;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                MIME_h_Parameter retVal = null;
                if (m_pParameters.TryGetValue(name, out retVal))
                {
                    retVal.Value = value;
                }
                else
                {
                    m_pParameters.Add(name, new MIME_h_Parameter(name, value));
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner MIME header field.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>owner</b> is null reference.</exception>
        public MIME_h_ParameterCollection(MIME_h owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            m_pOwner = owner;

            m_pParameters = new Dictionary<string, MIME_h_Parameter>(StringComparer.CurrentCultureIgnoreCase);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes specified parametr from the collection.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
        public void Remove(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (m_pParameters.Remove(name))
            {
                m_IsModified = true;
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            m_pParameters.Clear();
            m_IsModified = true;
        }

        /// <summary>
        /// Copies header fields parameters to new array.
        /// </summary>
        /// <returns>Returns header fields parameters array.</returns>
        public MIME_h_Parameter[] ToArray()
        {
            MIME_h_Parameter[] retVal = new MIME_h_Parameter[m_pParameters.Count];
            m_pParameters.Values.CopyTo(retVal, 0);

            return retVal;
        }

        /// <summary>
        /// Returns header field parameters as string.
        /// </summary>
        /// <returns>Returns header field parameters as string.</returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Returns header field parameters as string.
        /// </summary>
        /// <param name="charset">Charset to use to encode 8-bit characters. Value null means parameters not encoded.</param>
        /// <returns>Returns header field parameters as string.</returns>
        public string ToString(Encoding charset)
        {
            /* RFC 2231.
             *      If parameter conatins 8-bit byte, we need to encode parameter value
             *      If parameter value length bigger than MIME maximum allowed line length,
             *      we need split value.
            */

            if (charset == null)
            {
                charset = Encoding.Default;
            }

            StringBuilder retVal = new StringBuilder();
            foreach (MIME_h_Parameter parameter in ToArray())
            {
                if (string.IsNullOrEmpty(parameter.Value))
                {
                    retVal.Append(";\r\n\t" + parameter.Name);
                }
                    // We don't need to encode or split value.
                else if ((charset == null || Core.IsAscii(parameter.Value)) && parameter.Value.Length < 76)
                {
                    retVal.Append(";\r\n\t" + parameter.Name + "=" + TextUtils.QuoteString(parameter.Value));
                }
                    // We need to encode/split value.
                else
                {
                    byte[] byteValue = charset.GetBytes(parameter.Value);

                    List<string> values = new List<string>();
                    // Do encoding/splitting.
                    int offset = 0;
                    char[] valueBuff = new char[50];
                    foreach (byte b in byteValue)
                    {
                        // We need split value as RFC 2231 says.
                        if (offset >= (50 - 3))
                        {
                            values.Add(new string(valueBuff, 0, offset));
                            offset = 0;
                        }

                        // Normal char, we don't need to encode.
                        if (MIME_Reader.IsAttributeChar((char) b))
                        {
                            valueBuff[offset++] = (char) b;
                        }
                            // We need to encode byte as %X2.
                        else
                        {
                            valueBuff[offset++] = '%';
                            valueBuff[offset++] = (b >> 4).ToString("X")[0];
                            valueBuff[offset++] = (b & 0xF).ToString("X")[0];
                        }
                    }
                    // Add pending buffer value.
                    if (offset > 0)
                    {
                        values.Add(new string(valueBuff, 0, offset));
                    }

                    for (int i = 0; i < values.Count; i++)
                    {
                        // Only fist value entry has charset and language info.
                        if (charset != null && i == 0)
                        {
                            retVal.Append(";\r\n\t" + parameter.Name + "*" + i + "*=" + charset.WebName + "''" +
                                          values[i]);
                        }
                        else
                        {
                            retVal.Append(";\r\n\t" + parameter.Name + "*" + i + "*=" + values[i]);
                        }
                    }
                }
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Parses parameters from the specified value.
        /// </summary>
        /// <param name="value">Header field parameters string.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Parse(new MIME_Reader(value));
        }

        /// <summary>
        /// Parses parameters from the specified reader.
        /// </summary>
        /// <param name="reader">MIME reader.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>reader</b> is null reference.</exception>
        public void Parse(MIME_Reader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            /* RFC 2231.
            */

            while (true)
            {
                // End os stream reached.
                if (reader.Peek(true) == -1)
                {
                    break;
                }
                    // Next parameter start, just eat that char.
                else if (reader.Peek(true) == ';')
                {
                    reader.Char(false);
                }
                else
                {
                    string name = reader.Token();

                    if (name == null)
                        break;

                    string value = "";
                    // Parameter value specified.
                    if (reader.Peek(true) == '=')
                    {
                        reader.Char(false);

                        string v = reader.Word();
                        // Normally value may not be null, but following case: paramName=EOS.
                        if (v != null)
                        {
                            value = v;
                        }
                    }

                    // RFC 2231 encoded/splitted parameter.
                    if (name.IndexOf('*') > -1)
                    {
                        string[] name_x_no_x = name.Split('*');
                        name = name_x_no_x[0];

                        Encoding charset = Encoding.ASCII;
                        StringBuilder valueBuffer = new StringBuilder();
                        // We must have charset'language'value.
                        // Examples:
                        //      URL*=utf-8''test;
                        //      URL*0*=utf-8''"test";
                        if ((name_x_no_x.Length == 2 && name_x_no_x[1] == "") || name_x_no_x.Length == 3)
                        {
                            string[] charset_language_value = value.Split('\'');
                            charset = EncodingTools.GetEncodingByCodepageName(charset_language_value[0]) ?? Encoding.ASCII;
                            valueBuffer.Append(charset_language_value[2]);
                        }
                            // No encoding, probably just splitted ASCII value.
                            // Example:
                            //     URL*0="value1";
                            //     URL*1="value2";
                        else
                        {
                            valueBuffer.Append(value);
                        }

                        // Read while value continues.
                        while (true)
                        {
                            // End os stream reached.
                            if (reader.Peek(true) == -1)
                            {
                                break;
                            }
                                // Next parameter start, just eat that char.
                            else if (reader.Peek(true) == ';')
                            {
                                reader.Char(false);
                            }
                            else
                            {
                                if (!reader.StartsWith(name + "*"))
                                {
                                    break;
                                }
                                reader.Token();

                                // Parameter value specified.
                                if (reader.Peek(true) == '=')
                                {
                                    reader.Char(false);

                                    string v = reader.Word();
                                    // Normally value may not be null, but following case: paramName=EOS.
                                    if (v != null)
                                    {
                                        valueBuffer.Append(v);
                                    }
                                }
                            }
                        }

                        this[name] = DecodeExtOctet(valueBuffer.ToString(),charset);
                    }
                        // Regular parameter.
                    else
                    {
                        this[name] = value;
                    }
                }
            }

            m_IsModified = false;
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return m_pParameters.GetEnumerator();
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Decodes non-ascii text with MIME <b>ext-octet</b> method. Defined in RFC 2231 7.
        /// </summary>
        /// <param name="text">Text to decode,</param>
        /// <param name="charset">Charset to use.</param>
        /// <returns>Returns decoded text.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> or <b>charset</b> is null.</exception>
        private static string DecodeExtOctet(string text, Encoding charset)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (charset == null)
            {
                throw new ArgumentNullException("charset");
            }

            return HttpUtility.UrlDecode(text, charset);
			/*
            int offset = 0;
            byte[] decodedBuffer = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '%')
                {
                    decodedBuffer[offset++] = byte.Parse(text[i + 1] + text[i + 2].ToString(),
                                                         NumberStyles.HexNumber);
                    i += 2;
                }
                else
                {
                    decodedBuffer[offset++] = (byte) text[i];
                }
            }

            return charset.GetString(decodedBuffer, 0, offset);*/
        }

        #endregion
    }
}