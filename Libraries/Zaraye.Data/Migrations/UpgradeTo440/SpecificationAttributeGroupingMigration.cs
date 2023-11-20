using FluentMigrator;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Data.Mapping;
using Zaraye.Data.Extensions;

namespace Zaraye.Data.Migrations.UpgradeTo440
{
    [ZarayeMigration("2020/03/08 11:26:08:9037680", "Specification attribute grouping", MigrationProcessType.Update)]
    public class SpecificationAttributeGroupingMigration : MigrationBase
    {

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttributeGroup))).Exists())
                Create.TableFor<SpecificationAttributeGroup>();

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttribute))).Column(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).Exists())
            {
                //add new column
                Alter.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttribute)))
                    .AddColumn(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).AsInt32().Nullable().ForeignKey<SpecificationAttributeGroup>();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        #endregion
    }
}
