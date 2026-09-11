using System;
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        private int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        private Vector2D Location { get; set; }

        [JsonProperty(PropertyName = "dir")]
        private Vector2D Orientation { get; set; }

        [JsonProperty(PropertyName = "died")]
        private bool Died { get; set; }

        [JsonProperty(PropertyName = "owner")]
        private int OwnerID { get; set; }

        public Projectile()
        {
            this.Died = false;
        }
    }
}
