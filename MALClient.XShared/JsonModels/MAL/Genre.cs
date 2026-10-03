using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class Genre
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
