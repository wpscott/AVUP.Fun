namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunGift : AcFunAcerBase
    {
        public new const string Topic = "gift";

        public override string Type => Topic;
        public long GiftId { get; init; }
        public string GiftName { get; init; }
        public string GiftComboId { get; init; }
        public int GiftCount { get; init; }
        public int GiftCombo { get; init; }
        public long GiftValue { get; init; }
        public string Pic { get; init; }
    }
}
