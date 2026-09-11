// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Class that represents a command sent by a client to the server
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCMD
    {
        [JsonProperty(PropertyName = "moving")]
        private string Moving;

        [JsonProperty(PropertyName = "fire")]
        private string Fire;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D TurretDir;

        /// <summary>
        /// Default settings for a command where the tank is not moving, not firing, and aims to the right
        /// </summary>
        public ControlCMD()
        {
            Moving = "none";
            Fire = "none";
            TurretDir = new Vector2D(1, 0);
        }

        public string getMoveDir()
        {
            return Moving;
        }

        public void setMoveDir(string s)
        {
            Moving = s;
        }

        public string getFireMode()
        {
            return Fire;
        }

        public void setFireMode(string s)
        {
            Fire = s;
        }

        public Vector2D getTurretDir()
        {
            return TurretDir;
        }

        public void setTurretDir(Vector2D v)
        {
            TurretDir = v;
        }
    }
}
