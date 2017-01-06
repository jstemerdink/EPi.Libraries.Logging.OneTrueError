// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryDetails.cs" company="Valtech">
//     Copyright © 2017 Valtech.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EPi.Libraries.Logging.OneTrueError
{
    using System;

    /// <summary>
    ///     Context View Model attached to all reported exceptions
    /// </summary>
    public class LogEntryDetails
    {
        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public string LogLevel { get; set; }

        /// <summary>
        ///     Gets or sets the logged message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the time stamp
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}