using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Text.RegularExpressions;   

namespace groupwork
{
    public partial class Default : Form
    {
        private string CardNumber = "";
        private string LastCardNumber = "";
        public Default()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
        private SerialPort Comm = new SerialPort();
        private StringBuilder builder = new StringBuilder();
        private void PortRead()
        {
            if (Comm.IsOpen == false)
                return;
            string DataRecive = "";
            int n = Comm.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致   
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据     
            Comm.Read(buf, 0, n);//读取缓冲数据   
            builder.Clear();//清除字符串构造器的内容   
            //因为要访问ui资源，所以需要使用invoke方式同步ui。   
            this.Invoke((EventHandler)(delegate
            {
                foreach (byte b in buf)
                {
                    builder.Append(b.ToString("X2")+"");
                }
            }));
            DataRecive = builder.ToString();
            if (DataRecive != "")
            {
                if(DataRecive.Length==12)
                {
                    DataRecive = DataRecive.Substring(0, DataRecive.Length - 1);
                    CardNumber = DataRecive;
                }
            }
        }
        private void CardRead()
        {
            PortRead();
            if(CardNumber!=""&&CardNumber!=LastCardNumber)
            {
                CardRecord();
                CardNumber = String.Empty;
            }
        }
        private void CardRecord()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu;
            SqlCommand SqlCmd;
            Object Obj;
            SqlCommu = "create view studentcheck as select * from dbo.用户信息数据表 WHERE 用户信息数据表.账号 ='" + CardNumber + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from studentcheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            int AccountNum = int.Parse(Obj.ToString());
            SqlCommu = "select 身份 from studentcheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            SqlCommu = "DROP VIEW studentcheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            int UserIdentify = int.Parse(Obj.ToString());
            if (AccountNum == 0||UserIdentify!=2)
            {
                MessageBox.Show(CardNumber + "刷卡失败");
                CardNumber = String.Empty;
                Conn.Close();
               return;
            }
            else
            {
                SqlCommu = "CREATE view CoursePick as SELECT * FROM 学生选课信息表 WHERE 学生选课信息表.学生账号 ='" + CardNumber + "'";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                SqlCommu = "select count(*) from CoursePick";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                Obj = SqlCmd.ExecuteScalar();
                int CoursePickNum = int.Parse(Obj.ToString());
                if(CoursePickNum==0)
                {
                    SqlCommu = "DROP VIEW CoursePick";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    SqlCmd.ExecuteNonQuery();
                    Conn.Close();
                    return;
                }
                String DOW=DateTime.Now.DayOfWeek.ToString();
                SqlCommu = "SELECT 课程数据表.课程编号 FROM 课程数据表,CoursePick WHERE 课程数据表.课程编号=CoursePick.课程编号 AND CONVERT(varchar(10),GETDATE(),23) between 课程数据表.开课时间 and 课程数据表.结课时间 AND CONVERT(varchar(100), GETDATE(), 8) between 课程数据表.上课时间 and 课程数据表.下课时间 and 课程数据表.上课日 = '"+DOW+"'";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                Obj = SqlCmd.ExecuteScalar();
                String CardRecordCourseNum = Obj.ToString();//刷卡课程
                SqlCommu = "DROP VIEW CoursePick";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                SqlCommu = "CREATE view RecordCheck as SELECT * FROM 刷卡信息数据表 WHERE 刷卡信息数据表.学生账号 ='" + CardNumber + "'and 刷卡信息数据表.课程编号='" + CardRecordCourseNum + " ' and CONVERT(varchar(10),GETDATE(),23) = 刷卡信息数据表.刷卡日期";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                SqlCommu = "select count(*) from RecordCheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                Obj = SqlCmd.ExecuteScalar();
                int IsRecord = int.Parse(Obj.ToString());
                SqlCommu = "DROP VIEW RecordCheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                if(IsRecord>0)
                {
                    Conn.Close();
                    return;
                }
                else
                {
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
                    long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
                    SqlCommu = "INSERT INTO 刷卡信息数据表 VALUES('" + timeStamp.ToString() + "','" + CardNumber + "','" + CardRecordCourseNum + "','" + DateTime.Now.ToShortDateString().ToString() + "','" + DateTime.Now.ToShortTimeString().ToString() + "')";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    SqlCmd.ExecuteNonQuery();
                    MessageBox.Show("刷卡信息记录成功");//加延时窗口
                    LastCardNumber = CardNumber;
                    CardNumber = "";
                }
                Conn.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            String strYMD = DateTime.Now.ToLongDateString();;
            String strT = currentTime.ToString("t");
            label5.Text = strYMD;
            label6.Text = strT;
            CardRead();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            Login LoginForm = new Login(this);
            this.Hide();
            LoginForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AboutBox1 About = new AboutBox1();
            About.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] Ports = SerialPort.GetPortNames();
            Array.Sort(Ports);
            if (Ports != null/* && Ports.Length == 0*/)
            {
                comboBox1.Items.Clear();
                int temp = 0;
                foreach (string PortName in Ports)
                {
                    comboBox1.Items.Add(PortName);
                    temp++;
                }
                if(temp>0)
                    comboBox1.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txt = "";
            txt = comboBox1.Text;
            if (txt == "")
                return;
            if (Comm.PortName == txt)
                return;
            Comm.PortName = txt;
            Comm.BaudRate = 19200;
            Comm.Open();
        }
    }
}
