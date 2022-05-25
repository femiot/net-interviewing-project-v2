namespace Insurance.Core.Interfaces.Entities
{
    public class ProductTypeSurchargeCost : BaseEntity
    {
        public int ProductTypeId { get; set; }
        public float Rate { get; set; }
    }
}
