using Microsoft.EntityFrameworkCore;
using Vue_Project.Server.Models.Tables;

namespace Vue_Project.Server.API.Context;

public class EfContext : DbContext
{
    public EfContext(DbContextOptions<EfContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
