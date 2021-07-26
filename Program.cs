using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainManager
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public class Configini
    {
        //两个读写ini文件的API

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        //读取ini文件 section表示ini文件中的节点名，key表示键名 def没有查到的话返回的默认值 filePath文件路径
        public static string Read(string section, string key, string def, string filePath)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, sb, 1024, filePath);
            return sb.ToString();
        }

        //写如ini文件 section表示ini文件中的节点名，key表示键名 value写入的值 filePath文件路径
        public static int Write(string section, string key, string value, string filePath)
        {
            //CheckPath(filePath);
            return WritePrivateProfileString(section, key, value, filePath);
        }

        //删除section
        public static int DeleteSection(string section, string filePath)
        {
            return Write(section, null, null, filePath);
        }

        //删除键
        public static int DeleteKey(string section, string key, string filePath)
        {
            return Write(section, key, null, filePath);
        }
    }
}
