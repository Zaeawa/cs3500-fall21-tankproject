// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Represents the "Model" of the program. For the client, holds the current world data received from the server
    /// </summary>
    public class World
    {
        private int worldSize = -1;
        private int viewSize;

        private int myID = -1;
        private string myName;

        private string jsonWalls;

        // Server-related properties
        private int MSPerFrame;
        private int startingHP;
        private int respawnDelay;

        private int projectileSpeed;
        private int projectileFireDelay;

        private Random tankRandomizer = new Random();
        private int tankSpeed;

        private Random powerupRandomizer = new Random();
        private int maxPowerups;
        private int currentPowerupDelay;
        private int maxPowerupDelay;
        private int powerupDelayFrame;

        // Dictionaries
        Dictionary<int, Wall> walls;
        Dictionary<int, Tank> tanks;
        Dictionary<int, Powerup> powerups;
        Dictionary<int, Projectile> projectiles;
        Dictionary<int, Beam> beams;
        Dictionary<int, DeathExplode> deathExplodes;
        /// <summary>
        /// theWorld constructor for the View
        /// </summary>
        public World(int vSize)
        {
            viewSize = vSize;

            walls = new Dictionary<int, Wall>();
            tanks = new Dictionary<int, Tank>();
            powerups = new Dictionary<int, Powerup>();
            projectiles = new Dictionary<int, Projectile>();
            beams = new Dictionary<int, Beam>();
            deathExplodes = new Dictionary<int, DeathExplode>();
        }

        /// <summary>
        /// theWorld constructor for the server
        /// </summary>
        public World(int ws, int mpf, int shp, int rd, int ps, int pfd, int ts, int mp, int mpd, List<Wall> wls)
        {
            worldSize = ws;
            MSPerFrame = mpf;
            startingHP = shp;
            respawnDelay = rd;

            projectileSpeed = ps;
            projectileFireDelay = pfd;
            tankSpeed = ts;

            maxPowerups = mp;
            currentPowerupDelay = mpd;
            maxPowerupDelay = mpd;
            powerupDelayFrame = 0;

            walls = new Dictionary<int, Wall>();
            tanks = new Dictionary<int, Tank>();
            powerups = new Dictionary<int, Powerup>();
            projectiles = new Dictionary<int, Projectile>();
            beams = new Dictionary<int, Beam>();

            foreach (Wall w in wls)
            {
                walls[w.getID()] = w;
                jsonWalls += JsonConvert.SerializeObject(w) + "\n";
            }
        }

        public int getViewSize()
        {
            return viewSize;
        }

        public string getJSONWallRepresentation()
        {
            return jsonWalls;
        }

        public int getMSPerFrame()
        {
            return MSPerFrame;
        }

        public int getStartingHealth()
        {
            return startingHP;
        }

        public int getRespawnDelay()
        {
            return respawnDelay;
        }

        /// <summary>
        /// Checks if we can spawn a powerup
        /// </summary>
        /// <returns> True if the current amount of powerups is less than max powerups  /// </returns> 
        public bool canSpawnPowerup()
        {
            return powerups.Count < maxPowerups;
        }

        /// <summary>
        /// Tries to create a new powerup in theWorld, if possible.
        /// </summary>
        public void trySpawnPowerup()
        {
            if (powerupDelayFrame == currentPowerupDelay)
            {
                Powerup p = new Powerup();
                findEmptyPowerupSpawnLocation(p);
                this.updatePowerups(p.getID(), p);

                // Reset frame counter, adjust current cap to new one
                powerupDelayFrame = 0;
                currentPowerupDelay = powerupRandomizer.Next(0, maxPowerupDelay);
            }
            else
            {
                powerupDelayFrame++;

            }
        }

        /// <summary>
        /// This gives a provided powerup a random location to spawn without colliding with the wall
        /// </summary>
        public void findEmptyPowerupSpawnLocation(Powerup p)
        {
            int worldMin = (int)(worldSize * -0.5);
            int worldMax = (int)(worldSize * 0.5);

            Vector2D powRespawnLoc = new Vector2D(powerupRandomizer.Next(worldMin, worldMax), powerupRandomizer.Next(worldMin, worldMax));
            p.setLocation(powRespawnLoc);
            while (!p.canSpawnHere(walls))
            {
                powRespawnLoc = new Vector2D(powerupRandomizer.Next(worldMin, worldMax), powerupRandomizer.Next(worldMin, worldMax));
                p.setLocation(powRespawnLoc);
            }
        }

        /// <summary>
        /// This gives a provided tank a random location to spawn without colliding with the wall
        /// Used as a delegate in the cleanup tank method
        /// </summary>
        public void findEmptyTankSpawnLocation(int id)
        {
            int worldMin = (int)(worldSize * -0.5);
            int worldMax = (int)(worldSize * 0.5);

            Vector2D tankRespawnLoc = new Vector2D(tankRandomizer.Next(worldMin, worldMax), tankRandomizer.Next(worldMin, worldMax));
            tanks[id].setLocation(tankRespawnLoc);
            while (!tankMovementSuccess(tanks[id]))
            {
                tankRespawnLoc = new Vector2D(tankRandomizer.Next(worldMin, worldMax), tankRandomizer.Next(worldMin, worldMax));
                tanks[id].setLocation(tankRespawnLoc);
            }
        }

        public Dictionary<int, Tank> getTankList()
        {
            return tanks;
        }
        public Dictionary<int, Wall> getWallList()
        {
            return walls;
        }

        public Dictionary<int, Projectile> getProjList()
        {
            return projectiles;
        }

        public Dictionary<int, Powerup> getPowerupList()
        {
            return powerups;
        }

        public Dictionary<int, Beam> getBeamList()
        {
            return beams;
        }

        public Dictionary<int, DeathExplode> getDEList()
        {
            return deathExplodes;
        }

        public int getWorldSize()
        {
            return worldSize;
        }

        public int getMyID()
        {
            return myID;
        }

        public void setMyID(int i)
        {
            myID = i;
        }

        public string getMyName()
        {
            return myName;
        }

        public void setMyName(string n)
        {
            myName = n;
        }

        public void setWorldSize(int s)
        {
            worldSize = s;
        }

        /// <summary>
        /// Add/Modify the specified wall with the specified id in the world
        /// </summary>
        public void updateWalls(int id, Wall w)
        {
            walls[id] = w;
        }

        /// <summary>
        /// Add/Modify the specified tank with the specified id in the world
        /// </summary>
        public void updateTanks(int id, Tank t)
        {
            tanks[id] = t;
        }

        /// <summary>
        /// Removes the tank specified by the id from the world
        /// </summary>
        public void removeTank(int id)
        {
            tanks.Remove(id);
        }

       
        public void setTankDisconnected(int id)
        {
            tanks[id].setDisconnectionStatus(true);
        }

        /// <summary>
        /// Tries to move a tank given a specific player ID and movement direction.
        /// </summary>
        public void tryMoveTank(int id, string dir)
        {
            Tank t = tanks[id];
            Vector2D oldLocation = t.getLocation();
            Vector2D newLocation = null;

            // Get the new location the tank wants to move to
            switch (dir)
            {
                case "up":
                    newLocation = new Vector2D(oldLocation.GetX(), oldLocation.GetY() - tankSpeed);
                    t.setBodyOrientation(new Vector2D(0, 1));
                    break;
                case "down":
                    newLocation = new Vector2D(oldLocation.GetX(), oldLocation.GetY() + tankSpeed);
                    t.setBodyOrientation(new Vector2D(0, -1));
                    break;
                case "left":
                    newLocation = new Vector2D(oldLocation.GetX() - tankSpeed, oldLocation.GetY());
                    t.setBodyOrientation(new Vector2D(-1, 0));
                    break;
                case "right":
                    newLocation = new Vector2D(oldLocation.GetX() + tankSpeed, oldLocation.GetY());
                    t.setBodyOrientation(new Vector2D(1, 0));
                    break;
            }

            int halfWorldSize = (int)(worldSize * 0.5);
            // Check if the new location exceeds world boundaries
            if (newLocation.GetX() > halfWorldSize)
            {
                newLocation = new Vector2D(newLocation.GetX() - worldSize, newLocation.GetY());
            }
            else if (newLocation.GetX() < -halfWorldSize)
            {
                newLocation = new Vector2D(newLocation.GetX() + worldSize, newLocation.GetY());
            }
            else if (newLocation.GetY() > halfWorldSize)
            {
                newLocation = new Vector2D(newLocation.GetX(), newLocation.GetY() - worldSize);
            }
            else if (newLocation.GetY() < -halfWorldSize)
            {
                newLocation = new Vector2D(newLocation.GetX(), newLocation.GetY() + worldSize);
            }

            // Take the new location and make sure it isn't colliding with any walls
            // If it isn't, update that tank's location
            t.setLocation(newLocation);
            if (!tankMovementSuccess(t))
            {
                t.setLocation(oldLocation);
            }
            tanks[id] = t;
        }

        /// <summary>
        /// Tries to move all the projectiles and deletes projectiles if they collide 
        /// </summary>
        public void tryMoveAllProjectiles()
        {
            List<Projectile> currentProjectiles = new List<Projectile>(getProjList().Values);
            foreach(Projectile p in currentProjectiles)
            {
                Vector2D oldLocation = p.getLocation();
                Vector2D orientation = p.getOrientation();
                Vector2D newLocation = new Vector2D(oldLocation.GetX() + ((int)(projectileSpeed * orientation.GetX())), oldLocation.GetY() + ((int)(projectileSpeed * orientation.GetY())));
                bool hasCollided = false;

                int halfWS = (int)(worldSize * 0.5);
                // Remove projectile if it leaves world bounds
                if (newLocation.GetX() <= -halfWS || newLocation.GetX() >= halfWS || newLocation.GetY() <= -halfWS || newLocation.GetY() >= halfWS)
                {
                    projectiles[p.getID()].setDeathStatus(true);
                    continue;
                }

                // Check collisions with walls, if it collides then delete and continue the foreach
                p.setLocation(newLocation);
                foreach(Wall w in walls.Values)
                {
                    if (p.collidesWith(w))
                    {
                        projectiles[p.getID()].setDeathStatus(true);
                        hasCollided = true;
                        break;
                    }

                }
                if (hasCollided)
                {
                    continue;
                }

                // Check collisions with tanks, if it collides then delete, decrement tank hp/kill and give score
                foreach(Tank tank in tanks.Values)
                {
                    if (tank.getHp() > 0 && p.collidesWith(tank) && p.getOwnerID() != tank.getID()) 
                    {
                        projectiles[p.getID()].setDeathStatus(true);
                        if (tank.hitByProjectile())
                        {
                            tanks[p.getOwnerID()].incrementScore();
                        }
                        break;
                    }

                }
                
            }
        }

        /// <summary>
        /// Helper method for tryTankMovement method
        /// This method compares the given tank with all the walls to check if they overlap
        /// </summary>
        /// <returns> True if the tank didn't collide with any wall </returns>
        private bool tankMovementSuccess(Tank tank)
        {
            
            foreach(Wall w in walls.Values)
            {
                if (tank.collidesWith(w))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This just updates the turret angle of the player of the given ID
        /// </summary>
        public void updateTankTurretAngle(int id, Vector2D angle)
        {
            tanks[id].setTurretOrientation(angle);
        }

        /// <summary>
        /// Tries to fire a projectile from the tank of the given ID
        /// </summary>
        public void tryTankFireMain(int id)
        {
            if (tanks[id].getFireCounter() > projectileFireDelay)
            {
                Projectile p = new Projectile(tanks[id].getLocation(), tanks[id].getTurretOrientation(), id);
                projectiles.Add(p.getID(), p);
                tanks[id].resetFireCounter();
            }
        }

        /// <summary>
        /// Tries to fire a beam from the tank of the given ID
        /// </summary>
        public void tryTankFireAlt(int id)
        {
            if (tanks[id].hasPowerupAcquired())
            {
                Beam b = new Beam(tanks[id].getLocation(), tanks[id].getTurretOrientation(), id);
                beams.Add(b.getID(), b);
                tanks[id].setPowerupStatus(false);
            }
        }

        /// <summary>
        /// Add/Modify the specified tank with the specified id in the world
        /// </summary>
        public void updatePowerups(int id, Powerup p)
        {
            powerups[id] = p;
        }

        /// <summary>
        /// Removes the powerup specified by the id from the world
        /// </summary>
        public void removePowerup(int id)
        {
            powerups.Remove(id);
        }

        /// <summary>
        /// Add/Modify the specified tank with the specified id in the world
        /// </summary>
        public void updateProjectiles(int id, Projectile pr)
        {
            projectiles[id] = pr;
        }

        /// <summary>
        /// Removes the projectile specified by the id from the world
        /// </summary>
        public void removeProjectile(int id)
        {
            projectiles.Remove(id);
        }

        /// <summary>
        /// Add/Modify the specified tank with the specified id in the world
        /// </summary>
        public void updateBeams(int id, Beam b)
        {
            beams[id] = b;
        }

        /// <summary>
        /// Removes the beam specified by the id from the world
        /// </summary>
        public void removeBeams(int id)
        {
            beams.Remove(id);
        }

        /// <summary>
        /// Add/Modify the specified tank with the specified id in the world
        /// </summary>
        public void updateExplosion(int id, DeathExplode de)
        {
            deathExplodes[id] = de;
        }

        /// <summary>
        /// Removes the death explosion specified by the id from the world
        /// </summary>
        public void removeExplosion(int id)
        {
            deathExplodes.Remove(id);
        }

        /// <summary>
        /// Tries to increment the frame count of the beam given by the provided id. Returns true, if
        /// successful (did not reach frame cap), false otherwise
        /// </summary>
        public bool tryIncrementBeam(int id)
        {
            return beams[id].incrementFrame();
        }

        /// <summary>
        /// Tries to increment the frame count of the death explosion given by the provided id. Returns true, if
        /// successful (did not reach frame cap), false otherwise
        /// </summary>
        public bool tryIncrementExplosion(int id)
        {
            return deathExplodes[id].incrementFrame();
        }

        /// <summary>
        /// This method just checks if all the powerups in theWorld collide with all the tanks
        /// </summary>
        public void tankPowerupCollision()
        {
            Dictionary<int, Powerup> currentPowerups = getPowerupList();
            foreach(Powerup p in currentPowerups.Values)
            {
                foreach(Tank t in tanks.Values)
                {
                    if (p.collidesWith(t))
                    {
                        powerups[p.getID()].setDeathStatus(true);
                        t.setPowerupStatus(true);
                        break;
                    }
                    
                }
            }
        }

        /// <summary>
        /// This method just checks if all the beams in theWorld collide with all the tanks
        /// </summary>
        public void beamTankCollision()
        {
            foreach(Beam b in beams.Values)
            {
                foreach(Tank t in tanks.Values)
                {
                    if (t.getID() != b.getOwnerID() && Intersects((b.getLocation()), b.getOrientation(), t.getLocation(),30))
                    {
                        tanks[b.getOwnerID()].incrementScore();
                        t.setHP(0);
                        t.setDeathStatus(true);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

        /// <summary>
        /// Converts all the objects in theWorld to a JSON string
        /// </summary>
        /// <returns> JSON representation of theWorld </returns>
        public string toJSON()
        {
            string JSONWorld = "";

            foreach(Tank t in tanks.Values)
            {
                JSONWorld += JsonConvert.SerializeObject(t) + "\n";
            }

            foreach (Beam b in beams.Values)
            {
                JSONWorld += JsonConvert.SerializeObject(b) + "\n";
            }

            foreach (Powerup p in powerups.Values)
            {
                JSONWorld += JsonConvert.SerializeObject(p) + "\n";
            }

            foreach (Projectile p in projectiles.Values)
            {
                JSONWorld += JsonConvert.SerializeObject(p) + "\n";
            }

            return JSONWorld;
        }

        /// <summary>
        /// This removes disconnected tanks and dead objects from theWorld
        /// </summary>
        public void cleanupWorld()
        {
            Dictionary<int, Tank> currentTank = this.getTankList();
            Dictionary<int, Projectile> currentProjectile = this.getProjList();
            Dictionary<int, Powerup> currentPowerup = this.getPowerupList();
            foreach (Tank t in currentTank.Values)
            {
                if (t.hasDisconnected())
                {
                    tanks.Remove(t.getID());
                }

                else
                {
                    tanks[t.getID()].cleanupTank(findEmptyTankSpawnLocation);
                }
            }
            
            foreach(Projectile p in currentProjectile.Values)
            {
                if (p.hasDied())
                {
                    projectiles.Remove(p.getID());
                }

            }

            foreach (Powerup p in currentPowerup.Values)
            {
                if (p.hasDied())
                {
                    powerups.Remove(p.getID());
                }

            }
            beams.Clear();
        }


    }
}
