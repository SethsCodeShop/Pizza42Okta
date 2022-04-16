using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pizza42Okta
{
    public class OktaUser
    {
        [JsonProperty("User_Id")]
        public string UserId { get; set; }
        [JsonProperty("user_metadata")]
        public OktaOrder Orders { get; set; }
    }
    public class OktaOrder
    {
        [JsonProperty("orders")]
        public List<PizzaOrder> Orders { get; set; }
    }
}
