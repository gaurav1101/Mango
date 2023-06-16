using Mango.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Mango.Services.CouponAPI.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                CouponCode = "180FF",
                DiscountAmount = 10,
                MinimumAmount = 20

            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 2,
                CouponCode = "20OFF",
                DiscountAmount = 20,
                MinimumAmount = 40

            });
        }
    }
}
