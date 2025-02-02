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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantTooShortException : Exception
    {
        public int MinLength = 0;
        public int MaxLength = 0;

        public TenantTooShortException(string message)
            : base(message)
        {
        }

        public TenantTooShortException(string message, int minLength, int maxLength)
            : base(message)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        protected TenantTooShortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantIncorrectCharsException : Exception
    {
        public TenantIncorrectCharsException(string message)
            : base(message)
        {
        }

        protected TenantIncorrectCharsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantAlreadyExistsException : Exception
    {
        public IEnumerable<string> ExistsTenants
        {
            get;
            private set;
        }

        public TenantAlreadyExistsException(string message, IEnumerable<string> existsTenants)
            : base(message)
        {
            ExistsTenants = existsTenants ?? Enumerable.Empty<string>();
        }

        protected TenantAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}