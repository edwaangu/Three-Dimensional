using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Three_Dimensional
{
    public partial class GameScreen : UserControl
    {
        /**
         * Three-Dimensional - Created by Ted Angus (2021/10/31)
         * 
         * About:
         * - A program that will soon become a very cool basic 3d engine to be used in other programs!
         * 
         * Things it can do right now:
         * - Display 2D top down mode with many lines
         * - Basic 3D mode
         * 
         * 
         * To Do Soon:
         * - Closeness of points based on a bulge from the player circle
         * - Don't show only if beyond 90 fov
         * - Fix any bugs that I have not found yet
         * 
         * To Do FAR IN THE FUTURE:
         * - Player Z Coordinate
         * - Two Direction Axises (Similar to Minecraft)
         * 
         * 
         * Version History:
         * 
         * v1.0.5a:
         * - Very very very alpha Z Axis (Needs input from XY)
         * 
         * 
         * v1.0.4:
         * - Fixed offscreen issues (For the most part, ceiling or floor planes will not work yet)
         * - Saved a bunch of code and memory (probably)
         * - Direction indicating with yellow line in 2D
         * 
         * v1.0.3:
         * - Improved movement
         * - Better insights into position of thing
         * 
         * v1.0.2:
         * - This header comment
         * - Lots of comments added to the program for anyone who is interested in viewing, and for myself to remember what everything does.
         * - Even more lines of code saved and memory saved
         * 
         * v1.0.1:
         * - Bug fixes
         * - Less code
         * 
         * v1.0:
         * - Point3 and Plane3 classes
         * - Player (direction, x and y)
         * - Field of view
         * - 2D Topdown Mode
         * - 3D Mode (Alpha)
         * - Signature FPS Counter
         * - Basic Movement
         *
        */




        // Main variables
        double rad2Deg = 180 / Math.PI;
        int mode = 1; // 2 is 2d TOP, 3 is 3d
        bool[] theKeys = new bool[256];

        // Player related variables
        Player p = new Player(-20, 100, 0, new PointF(-90, -90));
        PointF fov = new PointF(40, 30);

        // Object related variables
        List<Plane3> planes = new List<Plane3>();

        // Framerate related variables
        int framesSinceLastSecond = 0;
        int accurateSec = -1;
        int lastSec = -1;
        int fps = 0;

        int zapdist = 400;

        // Functions
        double directionFromPoint(double x, double y, double px, double py)
        {
            double value = -Math.Atan2(py - y, px - x) + 90 / rad2Deg;
            return value;
        }

        double distanceFromPoint(double x1, double y1, double x2, double y2)
        {
            double value = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return value;
        }

        double whereInFov(double pDir, double distAway, double playerDir, double theFov)
        {
            double whereShould = 0;

            whereShould = (pDir - playerDir);
            if(Math.Abs(whereShould) > Math.Abs(whereShould - 360))
            {
                whereShould -= 360;
            }
            if (Math.Abs(whereShould) > Math.Abs(whereShould + 360))
            {
                whereShould += 360;
            }
            return -whereShould / theFov;
        }

        public GameScreen()
        {
            InitializeComponent();

            // Add planes of four points
            planes.Add(new Plane3(new Point3[] { new Point3(50, -50, -50), new Point3(50, 50, -50), new Point3(50, 50, 50), new Point3(50, -50, 50) }));
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {

        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Calculate minimum fov and maximum fov
            double minfovX = ((-fov.X - p.direction.X) + 90) % 360 - 90;
            double maxfovX = ((fov.X - p.direction.X) + 90) % 360 - 90;

            double minfovY = ((-fov.Y - p.direction.Y) + 90) % 360 - 90;
            double maxfovY = ((fov.Y - p.direction.Y) + 90) % 360 - 90;

            if (minfovX < -90)
            {
                minfovX += 360;
            }
            if (maxfovX < -90)
            {
                maxfovX += 360;
            }

            // 2D TOPDOWN Z MODE
            if (mode == 1)
            {

                // Translation from 0, 0
                e.Graphics.TranslateTransform(450, 300);

                // Player
                e.Graphics.FillEllipse(new SolidBrush(Color.Black), -5 + Convert.ToSingle(p.x), -5 + Convert.ToSingle(p.z), 10, 10);

                // Planes
                foreach (Plane3 pl in planes)
                {

                    e.Graphics.DrawPolygon(new Pen(Color.Black), new PointF[] { new PointF(pl.plist[0].x, pl.plist[0].z), new PointF(pl.plist[1].x, pl.plist[1].z), new PointF(pl.plist[2].x, pl.plist[2].z), new PointF(pl.plist[3].x, pl.plist[3].z) });
                    e.Graphics.FillPolygon(new SolidBrush(Color.Gray), new PointF[] { new PointF(pl.plist[0].x, pl.plist[0].z), new PointF(pl.plist[1].x, pl.plist[1].z), new PointF(pl.plist[2].x, pl.plist[2].z), new PointF(pl.plist[3].x, pl.plist[3].z) });

                    // Calculate Lines
                    for (int i = 0; i < pl.plist.Count(); i++)
                    {
                        // Calculate Distance and Direction from point
                        double dirFromPoint = directionFromPoint(p.x, p.z, pl.plist[i].x, pl.plist[i].z); // Calculate direction from point
                        double distFromPoint = distanceFromPoint(p.x, p.z, pl.plist[i].x, pl.plist[i].z); // Calculate distance from point

                        double whereFov = whereInFov(dirFromPoint * rad2Deg, distFromPoint, -p.direction.Y, fov.Y); // FOV

                        // Calculate whether the point should be showing based on the player's fov
                        bool shouldBeShowing = Math.Abs(whereFov) <= 1 ? true : false;

                        // Draw a line based on the direction of player and point
                        e.Graphics.DrawLine(new Pen(Color.Red), Convert.ToSingle(p.x), Convert.ToSingle(p.z), Convert.ToSingle(p.x) + Convert.ToSingle(zapdist * Math.Sin(dirFromPoint)), Convert.ToSingle(p.z) + Convert.ToSingle(zapdist * Math.Cos(dirFromPoint)));

                        // Draw a line based on the distance of player and point
                        e.Graphics.DrawLine(new Pen(shouldBeShowing ? Color.LimeGreen : Color.DarkRed), Convert.ToSingle(p.x), Convert.ToSingle(p.z), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.z) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));
                        e.Graphics.DrawString($"{whereInFov(dirFromPoint * rad2Deg, dirFromPoint, -p.direction.Y, fov.Y)} : : {dirFromPoint * rad2Deg}", DefaultFont, new SolidBrush(Color.Red), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.z) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));
                    }
                }

                // FOV Lines
                e.Graphics.DrawLine(new Pen(Color.DarkBlue, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.z), Convert.ToSingle(p.x + zapdist * Math.Sin((-fov.Y - p.direction.Y) / rad2Deg)), Convert.ToSingle(p.z + zapdist * Math.Cos((-fov.Y - p.direction.Y) / rad2Deg))); // Minimum FOV Line
                e.Graphics.DrawLine(new Pen(Color.Blue, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.z), Convert.ToSingle(p.x + zapdist * Math.Sin((fov.Y - p.direction.Y) / rad2Deg)), Convert.ToSingle(p.z + zapdist * Math.Cos((fov.Y - p.direction.Y) / rad2Deg))); // Maximum FOV Line


                e.Graphics.DrawLine(new Pen(Color.Yellow, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.z), Convert.ToSingle(p.x + zapdist * Math.Sin((-p.direction.Y) / rad2Deg)), Convert.ToSingle(p.z + zapdist * Math.Cos((-p.direction.Y) / rad2Deg))); // Dir Line

                // FOV Text
                e.Graphics.DrawString($"{minfovY}", DefaultFont, new SolidBrush(Color.DarkBlue), Convert.ToSingle(p.x + zapdist * Math.Sin((-fov.Y - p.direction.Y) / rad2Deg)), Convert.ToSingle(p.z + zapdist * Math.Cos((-fov.Y - p.direction.Y) / rad2Deg))); // Minimum FOV
                e.Graphics.DrawString($"{maxfovY}", DefaultFont, new SolidBrush(Color.Blue), Convert.ToSingle(p.x + zapdist * Math.Sin((fov.Y - p.direction.Y) / rad2Deg)), Convert.ToSingle(p.z + zapdist * Math.Cos((fov.Y - p.direction.Y) / rad2Deg))); // Maximum FOV

                e.Graphics.DrawString($"{-p.direction.Y}", DefaultFont, new SolidBrush(Color.Yellow), Convert.ToSingle(p.x + zapdist * Math.Sin((-p.direction.Y) / rad2Deg)), Convert.ToSingle(p.z + zapdist * Math.Cos((-p.direction.Y) / rad2Deg)));

                // Finish Transform back to 0, 0
                e.Graphics.ResetTransform();
            }

            // 2D TOP DOWN X MODE
            if (mode == 2)
            {
                // Translation from 0, 0
                e.Graphics.TranslateTransform(450, 300);

                // Player
                e.Graphics.FillEllipse(new SolidBrush(Color.Black), -5 + Convert.ToSingle(p.x), -5 + Convert.ToSingle(p.y), 10, 10);

                // Planes
                foreach (Plane3 pl in planes)
                {

                    e.Graphics.DrawPolygon(new Pen(Color.Black), new PointF[]{ new PointF(pl.plist[0].x, pl.plist[0].y), new PointF(pl.plist[1].x, pl.plist[1].y), new PointF(pl.plist[2].x, pl.plist[2].y), new PointF(pl.plist[3].x, pl.plist[3].y)});
                    e.Graphics.FillPolygon(new SolidBrush(Color.Gray), new PointF[] { new PointF(pl.plist[0].x, pl.plist[0].y), new PointF(pl.plist[1].x, pl.plist[1].y), new PointF(pl.plist[2].x, pl.plist[2].y), new PointF(pl.plist[3].x, pl.plist[3].y) });

                    // Calculate Lines
                    for (int i = 0; i < pl.plist.Count(); i++)
                    {
                        // Calculate Distance and Direction from point
                        double dirFromPoint = directionFromPoint(p.x, p.y, pl.plist[i].x, pl.plist[i].y); // Calculate direction from point
                        double distFromPoint = distanceFromPoint(p.x, p.y, pl.plist[i].x, pl.plist[i].y); // Calculate distance from point

                        double whereFov = whereInFov(dirFromPoint * rad2Deg, distFromPoint, -p.direction.X, fov.X); // FOV

                        // Calculate whether the point should be showing based on the player's fov
                        bool shouldBeShowing = Math.Abs(whereFov) <= 1 ? true : false;

                        // Draw a line based on the direction of player and point
                        e.Graphics.DrawLine(new Pen(Color.Red), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x) + Convert.ToSingle(zapdist * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(zapdist * Math.Cos(dirFromPoint)));

                        // Draw a line based on the distance of player and point
                        e.Graphics.DrawLine(new Pen(shouldBeShowing ? Color.LimeGreen : Color.DarkRed), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));

                        // Draw text displaying the direction of player and point
                        //e.Graphics.DrawString($"{dirFromPoint * rad2Deg}", DefaultFont, new SolidBrush(Color.Red), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));
                        e.Graphics.DrawString($"{whereInFov(dirFromPoint * rad2Deg, dirFromPoint, -p.direction.X, fov.X)} : : {dirFromPoint * rad2Deg}", DefaultFont, new SolidBrush(Color.Red), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));
                    }
                }

                // FOV Lines
                e.Graphics.DrawLine(new Pen(Color.DarkBlue, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x + zapdist * Math.Sin((-fov.X - p.direction.X) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((-fov.X - p.direction.X) / rad2Deg))); // Minimum FOV Line
                e.Graphics.DrawLine(new Pen(Color.Blue, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x + zapdist * Math.Sin((fov.X - p.direction.X) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((fov.X - p.direction.X) / rad2Deg))); // Maximum FOV Line


                e.Graphics.DrawLine(new Pen(Color.Yellow, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x + zapdist * Math.Sin((-p.direction.X) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((-p.direction.X) / rad2Deg))); // Dir Line

                // FOV Text
                e.Graphics.DrawString($"{minfovX}", DefaultFont, new SolidBrush(Color.DarkBlue), Convert.ToSingle(p.x + zapdist * Math.Sin((-fov.X - p.direction.X) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((-fov.X - p.direction.X) / rad2Deg))); // Minimum FOV
                e.Graphics.DrawString($"{maxfovX}", DefaultFont, new SolidBrush(Color.Blue), Convert.ToSingle(p.x + zapdist * Math.Sin((fov.X - p.direction.X) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((fov.X - p.direction.X) / rad2Deg))); // Maximum FOV

                e.Graphics.DrawString($"{-p.direction.X}", DefaultFont, new SolidBrush(Color.Yellow), Convert.ToSingle(p.x + zapdist * Math.Sin((-p.direction.X) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((-p.direction.X) / rad2Deg)));

                // Finish Transform back to 0, 0
                e.Graphics.ResetTransform();
            }

            // 3D MODE
            if (mode == 3)
            {
                // Transform from 0, 0
                e.Graphics.TranslateTransform(450, 300);

                // 3D Planes
                foreach (Plane3 pl in planes)
                {
                    // An array for the updated positions of points
                    PointF[] newPs = new PointF[4];

                    // Check if this should show at all
                    bool shouldShow = false;

                    // Calculate each point's position
                    for (int i = 0; i < pl.plist.Count(); i++)
                    {
                        double dirFromPointX = directionFromPoint(p.x, p.y, pl.plist[i].x, pl.plist[i].y); // Calculate direction from point
                        double distFromPointX = distanceFromPoint(p.x, p.y, pl.plist[i].x, pl.plist[i].y); // Calculate distance from point


                        double dirFromPointZ = directionFromPoint(p.x, p.z, pl.plist[i].x, pl.plist[i].z); // Calculate direction from point
                        double distFromPointZ = distanceFromPoint(p.x, p.z, pl.plist[i].x, pl.plist[i].z); // Calculate distance from point

                        double whereFovX = whereInFov(dirFromPointX * rad2Deg, distFromPointX, -p.direction.X, fov.X);
                        double whereFovZ = whereInFov(dirFromPointZ * rad2Deg, distFromPointZ, -p.direction.Y, fov.Y);

                        // Calculate where the point should be positioned on the 3d screen
                        if (Math.Abs(whereFovX) <= 1 && Math.Abs(whereFovZ) <= 1)
                        {
                            shouldShow = true;
                        }

                        // Position the updated point based on theMode, dirFromPoint, distFromPoint, and Z Coordinate of point (X-Y Axis, Z - up)
                        //newPs[i] = new PointF(Convert.ToSingle(whereFov) * 450, -Convert.ToSingle((600 / (distFromPoint == 0 ? 0.001 : distFromPoint)) * pl.plist[i].z));

                        newPs[i] = new PointF(Convert.ToSingle(whereFovX) * 450, Convert.ToSingle(whereFovZ) * 300);
                    }

                    if (shouldShow)
                    {
                        // Add outline
                        e.Graphics.DrawPolygon(new Pen(Color.Black), newPs);

                        // Fill outline
                        e.Graphics.FillPolygon(new SolidBrush(Color.Gray), newPs);
                    }
                }

                // Reset transform back to 0, 0
                e.Graphics.ResetTransform();
            }
        }

        private void gameTick_Tick(object sender, EventArgs e)
        {
            // Framerate
            framesSinceLastSecond++;
            accurateSec = DateTime.Now.Second;
            if(lastSec != accurateSec)
            {
                lastSec = accurateSec;
                fps = framesSinceLastSecond;
                framesSinceLastSecond = 0;
                fpsLabel.Text = $"FPS: {fps}";
            }

            // Changing direction X based on key presses (Left/right arrow keys)
            if (theKeys[39])
            {
                p.direction.X++;
            }
            else if (theKeys[37])
            {
                p.direction.X--;
            }

            if(p.direction.X >= 90)
            {
                p.direction.X -= 360;
            }
            if(p.direction.X < -270)
            {
                p.direction.X += 360;
            }

            // Changing direction Y based on key presses (up/down arrow keys)
            if (theKeys[38])
            {
                p.direction.Y--;
            }
            else if (theKeys[40])
            {
                p.direction.Y++;
            }

            if (p.direction.Y > 0)
            {
                p.direction.Y = 0;
            }
            if (p.direction.Y < -180)
            {
                p.direction.Y = -180;
            }

            // Moving based on key presses (WASD)
            if (theKeys[68]) // D
            {
                p.x -= Math.Sin((p.direction.X + 90) / rad2Deg) * 2;
                p.y += Math.Cos((p.direction.X + 90) / rad2Deg) * 2;
            }
            if (theKeys[65]) // A
            {
                p.x += Math.Sin((p.direction.X + 90) / rad2Deg) * 2;
                p.y -= Math.Cos((p.direction.X + 90) / rad2Deg) * 2;
            }
            if (theKeys[83]) // W
            {
                p.x += Math.Sin((p.direction.X) / rad2Deg) * 2;
                p.y -= Math.Cos((p.direction.X) / rad2Deg) * 2;
            }
            if (theKeys[87]) // S
            {
                p.x -= Math.Sin((p.direction.X) / rad2Deg) * 2;
                p.y += Math.Cos((p.direction.X) / rad2Deg) * 2;
            }

            // Refresh
            this.Refresh();
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Updating key array with keys pressed
            theKeys[e.KeyValue] = true;

            // Changing the mode based on pressing space
            if(e.KeyCode == Keys.Space)
            {
                mode = mode >= 3 ? 1 : mode + 1;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            // Release any key that was released in the key array
            theKeys[e.KeyValue] = false;
        }
    }
}
