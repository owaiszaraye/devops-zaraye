using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Common
{
    public partial class AppliedCreditCustomer : BaseEntity, IActiveActivityLogEntity
    {
        public int CustomerId { get; set; }
        public int AnnualBusinessRevenue { get; set; }
        public int CurrentAssetValuation { get; set; }
        public string DatedChecque { get; set; }
        public int FinancialStatementFileId { get; set; }
        public int FinancingAmountRequired { get; set; }
        public int FixedAssetValuation { get; set; }
        public string HistoricInvoicesFileIds { get; set; }
        public int NoOfUniqueClients { get; set; }
        public int TaxationCertificateFileId { get; set; }
        public int TenancyContractFileId { get; set; }
        public string BusinessName { get; set; }
        public string CnicFrontAndBackFileIds { get; set; }
        public string EmailAddress { get; set; }
        public int IncorporationCertificateFileId { get; set; }
        public string PhoneNumber { get; set; }
        public int BusinessCityId { get; set; }
        public string UtilityBillFileIds { get; set; }
        public int StatusId { get; set; }
        public string Comment { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }

        public AppliedCreditCustomerStatusEnum AppliedCreditCustomerStatusEnum
        {
            get => (AppliedCreditCustomerStatusEnum)StatusId;
            set => StatusId = (int)value;
        }
        public int ApprovedById { get; set; }
    }
}
