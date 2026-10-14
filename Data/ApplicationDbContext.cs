using Microsoft.EntityFrameworkCore;

namespace proyecto_aspnet.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // TABLAS
    
}
