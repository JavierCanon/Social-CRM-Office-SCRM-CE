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


namespace ASC.Mail.Net.POP3.Client
{
    #region usings

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using System.Globalization;

    #endregion

    /// <summary>
    /// This class represents POP3 client message.
    /// </summary>
    public class POP3_ClientMessage
    {
        #region Members

        private readonly int m_SequenceNumber = 1;
        private readonly int m_Size;
        private bool m_IsDisposed;
        private bool m_IsMarkedForDeletion;
        private POP3_Client m_Pop3Client;

        #endregion

        #region Properties

        /// <summary>
        /// Gets if POP3 message is Disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return m_IsDisposed; }
        }

        /// <summary>
        /// Gets message 1 based sequence number.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int SequenceNumber
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_SequenceNumber;
            }
        }

        /// <summary>
        /// Gets message UID. NOTE: Before accessing this property, check that server supports UIDL command.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="NotSupportedException">Is raised when POP3 server doesnt support UIDL command.</exception>
        public string UIDL {get; set;}

        /// <summary>
        /// Gets message size in bytes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int Size
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_Size;
            }
        }

        /// <summary>
        /// Gets if message is marked for deletion.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsMarkedForDeletion
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_IsMarkedForDeletion;
            }
        }


        public string MD5 { get; set; }
        public bool IsNew { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="pop3">Owner POP3 client.</param>
        /// <param name="seqNumber">Message 1 based sequence number.</param>
        /// <param name="size">Message size in bytes.</param>
        internal POP3_ClientMessage(POP3_Client pop3, int seqNumber, int size)
        {
            m_Pop3Client = pop3;
            m_SequenceNumber = seqNumber;
            m_Size = size;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks message as deleted.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public void MarkForDeletion()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (IsMarkedForDeletion)
            {
                return;
            }
            m_IsMarkedForDeletion = true;

            m_Pop3Client.MarkMessageForDeletion(SequenceNumber);
        }

        /// <summary>
        /// Gets message header as string.
        /// </summary>
        /// <returns>Returns message header as string.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when message is marked for deletion and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public string HeaderToString()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            return Encoding.Default.GetString(HeaderToByte());
        }

        /// <summary>
        /// Gets message header as byte[] data.
        /// </summary>
        /// <returns>Returns message header as byte[] data.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when message is marked for deletion and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public byte[] HeaderToByte()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            MemoryStream retVal = new MemoryStream();
            MessageTopLinesToStream(retVal, 0);

            return retVal.ToArray();
        }

        /// <summary>
        /// Stores message header to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store data.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when argument <b>stream</b> value is null.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public void HeaderToStream(Stream stream)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (stream == null)
            {
                throw new ArgumentNullException("Argument 'stream' value can't be null.");
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            MessageTopLinesToStream(stream, 0);
        }

        /// <summary>
        /// Gets message as byte[] data.
        /// </summary>
        /// <returns>Returns message as byte[] data.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when message is marked for deletion and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public byte[] MessageToByte()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            MemoryStream retVal = new MemoryStream();
            MessageToStream(retVal);

            return retVal.ToArray();
        }

        /// <summary>
        /// Stores message to specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store message.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when argument <b>stream</b> value is null.</exception>
        /// <exception cref="InvalidOperationException">Is raised when message is marked for deletion and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public void MessageToStream(Stream stream)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (stream == null)
            {
                throw new ArgumentNullException("Argument 'stream' value can't be null.");
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            m_Pop3Client.GetMessage(SequenceNumber, stream);
        }

        /// <summary>
        /// Gets message header + specified number lines of message body.
        /// </summary>
        /// <param name="lineCount">Number of lines to get from message body.</param>
        /// <returns>Returns message header + specified number lines of message body.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when <b>numberOfLines</b> is negative value.</exception>
        /// <exception cref="InvalidOperationException">Is raised when message is marked for deletion and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public byte[] MessageTopLinesToByte(int lineCount)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (lineCount < 0)
            {
                throw new ArgumentException("Argument 'lineCount' value must be >= 0.");
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            MemoryStream retVal = new MemoryStream();
            MessageTopLinesToStream(retVal, lineCount);

            return retVal.ToArray();
        }

        /// <summary>
        /// Stores message header + specified number lines of message body to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store data.</param>
        /// <param name="lineCount">Number of lines to get from message body.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when argument <b>stream</b> value is null.</exception>
        /// <exception cref="InvalidOperationException">Is raised when message is marked for deletion and this method is accessed.</exception>
        /// <exception cref="POP3_ClientException">Is raised when POP3 serveer returns error.</exception>
        public void MessageTopLinesToStream(Stream stream, int lineCount)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (stream == null)
            {
                throw new ArgumentNullException("Argument 'stream' value can't be null.");
            }
            if (IsMarkedForDeletion)
            {
                throw new InvalidOperationException("Can't access message, it's marked for deletion.");
            }

            m_Pop3Client.GetTopOfMessage(SequenceNumber, stream, lineCount);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Disposes message.
        /// </summary>
        internal void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_IsDisposed = true;
            m_Pop3Client = null;
        }

        /// <summary>
        /// Sets IsMarkedForDeletion flag value.
        /// </summary>
        /// <param name="isMarkedForDeletion">New IsMarkedForDeletion value.</param>
        internal void SetMarkedForDeletion(bool isMarkedForDeletion)
        {
            m_IsMarkedForDeletion = isMarkedForDeletion;
        }


        public void CalculateMD5()
        {
            if (!string.IsNullOrEmpty(MD5)) return;
            MD5 hash = System.Security.Cryptography.MD5.Create();
            byte[] data = hash.ComputeHash(HeaderToByte());
            string result_md5 = "";
            for (int i = 0; i < data.Length; i++)
            {
                result_md5 += data[i].ToString("x2", CultureInfo.InvariantCulture);
            }
            MD5 = result_md5;
        }


        #endregion
    }
}