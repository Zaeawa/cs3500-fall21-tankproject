// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Class that represents a wall row/column in the world
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        private int ID;

        [JsonProperty(PropertyName = "p1")]
        private Vector2D Point1;

        [JsonProperty(PropertyName = "p2")]
        private Vector2D Point2;

        private static int nextID = 0;
        private double[] wallBounds;
        public Wall(int p1_x, int p1_y, int p2_x, int p2_y)
        {
            ID = nextID;
            nextID++;
            Point1 = new Vector2D(p1_x, p1_y);
            Point2 = new Vector2D(p2_x, p2_y);
            wallBounds = new double[4];

            if (this.isHorizontal())
            {
                wallBounds[0] = Math.Min(Point1.GetX(), Point2.GetX()) - 25;
                wallBounds[1] = Math.Max(Point1.GetX(), Point2.GetX()) + 25;
                wallBounds[2] = Point1.GetY() - 25;
                wallBounds[3] = Point1.GetY() + 25;
            }

            else
            {
                wallBounds[0] = Point1.GetX() - 25;
                wallBounds[1] = Point1.GetX() + 25;
                wallBounds[2] = Math.Min(Point1.GetY(), Point2.GetY()) - 25;
                wallBounds[3] = Math.Max(Point1.GetY(), Point2.GetY()) + 25;
            }

        }

        public double [] getBounds()
        {
            return wallBounds;
        }

        /// <summary>
        /// Determines if a given point collides with the wall horizontally
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesHorizontally(double point)
        {
            return wallBounds[0] <= point && point <= wallBounds[1];
        }

        /// <summary>
        /// Determines if a given point collides with the wall vertically
        /// </summary>
        /// <returns> True if there is a collision </returns>
        public bool collidesVertically(double point)
        {
            return wallBounds[2] <= point && point <= wallBounds[3];
        }

        public int getID()
        {
            return ID;
        }
        public Vector2D getP1()
        {
            return Point1;
        }
        public Vector2D getP2()
        {
            return Point2;
        }

        /// <summary>
        /// Determines if the wall is a horizontal wall
        /// </summary>
        /// <returns>True if its two y-cood are equal </returns>
        public bool isHorizontal()
        {
            return Point1.GetY() == Point2.GetY();
        }
    }
}
