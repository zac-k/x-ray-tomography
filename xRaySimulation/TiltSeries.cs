using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;




namespace xRaySimulation
{
    public class TiltSeries
    {
        public List<List<List<double>>> Series { get; set; }
        public double Width { get; set; }
        private double tiltRange = 180;

        // Constructor
        public TiltSeries(int m, int n, int t, double width)
        {
            Series = Utility.CreateList3d<double>(t, m, n);
            Width = width;
        }

        


        public void setImage(List<List<double>> image, int t)
        {
            for(int i = 0; i < image.Count; i++)
            {
                for(int j = 0; j < image[0].Count; j++)
                {
                    Series[t][j][i] = image[j][i];
                }
            }
            
        }

        public void Construct(Sample sample, BackgroundWorker bgw)
        {
            /* Beam class will need to be made non-static
             * in order to utilise the Parallel.For loop */

            for (int t = 0; t < this.Series.Count; t++)
            //Parallel.For(0, this.Series.Count, t =>
            {
                bgw.ReportProgress(100 * t / Math.Max(this.Series.Count - 1, 1));

                Sample localSample = sample.Clone();
                Beam.Reset();
                localSample.Specimen = sample.rotated(180.0 * (double)t / (double)this.Series.Count);

                // Project sample       
                Beam.projectThroughSample(localSample);
                this.setImage(Beam.Intensity, t);                
            }//);
        }



        public List<List<List<double>>> ReconstructFBP()
        {
            // This should be generalised for non-cubic cuboids
            // That is, it should include N and P, which requires
            // knowledge of the sample grid geometry

            int M = this.Series[0][0].Count;
            int nt = this.Series.Count;
            List<List<List<Complex>>> R = Utility.CreateList3d<Complex>(M, M, M);
            
            List<List<List<Complex>>> transformedSeries = Utility.CreateList3d<Complex>(nt, M, M);
            
            for (int t = 0; t < nt; t++)
            {
                transformedSeries[t] = Utility.DoubleToComplex(this.Series[t]);
                for (int j = 0; j < M; j++)
                {
                    
                    transformedSeries[t][j] = FFT.Forward(transformedSeries[t][j]);
                }
            }

            List<List<List<Complex>>> C = Utility.CreateList3d<Complex>(nt, M, M);
                        
            double dk = 1 / (Width * M);

            int nz = M;
            int ny = M;
            int nx = M;
            

            double dz = Width / nz;
            double dy = Width / ny;
            double dx = Width / nx;

            double lz0 = Width / 2.0;
            double ly0 = Width / 2.0;
            double lx0 = Width / 2.0;
            double x, y, z, theta, r, r0, r1;
            double dth = tiltRange * Math.PI / (nt * 180.0);


            double ky, kr;

            double dkx = 1.0 / Width;
            double dky = 1.0 / Width;
            double dkz = 1.0 / Width;
            double kx0 = dkx * nx / 2.0;
            double ky0 = dky * ny / 2.0;
            double kz0 = dkz * nz / 2.0;

            double delta = 0.001;

            int ki;
            
            Kernel kernel = new Kernel(nx, "ram-lak", Width);

           

            for (int t = 0; t < nt; t++)
            {
                theta = t * dth;
                for (int j = 0; j < ny; j++)
                {
                    ky = j * dky - ky0;
                    for (int l = 0; l < nx; l++)
                    {
                        kr = l * dkx - kx0;     
                        C[t][j][l] = transformedSeries[t][j][l] * kernel.kernel[l];
                    }
                }
                /*
                    2D Inverse Fourier transform filtering
                */

                for (int j = 0; j < M; j++)
                {
                    
                    C[t][j] = FFT.Reverse(C[t][j]);
                    
                }
            }



            for (int k = 0; k < nz; k++)
            {
                z = k * dz - lz0;
                for (int j = 0; j < ny; j++)
                {
                    y = j * dy - ly0;
                    for (int i = 0; i < nx; i++)
                    {
                        x = i * dx - lx0;


                        R[k][j][i] = new Complex(0.0, 0.0);
                        for (int t = 0; t < nt; t++)
                        {
                            theta = t * dth;
                            r = z * Math.Cos(theta) + x * Math.Sin(theta);
                            //r = x * Math.Cos(theta) + z * Math.Sin(theta);
                            ki = (int)Math.Floor((r + lz0) / dz);
                            r0 = ki * dz - lz0;
                            r1 = r0 + dz;
                            if (ki >= 0 && ki < (int)nz - 1)
                            {
                                // Linear interploation
                                R[k][j][i] += ((r1 - r) * C[t][j][ki]
                                         + (r - r0) * C[t][j][ki + 1]) * dth / dz;


                            }

                        }
                    }
                }
            }

            


            return Utility.ComplexToReal(R);

        }


        public List<List<List<double>>> ReconstructFBP_old()
        {
            // This should be generalised for non-cubic cuboids
            // That is, it should include N and P, which requires
            // knowledge of the sample grid geometry

            int M = this.Series[0][0].Count;
            int nt = this.Series.Count;
            List<List<List<Complex>>> R = Utility.CreateList3d<Complex>(M, M, M);


            List<List<List<Complex>>> transformedSeries = Utility.CreateList3d<Complex>(nt,M, M);

            
            for (int t = 0; t < nt; t++)
            {
                transformedSeries[t] = Utility.DoubleToComplex(this.Series[t]);
                transformedSeries[t] = FFT.Forward(transformedSeries[t]);
                
            }

          
   

            List<List<List<Complex>>> C = Utility.CreateList3d<Complex>(nt, M, M);

            //for (int kz = 0; kz < M; kz++)
            //{

            double dk = 1 / (Width * M);

            int nz = M;
            int ny = M;
            int nx = M;

            double tiltRange = 180.0;

            double dz = Width / nz;
            double dy = Width / ny;
            double dx = Width / nx;

            double lz0 = Width / 2.0;
            double ly0 = Width / 2.0;
            double lx0 = Width / 2.0;
            double x,y,z,theta,r,r0,r1;
            double dth = tiltRange * Math.PI / (nt * 180.0);


            double ky,kr;

            double dkx = 1.0 / Width;
            double dky = 1.0 / Width;
            double dkz = 1.0 / Width;
            double kx0 = dkx * nx / 2.0;
            double ky0 = dky * ny / 2.0;
            double kz0 = dkz * nz / 2.0;

            double delta = 0.001;

            int ki;



            // Construct filter kernel
            List<List<double>> kernel = new List<List<double>>();
            CreateKernel(kernel);




            for (int t = 0; t < nt; t++)
            {
                theta = t * dth;
                for (int j = 0; j < ny; j++)
                {
                    ky = j * dky - ky0;
                    for (int l = 0; l < nx; l++)
                    {
                        kr = l * dkx - kx0;

                        C[t][j][l] = transformedSeries[t][j][l] * kernel[l][j];
                    }
                }
                /*
                    2D Inverse Fourier transform filtering
                */
          
                C[t] = FFT.Reverse(C[t]);
            }

            

            for (int k = 0; k < nz; k++)
            {
                z = k * dz - lz0;
                for (int j = 0; j < ny; j++)
                {
                    y = j * dy - ly0;
                    for (int i = 0; i < nx; i++)
                    {
                        x = i * dx - lx0;


                        R[k][i][j] = new Complex(0.0, 0.0);
                        for (int t = 0; t < nt; t++)
                        {
                            theta = t * dth;
                            r = x * Math.Cos(theta) - z * Math.Sin(theta);
                            //r = x * Math.Cos(theta) + z * Math.Sin(theta);
                            ki = (int)Math.Floor((r + lz0) / dz);
                            r0 = ki * dz - lz0;
                            r1 = r0 + dz;
                            if (ki >= 0 && ki < (int)nz - 1)
                            {
                               // Linear interploation
                               R[k][i][j] += ((r1 - r) * C[t][ki][j]
                                        + (r - r0) * C[t][ki + 1][j]) * dth / dz;


                            }
                            
                        }
                    }
                }
            }
            

            

            return Utility.ComplexToReal(R);

        }

        public Reconstruction ReconstructART()
        {
            int iterations = 2;
            int M = this.Series[0][0].Count;
            int nt = this.Series.Count;
            double dth = tiltRange / nt;

            Reconstruction Reconstruction = new Reconstruction(M, M, M, Width, 0.01);
            Reconstruction RotatedReconstruction = new Reconstruction(M, M, M, Width, 1.0);
            List<List<List<double>>> ProjectedReconstruction = Utility.CreateList3d<double>(nt, M, M, Width);

            double lambda = 1.0;



            for (int q = 0; q < iterations; q++)
            {
                
                for (int t = 0; t < nt; t++)
                {
                    double theta = t * dth;
                    ProjectedReconstruction[t] = Reconstruction.Project(theta);
                    RotatedReconstruction.Density = Reconstruction.Rotate(theta);

                    for(int i = 0; i < M; i++)
                    {
                        for(int j = 0; j < M; j++)
                        {
                            
                            double correction = (Series[t][j][i] - ProjectedReconstruction[t][j][i]) / (2*M);
                            for (int k = 0; k < M; k++)
                            {

                                //Rrot.Density[k][j][i] = Math.Max(Rrot.Density[k][j][i] + lambda * correction, 0.0);
                                //Rrot.Density[k][j][i] = Math.Min(Rrot.Density[k][j][i], 1.0);
                                double pr = ProjectedReconstruction[t][j][i];
                                RotatedReconstruction.Density[k][j][i] *= Series[t][j][i] / pr; //* pr / (pr*pr + 0.001*0.001);
                            }
                        }
                    }
                   // io.PrintList2D(Series[0]);
                    //Console.WriteLine("");
                    //io.PrintList2D(RotatedReconstruction.Density[q]);

                    Reconstruction.Density = RotatedReconstruction.Rotate(-theta);

                }

            


            }

            
            return Reconstruction;

        }


        

        


        private void CreateKernel(List<List<double>> kernel)
        {
            int nx = this.Series[0][0].Count;
            int ny = this.Series[0].Count;

            //List<List<double>> kernel = Utility.CreateList2d<double>(nx, ny);

            double kr, ky;
            double dkx = 1.0 / Width;
            double dky = 1.0 / Width;
            double dkz = 1.0 / Width;
            double kx0 = dkx * nx / 2.0;
            double ky0 = dky * ny / 2.0;
            double rad = 0.3;

            double kernelNorm = 0;
            for (int j = 0; j < ny; j++)
            {
                ky = j * dky - ky0;
                for (int l = 0; l < nx; l++)
                {
                    kr = l * dkx - kx0;
                    if (kr * kr < (ny/Width * rad) * (nx/Width * rad))
                    {
                        kernel[j][l] =  Math.Sqrt(kr * kr);
                        kernelNorm += kernel[j][l];
                    }
                    else
                    {
                        kernel[j][l] = 0;
                    }
                    
                }
            }
            for (int j = 0; j < ny; j++)
            {
                for (int l = 0; l < nx; l++)
                {
                    kernel[j][l] *= (nx * ny / kernelNorm);

                }
            }
            
        }


       

        


    }
}
