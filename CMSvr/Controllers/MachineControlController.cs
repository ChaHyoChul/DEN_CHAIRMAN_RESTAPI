using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.Services;
using System;

namespace CMSvr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineControlController : ControllerBase
    {
        private readonly MachineControlService _controlService;

        public MachineControlController(MachineControlService controlService)
        {
            _controlService = controlService;
        }

        [HttpPost("run")]
        public IActionResult Run([FromQuery] int startLine = 0)
        {
            if (_controlService.Run(startLine)) 
                return Ok($"Run command (StartLine: {startLine}) sent successfully.");
            return BadRequest("Failed to send Run command.");
        }

        [HttpPost("stop")]
        public IActionResult Stop()
        {
            if (_controlService.Stop()) return Ok("Stop command sent successfully.");
            return BadRequest("Failed to send Stop command.");
        }

        [HttpPost("pause")]
        public IActionResult Pause()
        {
            if (_controlService.Pause()) return Ok("Pause command sent successfully.");
            return BadRequest("Failed to send Pause command.");
        }

        [HttpPost("emergency-stop")]
        public IActionResult EmergencyStop()
        {
            if (_controlService.EmergencyStop()) return Ok("Emergency Stop command sent successfully.");
            return BadRequest("Failed to send Emergency Stop command.");
        }

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            if (_controlService.Reset()) return Ok("Reset command sent successfully.");
            return BadRequest("Failed to send Reset command.");
        }

        [HttpPost("home")]
        public IActionResult Home()
        {
            if (_controlService.Home()) return Ok("Home command sent successfully.");
            return BadRequest("Failed to send Home command.");
        }

        [HttpPost("close-file")]
        public IActionResult CloseNcFile()
        {
            if (_controlService.CloseNcFile()) return Ok("Close File command sent successfully.");
            return BadRequest("Failed to send Close File command.");
        }

        [HttpPost("open-file/index/{index}")]
        public IActionResult OpenNcFileByIndex(int index)
        {
            if (_controlService.OpenNcFileByIndex(index))
                return Ok($"Open File command (Index: {index}) sent successfully.");
            return BadRequest("Failed to send Open File command.");
        }

        [HttpPost("open-file/name/{fileName}")]
        public IActionResult OpenNcFileByName(string fileName, [FromServices] NcFileService ncFileService)
        {
            if (_controlService.OpenNcFileByName(fileName, ncFileService))
                return Ok($"Open File command (Name: {fileName}) sent successfully.");
            return BadRequest($"Failed to send Open File command. File '{fileName}' might not exist in the list.");
        }
    }
}
