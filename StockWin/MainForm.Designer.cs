﻿namespace StockWin
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
            this.label1 = new System.Windows.Forms.Label();
            this.lstSites = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.chkStartCatch = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCatchHour = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labTaskNum = new System.Windows.Forms.Label();
            this.txtKeyWords = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
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
            this.splitContainer1.Panel1.Controls.Add(this.chkStartCatch);
            this.splitContainer1.Panel1.Controls.Add(this.btnExport);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.lstSites);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "网站：";
            // 
            // lstSites
            // 
            this.lstSites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstSites.FormattingEnabled = true;
            this.lstSites.Location = new System.Drawing.Point(60, 57);
            this.lstSites.Name = "lstSites";
            this.lstSites.Size = new System.Drawing.Size(121, 20);
            this.lstSites.TabIndex = 0;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(187, 55);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(195, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "根据下面的关键词导出Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // chkStartCatch
            // 
            this.chkStartCatch.AutoSize = true;
            this.chkStartCatch.Location = new System.Drawing.Point(15, 12);
            this.chkStartCatch.Name = "chkStartCatch";
            this.chkStartCatch.Size = new System.Drawing.Size(120, 16);
            this.chkStartCatch.TabIndex = 3;
            this.chkStartCatch.Text = "开启循环增量抓取";
            this.chkStartCatch.UseVisualStyleBackColor = true;
            this.chkStartCatch.CheckedChanged += new System.EventHandler(this.chkStartCatch_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "增量抓取时间间隔：";
            // 
            // txtCatchHour
            // 
            this.txtCatchHour.Location = new System.Drawing.Point(121, 32);
            this.txtCatchHour.Name = "txtCatchHour";
            this.txtCatchHour.Size = new System.Drawing.Size(34, 21);
            this.txtCatchHour.TabIndex = 4;
            this.txtCatchHour.Text = "24";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(161, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "小时";
            // 
            // labTaskNum
            // 
            this.labTaskNum.AutoSize = true;
            this.labTaskNum.ForeColor = System.Drawing.Color.Red;
            this.labTaskNum.Location = new System.Drawing.Point(141, 12);
            this.labTaskNum.Name = "labTaskNum";
            this.labTaskNum.Size = new System.Drawing.Size(0, 12);
            this.labTaskNum.TabIndex = 1;
            // 
            // txtKeyWords
            // 
            this.txtKeyWords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyWords.Location = new System.Drawing.Point(0, 0);
            this.txtKeyWords.Multiline = true;
            this.txtKeyWords.Name = "txtKeyWords";
            this.txtKeyWords.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtKeyWords.Size = new System.Drawing.Size(730, 265);
            this.txtKeyWords.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(509, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 36);
            this.label5.TabIndex = 1;
            this.label5.Text = "- 多个关键词以空格或换行分隔；\r\n- 根据关键词对公司名称进行筛选；\r\n- 关键词为空，表示不筛选，全部导出";
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
            this.splitContainer2.Panel1.Controls.Add(this.txtKeyWords);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labTime);
            this.splitContainer2.Size = new System.Drawing.Size(730, 292);
            this.splitContainer2.SplitterDistance = 265;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 1;
            this.splitContainer2.TabStop = false;
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCatchHour;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labTaskNum;
        private System.Windows.Forms.TextBox txtKeyWords;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Timer timer1;
    }
}
