using CMSvr.Domain.Entities;
using CMSvr.Domain.Dtos.MachineStatus;
using CMSvr.Infrastructure.Utils;

namespace CMSvr.Infrastructure.Services
{
    public class MachineStatusService
    {
        private readonly SharedMemoryService _shmService;

        public MachineStatusService(SharedMemoryService shmService)
        {
            _shmService = shmService;
        }

        public unsafe MachineStatusDto GetMachineStatus()
        {
            // 1. 필요한 공유 메모리 데이터 읽기
            var paStatus = _shmService.ReadSharedMemory<SPAStatus>(SharedMemoryObjectNames.PmacState);
            var threadState = _shmService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);

            // 2. DTO 구성
            var dto = new MachineStatusDto
            {
                RunMode = threadState.hRunMode.ToString(),
                CurrentToolNo = paStatus.nCurrentToolNo,
                CurrentTool2No = paStatus.nCurrentTool2No,
                IsNcFileLoaded = threadState.bIsOpenNCFile == 1,
                NcFileName = BytePtrConverter.GetString(threadState.hNCFileInfo.file_name, 257),
                TotalLines = threadState.hNCFileInfo.total_lines,
                CurrentLine = paStatus.nLineNumber,
                ErrorType = threadState.nErrorType,
                ErrorCode = threadState.nErrorCode
            };

            // 3. 배열 데이터 복사 (Position)
            for (int i = 0; i < 5; i++)
            {
                dto.Position[i] = paStatus.fPosition[i];
            }

            // 4. 비트 상태 변환 (Inputs/Outputs)
            for (int i = 0; i < 25; i++)
            {
                dto.Inputs[i] = paStatus.bInput[i] == 1;
            }
            for (int i = 0; i < 24; i++)
            {
                dto.Outputs[i] = paStatus.bOutput[i] == 1;
            }

            return dto;
        }

        public unsafe ToolListDto GetToolStatus()
        {
            var toolData = _shmService.ReadSharedMemory<SToolData>(SharedMemoryObjectNames.ToolMgr);
            var dto = new ToolListDto { TotalToolCount = 6 };

            for (int i = 1; i <= 6; i++)
            {
                var tool = toolData.hTool[i];
                
                // 에러 코드 매핑
                var (mappedCode, message) = MapToolError(tool.dwErrCode);

                // 99% 이상이면 수명 만료 표시
                if (tool.fUsingRate >= 99.0)
                {
                    message = "Timeout";
                }

                dto.Tools.Add(new ToolStatusDto
                {
                    ToolNo = i,
                    UsingRate = Math.Round(tool.fUsingRate, 1),
                    ErrorCode = mappedCode,
                    UsingTime = tool.dwUsingTime,
                    MaximumTime = tool.dwMaximumTime,
                    ErrorMessage = message
                });
            }

            return dto;
        }

        private (int code, string message) MapToolError(uint dwErrCode)
        {
            return dwErrCode switch
            {
                0 => (0, "None"),
                10 => (1, "Empty"),
                14 => (2, "Broken"),
                18 => (3, "Long"),
                19 => (4, "Short"),
                _ => (5, "Unknown")
            };
        }
    }
}
