using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameClient
{
    public partial class FormLogin : Form
    {
        private bool VerifyFail = false;
        private GameClient m_GameClient;
        public GameClient GameClient
        {
            get
            {
                return m_GameClient;
            }
            set
            {
                m_GameClient = value;
                m_GameClient.OnMessage = this.OnMessage;
            }
        }
        public FormLogin()
        {
            InitializeComponent();
        }
        private void ShowTips(string tips)
        {
            MessageBox.Show(tips, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void InvokeOnMessage(Message msg)
        {
            switch(msg.Command)
            {
                case "verify":
                    if (msg.Parameters.Length == 1 && msg.Parameters[0] == "yes")
                    {
                        GameClient.State = ClientState.Verified;
                    }
                    else
                    {
                        ShowTips("客户端版本不是最新版，需要升级");
                        VerifyFail = true;
                    }
                    break;
                case "login":
                    if (msg.Parameters.Length >= 1 && msg.Parameters[0] == "yes" && msg.Prefix != null && msg.Prefix == textBoxLoginName.Text)
                    {
                        GameClient.State = ClientState.Logined;
                        GameClient.ID = textBoxLoginName.Text;
                        FormMain main = new FormMain();
                        main.LoginForm = this;
                        main.GameClient = GameClient;
                        this.Hide();
                        main.Show();
                    }
                    else if (msg.Parameters.Length >= 1 && msg.Parameters[0] == "no")
                    {
                        ShowTips("该昵称正在使用中，试试看换个昵称吧");
                    }
                    break;
            }
        }
        private void OnMessage(Message msg)
        {
            if (GameClient.State == ClientState.Logined || GameClient.State == ClientState.Matching)
            {
                return;
            }
            this.Invoke(new OnMessageDelegate(InvokeOnMessage), msg);
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (VerifyFail)
            {
                ShowTips("客户端版本不是最新版，需要升级");
                return;
            }
            string loginName = textBoxLoginName.Text;
            if (loginName.Contains(' '))
            {
                ShowTips("昵称中不能含有空格");
                return;
            }
            if (loginName.Contains(':'))
            {
                ShowTips("昵称中不能含有冒号");
                return;
            }
            GameClient.PostMessage(new Message(null, "login", loginName));
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameClient.Close();
        }

        private void textBoxLoginName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
