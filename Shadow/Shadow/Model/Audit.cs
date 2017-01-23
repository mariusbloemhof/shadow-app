using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shadow
{
    public class Audit
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "eventDescription")]
        public string eventDescription { get; set; }

        [JsonProperty(PropertyName = "eventType")]
        public string eventType { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTime timeStamp { get; set; }

        [JsonProperty(PropertyName = "eventStatus")]
        public int eventStatus { get; set; }

    }
}
