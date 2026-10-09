namespace SecurityApplicationPresentation.Bootstrap
{
    public static class ApplicationBootstrapExtensions
    {
        public static async Task SeedRolesAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider
                .GetRequiredService<SecurityApplicationLogic.Interfaces.IRoleSeeder>();
            await seeder.SeedAsync();
        }
    }
}
