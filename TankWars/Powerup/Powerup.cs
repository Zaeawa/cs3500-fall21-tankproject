using System;
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        private int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        private Vector2D Location { get; set; }

        [JsonProperty(PropertyName = "died")]
        private bool Died { get; set; }

        public Powerup()
        {
            this.Died = false;
        }
    }
}
