using System;
using System.Collections.Generic;

namespace xRaySimulation
{
    /// <summary>
    /// Holds all the information
    /// describing the x-ray beam,
    /// and provides methods for
    /// manipulating it.
    /// </summary>
    static class Beam
    {

        public static List<List<double>> Intensity { get; set; }
        private static List<List<double>> initialIntensity;
        public static EnergySpectrum Omega { get; set; } 

        

        // Methods

        public static void normalise()
        {
            Omega.normalise();
        }


        public static void projectThroughSample(Sample sample)
        {
            List<List<double>> output = Utility.CreateList2d<double>(sample.Specimen.Count, sample.Specimen[0].Count);
            for (int i = 0; i < sample.Specimen.Count; i++)
            {
                for (int j = 0; j < sample.Specimen[0].Count; j++)
                {

                    // Fill up the energy values of innerIntegral
                    // and set the second column to zero
                    List<List<double>> innerIntegral = new List<List<double>>();
                    for (int p = 0; p < Omega.Array.Count; p++)
                    {
                        List<double> row = new List<double>();

                        row.Add(Omega.Array[p][0]);
                        row.Add(0);

                        innerIntegral.Add(row);
                    }




                    for (int k = 0; k < sample.Specimen[0][0].Count; k++)
                    {
                        for (int p = 0; p < innerIntegral.Count; p++)
                        {
                            innerIntegral[p][1] += sample.Specimen[k][j][i].Mu.Array[p][1] * sample.Width / sample.Specimen[0][0].Count;

                        }
                    }
                    double outerIntegral = 0;
                    for (int p = 0; p < Omega.Array.Count; p++)
                    {
                        outerIntegral += Omega.Array[p][1] * Math.Exp(-1 * innerIntegral[p][1])
                            * (Omega.Array[Omega.Array.Count - 1][0] - Omega.Array[0][0])
                                    / Omega.Array.Count;
                    }

                    Intensity[j][i] *= outerIntegral;


                }

            }
        }

        public static void init(int M, double constIntensity, String path)
        {
            Intensity = new List<List<double>>();
            initialIntensity = new List<List<double>>();


            for (int i = 0; i < M; i++)
            {
                List<double> row = new List<double>();

                for (int j = 0; j < M; j++)
                {
                    row.Add(constIntensity);
                }
                Intensity.Add(row);
            }

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Intensity[j][i] = constIntensity;
                }
            }
            initialIntensity = Utility.Clone(Intensity);
            Omega = new xRaySimulation.EnergySpectrum(path);


            Beam.normalise();
        }



        public static void Reset()
        {
            Intensity = Utility.Clone(initialIntensity);
        }

    }




}
