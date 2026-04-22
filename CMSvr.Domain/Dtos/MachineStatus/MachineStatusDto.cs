namespace CMSvr.Domain.Dtos.MachineStatus
{
    public class MachineStatusDto
    {
        public string RunMode { get; set; } = string.Empty;
        public double[] Position { get; set; } = new double[5];
        public bool[] Inputs { get; set; } = new bool[25];
        public bool[] Outputs { get; set; } = new bool[24];
        public int CurrentToolNo { get; set; }
        public int CurrentTool2No { get; set; }
        public bool IsNcFileLoaded { get; set; }
        public string NcFileName { get; set; } = string.Empty;
        public uint TotalLines { get; set; }
        public int CurrentLine { get; set; }
        public int ErrorType { get; set; }
        public int ErrorCode { get; set; }
    }
}
