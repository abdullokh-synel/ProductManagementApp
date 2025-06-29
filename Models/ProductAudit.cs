namespace ProductManagementApp.Models;

public class ProductAudit
{
    public int Id { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? OriginalData { get; set; }
    public string? NewData { get; set; }
}
