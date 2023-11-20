using Zaraye.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Orders
{
    public class OrderSalesReturnRequest : BaseEntity,StoreEntity, IActiveActivityLogEntity, DefaultColumns, ISoftDeletedEntity
    {
        public int OrderId { get; set; }
        public int SupplierId { get; set; }
        public int QuotationId { get; set; }
        public DateTime ReturnRequestDateUtc { get; set; }
        public decimal Quantity { get; set; }
        public string PickupAddress { get; set; }
        public string DropOffAddress { get; set; }
        public bool IsInventory { get; set; }
        public string ReturnReason { get; set; }
        public int AgentId { get; set; }
        public string Note { get; set; }
        public DateTime? TicketExpiryDate { get; set; }
        public int TicketPirority { get; set; }
        public int VerifiedUserId { get; set; }
        public int StatusId { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }

        public int StoreId { get; set; }



        public OrderSalesReturnRequestEnum orderSalesReturnRequestEnum
        {
            get => (OrderSalesReturnRequestEnum)StatusId;
            set => StatusId = (int)value;
        }
        public TicketEnum TicketEnum
        {
            get => (TicketEnum)TicketPirority;
            set => TicketPirority = (int)value;
        }
    }
}
