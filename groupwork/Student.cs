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
    public partial class Student : Form
    {
        private Default returnDefault = null;
        private String UserName = null;
        private String UserAccount = null;

        public Student(Default F1,Login F2)
        {
            InitializeComponent();
            UserName = F2.UserName;
            UserAccount = F2.UserAccount;
            GetCourse();
            label1.Text = "欢迎" + UserName + "同学";
            this.returnDefault = F1;
            DataShow();
        }
        private void GetCourse()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu;
            SqlCommand SqlCmd;
            Object Obj;
            SqlCommu = "CREATE view CoursePick as SELECT * FROM 学生选课信息表 WHERE 学生选课信息表.学生账号 ='" + UserAccount + "'";
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
            SqlCommu = "SELECT 课程数据表.课程编号,课程名称 FROM 课程数据表,CoursePick WHERE 课程数据表.课程编号=CoursePick.课程编号 AND CONVERT(varchar(10),GETDATE(),23) between 课程数据表.开课时间 and 课程数据表.结课时间";
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
            String PickCourseNumber = "";
            PickCourseNumber = comboBox1.Text;
            if (PickCourseNumber == "System.Data.DataRowView")
                return;
            Conn.Open();
            String SqlCommu;
            SqlCommu = "SELECT 课程数据表.课程名称,刷卡信息数据表.刷卡日期,刷卡信息数据表.刷卡时间 FROM 刷卡信息数据表,课程数据表 WHERE 刷卡信息数据表.课程编号=课程数据表.课程编号 and 刷卡信息数据表.学生账号 ='" + UserAccount + "' and 课程数据表.课程编号 ='" + PickCourseNumber + "'";
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
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataShow();
        }
    }
}
