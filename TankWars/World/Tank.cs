// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// A class that represents a tank object in the world
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        private int ID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D Location;

        [JsonProperty(PropertyName = "bdir")]
        private Vector2D Orientation;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D Aiming;

        [JsonProperty(PropertyName = "name")]
        private string Name;

        [JsonProperty(PropertyName = "hp")]
        private int HitPoints;

        [JsonProperty(PropertyName = "score")]
        private int Score;

        [JsonProperty(PropertyName = "died")]
        private bool Died;

        [JsonProperty(PropertyName = "dc")]
        private bool Disconnected;

        [JsonProperty(PropertyName = "join")]
        private bool Joined { get; set; }

        private int framesSinceFire = 0;
        private bool hasPowerup = false;
        private int currentRespawnFrames = 0;
        private int totalRespawnFrames;
        private int maxHP;
        public Tank(int id, string name, int maxhp, int respawnTime)
        {
            this.ID = id;
            this.Name = name;
            this.Aiming = new Vector2D(0, -1);
            this.Orientation = new Vector2D(0, -1);
            this.HitPoints = maxhp;
            this.Score = 0;
            this.Died = false;
            this.Disconnected = false;
            this.Joined = false;
            this.totalRespawnFrames = respawnTime;
            this.maxHP = 0 + maxhp;

        }

        public int getID()
        {
            return ID;
        }

        public int getHp()
        {
            return HitPoints;
        }

        public void setHP(int powerup)
        {
            HitPoints = powerup;
        }

        public string getName()
        {
            return Name;
        }

        public int getScore()
        {
            return Score;
        }

        public bool hasDisconnected()
        {
            return Disconnected;
        }

        public void setDisconnectionStatus(bool state)
        {
            Disconnected = state;
        }

        public bool hasDied()
        {
            return Died;
        }

        public void setDeathStatus(bool status)
        {
            Died = status;
        }

        public Vector2D getLocation()
        {
            return Location;
        }

        public void setLocation(Vector2D v2d)
        {
            Location = v2d;
        }

        public Vector2D getBodyOrientation()
        {
            return Orientation;
        }

        public void setBodyOrientation(Vector2D v2d)
        {
            Orientation = v2d;
        }

        public Vector2D getTurretOrientation()
        {
            return Aiming;
        }

        public void setTurretOrientation(Vector2D v2d)
        {
            Aiming = v2d;
        }

        public int getFireCounter()
        {
            return framesSinceFire;
        }

  
        public void resetFireCounter()
        {
            framesSinceFire = 0;
        }

        public bool hasPowerupAcquired()
        {
            return hasPowerup;
        }

        public void setPowerupStatus(bool s)
        {
            hasPowerup = s;
        }

        /// <summary>
        ///  This method just checks if this tank collides with a wall
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesWith(Wall wall)
        {
            bool collidesHorizontally = wall.collidesHorizontally(this.Location.GetX()) || wall.collidesHorizontally(this.Location.GetX() - 30) || wall.collidesHorizontally(this.Location.GetX() + 30);
            bool collidesVertically = wall.collidesVertically(this.Location.GetY()) || wall.collidesVertically(this.Location.GetY() - 30) || wall.collidesVertically(this.Location.GetY() + 30);


            if(collidesHorizontally && collidesVertically)
            {
                return true;
            }

            return false;
       
            
        }

        /// <summary>
        /// Determines if a given point collides with the tank horizontally
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesHorizontally(double point)
        {
            return Location.GetX() - 30 <= point && point <= Location.GetX() + 30;
        }

        /// <summary>
        /// Determines if a given point collides with the tank vertically
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesVertically(double point)
        {
            return Location.GetY() - 30 <= point && point <= Location.GetY() + 30;
        }

        /// <summary>
        /// Performs actions representing being hit by a projectile
        /// </summary>
        /// <returns> True if this projectile dealt the last blow to this tank </returns>
        public bool hitByProjectile()
        {
            HitPoints--;
            if(HitPoints <= 0)
            {
                Died = true;
                return true;
            }
            return false;
        }

        public void incrementScore()
        {
            Score++;
        }

        /// <summary>
        /// Resets the death status and increments the tank's shot counter
        /// Respawns the tank at a random location if it has been dead for long enough
        /// </summary>
        public void cleanupTank(Action<int> respawnLocator)
        {
            Died = false;
            framesSinceFire++;
            if(HitPoints <= 0)
            {
                if(currentRespawnFrames == totalRespawnFrames)
                {
                    currentRespawnFrames = 0;
                    HitPoints = maxHP;
                    respawnLocator(this.ID);

                }
                else
                {
                    currentRespawnFrames++;
                }
            }
        }
    }
}
