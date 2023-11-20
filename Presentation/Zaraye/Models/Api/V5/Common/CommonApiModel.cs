using System.Collections.Generic;

namespace Zaraye.Models.Api.V5.Common
{
    public partial class CommonApiModel : BaseApiModel
    {
        public class CountryApiModel
        {
            public string Id { get; set; }
            public string Name { get; set; }     
        }

        public class StateApiModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class ProductItemAttributeApiModel
        {
            public int AttributeControlTypeId { get; set; }
            public int AttributeId { get; set; }
            public string AttributeName { get; set; }
            public int ValueId { get; set; }
            public string Value { get; set; }
        }

        public class CombinationAttributeApiModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ProductAttributesApiModel
        {
            public ProductAttributesApiModel()
            {
                Values = new List<ProductAttributeValueApiModel>();
            }

            public int Id { get; set; }
            public int ProductId { get; set; }
            public int ProductAttributeId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string TextPrompt { get; set; }
            public bool IsRequired { get; set; }
            public int AttributeControlType { get; set; }
            public string DefaultValue { get; set; }
            public int? ValidationMinLength { get; set; }
            public int? ValidationMaxLength { get; set; }
            public bool HasCondition { get; set; }

            public int? SelectedDay { get; set; }
            public int? SelectedMonth { get; set; }
            public int? SelectedYear { get; set; }

            public IList<ProductAttributeValueApiModel> Values { get; set; }

            public class ProductAttributeValueApiModel
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string ColorSquaresRgb { get; set; }
                public bool IsPreSelected { get; set; }
                public bool CustomerEntersQty { get; set; }
                public int Quantity { get; set; }
            }
        }
    }
}