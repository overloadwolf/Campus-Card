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
    public partial class Login : Form
    {
        public String UserName;
        public String UserAccount;
        public String UserGroup;
        private Default returnDefault = null;
　　　　public Login(Default F1)
　　　　{
　　　　　　InitializeComponent();
　　　　　　// 接受Form1对象
            this.returnDefault = F1;
　　　　}

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.returnDefault.Visible = true;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "请输入账号")
            {
                textBox1.Focus();
                textBox1.SelectAll();
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "请输入账号")
            {
                textBox1.Focus();
                textBox1.SelectAll();
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "请输入密码")
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "请输入密码")
            {
                textBox2.Focus();
                textBox2.SelectAll();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 'A' || e.KeyChar > 'Z' )&& (e.KeyChar < 'a' || e.KeyChar > 'z')&& (e.KeyChar< '0' ||e.KeyChar> '9') )
            {
                e.Handled = true;

            }
            if (e.KeyChar == 8||e.KeyChar=='.'||e.KeyChar=='_')
            {

                e.Handled = false;

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            String AccountInput;
            String PasswordInput;
            int UserStatus;
            AccountInput = textBox1.Text;
            PasswordInput = textBox2.Text;
            UserAccount = AccountInput;
            if (AccountInput == "请输入账号")
            {MessageBox.Show("请输入账号"); return;}
            if (PasswordInput == "请输入密码")
            {MessageBox.Show("请输入密码"); return;}
            SqlConnection Conn =new SqlConnection("Data Source=DESKTOP-HNRD4LA\\SQLTEST;Initial Catalog=groupworkDB;Integrated Security=True");
            Conn.Open();
            String SqlCommu;
            SqlCommand SqlCmd;
            object Obj;
            //SqlCommu = "DROP VIEW usercheck";
            //SqlCmd = new SqlCommand(SqlCommu, Conn);
            //SqlCmd.ExecuteNonQuery();
            SqlCommu = "create view usercheck as select * from dbo.用户信息数据表 WHERE 用户信息数据表.账号 ='" + AccountInput+"'";
            SqlCmd = new SqlCommand(SqlCommu,Conn);
            SqlCmd.ExecuteNonQuery();
            SqlCommu = "select count(*) from usercheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            Obj = SqlCmd.ExecuteScalar();
            int AccountNum = int.Parse(Obj.ToString());
            if (AccountNum == 0)
            {
                MessageBox.Show("不存在账号" + AccountInput);
            }
            else
            {
                SqlCommu = "select 密码 from usercheck";
                SqlCmd = new SqlCommand(SqlCommu, Conn);
                Obj = SqlCmd.ExecuteScalar();
                if (Obj.ToString() != PasswordInput)
                    MessageBox.Show("账号或密码错误");
                else
                {
                    SqlCommu = "select 姓名 from usercheck";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    Obj = SqlCmd.ExecuteScalar();
                    UserName = Obj.ToString();
                    SqlCommu = "select 身份 from usercheck";
                    SqlCmd = new SqlCommand(SqlCommu, Conn);
                    Obj = SqlCmd.ExecuteScalar();
                    UserGroup = Obj.ToString();
                    UserStatus = int.Parse(UserGroup);
                    switch (UserStatus)
                    {
                        case 0:
                            {
                                Admin Admini = new Admin(returnDefault, this);
                                Admini.Show();
                                this.Close();
                                break;
                            }
                        case 1:
                            {
                                Teacher Teach = new Teacher(returnDefault, this);
                                Teach.Show();
                                this.Close();
                                break;
                            }
                        case 2:
                            {
                                Student Stud = new Student(returnDefault, this);
                                Stud.Show();
                                this.Close();
                                break;
                            }
                    }

                }
            }
            SqlCommu="DROP VIEW usercheck";
            SqlCmd = new SqlCommand(SqlCommu, Conn);
            SqlCmd.ExecuteNonQuery();
            Conn.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//判断回车键
            {

                textBox2.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//判断回车键
            {

                button1_Click(sender,e);
            }
        }

    }
}
