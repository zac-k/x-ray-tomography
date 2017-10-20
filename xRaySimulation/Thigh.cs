using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{
    class Thigh : Sample
    {

        public Thigh(int M, double width, EnergySpectrum omega) : base(M, width)
        {
            makeThigh(omega);
        }

        public void makeThigh(EnergySpectrum omega)
        {
            int M = this.Specimen.Count;
            int N = this.Specimen[0].Count;
            int P = this.Specimen[0][0].Count;

           

            List<List<List<double>>> femur = io.ReadCSV3D(@"./samples/octohedron.txt", ' ');
            //List<List<List<double>>> head = io.ReadCSV3D(@"./samples/head.txt", ' ');


            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < P; k++)
                    {
                        int ic = i - M / 2;
                        int jc = j - N / 2;
                        int kc = k - P / 2;

                        //if (ic * ic + kc * kc < 0.15 * M * 0.15 * M)
                        if (femur[i+128][j + 128][k + 128] != 0)
                        {
                            this.Specimen[i][j][k].setMaterial("Bone");
                        }
                        else if (ic * ic + kc * kc < 0.3 * M * 0.3 * M)
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
    }
}
