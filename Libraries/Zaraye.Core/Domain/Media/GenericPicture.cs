using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Media
{
    public partial class GenericPicture : BaseEntity
    {
        public int PictureId { get; set; }
        public int EntityId { get; set; }
        public int EntityType { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
