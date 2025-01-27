using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Stocks.Data
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {
        }

        public DbSet<ProductStock> ProductStocks { get; set; }
    }
}
