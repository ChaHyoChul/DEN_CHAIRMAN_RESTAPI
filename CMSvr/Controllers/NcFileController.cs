using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.Services;
using CMSvr.Domain.Dtos.NcFile;
using System;

namespace CMSvr.Controllers
{
    [NonController]
    [ApiController]
    [Route("api/[controller]")]
    public class NcFileController : ControllerBase
    {
        private readonly NcFileService _ncFileService;

        public NcFileController(NcFileService ncFileService)
        {
            _ncFileService = ncFileService;
        }

        [HttpGet("list")]
        public ActionResult<NcFileListDto> GetNcFileList()
        {
            try
            {
                var list = _ncFileService.GetNcFileList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/index/{index}")]
        public IActionResult DeleteNcFile(int index)
        {
            try
            {
                var result = _ncFileService.DeleteNcFile(index);
                if (!result)
                {
                    return BadRequest("Invalid index or failed to delete.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/name/{fileName}")]
        public IActionResult DeleteNcFileByName(string fileName)
        {
            try
            {
                var result = _ncFileService.DeleteNcFileByName(fileName);
                if (!result)
                {
                    return BadRequest($"File '{fileName}' not found or failed to delete.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public IActionResult AddNcFile([FromBody] string fullPath)
        {
            try
            {
                var result = _ncFileService.AddNcFile(fullPath);
                if (!result)
                {
                    return BadRequest("Failed to add file. Check if path is valid, file already exists, or list is full.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("open/index/{index}")]
        public IActionResult OpenNcFileByIndex(int index)
        {
            try
            {
                var result = _ncFileService.OpenNcFileByIndex(index);
                if (!result)
                {
                    return BadRequest("Invalid index or failed to open.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("open/name/{fileName}")]
        public IActionResult OpenNcFileByName(string fileName)
        {
            try
            {
                var result = _ncFileService.OpenNcFileByName(fileName);
                if (!result)
                {
                    return BadRequest($"File '{fileName}' not found or failed to open.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("close")]
        public IActionResult CloseNcFile()
        {
            try
            {
                var result = _ncFileService.CloseNcFile();
                if (!result)
                {
                    return BadRequest("Failed to close file.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
