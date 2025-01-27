using Microsoft.EntityFrameworkCore;

namespace Stocks.Data
{

    [Keyless]

    public class ProductStock
    {
        public int ProductId { get; set; }
        public int Stock { get; set; }
    }
}
