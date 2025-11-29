//FLUID SIMULATION
//This code implements a simple 2D Eulerian fluid simulation of an incompressible fluid

using System;

public class FluidSim
{
    int numX, numY;
    float density;
    float spacing;

    public float[] u;
    public float[] v;
    public float[] newU;
    public float[] newV;
    public float[] m;
    public float[] newM;
    public float[] s;

    public FluidSim(int x, int y, float h)
    {
        this.numX = x;
        this.numY = y;
        this.spacing = h;
        this.density = 1.0f;

        int size = numX * numY;

        u = new float[size];
        v = new float[size];
        newU = new float[size];
        newV = new float[size];
        m = new float[size];
        newM = new float[size];
        s = new float[size];

        Array.Fill(s, 1.0f);

        for (int i=0; i<numX; i++)
        {
            for (int j=0; j<numY; j++)
            {
                if (i==0 || i==numX-1|| j==0 || j == numY - 1)
                {
                    s[i+j*numX] = 0.0f;
                }
            }
        }
    }

    public void Integrate(float dt, float gravity)
    {
        for(int i = 0; i<numX*numY; i++)
        {
            if (s[i] > 0.0f)
            {
                v[i] += gravity * dt;   
            }
        }
    }

    public void Solve(int iter, float overRelaxation)
    {
        for (int k = 0; k < iter; k++)
        {
            for (int j = 0; j < numY -1; j++)
            {
                for (int i = 1; i < numX-1; i++)
                {
                    int idx = i + j*numX;
                    if (s[idx] == 0.0f) continue;

                    int right = (i+1) + j * numX;
                    int left = i + j*numX;
                    int down = i + (j+1) * numX;
                    int up = i + j *numX;

                    float div = u[right] - u[left] + v[down] - v[up];

                    //correct div if neighbors are walls
                    if (s[right] == 0.0f) div -= u[right];
                    if (s[left] == 0.0f) div += u[left];
                    if (s[down] == 0.0f) div -= u[down];
                    if (s[up] == 0.0f) div += u[up];

                    //fluid neighbors
                    float sx0 = s[left];
                    float sx1 = s[right];
                    float sy0 = s[up];
                    float sy1 = s[down];
                    float sTot = sx0 + sx1 + sy0 + sy1;

                    // If surrounded by walls, skip
                    if (sTot == 0.0f) continue;

                    // Calculate how much to adjust velocity to fix the extra fluid
                    float div = d * overRelaxation; 
                    
                    // Distribute the fix among the valid neighbors
                    u[left] += div * (sx0 / sTot);
                    u[right] -= div * (sx1 / sTot);
                    v[up] += div * (sy0 / sTot);
                    v[down] -= div * (sy1 / sTot);
                }
            }
        }
    }
}