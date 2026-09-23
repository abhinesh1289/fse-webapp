using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Assest> Assets { get; set; }
            = new List<Assest>();
    }
}
