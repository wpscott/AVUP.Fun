namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunLike : AcFunAcerBase
    {
        public new const string Topic = "like";

        public override string Type => Topic;
    }
}
