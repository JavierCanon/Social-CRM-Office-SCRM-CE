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

namespace ASC.Web.Core.Calendars
{
    [System.AttributeUsage(System.AttributeTargets.Class,
                   AllowMultiple = false,
                   Inherited = true)]
    public class AllDayLongUTCAttribute : Attribute
    { }

    public enum EventRepeatType
    {
        Never = 0,
        EveryDay = 3,
        EveryWeek = 4,
        EveryMonth = 5,
        EveryYear = 6
    }

    public enum EventAlertType
    {
        Default = -1,
        Never = 0,
        FiveMinutes = 1,
        FifteenMinutes = 2,
        HalfHour = 3,
        Hour = 4,
        TwoHours = 5,
        Day = 6
    }

    public enum EventStatus
    {
        Tentative = 0,
        Confirmed = 1,
        Cancelled = 2
    }

    public class EventContext : ICloneable
    {
        //public EventRepeatType RepeatType { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public interface IEvent : IiCalFormatView
    {
        string Id { get; }
        string Uid { get; }
        string CalendarId { get; }
        string Name { get; }
        string Description { get; }
        Guid OwnerId { get; }
        DateTime UtcStartDate { get; }
        DateTime UtcEndDate { get; }
        DateTime UtcUpdateDate { get; }
        EventAlertType AlertType { get; }
        bool AllDayLong { get; }
        RecurrenceRule RecurrenceRule { get; }
        EventContext Context { get; }
        SharingOptions SharingOptions { get;}
        EventStatus Status { get; }
    }
}
