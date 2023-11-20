using Zaraye.Core.Domain.Catalog;
using Zaraye.Services.Directory;
using System;
using System.Threading.Tasks;

namespace Zaraye.Services.Catalog
{
    public partial class QuantityFormatter : IQuantityFormatter
    {
        #region fields
        private readonly IProductService _productService;
        private readonly IMeasureService _measureService;
        private readonly CatalogSettings _catalogSettings;

        public QuantityFormatter(IProductService productService,
            IMeasureService measureService,
            CatalogSettings catalogSettings)
        {
            _productService = productService;
            _measureService = measureService;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        //A boolean function to identify whether the value is decimal or integer
        static bool IsDecimal(decimal value)
        {
            decimal fractionalPart = value % 1;
            if (fractionalPart == 0)
            {
                return false; // It's an integer
            }
            else
            {
                return true; // It's a decimal
            }
        }

        #endregion

        #region methods

        public virtual decimal FormatDecimalQuantity(decimal quantity)
        {
            if (!IsDecimal(quantity))
            {
                quantity = Convert.ToInt32(quantity);
                return quantity;
            }

            return Math.Round(quantity, _catalogSettings.DecimalsSettings);
        }

        public virtual string FormatQuantity(decimal quantity)
        {
            if (!IsDecimal(quantity))
            {
                quantity = Convert.ToInt32(quantity);
                return quantity.ToString();
            }

            return Math.Round(quantity, _catalogSettings.DecimalsSettings).ToString();
        }

        public virtual async Task<string> FormatQuantity(decimal quantity, int productId = 0)
        {
            var unit = string.Empty;
            string quantityWithUnit = string.Empty;

            if (productId > 0)
            {
                var product = (await _productService.GetProductByIdAsync(productId));
                if (product != null)
                    unit = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty;
            }

            if (!IsDecimal(quantity))
            {
                quantity = Convert.ToInt32(quantity);

                if (!String.IsNullOrWhiteSpace(unit))
                    quantityWithUnit = $"{quantity} - {unit}";
                else
                    quantityWithUnit = quantity.ToString();

                return quantityWithUnit;
            }

            if (!String.IsNullOrWhiteSpace(unit))
                quantityWithUnit = $"{Math.Round(quantity, _catalogSettings.DecimalsSettings)} - {unit}";
            else
                quantityWithUnit = Math.Round(quantity, _catalogSettings.DecimalsSettings).ToString();

            return quantityWithUnit;
        }

        #endregion
    }
}
