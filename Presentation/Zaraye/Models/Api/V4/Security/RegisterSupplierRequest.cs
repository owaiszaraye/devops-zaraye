using System.ComponentModel.DataAnnotations;

namespace Zaraye.Models.Api.V4.Security
{
    public class RegisterSupplierRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Phone no. is required")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Country Id should be greater than zero")]
        public int CountryId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "City Id should be greater than zero")]
        public int CityId { get; set; }
        public string ProductDescription { get; set; }
    }
}
