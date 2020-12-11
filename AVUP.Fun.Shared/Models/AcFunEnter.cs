namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunEnter : AcFunAcerBase
    {
        public new const string Topic = "enter";

        public override string Type => Topic;
    }
}
