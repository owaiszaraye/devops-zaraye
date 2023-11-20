namespace Zaraye.Core.Caching
{
    /// <summary>
    /// Represents default values related to caching entities
    /// </summary>
    public static partial class ZarayeEntityCacheDefaults<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Gets an entity type name used in cache keys
        /// </summary>
        public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

        /// <summary>
        /// Gets a key for caching entity by identifier
        /// </summary>
        /// <remarks>
        /// {0} : entity id
        /// </remarks>
        public static CacheKey ByIdCacheKey => new($"Zaraye.{EntityTypeName}.byid.{{0}}", ByIdPrefix, Prefix);

        /// <summary>
        /// Gets a key for caching entities by identifiers
        /// </summary>
        /// <remarks>
        /// {0} : entity ids
        /// </remarks>
        public static CacheKey ByIdsCacheKey => new($"Zaraye.{EntityTypeName}.byids.{{0}}", ByIdsPrefix, Prefix);

        /// <summary>
        /// Gets a key for caching all entities
        /// </summary>
        public static CacheKey AllCacheKey => new($"Zaraye.{EntityTypeName}.all.", AllPrefix, Prefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string Prefix => $"Zaraye.{EntityTypeName}.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ByIdPrefix => $"Zaraye.{EntityTypeName}.byid.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ByIdsPrefix => $"Zaraye.{EntityTypeName}.byids.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AllPrefix => $"Zaraye.{EntityTypeName}.all.";
    }
}