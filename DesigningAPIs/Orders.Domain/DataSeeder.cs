using OrdersApi.Data;


namespace ProductsApi.Data
{
    public static class DataSeeder
    {
        public static void SeedData(OrderContext _context)
        {
            if (!_context.Orders.Any())
            {
              //  _context.Orders.AddRange(LoadProducts());
                _context.SaveChanges();
            }
           
        }


    }
}
