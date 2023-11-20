using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.PriceDiscovery;

namespace Zaraye.Data.Mapping.Builders.PriceDiscovery
{
    public partial class DailyRateBuilder : ZarayeEntityBuilder<DailyRate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(DailyRate.Rate)).AsDecimal().NotNullable();
        }

        //public override void MapEntity(CreateTableExpressionBuilder table) => table

        //    table.WithColumn(nameof(SpecificationAttributeGroup.Name)).AsString(int.MaxValue).NotNullable();

        //    .WithColumn(nameof(DailyRate.SupplierId)).AsInt32().ForeignKey<Customer>()
        //    .WithColumn(nameof(DailyRate.IndustryId)).AsInt32().ForeignKey<Industry>()
        //    .WithColumn(nameof(DailyRate.CategoryId)).AsInt32().ForeignKey<Category>()
        //    .WithColumn(nameof(DailyRate.ProductId)).AsInt32().ForeignKey<Product>()
        //    .WithColumn(nameof(DailyRate.BrandId)).AsInt32().ForeignKey<Manufacturer>();

        #endregion
    }
}
