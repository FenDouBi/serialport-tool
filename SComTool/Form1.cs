using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SComTool
{

    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        public static extern bool SetCapture();

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;







        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbtASCII.Checked)
                {
                    string sendStr = tbxSend.Text.Trim();
                    byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
                    serialPort1.Write(sendBytes, 0, sendBytes.Length);
                }
                else
                {
                    string[] sendStrs = tbxSend.Text.Trim().Split(new char[] { ' ' });
                    byte[] sendBytes = sendStrs.Select(new Func<string, byte>((item) => Convert.ToByte(item, 16))).ToArray();
                    serialPort1.Write(sendBytes, 0, sendBytes.Length);
                }
            }
            catch
            {

            }

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(50);
            int nums = serialPort1.BytesToRead;
            byte[] receiveBytes = new byte[nums];
            serialPort1.Read(receiveBytes, 0, nums);

            StringBuilder sb = new StringBuilder();
            if (rbt16.Checked)
            {
                for (int i = 0; i < nums; i++)
                {
                    byte[] tempBytes = new byte[] { receiveBytes[i] };
                    string str = BitConverter.ToString(tempBytes);
                    string tempStr = (i % 2) == 0 ? str + " " : str;
                    sb.Append(tempStr);
                }
                sb.AppendLine("\n");
                UpdateUiString(sb.ToString());
            }
            else
            {
                string str = Encoding.ASCII.GetString(receiveBytes);
                UpdateUiString(str);
            }
            sb.Clear();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }


        private void InitialComParam()
        {
            string[] portNameList = SerialPort.GetPortNames();
            int count = portNameList.Length;
            for (int i = 0; i < count; i++)
            {
                comboBox_PortName.Items.Add(portNameList[i]);
            }
            comboBox_PortName.SelectedIndex = 0;


            int[] baudRateString = new int[] { 9600, 19200, 38400, 57600, 115200 };
            comboBox_BaudRate.Items.AddRange(baudRateString.Select(item => item.ToString()).ToArray());
            comboBox_BaudRate.SelectedIndex = 0;

            string[] parity = new string[] { "None", "Odd" };
            comboBox_Parity.Items.AddRange(parity);
            comboBox_Parity.SelectedIndex = 0;

            string[] dataBit = new string[] { "8", "7", "6" };
            comboBox_DataBit.Items.AddRange(dataBit);
            comboBox_DataBit.SelectedIndex = 0;

            string[] stopBit = new string[] { "1", "2" };
            comboBox_StopBit.Items.AddRange(stopBit);
            comboBox_StopBit.SelectedIndex = 0;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            SetCapture();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            SetCapture();
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            SetCapture();
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void label4_MouseUp(object sender, MouseEventArgs e)
        {
            SetCapture();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitialComParam();
            Task timer = new Task(UpdateScanBarcode);
            timer.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (button4.Text == "打开串口")
            {
                try
                {
                    if (serialPort1.IsOpen == false)
                    {
                        string portName = comboBox_PortName.SelectedItem.ToString();
                        int baudRate = Convert.ToInt32(comboBox_BaudRate.SelectedItem);




                        serialPort1.PortName = portName;
                        serialPort1.BaudRate = baudRate;
                        serialPort1.StopBits = StopBits.One;
                        serialPort1.Parity = Parity.None;
                        serialPort1.DataBits = 8;
                        serialPort1.Open();
                        if (serialPort1.IsOpen)
                        {
                            button4.Text = "关闭串口";

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        button4.Text = "关闭串口";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                button4.Text = "打开串口";
            }

        }


        private void UpdateUiString(string str)
        {
            this.Invoke(new Action(() =>
            {
                string[] strArray = richTextBox1.Lines;
                int length = strArray.Length;
                if (length > 200)
                {
                    richTextBox1.Clear();
                }
                richTextBox1.AppendText(str);
            }));

        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }


        private void UpdateScanBarcode()
        {
            while (true)
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        button2.BackColor = Color.LimeGreen;
                    }));
                    Thread.Sleep(500);
                    this.Invoke(new Action(() =>
                    {
                        button2.BackColor = Color.Red;
                    }));
                  
                   
                }
                catch(Exception ex)
                {

                }
                Thread.Sleep(500);

            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //Form2 form2 = new Form2();
            //form2.ShowDialog();
        }
    }
}
