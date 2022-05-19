using Newtonsoft.Json;

namespace Insurance.Shared.DTOs
{
    public class ProductIntegrationDto
    {
        [JsonProperty("productTypeId")]
        public int ProductTypeId { get; set; }
        [JsonProperty("salesPrice")]
        public float SalesPrice { get; set; }
    }
}
