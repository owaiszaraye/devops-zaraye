using System.Collections.Generic;

namespace Zaraye.Data.Mapping
{
    public partial class ZarayeEntityDescriptor
    {
        public ZarayeEntityDescriptor()
        {
            Fields = new List<ZarayeEntityFieldDescriptor>();
        }

        public string EntityName { get; set; }
        public string SchemaName { get; set; }
        public ICollection<ZarayeEntityFieldDescriptor> Fields { get; set; }
    }
}