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
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This class represents RTP single-media and multimedia session.
    /// </summary>
    public class RTP_MultimediaSession : IDisposable
    {
        #region Events

        /// <summary>
        /// Is raised when unknown error has happened.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error = null;

        /// <summary>
        /// Is raised when new remote participant has joined to session.
        /// </summary>
        public event EventHandler<RTP_ParticipantEventArgs> NewParticipant = null;

        /// <summary>
        /// Is raised when new session has created.
        /// </summary>
        public event EventHandler<EventArgs<RTP_Session>> SessionCreated = null;

        #endregion

        #region Members

        private bool m_IsDisposed;
        private RTP_Participant_Local m_pLocalParticipant;
        private Dictionary<string, RTP_Participant_Remote> m_pParticipants;
        private List<RTP_Session> m_pSessions;

        #endregion

        #region Properties

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return m_IsDisposed; }
        }

        /// <summary>
        /// Gets media sessions.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Session[] Sessions
        {
            get
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pSessions.ToArray();
            }
        }

        /// <summary>
        /// Gets local participant.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Participant_Local LocalParticipant
        {
            get
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pLocalParticipant;
            }
        }

        /// <summary>
        /// Gets session remote participants.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Participant_Remote[] RemoteParticipants
        {
            get
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                lock (m_pParticipants)
                {
                    RTP_Participant_Remote[] retVal = new RTP_Participant_Remote[m_pParticipants.Count];
                    m_pParticipants.Values.CopyTo(retVal, 0);

                    return retVal;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cname">Canonical name of participant. <seealso cref="RTP_Utils.GenerateCNAME"/>RTP_Utils.GenerateCNAME 
        /// can be used to create this value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cname</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public RTP_MultimediaSession(string cname)
        {
            if (cname == null)
            {
                throw new ArgumentNullException("cname");
            }
            if (cname == string.Empty)
            {
                throw new ArgumentException("Argument 'cname' value must be specified.");
            }

            m_pLocalParticipant = new RTP_Participant_Local(cname);
            m_pSessions = new List<RTP_Session>();
            m_pParticipants = new Dictionary<string, RTP_Participant_Remote>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }
            foreach (RTP_Session session in m_pSessions.ToArray())
            {
                session.Dispose();
            }
            m_IsDisposed = true;

            m_pLocalParticipant = null;
            m_pSessions = null;
            m_pParticipants = null;

            NewParticipant = null;
            Error = null;
        }

        /// <summary>
        /// Closes RTP multimedia session, sends BYE with optional reason text to remote targets.
        /// </summary>
        /// <param name="closeReason">Close reason. Value null means not specified.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        public void Close(string closeReason)
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            foreach (RTP_Session session in m_pSessions.ToArray())
            {
                session.Close(closeReason);
            }

            Dispose();
        }

        /// <summary>
        /// Starts session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        public void Start()
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            // TODO:
        }

        /// <summary>
        /// Stops session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        public void Stop()
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            // TODO:
        }

        /// <summary>
        /// Creates new RTP session.
        /// </summary>
        /// <param name="localEP">Local RTP end point.</param>
        /// <param name="clock">RTP media clock.</param>
        /// <returns>Returns created session.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>localEP</b> or <b>clock</b> is null reference.</exception>
        public RTP_Session CreateSession(RTP_Address localEP, RTP_Clock clock)
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (localEP == null)
            {
                throw new ArgumentNullException("localEP");
            }
            if (clock == null)
            {
                throw new ArgumentNullException("clock");
            }

            RTP_Session session = new RTP_Session(this, localEP, clock);
            session.Disposed += delegate(object s, EventArgs e) { m_pSessions.Remove((RTP_Session) s); };
            m_pSessions.Add(session);

            OnSessionCreated(session);

            return session;
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Raises <b>SessionCreated</b> event.
        /// </summary>
        /// <param name="session">RTP session.</param>
        private void OnSessionCreated(RTP_Session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            if (SessionCreated != null)
            {
                SessionCreated(this, new EventArgs<RTP_Session>(session));
            }
        }

        /// <summary>
        /// Raises <b>NewParticipant</b> event.
        /// </summary>
        /// <param name="participant">New participant.</param>
        private void OnNewParticipant(RTP_Participant_Remote participant)
        {
            if (NewParticipant != null)
            {
                NewParticipant(this, new RTP_ParticipantEventArgs(participant));
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Gets or creates new participant if participant does not exist.
        /// </summary>
        /// <param name="cname">Participant canonical name.</param>
        /// <returns>Returns specified participant.</returns>
        internal RTP_Participant_Remote GetOrCreateParticipant(string cname)
        {
            if (cname == null)
            {
                throw new ArgumentNullException("cname");
            }
            if (cname == string.Empty)
            {
                throw new ArgumentException("Argument 'cname' value must be specified.");
            }

            lock (m_pParticipants)
            {
                RTP_Participant_Remote participant = null;
                if (!m_pParticipants.TryGetValue(cname, out participant))
                {
                    participant = new RTP_Participant_Remote(cname);
                    participant.Removed += delegate { m_pParticipants.Remove(participant.CNAME); };
                    m_pParticipants.Add(cname, participant);

                    OnNewParticipant(participant);
                }

                return participant;
            }
        }

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="exception">Exception.</param>
        internal void OnError(Exception exception)
        {
            if (Error != null)
            {
                Error(this, new ExceptionEventArgs(exception));
            }
        }

        #endregion
    }
}