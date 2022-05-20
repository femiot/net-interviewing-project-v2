using System.Text.Json.Serialization;

namespace Insurance.Shared.DTOs
{
    public class SurchargeRateDto
    {
        [JsonIgnore]
        public int ProductTypeId { get; set; }
        [JsonIgnore]
        public float SurchargeRate { get; set; }
    }
}
