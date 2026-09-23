using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Models
{
    public class Assest
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Asset Code")]
        public string AssestCode { get; set; } = string.Empty;
        [Required]
        [StringLength(150)]
        [Display(Name = "Asset Name")]
        public string AssestName { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PurchaseCost { get; set; }
        [StringLength(150)]
        public string? Location { get; set; }
        [StringLength(150)]
        public string? Department { get; set; }
        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Available";
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
