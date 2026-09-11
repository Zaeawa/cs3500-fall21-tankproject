// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// Class that represents the explosion created upon death in the world
    /// </summary>
    public class DeathExplode
    {
        private Vector2D Position;
        private int OwnerID;

        private int currentFrame = -1;
        private const int timeMultiplier = 3;
        private int totalFrame = 6 * timeMultiplier;

        public DeathExplode(Vector2D pos, int id)
        {
            Position = pos;
            OwnerID = id;
        }

        public int getID()
        {
            return OwnerID;
        }

        public Vector2D getPosition()
        {
            return Position;
        }

        public int getCurrentFrame()
        {
            return currentFrame;
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
    }
}
