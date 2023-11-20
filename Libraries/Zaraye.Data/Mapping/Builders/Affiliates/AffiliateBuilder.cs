using System.Data;
using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Affiliates;
using Zaraye.Core.Domain.Common;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Mapping.Builders.Affiliates
{
    /// <summary>
    /// Represents a affiliate entity builder
    /// </summary>
    public partial class AffiliateBuilder : ZarayeEntityBuilder<Affiliate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Affiliate.AddressId)).AsInt32().ForeignKey<Address>().OnDelete(Rule.None);
        }

        #endregion
    }
}