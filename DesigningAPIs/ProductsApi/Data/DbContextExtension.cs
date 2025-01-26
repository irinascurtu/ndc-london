namespace ProductsApi.Data
{
    public static class DbContextExtension
    {

        public static void EnsureSeeded(this ProductContext context)
        {
            DataSeeder.SeedData(context);
        }

    }
}
