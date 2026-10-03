using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class Season
    {
        [JsonPropertyName("year")]
        public long? Year { get; set; }

        [JsonPropertyName("season")]
        public string Name { get; set; }
    }
}
