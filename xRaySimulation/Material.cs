using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xRaySimulation
{
    /// <summary>
    /// Describes a material such as 
    /// "bone" or "soft tissue". It 
    /// includes the energy-dependence 
    /// of the Beer-Lambert attenuation 
    /// coefficient "mu".
    /// </summary>
    public class Material
    {
        private AttenuationCoefficient mu;
        private static AttenuationCoefficient bone;
        private static AttenuationCoefficient softTissue;
        private static AttenuationCoefficient air;
        private static AttenuationCoefficient vacuum;

        public AttenuationCoefficient Mu
        {
            get
            {
                return mu;
            }
            set
            {
                this.mu = value;
            }
        }

        

        
        
        
        public void setMaterial(string mat)
        {
            switch(mat)
            {
                // These attenuation coeffiecients are from
                // http://physics.nist.gov/PhysRefData/XrayMassCoef/tab4.html
                case "Bone":
                    if (bone == null)
                    {
                        //Console.WriteLine("null");
                        bone = new AttenuationCoefficient(@"./attenuation coefficients/bone_attenuation.csv");
                        bone.interpolate(Beam.Omega);
                    }
                    this.Mu = bone;            
                    break;
                case "Soft Tissue":
                    if (softTissue == null)
                    {
                        softTissue = new AttenuationCoefficient(@"./attenuation coefficients/soft_tissue_attenuation.csv");
                        softTissue.interpolate(Beam.Omega);
                    }
                    this.Mu = softTissue;
                    break;
                case "Air":
                    if (air == null)
                    {
                        air = new AttenuationCoefficient(@"./attenuation coefficients/air_attenuation.csv");
                        air.interpolate(Beam.Omega);
                    }
                    this.Mu = air;
                    break;
                case "Vacuum":
                    if (vacuum == null)
                    {
                        vacuum = new AttenuationCoefficient(Beam.Omega.Array.Count, Beam.Omega.Array[0].Count);
                        for (int i = 0; i < Beam.Omega.Array.Count; i++)
                        {
                            vacuum.Array[i][0] = Beam.Omega.Array[i][0];
                            vacuum.Array[i][1] = 0.0;
                        }

                    }
                    this.Mu = vacuum;
                    break;
                default:
                    MessageBox.Show("Unknown Material Type");
                    break;

            }
            
        }

    }



   
}
