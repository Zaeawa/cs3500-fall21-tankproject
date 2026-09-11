/// @Authors Zachary Dean Wallace and Abhiveer Sharma
using System;
using Newtonsoft.Json;
using TankWars;
using NetworkUtil;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Xml;

namespace Server
{
    /// <summary>
    /// Runs a console that hosts TankWars game
    /// </summary>
    class Server
    {
        World theWorld;
        Dictionary<long, SocketState> clients;
        Dictionary<int, string> newPlayers;
        List<long> disconnectedClients;
        Dictionary<int, ControlCMD> clientRequests;

        private int nextPlayerID = 0;

        static void Main(string[] args)
        {
            Server server = new Server();
            server.startServer();
            Console.Read();
        }
        /// <summary>
        /// This is the constructor of this class
        /// </summary>

        public Server()
        {
            theWorld = readSettings();
            clients = new Dictionary<long, SocketState>();
            clientRequests = new Dictionary<int, ControlCMD>();
            newPlayers = new Dictionary<int, string>();
            disconnectedClients = new List<long>();
        }

        /// <summary>
        /// Runs networking, starts server and creates a separate thread for updating the theWorld
        /// </summary>
        public void startServer()
        {
            Networking.StartServer(newClientConnected, 11000);

            Thread worldThread = new Thread(updateWorldAndSend);
            worldThread.Start();

            Console.WriteLine("Server is running");
        }

        /// <summary>
        /// Updates all components of theWorld, for example - powerup spawning and projectile movement.
        /// Send theWorld to all the connected clients
        /// Uses a busy loop to stall computation based on the frame rate.
        /// </summary>
        private void updateWorldAndSend()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // Begin the update loop
            while (true)
            {
                while (watch.ElapsedMilliseconds < theWorld.getMSPerFrame())
                { /* do nothing */ }
                watch.Restart();
                
                // Update the world, send clients the world
                lock (theWorld)
                {
                    // Spawn new players
                    foreach(int id in newPlayers.Keys)
                    {
                        theWorld.updateTanks(id, new Tank(id, newPlayers[id], theWorld.getStartingHealth(),theWorld.getRespawnDelay()));
                        theWorld.findEmptyTankSpawnLocation(id);
                        newPlayers.Remove(id);
                    }

                    // Spawn new powerups
                    if (theWorld.canSpawnPowerup())
                    {
                        theWorld.trySpawnPowerup();
                    }

                    // Set disconnected players to be disconnected
                    foreach(long id in disconnectedClients)
                    {
                        theWorld.setTankDisconnected((int)id);
                    }
                    disconnectedClients.Clear();
                   

                    // Calculate tank control and tank x wall collision (look at control commands received in this frame)
                    foreach(int id in clientRequests.Keys)
                    {
                        // Ignore a client's command if their tank is dead
                        if (theWorld.getTankList()[id].getHp() <= 0)
                        {
                            continue;
                        }

                        ControlCMD cmd = clientRequests[id];

                        if (cmd.getMoveDir() != "none")
                        {
                            theWorld.tryMoveTank(id, cmd.getMoveDir());
                        }

                        theWorld.updateTankTurretAngle(id, cmd.getTurretDir());

                        if (cmd.getFireMode() != "none")
                        {
                            if (cmd.getFireMode() == "main")
                            {
                                theWorld.tryTankFireMain(id);
                            }
                            else
                            {
                                theWorld.tryTankFireAlt(id);
                            }
                        }
                    }
                    clientRequests.Clear(); // clear this instance of client requests (PUT IN THE CLEANUP METHOD AT THE BOTTOM)

                    // Calculate projectile movement and tank x projectile collision
                    theWorld.tryMoveAllProjectiles();

                    // Calculate tank x powerup collision
                    theWorld.tankPowerupCollision();

                    // Calculate tank x beam collision
                    theWorld.beamTankCollision();
                    // Pack the world info into a JSON string
                    string JSONWorld = theWorld.toJSON();
                    // Send the world to all the clients on this frame
                    foreach(SocketState s in clients.Values)
                    {
                        Networking.Send(s.TheSocket, JSONWorld);
                    }
                    // Cleanup (Remove disconnected tanks/dead objects, etc.)
                    theWorld.cleanupWorld();
                }
            }
        }

        /// <summary>
        /// Callback when a new client connects to the server.
        /// </summary>
        private void newClientConnected(SocketState s)
        {
            if (s.ErrorOccurred)
            {
                // Don't connect the client at all
                return;
            }

            s.OnNetworkAction = nameReceived;

            Networking.GetData(s);
        }

        /// <summary>
        /// Callback for when a client sends their name and if successful, client gets added to the collection of clients
        /// </summary>
        private void nameReceived(SocketState s)
        {
            if (s.ErrorOccurred)
            {
                return;
            }

            // Get the name from the client
            string sData = s.GetData();
            string newPlayerName = sData.Substring(0, sData.Length - 1);
            s.RemoveData(0, sData.Length);

            lock (theWorld)
            {
                // Add the new player to the set of new players
                int newPlayerID = nextPlayerID;
                newPlayers[nextPlayerID] = newPlayerName;
                nextPlayerID++;
            
                // Send the client the world data (walls, etc.)
                Networking.Send(s.TheSocket, newPlayerID + "\n" + theWorld.getWorldSize() + "\n");
                Networking.Send(s.TheSocket, theWorld.getJSONWallRepresentation());

                // Add this client to the dictionary of clients to send world data to
                clients[s.ID] = s;

                Console.WriteLine("Client " + s.ID + " connected");
            }

            s.OnNetworkAction = messageReceived;

            Networking.GetData(s);
        }

        /// <summary>
        /// Callback for when a client send a control command
        /// Removes the disconnected client from the collection of clients
        /// </summary>
        private void messageReceived(SocketState s)
        {
            // Remove the client if they aren't still connected
            if (s.ErrorOccurred)
            {
                Console.WriteLine("Client " + s.ID + " disconnected");
                lock (theWorld)
                {
                    clients.Remove(s.ID);
                    disconnectedClients.Add(s.ID);
                }
                return;
            }

            processMessage(s);

            // Continue the event loop that receives messages from this client
            Networking.GetData(s);

        }

        /// <summary>
        /// Process message containing all control commands from the client
        /// </summary>
        private void processMessage(SocketState s)
        {
            string totalData = s.GetData();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // Loop until we have processed all messages.
            // We may have received more than one.
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                ControlCMD clientReq = JsonConvert.DeserializeObject<ControlCMD>(p);

                lock (theWorld)
                {
                    clientRequests[(int)s.ID] = clientReq;
                }

                // Remove it from the SocketState's growable buffer
                s.RemoveData(0, p.Length);
            }
        }

        /// <summary>
        /// Reads the xml settings file and creates a new world object for the server 
        /// </summary>
        /// <returns></returns>
        private World readSettings()
        {
            int worldSize = 500;
            int MSPerFrame = 17;
            int startingHP = 3;
            int respawnDelay = 300;
            int projectileSpeed = 25;
            int projectileFireDelay = 80;
            int tankSpeed = 3;
            int maxPowerups = 2;
            int maxPowerupDelay = 1650;
            List<Wall> wallHolder = new List<Wall>();

            using (XmlReader reader = XmlReader.Create(@"..\..\..\..\Resources\settings.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name) 
                        {
                            case "UniverseSize":
                                reader.Read();
                                worldSize = int.Parse(reader.Value);
                                break;
                            case "MSPerFrame":
                                reader.Read();
                                MSPerFrame = int.Parse(reader.Value);
                                break;
                            case "FramesPerShot":
                                reader.Read();
                                projectileFireDelay = int.Parse(reader.Value);
                                break;
                            case "RespawnRate":
                                reader.Read();
                                respawnDelay = int.Parse(reader.Value);
                                break;
                            case "MaximumHealth":
                                reader.Read();
                                startingHP = int.Parse(reader.Value);
                                break;
                            case "ShotSpeed":
                                reader.Read();
                                projectileSpeed = int.Parse(reader.Value);
                                break;
                            case "PlayerSpeed":
                                reader.Read();
                                tankSpeed = int.Parse(reader.Value);
                                break;
                            case "MaximumPowerups":
                                reader.Read();
                                maxPowerups = int.Parse(reader.Value);
                                break;
                            case "PowerupRate":
                                reader.Read();
                                maxPowerupDelay = int.Parse(reader.Value);
                                break;
                            case "Wall":
                                reader.Read();
                                reader.Read();
                                // repeat twice to get p1 and p2
                                int p1_x = 0, p1_y = 0, p2_x = 0, p2_y = 0;

                                for (int j = 0; j < 2; j++)
                                {
                                    if (reader.Name == "p1")
                                    {
                                        reader.ReadToFollowing("x");
                                        p1_x = reader.ReadElementContentAsInt();
                                        p1_y = reader.ReadElementContentAsInt();
                                        reader.Read();
                                        reader.Read();
                                    }
                                    else if (reader.Name == "p2")
                                    {
                                        reader.ReadToFollowing("x");
                                        p2_x = reader.ReadElementContentAsInt();
                                        p2_y = reader.ReadElementContentAsInt();
                                        reader.Read();
                                        reader.Read();
                                    }
                                }

                                Wall w = new Wall(p1_x, p1_y, p2_x, p2_y);
                                wallHolder.Add(w);
                                break;
                        }
                    }
                }
            }

            return new World(worldSize, MSPerFrame, startingHP, respawnDelay, projectileSpeed, projectileFireDelay, tankSpeed, maxPowerups, maxPowerupDelay, wallHolder);
        }
    }
}
