namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunFollow : AcFunAcerBase
    {
        public new const string Topic = "follow";

        public override string Type => Topic;
    }
}
