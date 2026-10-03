using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class AnimeNode<TResponse>
    {
        [JsonPropertyName("node")]
        public TResponse Node { get; set; }
    }
}
