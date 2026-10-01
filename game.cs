// javaDroneGame cSharp:
// (Java project converted to c# using chatGPT, later just work on my java -> c# game engine converter)

using System;
using System.Drawing;
using System.Windows.Forms;

public class GameForm : Form
{
    Timer timer = new Timer();

    Player d = new Player(50, 50);
    NozamaWarehouse n = new NozamaWarehouse();

    Rectangle wht = new Rectangle(0, 310, 100, 50);

    Bird b = new Bird(200);
    Bird b1 = new Bird(300);
    Bird b2 = new Bird(100);

    DeliveryPad dp = new DeliveryPad();
    Rectangle dPh;

    double xSpeed = 0;
    double ySpeed = 0;

    int timePassed = 0;
    int score = 0;

    bool rightKeyDown = false;
    bool leftKeyDown = false;
    bool upKeyDown = false;
    bool downKeyDown = false;

    int timeSincePackage = 0;
    bool hasPackageTimer = false;

    bool hasCrashed = false;

    public GameForm()
    {
        Width = 480;
        Height = 360;
        DoubleBuffered = true;

        dPh = new Rectangle(430, dp.Y, 50, 5);

        timer.Interval = 20;
        timer.Tick += UpdateGame;
        timer.Start();

        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        g.DrawString("Score: " + score, Font, Brushes.Black, 200, 25);
        g.DrawString("Time Delivering: " + timeSincePackage, Font, Brushes.Black, 200, 50);

        n.Draw(g);

        b.Draw(g);
        b1.Draw(g);
        b2.Draw(g);

        d.Draw(g);
        dp.Draw(g);
    }

    private void UpdateGame(object sender, EventArgs e)
    {
        if (hasPackageTimer)
            timeSincePackage++;

        timePassed++;

        Rectangle pH = new Rectangle((int)d.X, (int)d.Y, 25, 15);

        if (pH.IntersectsWith(wht))
        {
            d.GivePackage();
            timeSincePackage = 0;
            hasPackageTimer = true;
        }

        if (d.HasPackage && pH.IntersectsWith(dPh))
        {
            score += (timeSincePackage <= 200) ? 3 : 1;
            d.TakePackage();
            hasPackageTimer = false;
        }

        // Birds
        b.Move(4);
        if (pH.IntersectsWith(b.Hitbox)) hasCrashed = true;

        b1.Move(5);
        if (pH.IntersectsWith(b1.Hitbox)) hasCrashed = true;

        b2.Move(3);
        if (pH.IntersectsWith(b2.Hitbox)) hasCrashed = true;

        // Movement bounds
        if (d.X < 480 - 25 && d.X > 0)
            d.Move(xSpeed, 0);
        else
        {
            d.Move(-xSpeed, 0);
            hasCrashed = true;
        }

        if (d.Y > 0 && d.Y < 360 - 15)
            d.Move(0, ySpeed);
        else
        {
            d.Move(0, -ySpeed);
            hasCrashed = true;
        }

        // X movement
        if (rightKeyDown && !leftKeyDown)
        {
            if (xSpeed <= 2) xSpeed += 0.1;
        }
        else if (leftKeyDown && !rightKeyDown)
        {
            if (xSpeed >= -2) xSpeed -= 0.1;
        }
        else
        {
            if (xSpeed > 0) xSpeed -= 0.03;
            else if (xSpeed < 0) xSpeed += 0.03;
        }

        // Y movement
        if (upKeyDown && !downKeyDown)
        {
            if (ySpeed > -1.5) ySpeed -= 0.1;
        }
        else if (downKeyDown && !upKeyDown)
        {
            ySpeed += 0.2;
        }
        else
        {
            if (ySpeed > 0) ySpeed -= 0.1;
            else if (ySpeed < 0) ySpeed += 0.02;
        }

        if (hasCrashed)
            d.Crash();

        Invalidate();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up) upKeyDown = true;
        if (e.KeyCode == Keys.Down) downKeyDown = true;
        if (e.KeyCode == Keys.Left) leftKeyDown = true;
        if (e.KeyCode == Keys.Right) rightKeyDown = true;
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up) upKeyDown = false;
        if (e.KeyCode == Keys.Down) downKeyDown = false;
        if (e.KeyCode == Keys.Left) leftKeyDown = false;
        if (e.KeyCode == Keys.Right) rightKeyDown = false;
    }

    [STAThread]
    public static void Main()
    {
        Application.Run(new GameForm());
    }

    // ================= CLASSES =================

    class Player
    {
        public double X, Y;
        public bool HasPackage = false;
        bool crashed = false;

        public Player(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Crash() => crashed = true;
        public void GivePackage() => HasPackage = true;
        public void TakePackage() => HasPackage = false;

        public void Move(double dx, double dy)
        {
            if (!crashed)
            {
                X += dx;
                Y += dy;
            }
        }

        public void Draw(Graphics g)
        {
            if (crashed)
            {
                g.DrawString("You Crashed!", SystemFonts.DefaultFont, Brushes.Black, 50, 50);
                g.FillRectangle(Brushes.Red, (int)X, (int)Y, 20, 10);
            }
            else
            {
                g.FillRectangle(Brushes.Black, (int)X, (int)Y, 5, 5);
                g.FillRectangle(Brushes.Black, (int)X + 20, (int)Y, 5, 5);
                g.FillRectangle(Brushes.Gray, (int)X + 5, (int)Y + 5, 15, 5);
            }

            if (HasPackage)
                g.FillRectangle(Brushes.Orange, (int)X + 5, (int)Y + 15, 15, 10);
        }
    }

    class Bird
    {
        public int X = 0;
        public int Y;
        bool isLeft = false;

        public Rectangle Hitbox;

        public Bird(int y)
        {
            Y = y;
            Hitbox = new Rectangle(X, Y, 10, 10);
        }

        public void Move(int speed)
        {
            if (isLeft)
            {
                X -= speed;
                if (X <= 0) isLeft = false;
            }
            else
            {
                X += speed;
                if (X >= 480) isLeft = true;
            }

            Hitbox.Location = new Point(X, Y);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Orange, X, Y, 10, 6);
        }
    }

    class DeliveryPad
    {
        public int Y;

        public DeliveryPad()
        {
            Y = new Random().Next(0, 150);
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Green, 430, Y, 50, 5);
        }
    }

    class NozamaWarehouse
    {
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Blue, 0, 310, 100, 50);
        }
    }
}
