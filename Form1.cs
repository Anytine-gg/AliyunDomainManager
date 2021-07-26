using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


namespace DomainManager
{
    public partial class Form1 : Form
    {
        public void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public Form1()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string Inipath = path + "set.ini";
            int.TryParse(Configini.Read("SETTINGS", "AutoLogin", "0", Inipath), out autologin);
            accessKeyId = Configini.Read("KEY", "accessKeyId", "", Inipath);
            secret = Configini.Read("KEY", "secret", "", Inipath);
            if (autologin == 1)
            {
                domain = new Domain(accessKeyId, secret);
                UpdateList();
                On();
            }
            AddNew.Enabled = false;
            RemoveSingle.Enabled = false;
            updateDomain.Enabled = false;

            this.dlist.Width = this.DomainList.Width - 5;
            this.Domainson.Width = this.listView1.Width - 5;
            ADM.Visible = true;
        }
        ///
        /// 全局变量区域
        ///
        public static string accessKeyId;
        public static string secret;
        private Domain domain;
        private JObject domainNameListJObject;
        private JObject SubdomainNameListJObject = new JObject();
        private string MainDomain; //全局选择域名
        private string SubDomain; //全局子域名
        private string RecordId; //选择的域名记录ID
        private int DomainlistId;
        private int SubDomainlistID;
        static string BR = "\r\n";
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private Image green = global::DomainManager.Properties.Resources.icon_status_dot_green;
        private static int autologin;
        Form2 f = new Form2();
        ///
        ///有效方法区
        ///
        private void On()
        {
            AddNew.Enabled = true;
            RemoveSingle.Enabled = true;
            updateDomain.Enabled = true;
        }
        public void UpdateList()
        {
            DomainList.Items.Clear();
            SubdomainNameListJObject.RemoveAll();
            int i = 1;
            domainNameListJObject = domain.GetInfo();
            if (domainNameListJObject != null)
            {
                pictureBox1.Image = this.green;
                button2.Enabled = false;
                DomainList.BeginUpdate();
                foreach (var val in domainNameListJObject["Domains"]["Domain"])
                {
                    string domainName = val["DomainName"].ToString(); //域名名称
                    JObject details = domain.GetInfo(domainName); //域名下的子域名
                    SubdomainNameListJObject.Add(domainName, details); // 添加json {domainName:details}
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = domainName;
                    this.DomainList.Items.Add(lvi);
                    i++;
                }
                DomainList.EndUpdate();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (accessKeyId == "" || secret == "")
            {
                MessageBox.Show("尚未设置AK!点击以设置AK");
                f.ShowDialog();
            }
            else
            {
                domain = new Domain(accessKeyId, secret);

                On();
                UpdateList();
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (secret == "" || accessKeyId == "")
            {
                MessageBox.Show("设置AK!");
                f.ShowDialog();
            }
        }
        private void DomainList_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            ListView.SelectedIndexCollection indexes = this.DomainList.SelectedIndices;
            if (indexes.Count > 0)
            {
                listView1.BeginUpdate();
                int index = indexes[0];
                this.DomainlistId = index;
                string domainname = this.DomainList.Items[index].SubItems[0].Text; //获取第一列的值
                this.MainDomain = domainname;
                textBox2.Text = this.MainDomain;
                JToken JSON = SubdomainNameListJObject[domainname];
                for (int i = 0; i < JSON["DomainRecords"]["Record"].Count(); i++)
                {
                    JToken val = JSON["DomainRecords"]["Record"][i];
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = val["RR"].ToString() + "." + val["DomainName"].ToString(); //第一栏为子域名 sss.xxx.com
                    lvi.SubItems.Add(val["DomainName"].ToString()); //第二栏为父域名 xxx.com
                    lvi.SubItems.Add(i.ToString()); //第三栏为id
                    this.listView1.Items.Add(lvi);
                }
                listView1.EndUpdate();
            }
            CleanAll();
            ClearMemory();
        }
        private void DomainList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = this.DomainList.Columns[e.ColumnIndex].Width;
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = this.listView1.SelectedIndices;
            if (indexes.Count > 0)
            {
                int index = indexes[0]; //index为选中数据的行数
                this.SubDomainlistID = index;
                string subdomainname = this.listView1.Items[index].SubItems[0].Text; //获取第一列的值
                string DOMIANNAME = this.listView1.Items[index].SubItems[1].Text; //获取第二列的值
                int id;
                int.TryParse(this.listView1.Items[index].SubItems[2].Text, out id); //获取第三列的值
                this.SubDomain = subdomainname;
                textBox2.Text = subdomainname;
                JToken jt = SubdomainNameListJObject[DOMIANNAME]["DomainRecords"]["Record"][id];
                textBox1.Text = jt["RR"].ToString();
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(jt["Type"].ToString());
                textBox3.Text = jt["Value"].ToString();
                textBox4.Text = jt["TTL"].ToString();
                this.RecordId = jt["RecordId"].ToString();
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = this.textBox1.Text + "." + this.MainDomain;
        }
        private void updateDomain_Click(object sender, EventArgs e)
        {
            int TTL;
            int.TryParse(textBox4.Text, out TTL);
            int res = 0;
            if (listView1.SelectedItems.Count > 0)
            {
                DialogResult result =
                    MessageBox.Show("确定更新该记录吗", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                if (result == DialogResult.OK)
                {
                    res = domain.UPDATE_Domain(this.RecordId,
                        textBox1.Text, comboBox1.Text,
                        textBox3.Text, TTL);
                }
            }
            else
            {
                MessageBox.Show("请选择一个域名!");
            }
            if (res == 1)
            {
                focusOn();
                this.listView1.Focus();
                this.listView1.Items[this.SubDomainlistID].Selected = true;
            }
        }
        private void RemoveSingle_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string Domain = this.MainDomain;
                string RR = textBox1.Text;
                string Type = comboBox1.Text;
                string Value = textBox3.Text;
                int TTL;
                int.TryParse(textBox4.Text, out TTL);
                string tip = "确定要删除该记录吗？\r\n信息：\r\n" + "域名：" + Domain + BR
                             + "主机记录：" + RR + BR + "记录类型：" + Type +
                             BR + "记录值：" + Value +
                             BR + "TTL：" + TTL + BR;
                DialogResult result =
                    MessageBox.Show(tip, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                if (result == DialogResult.OK)
                {
                    DialogResult result1 = MessageBox.Show("注意：该操作无法撤回！！！", "提示", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Asterisk);
                    if (result1 == DialogResult.OK)
                    {
                        int res = domain.DelSingleDomain(this.RecordId);
                        if (res == 1)
                        {
                            focusOn();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择一个域名！");
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    this.Visible = false;
                    return;
                }
            }

            base.WndProc(ref m);
        }
        private void focusOn()
        {
            UpdateList();
            this.DomainList.Focus();
            this.DomainList.Items[this.DomainlistId].Selected = true;
        }
        private void AddNew_Click(object sender, EventArgs e)
        {
            string Domain = this.MainDomain;
            string RR = textBox1.Text;
            string Type = comboBox1.Text;
            string Value = textBox3.Text;
            int TTL;
            int.TryParse(textBox4.Text, out TTL);
            string BR = "\r\n";
            string tip = "确定要添加该记录吗？\r\n信息：\r\n" + "域名：" + Domain + BR + "主机记录：" + RR + BR + "记录类型：" + Type + BR +
                         "记录值：" + Value + BR + "TTL：" + TTL + BR;
            DialogResult result = MessageBox.Show(tip, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            if (result == DialogResult.OK)
            {
                int a = domain.AddNewRecord(Domain, RR, Type, Value, TTL);
                if (a == 1)
                {
                    focusOn();
                }
            }
        }
        private void 更改AKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f.ShowDialog();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void 显示窗体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                this.Show();
                this.Focus();
            }
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void CleanAll()
        {
            textBox1.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = -1;
        }


        [DllImport("user32", EntryPoint = "HideCaret")]
        private static extern bool HideCaret(IntPtr hWnd);
        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            HideCaret(((TextBox)sender).Handle);
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
