namespace StockWin
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labTaskNum = new System.Windows.Forms.Label();
            this.txtCatchHour = new System.Windows.Forms.TextBox();
            this.chkIncr = new System.Windows.Forms.CheckBox();
            this.chkStartCatch = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.chkTitle = new System.Windows.Forms.CheckBox();
            this.chkSingle = new System.Windows.Forms.CheckBox();
            this.txtKeyWords = new System.Windows.Forms.TextBox();
            this.lstSites = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.labTaskNum);
            this.splitContainer1.Panel1.Controls.Add(this.txtCatchHour);
            this.splitContainer1.Panel1.Controls.Add(this.chkIncr);
            this.splitContainer1.Panel1.Controls.Add(this.chkStartCatch);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(730, 375);
            this.splitContainer1.SplitterDistance = 82;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // labTaskNum
            // 
            this.labTaskNum.AutoSize = true;
            this.labTaskNum.ForeColor = System.Drawing.Color.Red;
            this.labTaskNum.Location = new System.Drawing.Point(13, 40);
            this.labTaskNum.Name = "labTaskNum";
            this.labTaskNum.Size = new System.Drawing.Size(23, 12);
            this.labTaskNum.TabIndex = 1;
            this.labTaskNum.Text = "000";
            // 
            // txtCatchHour
            // 
            this.txtCatchHour.Location = new System.Drawing.Point(262, 10);
            this.txtCatchHour.Name = "txtCatchHour";
            this.txtCatchHour.Size = new System.Drawing.Size(34, 21);
            this.txtCatchHour.TabIndex = 4;
            this.txtCatchHour.Text = "24";
            // 
            // chkIncr
            // 
            this.chkIncr.AutoSize = true;
            this.chkIncr.Checked = true;
            this.chkIncr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncr.Location = new System.Drawing.Point(136, 12);
            this.chkIncr.Name = "chkIncr";
            this.chkIncr.Size = new System.Drawing.Size(132, 16);
            this.chkIncr.TabIndex = 3;
            this.chkIncr.Text = "增量抓取时间间隔：";
            this.chkIncr.UseVisualStyleBackColor = true;
            // 
            // chkStartCatch
            // 
            this.chkStartCatch.AutoSize = true;
            this.chkStartCatch.Location = new System.Drawing.Point(15, 12);
            this.chkStartCatch.Name = "chkStartCatch";
            this.chkStartCatch.Size = new System.Drawing.Size(72, 16);
            this.chkStartCatch.TabIndex = 3;
            this.chkStartCatch.Text = "开启抓取";
            this.chkStartCatch.UseVisualStyleBackColor = true;
            this.chkStartCatch.CheckedChanged += new System.EventHandler(this.chkStartCatch_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(302, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "小时";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.chkTitle);
            this.splitContainer2.Panel1.Controls.Add(this.chkSingle);
            this.splitContainer2.Panel1.Controls.Add(this.txtKeyWords);
            this.splitContainer2.Panel1.Controls.Add(this.lstSites);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.btnExport);
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labTime);
            this.splitContainer2.Size = new System.Drawing.Size(730, 292);
            this.splitContainer2.SplitterDistance = 263;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 1;
            this.splitContainer2.TabStop = false;
            // 
            // chkTitle
            // 
            this.chkTitle.AutoSize = true;
            this.chkTitle.Checked = true;
            this.chkTitle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTitle.Location = new System.Drawing.Point(12, 51);
            this.chkTitle.Name = "chkTitle";
            this.chkTitle.Size = new System.Drawing.Size(108, 16);
            this.chkTitle.TabIndex = 3;
            this.chkTitle.Text = "导出文件带标题";
            this.chkTitle.UseVisualStyleBackColor = true;
            // 
            // chkSingle
            // 
            this.chkSingle.AutoSize = true;
            this.chkSingle.Checked = true;
            this.chkSingle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSingle.Location = new System.Drawing.Point(141, 51);
            this.chkSingle.Name = "chkSingle";
            this.chkSingle.Size = new System.Drawing.Size(126, 16);
            this.chkSingle.TabIndex = 3;
            this.chkSingle.Text = "导出单个Excel文件";
            this.chkSingle.UseVisualStyleBackColor = true;
            // 
            // txtKeyWords
            // 
            this.txtKeyWords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyWords.Location = new System.Drawing.Point(276, 0);
            this.txtKeyWords.Margin = new System.Windows.Forms.Padding(13);
            this.txtKeyWords.Multiline = true;
            this.txtKeyWords.Name = "txtKeyWords";
            this.txtKeyWords.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtKeyWords.Size = new System.Drawing.Size(454, 265);
            this.txtKeyWords.TabIndex = 0;
            this.txtKeyWords.Text = "厦门\r\n福州\r\n南平\r\n三明\r\n莆田\r\n泉州\r\n漳州\r\n龙岩\r\n宁德\r\n长乐\r\n石狮\r\n晋江\r\n福建";
            // 
            // lstSites
            // 
            this.lstSites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstSites.FormattingEnabled = true;
            this.lstSites.Location = new System.Drawing.Point(54, 12);
            this.lstSites.Name = "lstSites";
            this.lstSites.Size = new System.Drawing.Size(121, 20);
            this.lstSites.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "网站：";
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExport.Location = new System.Drawing.Point(54, 73);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(170, 56);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "根据右边的关键词\r\n\r\n导出Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 60);
            this.label5.TabIndex = 1;
            this.label5.Text = "- 多个关键词以空格或换行分隔；\r\n\r\n- 根据关键词对公司名称进行筛选；\r\n\r\n- 关键词为空，表示不筛选，全部导出";
            // 
            // labTime
            // 
            this.labTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labTime.AutoSize = true;
            this.labTime.ForeColor = System.Drawing.Color.Red;
            this.labTime.Location = new System.Drawing.Point(6, 7);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(0, 12);
            this.labTime.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 375);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "融资事件";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstSites;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox chkStartCatch;
        private System.Windows.Forms.TextBox txtCatchHour;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labTaskNum;
        private System.Windows.Forms.TextBox txtKeyWords;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chkSingle;
        private System.Windows.Forms.CheckBox chkTitle;
        private System.Windows.Forms.CheckBox chkIncr;
    }
}

