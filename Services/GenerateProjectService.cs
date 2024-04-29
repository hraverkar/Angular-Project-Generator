using Angular_Project_Generator.Models.Helper;
using Angular_Project_Generator.Models.Model;
using Angular_Project_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;

namespace Angular_Project_Generator.Services
{
    public class GenerateProjectService(ILogger<GenerateProjectService> logger) : IGenerateProjectService
    {
        private readonly ILogger<GenerateProjectService> _logger = logger;
        public async Task<FileContentResult> DownloadAngularProject(AppConfiguration request)
        {
            string zipFilePath = string.Empty;
            string folderPath = string.Empty;
            try
            {
                var builder = new AppBuilder(request);
                _logger.LogInformation($"****** Generating New Project : {request.Name}******");
                folderPath = Path.Combine(Directory.GetCurrentDirectory(), request.Name);
                // Check if the folder exists and delete it if it does
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    _logger.LogInformation($"Deleted existing folder: {folderPath}");
                }
                var projectModel = new ProjectModel();
                projectModel.Title = request.Name;
                projectModel.Description = "Angular Generator Project";
                projectModel.Template = "Angular-CLI";
                projectModel.Tags = ["stackblitz", "sdk"];

                bool result = await builder.GenerateProject(projectModel, folderPath);
                if (!result)
                {
                    throw new Exception("Invalid request ...");
                }
                string zipFileName = request.Name + ".zip";
                zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);

                var t = ProcessZipFile(zipFilePath);
                var res = await DownloadZipFile(zipFilePath, zipFileName, t);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating Angular project.");
                throw; // Re-throw the exception to maintain consistent error handling
            }
            finally
            {
                // Clean up: Delete the ZIP file
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                    Directory.Delete(folderPath, true);
                }
            }
        }

        private async Task<FileContentResult> DownloadZipFile(string zipFilePath, string zipFileName, MemoryStream memoryStream)
        {
            // var bytes = await File.ReadAllBytesAsync(zipFilePath);

            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    var zipEntry = zipArchive.CreateEntry(zipFileName);

                    using (var originalFileStream = memoryStream)
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                }
                return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = zipFileName };
            }

        }


        public MemoryStream ProcessZipFile(string zipFilePath)
        {
            using (FileStream fileStream = new FileStream(zipFilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    // Create a MemoryStream to store the extracted contents
                    MemoryStream memoryStream = new MemoryStream();

                    // Extract each entry in the zip file
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        using (Stream entryStream = entry.Open())
                        {
                            entryStream.CopyTo(memoryStream);
                        }
                    }

                    // Reset the position of the MemoryStream to the beginning
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Return the MemoryStream containing the zip file contents
                    return memoryStream;
                }
            }
        }

        public async Task<ProjectModel> GenerateAngularProject(AppConfiguration request)
        {
            string zipFilePath = string.Empty;
            string folderPath = string.Empty;
            try
            {
                var builder = new AppBuilder(request);
                _logger.LogInformation($"****** Generating New Project : {request.Name}******");
                folderPath = Path.Combine(Directory.GetCurrentDirectory(), request.Name);
                // Check if the folder exists and delete it if it does
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    _logger.LogInformation($"Deleted existing folder: {folderPath}");
                }
                var projectModel = new ProjectModel();
                projectModel.Title = request.Name;
                projectModel.Description = "Angular Generator Project";
                projectModel.Template = "Angular-CLI";
                projectModel.Tags = ["stackblitz", "sdk"];

                bool result = await builder.GenerateProject(projectModel, folderPath);
                if (!result)
                {
                    throw new Exception("Invalid request ...");
                }
                return projectModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating Angular project.");
                throw; // Re-throw the exception to maintain consistent error handling
            }
            finally
            {
                // Clean up: Delete the ZIP file
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                    Directory.Delete(folderPath, true);
                }
            }
        }

    }
}