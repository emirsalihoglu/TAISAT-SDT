using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace taisatSanayi
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;


        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime suan = DateTime.Now;
            label1.Text = suan.ToString("HH:mm:ss");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListCameras();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        //Camera:
        private void ListCameras()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo device in videoDevices)
            {
                comboBox1.Items.Add(device.Name);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Kamera bulunamadı!");
            }
        }

        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = frame;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.Stop();
                pictureBox1.Image = null;
            }

            videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);

            VideoCapabilities[] videoCapabilities = videoSource.VideoCapabilities;

            VideoCapabilities highestResolution = videoCapabilities[0];
            foreach (VideoCapabilities cap in videoCapabilities)
            {
                if (cap.FrameSize.Width * cap.FrameSize.Height > highestResolution.FrameSize.Width * highestResolution.FrameSize.Height)
                {
                    highestResolution = cap;
                }
            }

            videoSource.VideoResolution = highestResolution;

            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            videoSource.Start();
        }
    }
}
