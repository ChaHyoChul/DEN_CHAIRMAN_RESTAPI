using CMSvr.Domain.Entities;
using CMSvr.Domain.Enums;
using CMSvr.Infrastructure.Services;
using CMSvr.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace CMSvr.Infrastructure.VatechService
{
    public class V1NcFileService
    {
        private readonly NcFileService _ncFileService;
        private readonly SharedMemoryService _sharedMemoryService;
        private readonly IConfiguration _configuration;

        public string LastErrorMessage { get; private set; } = string.Empty;

        public V1NcFileService(
            NcFileService ncFileService,
            SharedMemoryService sharedMemoryService,
            IConfiguration configuration)
        {
            _ncFileService = ncFileService;
            _sharedMemoryService = sharedMemoryService;
            _configuration = configuration;
        }

        /// <summary>
        /// 설정된 NC 파일 저장 경로를 조회합니다.
        /// </summary>
        public string GetNcFilePath()
        {
            return _configuration["NCFiles"] ?? "c:\\temp\\ncfiles";
        }

        /// <summary>
        /// 파일 이름을 사용하여 NC 파일을 로드합니다. (STOP 모드에서만 가능하며, 완료될 때까지 대기합니다)
        /// </summary>
        public unsafe bool OpenNcFile(string fileName)
        {
            LastErrorMessage = string.Empty;

            // 1. 인터락 체크: RunMode가 STOP(0)인 경우에만 허용
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            if (state.hRunMode != RunMode.Stop)
            {
                LastErrorMessage = $"Interlock: Machine is in {state.hRunMode} mode. Open is only allowed in STOP mode.";
                return false; 
            }

            // 2. 현재 열려있는 파일이 있는지 확인하여 기존 파일 닫기
            // - CMSvr에서 Close 하지 않고, EPncMDll에서 close 할 경우 ncfile이 열리지 않는 문제때문에 
            // - 아래 if문을 주석처리 한다 
            // if (state.bIsOpenNCFile == 1)
            {
                CloseNcFile();
            }

            // 3. 새로운 파일 추가 및 로드
            string ncFilePath = GetNcFilePath() + "\\" + fileName;
            if (!File.Exists(ncFilePath))
            {
                LastErrorMessage = $"File not found in storage: {fileName}";
                return false;
            }

            if (_ncFileService.AddNcFile(ncFilePath) == false)
            {
                LastErrorMessage = "Failed to register NC file to the machine list.";
                return false;
            }

            // 4. 장비에 Open 명령 전송 및 대기 로직 시작
            if (_ncFileService.OpenNcFileByName(fileName) == false)
            {
                LastErrorMessage = "Failed to send Open command to the machine.";
                return false;
            }

            // 5. 완료 대기 (Polling)
            int timeoutSeconds = int.TryParse(_configuration["NCFileOpenTimeout"], out int t) ? t : 10;
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
            {
                var currentState = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);

                // 명령 처리가 완료되었는지 확인
                if (currentState.bIpcCmdComplete_ == 1)
                {
                    // 에러가 발생했는지 확인
                    if (currentState.nErrorCode != 0)
                    {
                        LastErrorMessage = BytePtrConverter.GetString(currentState.szErrorMessage, 512);
                        if (string.IsNullOrEmpty(LastErrorMessage))
                        {
                            LastErrorMessage = $"Machine reported error code: {currentState.nErrorCode}";
                        }
                        return false;
                    }

                    // 정상 완료
                    return true;
                }

                System.Threading.Thread.Sleep(200); // 0.2초 간격으로 확인
            }

            LastErrorMessage = $"Open operation timed out after {timeoutSeconds} seconds.";
            return false;
        }

        /// <summary>
        /// 현재 로드된 NC 파일을 닫고, 물리 파일을 삭제한 후 리스트를 초기화합니다.
        /// (STOP 또는 ERROR 모드에서만 가능)
        /// </summary>
        public bool CloseNcFile()
        {
            LastErrorMessage = string.Empty;

            // 1. 인터락 체크: RunMode가 STOP(0) 또는 ERROR(5)인 경우에만 허용
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            if (state.hRunMode != RunMode.Stop && state.hRunMode != RunMode.Error)
            {
                LastErrorMessage = $"Interlock: Machine is in {state.hRunMode} mode. Close is only allowed in STOP or ERROR mode.";
                return false;
            }

            // 2. 현재 열려있는 파일 이름 미리 확보
            string fileName = GetOpenedFileName();

            // 3. NC 파일 닫기 명령 전송 (EPncMDLL)
            if (_ncFileService.CloseNcFile() == false)
            {
                LastErrorMessage = "Failed to send Close command to the machine.";
                return false;
            }

            // 4. 물리적 파일 삭제
            if (!string.IsNullOrEmpty(fileName))
            {
                _ncFileService.DeleteNcFile(fileName);
            }

            // 5. NC 파일 리스트의 모든 정보 삭제 (공유 메모리 리스트 초기화)
            return _ncFileService.ClearNcFileList();
        }

        /// <summary>
        /// 현재 로드된 NC 파일의 이름을 조회합니다.
        /// </summary>
        public unsafe string GetOpenedFileName()
        {
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            
            if (state.bIsOpenNCFile == 1)
            {
                return BytePtrConverter.GetString(state.hNCFileInfo.file_name, 257);
            }

            return "";
        }
    }
}
