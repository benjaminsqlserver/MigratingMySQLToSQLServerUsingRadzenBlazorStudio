using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using ProductCatalogue.Data;

namespace ProductCatalogue.Controllers
{
    public partial class ExportConDataController : ExportController
    {
        private readonly ConDataContext context;
        private readonly ConDataService service;

        public ExportConDataController(ConDataContext context, ConDataService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ConData/products/csv")]
        [HttpGet("/export/ConData/products/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProducts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/products/excel")]
        [HttpGet("/export/ConData/products/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProducts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/productcategories/csv")]
        [HttpGet("/export/ConData/productcategories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductcategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProductcategories(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/productcategories/excel")]
        [HttpGet("/export/ConData/productcategories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductcategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProductcategories(), Request.Query, false), fileName);
        }
    }
}
