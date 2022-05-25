namespace Insurance.Core.Interfaces.Entities
{
    public class InsuranceExtraCost : BaseEntity
    {
        public string ProductName { get; set; } = null!;
        public float ExtraCost { get; set; }
        public bool ApplyCostRangeRule { set; get; }
    }
}
