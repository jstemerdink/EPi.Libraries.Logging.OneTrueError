// Copyright © 2017 Jeroen Stemerdink.
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
namespace EPi.Libraries.Logging.OneTrueError.Configuration
{
    using System;
    using System.Configuration;

    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Logging;

    using global::OneTrueError.Client;

    /// <summary>
    ///     The initialization module for configuring <see cref="EPi.Libraries.Logging.OneTrueError.Configuration"/>.
    /// </summary>
    [InitializableModule]
    public class OneTrueErrorInitialization : IInitializableModule
    {
        /// <summary>
        ///     Check if the initialization has been done.
        /// </summary>
        private static bool initialized;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <remarks>
        ///     Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        ///     only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        ///     method will be called repeatedly for each request reaching the site until the method succeeds.
        /// </remarks>
        /// <exception cref="ConfigurationErrorsException">Required settings for OneTrueError not configured.</exception>
        public void Initialize(InitializationEngine context)
        {
            // If there is no context, we can't do anything.
            if (context == null)
            {
                return;
            }

            // If already initialized, no need to do it again.
            if (initialized)
            {
                return;
            }

            string host = ConfigurationManager.AppSettings["onetrueerror:host"];
            string appKey = ConfigurationManager.AppSettings["onetrueerror:appkey"];
            string sharedSecret = ConfigurationManager.AppSettings["onetrueerror:sharedsecret"];

            if (string.IsNullOrWhiteSpace(host))
            {
               throw new ConfigurationErrorsException("Host for OneTrueError not configured in appSettings."); 
            }

            if (string.IsNullOrWhiteSpace(appKey))
            {
                throw new ConfigurationErrorsException("App key for OneTrueError not configured in appSettings.");
            }

            if (string.IsNullOrWhiteSpace(sharedSecret))
            {
                throw new ConfigurationErrorsException("Shared secret for OneTrueError not configured in appSettings.");
            }

            Uri url = new Uri(host);
            OneTrue.Configuration.Credentials(url, appKey, sharedSecret);
            OneTrue.Configuration.CatchMvcExceptions();
            OneTrue.Configuration.ContextProviders.Add(new EPiServerContextProvider());

            initialized = true;
        }

        /// <summary>
        ///     Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         This method is usually not called when running under a web application since the web app may be shut down very
        ///         abruptly, but your module should still implement it properly since it will make integration and unit testing
        ///         much simpler.
        ///     </para>
        ///     <para>
        ///         Any work done by
        ///         <see
        ///             cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)" />
        ///         as well as any code executing on
        ///         <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete" />
        ///         should be reversed.
        ///     </para>
        /// </remarks>
        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}