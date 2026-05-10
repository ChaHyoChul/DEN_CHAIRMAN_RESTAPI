using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CMSvr.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UidController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UidController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// appsettings.json에 설정된 TAG_NAME을 조회합니다.
        /// </summary>
        [HttpGet]
        public IActionResult GetUid()
        {
            // appsettings.json에서 TAG_NAME 값을 읽어옵니다. (없을 경우 기본값 "")
            string tagName = _configuration["TAG_NAME"] ?? "NONE";

            var result = new {
                Uid = tagName
            };

            return Ok(result);
        }
    }
}
