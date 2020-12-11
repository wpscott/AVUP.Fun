namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunComment : AcFunAcerBase
    {
        public new const string Topic = "comment";

        public override string Type => Topic;
        public string Comment { get; init; }
    }
}
