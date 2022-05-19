using Newtonsoft.Json;

namespace Insurance.Shared.DTOs
{
    public class ProductTypeIntegrationDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
        [JsonProperty("canBeInsured")]
        public bool CanBeInsured { get; set; } 
    }
}
