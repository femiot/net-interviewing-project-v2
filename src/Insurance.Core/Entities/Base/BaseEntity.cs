namespace Insurance.Core.Interfaces.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string DateCreated { get; set; } = null!;
        public string CreatedByUserId { get; set; } = null!;
        public string? DateModified { get; set; } 
        public string? LastUpdatedByUserId { get; set; }
    }
}
