// Copyright © 2016 Jeroen Stemerdink.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
namespace EPi.Libraries.Logging.OneTrueError
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using EPiServer.Framework.Configuration;
    using EPiServer.Logging;

    using global::OneTrueError.Client;

    using ILogger = EPiServer.Logging.ILogger;

    /// <summary>
    ///     Class OneTrueErrorLogger.
    /// </summary>
    public class OneTrueErrorLogger : ILogger
    {
        /// <summary>
        /// The levels to log
        /// </summary>
        private static string[] levels;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTrueErrorLogger"/> class.
        /// </summary>
        public OneTrueErrorLogger()
        {
            string availableLevels = ConfigurationManager.AppSettings["onetrueerror:levels"];

            levels = string.IsNullOrWhiteSpace(availableLevels) ? new[] { "Fatal", "Error" } : availableLevels.Split(',');
        }

        /// <summary>
        ///     Determines whether logging at the specified level is enabled.
        /// </summary>
        /// <param name="level">The level to check.</param>
        /// <returns><c>true</c> if logging on the provided level is enabled; otherwise <c>false</c></returns>
        public bool IsEnabled(Level level)
        {
            return OneTrue.Configuration != null && levels.Contains(level.ToString());
        }

        /// <summary>
        /// Logs the provided <paramref name="state"/> with the specified level.
        /// </summary>
        /// <typeparam name="TState">The type of the state object.</typeparam><typeparam name="TException">The type of the exception.</typeparam><param name="level">The criticality level of the log message.</param><param name="state">The state that should be logged.</param><param name="exception">The exception that should be logged.</param><param name="messageFormatter">The message formatter used to write the state to the log provider.</param><param name="boundaryType">The type at the boundary of the logging framework facing the code using the logging.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public void Log<TState, TException>(
            Level level, 
            TState state, 
            TException exception, 
            Func<TState, TException, string> messageFormatter, 
            Type boundaryType) where TException : Exception
        {
            if (messageFormatter == null)
            {
                return;
            }

            if (this.IsEnabled(level) && exception != null)
            {
                OneTrue.Report(
                    exception,
                    new LogEntryDetails
                        {
                            LogLevel = level.ToString(),
                            Message = messageFormatter(state, exception),
                            Timestamp = DateTime.Now
                        });
            }
        }
    }
}