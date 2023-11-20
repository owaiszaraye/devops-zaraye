using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Customers
{
    /// <summary>
    /// Represents a customer address mapping entity builder
    /// </summary>
    public partial class CustomerIndustryMappingBuilder : ZarayeEntityBuilder<CustomerIndustryMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerIndustryMapping), nameof(CustomerIndustryMapping.IndustryId)))
                    .AsInt32().ForeignKey<Industry>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerIndustryMapping), nameof(CustomerIndustryMapping.CustomerId)))
                    .AsInt32().ForeignKey<Customer>().PrimaryKey();
        }

        #endregion
    }
}