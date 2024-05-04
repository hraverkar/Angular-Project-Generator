using Angular_Project_Generator.Models.Model;
using Angular_Project_Generator.Services;
using Angular_Project_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Project_Generator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileDownloadController(IGenerateProjectService generateProjectService) : ControllerBase
    {
        private readonly IGenerateProjectService _generateProjectService = generateProjectService;

        [HttpPost("downloadApp")]
        public async Task<IActionResult> DownloadApp([FromBody] AppConfiguration appConfiguration)
        {
            var res = await _generateProjectService.DownloadAngularProject(appConfiguration);
            return res;
        }

        [HttpPost("generateApp")]
        public async Task<IActionResult> GenerateApp([FromBody] AppConfiguration appConfiguration)
        {
            var response = await _generateProjectService.GenerateAngularProject(appConfiguration);
            return Ok(response);
        }
    }
}
