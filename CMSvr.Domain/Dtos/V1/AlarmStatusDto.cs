namespace CMSvr.Domain.Dtos.V1
{
    public class AlarmStatusDto
    {
        public string Uid { get; set; } = "NONE";
        public int alarmCount { get; set; } = 0;
        public List<AlarmItem> alarmItems { get; set; } = new List<AlarmItem>();
    }

    public class AlarmItem
    {
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
