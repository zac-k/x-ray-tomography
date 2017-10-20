using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{
    /// <summary>
    /// Describes the optical system.
    /// </summary>
    class OpticalSystem
    {
        private double L; // Source to detector distance
        private double D; // Soure to iso distance

        public OpticalSystem(double L, double D)
        {
            this.L = L;
            this.D = D;
        }
    }
}
