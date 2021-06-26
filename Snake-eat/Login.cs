using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Game Visitor = new Game();
            //Visitor.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            GameOne game1 = new GameOne();
            game1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Game game2 = new Game();
            game2.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form1 game3 = new Form1();
            game3.ShowDialog();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
