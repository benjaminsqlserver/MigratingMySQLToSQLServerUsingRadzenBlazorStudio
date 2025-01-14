﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductCatalogue.Data;

#nullable disable

namespace ProductCatalogue.Migrations
{
    [DbContext(typeof(ConDataContext))]
    [Migration("20240410123939_PortToSqlServer")]
    partial class PortToSqlServer
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductCatalogue.Models.ConData.Product", b =>
                {
                    b.Property<int>("product_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("product_id"));

                    b.Property<int?>("category_id")
                        .IsConcurrencyToken()
                        .HasColumnType("int");

                    b.Property<decimal>("price")
                        .IsConcurrencyToken()
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("product_name")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("product_id");

                    b.HasIndex("category_id");

                    b.ToTable("product");
                });

            modelBuilder.Entity("ProductCatalogue.Models.ConData.Productcategory", b =>
                {
                    b.Property<int>("category_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("category_id"));

                    b.Property<string>("category_name")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("category_id");

                    b.ToTable("productcategory");
                });

            modelBuilder.Entity("ProductCatalogue.Models.ConData.Product", b =>
                {
                    b.HasOne("ProductCatalogue.Models.ConData.Productcategory", "Productcategory")
                        .WithMany("Products")
                        .HasForeignKey("category_id");

                    b.Navigation("Productcategory");
                });

            modelBuilder.Entity("ProductCatalogue.Models.ConData.Productcategory", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
