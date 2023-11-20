using System;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Seo;
using Zaraye.Core.Domain.Stores;

namespace Zaraye.Core.Domain.Blogs
{
    /// <summary>
    /// Represents a blog post
    /// </summary>
    public partial class BlogPost : BaseEntity,StoreEntity, ISlugSupported, IStoreMappingSupported, DefaultColumns, IActiveActivityLogEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this blog post should be included in sitemap
        /// </summary>
        public bool IncludeInSitemap { get; set; }

        /// <summary>
        /// Gets or sets the blog post title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the blog post body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the blog post overview. If specified, then it's used on the blog page instead of the "Body"
        /// </summary>
        public string BodyOverview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the blog post comments are allowed 
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets the blog tags
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the blog post start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the blog post end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public virtual bool LimitedToStores { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public string AuthorName { get; set; }

        public int PictureId { get; set; }

        public int StoreId { get; set; }
    }
}