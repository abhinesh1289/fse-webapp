using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Models
{
    public class Organisation
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? Phone { get; set; }
        [StringLength(150)]
        public string? Email { get; set; }
        [StringLength(250)]
        public string? Website { get; set; }
        [StringLength(500)]
        public string? LogoPath { get; set; }
    }
}
