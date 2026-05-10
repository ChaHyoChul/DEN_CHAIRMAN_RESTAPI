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
        private Semaphore? _semaphore;
        private bool _isInitialized = false;

        private const int OFFSET_DATA_COUNT = 0;
        private const int OFFSET_READ_POINT = 4;
        private const int OFFSET_WRITE_POINT = 8;
        private const int OFFSET_TOP_USED_COUNT = 12;
        private const int OFFSET_DATA_BASE = 16;

        public IpcQueueService(string queueName = "IPC_SERVER", string tagName = "NONE", int maxCount = 10)
        {
            _queueName = queueName;
            _tagName = tagName;
            _maxCount = maxCount;
            _itemSize = sizeof(SIpcCommCommand);
            
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            lock (this)
            {
                if (_isInitialized) return;

                string baseFullName = $"{_queueName}_{_tagName}";
                string shmName = $"QSHM_{baseFullName}";
                string mtxName = $"QMTX_{baseFullName}";
                string smpName = $"QSMP_{baseFullName}";

                int totalSize = OFFSET_DATA_BASE + (_maxCount * _itemSize);

                try
                {
                    // 기존에 생성된 커널 객체에 연결
                    _mmf = MemoryMappedFile.OpenExisting(shmName, MemoryMappedFileRights.ReadWrite);
                    _accessor = _mmf.CreateViewAccessor(0, totalSize, MemoryMappedFileAccess.ReadWrite);
                    
                    if (!Mutex.TryOpenExisting(mtxName, out _mutex))
                    {
                        _mutex = new Mutex(false, mtxName);
                    }

                    if (!Semaphore.TryOpenExisting(smpName, out _semaphore))
                    {
                        _semaphore = new Semaphore(0, _maxCount, smpName);
                    }

                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine($"[IpcQueue] Successfully initialized singleton for {shmName}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[IpcQueue] Initialization deferred (Device might be offline): {ex.Message}");
                }
            }
        }

        public bool Write(SIpcCommCommand command)
        {
            // 지연 초기화 지원
            if (!_isInitialized) Initialize();
            if (!_isInitialized || _accessor == null || _mutex == null) return false;

            try
            {
                if (!_mutex.WaitOne(2000)) return false;

                byte* basePtr = null;
                _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref basePtr);
                try
                {
                    int* pDataCount = (int*)(basePtr + OFFSET_DATA_COUNT);
                    if (*pDataCount >= _maxCount) return false;

                    int* pWritePoint = (int*)(basePtr + OFFSET_WRITE_POINT);
                    byte* targetPtr = basePtr + OFFSET_DATA_BASE + (*pWritePoint * _itemSize);

                    *(SIpcCommCommand*)targetPtr = command;

                    *pWritePoint = (*pWritePoint + 1) % _maxCount;
                    (*pDataCount)++;

                    int* pTopUsedCount = (int*)(basePtr + OFFSET_TOP_USED_COUNT);
                    if (*pDataCount > *pTopUsedCount) *pTopUsedCount = *pDataCount;

                    _accessor.Flush();
                    _semaphore?.Release(1);

                    return true;
                }
                finally
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[IpcQueue] Write failed: {ex.Message}");
                return false;
            }
            finally
            {
                try { _mutex?.ReleaseMutex(); } catch { }
            }
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
            _mutex?.Dispose();
            _semaphore?.Dispose();
            _isInitialized = false;
        }
    }
}
