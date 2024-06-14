using Microsoft.EntityFrameworkCore;
using POC_Bangchak.Models;

namespace POC_Bangchak.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

        //public DbSet<Product> Products { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
