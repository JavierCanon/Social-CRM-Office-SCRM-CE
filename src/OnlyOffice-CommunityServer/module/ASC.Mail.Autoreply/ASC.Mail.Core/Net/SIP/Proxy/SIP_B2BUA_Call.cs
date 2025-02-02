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


namespace ASC.Mail.Net.SIP.Proxy
{
    #region usings

    using System;
    using Message;
    using Stack;

    #endregion

    /// <summary>
    /// This class represents B2BUA call.
    /// </summary>
    public class SIP_B2BUA_Call
    {
        #region Members

        private readonly string m_CallID = "";
        private readonly SIP_B2BUA m_pOwner;
        private readonly DateTime m_StartTime;
        private bool m_IsTerminated;
        private SIP_Dialog m_pCallee;
        private SIP_Dialog m_pCaller;

        #endregion

        #region Properties

        /// <summary>
        /// Gets call start time.
        /// </summary>
        public DateTime StartTime
        {
            get { return m_StartTime; }
        }

        /// <summary>
        /// Gets current call ID.
        /// </summary>
        public string CallID
        {
            get { return m_CallID; }
        }

        /// <summary>
        /// Gets caller SIP dialog.
        /// </summary>
        public SIP_Dialog CallerDialog
        {
            get { return m_pCaller; }
        }

        /// <summary>
        /// Gets callee SIP dialog.
        /// </summary>
        public SIP_Dialog CalleeDialog
        {
            get { return m_pCallee; }
        }

        /// <summary>
        /// Gets if call has timed out and needs to be terminated.
        /// </summary>
        public bool IsTimedOut
        {
            // TODO:

            get { return false; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner B2BUA server.</param>
        /// <param name="caller">Caller side dialog.</param>
        /// <param name="callee">Callee side dialog.</param>
        internal SIP_B2BUA_Call(SIP_B2BUA owner, SIP_Dialog caller, SIP_Dialog callee)
        {
            m_pOwner = owner;
            m_pCaller = caller;
            m_pCallee = callee;
            m_StartTime = DateTime.Now;
            m_CallID = Guid.NewGuid().ToString().Replace("-", "");

            //m_pCaller.RequestReceived += new SIP_RequestReceivedEventHandler(m_pCaller_RequestReceived);
            //m_pCaller.Terminated += new EventHandler(m_pCaller_Terminated);

            //m_pCallee.RequestReceived += new SIP_RequestReceivedEventHandler(m_pCallee_RequestReceived);
            //m_pCallee.Terminated += new EventHandler(m_pCallee_Terminated);           
        }

        #endregion

        #region Methods

        /// <summary>
        /// Terminates call.
        /// </summary>
        public void Terminate()
        {
            if (m_IsTerminated)
            {
                return;
            }
            m_IsTerminated = true;

            m_pOwner.RemoveCall(this);

            if (m_pCaller != null)
            {
                //m_pCaller.Terminate();
                m_pCaller.Dispose();
                m_pCaller = null;
            }
            if (m_pCallee != null)
            {
                //m_pCallee.Terminate();
                m_pCallee.Dispose();
                m_pCallee = null;
            }

            m_pOwner.OnCallTerminated(this);
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Is called when caller sends new request.
        /// </summary>
        /// <param name="e">Event data.</param>
        private void m_pCaller_RequestReceived(SIP_RequestReceivedEventArgs e)
        {
            // TODO: If we get UPDATE, but callee won't support it ? generate INVITE instead ?
            /*
            SIP_Request request = m_pCallee.CreateRequest(e.Request.RequestLine.Method);
            CopyMessage(e.Request,request,new string[]{"Via:","Call-Id:","To:","From:","CSeq:","Contact:","Route:","Record-Route:","Max-Forwards:","Allow:","Require:","Supported:"});
            // Remove our Authentication header if it's there.
            foreach(SIP_SingleValueHF<SIP_t_Credentials> header in request.ProxyAuthorization.HeaderFields){
                try{
                    Auth_HttpDigest digest = new Auth_HttpDigest(header.ValueX.AuthData,request.RequestLine.Method);
                    if(m_pOwner.Stack.Realm == digest.Realm){
                        request.ProxyAuthorization.Remove(header);
                    }
                }
                catch{
                    // We don't care errors here. This can happen if remote server xxx auth method here and
                    // we don't know how to parse it, so we leave it as is.
                }
            }

            SIP_ClientTransaction clientTransaction = m_pCallee.CreateTransaction(request);
            clientTransaction.ResponseReceived += new EventHandler<SIP_ResponseReceivedEventArgs>(m_pCallee_ResponseReceived);
            clientTransaction.Tag = e.ServerTransaction;
            clientTransaction.Start();*/
        }

        /// <summary>
        /// This method is called when caller dialog has terminated, normally this happens 
        /// when dialog gets BYE request.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pCaller_Terminated(object sender, EventArgs e)
        {
            Terminate();
        }

        /// <summary>
        /// This method is called when callee dialog client transaction receives response.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pCallee_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
        {
            SIP_ServerTransaction serverTransaction = (SIP_ServerTransaction) e.ClientTransaction.Tag;
            //SIP_Response response = serverTransaction.Request.CreateResponse(e.Response.StatusCode_ReasonPhrase);
            //CopyMessage(e.Response,response,new string[]{"Via:","Call-Id:","To:","From:","CSeq:","Contact:","Route:","Record-Route:","Allow:","Supported:"});
            //serverTransaction.SendResponse(response);
        }

        /// <summary>
        /// Is called when callee sends new request.
        /// </summary>
        /// <param name="e">Event data.</param>
        private void m_pCallee_RequestReceived(SIP_RequestReceivedEventArgs e)
        {
            /*
            SIP_Request request = m_pCaller.CreateRequest(e.Request.RequestLine.Method);
            CopyMessage(e.Request,request,new string[]{"Via:","Call-Id:","To:","From:","CSeq:","Contact:","Route:","Record-Route:","Max-Forwards:","Allow:","Require:","Supported:"});
            // Remove our Authentication header if it's there.
            foreach(SIP_SingleValueHF<SIP_t_Credentials> header in request.ProxyAuthorization.HeaderFields){
                try{
                    Auth_HttpDigest digest = new Auth_HttpDigest(header.ValueX.AuthData,request.RequestLine.Method);
                    if(m_pOwner.Stack.Realm == digest.Realm){
                        request.ProxyAuthorization.Remove(header);
                    }
                }
                catch{
                    // We don't care errors here. This can happen if remote server xxx auth method here and
                    // we don't know how to parse it, so we leave it as is.
                }
            }

            SIP_ClientTransaction clientTransaction = m_pCaller.CreateTransaction(request);
            clientTransaction.ResponseReceived += new EventHandler<SIP_ResponseReceivedEventArgs>(m_pCaller_ResponseReceived);
            clientTransaction.Tag = e.ServerTransaction;
            clientTransaction.Start();*/
        }

        /// <summary>
        /// This method is called when callee dialog has terminated, normally this happens 
        /// when dialog gets BYE request.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pCallee_Terminated(object sender, EventArgs e)
        {
            Terminate();
        }

        /// <summary>
        /// This method is called when caller dialog client transaction receives response.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pCaller_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
        {
            SIP_ServerTransaction serverTransaction = (SIP_ServerTransaction) e.ClientTransaction.Tag;
            //SIP_Response response = serverTransaction.Request.CreateResponse(e.Response.StatusCode_ReasonPhrase);
            //CopyMessage(e.Response,response,new string[]{"Via:","Call-Id:","To:","From:","CSeq:","Contact:","Route:","Record-Route:","Allow:","Supported:"});
            //serverTransaction.SendResponse(response);
        }

        /*
        /// <summary>
        /// Transfers call to specified recipient.
        /// </summary>
        /// <param name="to">Address where to transfer call.</param>
        public void CallTransfer(string to)
        {
            throw new NotImplementedException();
        }*/

        /// <summary>
        /// Copies header fileds from 1 message to antother.
        /// </summary>
        /// <param name="source">Source message.</param>
        /// <param name="destination">Destination message.</param>
        /// <param name="exceptHeaders">Header fields not to copy.</param>
        private void CopyMessage(SIP_Message source, SIP_Message destination, string[] exceptHeaders)
        {
            foreach (SIP_HeaderField headerField in source.Header)
            {
                bool copy = true;
                foreach (string h in exceptHeaders)
                {
                    if (h.ToLower() == headerField.Name.ToLower())
                    {
                        copy = false;
                        break;
                    }
                }

                if (copy)
                {
                    destination.Header.Add(headerField.Name, headerField.Value);
                }
            }

            destination.Data = source.Data;
        }

        #endregion
    }
}