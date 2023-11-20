namespace Zaraye.Models.Api.V4.Buyer
{
    public class CreditApplicationModel
    {
        public CreditApplicationModel()
        {
            HistoricInvoicesFileIds = new List<int>();
            UtilityBillFileIds = new List<int>();
            CnicFrontAndBackFileIds = new List<int>();
            BankStatements = new List<BankStatementModel>();
        }

        public int AnnualBusinessRev { get; set; }
        public int CurrentAssetValuation { get; set; }
        public string DatedChecque { get; set; }
        public int FinancialStatementFileId { get; set; }
        public int FinancingAmountRequired { get; set; }
        public int FixedAssetValuation { get; set; }
        public List<int> HistoricInvoicesFileIds { get; set; }
        public int NoOfUniqueClients { get; set; }
        public int TaxationCertificateFileId { get; set; }
        public int TenancyContractFileId { get; set; }
        public List<BankStatementModel> BankStatements { get; set; }
        public string BusinessName { get; set; }
        public List<int> CnicFrontAndBackFileIds { get; set; }
        public string EmailAddress { get; set; }
        public int IncorporationCertificateFileId { get; set; }
        public string PhoneNumber { get; set; }
        public int BusinessCityId { get; set; }
        public List<int> UtilityBillFileIds { get; set; }
    }
    public class BankStatementModel
    {
        public string BankName { get; set; }
        public int FileId { get; set; }
       // public string url { get; set; }
    }
}
