using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.Services;
using CMSvr.Domain.Entities;
using CMSvr.Infrastructure.Utils;

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
                LCDRefresh = status.bLCDRefresh,
                CurrentFileName = BytePtrConverter.GetString(status.szCurrentFileName, 128)
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

        [HttpGet("thread-state")]
        public unsafe IActionResult GetThreadState()
        {
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            
            var result = new {
                ConnectStatus = state.hConnectStatus.ToString(),
                RunMode = state.hRunMode.ToString(),
                IsOpenNCFile = state.bIsOpenNCFile == 1,
                FileName = BytePtrConverter.GetString(state.hNCFileInfo.file_name, 257),
                RunningTime = state.dwRunningTime,
                IsNCFileRun = state.bIsNCFileRun_ == 1,
                RawIsNCFileRun = state.bIsNCFileRun_,
                JogSpeed = state.nJogSpeed_,
                ShowSetupDialog = state.bShowSetupDialog_ == 1
            };

            return Ok(result);
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
