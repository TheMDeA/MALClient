using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class RelatedEntry<TNode>
    {
        [JsonPropertyName("node")]
        public TNode Node { get; set; }

        [JsonPropertyName("relation_type")]
        public string RelationType { get; set; }

        [JsonPropertyName("relation_type_formatted")]
        public string RelationTypeFormatted { get; set; }
    }
}
