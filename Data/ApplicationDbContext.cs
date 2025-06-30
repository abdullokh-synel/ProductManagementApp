using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManagementApp.Models;
using System;

namespace ProductManagementApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductAudit> ProductAudits { get; set; }

    public override int SaveChanges()
    {
        var modifiedEntries = ChangeTracker.Entries<Product>()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entry in modifiedEntries)
        {
            var audit = new ProductAudit
            {
                ProductTitle = entry.Entity.Title,
                ChangedBy = "Unknown",
                ChangedAt = DateTime.UtcNow,
                ChangeType = entry.State.ToString(),
                OriginalData = System.Text.Json.JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p])),
                NewData = entry.State == EntityState.Modified ? System.Text.Json.JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p])) : null
            };
            ProductAudits.Add(audit);
        }

        return base.SaveChanges();
    }
}
