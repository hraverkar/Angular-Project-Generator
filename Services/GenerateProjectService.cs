using Angular_Project_Generator.Models.Helper;
using Angular_Project_Generator.Models.Model;
using Angular_Project_Generator.Services.Interfaces;

namespace Angular_Project_Generator.Services
{
    public class GenerateProjectService(ILogger<GenerateProjectService> logger) : IGenerateProjectService
    {
        private readonly ILogger<GenerateProjectService> _logger = logger;
        public async Task<ProjectModel> GenerateAngularProject(AppConfiguration request)
        {
            try
            {
                var builder = new AppBuilder(request);

                _logger.LogInformation($"****** Generating New Project : {request.Name}******");

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), request.Name);

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
                return await Task.FromResult(projectModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating Angular project.");
                throw; // Re-throw the exception to maintain consistent error handling
            }

            return null;
        }
    }
}