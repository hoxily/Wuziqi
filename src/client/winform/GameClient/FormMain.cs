using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace GameClient
{
    public partial class FormMain : Form
    {
        private SoundPlayer m_SoundPlayer;
        private Form m_loginForm;
        public Form LoginForm
        {
            get
            {
                return m_loginForm;
            }
            set
            {
                m_loginForm = value;
            }
        }
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
        private void ScrollListBoxToBottom()
        {
            //listBoxInfoList.TopIndex = listBoxInfoList.Items.Count - (listBoxInfoList.Height / listBoxInfoList.ItemHeight);
            // 这样子计算有偏差，总是有两三行挡住没滚到底。
            int count = listBoxInfoList.Items.Count;
            if(count > 0)
            {
                listBoxInfoList.BeginUpdate();
                listBoxInfoList.SelectedIndex = count - 1;
                listBoxInfoList.ClearSelected();
                listBoxInfoList.EndUpdate();
            }
        }
        private void AddLogToAll(string info)
        {
            m_InfoListAll.Add(info);
            if (radioButtonAll.Checked)
            {
                listBoxInfoList.DataSource = null;
                listBoxInfoList.DataSource = m_InfoListAll;
                ScrollListBoxToBottom();
            }
        }
        private void AddLogToWorld(string info)
        {
            AddLogToAll(info);
            m_InfoListWorld.Add(info);
            if(radioButtonWorld.Checked)
            {
                listBoxInfoList.DataSource = null;
                listBoxInfoList.DataSource = m_InfoListWorld;
                ScrollListBoxToBottom();
            }
        }
        private void AddLogToDiscuss(string info)
        {
            AddLogToAll(info);
            m_InfoListDiscuss.Add(info);
            if(radioButtonDiscuss.Checked)
            {
                listBoxInfoList.DataSource = null;
                listBoxInfoList.DataSource = m_InfoListDiscuss;
                ScrollListBoxToBottom();
            }
        }
        private void AddLogToWhisper(string info)
        {
            AddLogToAll(info);
            m_InfoListWhisper.Add(info);
            if(radioButtonWhisper.Checked)
            {
                listBoxInfoList.DataSource = null;
                listBoxInfoList.DataSource = m_InfoListWhisper;
                ScrollListBoxToBottom();
            }
        }
        private void InvokeOnMessage(Message msg)
        {
            switch (msg.Command)
            {
                case "logout":
                    AddLogToWorld(msg.Prefix + " 已离开");
                    break;
                case "login":
                    AddLogToWorld(msg.Prefix + " 成功登录");
                    break;
                case "speak":
                    AddLogToWorld(msg.Prefix + " 喊话：" + msg.Parameters[0]);
                    break;
                case "discuss":
                    AddLogToDiscuss(msg.Prefix + " 发言：" + msg.Parameters[0]);
                    break;
                case "whisper":
                    AddLogToWhisper(msg.Prefix + " 耳语：" + msg.Parameters[0]);
                    break;
                case "match":
                    AddLogToAll("自动匹配失败");
                    buttonMatch.Enabled = true;
                    panelBoard.Cursor = Cursors.Default;
                    GameClient.State = ClientState.Logined;
                    break;
                case "participate":
                    GameClient.Location = msg.Parameters[0];
                    if (msg.Parameters[1] == "player1" )
                    {
                        GameClient.PlayerType = PlayerType.Black;
                        panelBoard.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        GameClient.PlayerType = PlayerType.White;
                        panelBoard.Cursor = Cursors.No;
                    }
                    AddLogToAll("匹配成功！进入竞技场：" + GameClient.Location + "，身份：" + GameClient.PlayerType);
                    labelLocation.Text = GameClient.Location + " : " + GameClient.PlayerType;
                    buttonMatch.Visible = false;
                    buttonPart.Visible = true;
                    buttonPart.Enabled = true;
                    panelBoard.Refresh();
                    break;
                case "part":
                    AddLogToAll("离开竞技场：" + GameClient.Location);
                    labelLocation.Text = "";
                    buttonMatch.Visible = true;
                    buttonMatch.Enabled = true;
                    buttonPart.Visible = false;
                    GameClient.Location = "";
                    panelBoard.Cursor = Cursors.Default;
                    panelBoard.Refresh();
                    m_ChessLayAtList.Clear();
                    break;
                case "layat":
                    int x = Convert.ToInt32(msg.Parameters[0]);
                    int y = Convert.ToInt32(msg.Parameters[1]);
                    string sx = (Convert.ToChar(Convert.ToInt32('A') + x)).ToString();
                    string sy = (y+1).ToString();
                    LayAt(x, y);
                    if (msg.Prefix == GameClient.ID)
                    {
                        AddLogToDiscuss("成功放置棋子于 " + sx + sy);
                        panelBoard.Cursor = Cursors.No;
                        if (IsGameOver())
                        {
                            AddLogToDiscuss("胜利！");
                        }
                    }
                    else
                    {
                        AddLogToDiscuss("对手放置棋子于 " + sx + sy);
                        
                        if (IsGameOver())
                        {
                            AddLogToDiscuss("失败！");
                            panelBoard.Cursor = Cursors.No;
                        }
                        else
                        {
                            panelBoard.Cursor = Cursors.Hand;
                        }
                    }
                    m_SoundPlayer.Play();
                    break;
            }
        }
        private bool IsGameOver()
        {
            if (m_ChessLayAtList.Count == 15 * 15)
            {
                return true;
            }
            bool[,] board = new bool[15, 15];
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    board[x, y] = false;
                }
            }
            int i = m_ChessLayAtList.Count - 1;
            int count = 0;
            while (i >= 0)
            {
                Point p = m_ChessLayAtList[i];
                board[p.X, p.Y] = true;
                i -= 2;
                count++;
            }
            if (count >= 5)
            {
                Point lastPoint = m_ChessLayAtList[m_ChessLayAtList.Count - 1];
                int x = lastPoint.X;
                int y = lastPoint.Y;
                // count vertical line
                int vCount = CountConnectedChess(board, x, y, -1, 0) + CountConnectedChess(board, x, y, 1, 0) - 1;
                if (vCount >= 5)
                {
                    return true;
                }
                // count horizontal line
                int hCount = CountConnectedChess(board, x, y, 0, -1) + CountConnectedChess(board, x, y, 0, 1) - 1;
                if (hCount >= 5)
                {
                    return true;
                }
                // count x1 line : /
                int x1Count = CountConnectedChess(board, x, y, 1, -1) + CountConnectedChess(board, x, y, -1, 1) - 1;
                if (x1Count >= 5)
                {
                    return true;
                }
                // count x2 line: \
                int x2Count = CountConnectedChess(board, x, y, -1, -1) + CountConnectedChess(board, x, y, 1, 1) - 1;
                if (x2Count >= 5)
                {
                    return true;
                }
            }
            return false;
        }
        private int CountConnectedChess(bool[,] board, int x0, int y0, int deltaX, int deltaY)
        {
            int count = 0;
            while(0 <= x0 && x0 < 15 && 0<= y0 && y0 < 15)
            {
                if (board[x0,y0])
                {
                    count++;
                }
                else
                {
                    break;
                }
                x0 += deltaX;
                y0 += deltaY;
            }
            return count;
        }
        private bool IsWin()
        {
            if (IsGameOver())
            {
                if (m_ChessLayAtList.Count % 2 == 0 && GameClient.PlayerType == PlayerType.Black)
                {
                    return true;
                }
                else if(m_ChessLayAtList.Count % 2 == 1 && GameClient.PlayerType == PlayerType.White)
                {
                    return true;
                }
            }
            return false;
        }
        private void LayAt(int x, int y)
        {
            int xpadding = 5;
            int ypadding = 5;
            int chessRadius = 18;
            m_ChessLayAtList.Add(new Point(x, y));
            if (m_ShadowChessX == x && m_ShadowChessY == y)
            {
                m_ShadowChessX = -1;
                m_ShadowChessY = -1;
            }
            Graphics g = panelBoard.CreateGraphics();
            // 截取初始棋盘镜像的一块补丁，贴到当前棋盘上。
            int left = x * 2 * chessRadius + xpadding;
            int top = y * 2 * chessRadius + ypadding;
            g.DrawImage(m_BoardImage, left, top, new Rectangle(left, top, 2 * chessRadius, 2 * chessRadius), GraphicsUnit.Pixel);
            DrawLastChess(g, new Rectangle());
            g.Dispose();
        }
        private bool CanLayAt(int x, int y)
        {
            foreach(Point p in m_ChessLayAtList)
            {
                if (p.X == x && p.Y == y)
                {
                    return false;
                }
            }
            return true;
        }
        private List<Point> m_ChessLayAtList = new List<Point>();
        private void OnMessage(Message msg)
        {
            this.Invoke(new OnMessageDelegate(InvokeOnMessage), msg);
        }
        private List<string> m_InfoListAll;
        private List<string> m_InfoListWorld;
        private List<string> m_InfoListDiscuss;
        private List<string> m_InfoListWhisper;
        public FormMain()
        {
            InitializeComponent();
            m_InfoListAll = new List<string>();
            m_InfoListWorld = new List<string>();
            m_InfoListDiscuss = new List<string>();
            m_InfoListWhisper = new List<string>();
            radioButtonAll.Checked = true;
            Image boardBackgroundImage = Image.FromFile(Application.StartupPath + "\\Resources\\chessboard_background.bmp");
            panelBoard.BackgroundImage = boardBackgroundImage;
            panelBoard.BackgroundImageLayout = ImageLayout.Tile;
            buttonPart.Visible = false;
            m_BlackChessImage = Image.FromFile(Application.StartupPath + "\\Resources\\black_chess.png");
            m_WhiteChessImage = Image.FromFile(Application.StartupPath + "\\Resources\\white_chess.png");
            comboBoxTarget.Items.Clear();
            comboBoxTarget.Items.Add(":世界");
            comboBoxTarget.Items.Add(":竞技场");
            comboBoxTarget.SelectedIndex = 0;
            m_SoundPlayer = new SoundPlayer(Application.StartupPath + "\\Resources\\soundeffect.wav");
            m_SoundPlayer.Load();
        }
        private Image m_BlackChessImage;
        private Image m_WhiteChessImage;
        private int ColorDifference(Color a, Color b)
        {
            return Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoginForm.Close();
        }

        private void radioButtonWhisper_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked)
            {
                listBoxInfoList.DataSource = m_InfoListAll;
            }
            else if(radioButtonWorld.Checked)
            {
                listBoxInfoList.DataSource = m_InfoListWorld;
            }
            else if(radioButtonDiscuss.Checked)
            {
                listBoxInfoList.DataSource = m_InfoListDiscuss;
            }
            else if(radioButtonWhisper.Checked)
            {
                listBoxInfoList.DataSource = m_InfoListWhisper;
            }
        }


        private void listBoxInfoList_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxInfoList.SelectedIndex >= 0)
            {
                string info = listBoxInfoList.Items[listBoxInfoList.SelectedIndex] as string;
                if (info != null)
                {
                    Regex regex = new Regex(@"^[^ ]+ (喊话：|发言：|耳语：)");
                    if (regex.IsMatch(info))
                    {
                        string match = regex.Match(info).Value;
                        string[] splits = match.Split(' ');
                        string id = splits[0];
                        if (comboBoxTarget.Items.Count == 2)
                        {
                            comboBoxTarget.Items.Add(id);
                        }
                        else
                        {
                            comboBoxTarget.Items.RemoveAt(2);
                            comboBoxTarget.Items.Add(id);
                            comboBoxTarget.SelectedIndex = 2;
                        }
                    }
                }
            }
        }
        private void listBoxInfoList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBoxInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxInput.TextLength <= 0)
            {
                return;
            }
            if (e.KeyChar == '\r')
            {
                string target = "";
                if (comboBoxTarget.SelectedIndex >= 0)
                {
                    target = comboBoxTarget.Items[comboBoxTarget.SelectedIndex] as string;
                }
                if (target == ":世界")
                {
                    //speak
                    GameClient.PostMessage(new Message(null, "speak", textBoxInput.Text));
                }
                else if (target == ":竞技场" && GameClient.Location != "")
                {
                    //discuss
                    GameClient.PostMessage(new Message(null, "discuss", textBoxInput.Text));
                }
                else
                {
                    //whisper
                    GameClient.PostMessage(new Message(null, "whisper", target, textBoxInput.Text));
                    AddLogToWhisper("你对 " + target + " 耳语：" + textBoxInput.Text);
                }
                textBoxInput.Clear();
                e.Handled = true;
            }
        }
        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void panelBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle r = e.ClipRectangle;
            if (string.IsNullOrEmpty(GameClient.Location))
            {
                return;
            }
            DrawChessBoard(g, r);
            if (m_BoardImage != null)
            {
                DrawChessShadow(g, r);
            }
            DrawChess(g, r);
            if (m_BoardImage == null)
            {
                Bitmap tmp = new Bitmap(panelBoard.Width, panelBoard.Height);
                panelBoard.DrawToBitmap(tmp, new Rectangle(0, 0, tmp.Width, tmp.Height));
                m_BoardImage = tmp;
            }
        }

        private void DrawChessShadow(Graphics g, Rectangle r)
        {
            if (panelBoard.Cursor != Cursors.Hand)
            {
                return;
            }
            if (m_ShadowChessX == -1 || m_ShadowChessY == -1)
            {
                return;
            }
            int radius = 18 - 2;
            Point p = CalculateChessLocation(m_ShadowChessX, m_ShadowChessY);
            g.DrawPie(new Pen(Color.Blue), p.X - radius, p.Y - radius, 2*radius +1, 2 *radius +1, 0, 360);
        }

        private void DrawChess(Graphics g, Rectangle r)
        {
            Font font = new Font(FontFamily.GenericMonospace, 9, FontStyle.Bold);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            for (int i = 0; i < m_ChessLayAtList.Count; i++)
            {
                Point chessP = m_ChessLayAtList[i];
                Point p = CalculateChessLocation(chessP.X, chessP.Y);
                Point realP = new Point(p.X - m_BlackChessImage.Width / 2, p.Y - m_BlackChessImage.Height / 2);
                string number = (i + 1).ToString();
                SizeF size = g.MeasureString(number, font);
                if (i % 2 == 0)
                {
                    g.DrawImage(m_BlackChessImage, realP);
                    g.DrawString(number, font, whiteBrush, p.X - size.Width / 2, p.Y - size.Height / 2);
                }
                else
                {
                    g.DrawImage(m_WhiteChessImage, realP);
                    g.DrawString(number, font, blackBrush, p.X - size.Width / 2, p.Y - size.Height / 2);
                }
            }
        }
        private void DrawLastChess(Graphics g, Rectangle r)
        {
            Font font = new Font(FontFamily.GenericMonospace, 9, FontStyle.Bold);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            if (m_ChessLayAtList.Count <= 0)
            {
                return;
            }
            int i = m_ChessLayAtList.Count - 1;
            Point chessP = m_ChessLayAtList[i];
            Point p = CalculateChessLocation(chessP.X, chessP.Y);
            Point realP = new Point(p.X - m_BlackChessImage.Width / 2, p.Y - m_BlackChessImage.Height / 2);
            string number = (i + 1).ToString();
            SizeF size = g.MeasureString(number, font);
            if (i % 2 == 0)
            {
                g.DrawImage(m_BlackChessImage, realP);
                g.DrawString(number, font, whiteBrush, p.X - size.Width / 2, p.Y - size.Height / 2);
            }
            else
            {
                g.DrawImage(m_WhiteChessImage, realP);
                g.DrawString(number, font, blackBrush, p.X - size.Width / 2, p.Y - size.Height / 2);
            }
        }

        private void DrawChessBoard(Graphics g, Rectangle r)
        {
            // vertical lines
            for (int x = 0; x < 15; x++)
            {
                Point p0 = CalculateChessLocation(x, 0);
                Point p1 = CalculateChessLocation(x, 14);
                g.DrawLine(new Pen(Color.Black), p0, p1);
            }
            // horizontal lines
            for (int y = 0; y < 15; y++)
            {
                Point p0 = CalculateChessLocation(0, y);
                Point p1 = CalculateChessLocation(14, y);
                g.DrawLine(new Pen(Color.Black), p0, p1);
            }
            // five anchors
            DrawChessBoardAnchor(g, r, 3, 3);// left up anchor
            DrawChessBoardAnchor(g, r, 11, 3);// right up anchor
            DrawChessBoardAnchor(g, r, 7, 7);// center anchor
            DrawChessBoardAnchor(g, r, 3, 11);// left down anchor
            DrawChessBoardAnchor(g, r, 11, 11);// right down anchor
            // scales
            DrawChessBoardScales(g, r);
        }

        private void DrawChessBoardScales(Graphics g, Rectangle r)
        {
            Font font = new Font(FontFamily.GenericMonospace, 9.0f, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            string[] hscales = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
            string[] vscales = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            SizeF size15 = g.MeasureString("15", font);
            SizeF sizeO = g.MeasureString("O", font);
            // vertical scale
            for (int y = 0; y < 15; y++)
            {
                Point p = CalculateChessLocation(0, y);
                PointF fp = new PointF(p.X - size15.Width, p.Y - size15.Height / 2.0f);
                g.DrawString(vscales[y], font, brush, fp);
            }
            // horizontal scale
            for (int x = 0; x < 15; x++)
            {
                Point p = CalculateChessLocation(x, 0);
                PointF fp = new PointF(p.X - sizeO.Width / 2.0f, p.Y - sizeO.Height);
                g.DrawString(hscales[x], font, brush, fp);
            }
        }

        private void DrawChessBoardAnchor(Graphics g, Rectangle r, int chessX, int chessY)
        {
            Point rectangleCenter = CalculateChessLocation(chessX, chessY);
            int rectangleWidthHeight = 9;
            Rectangle rect = new Rectangle(rectangleCenter.X - rectangleWidthHeight / 2, rectangleCenter.Y - rectangleWidthHeight / 2, rectangleWidthHeight, rectangleWidthHeight);
            g.FillRectangle(new SolidBrush(Color.Black), rect);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chessX">0 &lt;= chessX &lt;= 14</param>
        /// <param name="chessY">0 &lt;= chessY &lt;= 14</param>
        /// <returns></returns>
        private Point CalculateChessLocation(int chessX, int chessY)
        {
            int xpadding = 5;
            int ypadding = 5;
            int chessRadius = 18;
            int x = xpadding + chessRadius * (2 * chessX + 1);
            int y = ypadding + chessRadius * (2 * chessY + 1);
            return new Point(x, y);
        }

        private void buttonMatch_Click(object sender, EventArgs e)
        {
            AddLogToAll("请求自动匹配...");
            buttonMatch.Enabled = false;
            panelBoard.Cursor = Cursors.WaitCursor;
            GameClient.PostMessage(new Message(null, "match"));
            GameClient.State = ClientState.Matching;
        }

        private void buttonPart_Click(object sender, EventArgs e)
        {
            AddLogToAll("请求离开竞技场：" + GameClient.Location);
            buttonPart.Enabled = false;
            panelBoard.Cursor = Cursors.WaitCursor;
            GameClient.PostMessage(new Message(null, "part", GameClient.Location));
        }
        private int m_ShadowChessX = -1;
        private int m_ShadowChessY = -1;
        private Bitmap m_BoardImage = null;
        private void panelBoard_MouseMove(object sender, MouseEventArgs e)
        {
            int xpadding = 5;
            int ypadding = 5;
            int chessRadius = 18;
            if (e.X <= xpadding || e.X >= panelBoard.Width - xpadding)
            {
                return;
            }
            if (e.Y <= ypadding || e.Y >= panelBoard.Height - ypadding)
            {
                return;
            }
            int x = (e.X - xpadding) / (2 * chessRadius);
            int y = (e.Y - ypadding) / (2 * chessRadius);
            if (x != m_ShadowChessX || y != m_ShadowChessY)
            {
                if (!HaveChessAt(x, y))
                {
                    UpdateShadow(x, y);
                }
            }
        }

        private bool HaveChessAt(int x, int y)
        {
            foreach (Point p in m_ChessLayAtList)
            {
                if (p.X == x && p.Y == y)
                {
                    return true;
                }
            }
            return false;
        }
        private void UpdateShadow(int newX, int newY)
        {
            int xpadding = 5;
            int ypadding = 5;
            int chessRadius = 18;
            Graphics g = panelBoard.CreateGraphics();
            // clear previous shadow
            if (m_ShadowChessX != -1 && m_ShadowChessY != -1 && m_BoardImage != null && !string.IsNullOrEmpty(GameClient.Location))
            {
                // 截取初始棋盘镜像的一块补丁，贴到当前棋盘上。
                int left = m_ShadowChessX * 2 * chessRadius + xpadding;
                int top = m_ShadowChessY * 2 * chessRadius + ypadding;
                g.DrawImage(m_BoardImage, left, top, new Rectangle(left, top, 2 * chessRadius, 2 * chessRadius), GraphicsUnit.Pixel);
            }
            m_ShadowChessX = newX;
            m_ShadowChessY = newY;
            DrawChessShadow(g, new Rectangle());
            g.Dispose();
        }

        private void panelBoard_MouseDown(object sender, MouseEventArgs e)
        {
            if (panelBoard.Cursor == Cursors.WaitCursor)
            {
                return;
            }
            if (panelBoard.Cursor == Cursors.No)
            {
                return;
            }
            if (GameClient.Location == "")
            {
                return;
            }
            if (IsGameOver())
            {
                return;
            }
            int xpadding = 5;
            int ypadding = 5;
            int chessRadius = 18;
            if (e.X <= xpadding || e.X >= panelBoard.Width - xpadding)
            {
                return;
            }
            if (e.Y <= ypadding || e.Y >= panelBoard.Height - ypadding)
            {
                return;
            }
            int x = (e.X - xpadding) / (2 * chessRadius);
            int y = (e.Y - ypadding) / (2 * chessRadius);
            if (CanLayAt(x, y))
            {
                GameClient.PostMessage(new Message(null, "layat", x.ToString(), y.ToString()));
                panelBoard.Cursor = Cursors.WaitCursor;
            }
        }


    }
}
