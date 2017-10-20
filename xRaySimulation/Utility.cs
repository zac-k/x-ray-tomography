using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xRaySimulation
{
    static class Utility
    {

        public static List<List<List<T>>> CreateList3d<T>(int M, int N, int P) where T : new()
        {
            List<List<List<T>>> array = new List<List<List<T>>>();
            for (int i = 0; i<M; i++)
            {
                List<List<T>> D2 = new List<List<T>>();
                for (int j = 0; j<N; j++)
                {
                    List<T> D1 = new List<T>();
                    for (int k = 0; k<P; k++)
                    {
                        D1.Add(new T());
                    }


                    D2.Add(D1);
                }
                array.Add(D2);
            }

            return array;
        }


        public static List<List<List<T>>> CreateList3d<T>(int M, int N, int P, T constant) where T : new()
        {
            List<List<List<T>>> array = new List<List<List<T>>>();
            for (int i = 0; i < M; i++)
            {
                List<List<T>> D2 = new List<List<T>>();
                for (int j = 0; j < N; j++)
                {
                    List<T> D1 = new List<T>();
                    for (int k = 0; k < P; k++)
                    {
                        D1.Add(constant);
                    }


                    D2.Add(D1);
                }
                array.Add(D2);
            }

            return array;
        }

        public static List<List<T>> CreateList2d<T>(int M, int N) where T : new()
        {
            List<List<T>> D2 = new List<List<T>>();
            for (int i = 0; i < M; i++)
            {
                List<T> D1 = new List<T>();
                for (int j = 0; j < N; j++)
                {                   
                        D1.Add(new T());
                }
                D2.Add(D1);
            }

            return D2;
        }

        public static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public static List<List<T>> CreateList2d<T>(int M, int N, T constant) where T : new()
        {
            List<List<T>> D2 = new List<List<T>>();
            for (int i = 0; i < M; i++)
            {
                List<T> D1 = new List<T>();
                for (int j = 0; j < N; j++)
                {
                    D1.Add(constant);
                }
                D2.Add(D1);
            }

            return D2;
        }



        public static List<List<T>> Clone<T>(List<List<T>> source) where T : new()
        {
            int m = source.Count;
            int n = source[0].Count;

            List<List<T>> dest = CreateList2d<T>(m, n);


            for (int i = 0; i < m; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    dest[i][j] = source[i][j];

                }
            }

            return dest;

        }








        




        public static List<List<Complex>> DoubleToComplex(List<List<double>> array)
        {
            int M = array[0].Count;
            int N = array.Count;

            List<List<Complex>> output = Utility.CreateList2d<Complex>(M,N);

            for(int i = 0; i < M; i++)
            {
                for(int j = 0; j < N; j++)
                {
                    output[i][j] = new Complex(array[i][j], 0.0);
                }
            }

            return output;
        }

        

        public static List<List<double>> ComplexToReal(List<List<Complex>> array)
        {
            int M = array[0].Count;
            int N = array.Count;

            List<List<double>> output = Utility.CreateList2d<double>(M, N);

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    output[i][j] = array[i][j].Real;
                }
            }

            return output;
        }

        public static List<List<List<double>>> ComplexToReal(List<List<List<Complex>>> array)
        {
            int M = array[0][0].Count;
            int N = array[0].Count;
            int P = array.Count;

           


            List<List<List<double>>> output = Utility.CreateList3d<double>(P, M, N);

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    for (int k = 0; k < P; k++)
                    {
                       
                        output[k][i][j] = array[k][i][j].Real;
                       
                    }
                    
                }
            }

            return output;
        }








    }



}
