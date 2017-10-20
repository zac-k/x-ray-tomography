using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace xRaySimulation
{
    public partial class MainForm : Form
    {
        //private UI_Args args;

        public MainForm()
        {
            InitializeComponent();

            // Disable/enable GUI components
            // depending on selections
            comboBox1.Enabled = radioButton1.Checked;
            textBoxSample.Enabled = radioButton2.Checked;
            textBoxSampleST.Enabled = radioButton2.Checked;
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            this.button_run.Enabled = false;
            UI_Args args = new UI_Args(this);

            backgroundWorkerProject.RunWorkerAsync(args);
            
        }

        private void backgroundWorkerProject_ProgressChanged(object sender,
                                                               ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            project_progressBar.Value = e.ProgressPercentage;

            // Set the text.
            this.progressLabel.Text = e.ProgressPercentage.ToString() + @"%";
        }

        private void backgroundWorkerProject_DoWork(object sender, DoWorkEventArgs e)
        {
            UI_Args args = (UI_Args)e.Argument;

            // Create polychromatic x-ray beam
            Beam.init(args.M, 1.0, @"./spectrum.csv");

            // Make sample
            Sample sample = new Sample(args);

            // Build tilt series
            TiltSeries tiltSeries = new TiltSeries(args.M, args.M, args.Nt, args.Width);
            tiltSeries.Construct(sample, backgroundWorkerProject);
            

            // Display tilt series
            Display form2 = new Display(this, tiltSeries);
            form2.ShowDialog();

            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            textBoxSample.Enabled = btn.Checked;
            textBoxSampleST.Enabled = btn.Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;            
            comboBox1.Enabled = btn.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public struct UI_Args
        {
            private readonly string shape;
            private readonly double width;
            private readonly int m;
            private readonly int nt;
            private readonly bool isPrimitive;
            private readonly bool isFileSample;
            private readonly string boneSamplePath;
            private readonly string softTissueSamplePath;

            public UI_Args(MainForm mainForm)
            {
                this.shape = mainForm.comboBox1.Text;
                this.m = Int32.Parse(mainForm.m_textbox.Text);
                this.width = Int32.Parse(mainForm.a_textbox.Text); // cm
                this.isPrimitive = mainForm.radioButton1.Checked;
                this.isFileSample = mainForm.radioButton2.Checked;
                this.boneSamplePath = mainForm.textBoxSample.Text;
                this.softTissueSamplePath = mainForm.textBoxSampleST.Text;
                this.nt = Int32.Parse(mainForm.tmax_textbox.Text);
            }

            public string Shape { get{ return shape;} }
            public double Width { get { return width; } }
            public int M { get { return m; } }
            public int Nt { get { return nt; } }
            public bool IsPrimitive { get { return isPrimitive; } }
            public bool IsFileSample { get { return isFileSample; } }
            public string BoneSamplePath{ get { return boneSamplePath; } }
            public string SoftTissueSamplePath { get { return softTissueSamplePath; } }

        }

        public void Reset()
        {
            
            button_run.Enabled = true;
            project_progressBar.Value = 0;
            progressLabel.Text = "";
        }

        private void backgroundWorkerProject_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Reset();
        }
    }

    
}
