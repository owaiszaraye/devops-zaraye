namespace Zaraye.Core.Domain.CustomerLedgers
{
    public enum ReceivableAndPayablePaymentStatus
    {
        //add new payment stautes by zaraye payment system   
        WatingBusinessHeadApproval =10,
        RejectByBusinessHead = 20,
        WatingFinanceApproval = 30,
        RejectByFinance = 40,
        PaymentVerfied = 50,
        WatingOpsHeadApproval = 60,
    }
}
