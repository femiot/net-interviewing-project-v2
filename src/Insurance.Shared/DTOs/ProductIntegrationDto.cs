using System.Text.Json.Serialization;

namespace Insurance.Shared.DTOs
{
    public class ProductIntegrationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("productTypeId")]
        public int ProductTypeId { get; set; }
        [JsonPropertyName("salesPrice")]
        public float SalesPrice { get; set; }
    }
}
