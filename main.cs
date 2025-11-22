//FLUID SIMULATION
//This code implements a simple 2D Eulerian fluid simulation of an incompressible fluid

public class Grid
{
    public readonly int Nx, Ny;
    public readonly int dx, dy;
    
    public double[,] p;
    public double[,] u;
    public double[,] v;

    public Grid(int nx, int ny, double Lx, double Ly)
    {
        Nx = nx;
        Ny = ny;

        dx = Lx/nx;
        dy = Ly/ny;

        p = new double[Nx, Ny];
        u = new double[Nx+1, Ny];
        v = new double[Nx, Ny+1];
    }
}