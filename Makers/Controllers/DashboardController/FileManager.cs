//using Makers.Utilities;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json.Linq;
//using StatusCodes = Makers.Utilities.StatusCodes;

//namespace Makers.Controllers;

//public partial class DashboardController
//{
//    [HttpPost]
//    public async Task<IActionResult> UploadImage(IFormFile Image, string reqBody)
//    {
//        var Body = JObject.Parse(reqBody);
//        var RefId = Body.GetParameter<int>("REF_ID");
//        var RefType = Body.GetParameter<string>("REF_TYPE");

//        var result = fileManager.SaveFile(Image, "Image", false);

//        if (result.Item1)
//        {
//            switch (RefType)
//            {
//                case "INST":
//                    var inst = db.T_INST.First(e => e.ID == RefId);
//                    inst.LOGO = result.Item3;
//                    db.T_INST.Update(inst);
//                    break;
//                case "PROJ":
//                    var proj = db.T_PROJECTS.First(e => e.ID == RefId);
//                    proj.ICON = result.Item3;
//                    db.T_PROJECTS.Update(proj);
//                    break;
//                default:
//                    break;
//            }

//            await db.SaveChangesAsync();

//            return this.Response(result.Item2, result.Item3);
//        }

//        return this.Response(result.Item2, result.Item3, StatusCodes.Error);
//    }
//}

