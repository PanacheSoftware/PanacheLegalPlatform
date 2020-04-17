using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Identity;
using System;

namespace PanacheSoftware.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<IdentityTenant> IdetityTenants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            //We have renamed all the tables from AspNet* to Identity*
            
            //We have specified a seperate ApplicationUser so we change the table name on that entity instead
            //builder.Entity<IdentityUser>(b =>
            //{
            //    b.ToTable("IdentityUsers");
            //});

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("IdentityUsers");
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.ToTable("IdentityUserClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.ToTable("IdentityUserLogins");
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.ToTable("IdentityUserTokens");
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.ToTable("IdentityRoles");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.ToTable("IdentityRoleClaims");
            });

            builder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.ToTable("IdentityUserRoles");
            });

            builder.ApplyConfiguration(new IdentityTenantsConfiguration());
        }
    }
}
