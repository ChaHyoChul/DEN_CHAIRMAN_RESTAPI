using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using CMSvr.Domain.Entities;

namespace CMSvr.Infrastructure.Services
{
    public unsafe class IpcQueueService : IDisposable
    {
        private readonly string _queueName;
        private readonly string _tagName;
        private readonly int _maxCount;
        private readonly int _itemSize;
        
        private MemoryMappedFile? _mmf;
        private MemoryMappedViewAccessor? _accessor;
        private Mutex? _mutex;

        // 제어 정보 오프셋
        private const int OFFSET_DATA_COUNT = 0;
        private const int OFFSET_READ_POINT = 4;
        private const int OFFSET_WRITE_POINT = 8;
        private const int OFFSET_TOP_USED_COUNT = 12;
        private const int OFFSET_DATA_BASE = 16;

        public IpcQueueService(string queueName = "IPC_SERVER", string tagName = "01", int maxCount = 10)
        {
            _queueName = queueName;
            _tagName = tagName;
            _maxCount = maxCount;
            _itemSize = sizeof(SIpcCommCommand);
            
            Initialize();
        }

        private void Initialize()
        {
            string fullName = $"{_queueName}_{_tagName}";
            int totalSize = OFFSET_DATA_BASE + (_maxCount * _itemSize);

            try
            {
                _mmf = MemoryMappedFile.OpenExisting(fullName, MemoryMappedFileRights.ReadWrite);
                _accessor = _mmf.CreateViewAccessor(0, totalSize, MemoryMappedFileAccess.ReadWrite);
            }
            catch (FileNotFoundException)
            {
                _mmf = MemoryMappedFile.CreateOrOpen(fullName, totalSize, MemoryMappedFileAccess.ReadWrite);
                _accessor = _mmf.CreateViewAccessor(0, totalSize, MemoryMappedFileAccess.ReadWrite);
            }

            _mutex = new Mutex(false, $"{fullName}_MUTEX");
        }

        public bool Write(SIpcCommCommand command)
        {
            if (_accessor == null || _mutex == null) return false;

            try
            {
                if (!_mutex.WaitOne(2000)) return false;

                byte* basePtr = null;
                _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref basePtr);
                try
                {
                    int* pDataCount = (int*)(basePtr + OFFSET_DATA_COUNT);
                    if (*pDataCount >= _maxCount)
                    {
                        return false;
                    }

                    int* pWritePoint = (int*)(basePtr + OFFSET_WRITE_POINT);
                    
                    // 데이터 쓰기 위치 계산
                    byte* targetPtr = basePtr + OFFSET_DATA_BASE + (*pWritePoint * _itemSize);

                    // 구조체 직접 복사 (포인터 대입)
                    *(SIpcCommCommand*)targetPtr = command;

                    // 인덱스 및 카운트 갱신
                    *pWritePoint = (*pWritePoint + 1) % _maxCount;
                    (*pDataCount)++;

                    // TopUsedCount 갱신
                    int* pTopUsedCount = (int*)(basePtr + OFFSET_TOP_USED_COUNT);
                    if (*pDataCount > *pTopUsedCount)
                    {
                        *pTopUsedCount = *pDataCount;
                    }

                    return true;
                }
                finally
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                _mutex?.ReleaseMutex();
            }
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
            _mutex?.Dispose();
        }
    }
}
