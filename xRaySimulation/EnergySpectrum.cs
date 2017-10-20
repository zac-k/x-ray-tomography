using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{
    /// <summary>
    /// Spectral density as a function of energy
    /// in keV.
    /// </summary>
    public class EnergySpectrum : FunctionOfOneVariable
    {
        // Constructor - Read from file
        public EnergySpectrum(string path) : base(path)
        {
        }
    }


}
