using CMSvr.Infrastructure.Services;
using CMSvr.Domain.Entities;
using CMSvr.Domain.Dtos.V1;
using CMSvr.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace CMSvr.Infrastructure.VatechService
{
    public class V1MachineStatusService
    {
        private readonly SharedMemoryService _sharedMemoryService;
        private readonly IConfiguration _configuration;

        public V1MachineStatusService(SharedMemoryService sharedMemoryService, IConfiguration configuration)
        {
            _sharedMemoryService = sharedMemoryService;
            _configuration = configuration;
        }

        private string GetConfigUid() => _configuration["TAG_NAME"] ?? "NONE";

        /// <summary>
        /// 현재 가공 모드(RunMode)를 조회하여 맵핑된 문자열을 반환합니다.
        /// </summary>
        public unsafe RunModeDto GetRunMode()
        {
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            int runModeValue = (int)state.hRunMode;

            string mappedMode = runModeValue switch
            {
                0 => "STOP",
                1 or 2 or 3 => "RUN",
                4 => "INIT",
                5 => "ERROR",
                6 => "PAUSE",
                _ => "UNKNOWN"
            };

            return new RunModeDto 
            { 
                Uid = GetConfigUid(),
                RunMode = mappedMode 
            };
        }

        /// <summary>
        /// 스핀들의 오프셋 정보를 조회합니다.
        /// </summary>
        public unsafe SpindleStatusDto GetSpindleStatus()
        {
            var config = _sharedMemoryService.ReadSharedMemory<SConfigData>(SharedMemoryObjectNames.ConfigData);
            
            return new SpindleStatusDto
            {
                Uid = GetConfigUid(),
                Spindle = new SpindleOffset
                {
                    x = 0.0,
                    y = 0.0,
                    z = config.fOptionData[4]
                }
            };
        }

        /// <summary>
        /// 장비의 Ready 상태를 조회합니다.
        /// (RunMode가 STOP이고, 원점 복구가 완료되었으며, 모터가 정지 상태일 때 true 반환)
        /// </summary>
        public unsafe ReadyStatusDto GetReadyStatus()
        {
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            var status = _sharedMemoryService.ReadSharedMemory<SPAStatus>(SharedMemoryObjectNames.PmacState);

            bool isReadyResult = (state.hRunMode == 0) && // RunMode_STOP 
                                (state.bIsOriginComplete_ == 1) && 
                                (status.nMotorMovingFlag == 0);

            return new ReadyStatusDto
            {
                Uid = GetConfigUid(),
                isReady = isReadyResult
            };
        }

        /// <summary>
        /// 현재 발생한 알람 및 에러 정보를 조회합니다.
        /// </summary>
        public unsafe AlarmStatusDto GetAlarms()
        {
            var state = _sharedMemoryService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            var items = new List<AlarmItem>();

            // 에러가 있는 경우에만 리스트에 추가 (예: nErrorCode가 0이 아닐 때)
            if (state.nErrorCode != 0)
            {
                items.Add(new AlarmItem
                {
                    Code = state.nErrorCode.ToString(),
                    Type = state.nErrorType.ToString(),
                    // fixed char* 포인터를 string으로 변환
                    Message = BytePtrConverter.GetString(state.szErrorMessage, 512)
                });
            }

            return new AlarmStatusDto
            {
                Uid = GetConfigUid(),
                alarmCount = items.Count,
                alarmItems = items 
            };
        }
    }
}
