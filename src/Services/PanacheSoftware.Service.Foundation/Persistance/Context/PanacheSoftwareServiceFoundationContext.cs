using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Language;
using PanacheSoftware.Core.Domain.Settings;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Foundation.Persistance.EntityConfiguration;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new LanguageHeaderConfiguration(_userProvider.GetTenantId()));
            modelBuilder.ApplyConfiguration(new LanguageCodeConfiguration(_userProvider.GetTenantId()));
            modelBuilder.ApplyConfiguration(new LanguageItemConfiguration(_userProvider.GetTenantId()));

            modelBuilder.ApplyConfiguration(new SettingHeaderConfiguration(_userProvider.GetTenantId()));
            modelBuilder.ApplyConfiguration(new UserSettingConfiguration(_userProvider.GetTenantId()));
        }
    }
}
