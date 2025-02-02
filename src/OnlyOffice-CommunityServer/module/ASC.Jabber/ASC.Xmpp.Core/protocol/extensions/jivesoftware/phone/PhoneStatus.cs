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


using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.jivesoftware.phone
{
    /// <summary>
    ///   A user's presence is updated when on a phone call. The Jive Messenger/Asterisk implementation will update the user's presence automatically by adding the following packet extension to the user's presence: &lt;phone-status xmlns="http://jivesoftware.com/xmlns/phone" status="ON_PHONE" &gt; Jive Messenger can also be configured to change the user's availability to "Away -- on the phone" when the user is on a call (in addition to the packet extension). This is useful when interacting with clients that don't understand the extended presence information or when using transports to other IM networks where extended presence information is not available.
    /// </summary>
    public class PhoneStatus : Element
    {
        /*
         * <phone-status xmlns="http://jivesoftware.com/xmlns/phone" status="ON_PHONE" >; 
         * 
         */

        public PhoneStatus()
        {
            TagName = "phone-status";
            Namespace = Uri.JIVESOFTWARE_PHONE;
        }

        public PhoneStatusType Status
        {
            set { SetAttribute("status", value.ToString()); }
            get { return (PhoneStatusType) GetAttributeEnum("status", typeof (PhoneStatusType)); }
        }
    }
}