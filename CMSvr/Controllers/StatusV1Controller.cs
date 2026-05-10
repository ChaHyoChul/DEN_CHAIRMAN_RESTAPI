using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.VatechService;

namespace CMSvr.Controllers
{
    [ApiController]
    [Route("api/v1/status")]
    public class StatusV1Controller : ControllerBase
    {
        private readonly V1MachineStatusService _statusService;

        public StatusV1Controller(V1MachineStatusService statusService)
        {
            _statusService = statusService;
        }

        /// <summary>
        /// 현재 가공 모드(RunMode)를 조회합니다.
        /// </summary>
        [HttpGet("RunMode")]
        public IActionResult GetRunMode()
        {
            return Ok(_statusService.GetRunMode());
        }

        /// <summary>
        /// 스핀들의 속도 및 동작 상태를 조회합니다.
        /// </summary>
        [HttpGet("Spindle")]
        public IActionResult GetSpindle()
        {
            return Ok(_statusService.GetSpindleStatus());
        }

        /// <summary>
        /// 장비의 Ready 상태 및 원점 복구(Homed) 상태를 조회합니다.
        /// </summary>
        [HttpGet("Ready")]
        public IActionResult GetReady()
        {
            return Ok(_statusService.GetReadyStatus());
        }

        /// <summary>
        /// 현재 발생한 알람 및 에러 정보를 조회합니다.
        /// </summary>
        [HttpGet("Alarms")]
        public IActionResult GetAlarms()
        {
            return Ok(_statusService.GetAlarms());
        }
    }
}
