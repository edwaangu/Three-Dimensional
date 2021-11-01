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
        // Main variables
        double rad2Deg = 180 / Math.PI;
        int mode = 2; // 2 is 2d TOP, 3 is 3d
        bool[] theKeys = new bool[256];

        // Player related variables
        Player p = new Player(-20, 100, 0, -90);
        double fov = 40;

        // Object related variables
        List<Plane3> planes = new List<Plane3>();
        

        // Framerate related variables
        int framesSinceLastSecond = 0;
        int accurateSec = -1;
        int lastSec = -1;
        int fps = 0;

        int zapdist = 400;
        double t = 0;

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

        int shouldPointShow(double pDir, double minfov, double maxfov)
        {
            int should = -1;
            if (pDir > minfov && pDir < maxfov)
            {
                should = 0;
            }
            if (pDir > (minfov + 360) && pDir < (maxfov + 360))
            {
                should = 1;
            }
            if (pDir > (minfov) && pDir < (maxfov + 360) && maxfov < minfov)
            {
                should = 2;
            }
            if (pDir > (minfov - 360) && pDir < (maxfov) && maxfov < minfov)
            {
                should = 3;
            }
            return should;
        }

        double whereInFov(int should, double pDir, double minfov, double maxfov)
        {
            double whereShould = 0;
            if(should >= 0)
            {
                if(should == 0)
                {
                    whereShould = (pDir - minfov) / fov - 1;
                }
                else if(should == 1)
                {
                    whereShould = (pDir - (minfov + 360)) / fov - 1;
                }
                else if(should == 2)
                {
                    whereShould = (pDir - minfov) / fov - 1;
                }
                else if (should == 3)
                {
                    whereShould = (pDir - (minfov - 360)) / fov - 1;
                }
            }
            else
            {
                whereShould = pDir > maxfov ? (1) : (-1);
            }
            return whereShould;
        }

        public GameScreen()
        {
            InitializeComponent();
            planes.Add(new Plane3(new Point3[] { new Point3(50, -50, -50), new Point3(50, 50, -50), new Point3(50, 50, 50), new Point3(50, -50, 50) }));
            planes.Add(new Plane3(new Point3[] { new Point3(-50, -50, -50), new Point3(50, 50, -50), new Point3(50, 50, 50), new Point3(-50, -50, 50) }));
            planes.Add(new Plane3(new Point3[] { new Point3(-50, -50, -50), new Point3(-150, 50, -50), new Point3(-150, 50, 50), new Point3(-50, -50, 50) }));
            //planes.Add(new Plane3(new Point3[] { new Point3(-50, -50, 50), new Point3(50, 50, 50), new Point3(50, 50, 50), new Point3(50, -50, 50) }));
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {

        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // 2D TOP DOWN MODE
            if (mode == 2)
            {
                e.Graphics.TranslateTransform(450, 300);
                e.Graphics.FillEllipse(new SolidBrush(Color.Black), -5 + Convert.ToSingle(p.x), -5 + Convert.ToSingle(p.y), 10, 10);
            }

            double minfov = ((-fov - p.direction) + 90) % 360 - 90;
            double maxfov = ((fov - p.direction) + 90) % 360 - 90;

            if(minfov < -90)
            {
                minfov += 360;
            }
            if (maxfov < -90)
            {
                maxfov += 360;
            }

            foreach (Plane3 pl in planes)
            {
                PointF[] ps = new PointF[4];
                for(int m = 0;m < pl.plist.Count();m++) {
                    ps[m] = new PointF(pl.plist[m].x, pl.plist[m].y);
                }
                //PointF[] ps = { new PointF(pl.p1.x, pl.p1.y), new PointF(pl.p2.x, pl.p2.y), new PointF(pl.p3.x, pl.p3.y), new PointF(pl.p4.x, pl.p4.y) };
                if (mode == 2)
                {
                    e.Graphics.DrawPolygon(new Pen(Color.Black), ps);
                    e.Graphics.FillPolygon(new SolidBrush(Color.Gray), ps);
                }

                // Red/Green lines
                for (int i = 0; i < ps.Count(); i++)
                {
                    double dirFromPoint = directionFromPoint(p.x, p.y, ps[i].X, ps[i].Y); // Calculate direction from point
                    double distFromPoint = distanceFromPoint(p.x, p.y, ps[i].X, ps[i].Y); // Calculate distance from point

                    bool shouldBeShowing = shouldPointShow(dirFromPoint * rad2Deg, minfov, maxfov) != -1 ? true : false;


                    if (mode == 2)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Red), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x) + Convert.ToSingle(zapdist * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(zapdist * Math.Cos(dirFromPoint)));
                        e.Graphics.DrawLine(new Pen(shouldBeShowing ? Color.LimeGreen : Color.DarkRed), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));

                        e.Graphics.DrawString($"{dirFromPoint * rad2Deg}", DefaultFont, new SolidBrush(Color.Red), Convert.ToSingle(p.x) + Convert.ToSingle(distFromPoint * Math.Sin(dirFromPoint)), Convert.ToSingle(p.y) + Convert.ToSingle(distFromPoint * Math.Cos(dirFromPoint)));
                    }
                }
            }
            // FOV Lines
            if (mode == 2)
            {
                e.Graphics.DrawLine(new Pen(Color.DarkBlue, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x + zapdist * Math.Sin((-fov - p.direction) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((-fov - p.direction) / rad2Deg)));
                e.Graphics.DrawLine(new Pen(Color.Blue, 1), Convert.ToSingle(p.x), Convert.ToSingle(p.y), Convert.ToSingle(p.x + zapdist * Math.Sin((fov - p.direction) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((fov - p.direction) / rad2Deg)));


                e.Graphics.DrawString($"{minfov}", DefaultFont, new SolidBrush(Color.DarkBlue), Convert.ToSingle(p.x + zapdist * Math.Sin((-fov - p.direction) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((-fov - p.direction) / rad2Deg)));
                e.Graphics.DrawString($"{maxfov}", DefaultFont, new SolidBrush(Color.Blue), Convert.ToSingle(p.x + zapdist * Math.Sin((fov - p.direction) / rad2Deg)), Convert.ToSingle(p.y + zapdist * Math.Cos((fov - p.direction) / rad2Deg)));

                e.Graphics.ResetTransform();
            }

            /** 3D Tiny **/

            if (mode == 3)
            {
                e.Graphics.TranslateTransform(450, 300);

                e.Graphics.DrawRectangle(new Pen(Color.Black), -450, -300, 900, 600);

                foreach (Plane3 pl in planes)
                {
                    PointF[] ps = new PointF[4];
                    for (int m = 0; m < pl.plist.Count(); m++)
                    {
                        ps[m] = new PointF(pl.plist[m].x, pl.plist[m].y);
                    }
                    PointF[] newPs = new PointF[4];

                    for (int i = 0; i < ps.Count(); i++)
                    {
                        double dirFromPoint = directionFromPoint(p.x, p.y, ps[i].X, ps[i].Y); // Calculate direction from point
                        double distFromPoint = distanceFromPoint(p.x, p.y, ps[i].X, ps[i].Y); // Calculate distance from point

                        int theMode = shouldPointShow(dirFromPoint * rad2Deg, minfov, maxfov);
                        newPs[i] = new PointF(Convert.ToSingle(whereInFov(theMode, dirFromPoint * rad2Deg, minfov, maxfov)) * 450, -Convert.ToSingle((600 / (distFromPoint == 0 ? 0.001 : distFromPoint)) * pl.plist[i].z));

                    }
                    e.Graphics.DrawPolygon(new Pen(Color.Black), newPs);
                    e.Graphics.FillPolygon(new SolidBrush(Color.Gray), newPs);
                }

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

            t+= 0.1;
            //p.x = Math.Sin(t / rad2Deg) * 200;
            //p.y = Math.Cos(t / rad2Deg) * 200;

            if (theKeys[39])
            {
                p.direction++;
            }
            else if (theKeys[37])
            {
                p.direction--;
            }


            if (theKeys[68])
            {
                p.x ++;
            }
            if (theKeys[65])
            {
                p.x--;
            }
            if (theKeys[83])
            {
                p.y++;
            }
            if (theKeys[87])
            {
                p.y--;
            }

            // Refresh
            this.Refresh();
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            theKeys[e.KeyValue] = true;
            if(e.KeyCode == Keys.Space)
            {
                mode = mode == 2 ? 3 : 2;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            theKeys[e.KeyValue] = false;
        }
    }
}
