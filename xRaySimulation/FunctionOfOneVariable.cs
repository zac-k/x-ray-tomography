using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xRaySimulation
{
    public class FunctionOfOneVariable
    {
        protected List<List<double>> array;


        // Constructor - Read from file
        public FunctionOfOneVariable(string path)
        {
            this.array = io.ReadCSV(path, ',');
        }

        // Constructor - zeros
        public FunctionOfOneVariable(int M, int N)
        {
            List<List<double>> array_temp = new List<List<double>>();

            for (int i = 0; i < M; i++)
            {
                List<double> row = new List<double>();
                for(int j = 0; j < N; j++)
                {
                    row.Add(0);
                }
                array_temp.Add(row);
                array = array_temp;
            }            
        }

        // Properties
        public List<List<double>> Array
        {
            get
            {
                return array;
            }
            set
            {
                this.array = value;
            }
        }

        // Methods
        public void normalise()
        {

            double sum = 0;

            // For calculating maximum and minimum height
            double max = array[0][1];
            double min = array[0][1];

            foreach (List<double> row in array)
            {
                // Update max and min
                if (row[1] > max) max = row[1];
                else if (row[1] < min) min = row[1];
                // Update sum                
                sum += row[1];
            }

            double normalisationFactor = sum;

            foreach (List<double> row in array)
            {
                row[1] /= normalisationFactor;
            }






        }

        public void interpolate(EnergySpectrum energy)
        {
            
            AttenuationCoefficient local = new AttenuationCoefficient(energy.Array.Count, energy.Array[0].Count);
            int preceding = 0;

            for (int i = 0; i < this.Array.Count; i++)
            {

                if (this.Array[i][0] < energy.Array[0][0])
                {
                    preceding++;
                }
                else
                {
                    break;
                }

            }
            
            for(int i = 0; i < preceding - 1; i++)
            {
                this.Array.RemoveAt(0);
            }

            
            int i0 = 0;
            for (int i = 0; i < this.Array.Count; i++)
            {
                if (this.Array[i][0] > energy.Array[energy.Array.Count - 1][0])
                {
                    i0 = i + 1;
                    break;
                }
            }
            while(this.Array.Count > i0)
            {
                this.Array.RemoveAt(i0);
            }
            

            for (int i = 0; i < energy.array.Count; i++)
            {
                local.array[i][0] = energy.array[i][0];      
                for(int p = 0; p < array.Count - 1; p++)
                {
                    if (local.array[i][0] == array[p][0])
                    {
                        local.array[i][1] = array[p][1];
                        break;
                    }
                    else if (local.array[i][0] < array[p][0])
                    {
                        // Linear interpolation
                        local.array[i][1] = array[p-1][1] + (array[p][1] - array[p-1][1]) * (local.array[i][0] - array[p-1][0]) / (array[p][0] - array[p-1][0]);
                        break;
                    }
                }
                                                 
            }

            this.array = local.array;


           
        }

        public List<List<double>> getArray()
        {
            return array;
        }

       
    
    }
}
