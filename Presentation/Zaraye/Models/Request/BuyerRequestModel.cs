using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Framework.Models;
using Zaraye.Models.Media;

namespace Zaraye.Models.Request
{
    public partial record BuyerRequestModel : BaseZarayeEntityModel
    {
        public BuyerRequestModel()
        {
            AvailableIndustries = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvailableBrands = new List<SelectListItem>();
            AvailableProducts = new List<SelectListItem>();
            ProductAttributes = new List<ProductAttributeModel>();
        }

        public string Brand { get; set; }

        public int PaymentPlanId { get; set; }

        public int PaymentDuration { get; set; }

        public int IndustryId { get; set; }
        public string IndustryName { get; set; }
        public IList<SelectListItem> AvailableIndustries { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

        public int BrandId { get; set; }
        public string OtherBrand { get; set; }
        public IList<SelectListItem> AvailableBrands { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public IList<SelectListItem> AvailableProducts { get; set; }

        public string Status { get; set; }
        public int StatusId { get; set; }

        public decimal Quantity { get; set; }

        public string RequestAttributesXml { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryDateFormatted { get; set; }

        public string DeliveryAddress { get; set; }

        public DateTime CreatedOn { get; set; }

        public int TotalBids { get; set; }

        public IList<ProductAttributeModel> ProductAttributes { get; set; }

        public bool Success { get; set; }

        public partial record ProductAttributeModel : BaseZarayeEntityModel
        {
            public ProductAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<ProductAttributeValueModel>();
            }

            public int ProductId { get; set; }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }
            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// A value indicating whether this attribute depends on some other attribute
            /// </summary>
            public bool HasCondition { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ProductAttributeValueModel> Values { get; set; }
        }

        public partial record ProductAttributeValueModel : BaseZarayeEntityModel
        {
            public ProductAttributeValueModel()
            {
                ImageSquaresPictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            //picture model is used with "image square" attribute type
            public PictureModel ImageSquaresPictureModel { get; set; }

            public string PriceAdjustment { get; set; }

            public bool PriceAdjustmentUsePercentage { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            //product picture ID (associated to this value)
            public int PictureId { get; set; }

            public bool CustomerEntersQty { get; set; }

            public int Quantity { get; set; }
        }
    }
}