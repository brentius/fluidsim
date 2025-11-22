//FLUID SIMULATION
//This code implements a simple 2D Eulerian fluid simulation of an incompressible fluid

public class Grid
{
    public readonly int Nx, Ny;
    public readonly int dx, dy;
    
    public double[,] u, v, p;
    public double[,] s;
    public double[,] m;
    public double[,] newU, newV, newM;

    public Grid(int nx, int ny, double Lx, double Ly){
        Nx = nx;
        Ny = ny;

        dx = Lx/nx;
        dy = Ly/ny;

        p = new double[Nx, Ny];
        u = new double[Nx + 1, Ny];
        v = new double[Nx, Ny + 1];
        s = new double[Nx, Ny];
        m = new double[Nx, Ny];

        // Helper Arrays
        newU = new double[Nx + 1, Ny];
        newV = new double[Nx, Ny + 1];
        newM = new double[Nx, Ny];

        //walls
        for (int i=0; i<Nx; i++)
        {
            for (int j=0; j<Ny; j++)
            {
                if (i == 0 || i == Nx - 1 || j == 0 || j == Ny - 1) s[i, j] = 0.0;
                else s[i, j] = 1.0;
            }
        }
    }
}

public class Solver
{
    private const int iterations = 40;
    private const double relaxation = 1.9;

    public void Simulate(Grid grid, double dt, double gravity)
    {
        Integrate(grid, dt, gravity);
        //reset pressure
        Array.Clear(grid.p, grid.p.length);
        Solve(grid);

        Advect(grid, dt);
    }
    
    private void Integrate(Grid g, double dt, double gravity)
    {
        for (int i=1; i<g.Nx-1; i++)
        {
            for (int j=1; j<g.Ny; j++)
            {
                if (g.s[i,j] !=0 && g.s[i, j-1] != 0)
                {
                    g.v[i,j] += gravity * dt;
                }
            }
        }
    }

    private void Solve(Grid g)
    {
        for (int iter=0; iter < iterations; iter++)
        {
            for (int i = 1; i<g.Nx -1; i++)
            {
                for (int j=1; j<g.Ny-1; j++)
                {
                    if (g.s[i, j] == 0.0) continue;
                    double div = g.u[i+1, j] - g.u[i, j] + g.v[i, j+1] - g.v[i, j];
                    double sTot = g.s[i + 1, j] + g.s[i - 1, j] + g.s[i, j + 1] + g.s[i, j - 1];

                    if (sTot == 0.0) continue;

                    double d = (-div / sTot) * relaxation;

                    g.u[i, j] -= d * g.s[i - 1, j];
                    g.u[i + 1, j] += d * g.s[i + 1, j];
                    g.v[i, j] -= d * g.s[i, j - 1];
                    g.v[i, j + 1] += d * g.s[i, j + 1];
                    
                    g.p[i,j] += d; 
                }
            }
        }
    }
}