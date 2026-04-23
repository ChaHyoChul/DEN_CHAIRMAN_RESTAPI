using System;
using System.Collections.Generic;
using CMSvr.Domain.Entities;
using CMSvr.Domain.Dtos.NcFile;
using CMSvr.Domain.Enums;
using CMSvr.Infrastructure.Utils;
using System.IO;
using System.Text;

namespace CMSvr.Infrastructure.Services
{
    public class NcFileService
    {
        private readonly SharedMemoryService _shmService;

        public NcFileService(SharedMemoryService shmService)
        {
            _shmService = shmService;
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

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string targetDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "Data", "NCFILES"));

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string destPath = Path.Combine(targetDir, fileName);

            try
            {
                File.Copy(fullPath, destPath, true);
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

        public unsafe bool CloseNcFile()
        {
            // 1. 리스트 선택 해제
            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            int count = Math.Clamp(fileMgr.nNumFiles, 0, 100);
            for (int i = 0; i < count; i++)
            {
                fileMgr.hNCFileInfo[i].is_select = (char)0;
            }
            fileMgr.nCurrentFileIndex = -1;
            _shmService.WriteSharedMemory(SharedMemoryObjectNames.NcFileMgr, fileMgr);

            // 2. SPAStatus 파일명 초기화
            var paStatus = _shmService.ReadSharedMemory<SPAStatus>(SharedMemoryObjectNames.PmacState);
            CopyStringToFixedByte("", paStatus.szCurrentFileName, 128);
            _shmService.WriteSharedMemory(SharedMemoryObjectNames.PmacState, paStatus);

            // 3. SThreadState 상태 해제
            var threadState = _shmService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            threadState.bIsOpenNCFile = 0;
            threadState.bUpdateNcFileList_ = 1;
            threadState.hNCFileInfo = default;
            
            _shmService.WriteSharedMemory(SharedMemoryObjectNames.PThreadState, threadState);

            return true;
        }

        public unsafe bool OpenNcFileByIndex(int index)
        {
            var fileMgr = _shmService.ReadSharedMemory<SNCFileMgr>(SharedMemoryObjectNames.NcFileMgr);
            if (index < 0 || index >= fileMgr.nNumFiles)
            {
                return false;
            }

            string selectedFileName = "";
            for (int i = 0; i < fileMgr.nNumFiles; i++)
            {
                bool isTarget = (i == index);
                fileMgr.hNCFileInfo[i].is_select = (char)(isTarget ? 1 : 0);
                if (isTarget)
                {
                    selectedFileName = BytePtrConverter.GetString(fileMgr.hNCFileInfo[i].file_name, 257);
                }
            }

            fileMgr.nCurrentFileIndex = index;
            _shmService.WriteSharedMemory(SharedMemoryObjectNames.NcFileMgr, fileMgr);

            // 1. SPAStatus 업데이트
            var paStatus = _shmService.ReadSharedMemory<SPAStatus>(SharedMemoryObjectNames.PmacState);
            CopyStringToFixedByte(selectedFileName, paStatus.szCurrentFileName, 128);
            _shmService.WriteSharedMemory(SharedMemoryObjectNames.PmacState, paStatus);

            // 2. SThreadState 업데이트
            var threadState = _shmService.ReadSharedMemory<SThreadState>(SharedMemoryObjectNames.PThreadState);
            threadState.bIsOpenNCFile = 1;
            threadState.bUpdateNcFileList_ = 1;
            threadState.hNCFileInfo = fileMgr.hNCFileInfo[index];
            
            _shmService.WriteSharedMemory(SharedMemoryObjectNames.PThreadState, threadState);

            return true;
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
