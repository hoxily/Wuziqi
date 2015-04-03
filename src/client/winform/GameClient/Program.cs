using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            GameClient client = new GameClient();
            client.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormLogin login = new FormLogin();
            login.GameClient = client;
            try
            {
                client.Start();
                Application.Run(login);
            }
            catch(Exception e)
            {
                MessageBox.Show("出错信息：\r\n" + e.Message, "非常抱歉，程序出错了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
        }
    }
}
