using System;
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        private int ID { get; set; }

        [JsonProperty(PropertyName = "org")]
        private Vector2D Origin { get; set; }

        [JsonProperty(PropertyName = "dir")]
        private Vector2D Direction { get; set; }

        [JsonProperty(PropertyName = "owner")]
        private int OwnerID { get; set; }
    }
}
