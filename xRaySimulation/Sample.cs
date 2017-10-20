using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xRaySimulation
{
    /// <summary>
    /// Holds a 3D list of materials.
    /// </summary>
    public class Sample
    {               
        public List<List<List<Material>>> Specimen { get; set; }
        public double Width { get; set; } // in cm
        protected int M { get; set; }
        


      
        public Sample(int M, double width)
        {
            Width = width;
            Specimen = Utility.CreateList3d<Material>(M, M, M);
            this.M = M;       
        }

        public Sample(MainForm.UI_Args args)
        {
            Width = args.Width;
            this.M = args.M;
            Specimen = Utility.CreateList3d<Material>(M, M, M);
            
            Set(args);

        }




       

       


        // Methods

            /// <summary>
            /// Rotates the specimen by angle 
            /// "angle" in degrees
            /// </summary>
            /// <param name="angle"></param>
        public List<List<List<Material>>> rotated(double angle)
        {
            int M = Specimen.Count;
            List<List<List<Material>>> specimenTemp = Utility.CreateList3d<Material>(M, M, M);


            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    for (int k = 0; k < M; k++)
                    {
                        double i0 = (double)i - M / 2;
                        double j0 = (double)j - M / 2;
                        double k0 = (double)k - M / 2;
                        if (i0*i0 + j0*j0 + k0*k0 < (M/2-2)*(M/2-2))
                        {
                            double theta = angle * 2 * Math.PI / 360;

                            double it = Math.Cos(-theta) * (double)i0 - Math.Sin(-theta) * (double)k0 + (double) M/2;
                            double kt = Math.Sin(-theta) * (double)i0 + Math.Cos(-theta) * (double)k0 + (double) M/2;

                            int ir = (int)Math.Round(it);
                            int kr = (int)Math.Round(kt);

                            specimenTemp[k][j][i] = Specimen[kr][j][ir];
                        }
                        else
                        {
                            specimenTemp[k][j][i].setMaterial("Air");
                        }
                       
                    }
                }
            }

            return specimenTemp;
            
        }

        private void Set(MainForm.UI_Args args)
        {
            if (args.IsPrimitive)
            {
                if (args.Shape == "Cuboid")
                {
                    Primitive cuboid = new Primitive(M, Width);

                    double[] abc = { 1.0, 4.0, 7.0 };
                    cuboid.makeCuboid(abc);
                    this.Specimen = cuboid.Specimen;
                }
            }
            else if (args.IsFileSample)
            {
                this.Load(args.BoneSamplePath, args.SoftTissueSamplePath);
            }
            else
            {
                MessageBox.Show("Unknown radio button");
            }
        }

        public void Rotate(double angle)
        {
            Specimen = this.rotated(angle);
        }

        public Sample Clone()
        {
            Sample output = new Sample(M, Width);
            output.Specimen = Utility.Clone(this.Specimen);
            return output;
        }



        public void Load( string path_b, string path_st )
        {
            int P = this.Specimen.Count;
            int N = this.Specimen[0].Count;
            int M = this.Specimen[0][0].Count;



            List<List<List<double>>> array_b = io.ReadCSV3D(path_b, ' ');
            int[] fac_b = CalculateFactor(array_b, M, N, P);
            List<List<List<double>>> array_st = io.ReadCSV3D(path_st, ' ');
            int[] fac_st = CalculateFactor(array_st, M, N, P);


            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < P; k++)
                    {
                        int ic = i - M / 2;
                        int jc = j - N / 2;
                        int kc = k - P / 2;

                       
                        if (array_b[i* fac_b[0]][j* fac_b[1]][k* fac_b[2]] != 0)
                        {
                            this.Specimen[i][j][k].setMaterial("Bone");
                        }
                        else if (array_st[i * fac_st[0]][j * fac_st[1]][k * fac_st[2]] != 0)
                        {
                            this.Specimen[i][j][k].setMaterial("Soft Tissue");
                        }
                        else
                        {
                            this.Specimen[i][j][k].setMaterial("Air");
                        }

                    }
                }
            }
        }


        public int[] CalculateFactor(List<List<List<double>>> array, int M, int N, int P)
        {
            int[] output = { (int)Math.Floor((double)array[0][0].Count / M),
                             (int)Math.Floor((double)array[0].Count / N),
                             (int)Math.Floor((double)array.Count / P)  };
            return output;
        }


        





    }
}
