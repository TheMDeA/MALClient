using System;
using System.Collections.Generic;
using System.Text;
using Android.Runtime;
using System.Text.Json.Serialization;

namespace MALClient.XShared.JsonModels.MAL
{
    [Preserve(AllMembers = true)]
    internal class PaginatedMALResponse<TResponse>
    {
        [JsonPropertyName("data")]
        public TResponse Data { get; set; }
    }
}
