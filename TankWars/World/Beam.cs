// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Class that represents a beam object in the world
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        private int ID;

        private static int nextID = 0;

        [JsonProperty(PropertyName = "org")]
        private Vector2D Origin;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D Direction;

        [JsonProperty(PropertyName = "owner")]
        private int OwnerID { get; set; }

        private int currentFrame = -1;
        private const int timeMultiplier = 3;
        private int totalFrame = 4 * timeMultiplier;

        /// <summary>
        /// Constructor for a beam and used by the server to spawn new beams
        /// </summary>
        public Beam(Vector2D org, Vector2D dir, int ownerId)
        {
            ID = nextID;
            nextID++;

            Origin = org;
            Direction = dir;
            OwnerID = ownerId;
        }

        public int getID()
        {
            return ID;
        }

        public Vector2D getLocation()
        {
            return Origin;
        }

        public Vector2D getOrientation()
        {
            return Direction;
        }

        public int getCurrentFrame()
        {
            return currentFrame;
        }

        /// <summary>
        /// Fixes the angle of the beam provided by the tank. This will correct the beam when drawn to
        /// display correctly when fired from a tank
        /// </summary>
        public void correctAngle()
        {
            Direction.Rotate(-90);
        }

        public int getTimeMult()
        {
            return timeMultiplier;
        }

        /// <summary>
        /// Attempts to go to the next frame of the animation. Will return true if it has
        /// not reached the frame cap yet
        /// </summary>
        public bool incrementFrame()
        {
            if (currentFrame < totalFrame)
            {
                currentFrame++;
                return true;
            }
            return false;
        }

        public int getOwnerID()
        {
            return OwnerID;
        }
    }
}
