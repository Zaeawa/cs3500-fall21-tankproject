// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Class that represents a projectile in the world
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        private int ID;

        private static int nextID = 0;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D Location { get; set; }

        [JsonProperty(PropertyName = "dir")]
        private Vector2D Orientation { get; set; }

        [JsonProperty(PropertyName = "died")]
        private bool Died;

        [JsonProperty(PropertyName = "owner")]
        private int OwnerID { get; set; }

        public Projectile(Vector2D loc, Vector2D aim, int ownerId)
        {
            ID = nextID;
            nextID++;

            Location = loc;
            Orientation = aim;
            OwnerID = ownerId;

            this.Died = false;
        }

        public int getID()
        {
            return ID;
        }

        public Vector2D getLocation()
        {
            return Location;
        }

        public Vector2D getOrientation()
        {
            return Orientation;
        }

        public bool hasDied()
        {
            return Died;
        }

        /// <summary>
        ///  This method just checks if this projectile collides with a wall
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesWith(Wall w)
        {
            return w.collidesHorizontally(Location.GetX()) && w.collidesVertically(Location.GetY());
        }

        /// <summary>
        ///  This method just checks if this projectile collides with a tank
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesWith(Tank tank)
        {
            return tank.collidesHorizontally(Location.GetX()) && tank.collidesVertically(Location.GetY());
        }

        public void setDeathStatus(bool ds)
        {
            Died = ds;
        }

        public void setLocation (Vector2D v2d)
        {
            Location = v2d;
        }

        public int getOwnerID()
        {
            return OwnerID;
        }
    }
}
