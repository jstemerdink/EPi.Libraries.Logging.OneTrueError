// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPiServerContextProvider.cs" company="Valtech">
//     Copyright © 2017 Valtech.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EPi.Libraries.Logging.OneTrueError.Configuration
{
    using System;
    using System.Collections.Generic;

    using EPiServer.Globalization;
    using EPiServer.ServiceLocation;
    using EPiServer.Web.Routing;

    using global::OneTrueError.Client.ContextProviders;
    using global::OneTrueError.Client.Contracts;
    using global::OneTrueError.Client.Reporters;

    using Jos.ContentJson.Extensions;

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

            try
            {
                contentRouteRouteHelper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
            }
            catch (ActivationException)
            {
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
                                                                 contentRouteRouteHelper?.Content?.ContentLink.ID.ToString()
                                                             },
                                                             {
                                                                 "ContentData",
                                                                 contentRouteRouteHelper?.Content?.ToJson()
                                                             }
                                                         };

            return new ContextCollectionDTO(this.Name, contextInfo);
        }
    }
}