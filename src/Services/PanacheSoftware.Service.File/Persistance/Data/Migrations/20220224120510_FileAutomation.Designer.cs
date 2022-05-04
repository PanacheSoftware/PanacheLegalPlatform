﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PanacheSoftware.Service.File.Persistance.Context;

namespace PanacheSoftware.Service.File.Persistance.Data.Migrations
{
    [DbContext(typeof(PanacheSoftwareServiceFileContext))]
    [Migration("20220224120510_FileAutomation")]
    partial class FileAutomation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileDetail", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<bool>("Automated")
                        .HasColumnType("bit");

                b.Property<Guid>("CreatedBy");

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(4000)");

                b.Property<string>("FileExtension")
                    .HasColumnType("nvarchar(1000)");

                b.Property<Guid>("FileHeaderId");

                b.Property<string>("FileTitle")
                    .HasColumnType("nvarchar(1000)");

                b.Property<string>("FileType")
                    .HasColumnType("nvarchar(1000)");

                b.Property<Guid>("LastUpdateBy");

                b.Property<DateTime>("LastUpdateDate");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("nvarchar(25)");

                b.Property<Guid>("TenantId");

                b.HasKey("Id");

                b.HasIndex("FileHeaderId")
                    .IsUnique();

                b.ToTable("FileDetail");
            });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileHeader", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<Guid>("CreatedBy");

                b.Property<DateTime>("CreatedDate");

                b.Property<Guid>("LastUpdateBy");

                b.Property<DateTime>("LastUpdateDate");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("nvarchar(25)");

                b.Property<Guid>("TenantId");

                b.HasKey("Id");

                b.ToTable("FileHeader");
            });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileLink", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<Guid>("CreatedBy");

                b.Property<DateTime>("CreatedDate");

                b.Property<Guid>("FileHeaderId");

                b.Property<Guid>("LastUpdateBy");

                b.Property<DateTime>("LastUpdateDate");

                b.Property<Guid>("LinkId");

                b.Property<string>("LinkType")
                    .IsRequired()
                    .HasColumnType("nvarchar(1000)");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("nvarchar(25)");

                b.Property<Guid>("TenantId");

                b.HasKey("Id");

                b.HasIndex("FileHeaderId");

                b.ToTable("FileLink");
            });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileVersion", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<byte[]>("Content");

                b.Property<Guid>("CreatedBy");

                b.Property<DateTime>("CreatedDate");

                b.Property<Guid>("FileHeaderId");

                b.Property<Guid>("LastUpdateBy");

                b.Property<DateTime>("LastUpdateDate");

                b.Property<long>("Size")
                    .HasColumnType("bigint");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("nvarchar(25)");

                b.Property<Guid>("TenantId");

                b.Property<string>("TrustedName")
                    .HasColumnType("nvarchar(1000)");

                b.Property<string>("URI")
                    .HasColumnType("nvarchar(1000)");

                b.Property<string>("UntrustedName")
                    .HasColumnType("nvarchar(1000)");

                b.Property<DateTime>("UploadDate");

                b.Property<int>("VersionNumber")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("FileHeaderId");

                b.ToTable("FileVersion");
            });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileDetail", b =>
            {
                b.HasOne("PanacheSoftware.Core.Domain.File.FileHeader", "FileHeader")
                    .WithOne("FileDetail")
                    .HasForeignKey("PanacheSoftware.Core.Domain.File.FileDetail", "FileHeaderId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileLink", b =>
            {
                b.HasOne("PanacheSoftware.Core.Domain.File.FileHeader", "FileHeader")
                    .WithMany("FileLinks")
                    .HasForeignKey("FileHeaderId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.File.FileVersion", b =>
            {
                b.HasOne("PanacheSoftware.Core.Domain.File.FileHeader", "FileHeader")
                    .WithMany("FileVersions")
                    .HasForeignKey("FileHeaderId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
        }
    }
}
