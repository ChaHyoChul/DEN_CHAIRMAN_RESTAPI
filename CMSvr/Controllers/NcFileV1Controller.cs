using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.VatechService;

namespace CMSvr.Controllers
{
    [ApiController]
    [Route("api/v1/NcFile")]
    public class NcFileV1Controller : ControllerBase
    {
        private readonly V1NcFileService _ncFileV1Service;

        public NcFileV1Controller(V1NcFileService ncFileV1Service)
        {
            _ncFileV1Service = ncFileV1Service;
        }

        /// <summary>
        /// appsettings.json에 설정된 NC 파일 저장 경로를 조회합니다.
        /// </summary>
        [HttpGet("path")]
        public IActionResult GetPath()
        {
            return Ok(new { folderPath = _ncFileV1Service.GetNcFilePath() });
        }

        /// <summary>
        /// 파일 이름을 사용하여 NC 파일을 장비에 로드(Open)합니다.
        /// <summary>
        /// 파일 이름을 사용하여 NC 파일을 장비에 로드(Open)합니다.
        /// </summary>
        [HttpPost("open/name/{fileName}")]
        public IActionResult OpenByName(string fileName)
        {
            bool success = _ncFileV1Service.OpenNcFile(fileName);
            if (success)
            {
                return Ok(new { Message = $"Successfully opened {fileName}" });
            }
            return BadRequest(new { Message = _ncFileV1Service.LastErrorMessage });
        }

        /// <summary>
        /// 현재 로드된 NC 파일을 닫습니다.
        /// </summary>
        [HttpPost("close")]
        public IActionResult Close()
        {
            bool success = _ncFileV1Service.CloseNcFile();
            if (success)
            {
                return Ok(new { Message = "Successfully closed NC file" });
            }
            return BadRequest(new { Message = _ncFileV1Service.LastErrorMessage });
        }

        /// <summary>
        /// 현재 장비에 로드(Open)되어 있는 NC 파일의 이름을 조회합니다.
        /// </summary>
        [HttpGet("get_opened_ncfile")]
        public IActionResult GetOpenedNcFile()
        {
            string fileName = _ncFileV1Service.GetOpenedFileName();
            return Ok(new { FileName = fileName });
        }
    }
}
