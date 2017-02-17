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
    using System.Collections.Generic;
    using System.IO;

    using EPiServer.Framework.Serialization.Json;
    using EPiServer.Globalization;
    using EPiServer.ServiceLocation;
    using EPiServer.Web.Routing;

    using global::OneTrueError.Client.ContextProviders;
    using global::OneTrueError.Client.Contracts;
    using global::OneTrueError.Client.Reporters;

    using Newtonsoft.Json;

    /// <summary>
    /// Class EPiServerContextProvider.
    /// </summary>
    /// <seealso cref="IContextInfoProvider" />
    public class EPiServerContextProvider : IContextInfoProvider
    {
        /// <summary>
        /// Gets the name of the collection that this provider adds.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return "EPiServer";
            }
        }

        /// <summary>
        /// Collect information
        /// </summary>
        /// <param name="context">Context information provided by the class which reported the error.</param>
        /// <returns>Collection. Items with multiple values are joined using <c>";;"</c></returns>
        [CLSCompliant(false)]
        public ContextCollectionDTO Collect(IErrorReporterContext context)
        {
            IContentRouteHelper contentRouteRouteHelper = null;
            JsonSerializer serializer = CreateSerializer();
            string contentData;

            try
            {
                contentRouteRouteHelper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
            }
            catch (ActivationException)
            {
            }

            if (contentRouteRouteHelper == null)
            {
                return new ContextCollectionDTO(this.Name, new Dictionary<string, string>());
            }

            // TODO: Check whether to use invariant culture 
            using (StringWriter stringWriter = new StringWriter(ContentLanguage.PreferredCulture))
            {
                serializer.Serialize(stringWriter, contentRouteRouteHelper.Content);
                contentData = stringWriter.ToString();
            }

            Dictionary<string, string> contextInfo = new Dictionary<string, string>
                                                         {
                                                             {
                                                                 "PreferredCulture",
                                                                 ContentLanguage.PreferredCulture
                                                                     .Name
                                                             },
                                                             {
                                                                 "ContentId",
                                                                 contentRouteRouteHelper
                                                                     .Content?.ContentLink.ID
                                                                     .ToString()
                                                             },
                                                             { "ContentData", contentData }
                                                         };

            return new ContextCollectionDTO(this.Name, contextInfo);
        }

        /// <summary>
        /// Creates the serializer.
        /// </summary>
        /// <returns>The <see cref="JsonSerializer"/>.</returns>
        private static JsonSerializer CreateSerializer()
        {
            ErrorContractResolver jsonResolver = new ErrorContractResolver();

            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new DateTimeConverter());
            jsonSerializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializer.TypeNameHandling = TypeNameHandling.Objects;
            jsonSerializer.MaxDepth = 25;
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializer.Formatting = Formatting.Indented;
            jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonSerializer.ContractResolver = jsonResolver;
            return jsonSerializer;
        }
    }
}