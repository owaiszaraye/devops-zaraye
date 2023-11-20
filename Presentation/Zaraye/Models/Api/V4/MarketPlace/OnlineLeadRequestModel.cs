using System.ComponentModel.DataAnnotations;

namespace Zaraye.Models.Api.V4.MarketPlace
{
    public class OnlineLeadRequestModel
    {
        [Required]
        public string Service { get; set; }
        [Range(1, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Required]
        public string Unit { get; set; }
        [Required]
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }

        public string Email { get; set; }
        public string CityName { get; set; }
        public string Source { get; set; }
    }
}
