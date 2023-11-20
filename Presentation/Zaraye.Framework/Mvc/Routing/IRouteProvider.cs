using Microsoft.AspNetCore.Routing;

namespace Zaraye.Framework.Mvc.Routing
{
    /// <summary>
    /// Route provider
    /// </summary>
    public partial interface IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder);

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        int Priority { get; }
    }
}
