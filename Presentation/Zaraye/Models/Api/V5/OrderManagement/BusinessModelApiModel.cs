using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Zaraye.Models.Api.V5.OrderManagement
{
    public partial class BusinessModelApiModel
    {
        public BusinessModelApiModel()
        {
            Payables = new List<Fields>();
            Receivables = new List<Fields>();
        }

        public decimal SalePrice { get; set; }
        public int RequestId { get; set; }
        public int DirectOrderId { get; set; }
        public int OrderCalculationId { get; set; }
        public decimal Quantity { get; set; }
        public string BuyerName { get; set; }
        public string SupplierName { get; set; }
        public int QuotationId { get; set; }
        public string BusinessModelName { get; set; }
        public IList<Fields> Payables { get; set; }
        public IList<Fields> Receivables { get; set; }

        public class Fields
        {
            public Fields()
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