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
    public partial class Admin : Form
    {
        private Default returnDefault = null;
        private String UserName = null;
        private String UserAccount = null;
        public Admin(Default F1, Login F2)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            UserName = F2.UserName;
            UserAccount = F2.UserAccount;
            label1.Text = "欢迎管理员" + UserName;
            panel2.Location = panel1.Location;
            panel3.Location = panel1.Location;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";
            this.returnDefault = F1;
            dateTimePicker2.MinDate = dateTimePicker1.Value;
            dateTimePicker1.MaxDate = dateTimePicker2.Value;
            AccountSetting();
        }
        private void AccountShow()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu = "select * from 用户信息数据表";
            SqlDataAdapter myda = new SqlDataAdapter(SqlCommu, Conn);
            DataTable dt = new DataTable();
            myda.Fill(dt);
            dataGridView1.DataSource = dt;
            Conn.Close();
        }
        private void CourseShow()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu = "select * from 课程数据表";
            SqlDataAdapter myda = new SqlDataAdapter(SqlCommu, Conn);
            DataTable dt = new DataTable();
            myda.Fill(dt);
            dataGridView1.DataSource = dt;
            Conn.Close();
        }
        private void StudentShow()
        {
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu = "select 用户信息数据表.姓名,刷卡信息数据表.* from 刷卡信息数据表,用户信息数据表 WHERE 用户信息数据表.账号=刷卡信息数据表.学生账号 Order by 刷卡信息数据表.刷卡日期,刷卡信息数据表.刷卡时间 DESC";
            SqlDataAdapter myda = new SqlDataAdapter(SqlCommu, Conn);
            DataTable dt = new DataTable();
            myda.Fill(dt);
            dataGridView1.DataSource = dt;
            Conn.Close();
        }
        private void AccountSetting()
        {
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel1.BringToFront();
            AccountShow();
        }
        private void CourseSetting()
        {
            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            this.panel2.BringToFront();
            CourseShow();
        }
        private void StudentSetting()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            this.panel3.BringToFront();
            StudentShow();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "账号信息")
            {
                AccountSetting();
            }
            if (comboBox1.Text == "课程信息")
            {
                CourseSetting();
            }
            if (comboBox1.Text == "刷卡信息")
            {
                StudentSetting();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.returnDefault.Visible = true;
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            String UserNameEdit, UserPasswordEdit, UserAccountEdit;
            int UserKindEdit;
            String SqlCommu;
            object Obj;
            SqlCommand SqlCmd;
            UserAccountEdit = textBox1.Text;
            UserPasswordEdit = textBox2.Text;
            UserNameEdit = textBox3.Text;
            UserKindEdit = comboBox2.SelectedIndex;
            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            SqlCommu = "Create view AccountCheck as select * from 用户信息数据表 where 用户信息数据表.账号 ='" + UserAccountEdit + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from AccountCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            int AccountNumEdit = int.Parse(Obj.ToString());
            SqlCommu = "DROP VIEW AccountCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            if (AccountNumEdit >= 1)
                MessageBox.Show("已存在账号" + UserAccountEdit);
            else
            {
                SqlCommu = "INSERT INTO 用户信息数据表 VALUES('" + UserAccountEdit + "','" + UserPasswordEdit + "','" + UserNameEdit + "','" + UserKindEdit  + "')";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                AccountShow();
            }
            Conn.Close();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (comboBox1.Text == "账号信息")
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                comboBox2.SelectedIndex = int.Parse(dataGridView1.SelectedRows[0].Cells[3].Value.ToString());
            }
            if(comboBox1.Text == "课程信息")
            {
                textBox4.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                textBox5.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                textBox6.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells[5].Value.ToString());
                dateTimePicker2.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells[6].Value.ToString());
                System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo("en-US", false).DateTimeFormat;
                dtfi.ShortTimePattern = "t";
                textBox7.Text = DateTime.Parse(dataGridView1.SelectedRows[0].Cells[3].Value.ToString()).ToShortTimeString().ToString();
                textBox8.Text = DateTime.Parse(dataGridView1.SelectedRows[0].Cells[4].Value.ToString()).ToShortTimeString().ToString();
                String DOW= dataGridView1.SelectedRows[0].Cells[7].Value.ToString();
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    String value = comboBox3.Items[i].ToString();
                    if(DOW==value)
                    {
                        comboBox3.SelectedIndex = i; 
                        break;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == null || textBox2.Text == null || textBox3.Text == null || !(comboBox2.SelectedIndex >= 0 && comboBox2.SelectedIndex <= 2))
                MessageBox.Show("输入不合法");
            else
            {
                if (textBox1.Text == UserAccount)
                    MessageBox.Show("不能删除自己");

                else
                {
                    String UserNameEdit, UserPasswordEdit, UserAccountEdit;
                    int UserKindEdit;
                    String SqlCommu;
                    object Obj;
                    SqlCommand SqlCmd;
                    UserAccountEdit = textBox1.Text;
                    UserPasswordEdit = textBox2.Text;
                    UserNameEdit = textBox3.Text;
                    UserKindEdit = comboBox2.SelectedIndex;
                    SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
                    Conn.Open();
                    SqlCommu = "Create view AccountCheck as select * from 用户信息数据表 where 用户信息数据表.账号 ='" + UserAccountEdit + "'";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    SqlCmd.ExecuteNonQuery();
                    SqlCommu = "select count(*) from AccountCheck";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    Obj = SqlCmd.ExecuteScalar();
                    int AccountNumEdit = int.Parse(Obj.ToString());
                    SqlCommu = "DROP VIEW AccountCheck";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    SqlCmd.ExecuteNonQuery();
                    if (AccountNumEdit < 1)
                        MessageBox.Show("不存在账号" + UserAccountEdit);
                    else
                    {
                        SqlCommu = "Delete from 用户信息数据表 where 用户信息数据表.账号 ='" + UserAccountEdit + "'";
                        SqlCmd = new SqlCommand(SqlCommu, Conn);
                        SqlCmd.ExecuteNonQuery();
                        AccountShow();
                    }
                    Conn.Close();
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == null || textBox5.Text == null || textBox6.Text == null || textBox7.Text==null||textBox8.Text==null)
                MessageBox.Show("输入不合法");
            else
            {
                String CourseNumber, CourseName, CourseTeacher;
                String StartTime, EndTime, StartDate, EndDate;
                String SqlCommu;
                object Obj;
                SqlCommand SqlCmd;
                CourseNumber = textBox4.Text;
                CourseName = textBox5.Text;
                CourseTeacher = textBox6.Text;
                StartTime = DateTime.Parse(textBox7.Text).ToShortTimeString().ToString();
                EndTime = DateTime.Parse(textBox8.Text).ToShortTimeString().ToString();
                StartDate = dateTimePicker1.Value.ToShortDateString().ToString();
                EndDate = dateTimePicker2.Value.ToShortDateString().ToString();
                String DOW = comboBox3.Text.ToString();
                SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
                Conn.Open();
                SqlCommu = "Create view CourseCheck as select * from 课程数据表 where 课程数据表.课程编号 ='" + CourseNumber + "'";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                SqlCommu = "select count(*) from CourseCheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                Obj = SqlCmd.ExecuteScalar();
                int CourseNumberEdit = int.Parse(Obj.ToString());
                SqlCommu = "DROP VIEW CourseCheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                if (CourseNumberEdit >= 1)
                    MessageBox.Show("已存在课程编号" + CourseNumber);
                else
                {
                    SqlCommu = "INSERT INTO 课程数据表 VALUES('" + CourseNumber + "','" + CourseName + "','" + CourseTeacher + "','" + StartTime + "','" + EndTime + "','" + StartDate + "','" + EndDate  +"','"+DOW+ "')";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    SqlCmd.ExecuteNonQuery();
                    CourseShow();
                 }
                 Conn.Close();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            String CourseNumber, CourseName, CourseTeacher;
            String StartTime, EndTime, StartDate, EndDate;
            String SqlCommu;
            object Obj;
            SqlCommand SqlCmd;
            CourseNumber = textBox4.Text;
            CourseName = textBox5.Text;
            CourseTeacher = textBox6.Text;
            StartTime = DateTime.Parse(textBox7.Text).ToShortTimeString().ToString();
            EndTime = DateTime.Parse(textBox8.Text).ToShortTimeString().ToString();
            StartDate = dateTimePicker1.Value.ToShortDateString().ToString();
            EndDate = dateTimePicker2.Value.ToShortDateString().ToString();

            SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            SqlCommu = "Create view CourseCheck as select * from 课程数据表 where 课程数据表.课程编号 ='" + CourseNumber + "'";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from CourseCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            int CourseNumberEdit = int.Parse(Obj.ToString());
            SqlCommu = "DROP VIEW CourseCheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            if (CourseNumberEdit < 1)
                MessageBox.Show("不存在课程" + CourseNumber);
            else
            {
                SqlCommu = "Delete from 课程数据表 where 课程数据表.课程编号 ='" + CourseNumber + "'";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                SqlCmd.ExecuteNonQuery();
                CourseShow();
            }
            Conn.Close();
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = dateTimePicker2.Value;
        }

        private void button6_Click(object sender, EventArgs e)
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
            StudentShow();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
