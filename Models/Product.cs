using System.ComponentModel.DataAnnotations;

namespace ProductManagementApp.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
