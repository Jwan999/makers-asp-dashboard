using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers;

public partial class DashboardController
{
    [HttpPost]
    public IActionResult GetProducts([FromBody] JObject reqBody)
    {
        //var PageNumber = reqBody.GetParameter<int>("PageNumber");
        //var PageSize = reqBody.GetParameter<int>("PageSize");
        //var Filter = reqBody.GetParameter<string>("Filter");

        var data = from e in db.T_PRODUCTS
                   orderby e.ID descending
                   select new
                   {
                       e.ID,
                       e.IS_ACTIVE,
                       e.NAMEX,
                       e.OVERVIEW,
                       IMG = BaseURL + "/Image/" + e.IMG,
                       e.FILEX,
                       e.COUNT,
                       e.PRICEX,
                   };

        //if (!string.IsNullOrEmpty(Filter))
        //{
        //    data = data.Where(e => e.NAMEX.Contains(Filter)).OrderByDescending(e => e.ID);
        //}

        //var dataCount = data.Count();

        //var resultData = SecurityHelper.Paging(PageSize, dataCount, data.Skip((PageNumber - 1) * PageSize).Take(PageSize));

        return this.Response(null, data);
    }

    [HttpPost]
    public IActionResult GetProduct([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = from e in db.T_PRODUCTS
                   where e.ID == Id
                   select e;

        return this.Response(null, data);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] JObject reqBody)
    {
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Overview = reqBody.GetParameter<string>("OVERVIEW");
        var Count = reqBody.GetParameter<int>("COUNT");
        var Pricex = reqBody.Value<string>("PRICEX").ParseStringToNullableInt();
        var Filex = reqBody.Value<string>("FILEX");
        var Img = reqBody.Value<string>("IMG");

        if (Pricex is not null)
        {
            if (Pricex / 1000 < 1)
            {
                throw new Exception("Price should not be less than 1000 IQD");
            }
        }

        T_PRODUCTS newProduct = new()
        {
            ID = null,
            INSDATE = DateTime.Now,
            LUPDATE = DateTime.Now,
            NAMEX = Namex,
            OVERVIEW = Overview,
            COUNT = Count,
            PRICEX = Pricex,
            IS_ACTIVE = Constants.No
        };
        await db.T_PRODUCTS.AddAsync(newProduct);
        await db.SaveChangesAsync();


        if (!string.IsNullOrWhiteSpace(Img) && Img != "undefined")
        {

            fileManager.UploadImage(db, Img, (int)newProduct.ID, "PROD_IMG");
        }

        if (!string.IsNullOrWhiteSpace(Filex) && Filex != "undefined")
        {
            fileManager.UploadFile(db, Filex, (int)newProduct.ID, "PROD_FILE");
        }

        await db.SaveChangesAsync();

        await db.AuditAsync(jwt, Constants.AuditActionInsert, newProduct, $"Product NAMEX: {newProduct.NAMEX}", true);

        return this.Response("Product added successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct([FromBody] JObject reqBody)
    {
        var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
        var Namex = reqBody.GetParameter<string>("NAMEX");
        var Overview = reqBody.GetParameter<string>("OVERVIEW");
        var Count = reqBody.GetParameter<int>("COUNT");
        var Pricex = reqBody.Value<string>("PRICEX").ParseStringToNullableInt();
        var Filex = reqBody.Value<string>("FILEX");
        var Img = reqBody.Value<string>("IMG");


        if (Pricex is not null)
        {
            if (Pricex / 1000 < 1)
            {
                throw new Exception("Price should not be less than 1000 IQD");
            }
        }

        var Product = db.T_PRODUCTS.First(e => e.ID == EditEntityId);

        Product.NAMEX = Namex;
        Product.OVERVIEW = Overview;
        Product.COUNT = Count;
        Product.PRICEX = Pricex;
        Product.LUPDATE = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(Img) && Img != "undefined")
        {

            fileManager.RemoveFile("Image", Product.FILEX);

            fileManager.UploadImage(db, Img, (int)Product.ID, "PROD_IMG");
        }

        if (!string.IsNullOrWhiteSpace(Filex) && Filex != "undefined")
        {
            fileManager.RemoveFile("Files", Product.FILEX);

            fileManager.UploadFile(db, Filex, (int)Product.ID, "PROD_FILE");
        }

        db.T_PRODUCTS.Update(Product);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Product, $"Product NAMEX: {Product.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Product updated successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Product = db.T_PRODUCTS.First(e => e.ID == Id);

        db.T_PRODUCTS.Remove(Product);

        await db.AuditAsync(jwt, Constants.AuditActionDelete, Product, $"Product NAMEX: {Product.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Product deleted successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProductStatus([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var Product = db.T_PRODUCTS.First(e => e.ID == Id);

        if (Product.IS_ACTIVE == Constants.Yes)
        {
            Product.IS_ACTIVE = Constants.No;
        }
        else
        {
            Product.IS_ACTIVE = Constants.Yes;
        }

        db.T_PRODUCTS.Update(Product);

        await db.AuditAsync(jwt, Constants.AuditActionUpdate, Product, $"Product NAMEX: {Product.NAMEX}");

        await db.SaveChangesAsync();

        return this.Response("Product changes status successfully", null);
    }

    [HttpPost]
    public async Task<IActionResult> DownloadProductFile([FromBody] JObject reqBody)
    {
        var Id = reqBody.GetParameter<int>("Id");

        var data = (from e in db.T_PRODUCTS
                    where e.ID == Id
                    select e).First();

        if (data.FILEX is null)
        {
            throw new Exception("No file was found");
        }

        await db.AuditAsync(jwt, Constants.AuditActionDownload, $"Download {Id}", true);

        using (var client = new HttpClient())
        {
            var fileBytes = await client.GetByteArrayAsync(BaseURL + "/Files/" + data.FILEX);
            return File(fileBytes, "application/octet-stream", $"{data.FILEX}");
        }
    }
}