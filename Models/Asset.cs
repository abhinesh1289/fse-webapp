namespace Inventory_Managgement1.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AssignedTo { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Category Category { get; set; } = null!;
    }
}
