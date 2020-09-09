using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace chuankou
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)     //窗口创建初始化函数
        {
            comboBox1.Items.Add("COM73");
            for (int i = 1; i < 16; i++)                        //端口号20个
            {
                comboBox1.Items.Add("COM" + i.ToString());      //统一添加"0x"
            }
            comboBox2.Items.Add(9600);
            for (int j = 2; j <= 6; j = j + 2)                  //波特率范围
            {
                comboBox2.Items.Add(9600 * j);
                if (j >= 6)
                {
                    comboBox2.Items.Add(115200);
                    comboBox2.Items.Add(230400);
                    comboBox2.Items.Add(460800);
                }
            }
            comboBox1.Text = "COM1";                            //串口初始值
            comboBox2.Text = "9600";                            //波特率初始值
            radioButton1.Checked = true;
            radioButton3.Checked = true;

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//必须手动添加事件处理
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)    //串口数据接收事件
        {
            if (!radioButton4.Checked)                          //如果接受模式不为 HEX 模式
            {
                string str = serialPort1.ReadExisting();        //字符串方式读取
                textBox1.AppendText(str);                       //添加内容
            }
            else                                                //如果接受模式为HEX模式
            {
                try
                {
                    while (true)
                    {
                        byte data;
                        data = (byte)serialPort1.ReadByte();                //此处需要强制类型转换，将（int）类型数据转换为（byte）类型数据
                        string str = Convert.ToString(data, 16).ToUpper();  //转换为大写十六进制字符串
                                                                            //textBox1.AppendText((str.Length == 1 ? "0" + str : str) + " ");//空位补“0”
                        textBox1.AppendText(str + " ");
                    }
                }
                catch
                {

                }
            }
        }

        private void SerachAndAddSerialToCombox(SerialPort MyPort, ComboBox MyBox)
        {                                                                   //将可用端口添加到ComboBox        
            string[] MyString = new string[20];                             //最多可容纳20个，再多会影响调试效率
            string Buffer;                                                  //缓存
            MyBox.Items.Clear();                                            //清空ComboBox的内容
            for (int i = 1; i < 20; i++)                                    //循环
            {
                try                                                         //核心是依靠try和catch完成遍历
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();                                          //如果失败，后边的代码不会执行
                    MyString[i - 1] = Buffer;
                    MyBox.Items.Add(Buffer);                                //打开成功，添加至下拉列表
                    MyPort.Close();                                         //关闭
                }
                catch
                {

                }
            }
            MyBox.Text = MyString[0];                                       //初始化
        }

        private void button1_Click(object sender, EventArgs e)              //打开串口按钮事件
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text,10);  //十进制数据转换
                serialPort1.Open();
                button1.Enabled = false;                                    //打开串口按钮不可用
                button2.Enabled = true;                                     //关闭串口
            }
            catch
            {
                MessageBox.Show("端口错误，请检查串口", "错误");
            }
        }

        private void button2_Click(object sender, EventArgs e)              //关闭串口事件
        {
            serialPort1.Close();
            button1.Enabled = true;                                         //打开串口按钮不可用
            button2.Enabled = false;                                        //关闭串口
        }

        private void button3_Click(object sender, EventArgs e)              //发送按钮事件
        {
            byte[] Data = new byte[1];                                      //定义一个字节的数组
            if (serialPort1.IsOpen)                                         //判断串口是否打开，如果打开执行下一步操作
            {
                if (textBox2.Text != "")
                {
                    if (!radioButton2.Checked)                              //如果发送模式不是 HEX 模式
                    {
                        try
                        {
                            serialPort1.WriteLine(textBox2.Text);           //写数据
                        }
                        catch
                        {
                            MessageBox.Show("串口数据写入错误", "错误");    //出错提示
                            serialPort1.Close();                            //关闭串口，如果是写数据时出错，此时窗口状态为开，就应关闭串口，防止下次不能使用
                            button1.Enabled = true;                         //打开串口按钮可用
                            button2.Enabled = false;                        //关闭串口按钮不可用
                        }
                    }
                    else                                                    //发送模式是 HEX 模式
                    {
                        string[] sendwithoutKG = textBox2.Text.Split(' ');
                        for (int i = 0; i < sendwithoutKG.Length; i++)
                        {
                            try
                            {
                                int value = Convert.ToInt32(sendwithoutKG[i], 16);
                                Byte[] BSendTemp = BitConverter.GetBytes(value);
                                serialPort1.Write(BSendTemp, 0, 1);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)   //文本控件内容改变时间
        {
            textBox1.ScrollToCaret();                                   //讲滚动条调至最下
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}