using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public partial class Form1 : Form
{
    // Settings
    const int GRID_SIZE = 256;
    const int WINDOW_SCALE = 3; // Window will be 768x768

    FluidSim sim;
    Bitmap bmp;
    Timer timer;
    byte[] pixelBuffer; // Raw bytes for fast drawing

    public Form1()
    {
        // Window Setup
        this.DoubleBuffered = true;
        this.ClientSize = new Size(GRID_SIZE * WINDOW_SCALE, GRID_SIZE * WINDOW_SCALE);
        this.Text = "fluidsim";

        // 1. Initialize Simulation
        // We use 1.0 / GRID_SIZE as the spacing "h"
        sim = new FluidSim(GRID_SIZE, GRID_SIZE, 1.0f / GRID_SIZE);

        // 2. Initialize Graphics
        bmp = new Bitmap(GRID_SIZE, GRID_SIZE, PixelFormat.Format32bppArgb);
        pixelBuffer = new byte[GRID_SIZE * GRID_SIZE * 4]; // 4 bytes per pixel (BGRA)

        // 3. Start Loop
        timer = new Timer();
        timer.Interval = 15; // Aim for ~60 FPS
        timer.Tick += GameLoop;
        timer.Start();

        // Interaction
        this.MouseMove += Form1_MouseMove;
    }

    private void GameLoop(object sender, EventArgs e)
    {
        // -- PHYSICS --
        float dt = 1.0f / 60.0f;
        
        // 1. Add Forces
        sim.Integrate(dt, 0.0f); // Gravity 0 so it floats around

        // 2. Solve Incompressibility (40 iterations is a good quality/speed balance)
        sim.Solve(40, 1.9f);

        // 3. Move Fluid
        sim.Advect(dt);

        // -- RENDERING --
        DrawFluid();
        this.Invalidate(); // Requests a screen repaint
    }

    // This method takes the "m" (smoke) array and converts it to pixels
    private void DrawFluid()
    {
        // 1. Map smoke density to byte array
        for (int i = 0; i < GRID_SIZE * GRID_SIZE; i++)
        {
            float density = sim.m[i];
            
            // Clamp density between 0 and 1
            if (density > 1.0f) density = 1.0f;
            if (density < 0.0f) density = 0.0f;

            byte color = (byte)(density * 255);

            // Set pixel colors (Blue-Greenish tint)
            int k = i * 4;
            pixelBuffer[k] = 255;       // Blue
            pixelBuffer[k + 1] = color; // Green
            pixelBuffer[k + 2] = 0;     // Red
            pixelBuffer[k + 3] = 255;   // Alpha
        }

        // 2. Copy bytes to Bitmap (Fastest method in C#)
        BitmapData data = bmp.LockBits(new Rectangle(0, 0, GRID_SIZE, GRID_SIZE), 
                                       ImageLockMode.WriteOnly, 
                                       PixelFormat.Format32bppArgb);
        
        Marshal.Copy(pixelBuffer, 0, data.Scan0, pixelBuffer.Length);
        
        bmp.UnlockBits(data);
    }

    // Draws the bitmap to the window
    protected override void OnPaint(PaintEventArgs e)
    {
        // NearestNeighbor gives it that crisp "pixel art" look
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.DrawImage(bmp, 0, 0, ClientSize.Width, ClientSize.Height);
    }

    // Add smoke and force with mouse
    private void Form1_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // Convert Screen coordinates to Grid coordinates
            int gx = e.X / WINDOW_SCALE;
            int gy = e.Y / WINDOW_SCALE;

            // Add to a small radius around the cursor
            int r = 3;
            for (int y = -r; y <= r; y++)
            {
                for (int x = -r; x <= r; x++)
                {
                    int ix = gx + x;
                    int iy = gy + y;

                    // Bounds check
                    if (ix > 1 && ix < GRID_SIZE - 1 && iy > 1 && iy < GRID_SIZE - 1)
                    {
                        int idx = ix + iy * GRID_SIZE;
                        
                        // Add Smoke
                        sim.m[idx] = 1.0f; 

                        // Add Velocity (Pushing roughly in mouse direction)
                        // A true implementation calculates (MouseX - LastMouseX), 
                        // but let's just push UP for testing.
                        sim.v[idx] -= 5.0f * (1.0f / 60.0f); 
                    }
                }
            }
        }
    }
}