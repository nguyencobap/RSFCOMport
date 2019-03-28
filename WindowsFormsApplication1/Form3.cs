using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    
    public partial class Form3 : Form
    {
        private BackgroundWorker mBW;
        private const int BUFFER_SIZE = 1024;
        static int PORT_NUMBER = 80;
        static ASCIIEncoding encoding = new ASCIIEncoding();
        private Stream stream;
        private TcpClient client = new TcpClient();
        private string filePath = System.IO.Path.GetTempPath() + "/ncb.txt";
        private string filePathCsv = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ncb.csv";
        private string ipAdr;
        
        
        public Form3()
        {
            InitializeComponent();
            mBW = new BackgroundWorker();
            mBW.WorkerSupportsCancellation = true;
            mBW.WorkerReportsProgress = true;
            mBW.DoWork +=new DoWorkEventHandler(mBW_DoWork);
            mBW.ProgressChanged += new ProgressChangedEventHandler(mBW_ProgressChanged);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
           
            label3.Text = "Chưa kết nối";
            label3.ForeColor = Color.Red;
            this.FormClosing += new FormClosingEventHandler(Form3_FormClosing);
          

           

        }

        void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void mBW_DoWork(object sender, DoWorkEventArgs args) 
        {
            File.AppendAllText(filePath, "Thời gian,Nhiệt độ,Độ ẩm", Encoding.UTF8);
            File.AppendAllText(filePath, Environment.NewLine);
            string data;
            var reader = new StreamReader(stream);
            while (!mBW.CancellationPending)
            {
                Console.WriteLine("Doing");
                try
                {
                    data = reader.ReadLine();
                }
                catch (Exception)
                {
                    data = "0,0";
                }
                string third = DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy");
                string input = third + "," + data+ Environment.NewLine;
                File.AppendAllText(filePath, input);
                
                Console.WriteLine(data);
                mBW.ReportProgress(10, data);
            }
        }

        private void mBW_ProgressChanged(object sender, ProgressChangedEventArgs args) {
            int i = args.ProgressPercentage;
            string mSplit = args.UserState as string;
            string[] mSplited = new string[2];
            try
            {
                mSplited = mSplit.Split(',');
            }
            catch (NullReferenceException) 
            {
                mSplited[0] = "0";
                mSplited[1] = "0";
               
            }
            string ts, hs;
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
                

                try
                {
                    nhietdo.BeginInvoke(new MethodInvoker(() => nhietdo.Text = ts));
                    doam.BeginInvoke(new MethodInvoker(() => doam.Text = hs));
                    SharedVar.temp = double.Parse(ts);
                    SharedVar.humi = double.Parse(hs);
                }
                catch (Exception)
                {
                    SharedVar.temp = 0;
                    SharedVar.humi = 0;
                }

            }
        }
        private void conServer()
        {
            ipAdr = tbipAdr.Text;
            if (ipAdr != "")
            {
                client = new TcpClient();
                try
                {
                    client.Connect(ipAdr, PORT_NUMBER);
                    stream = client.GetStream();
                    //Button 
                    label3.Text = "Đã kết nối";
                    label3.ForeColor = Color.Green;
                    button1.Text = "Ngắt kết nối";
                    if (!mBW.IsBusy)
                    {
                        mBW.RunWorkerAsync();
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Kết nối thất bại, thử lại sau");
                }


            }
            else
            {
                MessageBox.Show("Chưa nhập địa chỉ IP của server");
            }
        }

        private void disServer()
        {
            Console.WriteLine("Disconnecting");
            client.GetStream().Close();
            client.Close();
            mBW.CancelAsync();
            //Button
            label3.Text = "Đã ngắt kết nối";
            label3.ForeColor = Color.Red;
            button1.Text = "Kết nối";
            Console.WriteLine("Disconnected");
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            if (!client.Connected)
            {
                conServer();
            }
           
            else {
                disServer();   
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
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
                        MessageBox.Show("Xuất file thành công " + filePathCsv);
                        isExcept = true;
                    }
                }
            }

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(client.Connected)
                disServer();
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Hide();
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
        }
    }
}
