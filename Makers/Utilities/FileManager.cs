using Makers.Database.Contexts;

namespace Makers.Utilities;
using System;
using System.IO;
public interface IFileManager
{
    public (bool, string, string) SaveFile(IFormFile file, string directory, bool allowOverwrite);

    public (bool, string) RemoveFile(string directory = null, params string[] filenames);

    public (bool, string, string) UploadImage(Db db, string image, int refId, string refType);
    public (bool, string, string) UploadFile(Db db, string image, int refId, string refType);
}

public class FileManager : IFileManager
{
    private readonly string _wwwroot;
    public static readonly char _slash = Path.DirectorySeparatorChar;
    public FileManager()
    {
        _wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    }
    private bool _SaveFile(string filepath, IFormFile file)
    {
        try
        {
            var fileStream = new FileStream(filepath, FileMode.Create);
            file.CopyTo(fileStream);
            fileStream.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public (bool, string, string) SaveFile(IFormFile file, string directory = null, bool allowOverwrite = true)
    {
        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        if (!Directory.Exists(this._wwwroot))
            Directory.CreateDirectory(this._wwwroot);
        if (!Directory.Exists($"{this._wwwroot}{FileManager._slash}{directory}"))
            Directory.CreateDirectory($"{this._wwwroot}{FileManager._slash}{directory}");
        if (string.IsNullOrEmpty(directory))
            directory = "";
        if (!string.IsNullOrEmpty(fileName))
        {
            string filePath = Path.Combine(_wwwroot, directory, fileName);
            if (File.Exists(filePath))
            {
                if (allowOverwrite)
                {
                    bool result = _SaveFile(filePath, file);
                    if (result)
                        return (true, "File Saved Successfuly!", fileName);
                    else
                        return (false, "Unexpected Error Try Again!", null);
                }
                else
                    return (false, "File Already Exists", fileName);
            }
            else
            {
                bool result = _SaveFile(filePath, file);
                if (result)
                    return (true, "File Saved Successfuly!", fileName);
                else
                    return (false, "Unexpected Error Try Again!", null);
            }
        }

        return (false, "Filename Cannot Be Null Or Empty Value", null);
    }

    public (bool, string) RemoveFile(string directory = null, params string[] filenames)
    {
        List<string> errors = new List<string>();
        foreach (string filename in filenames)
        {
            try
            {
                string path = Path.Combine(_wwwroot, directory, filename);
                if (File.Exists(path))
                    File.Delete(path);
                else
                    errors.Add($"{filename} Not Exists ");
            }
            catch
            {
                errors.Add($"{filename}");
            }
        }

        if (errors.Count() > 0)
        {
            return (false, string.Join(", ", errors) + " Cannot Be Deleted!");
        }

        return (true, "All Files Deleted");
    }

    public (bool, string, string) UploadImage(Db db, string base64Image, int refId, string refType)
    {
        string fileExtension = string.Empty;

        if (base64Image.Contains(","))
        {
            var data = base64Image.Split(',')[0];
            base64Image = base64Image.Split(',')[1];

            // Extract the file extension
            var mime = data.Split(';')[0];
            var mimeSplit = mime.Split('/');
            if (mimeSplit.Length == 2)
            {
                fileExtension = mimeSplit[1];
            }
        }

        byte[] imageBytes = Convert.FromBase64String(base64Image);

        var memoryStream = new MemoryStream(imageBytes);

        var formFile = new FormFile(memoryStream, 0, imageBytes.Length, "name", $"fileName.{fileExtension}");

        var result = SaveFile(formFile, "Image", false);
        if (result.Item1)
        {
            switch (refType)
            {
                case "INST":
                    var inst = db.T_INST.First(e => e.ID == refId);
                    inst.LOGO = result.Item3;
                    db.T_INST.Update(inst);
                    break;
                case "PROJ":
                    var proj = db.T_PROJECTS.First(e => e.ID == refId);
                    proj.ICON = result.Item3;
                    db.T_PROJECTS.Update(proj);
                    break;
                case "STARTUP":
                    var startup = db.T_STARTUPS.First(e => e.ID == refId);
                    startup.LOGO = result.Item3;
                    db.T_STARTUPS.Update(startup);
                    break;
                case "PROD_IMG":
                    var product = db.T_PRODUCTS.First(e => e.ID == refId);
                    product.IMG = result.Item3;
                    db.T_PRODUCTS.Update(product);
                    break;
                case "PROD_FILE":
                    var pro = db.T_PRODUCTS.First(e => e.ID == refId);
                    pro.FILEX = result.Item3;
                    db.T_PRODUCTS.Update(pro);
                    break;
                case "SRVC_IMG":
                    var service = db.T_SERVICES.First(e => e.ID == refId);
                    service.IMG = result.Item3;
                    db.T_SERVICES.Update(service);
                    break;
                case "SRVC_FILE":
                    var srv = db.T_SERVICES.First(e => e.ID == refId);
                    srv.FILEX = result.Item3;
                    db.T_SERVICES.Update(srv);
                    break;
            }
        }
        db.SaveChanges();
        return (result.Item1, result.Item2, result.Item3);
    }


    public (bool, string, string) UploadFile(Db db, string base64Image, int refId, string refType)
    {
        string fileExtension = string.Empty;

        if (base64Image.Contains(","))
        {
            var data = base64Image.Split(',')[0];
            base64Image = base64Image.Split(',')[1];

            // Extract the file extension
            var mime = data.Split(';')[0];
            var mimeSplit = mime.Split('/');
            if (mimeSplit.Length == 2)
            {
                fileExtension = mimeSplit[1].Contains("compressed") ? "rar" : fileExtension;
            }
        }

        byte[] imageBytes = Convert.FromBase64String(base64Image);

        var memoryStream = new MemoryStream(imageBytes);

        var formFile = new FormFile(memoryStream, 0, imageBytes.Length, "name", $"fileName.{fileExtension}");

        var result = SaveFile(formFile, "Files", false);
        if (result.Item1)
        {
            switch (refType)
            {

                case "PROD_FILE":
                    var pro = db.T_PRODUCTS.First(e => e.ID == refId);
                    pro.FILEX = result.Item3;
                    db.T_PRODUCTS.Update(pro);
                    break;
            }
        }
        db.SaveChanges();
        return (result.Item1, result.Item2, result.Item3);
    }

}