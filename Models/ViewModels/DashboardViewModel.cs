namespace InventoryManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public string? UserName { get; set; }
        public string? UserRole { get; set; }
        public int TotalAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int MaintenanceAssets { get; set; }
        public int DamagedAssets { get; set; }
        public int RetiredAssets { get; set; }
        public List<CategorySummary> CategorySummary { get; set; }
            = new();
        public List<StatusSummary> StatusSummary { get; set; }
            = new();
        public List<RecentActivity> RecentActivities { get; set; }
            = new();
    }
    public class CategorySummary
    {
        public string CategoryName { get; set; }
            = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; }
            = "#94a3b8";
    }
    public class StatusSummary
    {
        public string Status { get; set; }
            = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; }
            = "#94a3b8";
    }
    public class RecentActivity
    {
        public string Description { get; set; }
            = string.Empty;
        public string Time { get; set; }
            = string.Empty;
        public string Icon { get; set; }
            = "bi-clock-history";
    }
}
