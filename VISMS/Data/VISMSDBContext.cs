using Microsoft.EntityFrameworkCore;
using VISMS.Models;

namespace VISMS.Data
{
    public class VISMSDBContext : DbContext
    {
        public VISMSDBContext(DbContextOptions<VISMSDBContext> options)
            : base(options)
        {
        }

        public DbSet<VISMS_ServiceList> VISMS_ServiceList { get; set; }

        public DbSet<VISMS_AllowedServerList> VISMS_AllowedServerList { get; set; }

        public DbSet<VISMS_UserList> VISMS_UserList { get; set; }

        public DbSet<VISMS_ProductList> VISMS_ProductList { get; set; }

        public DbSet<VISMS_PurchaseLog> VISMS_PurchaseLog { get; set; }
    }
}