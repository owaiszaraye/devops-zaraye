using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zaraye.Models.Api.V4.OrderManagement
{
    public partial class InventoryBusinessModelApiModel
    {
        public InventoryBusinessModelApiModel()
        {
            //Payables = new List<Fields>();
            Receivables = new List<InventoryFields>();
        }

        //public int SupplierId { get; set; }
        public int BuyerRequestId { get; set; }
        public int QuotationId { get; set; }
        public int InventoryId { get; set; }
        public decimal Quantity { get; set; }
        //public string SupplierName { get; set; }
        //public IList<Fields> Payables { get; set; }
        public IList<InventoryFields> Receivables { get; set; }

        public class InventoryFields
        {
            public InventoryFields()
            {
                toggleOptions = new string[] { };
                DropdownOptions = new List<SelectListItem>();
            }
            public string Name { get; set; }
            public string Label { get; set; }
            public string FieldType { get; set; }
            public object DefaultValue { get; set; }
            public string ValueFormatted { get; set; }
            public object Value { get; set; }
            public bool Required { get; set; }
            public bool IsReadOnly { get; set; }
            public string[] toggleOptions { get; set; }
            public IList<SelectListItem> DropdownOptions { get; set; }
            public bool NeedRequest { get; set; }
            public string CalculationType { get; set; }
        }
    }
}