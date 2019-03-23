using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {   
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Start();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var src = DateTime.Now; 
            var hm = new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, src.Second);
            label1.Text=hm.ToLongTimeString();
            chart1.Series["Nhietdo"].Points.AddXY(label1.Text, SharedVar.temp);
            chart1.Series["Doam"].Points.AddXY(label1.Text,SharedVar.humi);
            SharedVar.x++;
           
            if (SharedVar.x > 50)
            {
                chart1.Series["Nhietdo"].Points.Clear();
                chart1.Series["Doam"].Points.Clear();
                SharedVar.x = 0;
            }
        }

       

    }
}
