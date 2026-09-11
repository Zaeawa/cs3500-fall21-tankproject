// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Class that represents a powerup in the world
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        private int ID;

        private static int nextID = 0;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D Location;

        [JsonProperty(PropertyName = "died")]
        private bool Died;

        public Powerup()
        {
            this.ID = nextID;
            nextID++;
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

        public void setLocation(Vector2D v2d)
        {
            Location = v2d;
        }

        public bool hasDied()
        {
            return Died;
        }

        /// <summary>
        /// This method just checks if this powerup can be spawned without colliding with a wall
        /// </summary>
        /// <returns> True if there is no collision </returns>
        public bool canSpawnHere(Dictionary<int, Wall> theWalls)
        {
            foreach(Wall w in theWalls.Values)
            {
                if (collidesWith(w))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// A helper method for canSpawnHere that determines if a wall collides with this powerup
        /// </summary>
        /// <returns> True if it collides </returns>
        private bool collidesWith(Wall w)
        {
            return w.collidesHorizontally(Location.GetX()) && w.collidesVertically(Location.GetY());
        }

        /// <summary>
        ///  This method just checks if this powerup applies to a tank
        /// </summary>
        /// <param name="tank"></param>
        /// <returns></returns>
        public bool collidesWith(Tank tank)
        {
            return tank.collidesHorizontally(Location.GetX()) && tank.collidesVertically(Location.GetY());
        }

        public void setDeathStatus(bool bs)
        {
            Died = bs;
        }
    }
}
