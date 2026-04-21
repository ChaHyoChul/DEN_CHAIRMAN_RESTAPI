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
        private readonly ConcurrentDictionary<string, (MemoryMappedFile mmf, MemoryMappedViewAccessor accessor)> _cache = 
            new ConcurrentDictionary<string, (MemoryMappedFile, MemoryMappedViewAccessor)>();

        public SharedMemoryService(string tagName = "01")
        {
            _tagName = tagName;
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
                var mmf = MemoryMappedFile.OpenExisting(fullName, MemoryMappedFileRights.ReadWrite);
                var accessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.ReadWrite);
                
                var entry = (mmf, accessor);
                if (_cache.TryAdd(fullName, entry))
                {
                    return accessor;
                }
                else
                {
                    accessor.Dispose();
                    mmf.Dispose();
                    return _cache[fullName].accessor;
                }
            }
            catch (FileNotFoundException)
            {
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
                // 포인터 캐스팅을 통한 직접 읽기 (복사 비용 최소화)
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
                string fullName = GetFullObjectName(objectName);
                var mmf = MemoryMappedFile.CreateOrOpen(fullName, size, MemoryMappedFileAccess.ReadWrite);
                var newAccessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.ReadWrite);
                _cache.TryAdd(fullName, (mmf, newAccessor));
                accessor = newAccessor;
            }

            byte* ptr = null;
            accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
            try
            {
                // 포인터 캐스팅을 통한 직접 쓰기
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
        public const string NcFileMgr = "NCFILE_MGR"; // NCFILE_MGR은 01이 붙지 않음 (C++ 헤더 확인 결과)
        public const string NcFileObj = "NC_FILE_OBJ_01";
        public const string AutoCalParam = "AUTOCAL_PARAM_01";
        public const string CoordinateOffsetDataRange = "COORD_OFFSET_DATA_RANGE_01";
        public const string MeasureParamEtc = "MEASURE_PARAM_ETC_01";
    }
}
