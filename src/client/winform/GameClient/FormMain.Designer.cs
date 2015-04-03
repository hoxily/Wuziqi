namespace GameClient
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxInfoList = new System.Windows.Forms.ListBox();
            this.radioButtonAll = new System.Windows.Forms.RadioButton();
            this.radioButtonWorld = new System.Windows.Forms.RadioButton();
            this.radioButtonDiscuss = new System.Windows.Forms.RadioButton();
            this.radioButtonWhisper = new System.Windows.Forms.RadioButton();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.comboBoxTarget = new System.Windows.Forms.ComboBox();
            this.panelBoard = new System.Windows.Forms.Panel();
            this.buttonMatch = new System.Windows.Forms.Button();
            this.buttonPart = new System.Windows.Forms.Button();
            this.labelLocation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxInfoList
            // 
            this.listBoxInfoList.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxInfoList.FormattingEnabled = true;
            this.listBoxInfoList.HorizontalScrollbar = true;
            this.listBoxInfoList.ItemHeight = 12;
            this.listBoxInfoList.Location = new System.Drawing.Point(12, 36);
            this.listBoxInfoList.Name = "listBoxInfoList";
            this.listBoxInfoList.Size = new System.Drawing.Size(206, 460);
            this.listBoxInfoList.TabIndex = 0;
            this.listBoxInfoList.TabStop = false;
            this.listBoxInfoList.SelectedIndexChanged += new System.EventHandler(this.listBoxInfoList_SelectedIndexChanged);
            this.listBoxInfoList.DoubleClick += new System.EventHandler(this.listBoxInfoList_DoubleClick);
            // 
            // radioButtonAll
            // 
            this.radioButtonAll.AutoSize = true;
            this.radioButtonAll.Location = new System.Drawing.Point(12, 506);
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.Size = new System.Drawing.Size(47, 16);
            this.radioButtonAll.TabIndex = 1;
            this.radioButtonAll.Text = "全部";
            this.radioButtonAll.UseVisualStyleBackColor = true;
            this.radioButtonAll.CheckedChanged += new System.EventHandler(this.radioButtonWhisper_CheckedChanged);
            // 
            // radioButtonWorld
            // 
            this.radioButtonWorld.AutoSize = true;
            this.radioButtonWorld.Location = new System.Drawing.Point(65, 506);
            this.radioButtonWorld.Name = "radioButtonWorld";
            this.radioButtonWorld.Size = new System.Drawing.Size(47, 16);
            this.radioButtonWorld.TabIndex = 2;
            this.radioButtonWorld.Text = "世界";
            this.radioButtonWorld.UseVisualStyleBackColor = true;
            this.radioButtonWorld.CheckedChanged += new System.EventHandler(this.radioButtonWhisper_CheckedChanged);
            // 
            // radioButtonDiscuss
            // 
            this.radioButtonDiscuss.AutoSize = true;
            this.radioButtonDiscuss.Location = new System.Drawing.Point(118, 506);
            this.radioButtonDiscuss.Name = "radioButtonDiscuss";
            this.radioButtonDiscuss.Size = new System.Drawing.Size(47, 16);
            this.radioButtonDiscuss.TabIndex = 3;
            this.radioButtonDiscuss.Text = "讨论";
            this.radioButtonDiscuss.UseVisualStyleBackColor = true;
            this.radioButtonDiscuss.CheckedChanged += new System.EventHandler(this.radioButtonWhisper_CheckedChanged);
            // 
            // radioButtonWhisper
            // 
            this.radioButtonWhisper.AutoSize = true;
            this.radioButtonWhisper.Location = new System.Drawing.Point(171, 506);
            this.radioButtonWhisper.Name = "radioButtonWhisper";
            this.radioButtonWhisper.Size = new System.Drawing.Size(47, 16);
            this.radioButtonWhisper.TabIndex = 4;
            this.radioButtonWhisper.Text = "私聊";
            this.radioButtonWhisper.UseVisualStyleBackColor = true;
            this.radioButtonWhisper.CheckedChanged += new System.EventHandler(this.radioButtonWhisper_CheckedChanged);
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(65, 529);
            this.textBoxInput.MaxLength = 128;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(153, 21);
            this.textBoxInput.TabIndex = 7;
            this.textBoxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxInput_KeyDown);
            this.textBoxInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInput_KeyPress);
            // 
            // comboBoxTarget
            // 
            this.comboBoxTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTarget.FormattingEnabled = true;
            this.comboBoxTarget.Items.AddRange(new object[] {
            ""});
            this.comboBoxTarget.Location = new System.Drawing.Point(12, 530);
            this.comboBoxTarget.Name = "comboBoxTarget";
            this.comboBoxTarget.Size = new System.Drawing.Size(47, 20);
            this.comboBoxTarget.TabIndex = 6;
            this.comboBoxTarget.TabStop = false;
            // 
            // panelBoard
            // 
            this.panelBoard.Location = new System.Drawing.Point(224, 0);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(550, 550);
            this.panelBoard.TabIndex = 7;
            this.panelBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBoard_Paint);
            this.panelBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBoard_MouseDown);
            this.panelBoard.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelBoard_MouseMove);
            // 
            // buttonMatch
            // 
            this.buttonMatch.Location = new System.Drawing.Point(143, 3);
            this.buttonMatch.Name = "buttonMatch";
            this.buttonMatch.Size = new System.Drawing.Size(75, 23);
            this.buttonMatch.TabIndex = 0;
            this.buttonMatch.TabStop = false;
            this.buttonMatch.Text = "自动匹配";
            this.buttonMatch.UseVisualStyleBackColor = true;
            this.buttonMatch.Click += new System.EventHandler(this.buttonMatch_Click);
            // 
            // buttonPart
            // 
            this.buttonPart.Location = new System.Drawing.Point(143, 3);
            this.buttonPart.Name = "buttonPart";
            this.buttonPart.Size = new System.Drawing.Size(75, 23);
            this.buttonPart.TabIndex = 1;
            this.buttonPart.Text = "离开竞技场";
            this.buttonPart.UseVisualStyleBackColor = true;
            this.buttonPart.Click += new System.EventHandler(this.buttonPart_Click);
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.Location = new System.Drawing.Point(12, 9);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(0, 12);
            this.labelLocation.TabIndex = 8;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.labelLocation);
            this.Controls.Add(this.buttonMatch);
            this.Controls.Add(this.buttonPart);
            this.Controls.Add(this.panelBoard);
            this.Controls.Add(this.comboBoxTarget);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.radioButtonWhisper);
            this.Controls.Add(this.radioButtonDiscuss);
            this.Controls.Add(this.radioButtonWorld);
            this.Controls.Add(this.radioButtonAll);
            this.Controls.Add(this.listBoxInfoList);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "五子棋";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxInfoList;
        private System.Windows.Forms.RadioButton radioButtonAll;
        private System.Windows.Forms.RadioButton radioButtonWorld;
        private System.Windows.Forms.RadioButton radioButtonDiscuss;
        private System.Windows.Forms.RadioButton radioButtonWhisper;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.ComboBox comboBoxTarget;
        private System.Windows.Forms.Panel panelBoard;
        private System.Windows.Forms.Button buttonMatch;
        private System.Windows.Forms.Button buttonPart;
        private System.Windows.Forms.Label labelLocation;

    }
}