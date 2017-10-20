using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{

    

    class Primitive : Sample
    {
        public Primitive(int M, double width) : base(M, width)
        {
            // Parent constructor
        }

        public void makeSphere(double radius)
        {           
        }

        public void makeCuboid(double[] abc)
        {
            for(int i = 0; i < M; i++)
            {
                for(int j = 0; j < M; j++)
                {
                    for(int k = 0; k < M; k++)
                    {
                        bool[] inside = new bool[3];
                        inside[0] = Math.Abs((double)i / M - 0.5) < abc[0] / (2.0 * Width);
                        inside[1] = Math.Abs((double)j / M - 0.5) < abc[1] / (2.0 * Width);
                        inside[2] = Math.Abs((double)k / M - 0.5) < abc[2] / (2.0 * Width);
                        if (inside[0] && inside[1] && inside[2])
                        {
                            this.Specimen[i][j][k].setMaterial("Bone");
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
