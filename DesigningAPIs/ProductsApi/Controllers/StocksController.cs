using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ProductsApi.Service;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IProductService productService;

        public StocksController(IProductService productService)
        {
            this.productService = productService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetProductsStocks([FromQuery] List<int> productIds)
        {
            var products = await productService.GetProductStocksAsync(productIds);
            return Ok(products);
            ///TODO: add mappings and use the Model
            ///
        }

    }
}
