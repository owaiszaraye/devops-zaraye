using Newtonsoft.Json;

namespace Zaraye.Models.Api.V4.Common
{
    public partial class BaseApiModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}