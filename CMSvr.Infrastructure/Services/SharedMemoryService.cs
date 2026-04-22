using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using CMSvr.Domain.Entities;

namespace CMSvr.Infrastructure.Services
{
    public unsafe class SharedMemoryService : IDisposable
    {
        private readonly string _tagName;
        private readonly string _ipcFilePath;
        private readonly ConcurrentDictionary<string, (MemoryMappedFile mmf, MemoryMappedViewAccessor accessor)> _cache = 
            new ConcurrentDictionary<string, (MemoryMappedFile, MemoryMappedViewAccessor)>();

        public SharedMemoryService(string tagName = "01")
        {
            _tagName = tagName;
            // 기본 IPC 파일 경로 설정 (Chairman_VS2022 내부)
            _ipcFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Chairman_VS2022", "Data", "Epnc", "IPCFILES");
            // 만약 위 경로가 부정확하다면 실행 파일 위치에 맞게 조정 필요
        }

        private string GetFullObjectName(string baseName)
        {
            return $"{baseName}_{_tagName}";
        }

        private MemoryMappedViewAccessor GetAccessor(string objectName, int size)
        {
            string fullName = GetFullObjectName(objectName);

            if (_cache.TryGetValue(fullName, out var cached))
            {
                return cached.accessor;
            }

            try
            {
                // 1. 먼저 순수 커널 객체로 시도 (NULL 경로 생성 방식)
                var mmf = MemoryMappedFile.OpenExisting(fullName, MemoryMappedFileRights.ReadWrite);
                var accessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.ReadWrite);
                
                var entry = (mmf, accessor);
                _cache.TryAdd(fullName, entry);
                return accessor;
            }
            catch (FileNotFoundException)
            {
                // 2. 실패 시 파일 기반 매핑 시도
                string shmFileName = $"SHM_{fullName}";
                // 실제 환경에 맞게 경로 재탐색 (프로젝트 루트의 Data 폴더)
                string fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Chairman_VS2022", "Data", "Epnc", "IPCFILES", shmFileName));
                
                if (File.Exists(fullPath))
                {
                    try
                    {
                        var mmf = MemoryMappedFile.CreateFromFile(fullPath, FileMode.Open, fullName, size, MemoryMappedFileAccess.ReadWrite);
                        var accessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.ReadWrite);
                        _cache.TryAdd(fullName, (mmf, accessor));
                        return accessor;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        public T ReadSharedMemory<T>(string objectName) where T : unmanaged
        {
            int size = sizeof(T);
            var accessor = GetAccessor(objectName, size);
            
            if (accessor == null) return default;

            byte* ptr = null;
            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            try
            {
                return *(T*)ptr;
            }
            finally
            {
                accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        public void WriteSharedMemory<T>(string objectName, T data) where T : unmanaged
        {
            int size = sizeof(T);
            var accessor = GetAccessor(objectName, size);

            if (accessor == null)
            {
                // 새로 생성 로직은 현재 생략 (장비에서 생성하므로)
                return;
            }

            byte* ptr = null;
            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            try
            {
                *(T*)ptr = data;
            }
            finally
            {
                accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        public void Dispose()
        {
            foreach (var kvp in _cache)
            {
                kvp.Value.accessor.Dispose();
                kvp.Value.mmf.Dispose();
            }
            _cache.Clear();
        }
    }

    public static class SharedMemoryObjectNames
    {
        public const string PmacState = "PMAC_STATE_01";
        public const string PThreadState = "PTHREAD_STATE_01";
        public const string ToolMgr = "TOOL_MGR_01";
        public const string MaintenanceMgr = "MAINTENANCE_MGR_01";
        public const string ConfigData = "CONFIG_DATA_01";
        public const string NcFileMgr = "NCFILE_MGR";
        public const string NcFileObj = "NC_FILE_OBJ_01";
        public const string AutoCalParam = "AUTOCAL_PARAM_01";
        public const string CoordinateOffsetDataRange = "COORD_OFFSET_DATA_RANGE_01";
        public const string MeasureParamEtc = "MEASURE_PARAM_ETC_01";
    }
}
