using System.Collections.Generic;

namespace CMSvr.Domain.Dtos.NcFile
{
    public class NcFileInfoDto
    {
        public int Index { get; set; }
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public uint FileSize { get; set; }
        public uint TotalLines { get; set; }
        public int State { get; set; }
        public bool IsSelected { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string WorkTime { get; set; } = string.Empty;
    }

    public class NcFileListDto
    {
        public int TotalCount { get; set; }
        public int CurrentIndex { get; set; }
        public List<NcFileInfoDto> Files { get; set; } = new();
    }
}
