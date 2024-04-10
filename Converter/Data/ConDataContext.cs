using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Models.ConData;

namespace ProductCatalogue.Data
{
    public partial class ConDataContext : DbContext
    {
        public ConDataContext()
        {
        }

        public ConDataContext(DbContextOptions<ConDataContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductCatalogue.Models.ConData.Product>()
              .HasOne(i => i.Productcategory)
              .WithMany(i => i.Products)
              .HasForeignKey(i => i.category_id)
              .HasPrincipalKey(i => i.category_id);

            builder.Entity<ProductCatalogue.Models.ConData.Product>()
              .Property(p => p.price)
              .HasPrecision(10,2);
            this.OnModelBuilding(builder);
        }

        public DbSet<ProductCatalogue.Models.ConData.Product> Products { get; set; }

        public DbSet<ProductCatalogue.Models.ConData.Productcategory> Productcategories { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());

        }

    }
}