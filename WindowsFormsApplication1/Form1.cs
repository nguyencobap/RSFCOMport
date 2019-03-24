using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Net;
using System.Threading;

namespace WindowsFormsApplication1
{
   
    public partial class Form1 : Form
    {
       private BackgroundWorker mBackgroundworker;
       private string filePath = System.IO.Path.GetTempPath() + "/ncb.txt";
       private string filePathCsv = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ncb.csv";
        
        public Form1()
        {
            InitializeComponent();
            mBackgroundworker = new BackgroundWorker();
            mBackgroundworker.WorkerReportsProgress = true;
            mBackgroundworker.WorkerSupportsCancellation = true;
            mBackgroundworker.DoWork +=new DoWorkEventHandler(mBackgroundworker_DoWork);
            mBackgroundworker.ProgressChanged +=new ProgressChangedEventHandler(mBackgroundworker_ProgressChanged);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            Console.WriteLine("Init");
            label3.Text = "Chưa kết nối";
            label3.ForeColor = Color.Red;

           
            
           
        }

        void mBackgroundworker_DoWork(object sender, DoWorkEventArgs args) {
            File.AppendAllText(filePath, "Thời gian,Nhiệt độ,Độ ẩm", Encoding.UTF8);
            File.AppendAllText(filePath, Environment.NewLine);
            while (!mBackgroundworker.CancellationPending) {

                if (serialPort1.IsOpen)
                {
                    string data;
                    try
                    {
                        data = serialPort1.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        data = "0,0";
                        mBackgroundworker.CancelAsync();
                        serialPort1.Close();
                        label3.BeginInvoke(new MethodInvoker(()=>  label3.Text = "Đã ngắt kết nối"));
                        label3.BeginInvoke(new MethodInvoker(() => label3.ForeColor = Color.Red));
                        button1.BeginInvoke(new MethodInvoker(() => button1.Text = "Kết nối"));
                        
                        if(ex is UnauthorizedAccessException)
                            MessageBox.Show("Cổng COM đã đóng");
                    }

                    //Data to temp
                    string third = DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
                    string input = third + "," + data;
                    File.AppendAllText(filePath,input);

                    mBackgroundworker.ReportProgress(10,data);
                }
                Console.WriteLine("Running");
                Thread.Sleep(1000);
            }
        }

        void mBackgroundworker_ProgressChanged(object sender, ProgressChangedEventArgs args) {
            int i = args.ProgressPercentage;
            string mSplit = args.UserState as string;
            string[] mSplited = mSplit.Split(',');
            string ts,hs;
            try
            {
                ts = mSplited[0];
                hs = mSplited[1];
            }
            catch (Exception)
            {
                ts = hs = "0";
                
            }
            if (mSplited != null)
            {
                nhietdo.BeginInvoke(new MethodInvoker(() => nhietdo.Text = ts));
                doam.BeginInvoke(new MethodInvoker(() => doam.Text = hs));

                try
                {
                    SharedVar.temp = double.Parse(ts);
                    SharedVar.humi = double.Parse(hs);
                }
                catch (FormatException)
                {

                    SharedVar.temp = 0;
                    SharedVar.humi = 0;
                }

            }
            //nhietdo.Text = tach[0];
            //doam.Text = tach[1];

            //SharedVar.temp = double.Parse(tach[0]);
            //SharedVar.humi = double.Parse(tach[1]);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = SerialPort.GetPortNames();

                        
            
        }

        private void disSerial() 
        {
            Console.WriteLine("Disconnecting");
            mBackgroundworker.CancelAsync();
            serialPort1.Close();
            label3.Text = "Đã ngắt kết nối";
            label3.ForeColor = Color.Red;
            button1.Text = "Kết nối";
            Console.WriteLine("Disconnected");
        }

        private void conSerial() 
        {
            comboBox1.DataSource = SerialPort.GetPortNames();
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = 9600;
            try
            {
                serialPort1.Open();
                label3.Text = "Đã kết nối";
                label3.ForeColor = Color.Green;
                button1.Text = "Ngắt kết nối";
                if (!mBackgroundworker.IsBusy)
                {
                    mBackgroundworker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cổng COM chưa kết nối");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen)
            {
                conSerial();
                
            }
            else
            {
                disSerial();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Cổng COM chưa mở");
            }
            else
            {
                
            }
        }

    

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write("1");
            }
            else
            {
                MessageBox.Show("Chưa mở cổng COM");
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
           
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Form2 frm=new Form2();
            frm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            Boolean isExcept = false;
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Chưa có dữ liệu");
            }
            else
            {
                try
                {
                    File.Copy(filePath, filePathCsv);

                }
                catch (IOException)
                {

                    if (File.Exists(filePathCsv))
                    {
                        try
                        {
                            File.Delete(filePathCsv);
                            File.Copy(filePath, filePathCsv);
                        }
                        catch (IOException)
                        {
                            isExcept = true;
                            MessageBox.Show("Đóng Excel trước khi xuất file!");
                        }
                    }
                }
                finally
                {
                    if (!isExcept)
                    {
                        MessageBox.Show("Xuất file thành công "+filePathCsv);
                        isExcept = true;
                    }
                }
            }

            
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            
              disSerial();
              if (File.Exists(filePath))
              {
                  File.Delete(filePath);
              }
            this.Hide();
            Form3 frm3 = new Form3();
            frm3.Show();
            
            
            
            
        }



     
    
       

    }
    public static class SharedVar
    {
        static public double temp, humi;
        static public double x;
    }
}
