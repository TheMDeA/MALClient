using System.Collections.Generic;
using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class MangaEntry : IMalNode
    {
        [JsonPropertyName("id")]
        public long? MalId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("main_picture")]
        public MainPicture Picture { get; set; }

        [JsonPropertyName("num_volumes")]
        public long? Volumes { get; set; }

        [JsonPropertyName("num_chapters")]
        public long? Chapters { get; set; }

        [JsonPropertyName("mean")]
        public double? Score { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("media_type")]
        public string Type { get; set; }

        [JsonPropertyName("alternative_titles")]
        public AlternativeTitles AlternativeTitle { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("synopsis")]
        public string Synopsis { get; set; }

        [JsonPropertyName("related_anime")]
        public ICollection<RelatedEntry<AnimeEntry>> RelatedAnime { get; set; }

        [JsonPropertyName("related_manga")]
        public ICollection<RelatedEntry<MangaEntry>> RelatedManga { get; set; }
    }
}
