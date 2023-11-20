using System.ComponentModel.DataAnnotations;

namespace Zaraye.Models.Api.V4.Request
{
    public class RegisterRequestModel
    {
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string Phone { get; set; }
    }
}
