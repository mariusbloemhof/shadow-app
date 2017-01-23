using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Shadow.Model
{
    public class Contact
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "UserId")]
        public string userId { get; set; }

        [JsonProperty(PropertyName = "FirstName")]
        public string firstName { get; set; }

        [JsonProperty(PropertyName = "LastName")]
        public string lastName { get; set; }

        [JsonProperty(PropertyName = "PhoneNo")]
        public string phoneNo { get; set; }

        public bool deleted { get; set; }

        public bool Changed { get; set; }

    }
}
