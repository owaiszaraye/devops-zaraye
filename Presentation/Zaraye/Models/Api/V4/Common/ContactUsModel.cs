using System.ComponentModel.DataAnnotations;

namespace Zaraye.Models.Api.V4.Common
{
    public partial record ContactUsModel 
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Enquiry { get; set; }
        public string FullName { get; set; }
        public string ContactNo { get; set; }
        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }
        public string Country { get; set; }
        public string CompanyDomain { get; set; }
       
    }
}
