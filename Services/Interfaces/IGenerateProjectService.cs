using Angular_Project_Generator.Models.Model;

namespace Angular_Project_Generator.Services.Interfaces
{
    public interface IGenerateProjectService
    {
        Task<ProjectModel> GenerateAngularProject(AppConfiguration request);
    }
}
