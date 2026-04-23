namespace CMSvr.Domain.Dtos.MachineStatus
{
    public class MachineErrorDto
    {
        public int ErrorCode { get; set; }
        public int ErrorType { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
