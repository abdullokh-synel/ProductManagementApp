using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductManagementApp.Data;
using ProductManagementApp.Models;
using ProductManagementApp.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProductManagementApp.Services
{
    public class ProductService(ApplicationDbContext context, IOptions<VatOptions> vatOptions, IHttpContextAccessor httpContextAccessor) : IProductService
    {
        public async Task<List<ProductViewModel>> GetAllProducts()
        {
            var products = await context.Products.ToListAsync();

            return [.. products.Select(product => new ProductViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Quantity = product.Quantity,
                Price = product.Price,
                TotalPriceWithVAT = VatCalculator.CalculateTotalWithVat(product.Quantity, product.Price, vatOptions.Value.VatRate)
            })];
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await context.Products.FindAsync(id);
        }

        public async Task CreateProduct(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            await AuditChangeAsync(product.Title, GetUserId(), "Created", null, product);
        }

        public async Task UpdateProduct(int id, Product product)
        {
            var existing = await GetProductById(id)
                   ?? throw new InvalidOperationException($"Product with ID {id} not found.");

            var original = new Product
            {
                Id = existing.Id,
                Title = existing.Title,
                Quantity = existing.Quantity,
                Price = existing.Price
            };

            existing.Title = product.Title;
            existing.Quantity = product.Quantity;
            existing.Price = product.Price;

            await context.SaveChangesAsync();

            await AuditChangeAsync(existing.Title, GetUserId(), "Updated", original, existing);
        }

        public async Task DeleteProduct(int id)
        {
            var product = await GetProductById(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();

                await AuditChangeAsync(product.Title, GetUserId(), "Deleted", product, null);
            }

        }

        public async Task<IList<ProductAudit>> GetAudit(DateTime? from, DateTime? to)
        {
            var query = context.ProductAudits.AsQueryable();

            if (from.HasValue)
                query = query.Where(a => a.ChangedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.ChangedAt <= to.Value);

            return await query
                .OrderByDescending(a => a.ChangedAt)
                .ToListAsync();
        }

        private async Task AuditChangeAsync(string productTitle, string changedByUserId, string changeType, object? originalData, object? newData)
        {
            var audit = new ProductAudit
            {
                ProductTitle = productTitle,
                ChangedBy = changedByUserId,
                ChangedAt = DateTime.UtcNow,
                ChangeType = changeType,
                OriginalData = originalData != null ? JsonSerializer.Serialize(originalData) : null,
                NewData = newData != null ? JsonSerializer.Serialize(newData) : null
            };

            context.ProductAudits.Add(audit);
            await context.SaveChangesAsync();
        }

        private string GetUserId()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new Exception("User is not authenticated");
        }
    }
}