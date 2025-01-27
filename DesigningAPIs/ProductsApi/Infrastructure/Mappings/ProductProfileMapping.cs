using AutoMapper;
using ProductsApi.Data.Entities;
using ProductsApi.Models;

namespace ProductsApi.Infrastructure.Mappings
{
    public class ProductProfileMapping: Profile
    {
        public ProductProfileMapping()
        {
            CreateMap<Product, Models.ProductModel>();
            CreateMap<Product, Models.ProductTrimmedModel>();
            CreateMap<ProductModel, Product>();
        }
    }
}
