using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductsApi.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/products")]
    [ApiVersion("2")]
    [Asp.Versioning.ApiVersion("2")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        [HttpGet]
        [Produces("application/vnd.example.v2+json")]
        public IActionResult Get()
        {
            return Ok("Version 2");
        }
    }
}
