using Makers.Database.Entities;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Makers.Controllers
{
    public partial class DashboardController
    {
        [HttpPost]
        public IActionResult GetServices([FromBody] JObject reqBody)
        {
            //var PageNumber = reqBody.GetParameter<int>("PageNumber");
            //var PageSize = reqBody.GetParameter<int>("PageSize");
            //var Filter = reqBody.GetParameter<string>("Filter");

            var data = from e in db.T_SERVICES
                       orderby e.ID descending
                       select new
                       {
                           e.ID,
                           e.IS_ACTIVE,
                           e.NAMEX,
                           e.OVERVIEW,
                           IMG = BaseURL + "/Image/" + e.IMG,
                           e.PRICEX,
                           e.FILEX,
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
        public IActionResult GetService([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var data = from e in db.T_SERVICES
                       where e.ID == Id
                       select e;

            return this.Response(null, data);
        }

        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] JObject reqBody)
        {
            var Namex = reqBody.GetParameter<string>("NAMEX");
            var Overview = reqBody.GetParameter<string>("OVERVIEW");
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

            T_SERVICES newService = new()
            {
                ID = null,
                INSDATE = DateTime.Now,
                LUPDATE = DateTime.Now,
                NAMEX = Namex,
                OVERVIEW = Overview,
                PRICEX = Pricex,
                IS_ACTIVE = Constants.No
            };


            if (!string.IsNullOrWhiteSpace(Img) && Img != "undefined")
            {

                fileManager.UploadImage(db, Img, (int)newService.ID, "SRVC_IMG");
            }

            if (!string.IsNullOrWhiteSpace(Filex) && Filex != "undefined")
            {
                fileManager.UploadFile(db, Filex, (int)newService.ID, "SRVC_FILE");
            }


            await db.T_SERVICES.AddAsync(newService);

            await db.SaveChangesAsync();

            await db.AuditAsync(jwt, Constants.AuditActionInsert, newService, $"Service NAMEX: {newService.NAMEX}", true);

            return this.Response("Service added successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> EditService([FromBody] JObject reqBody)
        {
            var EditEntityId = reqBody.GetParameter<int>("EditEntityId");
            var Namex = reqBody.GetParameter<string>("NAMEX");
            var Overview = reqBody.GetParameter<string>("OVERVIEW");
            var Pricex = reqBody.Value<string>("PRICEX").ParseStringToNullableInt();
            var Filex = reqBody.Value<string>("FILEX");
            var Img = reqBody.Value<string>("IMG");

            if (Pricex is not null)
            {
                if (Pricex / 10000 < 1)
                {
                    throw new Exception("Price should not be less than 10000 IQD");
                }
            }

            var Service = db.T_SERVICES.First(e => e.ID == EditEntityId);

            Service.NAMEX = Namex;
            Service.OVERVIEW = Overview;
            Service.PRICEX = Pricex;
            Service.LUPDATE = DateTime.Now;


            if (!string.IsNullOrWhiteSpace(Img) && Img != "undefined")
            {
                fileManager.RemoveFile("Image", Service.IMG);

                fileManager.UploadImage(db, Img, (int)Service.ID, "SRVC_IMG");
            }

            if (!string.IsNullOrWhiteSpace(Filex) && Filex != "undefined")
            {
                fileManager.RemoveFile("Image", Service.IMG);

                fileManager.UploadFile(db, Filex, (int)Service.ID, "SRVC_FILE");
            }

            db.T_SERVICES.Update(Service);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Service, $"Service NAMEX: {Service.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Service updated successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");
            var Service = db.T_SERVICES.First(e => e.ID == Id);
            db.T_SERVICES.Remove(Service);

            await db.AuditAsync(jwt, Constants.AuditActionDelete, Service, $"Service NAMEX: {Service.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Service deleted successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeServiceStatus([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var Service = db.T_SERVICES.First(e => e.ID == Id);

            if (Service.IS_ACTIVE == Constants.Yes)
            {
                Service.IS_ACTIVE = Constants.No;
            }
            else
            {
                Service.IS_ACTIVE = Constants.Yes;
            }
            db.T_SERVICES.Update(Service);

            await db.AuditAsync(jwt, Constants.AuditActionUpdate, Service, $"Service NAMEX: {Service.NAMEX}");

            await db.SaveChangesAsync();

            return this.Response("Service changes status successfully", null);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadServicesFile([FromBody] JObject reqBody)
        {
            var Id = reqBody.GetParameter<int>("Id");

            var data = (from e in db.T_SERVICES
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
}

