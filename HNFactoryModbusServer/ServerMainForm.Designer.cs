namespace HNFactoryModbusServer
{
    partial class ServerMainForm
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button_play = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nodeMgrBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dvPLCServer = new System.Windows.Forms.DataGridView();
            this.btnConAll = new System.Windows.Forms.Button();
            this.Col_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLC_Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLC_IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLC_Port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLC_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PLC_RefRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeMgrBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dvPLCServer)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.btnConAll);
            this.groupBox1.Controls.Add(this.dvPLCServer);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button_play);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button_start);
            this.groupBox1.Controls.Add(this.button_stop);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 379);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox1.Size = new System.Drawing.Size(1168, 444);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(695, 42);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(99, 39);
            this.button4.TabIndex = 8;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button_play
            // 
            this.button_play.Location = new System.Drawing.Point(495, 35);
            this.button_play.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_play.Name = "button_play";
            this.button_play.Size = new System.Drawing.Size(138, 52);
            this.button_play.TabIndex = 7;
            this.button_play.Text = "播放数据";
            this.button_play.UseVisualStyleBackColor = true;
            this.button_play.Click += new System.EventHandler(this.button_play_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(180, 35);
            this.button3.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(138, 52);
            this.button3.TabIndex = 6;
            this.button3.Text = "查看寄存器";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(337, 35);
            this.button2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(138, 52);
            this.button2.TabIndex = 5;
            this.button2.Text = "查看日志";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(22, 35);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 52);
            this.button1.TabIndex = 4;
            this.button1.Text = "停止所选";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(851, 35);
            this.button_start.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(138, 52);
            this.button_start.TabIndex = 0;
            this.button_start.Text = "全部启动";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(1014, 35);
            this.button_stop.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(138, 52);
            this.button_stop.TabIndex = 1;
            this.button_stop.Text = "全部停止";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_No,
            this.Column_Name,
            this.Column_Port,
            this.Column_type,
            this.Column_Status});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 72;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1168, 379);
            this.dataGridView1.TabIndex = 9;
            // 
            // Column_No
            // 
            this.Column_No.HeaderText = "编号";
            this.Column_No.MinimumWidth = 9;
            this.Column_No.Name = "Column_No";
            this.Column_No.ReadOnly = true;
            // 
            // Column_Name
            // 
            this.Column_Name.HeaderText = "名称";
            this.Column_Name.MinimumWidth = 9;
            this.Column_Name.Name = "Column_Name";
            this.Column_Name.ReadOnly = true;
            // 
            // Column_Port
            // 
            this.Column_Port.HeaderText = "本地端口";
            this.Column_Port.MinimumWidth = 9;
            this.Column_Port.Name = "Column_Port";
            this.Column_Port.ReadOnly = true;
            // 
            // Column_type
            // 
            this.Column_type.HeaderText = "设备类型";
            this.Column_type.MinimumWidth = 9;
            this.Column_type.Name = "Column_type";
            // 
            // Column_Status
            // 
            this.Column_Status.HeaderText = "状态";
            this.Column_Status.MinimumWidth = 9;
            this.Column_Status.Name = "Column_Status";
            this.Column_Status.ReadOnly = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tag = "更新窗口显示";
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dvPLCServer
            // 
            this.dvPLCServer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvPLCServer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_No,
            this.PLC_Title,
            this.PLC_IP,
            this.PLC_Port,
            this.PLC_Status,
            this.PLC_RefRate});
            this.dvPLCServer.Location = new System.Drawing.Point(22, 111);
            this.dvPLCServer.Name = "dvPLCServer";
            this.dvPLCServer.RowHeadersWidth = 72;
            this.dvPLCServer.RowTemplate.Height = 33;
            this.dvPLCServer.Size = new System.Drawing.Size(1130, 244);
            this.dvPLCServer.TabIndex = 9;
            // 
            // btnConAll
            // 
            this.btnConAll.Location = new System.Drawing.Point(851, 386);
            this.btnConAll.Name = "btnConAll";
            this.btnConAll.Size = new System.Drawing.Size(138, 46);
            this.btnConAll.TabIndex = 10;
            this.btnConAll.Text = "全部连接";
            this.btnConAll.UseVisualStyleBackColor = true;
            this.btnConAll.Click += new System.EventHandler(this.btnConAll_Click);
            // 
            // Col_No
            // 
            this.Col_No.HeaderText = "编号";
            this.Col_No.MinimumWidth = 9;
            this.Col_No.Name = "Col_No";
            this.Col_No.ReadOnly = true;
            this.Col_No.Width = 120;
            // 
            // PLC_Title
            // 
            this.PLC_Title.HeaderText = "PLC服务器";
            this.PLC_Title.MinimumWidth = 9;
            this.PLC_Title.Name = "PLC_Title";
            this.PLC_Title.ReadOnly = true;
            this.PLC_Title.Width = 200;
            // 
            // PLC_IP
            // 
            this.PLC_IP.HeaderText = "服务器IP";
            this.PLC_IP.MinimumWidth = 9;
            this.PLC_IP.Name = "PLC_IP";
            this.PLC_IP.ReadOnly = true;
            this.PLC_IP.Width = 200;
            // 
            // PLC_Port
            // 
            this.PLC_Port.HeaderText = "服务器端口";
            this.PLC_Port.MinimumWidth = 9;
            this.PLC_Port.Name = "PLC_Port";
            this.PLC_Port.ReadOnly = true;
            this.PLC_Port.Width = 200;
            // 
            // PLC_Status
            // 
            this.PLC_Status.HeaderText = "连接状态";
            this.PLC_Status.MinimumWidth = 9;
            this.PLC_Status.Name = "PLC_Status";
            this.PLC_Status.ReadOnly = true;
            this.PLC_Status.Width = 200;
            // 
            // PLC_RefRate
            // 
            this.PLC_RefRate.HeaderText = "刷新频率";
            this.PLC_RefRate.MinimumWidth = 9;
            this.PLC_RefRate.Name = "PLC_RefRate";
            this.PLC_RefRate.ReadOnly = true;
            this.PLC_RefRate.Width = 175;
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tag = "更新PLC服务连接状态";
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1014, 386);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(138, 46);
            this.button5.TabIndex = 11;
            this.button5.Text = "全部断开";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // ServerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 823);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerMainForm";
            this.Text = "Modbus TCP服务主线程";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerMainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nodeMgrBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dvPLCServer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_play;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Port;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Status;
        private System.Windows.Forms.BindingSource nodeMgrBindingSource;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridView dvPLCServer;
        private System.Windows.Forms.Button btnConAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_No;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLC_Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLC_IP;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLC_Port;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLC_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn PLC_RefRate;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button button5;
    }
}