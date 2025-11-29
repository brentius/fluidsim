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
                    if s[right] = 
                }
            }
        }
    }
}