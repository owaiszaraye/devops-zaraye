using DocumentFormat.OpenXml.Spreadsheet;

namespace Zaraye.Models.Api.V5.Buyer
{
    public partial class ApplyCreditCustomerModel
    {
      public string FullName { get; set; }
      public string RegisteredphoneNumber { get; set; }
      public string BusinessAddress { get; set; }
      public string CnicFrontImageByte { get; set; }
      public string CnicBackImageByte { get; set; }
      public string Cnic { get; set; }
    }
}
