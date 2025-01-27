﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductsApi.Data.Entities;
using ProductsApi.Models;
using ProductsApi.Service;
using System.Text.Json;

namespace ProductsApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/products")]
    [Asp.Versioning.ApiVersion("1")]
    [ApiVersion("1")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper mapper;

        private readonly ILogger<ProductsController> logger;
        private readonly IMemoryCache memoryCache;
        private const string LimitedStockProductsKey = "LSPC";


        private readonly IDistributedCache distributedCache;
        private const string OverstockedProductsKey = "OSPK";

        public ProductsController(IProductService productService, IMapper mapper,
            ILogger<ProductsController> logger, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _productService = productService;
            this.mapper = mapper;
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.distributedCache = distributedCache;
        }


        [HttpGet]
        //[ResponseCache(Duration = 5, // Cache-Control: max-age=5
        //  Location = ResponseCacheLocation.Any, // Cache-Control: public
        //  VaryByHeader = "User-Agent" // Vary: User-Agent
        //  )]
        [Produces("application/vnd.example.v1+json")]
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
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow,
                    // SlidingExpiration = TimeSpan.FromSeconds(5),
                    Size = cachedValue?.Length
                };

                memoryCache.Set(LimitedStockProductsKey, cachedValue, cacheEntryOptions);
            }
            MemoryCacheStatistics? stats = memoryCache.GetCurrentStatistics();
            logger.LogInformation($"Memory cache. Total hits: {stats?
                  .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");
            return cachedValue ?? Enumerable.Empty<Product>();
        }

        private Product[]? GetOverStockedProductsFromDb()
        {
            // If the cached value is not found, get the value from the database.
            var products = _productService.GetProductsAsync().Result;
            var cachedValue = products.Where(p => p.Stock > 100)
                .ToArray();

            DistributedCacheEntryOptions cacheEntryOptions = new()
            {
                // Allow readers to reset the cache entry's lifetime.
                SlidingExpiration = TimeSpan.FromSeconds(5),
                // Set an absolute expiration time for the cache entry.
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
            };

            byte[]? cachedValueBytes =
              JsonSerializer.SerializeToUtf8Bytes(cachedValue);

            distributedCache.Set(OverstockedProductsKey, cachedValueBytes, cacheEntryOptions);
            return cachedValue;
        }


        [HttpGet]
        [Route("overstocked")]
        [Produces(typeof(Product[]))]
        public async Task<IEnumerable<Product>> GetOverStockedProducts()
        {
            // Try to get the cached value.
            byte[]? cachedValueBytes = distributedCache.Get(OverstockedProductsKey);
            Product[]? cachedValue = null;
            if (cachedValueBytes is null)
            {
                cachedValue = GetOverStockedProductsFromDb();
            }
            else
            {
                cachedValue = JsonSerializer
                  .Deserialize<Product[]?>(cachedValueBytes);
                if (cachedValue is null)
                {
                    cachedValue = GetOverStockedProductsFromDb();
                }
            }
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
