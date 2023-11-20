namespace Zaraye.Core.Http
{
    /// <summary>
    /// Represents default values related to HTTP features
    /// </summary>
    public static partial class ZarayeHttpDefaults
    {
        /// <summary>
        /// Gets the name of the default HTTP client
        /// </summary>
        public static string DefaultHttpClient => "default";

        /// <summary>
        /// Gets the name of a request item that stores the value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public static string IsPostBeingDoneRequestItem => "zaraye.IsPOSTBeingDone";

        /// <summary>
        /// Gets the name of a request item that stores the value that indicates whether the request is being redirected by the generic route transformer
        /// </summary>
        public static string GenericRouteInternalRedirect => "zaraye.RedirectFromGenericPathRoute";

        /// <summary>
        /// Gets the name of HTTP_CLUSTER_HTTPS header
        /// </summary>
        public static string HttpClusterHttpsHeader => "HTTP_CLUSTER_HTTPS";

        /// <summary>
        /// Gets the name of HTTP_X_FORWARDED_PROTO header
        /// </summary>
        public static string HttpXForwardedProtoHeader => "X-Forwarded-Proto";
    }
}