using Angular_Project_Generator.Models.Helper;
using Angular_Project_Generator.Models.Model;
using Angular_Project_Generator.Services.Interfaces;

namespace Angular_Project_Generator.Services
{
    public class GenerateProjectService(ILogger<GenerateProjectService> logger) : IGenerateProjectService
    {
        private readonly ILogger<GenerateProjectService> _logger = logger;
        public async Task<byte[]> DownloadAngularProject(AppConfiguration request)
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

                var result = await builder.GenerateProject(projectModel, folderPath);
                if (!result.Item1)
                {
                    throw new Exception("Invalid request ...");
                }
                var resultFile = await builder.GetFileFromBlob(result.Item2);
                return resultFile;
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

                var result = await builder.GenerateProject(projectModel, folderPath);
                if (!result.Item1)
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