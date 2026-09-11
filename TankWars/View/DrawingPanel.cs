// Authors: Zachary Wallace, Abhiveer Sharma
// Original: Daniel Kopta(?)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    public class DrawingPanel : Panel
    {
        private World theWorld;

        private List<Image> tankBodies;
        private List<Image> tankTurrets;
        private List<Image> projectileColors;
        private List<Image> beamFrames;
        private List<Image> explosionFrames;
        private Image background;
        private Image wallSegment;
        private Image powerup;
        private Image deadTank;


        public DrawingPanel(World w)
        {
            DoubleBuffered = true;
            theWorld = w;

            // Build the list of tank bodies
            tankBodies = new List<Image>();
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\BlueTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\DarkTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\GreenTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\LightGreenTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\OrangeTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\PurpleTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\RedTank.png"));
            tankBodies.Add(Image.FromFile(@"..\..\..\Resources\Images\YellowTank.png"));

            // Build the list of tank turrets
            tankTurrets = new List<Image>();
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\BlueTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\DarkTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\GreenTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\LightGreenTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\OrangeTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\PurpleTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\RedTurret.png"));
            tankTurrets.Add(Image.FromFile(@"..\..\..\Resources\Images\YellowTurret.png"));

            // Drawing dead tanks
            deadTank = Image.FromFile(@"..\..\..\Resources\Images\tank-ruins.png");

            // Build the list of projectile colors
            projectileColors = new List<Image>();
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-blue.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-brown.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-green.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-grey.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-red.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-violet.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-white.png"));
            projectileColors.Add(Image.FromFile(@"..\..\..\Resources\Images\shot-yellow.png"));

            // Build the list of beam frames
            beamFrames = new List<Image>();
            beamFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Beam\beam_0.png"));
            beamFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Beam\beam_1.png"));
            beamFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Beam\beam_2.png"));
            beamFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Beam\beam_3.png"));
            beamFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Beam\beam_4.png"));

            // Build the list of explosion frames
            explosionFrames = new List<Image>();
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_0.png"));
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_1.png"));
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_2.png"));
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_3.png"));
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_4.png"));
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_5.png"));
            explosionFrames.Add(Image.FromFile(@"..\..\..\Resources\Images\Explosion\death-explode_6.png"));

            //Drawing the background
            background = Image.FromFile(@"..\..\..\Resources\Images\Background.png");

            //Drawing the walls
            wallSegment = Image.FromFile(@"..\..\..\Resources\Images\WallSprite.png");

            //Drawing the powerup
            powerup = Image.FromFile(@"..\..\..\Resources\Images\powerup-sprite.png");

        }


        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform((int)worldX, (int)worldY);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw tank bodies
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankBodyDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            const int tankSize = 60;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle tankRect = new Rectangle(-(tankSize / 2), -(tankSize / 2), tankSize, tankSize);

            e.Graphics.DrawImage(tankBodies[t.getID() % 8], tankRect);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw tank heads (turrets)
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankTurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            const int turretSize = 50;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle turretRect = new Rectangle(-(turretSize / 2), -(turretSize / 2), turretSize, turretSize);

            e.Graphics.DrawImage(tankTurrets[t.getID() % 8], turretRect);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw tank remains
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void DeadTankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            const int tankSize = 60;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle tankRect = new Rectangle(-(tankSize / 2), -(tankSize / 2), tankSize, tankSize);

            e.Graphics.DrawImage(deadTank, tankRect);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw tank health bars
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void HealthbarDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            const int backRectHeight = 20;
            const int backRectWidth = 70;

            const int hpRectHeight = 15;
            int hpRectWidth = 20 * t.getHp();

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (System.Drawing.SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
            using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
            using (System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            {
                Rectangle backRect = new Rectangle(-(backRectWidth / 2), -(backRectHeight / 2), backRectWidth, backRectHeight);
                Rectangle hpRect = new Rectangle(-(hpRectWidth / 2), -(hpRectHeight / 2), hpRectWidth, hpRectHeight);

                // Draw Health bar background
                e.Graphics.FillRectangle(blackBrush, backRect);

                // Draw health bar
                if (t.getHp() == 3)
                {
                    e.Graphics.FillRectangle(greenBrush, hpRect);

                }
                else if (t.getHp() == 2)
                {
                    e.Graphics.FillRectangle(yellowBrush, hpRect);
                }
                else
                {
                    e.Graphics.FillRectangle(redBrush, hpRect);
                }
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw tank names
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void NameDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;

            string nameLabel = t.getName() + " - " + t.getScore();

            int nameWidth = 6 * nameLabel.Length;
            const int nameHeight = 20;

            using (System.Drawing.SolidBrush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            {
                Rectangle nameRect = new Rectangle(-(nameWidth / 2), -(nameHeight / 2), nameWidth, nameHeight);
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString(nameLabel, DefaultFont, blackBrush, nameRect);
            }

        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw powerups
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Powerup p = o as Powerup;

            const int powerupSize = 40;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle powerupRect = new Rectangle(-(powerupSize / 2), -(powerupSize / 2), powerupSize, powerupSize);

            e.Graphics.DrawImage(powerup, powerupRect);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw projectiles
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile pr = o as Projectile;

            const int projSize = 30;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle projRect = new Rectangle(-(projSize / 2), -(projSize / 2), projSize, projSize);

            e.Graphics.DrawImage(projectileColors[pr.getID() % 8], projRect);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw beams
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;

            const int beamSize = 20;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle beamRect = new Rectangle(beamSize, -(beamSize / 2), theWorld.getWorldSize(), beamSize);

            e.Graphics.DrawImage(beamFrames[b.getCurrentFrame() / b.getTimeMult()], beamRect);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform to draw death explosions
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void ExplodeDrawer(object o, PaintEventArgs e)
        {
            DeathExplode de = o as DeathExplode;

            const int DESize = 150;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle explodeRect = new Rectangle(-(DESize / 2), -(DESize / 2), DESize, DESize);

            e.Graphics.DrawImage(explosionFrames[de.getCurrentFrame() / de.getTimeMult()], explodeRect);
        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            // Stop the initial onpaint call from occuring
            if (theWorld.getMyID() == -1)
            {
                return;
            }

            int viewSize = theWorld.getViewSize();

            Dictionary<int, Tank> currentTanks = theWorld.getTankList();
            Dictionary<int, Wall> currentWalls = theWorld.getWallList();
            Dictionary<int, Projectile> currentProjs = theWorld.getProjList();
            Dictionary<int, Powerup> currentPowers = theWorld.getPowerupList();
            Dictionary<int, Beam> currentBeams = theWorld.getBeamList();
            Dictionary<int, DeathExplode> currentExplosions = theWorld.getDEList();

            // Center the view on the player's location
            if (currentTanks.Values.Count != 0)
            {
                double playerX = currentTanks[theWorld.getMyID()].getLocation().GetX();
                double playerY = currentTanks[theWorld.getMyID()].getLocation().GetY();
                e.Graphics.TranslateTransform((float)(-playerX + (viewSize / 2)), (float)(-playerY + (viewSize / 2)));
            }

            lock (theWorld)
            {
                int backgroundSize = theWorld.getWorldSize();

                // Draw the background
                e.Graphics.DrawImage(background,new Rectangle(-(backgroundSize/2),-(backgroundSize/2),backgroundSize,backgroundSize));

                //Draw the walls
                const int wallSize = 50;
                foreach(Wall wall in currentWalls.Values)
                {
                    Vector2D p1 = wall.getP1();
                    Vector2D p2 = wall.getP2();
                    
                    //comparing x-cood of p1 and p2 to see if they are vertically aligned
                    if(p1.GetX().Equals(p2.GetX()))
                    {
                        for (double i = Math.Min(p1.GetY(), p2.GetY()); i <= Math.Max(p1.GetY(), p2.GetY()) ; i += wallSize)
                        {
                            e.Graphics.DrawImage(wallSegment, new Rectangle((int)p1.GetX() - (wallSize/2),(int)i-(wallSize/2), wallSize, wallSize));
                        }
                    }
                    else
                    {
                        for (double i = Math.Min(p1.GetX(), p2.GetX()); i <= Math.Max(p1.GetX(), p2.GetX()); i += wallSize)
                        {
                            e.Graphics.DrawImage(wallSegment, new Rectangle((int)i - (wallSize / 2), (int)p1.GetY() - (wallSize / 2), wallSize, wallSize));
                        }
                    }

                }

                // Draw the tanks
                const int tankSize = 60;
                foreach (Tank tank in currentTanks.Values)
                {
                    if (tank.getHp() > 0)
                    {
                        DrawObjectWithTransform(e, tank, tank.getLocation().GetX(), tank.getLocation().GetY(), tank.getBodyOrientation().ToAngle(), TankBodyDrawer);
                        DrawObjectWithTransform(e, tank, tank.getLocation().GetX(), tank.getLocation().GetY(), tank.getTurretOrientation().ToAngle(), TankTurretDrawer);
                    }
                    else
                    {
                        DrawObjectWithTransform(e, tank, tank.getLocation().GetX(), tank.getLocation().GetY(), 0, DeadTankDrawer);
                    }
                    DrawObjectWithTransform(e, tank, tank.getLocation().GetX(), tank.getLocation().GetY() + (tankSize - 20), 0, HealthbarDrawer);
                    DrawObjectWithTransform(e, tank, tank.getLocation().GetX(), tank.getLocation().GetY() - (tankSize/2 + 5), 0, NameDrawer);
                }

                // Draw the death explosions
                foreach (DeathExplode de in currentExplosions.Values)
                {
                    DrawObjectWithTransform(e, de, de.getPosition().GetX(), de.getPosition().GetY(), 0, ExplodeDrawer);
                }

                // Draw the powerups
                foreach (Powerup pow in currentPowers.Values)
                {
                    DrawObjectWithTransform(e, pow, pow.getLocation().GetX(), pow.getLocation().GetY(), 0, PowerupDrawer);
                }

                // Draw the projectiles
                foreach (Projectile proj in currentProjs.Values)
                {
                    DrawObjectWithTransform(e, proj, proj.getLocation().GetX(), proj.getLocation().GetY(), proj.getOrientation().ToAngle(), ProjectileDrawer);
                }

                // Draw beams
                foreach (Beam b in currentBeams.Values)
                {
                    DrawObjectWithTransform(e, b, b.getLocation().GetX(), b.getLocation().GetY(), b.getOrientation().ToAngle(), BeamDrawer);
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

    }
}

