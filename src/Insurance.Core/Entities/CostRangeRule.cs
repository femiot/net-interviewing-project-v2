namespace Insurance.Core.Interfaces.Entities
{
    public class CostRangeRule : BaseEntity
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public float Value { get; set; }
        public bool IgnoreMax { get; set; }
    }
}
