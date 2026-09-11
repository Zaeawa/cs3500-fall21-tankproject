// Authors: Zachary Wallace, Abhiveer Sharma

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetworkUtil;
using Newtonsoft.Json;

namespace TankWars
{
    /// <summary>
    /// Controller class for the View of the client. Handles networking, updating the world, and control request concerns.
    /// </summary>
    public class GameController
    {
        private World theWorld;
        private SocketState state;

        private List<Keys> keyQueue;
        private HashSet<Keys> keyQueueDuplicateCheck;

        private bool mousePressed = false;
        private MouseButtons currentMouse;

        private Vector2D mousePosition = new Vector2D(0, 0);

        private bool notHasIdWorldSize = true;

        public delegate void ServerUpdateHandler();
        public delegate void ErrorOccured();

        public event ServerUpdateHandler UpdateArrived;
        public event ErrorOccured ErrorEvent;

        /// <summary>
        /// Constructor for the class. Takes in a provided view size
        /// </summary>
        /// <param name="vSize"></param>
        public GameController(int vSize)
        {
            theWorld = new World(vSize);
            keyQueue = new List<Keys>();
            keyQueueDuplicateCheck = new HashSet<Keys>();
        }

        /// <summary>
        /// Returns the world created by the class instance
        /// </summary>
        /// <returns></returns>
        public World getWorld()
        {
            return theWorld;
        }

        /// <summary>
        /// Called by the view to connect to the provided server with the provided player name
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="address"></param>
        public void connectToServer(string playerName, string address)
        {
            theWorld.setMyName(playerName);
            Networking.ConnectToServer(initialConnectionCallback, address, 11000);
        }

        /// <summary>
        /// Callback when a connection or failed connection occurs
        /// </summary>
        /// <param name="s"></param>
        private void initialConnectionCallback(SocketState s)
        {
            if (s.ErrorOccurred)
            {
                ErrorEvent();
                if (s.TheSocket != null)
                {
                    s.TheSocket.Close();
                }
                return;
            }

            state = s;

            // Change the OnNetworkAction (for accepting the next server messages)
            state.OnNetworkAction = UpdateCameFromServer;
            // Send the player name
            Networking.Send(state.TheSocket, theWorld.getMyName() + "\n");
            // Ask the server for the next info (the player's id and the world size)
            Networking.GetData(state);
        }

        /// <summary>
        /// Main callback for accepting new messages from the server. Takes server messages, deconverts from JSON, and uses the
        /// data appropriately to correctly update the world to the current state as is in the server
        /// </summary>
        /// <param name="s"></param>
        private void UpdateCameFromServer(SocketState s)
        {
            if (s.ErrorOccurred)
            {
                ErrorEvent();
                if (s.TheSocket != null)
                {
                    s.TheSocket.Close();
                }
                return;
            }

            List<Tank> newTanks = new List<Tank>();
            List<Powerup> newPowerups = new List<Powerup>();
            List<Projectile> newProjectiles = new List<Projectile>();
            List<Beam> newBeams = new List<Beam>();
            List<Wall> newWalls = new List<Wall>();

            // Deserialize the message
            List<string> messageList = ProcessMessages(s);

            if (notHasIdWorldSize)
            {
                theWorld.setMyID(int.Parse(messageList[0]));
                theWorld.setWorldSize(int.Parse(messageList[1]));
                notHasIdWorldSize = false;
            }

            // Add each JSON object to their correct list. If it is an id/world size, set the values
            foreach(string JObj in messageList)
            {
                if (JObj.Contains("tank"))
                {
                    Tank t = JsonConvert.DeserializeObject<Tank>(JObj);
                    newTanks.Add(t);
                }
                else if (JObj.Contains("power"))
                {
                    Powerup p = JsonConvert.DeserializeObject<Powerup>(JObj);
                    newPowerups.Add(p);
                }
                else if (JObj.Contains("proj"))
                {
                    Projectile pr = JsonConvert.DeserializeObject<Projectile>(JObj);
                    newProjectiles.Add(pr);
                }
                else if (JObj.Contains("beam"))
                {
                    Beam b = JsonConvert.DeserializeObject<Beam>(JObj);
                    newBeams.Add(b);
                }
                else if (JObj.Contains("wall"))
                {
                    Wall w = JsonConvert.DeserializeObject<Wall>(JObj);
                    newWalls.Add(w);
                }
            }

            // The server is not required to send updates about every object,
            // so we update our local copy of the world only for the objects that
            // the server gave us an update for.

            lock (theWorld)
            {
                foreach (Tank t in newTanks)
                {
                    if (t.hasDisconnected())
                    {
                        DeathExplode de = new DeathExplode(t.getLocation(), t.getID());
                        theWorld.updateExplosion(de.getID(), de);
                        theWorld.removeTank(t.getID());
                    }
                    else if (t.hasDied())
                    {
                        DeathExplode de = new DeathExplode(t.getLocation(), t.getID());
                        theWorld.updateExplosion(de.getID(), de);
                    }
                    else
                    {
                        theWorld.updateTanks(t.getID(), t);
                    }
                }
                List<int> currentExplodeKeys = new List<int>(theWorld.getDEList().Keys);
                foreach (int id in currentExplodeKeys)
                {
                    if (theWorld.tryIncrementExplosion(id) == false)
                    {
                        theWorld.removeExplosion(id);
                    }
                }

                foreach (Powerup p in newPowerups)
                {
                    if (p.hasDied())
                    {
                        theWorld.removePowerup(p.getID());
                    }
                    else
                    {
                        theWorld.updatePowerups(p.getID(), p);
                    }
                }

                foreach (Projectile pr in newProjectiles)
                {
                    if (pr.hasDied())
                    {
                        theWorld.removeProjectile(pr.getID());
                    }
                    else
                    {
                        theWorld.updateProjectiles(pr.getID(), pr);
                    }
                }

                foreach (Beam b in newBeams)
                {
                    b.correctAngle();
                    theWorld.updateBeams(b.getID(), b);
                }
                List<int> currentBeamKeys = new List<int>(theWorld.getBeamList().Keys);
                foreach (int id in currentBeamKeys)
                {
                    if (theWorld.tryIncrementBeam(id) == false)
                    {
                        theWorld.removeBeams(id);
                    }
                }

                foreach (Wall w in newWalls)
                {
                    theWorld.updateWalls(w.getID(), w);
                }
            }

            // Notify the view to redraw the new world
            if (UpdateArrived != null)
                UpdateArrived();

            // For whatever user inputs happened during the last frame,
            // process them.
            ProcessInputs(s);

            // Start looking for new data again
            Networking.GetData(s);
        }

        /// <summary>
        /// Process any buffered messages separated by '\n'
        /// Display them, then remove them from the buffer.
        /// </summary>
        /// <param name="state"></param>
        private static List<string> ProcessMessages(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            List<string> finalParts = new List<string>();

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

                // Save into the final collection
                finalParts.Add(p);

                // Then remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);
            }

            return finalParts;
        }

        /// <summary>
        /// Checks inputs in order to construct a control message for the server 
        /// </summary>
        private void ProcessInputs(SocketState s)
        {
            // Make a CMD object here (w/ default vals)
            ControlCMD newCommand = new ControlCMD();

            if (keyQueue.Count != 0)
            {
                switch(keyQueue[keyQueue.Count - 1])
                {
                    case Keys.W:
                        newCommand.setMoveDir("up");
                        break;
                    case Keys.A:
                        newCommand.setMoveDir("left");
                        break;
                    case Keys.S:
                        newCommand.setMoveDir("down");
                        break;
                    case Keys.D:
                        newCommand.setMoveDir("right");
                        break;
                }
            }

            if (mousePressed)
            {
                switch(currentMouse)
                {
                    case MouseButtons.Left:
                        newCommand.setFireMode("main");
                        break;
                    case MouseButtons.Right:
                        newCommand.setFireMode("alt");
                        break;
                }
            }

            //Get the mouse position to set the turret orientation
            Vector2D aimOrientation = new Vector2D(mousePosition.GetX() - (theWorld.getViewSize()/2), mousePosition.GetY() - (theWorld.getViewSize()/2));
            aimOrientation.Normalize();
            aimOrientation.Clamp();
            newCommand.setTurretDir(aimOrientation);

            // Send the finished command
            Networking.Send(s.TheSocket, JsonConvert.SerializeObject(newCommand) + "\n");
        }

        /// <summary>
        /// Handles a key press by adding it to the key queue
        /// </summary>
        public void HandleKeyRequest(Keys k)
        {
            // If the key is already queued, do not add it
            if (keyQueueDuplicateCheck.Add(k))
            {
                keyQueue.Add(k);
            }
        }

        /// <summary>
        /// Handles reoving keys that are no longer pressed from the key queue
        /// </summary>
        public void CancelKeyRequest(Keys k)
        {
            keyQueue.Remove(k);
            keyQueueDuplicateCheck.Remove(k);
        }

        /// <summary>
        /// Handles mouse clicks
        /// </summary>
        public void HandleMouseRequest(MouseButtons m)
        {
            mousePressed = true;
            currentMouse = m;
        }

        /// <summary>
        /// Removes mouse clicks from being active
        /// </summary>
        public void CancelMouseRequest()
        {
            mousePressed = false;
        }

        /// <summary>
        /// Called by the view whenever the mouse moves on the drawing panel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseMoved(int x, int y)
        {
            mousePosition = new Vector2D(x, y);
        }
    }
}
