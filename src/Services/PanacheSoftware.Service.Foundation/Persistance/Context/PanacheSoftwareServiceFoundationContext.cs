using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration;
using System;

namespace PanacheSoftware.Service.Foundation.Persistance.Context
{
    public class PanacheSoftwareServiceFoundationContext : DbContext
    {
        private readonly IUserProvider _userProvider;

        public PanacheSoftwareServiceFoundationContext(DbContextOptions<PanacheSoftwareServiceFoundationContext> options, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
        }

        public virtual DbSet<LanguageHeader> LanguageHeaders { get; set; }
        public virtual DbSet<LanguageCode> LanguageCodes { get; set; }
        public virtual DbSet<LanguageItem> LanguageItems { get; set; }

        public virtual DbSet<SettingHeader> SettingHeaders { get; set; }
        public virtual DbSet<UserSetting> UserSettings { get; set; }
        public virtual DbSet<TenantSetting> TenantSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new LanguageHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageCodeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageItemConfiguration());
            modelBuilder.ApplyConfiguration(new SettingHeaderConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingConfiguration());
            modelBuilder.ApplyConfiguration(new TenantSettingConfiguration());

            modelBuilder.Entity<LanguageHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()) || EF.Property<Guid>(e, "TenantId") == Guid.Empty);
            modelBuilder.Entity<LanguageCode>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()) || EF.Property<Guid>(e, "TenantId") == Guid.Empty);
            modelBuilder.Entity<LanguageItem>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<SettingHeader>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()) || EF.Property<Guid>(e, "TenantId") == Guid.Empty);
            modelBuilder.Entity<UserSetting>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
            modelBuilder.Entity<TenantSetting>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(_userProvider.GetTenantId()));
        }
    }
}
