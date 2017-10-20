using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{
    public class Reconstruction : ICloneable
    {
        public List<List<List<double>>> Density { get; set; }
        private int nx, ny, nz;
        private double Width { get; set; }



        public Reconstruction(int nz, int ny, int nx, double width, double init)
        {
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.Width = width;

            Density = Utility.CreateList3d<double>(nz, ny, nx, init);
        }

        public Reconstruction(int nz, int ny, int nx, double width)
        {
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.Width = width;

            Density = Utility.CreateList3d<double>(nz, ny, nx);
        }

        public List<List<double>> Project(double theta)
        {
            List<List<double>> output = Utility.CreateList2d(ny, nx, 0.0);
            List<List<List<double>>> rotated = this.Rotate(theta);
            double da = Width / nz;

            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    for (int k = 0; k < nz; k++)
                    {
                        output[j][i] += rotated[k][j][i] / nz;// Math.Exp(-rotated[k][j][i] * da) ;
                    }
                }
            }

                        return output;
        }

        public List<List<List<double>>> Rotate(double angle)
        {
            int M = nx;
            List<List<List<double>>> output = Utility.CreateList3d<double>(M, M, M);


            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    for (int k = 0; k < M; k++)
                    {
                        double i0 = (double)(i - M / 2);
                        double j0 = (double)(j - M / 2);
                        double k0 = (double)(k - M / 2);

                       
                        
                            double theta = angle * 2 * Math.PI / 360;

                            double it = Math.Cos(-theta) * (double)i0 - Math.Sin(-theta) * (double)k0 + (double)M / 2;
                            double kt = Math.Sin(-theta) * (double)i0 + Math.Cos(-theta) * (double)k0 + (double)M / 2;

                            int ir = (int)Math.Round(it);
                            int kr = (int)Math.Round(kt);
                        if (kr >= 0 && kr < M && ir >= 0 && ir < M)
                        {
                            output[k][j][i] = this.Density[kr][j][ir];
                        }
                        else
                        {
                            output[k][j][i] = 0.0;
                        }
                         /*

                    try
                        {
                            double theta = angle * 2 * Math.PI / 360;

                            double it = Math.Cos(-theta) * (double)i0 - Math.Sin(-theta) * (double)k0 + (double)M / 2;
                            double kt = Math.Sin(-theta) * (double)i0 + Math.Cos(-theta) * (double)k0 + (double)M / 2;

                            int ir = (int)Math.Round(it);
                            int kr = (int)Math.Round(kt);

                            output[k][j][i] = this.Density[kr][j][ir];
                        }
                    catch(ArgumentOutOfRangeException)
                        {
                            output[k][j][i] = 0.0;
                        }*/
                    }
                }
            }

            return output;

        }

        public object Clone()
        {
            Reconstruction output = new Reconstruction(nz, ny, nx, Width);
            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    for (int k = 0; k < nz; k++)
                    {
                        output.Density[k][j][i] = Density[k][j][i];
                    }
                }
            }

            return output;
        }



    }
}
