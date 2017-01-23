using System;
using System.Collections.Generic;
using System.Linq;

namespace Shadow
{
    public class UserContacts
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNo { get; set; }
        public bool deleted { get; set; }
        public bool Changed { get; set; }
    }
}
