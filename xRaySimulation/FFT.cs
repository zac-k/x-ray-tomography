using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace xRaySimulation
{

    
    static class FFT
    {
        
        

        public static List<List<Complex>> Forward(List<List<Complex>> signal)
        {
            signal = fft(signal, 1);
            return  shift(signal);

        }

        public static List<List<List<Complex>>> Forward(List<List<List<Complex>>> signal)
        {
            signal = fft(signal, 1);
            return shift(signal);
        }

        public static List<Complex> Forward(List<Complex> signal)
        {
            signal = fft(signal, 1);
            return shift(signal);
        }

        public static List<List<Complex>> Reverse(List<List<Complex>> signal)
        {
            signal = shift(signal);
            return fft(signal, -1);
        }

        public static List<List<List<Complex>>> Reverse(List<List<List<Complex>>> signal)
        {
            signal = shift(signal);
            return fft(signal, -1);
        }

        public static List<Complex> Reverse(List<Complex> signal)
        {
            signal = shift(signal);
            return fft(signal, -1);
        }





        static public List<List<Complex>> shift(List<List<Complex>> array)
        {
            int nx = array[0].Count;
            int ny = array.Count;
            List<List<Complex>> FFTShifted = Utility.CreateList2d<Complex>(nx, ny);

            for (int i = 0; i <= (nx / 2) - 1; i++)
                for (int j = 0; j <= (ny / 2) - 1; j++)
                {
                    FFTShifted[i + (nx / 2)][j + (ny / 2)] = array[i][j];
                    FFTShifted[i][j] = array[i + (nx / 2)][j + (ny / 2)];
                    FFTShifted[i + (nx / 2)][j] = array[i][j + (ny / 2)];
                    FFTShifted[i][j + (nx / 2)] = array[i + (nx / 2)][j];
                }

            return FFTShifted;


        }


        static public List<Complex> shift(List<Complex> array)
        {

            int nx = array.Count;
            List<Complex> FFTShifted = new List<Complex>();

            for (int i = 0; i < nx; i++)
            {
                FFTShifted.Add(0);
            }

            for (int i = 0; i <= (nx / 2) - 1; i++)
            {
                FFTShifted[i + (nx / 2)] = array[i];
                FFTShifted[i] = array[i + (nx / 2)];
            }

            return FFTShifted;


        }


        static public List<List<List<Complex>>> shift(List<List<List<Complex>>> array)
        {
            int nx = array[0][0].Count;
            int ny = array[0].Count;
            int nz = array.Count;
            List<List<List<Complex>>> FFTShifted = Utility.CreateList3d<Complex>(nx, ny, nz);

            for (int i = 0; i <= (nx / 2) - 1; i++)
                for (int j = 0; j <= (ny / 2) - 1; j++)
                {
                    for (int k = 0; k <= (nz / 2) - 1; k++)
                    {
                        FFTShifted[k + (nz / 2)][j + (ny / 2)][i + (nx / 2)] = array[k][j][i];
                        FFTShifted[k + (nz / 2)][j][i] = array[k][j + (ny / 2)][i + (nx / 2)];
                        FFTShifted[k + (nz / 2)][j][i + (nx / 2)] = array[k][j + (ny / 2)][i];
                        FFTShifted[k + (nz / 2)][j + (nx / 2)][i] = array[k][j][i + (nx / 2)];

                        FFTShifted[k][j + (ny / 2)][i + (nx / 2)] = array[k + (nz / 2)][j][i];
                        FFTShifted[k][j][i] = array[k + (nz / 2)][j + (ny / 2)][i + (nx / 2)];
                        FFTShifted[k][j][i + (nx / 2)] = array[k + (nz / 2)][j + (ny / 2)][i];
                        FFTShifted[k][j + (nx / 2)][i] = array[k + (nz / 2)][j][i + (nx / 2)];
                    }
                }

            return FFTShifted;


        }
        



        static public void crop(ref Complex[,] array, double rad)
        {
            int res = array.GetLength(0);
            for (int i = 0; i < res - 1; i++)
            {
                for (int j = 0; j < res - 1; j++)
                {
                    if (Math.Sqrt((double)((i - res / 2) * (i - res / 2) + (j - res / 2) * (j - res / 2))) > rad * (double)res)
                        array[i, j] = new Complex(0, 0);


                }
            }







        }







        /*-------------------------------------------------------------------------
                Perform a 3D FFT given a complex 3D array
                The direction dir, 1 for forward, -1 for reverse
                The size of the array (nx,ny)
            */
        static public List<List<List<Complex>>> fft(List<List<List<Complex>>> c, int dir)
        {

            int nx = c[0][0].Count;
            int ny = c[0].Count;
            int nz = c.Count;
            int i, j, k;
            int m;//Power of 2 for current number of points
            double[] real;
            double[] imag;
            List<List<List<Complex>>> output = Utility.CreateList3d<Complex>(nx, ny, nz);

            real = new double[nx];
            imag = new double[nx];
            for (k = 0; k < nz; k++)
            {
                for (j = 0; j < ny; j++)
                {
                    for (i = 0; i < nx; i++)
                    {
                        real[i] = c[k][j][i].Real;
                        imag[i] = c[k][j][i].Imaginary;
                    }
                    // Calling 1D FFT Function for Rows
                    m = (int)Math.Log((double)nx, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9
                    FFT1D(dir, m, ref real, ref imag);

                    for (i = 0; i < nx; i++)
                    {
                        //  c[i,j].real = real[i];
                        //  c[i,j].imag = imag[i];
                        output[k][j][i] = new Complex(real[i], imag[i]);
                    }
                }
            }
            // Transform the columns  
            real = new double[ny];
            imag = new double[ny];

            for (k = 0; k < nz; k++)
            {
                for (i = 0; i < nx; i++)
                {
                    for (j = 0; j < ny; j++)
                    {
                        //real[j] = c[i,j].real;
                        //imag[j] = c[i,j].imag;
                        real[j] = output[k][j][i].Real;
                        imag[j] = output[k][j][i].Imaginary;
                    }
                    // Calling 1D FFT Function for Columns
                    m = (int)Math.Log((double)ny, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9
                    FFT1D(dir, m, ref real, ref imag);
                    for (j = 0; j < ny; j++)
                    {
                        output[k][j][i] = new Complex(real[j], imag[j]);
                    }
                }
            }

            // Transform 3rd dimension
            real = new double[nz];
            imag = new double[nz];

            for (i = 0; i < nx; i++)
            {
                for (j = 0; j < ny; j++)
                {
                    for (k = 0; k < nz; k++)
                    {
                        real[k] = output[k][j][i].Real;
                        imag[k] = output[k][j][i].Imaginary;
                    }
                    // Call 1D FFT for 3rd dimension
                    m = (int)Math.Log((double)nz, 2);
                    FFT1D(dir, m, ref real, ref imag);
                    for (k = 0; k < nz; k++)
                    {
                        output[k][j][i] = new Complex(real[k], imag[k]);
                    }
                }
            }


            // c = output;
            // return(true);
            return (output);

        }

        /*-------------------------------------------------------------------------
                Perform a 2D FFT given a complex 2D array
                The direction dir, 1 for forward, -1 for reverse
                The size of the array (nx,ny)
            */
        static public List<List<Complex>> fft(List<List<Complex>> c, int dir)
        {

            int nx = c[0].Count;
            int ny = c.Count;
            int i, j;
            int m;//Power of 2 for current number of points
            double[] real;
            double[] imag;
            List<List<Complex>> output = Utility.CreateList2d<Complex>(nx, ny);// c;//=new Complex [nx,ny];
            //output = c.Clone(); // Copying Array
            // Transform the Rows 
            real = new double[nx];
            imag = new double[nx];

            for (j = 0; j < ny; j++)
            {
                for (i = 0; i < nx; i++)
                {
                    real[i] = c[i][j].Real;
                    imag[i] = c[i][j].Imaginary;
                }
                // Calling 1D FFT Function for Rows
                m = (int)Math.Log((double)nx, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9
                FFT1D(dir, m, ref real, ref imag);

                for (i = 0; i < nx; i++)
                {
                    //  c[i,j].real = real[i];
                    //  c[i,j].imag = imag[i];
                    output[i][j] = new Complex(real[i], imag[i]);
                }
            }
            // Transform the columns  
            real = new double[ny];
            imag = new double[ny];

            for (i = 0; i < nx; i++)
            {
                for (j = 0; j < ny; j++)
                {
                    //real[j] = c[i,j].real;
                    //imag[j] = c[i,j].imag;
                    real[j] = output[i][j].Real;
                    imag[j] = output[i][j].Imaginary;
                }
                // Calling 1D FFT Function for Columns
                m = (int)Math.Log((double)ny, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9
                FFT1D(dir, m, ref real, ref imag);
                for (j = 0; j < ny; j++)
                {
                    output[i][j] = new Complex(real[j], imag[j]);
                }
            }
            // c = output;
            // return(true);
            return (output);

        }

        static public List<Complex> fft(List<Complex> c, int dir)
        {


            int nx = c.Count;
            int i;
            int m;//Power of 2 for current number of points
            double[] real;
            double[] imag;
            List<Complex> output = new List<Complex>();// c;//=new Complex [nx,ny];
            //output = c.Clone(); // Copying Array
            // Transform the Rows 
            real = new double[nx];
            imag = new double[nx];


            for (i = 0; i < nx; i++)
            {

                real[i] = c[i].Real;
                imag[i] = c[i].Imaginary;
            }
            // Calling 1D FFT Function for Rows
            m = (int)Math.Log((double)nx, 2);//Finding power of 2 for current number of points e.g. for nx=512 m=9
            FFT1D(dir, m, ref real, ref imag);

            for (i = 0; i < nx; i++)
            {
                output.Add(new Complex(real[i], imag[i]));
            }


            return (output);

        }

        static private void FFT1D(int dir, int m, ref double[] x, ref double[] y)
        {
            long nn, i, i1, j, k, i2, l, l1, l2;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;
            /* Calculate the number of points */
            nn = 1;
            for (i = 0; i < m; i++)
                nn *= 2;
            /* Do the bit reversal */
            i2 = nn >> 1;
            j = 0;
            for (i = 0; i < nn - 1; i++)
            {
                if (i < j)
                {
                    tx = x[i];
                    ty = y[i];
                    x[i] = x[j];
                    y[i] = y[j];
                    x[j] = tx;
                    y[j] = ty;
                }
                k = i2;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }
            /* Compute the FFT */
            c1 = -1.0;
            c2 = 0.0;
            l2 = 1;
            for (l = 0; l < m; l++)
            {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0;
                u2 = 0.0;
                for (j = 0; j < l1; j++)
                {
                    for (i = j; i < nn; i += l2)
                    {
                        i1 = i + l1;
                        t1 = u1 * x[i1] - u2 * y[i1];
                        t2 = u1 * y[i1] + u2 * x[i1];
                        x[i1] = x[i] - t1;
                        y[i1] = y[i] - t2;
                        x[i] += t1;
                        y[i] += t2;
                    }
                    z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == 1)
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }
            /* Scaling for forward transform */
            if (dir == 1)
            {
                for (i = 0; i < nn; i++)
                {
                    x[i] /= (double)nn;
                    y[i] /= (double)nn;

                }
            }
            return;
        }

    }
}
