namespace QRPDaemon.Control
{
    partial class ucFile
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpTop = new System.Windows.Forms.TableLayoutPanel();
            this.lbInterval = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMeasureName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOriginFilePath = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.stStrip = new System.Windows.Forms.StatusStrip();
            this.tsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tlpTop.SuspendLayout();
            this.stStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpTop
            // 
            this.tlpTop.ColumnCount = 4;
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpTop.Controls.Add(this.lbInterval, 0, 0);
            this.tlpTop.Controls.Add(this.label2, 0, 1);
            this.tlpTop.Controls.Add(this.txtMeasureName, 1, 1);
            this.tlpTop.Controls.Add(this.label3, 0, 2);
            this.tlpTop.Controls.Add(this.txtOriginFilePath, 1, 2);
            this.tlpTop.Controls.Add(this.btnStart, 2, 0);
            this.tlpTop.Controls.Add(this.btnStop, 3, 0);
            this.tlpTop.Controls.Add(this.txtInterval, 1, 0);
            this.tlpTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpTop.Location = new System.Drawing.Point(0, 0);
            this.tlpTop.Name = "tlpTop";
            this.tlpTop.RowCount = 3;
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpTop.Size = new System.Drawing.Size(300, 80);
            this.tlpTop.TabIndex = 0;
            // 
            // lbInterval
            // 
            this.lbInterval.AutoSize = true;
            this.lbInterval.BackColor = System.Drawing.Color.Red;
            this.lbInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInterval.ForeColor = System.Drawing.Color.White;
            this.lbInterval.Location = new System.Drawing.Point(3, 0);
            this.lbInterval.Name = "lbInterval";
            this.lbInterval.Size = new System.Drawing.Size(54, 26);
            this.lbInterval.TabIndex = 0;
            this.lbInterval.Text = "주기(분)";
            this.lbInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "실험기기";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMeasureName
            // 
            this.tlpTop.SetColumnSpan(this.txtMeasureName, 3);
            this.txtMeasureName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMeasureName.Location = new System.Drawing.Point(63, 29);
            this.txtMeasureName.Multiline = true;
            this.txtMeasureName.Name = "txtMeasureName";
            this.txtMeasureName.ReadOnly = true;
            this.txtMeasureName.Size = new System.Drawing.Size(234, 20);
            this.txtMeasureName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 28);
            this.label3.TabIndex = 4;
            this.label3.Text = "파일위치";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtOriginFilePath
            // 
            this.tlpTop.SetColumnSpan(this.txtOriginFilePath, 3);
            this.txtOriginFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOriginFilePath.Location = new System.Drawing.Point(63, 55);
            this.txtOriginFilePath.Multiline = true;
            this.txtOriginFilePath.Name = "txtOriginFilePath";
            this.txtOriginFilePath.ReadOnly = true;
            this.txtOriginFilePath.Size = new System.Drawing.Size(234, 22);
            this.txtOriginFilePath.TabIndex = 5;
            // 
            // btnStart
            // 
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStart.Location = new System.Drawing.Point(153, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(69, 20);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "시작";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Visible = false;
            // 
            // btnStop
            // 
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStop.Location = new System.Drawing.Point(228, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(69, 20);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "중지";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            // 
            // txtInterval
            // 
            this.txtInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInterval.Location = new System.Drawing.Point(63, 3);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(84, 21);
            this.txtInterval.TabIndex = 8;
            // 
            // stStrip
            // 
            this.stStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatusLabel});
            this.stStrip.Location = new System.Drawing.Point(0, 278);
            this.stStrip.Name = "stStrip";
            this.stStrip.Size = new System.Drawing.Size(300, 22);
            this.stStrip.TabIndex = 10;
            this.stStrip.Text = "statusStrip1";
            // 
            // tsStatusLabel
            // 
            this.tsStatusLabel.Name = "tsStatusLabel";
            this.tsStatusLabel.Size = new System.Drawing.Size(121, 17);
            this.tsStatusLabel.Text = "toolStripStatusLabel1";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 80);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(300, 198);
            this.txtLog.TabIndex = 11;
            // 
            // ucFile
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.stStrip);
            this.Controls.Add(this.tlpTop);
            this.Name = "ucFile";
            this.Size = new System.Drawing.Size(300, 300);
            this.tlpTop.ResumeLayout(false);
            this.tlpTop.PerformLayout();
            this.stStrip.ResumeLayout(false);
            this.stStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpTop;
        private System.Windows.Forms.StatusStrip stStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        private System.Windows.Forms.Label lbInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMeasureName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOriginFilePath;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.TextBox txtLog;
    }
}
