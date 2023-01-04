using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KinectProject
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 데이터 베이스에 연결시 MySqlConnection 클래스를 사용해야 한다
                string myConnection = "datasource = localhost; port=3306; username=root; password=rootpw; database = c#db";
                
                // 클래스 생성시 Connection String을 넣어 주어야 한다.
                // datasource명, port번호, 사용자명, 암호를 지정해준다.
                MySqlConnection myConn = new MySqlConnection(myConnection);

                // MySqlCommand 클래스에 SQL문을 지정한다.
                MySqlCommand SelectCommand = new MySqlCommand("select * from user " + "where id = '"+this.id.Text + "' and pw = '"+this.pw.Text + "';", myConn);

                // 연결모드로 데이터를 서버에서 가져온다
                MySqlDataReader myReader;

                //MySql과 연결한다.
                myConn.Open();

                //지정된 sql을 실행한다.
                myReader = SelectCommand.ExecuteReader();
                int count = 0;
                

                // SQL 조건절에 맞는 데이터가 있을 때까지 읽고 count 변수를 증가시킨다
                while (myReader.Read())
                {
                    count = count + 1;
                }

                // count가 0보다 크면 로그인 성공이다
                if(count == 1)
                {
                    this.Hide();
                    MessageBox.Show("안녕하세요, 관리자님");
                    MainWindow height = new MainWindow();
                    height.Show();
                }
                // count가 1보다 크면 중복이다.
                else if(count > 1)
                {
                    MessageBox.Show("아이디가 중복입니다.");
                }
                //  그것도 아니라면 아이디 또는 패스워드가 틀렸다.
                else{
                    MessageBox.Show("아이디 또는 패스워드가 불일치합니다.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
