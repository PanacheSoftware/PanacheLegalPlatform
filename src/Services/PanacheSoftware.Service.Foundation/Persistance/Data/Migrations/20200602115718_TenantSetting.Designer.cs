﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PanacheSoftware.Service.Foundation.Persistance.Context;

namespace PanacheSoftware.Service.Foundation.Persistance.Data.Migrations
{
    [DbContext(typeof(PanacheSoftwareServiceFoundationContext))]
    [Migration("20200602115718_TenantSetting")]
    partial class TenantSetting
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "0.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Language.LanguageCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("LanguageCodeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.HasKey("Id");

                    b.ToTable("LanguageCode");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Language.LanguageHeader", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<long>("TextCode")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("LanguageHeader");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Language.LanguageItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<string>("LanguageCodeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("LanguageHeaderId");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("LanguageHeaderId");

                    b.ToTable("LanguageItem");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Settings.SettingHeader", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DefaultValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("SettingType")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.ToTable("SettingHeader");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Settings.TenantSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("SettingHeaderId");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("SettingHeaderId");

                    b.ToTable("TenantSetting");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Settings.UserSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<Guid>("SettingHeaderId");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<Guid>("UserId");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("SettingHeaderId");

                    b.ToTable("UserSetting");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Language.LanguageItem", b =>
                {
                    b.HasOne("PanacheSoftware.Core.Domain.Language.LanguageHeader", "LanguageHeader")
                        .WithMany("LanguageItems")
                        .HasForeignKey("LanguageHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Settings.TenantSetting", b =>
                {
                    b.HasOne("PanacheSoftware.Core.Domain.Settings.SettingHeader", "SettingHeader")
                        .WithMany("TenantSettings")
                        .HasForeignKey("SettingHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Settings.UserSetting", b =>
                {
                    b.HasOne("PanacheSoftware.Core.Domain.Settings.SettingHeader", "SettingHeader")
                        .WithMany("UserSettings")
                        .HasForeignKey("SettingHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
