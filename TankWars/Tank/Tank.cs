using System;
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        private int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        private Vector2D Location { get; set; }

        [JsonProperty(PropertyName = "bdir")]
        private Vector2D Orientation { get; set; }

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D Aiming { get; set; }

        [JsonProperty(PropertyName = "name")]
        private string Name { get; set; }

        [JsonProperty(PropertyName = "hp")]
        private int HitPoints { get; set; }

        [JsonProperty(PropertyName = "score")]
        private int Score { get; set; }

        [JsonProperty(PropertyName = "died")]
        private bool Died { get; set; }

        [JsonProperty(PropertyName = "dc")]
        private bool Disconnected { get; set; }

        [JsonProperty(PropertyName = "join")]
        private bool Joined { get; set; }

        public Tank(int id, string name)
        {
            this.ID = id;
            this.Name = name;

            this.Aiming = new Vector2D(0, -1);
            this.HitPoints = Constants.MaxHP;
            this.Score = 0;
            this.Died = false;
            this.Disconnected = false;
            this.Joined = false;
        }
    }
}
