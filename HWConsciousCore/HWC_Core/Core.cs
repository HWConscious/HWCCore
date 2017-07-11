﻿using System;

/// <summary>
/// Namespace for Core objects
/// </summary>
namespace HWC.Core
{
    #region Core Models

    /// <summary>
    /// Representation of general empty structure
    /// </summary>
    public class Empty
    { }

    /// <summary>
    /// Representation of general error structure
    /// </summary>
    public class Error
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string ReasonPharse { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="code">Error code</param>
        public Error(int code)
        {
            Code = code;
        }
    }

    /// <summary>
    /// Representation of Location
    /// </summary>
    public class Location
    {
        public LocationDeviceType Type { get; set; }
        public string DeviceID { get; set; }
        public DateTime? LocatedAtTimestamp { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">LocationDevice type</param>
        /// <param name="deviceID">LocationDevice ID. E.g., iBeacon device UUID</param>
        public Location (LocationDeviceType type, string deviceID)
        {
            Type = type;
            DeviceID = deviceID;
        }
    }

    /// <summary>
    /// Representation of Event
    /// </summary>
    public class Event
    {
        public EventType Type { get; set; }
        public DateTime EventAtTimestamp { get; set; }
        public EventSourceType SourceType { get; set; }
        public int SourceID { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">Event type</param>
        /// <param name="eventAtTimestamp">The DateTime when the Event occurred</param>
        /// <param name="sourceType">Specifies the origin of this event. For example, Notification, User etc.</param>
        /// <param name="sourceID">The identifier for the origin source of the event. For example, if the event occurred at the Notification level, the identifier would be the NotificationID.</param>
        public Event(EventType type, DateTime eventAtTimestamp, EventSourceType sourceType, int sourceID)
        {
            Type = type;
            EventAtTimestamp = eventAtTimestamp;
            SourceType = sourceType;
            SourceID  = sourceID;
        }
    }

    #endregion

    #region Enum types

    /// <summary>
    /// Representation of User types
    /// </summary>
    public enum UserType
    {
        None = 0,
        Guest = 1,
        Registered = 2
    }

    /// <summary>
    /// Representation of LocationDevice types
    /// </summary>
    public enum LocationDeviceType
    {
        None = 0,
        IBeacon = 1
    }

    /// <summary>
    /// Representation of MIME types
    /// </summary>
    public enum MimeType
    {
        None = 0,
        ImagePng = 1,
        ImageJpeg = 2,
        ImageJpg = 3
    }

    /// <summary>
    /// Representation of Event types
    /// </summary>
    public enum EventType
    {
        None = 0,
        DisplayEndpoint_Touch = 1
    }

    /// <summary>
    /// Representation of Event source types
    /// </summary>
    public enum EventSourceType
    {
        None = 0,
        Notification = 1,
        User = 2
    }

    #endregion
}
