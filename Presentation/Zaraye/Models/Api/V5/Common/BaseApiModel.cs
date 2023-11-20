using Newtonsoft.Json;

namespace Zaraye.Models.Api.V5.Common
{
    public partial class BaseApiModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}