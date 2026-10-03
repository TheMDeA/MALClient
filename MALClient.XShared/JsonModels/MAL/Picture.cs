using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class MainPicture
    {
        [JsonPropertyName("medium")]
        public String Medium { get; set; }

        [JsonPropertyName("large")]
        public String Large { get; set; }
    }
}
