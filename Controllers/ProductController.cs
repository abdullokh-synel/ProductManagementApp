using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementApp.Models;

namespace ProductManagementApp.Controllers;

[Authorize]
public class ProductController(ILogger<ProductController> logger, IProductService productService) : Controller
{
    [Authorize(Roles = "admin,user")]
    public async Task<IActionResult> Index()
    {
        var products = await productService.GetAllProducts();
        return View(products);
    }

    [Authorize(Roles = "admin,user")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize(Roles = "admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        await productService.CreateProduct(product);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int id)
    {
        return await GetProductById(id, "Edit");
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        await productService.UpdateProduct(id, product);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        return await GetProductById(id, "Delete");
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [Authorize(Roles = "admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await productService.DeleteProduct(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> GetProductById(int id, string viewName)
    {
        var product = await productService.GetProductById(id);
        return product == null ? NotFound() : View(viewName, product);
    }

}
