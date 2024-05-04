using Angular_Project_Generator.Models.Model;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Project_Generator.Services.Interfaces
{
    public interface IGenerateProjectService
    {
        Task<FileContentResult> DownloadAngularProject(AppConfiguration request);
        Task<ProjectModel> GenerateAngularProject(AppConfiguration request);
    }
}
