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
            if (appConfiguration == null)
            {
                return BadRequest("App configuration is null.");
            }

            var res = await _generateProjectService.DownloadAngularProject(appConfiguration);

            return res == null ? NotFound("Failed to download project.") : res;
        }

        [HttpPost("generateApp")]
        public async Task<IActionResult> GenerateApp([FromBody] AppConfiguration appConfiguration)
        {
            if (appConfiguration == null)
            {
                return BadRequest("App configuration is null.");
            }
            var response = await _generateProjectService.GenerateAngularProject(appConfiguration);
            if (response == null)
            {
                return NotFound("Failed to download project.");
            }
            return Ok(response);
        }

        [HttpGet("say-hello")]
        public async Task<IActionResult> Hello([FromQuery] string name)
        {
            if (name == null)
            {
                return BadRequest("App configuration is null.");
            }
            var response = $"Hello {name}, Hope you are doing great.";
            if (response == null)
            {
                return NotFound("Failed to download project.");
            }
            return Ok(response);
        }
    }
}
