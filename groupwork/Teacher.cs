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

namespace groupwork
{
    public partial class Teacher : Form
    {
        private Default returnDefault = null;
        private String UserName = null;
        private String UserAccount = null;
        public Teacher(Default F1, Login F2)
        {
            InitializeComponent();
            UserName = F2.UserName;
            UserAccount = F2.UserAccount;
            label1.Text = "欢迎" + UserName + "老师";
            this.returnDefault = F1;
            GetCourse();
        }
        private void GetCourse()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu;
            SqlCommand SqlCmd;
            Object Obj;
            SqlCommu = "CREATE view CoursePick as SELECT * FROM 课程数据表 WHERE 任课教师='" + UserName + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from CoursePick";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            int CoursePickNum = int.Parse(Obj.ToString());
            if (CoursePickNum == 0)
            {
                SqlCommu = "DROP VIEW CoursePick";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                Conn.Close();
                return;
            }
            SqlCommu = "SELECT 课程数据表.课程编号,课程数据表.课程名称 FROM 课程数据表,CoursePick WHERE 课程数据表.课程编号=CoursePick.课程编号 AND CONVERT(varchar(10),GETDATE(),23) between 课程数据表.开课时间 and 课程数据表.结课时间";
            SqlDataAdapter myda = new SqlDataAdapter(SqlCommu, Conn);
            DataTable dt = new DataTable();
            myda.Fill(dt);
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "课程编号";
            SqlCommu = "DROP VIEW CoursePick";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            Conn.Close();
        }
        private void DataShow()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String CourseNumber = "";
            CourseNumber = comboBox1.Text;
            String SqlCommu = "select 用户信息数据表.姓名,刷卡信息数据表.* from 刷卡信息数据表,用户信息数据表 WHERE 用户信息数据表.账号=刷卡信息数据表.学生账号 and 刷卡信息数据表.课程编号='" + CourseNumber + "'Order by 刷卡信息数据表.刷卡日期,刷卡信息数据表.刷卡时间 DESC";
            SqlDataAdapter myda = new SqlDataAdapter(SqlCommu, Conn);
            DataTable dt = new DataTable();
            myda.Fill(dt);
            dataGridView1.DataSource = dt;
            Conn.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.returnDefault.Visible = true;
            this.Close();
        }

        private void Teacher_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataShow();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String StudentNumber = textBox1.Text;
            String CourseNumber = "";
            SqlCommand SqlCmd;
            String SqlCommu;
            Object Obj;
            CourseNumber = comboBox1.Text;
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            SqlCommu = "CREATE view AddRecordCheck as select 课程数据表.* from 课程数据表,学生选课信息表 where 学生选课信息表.课程编号=课程数据表.课程编号 and 学生选课信息表.学生账号='" + StudentNumber + "' and 课程数据表.课程编号 ='" + CourseNumber + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from AddRecordCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            SqlCommu = "DROP VIEW AddRecordCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            int AddRecord = int.Parse(Obj.ToString());
            if(AddRecord <= 0)
            {
                MessageBox.Show("不存在该学生");
                Conn.Close();
                return;
            }
            else
            {
                SqlCommu = "CREATE view RecordCheck as SELECT * FROM 刷卡信息数据表 WHERE 刷卡信息数据表.学生账号 ='" + StudentNumber + "'and 刷卡信息数据表.课程编号='" + CourseNumber + " ' and CONVERT(varchar(10),GETDATE(),23) = 刷卡信息数据表.刷卡日期";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                SqlCommu = "select count(*) from RecordCheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                Obj = SqlCmd.ExecuteScalar();
                int IsRecord = int.Parse(Obj.ToString());
                SqlCommu = "DROP VIEW RecordCheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                if (IsRecord > 0)
                {
                    MessageBox.Show("该学生今日已签到");
                    Conn.Close();
                    return;
                }
                else
                {
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
                    long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
                    SqlCommu = "INSERT INTO 刷卡信息数据表 VALUES('" + timeStamp.ToString() + "','" + StudentNumber + "','" + CourseNumber + "','" + DateTime.Now.ToShortDateString().ToString() + "','" + DateTime.Now.ToShortTimeString().ToString() + "')";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    SqlCmd.ExecuteNonQuery();
                    MessageBox.Show("刷卡信息记录成功");//加延时窗口
                    DataShow();
                }
                Conn.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String ReadyToDelete = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            String SqlCommu;
            object Obj;
            SqlCommand SqlCmd;
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            SqlCommu = "Create view RecordCheck as select * from 刷卡信息数据表 where 信息编号 ='" + ReadyToDelete + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from RecordCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            int CourseNumberEdit = int.Parse(Obj.ToString());
            SqlCommu = "DROP VIEW RecordCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            if (CourseNumberEdit < 1)
            {
                MessageBox.Show("不存在记录");
                Conn.Close();
                return;
            }
            SqlCommu = "Delete from 刷卡信息数据表 where 信息编号 ='" + ReadyToDelete + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            DataShow();
            Conn.Close();
        }
    }
}
