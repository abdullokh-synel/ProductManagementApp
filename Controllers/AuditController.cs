using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagementApp.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController(ILogger<ProductController> logger, IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAudit([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var audit = await productService.GetAudit(from, to);
            return Ok(audit);
        }
    }
}