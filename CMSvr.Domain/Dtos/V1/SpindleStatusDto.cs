namespace CMSvr.Domain.Dtos.V1
{
    public class SpindleStatusDto
    {
        public string Uid { get; set; } = "NONE";
        public SpindleOffset Spindle { get; set; } = new SpindleOffset();
    }

    public class SpindleOffset
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }
}
