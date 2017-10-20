using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xRaySimulation
{
    public partial class Display : Form
    {
        private System.Windows.Forms.Button button1 = new Button();
        private List<System.Windows.Forms.PictureBox> pictureBox1 = new List<System.Windows.Forms.PictureBox>();
        private Random _rnd = new Random();
        private Form CallingForm { get; set; }
        private TiltSeries TiltSeries { get; set; }

        

        public Display(Form callingForm, TiltSeries tiltSeries)
        {
            InitializeComponent();
            this.CallingForm = callingForm;
            this.TiltSeries = tiltSeries;
            List<List<List<double>>> series = tiltSeries.Series;
            int nt = series.Count;
            int cols = 5;

            for (int t = 0; t < nt; t++)
            {
                //Bitmap bmp = io.arrayToBitmap(series[t]);

                //display
                this.pictureBox1.Add(new PictureBox());
                this.pictureBox1[t].Image = io.arrayToBitmap(series[t]);

                //this is usually done in the designer

                
                int x = (t % cols) * (series[t].Count + 2) + 13;
                int y = CalculateHeight(t, cols);

                this.pictureBox1[t].Location = new System.Drawing.Point(x, y);
                this.pictureBox1[t].Name = "pictureBox"+t.ToString();
                // this.pictureBox1[t].Size = new System.Drawing.Size(447, 325);
                this.pictureBox1[t].Size = new System.Drawing.Size(series[t].Count, series[t][0].Count);
                this.pictureBox1[t].TabIndex = t;

                this.ClientSize = new System.Drawing.Size(500, 470);
                this.Controls.Add(this.pictureBox1[t]);
            }

            int X = this.reconstructButton.Location.X;
            int Y = CalculateHeight(nt, cols) + CalculateHeight(cols, cols);
            this.reconstructButton.Location = new Point(X, Y);
            
        }

        private int CalculateHeight(int t, int cols)
        {
            return ((t - (t % cols)) / cols) * (TiltSeries.Series[0][0].Count + 2) + 13;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.reconstructButton.Enabled = false;
            backgroundWorker1.RunWorkerAsync();

            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<List<List<double>>> R;

            R = TiltSeries.ReconstructFBP();
            //R = TiltSeries.ReconstructART().Density;

            TiltSeries Rts = new TiltSeries(R.Count, R.Count, R.Count, Width);
            Rts.Series = Reorient(R);

            io.PrintList2D(R[15]);

            // Display tilt series
            Display form2 = new Display(this, Rts);
            form2.ShowDialog();

        }

        private List<List<List<double>>> Reorient(List<List<List<double>>> input)
        {
            int M = input.Count;
            List<List<List<double>>> output = Utility.CreateList3d<double>(M, M, M);
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    for (int k = 0; k < M; k++)
                    {
                        output[k][j][i] = input[j][k][i];
                    }
                }
            }

            return output;
        }
    }
}
