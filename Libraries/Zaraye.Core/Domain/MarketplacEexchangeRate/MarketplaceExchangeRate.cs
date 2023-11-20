using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Localization;

namespace Zaraye.Core.Domain.MarketplaceExchangeRate
{
    public partial class MarketplaceExchangeRate : BaseEntity, ILocalizedEntity, DefaultColumns
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Symbol { get; set; }
        public decimal Rate { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
