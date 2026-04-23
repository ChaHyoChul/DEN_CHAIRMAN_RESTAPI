using System;
using CMSvr.Domain.Entities;
using CMSvr.Domain.Enums;

namespace CMSvr.Infrastructure.Services
{
    public class MachineControlService
    {
        private readonly IpcQueueService _ipcQueueService;

        public MachineControlService(IpcQueueService ipcQueueService)
        {
            _ipcQueueService = ipcQueueService;
        }

        private bool SendCommand(EN_IPC_COMMAND mCmd, byte[]? param = null)
        {
            var command = new SIpcCommCommand { m_cmd = (byte)mCmd };
            if (param != null)
            {
                unsafe
                {
                    int len = Math.Min(param.Length, 63);
                    for (int i = 0; i < len; i++)
                    {
                        command.param[i] = param[i];
                    }
                }
            }
            return _ipcQueueService.Write(command);
        }

        /// <summary>
        /// 비상 정지 (EMG)
        /// </summary>
        public bool EmergencyStop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_EMG);

        /// <summary>
        /// 에러 리셋 (RESET)
        /// </summary>
        public bool Reset() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RESET);

        /// <summary>
        /// 원점 복귀 (HOME)
        /// </summary>
        public bool Home() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_HOME);

        /// <summary>
        /// 가공 시작 (RUN)
        /// </summary>
        /// <param name="startLine">시작할 라인 번호 (기본값: 0)</param>
        public bool Run(int startLine = 0)
        {
            byte[] param = new byte[4];
            // int 값을 바이트 배열(4bytes)로 변환
            if (!BitConverter.TryWriteBytes(param, startLine))
            {
                return false;
            }

            return SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RUN, param);
        }

        /// <summary>
        /// 일시 정지 (PAUSE)
        /// </summary>
        public bool Pause() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_PAUSE);

        /// <summary>
        /// 가공 중지 (STOP)
        /// </summary>
        public bool Stop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_STOP);

        /// <summary>
        /// 현재 열려 있는 NC 파일을 닫습니다. (EPncMDLL에서 상태 처리 수행)
        /// </summary>
        public bool CloseNcFile() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_CLOSE);

        /// <summary>
        /// 인덱스를 사용하여 NC 파일을 엽니다.
        /// </summary>
        /// <param name="index">NcFileList 내의 인덱스</param>
        public bool OpenNcFileByIndex(int index)
        {
            byte[] param = new byte[4];
            if (!BitConverter.TryWriteBytes(param, index))
            {
                return false;
            }

            return SendCommand(EN_IPC_COMMAND.IPC_COMMAND_OPEN, param);
        }

        /// <summary>
        /// 파일 이름을 사용하여 NC 파일을 엽니다.
        /// </summary>
        /// <param name="fileName">NC 파일 이름 (확장자 포함)</param>
        /// <param name="ncFileService">인덱스 조회를 위한 서비스</param>
        public bool OpenNcFileByName(string fileName, NcFileService ncFileService)
        {
            var list = ncFileService.GetNcFileList();
            // 목록에서 일치하는 파일 찾기
            var file = list.Files.Find(f => string.Equals(f.FileName, fileName, StringComparison.OrdinalIgnoreCase));

            if (file == null)
            {
                return false;
            }

            return OpenNcFileByIndex(file.Index);
        }
    }
}
