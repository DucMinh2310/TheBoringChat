namespace BackEnd.Context;

public class EFContext(DbContextOptions<EFContext> options) : DbContext(options)
{
    public virtual DbSet<Users> Users { get; set; }
    // public virtual DbSet<Friends> Friends { get; set; }
}