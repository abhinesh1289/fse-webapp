using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystem.Models
{
    public class Permission
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
