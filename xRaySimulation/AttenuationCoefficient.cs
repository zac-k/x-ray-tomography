using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{
    public class AttenuationCoefficient : FunctionOfOneVariable
    {
        // Constructor - Read from file
        public AttenuationCoefficient(string path) : base(path)
        {
            for(int i = 0; i < this.Array.Count; i++)
            {               
                // Convert from MeV to keV
                this.Array[i][0] *= 1000;                
            }
        }

        // Constructor - empty with dimensions
        public AttenuationCoefficient(int M, int N) : base(M, N)
        {
        }
    }
}
