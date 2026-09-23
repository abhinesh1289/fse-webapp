using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Assest> Assets { get; set; }
            = new List<Assest>();
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
        public ICollection<AuditLogEntry> AuditLogs { get; set; }
            = new List<AuditLogEntry>();
    }
}
