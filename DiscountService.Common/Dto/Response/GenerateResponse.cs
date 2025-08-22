using Newtonsoft.Json;
namespace DiscountService.Common.Dto.Response
{
    public class GenerateResponse
    {
        public bool Result { get; set; }
        public List<string> Codes { get; set; } = new();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ErrorMessage { get; set; }
    }
}
