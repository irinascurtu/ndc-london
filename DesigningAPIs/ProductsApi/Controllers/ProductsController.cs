using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductsApi.Data.Entities;
using ProductsApi.Models;
using ProductsApi.Service;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper mapper;

        private readonly ILogger<ProductsController> logger;
        private readonly IMemoryCache memoryCache;
        private const string LimitedStockProductsKey = "LSPC";

        public ProductsController(IProductService productService, IMapper mapper,
            ILogger<ProductsController> logger, IMemoryCache memoryCache)
        {
            _productService = productService;
            this.mapper = mapper;
            this.logger = logger;
            this.memoryCache = memoryCache;
        }


        [HttpGet]
        public async Task<ActionResult<List<ProductTrimmedModel>>> GetProducts()
        {
            var productsFromDb = await _productService.GetProductsAsync();
            var products = mapper.Map<List<ProductTrimmedModel>>(productsFromDb);

            return Ok(products);
        }


        // GET: api/products/limitedstock
        [HttpGet]
        [Route("limitedstock")]
        [Produces(typeof(Product[]))]
        public async Task<IEnumerable<Product>> GetLimitedStockProducts()
        {
            // Try to get the cached value.
            if (!memoryCache.TryGetValue(LimitedStockProductsKey, out Product[]? cachedValue))
            {


                // If the cached value is not found, get the value from the database.
                var products = await _productService.GetProductsAsync();
                cachedValue = products.Where(p => p.Stock <= 30)
                  .ToArray();


                MemoryCacheEntryOptions cacheEntryOptions = new()
                { //AbsoluteExpiration = DateTimeOffset.UtcNow,
                    SlidingExpiration = TimeSpan.FromSeconds(5),
                    Size = cachedValue?.Length
                };

                memoryCache.Set(LimitedStockProductsKey, cachedValue, cacheEntryOptions);
            }
            MemoryCacheStatistics? stats = memoryCache.GetCurrentStatistics();
            logger.LogInformation($"Memory cache. Total hits: {stats?
                  .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");
            return cachedValue ?? Enumerable.Empty<Product>();
        }


        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/Products/{id}
        [HttpHead("{id}")]
        public async Task<IActionResult> CheckIfProductExists(int id)
        {
            var product = await _productService.ProductExistsAsync(id);
            if (product == false)
            {
                return NotFound();
            }

            HttpContext.Response.Headers.Add("My-cool-header", "sdasdsada");
           
            return Ok("i found the product");
        }

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await _productService.UpdateProductAsync(product);
            }
            catch
            {
                if (!await _productService.ProductExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            var createdProduct = await _productService.AddProductAsync(product);
            return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
