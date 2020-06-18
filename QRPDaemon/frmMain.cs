using log4net;
using QRPDaemon.COM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRPDaemon
{
    public partial class frmMain : Form
    {
        readonly ILog logger = LogManager.GetLogger(typeof(frmMain));

        #region Var
        List<QRPDaemon.Control.ucFile> fSubList;
        #endregion Var

        public frmMain()
        {
            InitializeComponent();

            this.Load += FrmMain_Load;
            this.FormClosing += FrmMain_FormClosing;
            this.Resize += FrmMain_Resize;

            btnApply.Click += BtnApply_Click;
            btnAddRow.Click += BtnAddRow_Click;
            btnDeleteRow.Click += BtnDeleteRow_Click;
            btnAllStart.Click += BtnAllStart_Click;
            btnAllStop.Click += BtnAllStop_Click;

            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            dgvENVList.KeyPress += DgvENVList_KeyPress;
            dgvENVList.EditingControlShowing += DgvENVList_EditingControlShowing;
            dgvENVList.CellDoubleClick += DgvENVList_CellDoubleClick;

            //System.IO.FileInfo fi = new System.IO.FileInfo(@"XML\\log4net.xml");
            //log4net.Config.XmlConfigurator.Configure(fi);

            //btnLog.Click += BtnLog_Click;
            //TextBoxAppender.SetupTextBoxAppend(txtLog, "%date{HH:mm:ss,fff} %-5level %-33logger - %message%newline");
        }

        private void BtnLog_Click(object sender, EventArgs e)
        {
            logger.Info("TEST");
        }

        #region Form Events
        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitControl();
            InitConfig();

            //logger.Info("Load Complete");
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.Equals(CloseReason.UserClosing))
            {
                if (fSubList != null)
                {
                    int intCnt = fSubList.Count;
                    for (int i = 0; i < intCnt; i++)
                    {
                        if (fSubList[i] != null)
                        {
                            if (fSubList[i].ProgFlag)
                            {
                                if (MessageBox.Show("SMIS Interface가 실행중입니다. 종료하시겠습니까?", "확인창", MessageBoxButtons.YesNo).Equals(DialogResult.No))
                                {
                                    e.Cancel = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                this.Hide();
                this.trayIcon.Visible = false;
                e.Cancel = true;
                Application.Exit();
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized.Equals(WindowState))
            {
                ShowTray();
            }
        }
        #endregion Form Events

        #region UI Event
        private void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                this.tlPanel.Controls.Clear();

                int intCount = dgvENVList.Rows.Count;

                #region 파일생성

                fSubList = new List<Control.ucFile>();
                for (int i = 0; i < intCount; i++)
                {
                    if (dgvENVList.Rows[i].Cells["Apply"].Value.ToBool())
                    {
                        #region 입력체크
                        if (dgvENVList.Rows[i].Cells["PlantCode"].Value.ToString().Equals(string.Empty))
                        {
                            MessageBox.Show((i + 1).ToString() + "번째 행에 공장코드를 입력하세요.", "경고창", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvENVList.CurrentCell = this.dgvENVList["PlantCode", i];
                            dgvENVList.BeginEdit(true);
                            tlPanel.Controls.Clear();
                            return;
                        }
                        else if (dgvENVList.Rows[i].Cells["MeasureName"].Value.ToString().Equals(string.Empty))
                        {
                            MessageBox.Show((i + 1).ToString() + "번째 행에 실험기기를 입력하세요.", "경고창", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvENVList.CurrentCell = this.dgvENVList["MeasureName", i];
                            dgvENVList.BeginEdit(true);
                            tlPanel.Controls.Clear();
                            return;
                        }
                        else if (dgvENVList.Rows[i].Cells["OriginFilePath"].Value.ToString().Equals(string.Empty))
                        {
                            MessageBox.Show((i + 1).ToString() + "번째 행에 원본파일경로를 입력하세요.", "경고창", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvENVList.CurrentCell = this.dgvENVList["OriginFilePath", i];
                            dgvENVList.BeginEdit(true);
                            tlPanel.Controls.Clear();
                            return;
                        }
                        else if (dgvENVList.Rows[i].Cells["BackupFilePath"].Value.ToString().Equals(string.Empty))
                        {
                            MessageBox.Show((i + 1).ToString() + "번째 행에 파일백업경로를 입력하세요.", "경고창", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvENVList.CurrentCell = this.dgvENVList["BackupFilePath", i];
                            dgvENVList.BeginEdit(true);
                            tlPanel.Controls.Clear();
                            return;
                        }
                        else if (dgvENVList.Rows[i].Cells["Interval"].Value.ToString().Equals("0") ||
                            dgvENVList.Rows[i].Cells["Interval"].Value.ToString().Equals(string.Empty))
                        {
                            MessageBox.Show((i + 1).ToString() + "번째 행에 IF주기(분)를 입력하세요.", "경고창", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgvENVList.CurrentCell = this.dgvENVList["Interval", i];
                            dgvENVList.BeginEdit(true);
                            tlPanel.Controls.Clear();
                            return;
                        }
                        #endregion

                        QRPDaemon.Control.ucFile file = new QRPDaemon.Control.ucFile();
                        file.PlantCode = dgvENVList.Rows[i].Cells["PlantCode"].Value.ToString();
                        file.ProcessGroupCode = dgvENVList.Rows[i].Cells["ProcessGroupCode"].Value.ToString();
                        file.InspectTypeCode = dgvENVList.Rows[i].Cells["InspectTypeCode"].Value.ToString();
                        file.MeasureName = dgvENVList.Rows[i].Cells["MeasureName"].Value.ToString();
                        file.OriginFilePath = dgvENVList.Rows[i].Cells["OriginFilePath"].Value.ToString();
                        file.BackupFilePath = dgvENVList.Rows[i].Cells["BackupFilePath"].Value.ToString();
                        file.Intervar = dgvENVList.Rows[i].Cells["Interval"].Value.ToInt();
                        file.RowIndex = dgvENVList.Rows[i].Cells["RowIndex"].Value.ToInt();
                        file.FileExtension = dgvENVList.Rows[i].Cells["FileExtension"].Value.ToString();
                        file.Dock = DockStyle.Fill;
                        file.Show();

                        tlPanel.Controls.Add(file, i % 3, i / 3);
                        fSubList.Add(file);
                    }
                }

                #endregion

                mfCreateEnvXML();
                tabMain.SelectedTab = this.tabMain.TabPages["DETAIL"];
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnAddRow_Click(object sender, EventArgs e)
        {
            if (dgvENVList.DataSource == null)
                return;

            ((DataTable)dgvENVList.DataSource).Rows.Add(((DataTable)dgvENVList.DataSource).NewRow());
        }

        private void BtnDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                //for (int i = 0; i < dgvENVList.Rows.Count; i++)
                //{
                //    if (dgvENVList.Rows[i].Cells["Apply"].Value.ToBool())
                //    {
                //        dgvENVList.Rows[i].Cells["Apply"].Value = false;
                //        dgvENVList.Rows[i].Cells["PlantCode"].Value = string.Empty;
                //        dgvENVList.Rows[i].Cells["MeasureName"].Value = string.Empty;
                //        dgvENVList.Rows[i].Cells["OriginFilePath"].Value = string.Empty;
                //        dgvENVList.Rows[i].Cells["BackupFilePath"].Value = string.Empty;
                //        dgvENVList.Rows[i].Cells["Interval"].Value = 0;
                //    }
                //}

                foreach (DataGridViewRow row in dgvENVList.SelectedRows)
                {
                    dgvENVList.Rows.Remove(row);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnAllStart_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var sub in fSubList)
                {
                    sub.mfStop();
                    sub.mfStart();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                btnAllStart.Enabled = false;
                btnAllStop.Enabled = true;
            }
        }

        private void BtnAllStop_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var sub in fSubList)
                {
                    sub.mfStop();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                btnAllStart.Enabled = true;
                btnAllStop.Enabled = false;
            }
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            HideTray();
        }

        private void DgvENVList_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(char.IsDigit(e.KeyChar)) && e.KeyChar != Convert.ToChar(Keys.Back) && e.KeyChar != Convert.ToChar(Keys.Delete))
                    e.Handled = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DgvENVList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgvENVList.CurrentCell == null)
                    return;
                else if (dgvENVList.CurrentCell.OwningColumn.Name.Equals("Interval"))
                    e.Control.KeyPress += new KeyPressEventHandler(DgvENVList_KeyPress);
                else
                    e.Control.KeyPress -= new KeyPressEventHandler(DgvENVList_KeyPress);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DgvENVList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvENVList.CurrentCell == null)
                    return;
                else if (dgvENVList.CurrentCell.OwningColumn.Name.Equals("OriginFilePath") || dgvENVList.CurrentCell.OwningColumn.Name.Equals("BackupFilePath"))
                {
                    using (FolderBrowserDialog file = new FolderBrowserDialog())
                    {
                        file.ShowNewFolderButton = false;
                        if (file.ShowDialog().Equals(DialogResult.OK))
                        {
                            dgvENVList.CurrentCell.Value = file.SelectedPath;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion UI Event

        #region Method

        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        private void InitControl()
        {
            try
            {
                //메인 폼 아이콘 적용
                this.Icon = Properties.Resources.servicerunning;

                //Tray 아이콘 적용
                trayIcon.Text = this.Text;
                trayIcon.Icon = Properties.Resources.servicerunning;
                trayIcon.Visible = false;

                #region DataGridView 초기화

                DataTable dtEnvList = new DataTable("ENVLIST");
                dtEnvList.Columns.AddRange(new DataColumn[] {
                    new DataColumn{ ColumnName = "Apply", Caption = "적용", DefaultValue = true, DataType = typeof(bool) }
                    , new DataColumn{ ColumnName = "PlantCode", Caption = "Site", DefaultValue = string.Empty, DataType = typeof(string) }
                    , new DataColumn{ ColumnName = "ProcessGroupCode", Caption = "Plant", DefaultValue = string.Empty, DataType = typeof(string) }
                    , new DataColumn{ ColumnName = "InspectTypeCode", Caption = "검사유형", DefaultValue = string.Empty, DataType = typeof(string) }
                    , new DataColumn{ ColumnName = "MeasureName", Caption = "실험기기", DefaultValue = string.Empty, DataType = typeof(string) }
                    , new DataColumn{ ColumnName = "OriginFilePath", Caption = "원본경로", DefaultValue = string.Empty, DataType = typeof(string) }
                    , new DataColumn{ ColumnName = "BackupFilePath", Caption = "복사경로", DefaultValue = string.Empty, DataType = typeof(string) }
                    , new DataColumn{ ColumnName = "Interval", Caption = "IF주기(분)", DefaultValue = 5, DataType = typeof(Int32) }
                    , new DataColumn{ ColumnName = "RowIndex", Caption = "측정데이터시작행", DefaultValue = 5, DataType = typeof(Int32) }
                    , new DataColumn{ ColumnName = "FileExtension", Caption = "파일확장자", DefaultValue = string.Empty, DataType = typeof(string) }
                });

                dgvENVList.DataSource = dtEnvList;
                ((DataGridViewCheckBoxColumn)dgvENVList.Columns["Apply"]).HeaderText = "적용";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["PlantCode"]).MaxInputLength = 10;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["PlantCode"]).HeaderText = "Site";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["ProcessGroupCode"]).MaxInputLength = 10;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["ProcessGroupCode"]).HeaderText = "Plant";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["InspectTypeCode"]).MaxInputLength = 10;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["InspectTypeCode"]).HeaderText = "검사유형";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["MeasureName"]).MaxInputLength = 40;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["MeasureName"]).HeaderText = "실험기기";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["OriginFilePath"]).MaxInputLength = 500;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["OriginFilePath"]).HeaderText = "원본경로";
                //((DataGridViewTextBoxColumn)dgvENVList.Columns["OriginFilePath"]).ReadOnly = true;

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["BackupFilePath"]).MaxInputLength = 500;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["BackupFilePath"]).HeaderText = "복사경로";
                //((DataGridViewTextBoxColumn)dgvENVList.Columns["BackupFilePath"]).ReadOnly = true;

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["Interval"]).MaxInputLength = 2;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["Interval"]).HeaderText = "IF주기(분)";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["RowIndex"]).MaxInputLength = 2;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["RowIndex"]).HeaderText = "측정데이터시작행";

                ((DataGridViewTextBoxColumn)dgvENVList.Columns["FileExtension"]).MaxInputLength = 10;
                ((DataGridViewTextBoxColumn)dgvENVList.Columns["FileExtension"]).HeaderText = "파일확장자";

                dgvENVList.AllowUserToAddRows = false;
                dgvENVList.AllowUserToDeleteRows = false;
                #endregion

                btnAllStop.Enabled = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// XML 화일에 적용한 H/W Interface 환경설정 정보를 그리드에 보여줌.
        /// </summary>
        private void InitConfig()
        {
            try
            {
                if (System.IO.File.Exists(CommonCode.EnvXMLFileName))
                {
                    //this.dgvENVList.Rows.Clear();

                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.Load(CommonCode.EnvXMLFileName);

                    System.Xml.XmlNode xmlNodeEnvList = xmlDocument.SelectSingleNode("EnvSettings");
                    DataRow dr;
                    for (int i = 0; i < xmlNodeEnvList.ChildNodes.Count; i++)
                    {
                        dr = ((DataTable)dgvENVList.DataSource).NewRow();
                        dr["Apply"] = Convert.ToBoolean(xmlNodeEnvList.ChildNodes[i].SelectSingleNode("Apply").InnerText);
                        dr["PlantCode"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("PlantCode").InnerText;
                        dr["ProcessGroupCode"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("ProcessGroupCode").InnerText;
                        dr["MeasureName"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("MeasureName").InnerText;
                        dr["OriginFilePath"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("OriginFilePath").InnerText;
                        dr["BackupFilePath"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("BackupFilePath").InnerText;
                        dr["RowIndex"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("RowIndex").InnerText;
                        dr["Interval"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("Interval").InnerText;
                        dr["FileExtension"] = xmlNodeEnvList.ChildNodes[i].SelectSingleNode("FileExtension").InnerText;
                        ((DataTable)dgvENVList.DataSource).Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        // Tray 활성
        private void ShowTray()
        {
            try
            {
                trayIcon.Visible = true;
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        // Tray 비활성
        private void HideTray()
        {
            try
            {
                trayIcon.Visible = false;
                this.ShowInTaskbar = true;
                this.Visible = true;
                this.Show();
                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 환경설정 XML 파일생성
        /// </summary>
        private void mfCreateEnvXML()
        {
            try
            {
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "yes"));

                string strRootNodeName = "EnvSettings";
                string strChildRootNodeName = "EnvSetting";

                // Root Node
                System.Xml.XmlNode rootNode = xmlDocument.CreateNode(System.Xml.XmlNodeType.Element, strRootNodeName, string.Empty);
                xmlDocument.AppendChild(rootNode);

                for (int i = 0; i < dgvENVList.Rows.Count; i++)
                {
                    // Child Root Node
                    System.Xml.XmlNode childNode = xmlDocument.CreateNode(System.Xml.XmlNodeType.Element, strChildRootNodeName, string.Empty);
                    rootNode.AppendChild(childNode);

                    // Child element: Apply
                    System.Xml.XmlElement Apply = xmlDocument.CreateElement("Apply");
                    Apply.InnerText = this.dgvENVList.Rows[i].Cells["Apply"].Value.ToString();
                    childNode.AppendChild(Apply);

                    // Child element: PlantCode
                    System.Xml.XmlElement PlantCode = xmlDocument.CreateElement("PlantCode");
                    PlantCode.InnerText = dgvENVList.Rows[i].Cells["PlantCode"].Value.ToString();
                    childNode.AppendChild(PlantCode);

                    // Child element: ProcessGroupCode
                    System.Xml.XmlElement ProcessGroupCode = xmlDocument.CreateElement("ProcessGroupCode");
                    ProcessGroupCode.InnerText = dgvENVList.Rows[i].Cells["ProcessGroupCode"].Value.ToString();
                    childNode.AppendChild(ProcessGroupCode);

                    // Child element: InspectTypeCode
                    System.Xml.XmlElement InspectTypeCode = xmlDocument.CreateElement("InspectTypeCode");
                    InspectTypeCode.InnerText = dgvENVList.Rows[i].Cells["InspectTypeCode"].Value.ToString();
                    childNode.AppendChild(InspectTypeCode);

                    // Child element: OriginFilePath
                    System.Xml.XmlElement OriginFilePath = xmlDocument.CreateElement("OriginFilePath");
                    OriginFilePath.InnerText = dgvENVList.Rows[i].Cells["OriginFilePath"].Value.ToString();
                    childNode.AppendChild(OriginFilePath);

                    // Child element: BackupFilePath
                    System.Xml.XmlElement BackupFilePath = xmlDocument.CreateElement("BackupFilePath");
                    BackupFilePath.InnerText = dgvENVList.Rows[i].Cells["BackupFilePath"].Value.ToString();
                    childNode.AppendChild(BackupFilePath);

                    // Child element: Interval
                    System.Xml.XmlElement Interval = xmlDocument.CreateElement("Interval");
                    Interval.InnerText = dgvENVList.Rows[i].Cells["Interval"].Value.ToString();
                    childNode.AppendChild(Interval);

                    // Child element: MeasureName
                    System.Xml.XmlElement MeasureName = xmlDocument.CreateElement("MeasureName");
                    MeasureName.InnerText = dgvENVList.Rows[i].Cells["MeasureName"].Value.ToString();
                    childNode.AppendChild(MeasureName);

                    // Child element: RowIndex
                    System.Xml.XmlElement RowIndex = xmlDocument.CreateElement("RowIndex");
                    RowIndex.InnerText = dgvENVList.Rows[i].Cells["RowIndex"].Value.ToString();
                    childNode.AppendChild(RowIndex);

                    // Child element: FileExtension
                    System.Xml.XmlElement FileExtension = xmlDocument.CreateElement("FileExtension");
                    FileExtension.InnerText = dgvENVList.Rows[i].Cells["FileExtension"].Value.ToString();
                    childNode.AppendChild(FileExtension);
                }

                xmlDocument.Save(CommonCode.EnvXMLFileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
    }
}
