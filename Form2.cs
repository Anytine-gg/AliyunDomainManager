using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainManager
{
    public partial class Form2 : Form
    {
        private static string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private static string Inipath = path + "set.ini";

        public Form2()
        {
            InitializeComponent();
            int auto;

            int.TryParse(Configini.Read("SETTINGS", "AutoLogin", "1", Inipath), out auto);

            if (auto == 1)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

            textBox1.Text = Configini.Read("KEY", "accessKeyId", null, Inipath);
            textBox2.Text = Configini.Read("KEY", "secret", null, Inipath);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.TabIndex = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configini.Write("KEY", "accessKeyId", textBox1.Text, Inipath);
            Configini.Write("KEY", "secret", textBox2.Text, Inipath);
            Form1.accessKeyId = textBox1.Text;
            Form1.secret = textBox2.Text;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Configini.Write("SETTINGS", "AutoLogin", "1", Inipath);
            }
            else
            {
                Configini.Write("SETTINGS", "AutoLogin", "0", Inipath);
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Process.Start("https://usercenter.console.aliyun.com/#/manage/ak");
        }
    }
}
