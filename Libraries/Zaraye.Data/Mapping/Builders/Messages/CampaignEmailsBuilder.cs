using FluentMigrator.Builders.Create.Table;
using Zaraye.Core.Domain.Messages;

namespace Zaraye.Data.Mapping.Builders.Messages
{
  
    public partial class CampaignEmailsBuilder : ZarayeEntityBuilder<CampaignEmail>
    {
        #region Methods

       
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CampaignEmail.Email)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}