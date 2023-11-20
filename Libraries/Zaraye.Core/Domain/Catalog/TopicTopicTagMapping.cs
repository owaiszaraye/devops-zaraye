using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class TopicTopicTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the category tag identifier
        /// </summary>
        public int TopicTagId { get; set; }
    }
}
