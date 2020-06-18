namespace QRPDaemon
{
    partial class frmMain
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
            this.tabMain = new System.Windows.Forms.TabControl();
            this.ENV = new System.Windows.Forms.TabPage();
            this.dgvENVList = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.DETAIL = new System.Windows.Forms.TabPage();
            this.tlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAllStart = new System.Windows.Forms.Button();
            this.btnAllStop = new System.Windows.Forms.Button();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TEST = new System.Windows.Forms.TabPage();
            this.tlpTop = new System.Windows.Forms.TableLayoutPanel();
            this.btnLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabMain.SuspendLayout();
            this.ENV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvENVList)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.DETAIL.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.TEST.SuspendLayout();
            this.tlpTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.ENV);
            this.tabMain.Controls.Add(this.DETAIL);
            this.tabMain.Controls.Add(this.TEST);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1184, 761);
            this.tabMain.TabIndex = 0;
            // 
            // ENV
            // 
            this.ENV.Controls.Add(this.dgvENVList);
            this.ENV.Controls.Add(this.tableLayoutPanel1);
            this.ENV.Location = new System.Drawing.Point(4, 22);
            this.ENV.Name = "ENV";
            this.ENV.Padding = new System.Windows.Forms.Padding(3);
            this.ENV.Size = new System.Drawing.Size(1176, 735);
            this.ENV.TabIndex = 0;
            this.ENV.Text = "환경설정";
            this.ENV.UseVisualStyleBackColor = true;
            // 
            // dgvENVList
            // 
            this.dgvENVList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvENVList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvENVList.Location = new System.Drawing.Point(3, 33);
            this.dgvENVList.Name = "dgvENVList";
            this.dgvENVList.RowTemplate.Height = 23;
            this.dgvENVList.Size = new System.Drawing.Size(1170, 699);
            this.dgvENVList.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnApply, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDeleteRow, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAddRow, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1170, 30);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnApply.Location = new System.Drawing.Point(3, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(114, 24);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDeleteRow.Location = new System.Drawing.Point(243, 3);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(114, 24);
            this.btnDeleteRow.TabIndex = 1;
            this.btnDeleteRow.Text = "행삭제";
            this.btnDeleteRow.UseVisualStyleBackColor = true;
            // 
            // btnAddRow
            // 
            this.btnAddRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddRow.Location = new System.Drawing.Point(123, 3);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(114, 24);
            this.btnAddRow.TabIndex = 2;
            this.btnAddRow.Text = "행추가";
            this.btnAddRow.UseVisualStyleBackColor = true;
            // 
            // DETAIL
            // 
            this.DETAIL.Controls.Add(this.tlPanel);
            this.DETAIL.Controls.Add(this.tableLayoutPanel2);
            this.DETAIL.Location = new System.Drawing.Point(4, 22);
            this.DETAIL.Name = "DETAIL";
            this.DETAIL.Padding = new System.Windows.Forms.Padding(3);
            this.DETAIL.Size = new System.Drawing.Size(1176, 735);
            this.DETAIL.TabIndex = 1;
            this.DETAIL.Text = "상세내용";
            this.DETAIL.UseVisualStyleBackColor = true;
            // 
            // tlPanel
            // 
            this.tlPanel.ColumnCount = 3;
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanel.Location = new System.Drawing.Point(3, 33);
            this.tlPanel.Name = "tlPanel";
            this.tlPanel.RowCount = 2;
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanel.Size = new System.Drawing.Size(1170, 699);
            this.tlPanel.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnAllStart, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnAllStop, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1170, 30);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // btnAllStart
            // 
            this.btnAllStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAllStart.Location = new System.Drawing.Point(3, 3);
            this.btnAllStart.Name = "btnAllStart";
            this.btnAllStart.Size = new System.Drawing.Size(114, 24);
            this.btnAllStart.TabIndex = 0;
            this.btnAllStart.Text = "전체시작";
            this.btnAllStart.UseVisualStyleBackColor = true;
            // 
            // btnAllStop
            // 
            this.btnAllStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAllStop.Location = new System.Drawing.Point(123, 3);
            this.btnAllStop.Name = "btnAllStop";
            this.btnAllStop.Size = new System.Drawing.Size(114, 24);
            this.btnAllStop.TabIndex = 2;
            this.btnAllStop.Text = "전체중지";
            this.btnAllStop.UseVisualStyleBackColor = true;
            // 
            // trayIcon
            // 
            this.trayIcon.Text = "QRPDaemon";
            this.trayIcon.Visible = true;
            // 
            // TEST
            // 
            this.TEST.Controls.Add(this.txtLog);
            this.TEST.Controls.Add(this.tlpTop);
            this.TEST.Location = new System.Drawing.Point(4, 22);
            this.TEST.Name = "TEST";
            this.TEST.Size = new System.Drawing.Size(1176, 735);
            this.TEST.TabIndex = 2;
            this.TEST.Text = "테스트";
            this.TEST.UseVisualStyleBackColor = true;
            // 
            // tlpTop
            // 
            this.tlpTop.ColumnCount = 7;
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTop.Controls.Add(this.btnLog, 0, 0);
            this.tlpTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpTop.Location = new System.Drawing.Point(0, 0);
            this.tlpTop.Name = "tlpTop";
            this.tlpTop.RowCount = 2;
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTop.Size = new System.Drawing.Size(1176, 100);
            this.tlpTop.TabIndex = 0;
            // 
            // btnLog
            // 
            this.btnLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLog.Location = new System.Drawing.Point(3, 3);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(94, 44);
            this.btnLog.TabIndex = 0;
            this.btnLog.Text = "Log";
            this.btnLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 100);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(1176, 635);
            this.txtLog.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tabMain);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QRPDaemon";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabMain.ResumeLayout(false);
            this.ENV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvENVList)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.DETAIL.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.TEST.ResumeLayout(false);
            this.TEST.PerformLayout();
            this.tlpTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage ENV;
        private System.Windows.Forms.DataGridView dgvENVList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.TabPage DETAIL;
        private System.Windows.Forms.TableLayoutPanel tlPanel;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnAllStart;
        private System.Windows.Forms.Button btnAllStop;
        private System.Windows.Forms.TabPage TEST;
        private System.Windows.Forms.TableLayoutPanel tlpTop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnLog;
    }
}