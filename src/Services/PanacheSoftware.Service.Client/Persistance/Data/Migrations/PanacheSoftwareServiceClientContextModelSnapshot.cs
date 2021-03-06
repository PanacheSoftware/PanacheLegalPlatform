﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PanacheSoftware.Service.Client.Persistance.Context;

namespace PanacheSoftware.Service.Client.Persistance.Data.Migrations
{
    [DbContext(typeof(PanacheSoftwareServiceClientContext))]
    partial class PanacheSoftwareServiceClientContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "0.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressLine1")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("AddressLine3")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("AddressLine4")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("AddressLine5")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("AddressType")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("ClientContactId");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<string>("Email1")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Email2")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Email3")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Phone1")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Phone2")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Phone3")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Region")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("ClientContactId");

                    b.ToTable("ClientAddress");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientContact", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClientHeaderId");

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<bool>("MainContact")
                        .HasColumnType("bit");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("ClientHeaderId");

                    b.ToTable("ClientContact");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Base64Image")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<Guid>("ClientHeaderId");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<Guid>("LastUpdateBy");

                    b.Property<DateTime>("LastUpdateDate");

                    b.Property<string>("Region")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.Property<string>("url")
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("ClientHeaderId")
                        .IsUnique();

                    b.ToTable("ClientDetail");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientHeader", b =>
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

                    b.Property<string>("LongName")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("TenantId");

                    b.HasKey("Id");

                    b.ToTable("ClientHeader");
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientAddress", b =>
                {
                    b.HasOne("PanacheSoftware.Core.Domain.Client.ClientContact", "ClientContact")
                        .WithMany("ClientAddresses")
                        .HasForeignKey("ClientContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientContact", b =>
                {
                    b.HasOne("PanacheSoftware.Core.Domain.Client.ClientHeader", "ClientHeader")
                        .WithMany("ClientContacts")
                        .HasForeignKey("ClientHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PanacheSoftware.Core.Domain.Client.ClientDetail", b =>
                {
                    b.HasOne("PanacheSoftware.Core.Domain.Client.ClientHeader", "ClientHeader")
                        .WithOne("ClientDetail")
                        .HasForeignKey("PanacheSoftware.Core.Domain.Client.ClientDetail", "ClientHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
