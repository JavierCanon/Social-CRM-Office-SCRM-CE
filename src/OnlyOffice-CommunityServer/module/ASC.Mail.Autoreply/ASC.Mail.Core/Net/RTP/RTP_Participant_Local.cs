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


namespace ASC.Mail.Net.RTP
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class represents RTP session/multimedia-session local participant.
    /// </summary>
    /// <remarks>Term <b>participant</b> is not well commented/defined in RTP. In general for single media session <b>participant</b>
    /// is RTP session itself, for multimedia sesssion <b>participant</b> is multimedia session(RTP sessions group).</remarks>
    public class RTP_Participant_Local : RTP_Participant
    {
        #region Members

        private readonly CircleCollection<string> m_pOtionalItemsRoundRobin;
        private string m_Email;
        private string m_Location;
        private string m_Name;
        private string m_Note;
        private string m_Phone;
        private string m_Tool;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the real name, eg. "John Doe". Value null means not specified.
        /// </summary>
        public string Name
        {
            get { return m_Name; }

            set
            {
                m_Name = value;

                ConstructOptionalItems();
            }
        }

        /// <summary>
        /// Gets or sets email address. For example "John.Doe@example.com". Value null means not specified.
        /// </summary>
        public string Email
        {
            get { return m_Email; }

            set
            {
                m_Email = value;

                ConstructOptionalItems();
            }
        }

        /// <summary>
        /// Gets or sets phone number. For example "+1 908 555 1212". Value null means not specified.
        /// </summary>
        public string Phone
        {
            get { return m_Phone; }

            set
            {
                m_Phone = value;

                ConstructOptionalItems();
            }
        }

        /// <summary>
        /// Gets  or sets location string. It may be geographic address or for example chat room name.
        /// Value null means not specified.
        /// </summary>
        public string Location
        {
            get { return m_Location; }

            set
            {
                m_Location = value;

                ConstructOptionalItems();
            }
        }

        /// <summary>
        /// Gets or sets streaming application name/version.
        /// Value null means not specified.
        /// </summary>
        public string Tool
        {
            get { return m_Tool; }

            set
            {
                m_Tool = value;

                ConstructOptionalItems();
            }
        }

        /// <summary>
        /// Gets or sets note text. The NOTE item is intended for transient messages describing the current state
        /// of the source, e.g., "on the phone, can't talk". Value null means not specified.
        /// </summary>
        public string Note
        {
            get { return m_Note; }

            set
            {
                m_Note = value;

                ConstructOptionalItems();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cname">Canonical name of participant.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cname</b> is null reference.</exception>
        public RTP_Participant_Local(string cname) : base(cname)
        {
            m_pOtionalItemsRoundRobin = new CircleCollection<string>();
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Constructs optional SDES items round-robin.
        /// </summary>
        private void ConstructOptionalItems()
        {
            lock (m_pOtionalItemsRoundRobin)
            {
                m_pOtionalItemsRoundRobin.Clear();

                if (!string.IsNullOrEmpty(m_Note))
                {
                    m_pOtionalItemsRoundRobin.Add("note");
                }
                if (!string.IsNullOrEmpty(m_Name))
                {
                    m_pOtionalItemsRoundRobin.Add("name");
                }
                if (!string.IsNullOrEmpty(m_Email))
                {
                    m_pOtionalItemsRoundRobin.Add("email");
                }
                if (!string.IsNullOrEmpty(m_Phone))
                {
                    m_pOtionalItemsRoundRobin.Add("phone");
                }
                if (!string.IsNullOrEmpty(m_Location))
                {
                    m_pOtionalItemsRoundRobin.Add("location");
                }
                if (!string.IsNullOrEmpty(m_Tool))
                {
                    m_pOtionalItemsRoundRobin.Add("tool");
                }
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds next(round-robined) optional SDES item to SDES chunk, if any available.
        /// </summary>
        /// <param name="sdes">SDES chunk where to add item.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sdes</b> is null reference.</exception>
        internal void AddNextOptionalSdesItem(RTCP_Packet_SDES_Chunk sdes)
        {
            if (sdes == null)
            {
                throw new ArgumentNullException("sdes");
            }

            lock (m_pOtionalItemsRoundRobin)
            {
                if (m_pOtionalItemsRoundRobin.Count > 0)
                {
                    string itemName = m_pOtionalItemsRoundRobin.Next();

                    if (itemName == "name")
                    {
                        sdes.Name = m_Name;
                    }
                    else if (itemName == "email")
                    {
                        sdes.Email = m_Email;
                    }
                    else if (itemName == "phone")
                    {
                        sdes.Phone = m_Phone;
                    }
                    else if (itemName == "location")
                    {
                        sdes.Location = m_Location;
                    }
                    else if (itemName == "tool")
                    {
                        sdes.Tool = m_Tool;
                    }
                    else if (itemName == "note")
                    {
                        sdes.Note = m_Note;
                    }
                }
            }
        }

        #endregion

        // TODO: PRIV
    }
}