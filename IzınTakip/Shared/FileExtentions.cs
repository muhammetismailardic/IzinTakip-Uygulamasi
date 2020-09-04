using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IzinTakip.UI.Shared
{
    public class FileExtentions
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public string _rootImageDirectory;

        public FileExtentions() { }
        public FileExtentions(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _rootImageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images/");

        }
        public string UploadedFile(IFormFile formFile, string fileType)
        {
            string uniqueFileName = null;

            if (formFile != null)
            {
                string uploadsFolder = Path.Combine(_rootImageDirectory + fileType);

                if (!Directory.Exists(uploadsFolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = formFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public void DeleteFile(string rootFolder)
        {
            File.Delete(rootFolder);
        }
    }
}
