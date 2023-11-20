using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Common
{
    public partial class AwsS3Files : BaseEntity
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; }
        public string EntityName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }
    }
}
