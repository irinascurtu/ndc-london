using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stocks.Data;

public interface IProductStockRepository
{
    Task<IEnumerable<ProductStock>> GetProductStocksAsync(List<int> productIds);
}

public class ProductStockRepository : IProductStockRepository
{
    private readonly StockContext stockContext;

    public ProductStockRepository(StockContext stockContext)
    {
        this.stockContext = stockContext;
    }
    public async Task<IEnumerable<ProductStock>> GetProductStocksAsync(List<int> productIds)
    {
        // Generate the SQL parameter placeholders (e.g., @p0, @p1, @p2)
        var parameters = productIds.Select((id, index) => $"@p{index}").ToArray();
        var inClause = string.Join(",", parameters);

        // Construct the raw SQL query
        var sql = $"SELECT Id as ProductId, Stock FROM Products WHERE Id IN ({inClause})";

        // Generate the SqlParameter array
        var sqlParameters = productIds.Select((id, index) => new SqlParameter($"@p{index}", id)).ToArray();

        // Execute the raw SQL query
        var stocks = stockContext.ProductStocks
                                .FromSqlRaw(sql, sqlParameters)
                                .ToList();

        return stocks;
    }
}