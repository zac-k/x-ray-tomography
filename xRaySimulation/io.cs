using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace xRaySimulation
{
    /// <summary>
    /// Library of custom I/O
    /// methods.
    /// </summary>
    static class io
    {
        public static List<List<double>> ReadCSV(string path, char delimiter)
        {
            while (IsFileLocked(new FileInfo(path)))
            {
                // Check which in future version
                MessageBox.Show(path, "File is open or does not exist");
            }

            var reader = new System.IO.StreamReader(System.IO.File.OpenRead(path));
            List<List<double>> array = new List<List<double>>();

            while (!reader.EndOfStream)
            {
                List<double> row = new List<double>();
                var line = reader.ReadLine();
                if (line[0] != '#') // Ignore commented lines
                {
                    var values = line.Split(delimiter);

                    for (int i = 0; i < values.Length; i++)
                    {
                        double entry;
                        Double.TryParse(values[i], out entry);
                        row.Add(entry);
                    }

                    array.Add(row);
                }
            }
            reader.Close();
            return array;
            
            
        }

        public static List<List<List<double>>> ReadCSV3D(string path, char delimiter)
        {
            while (IsFileLocked(new FileInfo(path)))
            {
                // Check which in future version
                MessageBox.Show(path, "File is open or does not exist");
            }

            var reader = new System.IO.StreamReader(System.IO.File.OpenRead(path));
            List<List<List<double>>> array = new List<List<List<double>>>();
            var testLine = reader.ReadLine();
            int P = testLine.Split(delimiter).Length;
            int k = 0;

            while (k < P) // (!reader.EndOfStream)
            {
                //int k = 0;
                List<List<double>> array2D = new List<List<double>>();
                while (true)
                {
                    
                    List<double> row = new List<double>();
                    var line = reader.ReadLine();
                    if (line.Length != 0) // Ignore blank lines
                    {
                        if (line[0] != '#') // Ignore commented lines
                        {

                            var values = line.Split(delimiter);

                            for (int i = 0; i < values.Length; i++)
                            {
                                double entry;
                                Double.TryParse(values[i], out entry);
                                row.Add(entry);
                            }

                            array2D.Add(row);

                        }
                    }
                    else // Add 2D array at blank line
                    {
                        array2D.Add(row);
                        array.Add(array2D);
                        k++;
                        break;
                    }
                    
                    //;
                }
            }
            reader.Close();
            return array;


        }



        public static void WriteToFile(string path, char delimiter, List<List<double>> array)
        {
            

            var file = new System.IO.StreamWriter(path);

            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array[0].Count; j++)
                {
                    if (j != array[0].Count - 1)
                    {
                        file.Write(array[i][j]);
                        file.Write(delimiter);
                    }
                    else
                    {
                        file.WriteLine(array[i][j]);
                    }
                }
            }

        }

        public static void PrintList2D<T>(List<List<T>> List2D)
        {
            //List<double> row = new List<double>(List2D[0].Capacity);
            foreach (List<T> row in List2D)
            {
                foreach (var value in row)
                {
                    Console.Write("{0:0.00}", value);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


        public static Bitmap arrayToBitmap( List<List<double>> array)
        {
            //now we have to convert the 2 dimensional array into a one dimensional byte-array for use with 8bpp bitmaps
            byte[] pixels = new byte[array.Count * array[0].Count];
            for (int y = 0; y < array[0].Count; y++)
            {
                for (int x = 0; x < array.Count; x++)
                {
                    pixels[y * array.Count + x] = (byte)(array.ToArray()[x][y] * 512.0) ;
                }
            }

            //create a new Bitmap
            Bitmap bmp = new Bitmap(array.Count, array[0].Count, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            System.Drawing.Imaging.ColorPalette pal = bmp.Palette;

            //create grayscale palette
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = Color.FromArgb((int)255, i, i, i);
            }

            //assign to bmp
            bmp.Palette = pal;

            //lock it to get the BitmapData Object
            System.Drawing.Imaging.BitmapData bmData =
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //copy the bytes
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmData.Scan0, bmData.Stride * bmData.Height);

            //never forget to unlock the bitmap
            bmp.UnlockBits(bmData);
            return bmp;
        }


        /*
        public static Bitmap arrayToBitmap(List<List<double>> array)
        {

        }
        */

        }
}
