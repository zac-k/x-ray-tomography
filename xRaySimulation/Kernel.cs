using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Forms;

namespace xRaySimulation
{
    class Kernel
    {


        private int nx;
        public List<Complex> kernel { get; set; }
        private double width;
        private string type;

        public Kernel(int nx, string type, double width)
        {
            this.nx = nx;
            this.type = type;
            this.width = width;
            kernel = new List<Complex>();

            CreateKernel();
        }


        private void CreateKernel()
        {
            
            double kr;
            double dkx = 1.0 / width;
            double dl = width / nx;
             
            double kx0 = dkx * nx / 2.0;
            double kmax = kx0;// nx / width;
            double sincA, sincB;


            switch (type)
            {

                case "ram-lak":
                    
                    for (int i = 0; i < nx; i++)
                    {/*
                        double kx = (double)i * dkx + kx0;
                        double l = (double)i * dl - width / 2;

                        sincA = Math.Sin(2 * kmax * l) / (2 * kmax * l);
                        sincB = Math.Sin(kmax * l) / (kmax * l);
                        if(i == nx/2)
                        {
                            kernel.Add(new Complex(1.0, 0.0));
                        }
                        else
                        {
                            kernel.Add(new Complex(2 * kmax * kmax * sincA - kmax * kmax * sincB * sincB, 0));
                        }
                        */
                        
                        kr = i * dkx - kx0;
                        if (Math.Abs(kr) < 0.3 * 2.0 * kx0)
                        {
                            kernel.Add(Math.Abs(kr));
                        }
                        else
                        {
                            kernel.Add(0);
                        }
                        
                        
                    }
                    kernel[nx/2] = 1;
                    //kernel = FFT.Forward(kernel);
                    break;
                case "flat":
                    for (int i = 0; i < nx; i++)
                    {
                        kernel.Add(1);
                    }
                    break;
                default:
                    MessageBox.Show("Unknown kernel type");
                    break;
            }
        }
    }
}
