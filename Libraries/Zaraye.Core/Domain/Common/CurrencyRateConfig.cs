using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Common
{
    public partial class CurrencyRateConfig : BaseEntity
    {
        public int CurrencyId { get; set; }
        public decimal Rate { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
    }
}