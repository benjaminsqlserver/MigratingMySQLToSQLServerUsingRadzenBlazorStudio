using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using ProductCatalogue.Data;

namespace ProductCatalogue
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ConDataContext context;
        private readonly NavigationManager navigationManager;

        public ConDataService(ConDataContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportProductsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductsRead(ref IQueryable<ProductCatalogue.Models.ConData.Product> items);

        public async Task<IQueryable<ProductCatalogue.Models.ConData.Product>> GetProducts(Query query = null)
        {
            var items = Context.Products.AsQueryable();

            items = items.Include(i => i.Productcategory);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductGet(ProductCatalogue.Models.ConData.Product item);
        partial void OnGetProductByProductId(ref IQueryable<ProductCatalogue.Models.ConData.Product> items);


        public async Task<ProductCatalogue.Models.ConData.Product> GetProductByProductId(int productid)
        {
            var items = Context.Products
                              .AsNoTracking()
                              .Where(i => i.product_id == productid);

            items = items.Include(i => i.Productcategory);
 
            OnGetProductByProductId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCreated(ProductCatalogue.Models.ConData.Product item);
        partial void OnAfterProductCreated(ProductCatalogue.Models.ConData.Product item);

        public async Task<ProductCatalogue.Models.ConData.Product> CreateProduct(ProductCatalogue.Models.ConData.Product product)
        {
            OnProductCreated(product);

            var existingItem = Context.Products
                              .Where(i => i.product_id == product.product_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(product).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCreated(product);

            return product;
        }

        public async Task<ProductCatalogue.Models.ConData.Product> CancelProductChanges(ProductCatalogue.Models.ConData.Product item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductUpdated(ProductCatalogue.Models.ConData.Product item);
        partial void OnAfterProductUpdated(ProductCatalogue.Models.ConData.Product item);

        public async Task<ProductCatalogue.Models.ConData.Product> UpdateProduct(int productid, ProductCatalogue.Models.ConData.Product product)
        {
            OnProductUpdated(product);

            var itemToUpdate = Context.Products
                              .Where(i => i.product_id == product.product_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(product);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductUpdated(product);

            return product;
        }

        partial void OnProductDeleted(ProductCatalogue.Models.ConData.Product item);
        partial void OnAfterProductDeleted(ProductCatalogue.Models.ConData.Product item);

        public async Task<ProductCatalogue.Models.ConData.Product> DeleteProduct(int productid)
        {
            var itemToDelete = Context.Products
                              .Where(i => i.product_id == productid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductDeleted(itemToDelete);


            Context.Products.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductcategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/productcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/productcategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductcategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/productcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/productcategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductcategoriesRead(ref IQueryable<ProductCatalogue.Models.ConData.Productcategory> items);

        public async Task<IQueryable<ProductCatalogue.Models.ConData.Productcategory>> GetProductcategories(Query query = null)
        {
            var items = Context.Productcategories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductcategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductcategoryGet(ProductCatalogue.Models.ConData.Productcategory item);
        partial void OnGetProductcategoryByCategoryId(ref IQueryable<ProductCatalogue.Models.ConData.Productcategory> items);


        public async Task<ProductCatalogue.Models.ConData.Productcategory> GetProductcategoryByCategoryId(int categoryid)
        {
            var items = Context.Productcategories
                              .AsNoTracking()
                              .Where(i => i.category_id == categoryid);

 
            OnGetProductcategoryByCategoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductcategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductcategoryCreated(ProductCatalogue.Models.ConData.Productcategory item);
        partial void OnAfterProductcategoryCreated(ProductCatalogue.Models.ConData.Productcategory item);

        public async Task<ProductCatalogue.Models.ConData.Productcategory> CreateProductcategory(ProductCatalogue.Models.ConData.Productcategory productcategory)
        {
            OnProductcategoryCreated(productcategory);

            var existingItem = Context.Productcategories
                              .Where(i => i.category_id == productcategory.category_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Productcategories.Add(productcategory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(productcategory).State = EntityState.Detached;
                throw;
            }

            OnAfterProductcategoryCreated(productcategory);

            return productcategory;
        }

        public async Task<ProductCatalogue.Models.ConData.Productcategory> CancelProductcategoryChanges(ProductCatalogue.Models.ConData.Productcategory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductcategoryUpdated(ProductCatalogue.Models.ConData.Productcategory item);
        partial void OnAfterProductcategoryUpdated(ProductCatalogue.Models.ConData.Productcategory item);

        public async Task<ProductCatalogue.Models.ConData.Productcategory> UpdateProductcategory(int categoryid, ProductCatalogue.Models.ConData.Productcategory productcategory)
        {
            OnProductcategoryUpdated(productcategory);

            var itemToUpdate = Context.Productcategories
                              .Where(i => i.category_id == productcategory.category_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(productcategory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductcategoryUpdated(productcategory);

            return productcategory;
        }

        partial void OnProductcategoryDeleted(ProductCatalogue.Models.ConData.Productcategory item);
        partial void OnAfterProductcategoryDeleted(ProductCatalogue.Models.ConData.Productcategory item);

        public async Task<ProductCatalogue.Models.ConData.Productcategory> DeleteProductcategory(int categoryid)
        {
            var itemToDelete = Context.Productcategories
                              .Where(i => i.category_id == categoryid)
                              .Include(i => i.Products)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductcategoryDeleted(itemToDelete);


            Context.Productcategories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductcategoryDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}