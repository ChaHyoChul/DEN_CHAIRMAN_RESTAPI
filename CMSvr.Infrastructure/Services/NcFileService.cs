using System;
using System.Collections.Generic;
using CMSvr.Domain.Entities;
using CMSvr.Domain.Dtos.NcFile;
using CMSvr.Domain.Enums;
using CMSvr.Infrastructure.Utils;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CMSvr.Infrastructure.Services
{
    public class NcFileService
    {
        private readonly SharedMemoryService _shmService;
        private readonly MachineControlService _controlService;
        private readonly IConfiguration _configuration;

        public NcFileService(
            SharedMemoryService shmService, 
            MachineControlService controlService,
            IConfiguration configuration)
        {
            _shmService = shmService;
            _controlService = controlService;
            _configuration = configuration;
        }

        public unsafe NcFileListDto GetNcFileList()
        {
            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            var dto = new NcFileListDto
            {
                TotalCount = fileMgr.nNumFiles,
                CurrentIndex = fileMgr.nCurrentFileIndex
            };

            int count = Math.Clamp(fileMgr.nNumFiles, 0, 100);
            for (int i = 0; i < count; i++)
            {
                var fileInfo = fileMgr.hNCFileInfo[i];
                dto.Files.Add(new NcFileInfoDto
                {
                    Index = i,
                    Id = BytePtrConverter.GetString(fileInfo.id, 26),
                    FileName = BytePtrConverter.GetString(fileInfo.file_name, 257),
                    FileSize = fileInfo.file_size,
                    TotalLines = fileInfo.total_lines,
                    State = (int)fileInfo.state,
                    IsSelected = fileInfo.is_select != 0,
                    StartTime = BytePtrConverter.GetString(fileInfo.start_time, 20),
                    WorkTime = BytePtrConverter.GetString(fileInfo.work_time, 20)
                });
            }

            return dto;
        }

        public unsafe bool AddNcFile(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                return false;
            }

            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            if (fileMgr.nNumFiles >= 100)
            {
                return false;
            }

            string fileName = Path.GetFileName(fullPath);

            for (int i = 0; i < fileMgr.nNumFiles; i++)
            {
                string existingName = BytePtrConverter.GetString(fileMgr.hNCFileInfo[i].file_name, 257);
                if (string.Equals(existingName, fileName, StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"[NcFileService] File already in list: {fileName}");
                    return false;
                }
            }

            // appsettings.json의 NCFilesForCM 항목 사용
            string targetDir = _configuration["NCFilesForCM"] ?? "";
            
            // 만약 설정이 없으면 기존 로직(상대 경로) 사용
            if (string.IsNullOrEmpty(targetDir))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                targetDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "Data", "NCFILES"));
            }

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string destPath = Path.Combine(targetDir, fileName);

            try
            {
                // 원본 파일과 대상 파일이 같으면 복사 스킵
                if (!string.Equals(Path.GetFullPath(fullPath), Path.GetFullPath(destPath), StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(fullPath, destPath, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NcFileService] Copy failed: {ex.Message}");
                return false;
            }

            var fileInfo = new FileInfo(destPath);
            var newEntry = new SNCFileInfo();
            string id = DateTime.Now.ToString("yyMMdd_HHmmss_fff");

            CopyStringToFixedChar(id, newEntry.id, 26);
            CopyStringToFixedChar(fileName, newEntry.file_name, 257);

            newEntry.file_size = (uint)fileInfo.Length;
            newEntry.state = (char)NcFileState.Before;
            newEntry.is_select = (char)0;

            try 
            {
                newEntry.total_lines = (uint)File.ReadAllLines(destPath).Length;
            }
            catch 
            {
                newEntry.total_lines = 0;
            }

            fileMgr.hNCFileInfo[fileMgr.nNumFiles] = newEntry;
            fileMgr.nNumFiles++;

            _shmService.WriteSharedMemory(SharedMemoryObjectNames.NcFileMgr, fileMgr);
            return true;
        }

        public unsafe bool DeleteNcFile(int index)
        {
            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);

            if (index < 0 || index >= fileMgr.nNumFiles)
            {
                return false;
            }

            for (int i = index; i < fileMgr.nNumFiles - 1; i++)
            {
                fileMgr.hNCFileInfo[i] = fileMgr.hNCFileInfo[i + 1];
            }

            fileMgr.hNCFileInfo[fileMgr.nNumFiles - 1] = default;
            fileMgr.nNumFiles--;

            if (fileMgr.nCurrentFileIndex >= fileMgr.nNumFiles && fileMgr.nNumFiles > 0)
            {
                fileMgr.nCurrentFileIndex = fileMgr.nNumFiles - 1;
            }

            _shmService.WriteSharedMemory(SharedMemoryObjectNames.NcFileMgr, fileMgr);
            return true;
        }

        public unsafe bool DeleteNcFileByName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            int index = -1;
            int count = Math.Clamp(fileMgr.nNumFiles, 0, 100);

            for (int i = 0; i < count; i++)
            {
                string name = BytePtrConverter.GetString(fileMgr.hNCFileInfo[i].file_name, 257);
                if (string.Equals(name, fileName, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return false;

            return DeleteNcFile(index);
        }

        public unsafe bool ClearNcFileList()
        {
            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            
            // 파일 개수 및 인덱스 초기화
            fileMgr.nNumFiles = 0;
            fileMgr.nCurrentFileIndex = -1;

            // 전체 파일 정보 배열 초기화 (100개 항목)
            for (int i = 0; i < 100; i++)
            {
                fileMgr.hNCFileInfo[i] = default;
            }

            _shmService.WriteSharedMemory(SharedMemoryObjectNames.NcFileMgr, fileMgr);
            return true;
        }

        /// <summary>
        /// 물리적 경로에서 NC 파일을 삭제합니다.
        /// </summary>
        public bool DeleteNcFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            string targetDir = _configuration["NCFilesForCM"] ?? "";
            if (string.IsNullOrEmpty(targetDir))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                targetDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "Data", "NCFILES"));
            }

            string filePath = Path.Combine(targetDir, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NcFileService] Physical delete failed: {ex.Message}");
            }
            return false;
        }

        public unsafe bool CloseNcFile()
        {
            // 직접 메모리를 수정하지 않고 MachineControlService를 통해 명령 전송
            // EPncMDLL에서 로딩 및 UI 업데이트를 수행하도록 함
            return _controlService.CloseNcFile();
        }

        public unsafe bool OpenNcFileByIndex(int index)
        {
            // 직접 메모리를 수정하지 않고 MachineControlService를 통해 명령 전송
            // EPncMDLL에서 로딩 및 UI 업데이트를 수행하도록 함
            return _controlService.OpenNcFileByIndex(index);
        }

        public unsafe bool OpenNcFileByName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            int index = -1;
            int count = Math.Clamp(fileMgr.nNumFiles, 0, 100);

            for (int i = 0; i < count; i++)
            {
                string name = BytePtrConverter.GetString(fileMgr.hNCFileInfo[i].file_name, 257);
                if (string.Equals(name, fileName, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return false;

            return OpenNcFileByIndex(index);
        }

        private unsafe void CopyStringToFixedChar(string source, char* dest, int maxLength)
        {
            if (dest == null) return;
            int len = Math.Min(source.Length, maxLength - 1);
            for (int i = 0; i < len; i++)
            {
                dest[i] = source[i];
            }
            dest[len] = '\0';
        }

        private unsafe void CopyStringToFixedByte(string source, byte* dest, int maxLength)
        {
            if (dest == null) return;
            byte[] nameBytes = Encoding.Default.GetBytes(source);
            int len = Math.Min(nameBytes.Length, maxLength - 1);
            for (int i = 0; i < len; i++)
            {
                dest[i] = nameBytes[i];
            }
            dest[len] = 0;
        }
    }
}
