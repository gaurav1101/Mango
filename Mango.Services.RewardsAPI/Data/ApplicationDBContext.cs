using Mango.Services.RewardsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardsAPI.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Rewards> Rewards { get; set; }

    }
}
