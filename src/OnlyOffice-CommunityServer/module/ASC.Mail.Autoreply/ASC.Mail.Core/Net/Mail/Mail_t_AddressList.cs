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


namespace ASC.Mail.Net.Mail
{
    #region usings

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using MIME;

    #endregion

    /// <summary>
    /// This class represents <b>address-list</b>. Defined in RFC 5322 3.4.
    /// </summary>
    public class Mail_t_AddressList : IEnumerable
    {
        #region Members

        private readonly List<Mail_t_Address> m_pList;
        private bool m_IsModified;
        private static System.Text.RegularExpressions.Regex m_regParser = new System.Text.RegularExpressions.Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");//"^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$");

        #endregion

        #region Properties

        /// <summary>
        /// Gets if list has modified since it was loaded.
        /// </summary>
        public bool IsModified
        {
            get { return m_IsModified; }
        }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return m_pList.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>Returns the element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when <b>index</b> is out of range.</exception>
        public Mail_t_Address this[int index]
        {
            get
            {
                if (index < 0 || index >= m_pList.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return m_pList[index];
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Mail_t_AddressList()
        {
            m_pList = new List<Mail_t_Address>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a address into the collection at the specified location.
        /// </summary>
        /// <param name="index">The location in the collection where you want to add the item.</param>
        /// <param name="value">Address to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when <b>index</b> is out of range.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public void Insert(int index, Mail_t_Address value)
        {
            if (index < 0 || index > m_pList.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_pList.Insert(index, value);
            m_IsModified = true;
        }

        /// <summary>
        /// Adds specified address to the end of the collection.
        /// </summary>
        /// <param name="value">Address to add.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference value.</exception>
        public void Add(Mail_t_Address value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_pList.Add(value);
            m_IsModified = true;
        }

        /// <summary>
        /// Removes specified item from the collection.
        /// </summary>
        /// <param name="value">Address to remove.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference value.</exception>
        public void Remove(Mail_t_Address value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            m_pList.Remove(value);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            m_pList.Clear();
            m_IsModified = true;
        }

        /// <summary>
        /// Copies addresses to new array.
        /// </summary>
        /// <returns>Returns addresses array.</returns>
        public Mail_t_Address[] ToArray()
        {
            return m_pList.ToArray();
        }

        /// <summary>
        /// Returns address-list as string.
        /// </summary>
        /// <returns>Returns address-list as string.</returns>
        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            for (int i = 0; i < m_pList.Count; i++)
            {
                if (i == (m_pList.Count - 1))
                {
                    retVal.Append(m_pList[i].ToString());
                }
                else
                {
                    retVal.Append(m_pList[i] + ",");
                }
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return m_pList.GetEnumerator();
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Resets IsModified property to false.
        /// </summary>
        internal void AcceptChanges()
        {
            m_IsModified = false;
        }

        #endregion

        public static Mail_t_AddressList ParseAddressList(string value)
        {
            MIME_Reader r = new MIME_Reader(value);

            /* RFC 5322 3.4.
                address         =   mailbox / group

                mailbox         =   name-addr / addr-spec

                name-addr       =   [display-name] angle-addr

                angle-addr      =   [CFWS] "<" addr-spec ">" [CFWS] / obs-angle-addr

                group           =   display-name ":" [group-list] ";" [CFWS]

                display-name    =   phrase

                mailbox-list    =   (mailbox *("," mailbox)) / obs-mbox-list

                address-list    =   (address *("," address)) / obs-addr-list

                group-list      =   mailbox-list / CFWS / obs-group-list
            */

            Mail_t_AddressList retVal = new Mail_t_AddressList();

            while (true)
            {
                string word = r.QuotedReadToDelimiter(new[] { ',', '<', ':' });
                // We processed all data.
                if (word == null && r.Available == 0)
                {
                    if (retVal.Count == 0)
                    {
                        if (CheckEmail(value))
                        {
                            retVal.Add(new Mail_t_Mailbox(null, value));
                        }
                    }
                    
                    break;
                }
                // skip old group address format
                else if (r.Peek(true) == ':')
                {
                    // Consume ':'
                    r.Char(true);
                }
                // name-addr
                else if (r.Peek(true) == '<')
                {
                    string address = r.ReadParenthesized();

                    if (CheckEmail(address))
                    {
                        retVal.Add(
                            new Mail_t_Mailbox(
                                word != null
                                    ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(word))
                                    : null,
                                address));
                    }
                }
                // addr-spec
                else
                {
                    if (CheckEmail(word))
                    {
                        retVal.Add(new Mail_t_Mailbox(null, word));
                    }
                }

                // We have more addresses.
                if (r.Peek(true) == ',')
                {
                    r.Char(false);
                }
            }

            return retVal;
        }

        private static bool CheckEmail(string EmailAddress)
        {
            return !string.IsNullOrEmpty(EmailAddress) ? m_regParser.IsMatch(EmailAddress.Trim()) : false;
        }

    }
}