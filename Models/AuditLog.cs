using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Models
{
    public class AuditLogEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
