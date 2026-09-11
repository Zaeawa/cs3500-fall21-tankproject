using System;
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        private int ID { get; set; }

        [JsonProperty(PropertyName = "p1")]
        private Vector2D Point1 { get; set; }

        [JsonProperty(PropertyName = "p2")]
        private Vector2D Point2 { get; set; }

        public Wall()
        {

        }
    }
}
