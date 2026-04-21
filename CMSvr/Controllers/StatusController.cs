using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.Services;
using CMSvr.Domain.Entities;

namespace CMSvr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly SharedMemoryService _sharedMemoryService;

        public StatusController(SharedMemoryService sharedMemoryService)
        {
            _sharedMemoryService = sharedMemoryService;
        }

        [HttpGet]
        public unsafe IActionResult GetStatus()
        {
            var status = _sharedMemoryService.ReadSharedMemory<SPAStatus>(SharedMemoryObjectNames.PmacState);

            // 단순 반환 시 포인터나 fixed buffer가 포함된 구조체는 직렬화가 복잡할 수 있으므로, 
            // 필요한 정보만 추출하여 익명 객체로 반환하는 것이 좋습니다.

            var result = new {
                MDCode = status.nMDCode,
                LineNumber = status.nLineNumber,
                RunStatus = status.nRunStatus,
                Position = new double[] {
                    status.fPosition[0], status.fPosition[1], status.fPosition[2],
                    status.fPosition[3], status.fPosition[4]
                },
                ToolNo = status.nCurrentToolNo,
                SpindleSpeed = status.nSpindleSpeed,
                ServoPower = status.nServoPower == 1,
                InputBits = GetInputBits(status),
                RndErrorCode = status.nRndErrorCode,
                MotorOverRide = status.nMotorOverride,
                LCDReflash = status.bLCDRefresh
            };

            return Ok(result);
        }

        [HttpGet("position")]
        public unsafe IActionResult GetPosition()
        {
            var status = _sharedMemoryService.ReadSharedMemory<SPAStatus>(SharedMemoryObjectNames.PmacState);
            var pos = new double[] { 
                status.fPosition[0], status.fPosition[1], status.fPosition[2], 
                status.fPosition[3], status.fPosition[4] 
            };
            return Ok(pos);
        }

        private unsafe bool[] GetInputBits(SPAStatus status)
        {
            bool[] bits = new bool[25];
            for (int i = 0; i < 25; i++)
            {
                bits[i] = status.bInput[i] == 1;
            }
            return bits;
        }
    }
}
