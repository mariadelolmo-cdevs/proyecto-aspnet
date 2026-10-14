using Microsoft.EntityFrameworkCore;
using proyecto_aspnet.Models;

namespace proyecto_aspnet.Data;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // aplica  migraciones pendientes al arrancar la app
        context.Database.Migrate();

        // DATOS DEMO
        
    }
}
