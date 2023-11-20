using Newtonsoft.Json;
using System;

namespace Zaraye.Core.Domain.Orders
{
    public class DirectOrderDeliverySchedule : BaseEntity
    {
        [JsonProperty("expectedDeliveryDateUtc")]
        public DateTime? ExpectedDeliveryDateUtc { get; set; }
        [JsonProperty("expectedShipmentDateUtc")]
        public DateTime? ExpectedShipmentDateUtc { get; set; }
        [JsonProperty("expectedQuantity")]
        public decimal? ExpectedQuantity { get; set; }

        [JsonProperty("expectedDeliveryCost")]
        public decimal? ExpectedDeliveryCost { get; set; }
        [JsonProperty("createdOnUtc")]
        public DateTime CreatedOnUtc { get; set; }
        [JsonProperty("directOrderId")]
        public int DirectOrderId { get; set; }
    }
}
