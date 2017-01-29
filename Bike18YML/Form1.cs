using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bike18YML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            tbLogin.Text = Properties.Settings.Default.loginBike18.ToString();
            tbPassword.Text = Properties.Settings.Default.passwordBike18.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.loginBike18 = tbLogin.Text;
            Properties.Settings.Default.passwordBike18= tbPassword.Text;
            Properties.Settings.Default.Save();
        }
    }
}
