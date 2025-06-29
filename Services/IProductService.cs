using ProductManagementApp.Models;
using ProductManagementApp.ViewModels;

public interface IProductService
{
    Task<List<ProductViewModel>> GetAllProducts();
    Task<Product?> GetProductById(int id);
    Task CreateProduct(Product product);
    Task UpdateProduct(int id, Product product);
    Task DeleteProduct(int id);
    Task<IList<ProductAudit>> GetAudit(DateTime? from, DateTime? to);
}
