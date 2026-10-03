using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class RankingEntry<TNode>
    {
        [JsonPropertyName("node")]
        public TNode Node { get; set; }

        [JsonPropertyName("ranking")]
        public RankingInfo Ranking { get; set; }
    }

    [Preserve(AllMembers = true)]
    internal class RankingInfo
    {
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }
}
