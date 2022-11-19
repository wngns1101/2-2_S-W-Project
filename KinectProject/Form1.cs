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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SingInButton_Click(object sender, EventArgs e)
        {
            try
            {
                string myConnection = "datasource = localhost; port=3306; username=root; password=rootpw; database = c#db";
                MySqlConnection myConn = new MySqlConnection(myConnection);

                MySqlCommand SelectCommand = new MySqlCommand("select * from user " + "where id = '"+this.id.Text + "' and pw = '"+this.pw.Text + "';", myConn);

                MySqlDataReader myReader;
                myConn.Open();
                myReader = SelectCommand.ExecuteReader();
                int count = 0;

                while (myReader.Read())
                {
                    count = count + 1;
                }

                if(count == 1)
                {
                    MessageBox.Show("안녕세요, 관리자님");
                    MainWindow height = new MainWindow();
                }
                else
                {
                    MessageBox.Show("아이디와 패스워드가 불일치합니다.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
