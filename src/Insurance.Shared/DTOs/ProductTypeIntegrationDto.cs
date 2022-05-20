using System.Text.Json.Serialization;

namespace Insurance.Shared.DTOs
{
    public class ProductTypeIntegrationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } 
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("canBeInsured")]
        public bool CanBeInsured { get; set; } 
    }
}
