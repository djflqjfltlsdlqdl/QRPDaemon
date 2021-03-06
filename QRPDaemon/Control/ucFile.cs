﻿using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using log4net;
using ExcelDataReader;
using QRPDaemon.COM;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using log4net.Layout;

namespace QRPDaemon.Control
{
    public partial class ucFile : UserControl
    {
        #region Var
        //readonly ILog Logger = LogManager.GetLogger(typeof(ucFile));
        public log4net.ILog m_logger = null;
        public ILog Logger { get => m_logger; set => m_logger = value; }
        private System.Windows.Forms.Timer m_timerMain;
        private System.Windows.Forms.Timer m_timerSub;

        private bool m_bolProgFlag = false;
        private string m_strPlantCode = string.Empty;
        private string m_strProcessGroupCode = string.Empty;
        private string m_strInspectTypeCode = string.Empty;
        private string m_strOriginFilePath = string.Empty;
        private string m_strBackupFilePath = string.Empty;
        private int m_intIntervar = 5;
        private string m_strMeasureName = string.Empty;
        private string m_strFileExtension = string.Empty;
        private int m_intRowIndex = 0;
        private DateTime m_dateLastSampleDate = DateTime.MinValue;
        /// <summary>
        /// 진행상태
        /// </summary>
        public bool ProgFlag
        {
            get { return m_bolProgFlag; }
            //set { m_bolProgFlag = value; }
        }
        /// <summary>
        /// Site
        /// </summary>
        public string PlantCode
        {
            //get { return m_strPlantCode; }
            set { m_strPlantCode = value; }
        }
        /// <summary>
        /// Plant
        /// </summary>
        public string ProcessGroupCode
        {
            //get { return m_strProcessGroupCode; }
            set { m_strProcessGroupCode = value; }
        }
        /// <summary>
        /// 유형
        /// </summary>
        public string InspectTypeCode
        {
            //get { return m_strInspectTypeCode; }
            set { m_strInspectTypeCode = value; }
        }
        /// <summary>
        /// 원본파일경로
        /// </summary>
        public string OriginFilePath
        {
            //get { return m_strOriginFilePath; }
            set { m_strOriginFilePath = value; }
        }
        /// <summary>
        /// 복사경로
        /// </summary>
        public string BackupFilePath
        {
            //get { return m_strBackupFilePath; }
            set { m_strBackupFilePath = value; }
        }
        /// <summary>
        /// 주기
        /// </summary>
        public int Intervar
        {
            //get { return m_strIntervar; }
            set { m_intIntervar = value; }
        }
        /// <summary>
        /// 실험기기명
        /// </summary>
        public string MeasureName
        {
            //get { return m_strMeasureName; }
            set { m_strMeasureName = value; }
        }
        /// <summary>
        /// 파일 확장자
        /// </summary>
        public string FileExtension
        {
            //get { return m_strFileExtension; }
            set { m_strFileExtension = value; }
        }
        /// <summary>
        /// 시작 행 Index
        /// </summary>
        public int RowIndex
        {
            //get { return m_intRowIndex; }
            set { m_intRowIndex = value; }
        }
        #endregion Var

        public ucFile()
        {
            InitializeComponent();

            this.Load += UcFile_Load;

            btnStart.Click += new EventHandler(btnStart_Click);
            btnStop.Click += new EventHandler(btnStop_Click);
            txtInterval.KeyDown += new KeyEventHandler(txtInterval_KeyDown);
        }

        private void UcFile_Load(object sender, EventArgs e)
        {
            InitControl();
            mfReadLogData();

            clsScheduledTimer st = new clsScheduledTimer();
            TimeSpan ts = new TimeSpan();
            switch (m_strPlantCode)
            {
                case "03":
                    ts = Properties.Settings.Default.StartTime_03;
                    break;
                case "05":
                    ts = Properties.Settings.Default.StartTime_05;
                    break;
            }
            //st.SetTime(ts, mfDeleteGrid);

            //log4net.GlobalContext.Properties["LogName"] = $"{m_strPlantCode}.{m_strMeasureName}";
            //System.IO.FileInfo fiLogInfo = new System.IO.FileInfo(@"XML\\log4net.xml");
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(fiLogInfo);
            //TextBoxAppender.SetupTextBoxAppend(this.txtLog, "[ %date ] [%thread] [ %-5level ] %Logger : \"%message\" - %exception%newline");
            
            m_logger = SetLog();
            //TextBoxAppender.SetupTextBoxAppend(txtLog, "[ %date ] [%thread] [ %-5level ] %Logger : \"%message\" - %exception%newline");

            //Logger.Info("Load Complete!");
        }

        #region Initialize

        private void InitControl()
        {
            tsStatusLabel.Text = string.Empty;
            btnStop.Enabled = false;

            // 주기
            txtInterval.Text = m_intIntervar.ToString();

            txtLog.ReadOnly = true;

            txtMeasureName.Text = m_strMeasureName;
            txtOriginFilePath.Text = m_strOriginFilePath;
        }

        #endregion Initialize

        #region UI Event
        // 시작버튼 클릭 이벤트
        void btnStart_Click(object sender, EventArgs e)
        {
            mfStart();
        }

        // 종료버튼 클릭 이벤트
        void btnStop_Click(object sender, EventArgs e)
        {
            mfStop();
        }

        // 주기 텍스트박스 키다운이벤트(숫자만 입력가능)
        void txtInterval_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //숫자,백스페이스,마이너스,소숫점 만 입력받는다.
                if (!(Char.IsDigit(Convert.ToChar(e.KeyValue)) && e.KeyCode != Keys.Back && e.KeyCode != Keys.Delete)) //8:백스페이스,45:마이너스,46:소수점
                {
                    e.Handled = true;
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        // Main Timer 이벤트
        void m_timerMain_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!m_bolProgFlag)
                {
                    m_bolProgFlag = true;
                    Task work = mfDoWork();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Sub Timer 이벤트
        void m_timerSub_Tick(object sender, EventArgs e)
        {
            try
            {
                if (lbInterval.BackColor == System.Drawing.Color.Red)
                    lbInterval.BackColor = System.Drawing.Color.DarkGreen;
                else
                    lbInterval.BackColor = System.Drawing.Color.Red;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion UI Event

        #region Method
        /// <summary>
        /// 작업
        /// </summary>
        private async Task mfDoWork()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (txtLog.Lines.Length > 3000)
                        txtLog.Clear();

                    if (m_bolProgFlag)
                    {
                        mfSetToolStripStatusLabel("Start...");
                        Logger.Info("-- Start -------------------------------------------------------------");
                        SharedDirectory sd = new SharedDirectory();
                        try
                        {
                            int intErrCode = sd.mfConnectNetworkDrive(m_strOriginFilePath, Properties.Settings.Default.SharedID, Properties.Settings.Default.SharedPW);
                            if (intErrCode.Equals(0) || intErrCode.Equals(1219))
                            {
                                Logger.Debug("네트워크 연결 성공!");
                                System.IO.DirectoryInfo diInfo = new System.IO.DirectoryInfo(m_strOriginFilePath);
                                if (diInfo.Exists)
                                {
                                    DateTime dateSampleDate = DateTime.Now;
                                    //System.IO.FileInfo[] getFiles = diInfo.GetFiles($"*{m_strFileExtension}");
                                    System.Collections.Generic.IList<FileInfo> getFiles = mfGetFiles(diInfo);
                                    foreach (System.IO.FileInfo fi in getFiles)
                                    {
                                        if (!m_bolProgFlag)
                                        {
                                            Logger.Info("작업 중지...");
                                            break;
                                        }
                                        try
                                        {
                                            if (fi.Extension.ToUpper().Equals(m_strFileExtension.ToUpper()))
                                            {
                                                Logger.Info($"Parsing Start : {fi.FullName}");

                                                if (m_strPlantCode.Equals("03"))
                                                {
                                                    switch (m_strMeasureName)
                                                    {
                                                        case "밀도계":
                                                            dateSampleDate = mfParsing_Density_03(fi.FullName);
                                                            break;
                                                        case "TOC":
                                                            dateSampleDate = mfParsing_TOC_03(fi.FullName);
                                                            break;
                                                        case "음이온":
                                                            dateSampleDate = mfParsing_Anion_03(fi.FullName);
                                                            break;
                                                        case "양이온":
                                                            dateSampleDate = mfParsing_Cation_03(fi.FullName);
                                                            break;
                                                    }
                                                }
                                                else if (m_strPlantCode.Equals("05"))
                                                {
                                                    switch (m_strMeasureName)
                                                    {
                                                        case "밀도계":
                                                            dateSampleDate = mfParsing_Density_05(fi.FullName);
                                                            break;
                                                        case "TOC":
                                                            dateSampleDate = mfParsing_TOC_05(fi.FullName);
                                                            break;
                                                        // 음이온
                                                        case "ICS2100":
                                                        case "ICS5000":
                                                        case "IC930":
                                                        case "음이온":
                                                            dateSampleDate = mfParsing_Anion_05(fi.FullName);
                                                            break;
                                                        case "ICP-MS(7900)":
                                                        case "ICP-MS":
                                                            dateSampleDate = mfParsing_Cation_05_7900(fi.FullName);
                                                            break;
                                                        //case "ICP-MS(A-M90)":
                                                        //    mfParsing_Cation_05_M90(fi.FullName);
                                                        //    break;
                                                        case "ICP-OES":
                                                            dateSampleDate = mfParsing_Cation_05_OES(fi.FullName);
                                                            break;
                                                    }
                                                }
                                                Logger.Info($"Parsing End : {fi.FullName}");
                                            }

                                            if (!dateSampleDate.Equals(DateTime.MaxValue))
                                            {
                                                DateTime dateFileDate = dateSampleDate;
                                                if(m_strPlantCode.Equals("03"))
                                                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                                                else if(m_strPlantCode.Equals("05"))
                                                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                                                string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateFileDate.ToString("yyyy-MM-dd"), m_strMeasureName);
                                                System.IO.DirectoryInfo diTarget = new System.IO.DirectoryInfo(strTargetPath);
                                                if (!diTarget.Exists)
                                                    diTarget.Create();

                                                string strTargetFileName = System.IO.Path.Combine(strTargetPath, fi.Name);
                                                if (System.IO.File.Exists(strTargetFileName))
                                                    strTargetFileName = strTargetFileName + DateTime.Now.ToString("_HHmmss");
                                                try
                                                {
                                                    //fi.MoveTo(System.IO.Path.Combine(strTargetPath, fi.Name));
                                                    fi.CopyTo(strTargetFileName, true);
                                                    fi.Delete();
                                                }
                                                catch
                                                {
                                                }

                                                Logger.Info($"File BackUp : {fi.FullName}");
                                                m_dateLastSampleDate = dateSampleDate;
                                            }
                                            else
                                            {
                                                string strTargetPath = string.Format(@"{0}\BADFILE\{1}", m_strBackupFilePath, m_strMeasureName);
                                                System.IO.DirectoryInfo diTarget = new System.IO.DirectoryInfo(strTargetPath);
                                                if (!diTarget.Exists)
                                                    diTarget.Create();

                                                try
                                                {
                                                    fi.CopyTo(System.IO.Path.Combine(strTargetPath, fi.Name), true);
                                                    fi.Delete();
                                                }
                                                catch
                                                {
                                                }

                                                Logger.Info($"File BackUp : {fi.FullName}");
                                            }
                                        }
                                        catch(System.Exception ex)
                                        {
                                            Logger.Error($"Parsing Error In : {fi.FullName}", ex);
                                        }
                                    }

                                    if ((m_strPlantCode.Equals("03") && m_strMeasureName.Equals("밀도계")) || (m_strPlantCode.Equals("05") && m_strMeasureName.Equals("밀도계")))
                                    {
                                        System.IO.FileInfo[] delFiles = diInfo.GetFiles($"*.md5");
                                        foreach (System.IO.FileInfo fi in delFiles)
                                        {
                                            fi.Delete();
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.Info("파일경로 에러!");
                                }

                                Logger.Info("----Complete----");
                                mfSetToolStripStatusLabel("Complete..." + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                Logger.Info($"Network Path Error {intErrCode} : {sd.mfGetConnectErrorMessage(intErrCode)}");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Error("try 2", ex);
                        }
                        finally
                        {
                            sd.mfDisconnectNetworkDrive(m_strOriginFilePath);
                            sd.mfDisconnectNetworkDrive(m_strBackupFilePath);
                            Logger.Debug("네트워크 연결 해제!");
                        }
                    }

                    Logger.Info("-- End -------------------------------------------------------------");
                }
                catch (System.Exception ex)
                {
                    Logger.Error("try 1", ex);
                }
                finally
                {
                    m_bolProgFlag = false;
                }
            });
        }
        /// <summary>
        /// 시작
        /// </summary>
        public void mfStart()
        {
            try
            {
                if (m_timerMain == null)
                    m_timerMain = new Timer();
                if (m_timerSub == null)
                    m_timerSub = new Timer();

                m_timerMain.Tick += new EventHandler(m_timerMain_Tick);
                m_timerSub.Tick += new EventHandler(m_timerSub_Tick);

                m_timerSub.Interval = 1000;

                int intInterval = txtInterval.Text.ToInt();
                m_timerMain.Interval = intInterval * 1000 * 60;

                m_timerMain.Start();
                m_timerSub.Start();

                tsStatusLabel.Text = "시작...";

                if (!m_bolProgFlag)
                {
                    m_bolProgFlag = true;
                    Task work = mfDoWork();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error("Error In mfStart", ex);
            }
            finally
            {
                txtInterval.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }
        /// <summary>
        /// 중지
        /// </summary>
        public void mfStop()
        {
            try
            {
                if (m_timerMain != null)
                {
                    m_timerMain.Tick -= new EventHandler(m_timerMain_Tick);
                    m_timerMain.Stop();
                }
                if (m_timerSub != null)
                {
                    m_timerSub.Tick -= new EventHandler(m_timerSub_Tick);
                    m_timerSub.Stop();
                }

                m_bolProgFlag = false;

                m_timerMain = null;
                m_timerSub = null;

                lbInterval.BackColor = System.Drawing.Color.Red;
                tsStatusLabel.Text = "중지...";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                txtInterval.Enabled = true;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }
        /// <summary>
        /// 상태창 Text 변경 메소드
        /// </summary>
        /// <param name="strMessage"></param>
        private void mfSetToolStripStatusLabel(string strMessage)
        {
            try
            {
                stStrip.mfInvokeIfRequired(() =>
                {
                    stStrip.Items["tsStatusLabel"].Text = strMessage;
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// Log 파일 그리드 Binding
        /// </summary>
        private void mfReadLogData()
        {
            try
            {
                string strPath = $"{CommonCode.CreateLogPathName}\\{m_strPlantCode}.{m_strMeasureName}\\log.log";
                System.IO.FileInfo fi = new FileInfo(strPath);
                if (fi.Exists)
                {
                    //string[] strLogMessage = System.IO.File.ReadAllLines(strPath);
                    txtLog.AppendText(System.IO.File.ReadAllText(strPath, Encoding.Default));
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// Log 정보 삭제
        /// </summary>
        private void mfDeleteGrid()
        {
            txtLog.mfInvokeIfRequired(() =>
            {
                txtLog.Clear();
            });
        }
        /// <summary>
        /// Directory 안의 파일 반환
        /// </summary>
        /// <param name="diInfo">Root 디렉토리 정보</param>
        /// <returns></returns>
        private System.Collections.Generic.List<FileInfo> mfGetFiles(DirectoryInfo diInfo)
        {
            System.Collections.Generic.List<FileInfo> files = new System.Collections.Generic.List<FileInfo>();
            try
            {
                System.IO.FileInfo[] getFiles = diInfo.GetFiles($"*{m_strFileExtension}");

                foreach (FileInfo f in diInfo.GetFiles($"*{m_strFileExtension}"))
                {
                    files.Add(f);
                }
                foreach (DirectoryInfo d in diInfo.GetDirectories())
                {
                    files.AddRange(mfGetFiles(d));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in GetFIles", ex);
            }
            return files;
        }

        #region File Parsing
        /// <summary>
        /// 전주 양이온
        /// 개별
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_Cation_03(string strFilePath)
        {
            try
            {
                DateTime dateSampleDate = DateTime.MaxValue;
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                    {
                        DataSet dsFile = GetSaveDefaultDataSet();
                        DataRow dr;
                        string strSampleID = string.Empty;
                        string strSampleDate = null;
                        bool bolBreak = false;
                        do
                        {
                            while (reader.Read())
                            {
                                switch (reader.GetString(0)??"")
                                {
                                    case "Sample ID:":
                                        strSampleID = reader.GetString(1);
                                        break;
                                    case "Sample Date/Time:":
                                        strSampleDate = string.Format("{0} {1}", reader.GetValue(2), reader.GetValue(3));
                                        DateTime.TryParse(strSampleDate, out dateSampleDate);
                                        bolBreak = true;
                                        break;
                                }

                                if (bolBreak)
                                {
                                    break;
                                }
                            }
                        } while (reader.NextResult() && !bolBreak);

                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = true,

                                ReadHeaderRow = (rowReader) =>
                                {
                                    // F.ex skip the first row and use the 2nd row as column headers:
                                    for (int i = 0; i < m_intRowIndex; i++)
                                    {
                                        rowReader.Read();
                                    }
                                }
                            }
                        });

                        if (strSampleID.Equals(string.Empty))
                        {
                            Logger.Error($"SampleID is Empty!");
                            return dateSampleDate;
                        }
                        else if((strSampleDate??"").Equals(string.Empty))
                        {
                            Logger.Error($"SampleDate is Empty!");
                            return dateSampleDate;
                        }

                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = strSampleID;
                        dr["SampleDate"] = dateSampleDate;
                        dr["BatchIndex"] = 1;
                        dsFile.Tables["H"].Rows.Add(dr);

                        int intRowCount = result.Tables[0].Rows.Count;

                        for (int i = 0; i < intRowCount; i++)
                        {
                            foreach (DataColumn col in result.Tables[0].Columns)
                            {
                                dr = dsFile.Tables["D"].NewRow();
                                dr["ColID"] = col.Ordinal;
                                dr["RowIndex"] = i;
                                dr["InspectValue"] = result.Tables[0].Rows[i][col.ColumnName];
                                dr["BatchIndex"] = 1;
                                dsFile.Tables["D"].Rows.Add(dr);
                            }
                        }

                        DateTime dateFileDate = dateSampleDate;
                        if (m_strPlantCode.Equals("03"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                        else if (m_strPlantCode.Equals("05"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                        string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                        dr = dsFile.Tables["FI"].NewRow();
                        System.IO.FileInfo fi = new FileInfo(strFilePath);
                        dr["FileID"] = 0;
                        dr["FileName"] = fi.Name;
                        dr["OriginFilePath"] = strFilePath;
                        dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                        dsFile.Tables["FI"].Rows.Add(dr);

                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["FC"].NewRow();
                            dr["FileID"] = 0;
                            dr["ColID"] = col.Ordinal;
                            dr["ColumnName"] = col.ColumnName;

                            dsFile.Tables["FC"].Rows.Add(dr);
                        }

                        string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                        TransErrRtn ErrRtn = new TransErrRtn();
                        ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                        if (!ErrRtn.ErrNum.Equals(0))
                        {
                            Logger.Error($"Save Error : {strErrRtn}");
                            dateSampleDate = DateTime.MaxValue;
                        }
                        else
                        {
                            Logger.Info($"Save Success! : {strFilePath}");
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 전주 음이온
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_Anion_03(string strFilePath)
        {
            try
            {
                // RowIndex : 0
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);

                DataSet dsFile = GetSaveDefaultDataSet();
                DateTime dateSampleDate = DateTime.MaxValue;

                DataRow dr;
                int intBatchIndex = 1;
                int intRowCount = dsData.Tables[0].Rows.Count;
                string strSampleID = string.Empty;
                string strSampleDate = string.Empty;

                for (int i = 0; i < intRowCount; i++)
                {
                    if (!dsData.Tables[0].Rows[i]["Ident"].ToString().Equals(string.Empty))
                    {
                        DateTime.TryParse(dsData.Tables[0].Rows[i]["Determination start"].ToString().Substring(0, 19), out dateSampleDate);

                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = dsData.Tables[0].Rows[i]["Ident"];
                        dr["SampleDate"] = dsData.Tables[0].Rows[i]["Determination start"].ToString().Substring(0, 19);
                        dr["BatchIndex"] = intBatchIndex;
                        dsFile.Tables["H"].Rows.Add(dr);

                        strSampleID = dsData.Tables[0].Rows[i]["Ident"].ToString();
                        strSampleDate= dsData.Tables[0].Rows[i]["Determination start"].ToString().Substring(0, 19);

                        foreach (DataColumn col in dsData.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["D"].NewRow();
                            dr["ColID"] = col.Ordinal;
                            dr["RowIndex"] = i;
                            dr["InspectValue"] = dsData.Tables[0].Rows[i][col.ColumnName];
                            dr["BatchIndex"] = intBatchIndex;
                            dsFile.Tables["D"].Rows.Add(dr);
                        }

                        intBatchIndex++;
                    }
                }

                if (strSampleID.Equals(string.Empty))
                {
                    Logger.Error($"SampleID is Empty!");
                    return dateSampleDate;
                }
                else if ((strSampleDate ?? "").Equals(string.Empty))
                {
                    Logger.Error($"SampleDate is Empty!");
                    return dateSampleDate;
                }

                DateTime dateFileDate = dateSampleDate;
                if (m_strPlantCode.Equals("03"))
                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                else if (m_strPlantCode.Equals("05"))
                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                dr = dsFile.Tables["FI"].NewRow();
                System.IO.FileInfo fi = new FileInfo(strFilePath);
                dr["FileID"] = 0;
                dr["FileName"] = fi.Name;
                dr["OriginFilePath"] = strFilePath;
                dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                dsFile.Tables["FI"].Rows.Add(dr);

                foreach (DataColumn col in dsData.Tables[0].Columns)
                {
                    dr = dsFile.Tables["FC"].NewRow();
                    dr["FileID"] = 0;
                    dr["ColID"] = col.Ordinal;
                    dr["ColumnName"] = col.ColumnName;

                    dsFile.Tables["FC"].Rows.Add(dr);
                }

                string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                TransErrRtn ErrRtn = new TransErrRtn();
                ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                if (!ErrRtn.ErrNum.Equals(0))
                {
                    Logger.Error($"Save Error : {strErrRtn}");
                    dateSampleDate = DateTime.MaxValue;
                }
                else
                {
                    Logger.Info($"Save Success! : {strFilePath}");
                }

                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 전주 밀도계
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_Density_03(string strFilePath)
        {
            try
            {
                // RowIndex : 2
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);

                DataSet dsFile = GetSaveDefaultDataSet();
                //var vDataRow = dsData.Tables[0].AsEnumerable().Where(w => !w.Field<string>("Unique Sample Id").Equals(string.Empty) && !w.Field<string>("Sample Name").Equals(string.Empty));
                DateTime dateSampleDate = DateTime.MaxValue;

                DataRow dr;
                int intBatchIndex = 1;
                int intRowCount = dsData.Tables[0].Rows.Count;
                string strSampleID = string.Empty;
                string strSampleDate = string.Empty;

                for (int i = 0; i < intRowCount; i++)
                {
                    if (!dsData.Tables[0].Rows[i]["Unique Sample Id"].ToString().Equals(string.Empty) && !dsData.Tables[0].Rows[i]["Sample Name"].ToString().Equals(string.Empty))
                    {
                        DateTime.TryParse(dsData.Tables[0].Rows[i]["Date"].ToString(), out dateSampleDate);
                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = dsData.Tables[0].Rows[i]["Sample Name"];
                        dr["SampleDate"] = dsData.Tables[0].Rows[i]["Date"];
                        dr["BatchIndex"] = intBatchIndex;
                        dsFile.Tables["H"].Rows.Add(dr);

                        strSampleID = dsData.Tables[0].Rows[i]["Sample Name"].ToString();
                        strSampleDate = dsData.Tables[0].Rows[i]["Date"].ToString();

                        foreach (DataColumn col in dsData.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["D"].NewRow();
                            dr["ColID"] = col.Ordinal;
                            dr["RowIndex"] = i;
                            dr["InspectValue"] = dsData.Tables[0].Rows[i][col.ColumnName];
                            dr["BatchIndex"] = intBatchIndex;
                            dsFile.Tables["D"].Rows.Add(dr);
                    }

                        intBatchIndex++;
                    }
                }

                if (strSampleID.Equals(string.Empty))
                {
                    Logger.Error($"SampleID is Empty!");
                    return dateSampleDate;
                }
                else if ((strSampleDate ?? "").Equals(string.Empty))
                {
                    Logger.Error($"SampleDate is Empty!");
                    return dateSampleDate;
                }

                DateTime dateFileDate = dateSampleDate;
                if (m_strPlantCode.Equals("03"))
                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                else if (m_strPlantCode.Equals("05"))
                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                dr = dsFile.Tables["FI"].NewRow();
                System.IO.FileInfo fi = new FileInfo(strFilePath);
                dr["FileID"] = 0;
                dr["FileName"] = fi.Name;
                dr["OriginFilePath"] = strFilePath;
                dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                dsFile.Tables["FI"].Rows.Add(dr);

                foreach (DataColumn col in dsData.Tables[0].Columns)
                {
                    dr = dsFile.Tables["FC"].NewRow();
                    dr["FileID"] = 0;
                    dr["ColID"] = col.Ordinal;
                    dr["ColumnName"] = col.ColumnName;
                    if (intRowCount > 0)
                        dr["UnitName"] = dsData.Tables[0].Rows[0][col.ColumnName];

                    dsFile.Tables["FC"].Rows.Add(dr);
                }

                string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                TransErrRtn ErrRtn = new TransErrRtn();
                ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                if(!ErrRtn.ErrNum.Equals(0))
                {
                    Logger.Error($"Save Error : {strErrRtn}");
                    dateSampleDate = DateTime.MaxValue;
                }
                else
                {
                    Logger.Info($"Save Success! : {strFilePath}");
                }

                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 전주 TOC
        /// 개별
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_TOC_03(string strFilePath)
        {
            try
            {

                DateTime dateSampleDate = DateTime.MaxValue;
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        // Gets or sets the encoding to use when the input XLS lacks a CodePage
                        // record, or when the input CSV lacks a BOM and does not parse as UTF8. 
                        // Default: cp1252 (XLS BIFF2-5 and CSV only)
                        FallbackEncoding = Encoding.GetEncoding(949),
                        //FallbackEncoding = Encoding.ASCII,

                        //// Gets or sets the password used to open password protected workbooks.
                        //Password = "password",

                        //// Gets or sets an array of CSV separator candidates. The reader 
                        //// autodetects which best fits the input data. Default: , ; TAB | # 
                        //// (CSV only)
                        //AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },

                        //// Gets or sets a value indicating whether to leave the stream open after
                        //// the IExcelDataReader object is disposed. Default: false
                        //LeaveOpen = false,

                        //// Gets or sets a value indicating the number of rows to analyze for
                        //// encoding, separator and field count in a CSV. When set, this option
                        //// causes the IExcelDataReader.RowCount property to throw an exception.
                        //// Default: 0 - analyzes the entire file (CSV only, has no effect on other
                        //// formats)
                        //AnalyzeInitialCsvRows = 0,
                    }))
                    {
                        DataSet dsFile = GetSaveDefaultDataSet();
                        DataRow dr;
                        string strSampleID = string.Empty;
                        string strSampleDate = null;
                        bool bolBreak = false;
                        do
                        {
                            while (reader.Read())
                            {
                                switch (reader.GetString(0) ?? "")
                                {
                                    case "Sample Name":
                                        strSampleID = reader.GetString(1);
                                        break;
                                    case "Date/Time":
                                        strSampleDate = reader.GetString(1);
                                        DateTime.TryParse(strSampleDate, out dateSampleDate);
                                        bolBreak = true;
                                        break;
                                }

                                if (bolBreak)
                                {
                                    break;
                                }
                            }
                        } while (reader.NextResult() && !bolBreak);

                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = true,

                                ReadHeaderRow = (rowReader) =>
                                {
                                    // F.ex skip the first row and use the 2nd row as column headers:
                                    for (int i = 0; i < m_intRowIndex; i++)
                                    {
                                        rowReader.Read();
                                    }
                                },
                                FilterRow = rowReader =>
                                {
                                    var hasData = true;
                                    switch (rowReader.GetString(0))
                                    {
                                        case "[Link Files]":
                                        case "Archived File":
                                        case "Data Profile":
                                        case "PDF Report":
                                        case "ASCII File":
                                        case "":
                                            hasData = false;
                                            break;
                                    }

                                    return hasData;
                                },
                            }
                        });

                        if (strSampleID.Equals(string.Empty))
                        {
                            Logger.Error($"SampleID is Empty!");
                            return dateSampleDate;
                        }
                        else if ((strSampleDate ?? "").Equals(string.Empty))
                        {
                            Logger.Error($"SampleDate is Empty!");
                            return dateSampleDate;
                        }

                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = strSampleID;
                        dr["SampleDate"] = dateSampleDate;
                        dr["BatchIndex"] = 1;
                        dsFile.Tables["H"].Rows.Add(dr);

                        int intRowCount = result.Tables[0].Rows.Count;

                        for (int i = 0; i < intRowCount; i++)
                        {
                            foreach (DataColumn col in result.Tables[0].Columns)
                            {
                                dr = dsFile.Tables["D"].NewRow();
                                dr["ColID"] = col.Ordinal;
                                dr["RowIndex"] = i;
                                dr["InspectValue"] = result.Tables[0].Rows[i][col.ColumnName];
                                dr["BatchIndex"] = 1;
                                dsFile.Tables["D"].Rows.Add(dr);
                            }
                        }

                        DateTime dateFileDate = dateSampleDate;
                        if (m_strPlantCode.Equals("03"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                        else if (m_strPlantCode.Equals("05"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                        string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                        dr = dsFile.Tables["FI"].NewRow();
                        System.IO.FileInfo fi = new FileInfo(strFilePath);
                        dr["FileID"] = 0;
                        dr["FileName"] = fi.Name;
                        dr["OriginFilePath"] = strFilePath;
                        dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                        dsFile.Tables["FI"].Rows.Add(dr);

                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["FC"].NewRow();
                            dr["FileID"] = 0;
                            dr["ColID"] = col.Ordinal;
                            dr["ColumnName"] = col.ColumnName;

                            dsFile.Tables["FC"].Rows.Add(dr);
                        }

                        string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                        TransErrRtn ErrRtn = new TransErrRtn();
                        ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                        if (!ErrRtn.ErrNum.Equals(0))
                        {
                            Logger.Error($"Save Error : {strErrRtn}");
                            dateSampleDate = DateTime.MaxValue;
                        }
                        else
                        {
                            Logger.Info($"Save Success! : {strFilePath}");
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 울산 밀도계
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_Density_05(string strFilePath)
        {
            try
            {
                // RowIndex : 2
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);

                DataSet dsFile = GetSaveDefaultDataSet();
                DateTime dateSampleDate = DateTime.MaxValue;

                DataRow dr;
                int intBatchIndex = 1;
                int intRowCount = dsData.Tables[0].Rows.Count;
                string strSampleID = string.Empty;
                string strSampleDate = string.Empty;

                for (int i = 0; i < intRowCount; i++)
                {
                    if (!dsData.Tables[0].Rows[i]["Unique Sample Id"].ToString().Equals(string.Empty) && !dsData.Tables[0].Rows[i]["Sample Name"].ToString().Equals(string.Empty))
                    {
                        DateTime.TryParse(dsData.Tables[0].Rows[i]["Date"].ToString(), out dateSampleDate);
                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = dsData.Tables[0].Rows[i]["Sample Name"];
                        dr["SampleDate"] = dsData.Tables[0].Rows[i]["Date"];
                        dr["BatchIndex"] = intBatchIndex;
                        dsFile.Tables["H"].Rows.Add(dr);

                        strSampleID = dsData.Tables[0].Rows[i]["Sample Name"].ToString();
                        strSampleDate = dsData.Tables[0].Rows[i]["Date"].ToString();

                        foreach (DataColumn col in dsData.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["D"].NewRow();
                            dr["ColID"] = col.Ordinal;
                            dr["RowIndex"] = i;
                            dr["InspectValue"] = dsData.Tables[0].Rows[i][col.ColumnName];
                            dr["BatchIndex"] = intBatchIndex;
                            dsFile.Tables["D"].Rows.Add(dr);
                        }

                        intBatchIndex++;
                    }
                }

                if (strSampleID.Equals(string.Empty))
                {
                    Logger.Error($"SampleID is Empty!");
                    return dateSampleDate;
                }
                else if ((strSampleDate ?? "").Equals(string.Empty))
                {
                    Logger.Error($"SampleDate is Empty!");
                    return dateSampleDate;
                }

                DateTime dateFileDate = dateSampleDate;
                if (m_strPlantCode.Equals("03"))
                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                else if (m_strPlantCode.Equals("05"))
                    dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                dr = dsFile.Tables["FI"].NewRow();
                System.IO.FileInfo fi = new FileInfo(strFilePath);
                dr["FileID"] = 0;
                dr["FileName"] = fi.Name;
                dr["OriginFilePath"] = strFilePath;
                dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                dsFile.Tables["FI"].Rows.Add(dr);

                foreach (DataColumn col in dsData.Tables[0].Columns)
                {
                    dr = dsFile.Tables["FC"].NewRow();
                    dr["FileID"] = 0;
                    dr["ColID"] = col.Ordinal;
                    dr["ColumnName"] = col.ColumnName;
                    if (intRowCount > 0)
                        dr["UnitName"] = dsData.Tables[0].Rows[0][col.ColumnName];

                    dsFile.Tables["FC"].Rows.Add(dr);
                }

                string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                TransErrRtn ErrRtn = new TransErrRtn();
                ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                if (!ErrRtn.ErrNum.Equals(0))
                {
                    Logger.Error($"Save Error : {strErrRtn}");
                    dateSampleDate = DateTime.MaxValue;
                }
                else
                {
                    Logger.Info($"Save Success! : {strFilePath}");
                }

                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 울산 양이온(A-M90)
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Cation_05_M90(string strFilePath)
        {
            try
            {
                // RowIndex : 4
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 울산 양이온(7900)
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_Cation_05_7900(string strFilePath)
        {
            // RowIndex : 0
            try
            {
                DateTime dateSampleDate = DateTime.MaxValue;
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    DataSet dsFile = GetSaveDefaultDataSet();
                    DataRow dr;

                    var vBatch = dsData.Tables[0].AsEnumerable().Select(s => new
                    {
                        SampleID = s.Field<string>("Sample Name"),
                        SampleDate = s.Field<string>("Date and Time Acquired")
                    })
                        .Distinct().OrderBy(o => o.SampleDate);

                    int intBatchIndex = 0;
                    foreach (var batch in vBatch)
                    {
                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = batch.SampleID;
                        dr["SampleDate"] = batch.SampleDate;
                        dr["BatchIndex"] = intBatchIndex;
                        dsFile.Tables["H"].Rows.Add(dr);

                        var vDetail = dsData.Tables[0].AsEnumerable().Where(w => w.Field<string>("Sample Name").Equals(batch.SampleID) && w.Field<string>("Date and Time Acquired").Equals(batch.SampleDate));
                        foreach (var dd in vDetail)
                        {
                            foreach (DataColumn col in dsData.Tables[0].Columns)
                            {
                                dr = dsFile.Tables["D"].NewRow();
                                dr["ColID"] = col.Ordinal;
                                dr["RowIndex"] = dsData.Tables[0].Rows.IndexOf(dd);
                                dr["InspectValue"] = dd.Field<dynamic>(col.ColumnName);
                                dr["BatchIndex"] = intBatchIndex;
                                dsFile.Tables["D"].Rows.Add(dr);
                            }
                        }
                        DateTime.TryParse(batch.SampleDate, out dateSampleDate);
                        intBatchIndex++;
                    }

                    DateTime dateFileDate = dateSampleDate;
                    if (m_strPlantCode.Equals("03"))
                        dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                    else if (m_strPlantCode.Equals("05"))
                        dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                    string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                    dr = dsFile.Tables["FI"].NewRow();
                    System.IO.FileInfo fi = new FileInfo(strFilePath);
                    dr["FileID"] = 0;
                    dr["FileName"] = fi.Name;
                    dr["OriginFilePath"] = strFilePath;
                    dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                    dsFile.Tables["FI"].Rows.Add(dr);

                    foreach (DataColumn col in dsData.Tables[0].Columns)
                    {
                        dr = dsFile.Tables["FC"].NewRow();
                        dr["FileID"] = 0;
                        dr["ColID"] = col.Ordinal;
                        dr["ColumnName"] = col.ColumnName;

                        dsFile.Tables["FC"].Rows.Add(dr);
                    }

                    string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                    TransErrRtn ErrRtn = new TransErrRtn();
                    ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                    if (!ErrRtn.ErrNum.Equals(0))
                    {
                        Logger.Error($"Save Error : {strErrRtn}");
                        dateSampleDate = DateTime.MaxValue;
                    }
                    else
                    {
                        Logger.Info($"Save Success! : {strFilePath}");
                    }
                }

                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 울산 양이온 ICP-OES
        /// 개별
        /// </summary>
        /// <param name="strFilePath"></param>
        private DateTime mfParsing_Cation_05_OES(string strFilePath)
        {
            try
            {
                DateTime dateSampleDate = DateTime.MaxValue;
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet dsFile = GetSaveDefaultDataSet();
                        DataRow dr;

                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = false,

                                ReadHeaderRow = (rowReader) =>
                                {
                                    // RowIndex : 1
                                    for (int i = 0; i < m_intRowIndex; i++)
                                    {
                                        rowReader.Read();
                                    }
                                },
                                FilterRow = rowReader =>
                                {
                                    var hasData = false;
                                    for (var i = 0; i < rowReader.FieldCount; i++)
                                    {
                                        if (rowReader[i] == null || string.IsNullOrEmpty(rowReader[i].ToString()))
                                        {
                                            continue;
                                        }

                                        hasData = true;
                                        break;
                                    }

                                    if (rowReader[0] != null && rowReader[0].ToString().StartsWith("Worksheet exported"))
                                        return false;

                                    return hasData;
                                },
                            }
                        });

                        var vHeaders = result.Tables[0].AsEnumerable().Where(w => w.Field<object>(0).Equals("Label") && w.Field<object>(1).Equals("Type")).ToList();
                        if (vHeaders.Count() > 0)
                        {
                            var vHeader = vHeaders.First();

                            int intColCount = result.Tables[0].Columns.Count;
                            for (int i = 0; i < intColCount; i++)
                            {
                                result.Tables[0].Columns[i].ColumnName = vHeader[i].ToString();
                                result.Tables[0].Columns[i].Caption = vHeader[i].ToString();
                            }
                        }

                        foreach (var dd in vHeaders)
                        {
                            dd.Delete();
                        }

                        result.Tables[0].AcceptChanges();

                        var vBatch = result.Tables[0].AsEnumerable().Select(s => new
                        {
                            SampleID = s.Field<object>("Label"),
                            SampleDate = s.Field<DateTime>("Date Time")
                        })
                            .Distinct().OrderBy(o => o.SampleDate);

                        int intBatchIndex = 0;
                        foreach (var batch in vBatch)
                        {
                            dr = dsFile.Tables["H"].NewRow();
                            dr["BatchID"] = 0;
                            dr["PlantCode"] = m_strPlantCode;
                            dr["ProcessGroupCode"] = m_strProcessGroupCode;
                            dr["InspectTypeCode"] = m_strInspectTypeCode;
                            dr["SampleName"] = batch.SampleID;
                            dr["SampleDate"] = batch.SampleDate;
                            dr["BatchIndex"] = intBatchIndex;
                            dsFile.Tables["H"].Rows.Add(dr);

                            var vDetail = result.Tables[0].AsEnumerable().Where(w => w.Field<object>("Label").Equals(batch.SampleID) && w.Field<DateTime>("Date Time").Equals(batch.SampleDate));
                            foreach (var dd in vDetail)
                            {
                                foreach (DataColumn col in result.Tables[0].Columns)
                                {
                                    dr = dsFile.Tables["D"].NewRow();
                                    dr["ColID"] = col.Ordinal;
                                    dr["RowIndex"] = result.Tables[0].Rows.IndexOf(dd);
                                    dr["InspectValue"] = dd.Field<dynamic>(col.ColumnName);
                                    dr["BatchIndex"] = intBatchIndex;
                                    dsFile.Tables["D"].Rows.Add(dr);
                                }
                            }

                            dateSampleDate = batch.SampleDate;
                            intBatchIndex++;
                        }

                        DateTime dateFileDate = dateSampleDate;
                        if (m_strPlantCode.Equals("03"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                        else if (m_strPlantCode.Equals("05"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                        string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                        dr = dsFile.Tables["FI"].NewRow();
                        System.IO.FileInfo fi = new FileInfo(strFilePath);
                        dr["FileID"] = 0;
                        dr["FileName"] = fi.Name;
                        dr["OriginFilePath"] = strFilePath;
                        dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                        dsFile.Tables["FI"].Rows.Add(dr);

                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["FC"].NewRow();
                            dr["FileID"] = 0;
                            dr["ColID"] = col.Ordinal;
                            dr["ColumnName"] = col.ColumnName;

                            dsFile.Tables["FC"].Rows.Add(dr);
                        }

                        string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                        TransErrRtn ErrRtn = new TransErrRtn();
                        ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                        if (!ErrRtn.ErrNum.Equals(0))
                        {
                            Logger.Error($"Save Error : {strErrRtn}");
                            dateSampleDate = DateTime.MaxValue;
                        }
                        else
                        {
                            Logger.Info($"Save Success! : {strFilePath}");
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 울산 음이온(IC)
        /// 개별
        /// </summary>
        /// <param name="strFilePath"></param>
        private DateTime mfParsing_Anion_05(string strFilePath)
        {
            try
            {
                DateTime dateSampleDate = DateTime.MaxValue;
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet dsFile = GetSaveDefaultDataSet();
                        DataRow dr;
                        string strSampleID = string.Empty;
                        string strSampleDate = null;
                        bool bolBreak = false;
                        do
                        {
                            while (reader.Read())
                            {
                                switch (reader.GetValue(0) ?? "")
                                {
                                    case "Sample Name:":
                                        strSampleID = reader.GetString(2);
                                        break;
                                    case "Recording Time:":
                                        strSampleDate = reader.GetValue(2).ToString();
                                        dateSampleDate = reader.GetDateTime(2);
                                        bolBreak = true;
                                        break;
                                }

                                if (bolBreak)
                                {
                                    break;
                                }

                            }
                        } while (reader.NextResult() && !bolBreak);

                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = true,

                                ReadHeaderRow = (rowReader) =>
                                {
                                    // RowIndex : 26
                                    for (int i = 0; i < m_intRowIndex; i++)
                                    {
                                        rowReader.Read();
                                    }
                                }
                            }
                        });

                        if (strSampleID.Equals(string.Empty))
                        {
                            Logger.Error($"SampleID is Empty!");
                            return dateSampleDate;
                        }
                        else if ((strSampleDate ?? "").Equals(string.Empty))
                        {
                            Logger.Error($"SampleDate is Empty!");
                            return dateSampleDate;
                        }

                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = strSampleID;
                        dr["SampleDate"] = dateSampleDate;
                        dr["BatchIndex"] = 1;
                        dsFile.Tables["H"].Rows.Add(dr);

                        int intRowCount = result.Tables[0].Rows.Count;

                        for (int i = 0; i < intRowCount; i++)
                        {
                            foreach (DataColumn col in result.Tables[0].Columns)
                            {
                                dr = dsFile.Tables["D"].NewRow();
                                dr["ColID"] = col.Ordinal;
                                dr["RowIndex"] = i;
                                dr["InspectValue"] = result.Tables[0].Rows[i][col.ColumnName];
                                dr["BatchIndex"] = 1;
                                dsFile.Tables["D"].Rows.Add(dr);
                            }
                        }

                        DateTime dateFileDate = dateSampleDate;
                        if (m_strPlantCode.Equals("03"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                        else if (m_strPlantCode.Equals("05"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                        string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                        dr = dsFile.Tables["FI"].NewRow();
                        System.IO.FileInfo fi = new FileInfo(strFilePath);
                        dr["FileID"] = 0;
                        dr["FileName"] = fi.Name;
                        dr["OriginFilePath"] = strFilePath;
                        dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                        dsFile.Tables["FI"].Rows.Add(dr);

                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["FC"].NewRow();
                            dr["FileID"] = 0;
                            dr["ColID"] = col.Ordinal;
                            dr["ColumnName"] = col.ColumnName;

                            dsFile.Tables["FC"].Rows.Add(dr);
                        }

                        string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                        TransErrRtn ErrRtn = new TransErrRtn();
                        ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                        if (!ErrRtn.ErrNum.Equals(0))
                        {
                            Logger.Error($"Save Error : {strErrRtn}");
                            dateSampleDate = DateTime.MaxValue;
                        }
                        else
                        {
                            Logger.Info($"Save Success! : {strFilePath}");
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 울산 TOC
        /// 개별
        /// </summary>
        /// <param name="strFilePath"></param>
        private DateTime mfParsing_TOC_05(string strFilePath)
        {
            try
            {
                DateTime dateSampleDate = DateTime.MaxValue;
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        // Gets or sets the encoding to use when the input XLS lacks a CodePage
                        // record, or when the input CSV lacks a BOM and does not parse as UTF8. 
                        // Default: cp1252 (XLS BIFF2-5 and CSV only)
                        FallbackEncoding = Encoding.GetEncoding(949),
                        //FallbackEncoding = Encoding.ASCII,

                        //// Gets or sets the password used to open password protected workbooks.
                        //Password = "password",

                        //// Gets or sets an array of CSV separator candidates. The reader 
                        //// autodetects which best fits the input data. Default: , ; TAB | # 
                        //// (CSV only)
                        //AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },

                        //// Gets or sets a value indicating whether to leave the stream open after
                        //// the IExcelDataReader object is disposed. Default: false
                        //LeaveOpen = false,

                        //// Gets or sets a value indicating the number of rows to analyze for
                        //// encoding, separator and field count in a CSV. When set, this option
                        //// causes the IExcelDataReader.RowCount property to throw an exception.
                        //// Default: 0 - analyzes the entire file (CSV only, has no effect on other
                        //// formats)
                        //AnalyzeInitialCsvRows = 0,
                    }))
                    {
                        DataSet dsFile = GetSaveDefaultDataSet();
                        DataRow dr;
                        string strSampleID = string.Empty;
                        string strSampleDate = null;
                        bool bolBreak = false;
                        do
                        {
                            while (reader.Read())
                            {
                                switch (reader.GetString(0) ?? "")
                                {
                                    case "Sample Name":
                                        strSampleID = reader.GetString(1);
                                        break;
                                    case "Date/Time":
                                        strSampleDate = reader.GetString(1);
                                        DateTime.TryParse(strSampleDate, out dateSampleDate);
                                        if (dateSampleDate.ToString("yyyy-MM-dd HH:mm:ss").Equals("0001-01-01 00:00:00"))
                                        {
                                            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
                                            dateSampleDate = DateTime.ParseExact(strSampleDate, "dd/MM/yyyy HH:mm:ss", culture);
                                        }
                                        bolBreak = true;
                                        break;
                                }

                                if (bolBreak)
                                {
                                    break;
                                }
                            }
                        } while (reader.NextResult() && !bolBreak);

                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = true,

                                ReadHeaderRow = (rowReader) =>
                                {
                                    // F.ex skip the first row and use the 2nd row as column headers:
                                    for (int i = 0; i < m_intRowIndex; i++)
                                    {
                                        rowReader.Read();
                                    }
                                },
                                FilterRow = rowReader =>
                                {
                                    var hasData = true;
                                    switch (rowReader.GetString(0))
                                    {
                                        case "[Link Files]":
                                        case "Archived File":
                                        case "Data Profile":
                                        case "PDF Report":
                                        case "ASCII File":
                                        case "":
                                            hasData = false;
                                            break;
                                    }

                                    return hasData;
                                },
                            }
                        });

                        if (strSampleID.Equals(string.Empty))
                        {
                            Logger.Error($"SampleID is Empty!");
                            return dateSampleDate;
                        }
                        else if ((strSampleDate ?? "").Equals(string.Empty))
                        {
                            Logger.Error($"SampleDate is Empty!");
                            return dateSampleDate;
                        }

                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = m_strPlantCode;
                        dr["ProcessGroupCode"] = m_strProcessGroupCode;
                        dr["InspectTypeCode"] = m_strInspectTypeCode;
                        dr["SampleName"] = strSampleID;
                        dr["SampleDate"] = dateSampleDate;
                        dr["BatchIndex"] = 1;
                        dsFile.Tables["H"].Rows.Add(dr);

                        int intRowCount = result.Tables[0].Rows.Count;

                        for (int i = 0; i < intRowCount; i++)
                        {
                            foreach (DataColumn col in result.Tables[0].Columns)
                            {
                                dr = dsFile.Tables["D"].NewRow();
                                dr["ColID"] = col.Ordinal;
                                dr["RowIndex"] = i;
                                dr["InspectValue"] = result.Tables[0].Rows[i][col.ColumnName];
                                dr["BatchIndex"] = 1;
                                dsFile.Tables["D"].Rows.Add(dr);
                            }
                        }

                        DateTime dateFileDate = dateSampleDate;
                        if (m_strPlantCode.Equals("03"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_03;
                        else if (m_strPlantCode.Equals("05"))
                            dateFileDate = dateFileDate - Properties.Settings.Default.StartTime_05;

                        string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, dateSampleDate.ToString("yyyy-MM-dd"), m_strMeasureName);

                        dr = dsFile.Tables["FI"].NewRow();
                        System.IO.FileInfo fi = new FileInfo(strFilePath);
                        dr["FileID"] = 0;
                        dr["FileName"] = fi.Name;
                        dr["OriginFilePath"] = strFilePath;
                        dr["BackupFilePath"] = System.IO.Path.Combine(strTargetPath, fi.Name);
                        dsFile.Tables["FI"].Rows.Add(dr);

                        foreach (DataColumn col in result.Tables[0].Columns)
                        {
                            dr = dsFile.Tables["FC"].NewRow();
                            dr["FileID"] = 0;
                            dr["ColID"] = col.Ordinal;
                            dr["ColumnName"] = col.ColumnName;

                            dsFile.Tables["FC"].Rows.Add(dr);
                        }

                        string strErrRtn = QRPDaemon.BL.clsBL.mfSaveBatchFile(dsFile);
                        TransErrRtn ErrRtn = new TransErrRtn();
                        ErrRtn = ErrRtn.mfDecodingErrMessage(strErrRtn);
                        if (!ErrRtn.ErrNum.Equals(0))
                        {
                            Logger.Error($"Save Error : {strErrRtn}");
                            dateSampleDate = DateTime.MaxValue;
                        }
                        else
                        {
                            Logger.Info($"Save Success! : {strFilePath}");
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
                return dateSampleDate;
            }
            catch (Exception ex)
            {
                throw new System.ApplicationException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
        /// <summary>
        /// 파일읽기
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="intRowIndex"></param>
        /// <returns></returns>
        private DataSet mfReadFile(string strFilePath, int intRowIndex = 0)
        {
            try
            {
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (Path.GetExtension(strFilePath).ToUpper().Equals(".XLS") || Path.GetExtension(strFilePath).ToUpper().Equals(".XLSX"))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    EmptyColumnNamePrefix = "Column",
                                    UseHeaderRow = true,

                                    ReadHeaderRow = (rowReader) =>
                                    {
                                        // F.ex skip the first row and use the 2nd row as column headers:
                                        for (int i = 0; i < intRowIndex; i++)
                                        {
                                            rowReader.Read();
                                        }
                                    },
                                    FilterRow = rowReader =>
                                    {
                                        var hasData = false;
                                        for (var i = 0; i < rowReader.FieldCount; i++)
                                        {
                                            if (rowReader[i] == null || string.IsNullOrEmpty(rowReader[i].ToString()))
                                            {
                                                continue;
                                            }

                                            hasData = true;
                                            break;
                                        }

                                        return hasData;
                                    },
                                }
                            });
                            reader.Close();
                            stream.Close();

                            return result;
                        }
                    }
                    else if (Path.GetExtension(strFilePath).ToUpper().Equals(".CSV") || Path.GetExtension(strFilePath).ToUpper().Equals(".REP") || Path.GetExtension(strFilePath).ToUpper().Equals(".TXT"))
                    {
                        using (var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                        {
                            // Gets or sets the encoding to use when the input XLS lacks a CodePage
                            // record, or when the input CSV lacks a BOM and does not parse as UTF8. 
                            // Default: cp1252 (XLS BIFF2-5 and CSV only)
                            FallbackEncoding = Encoding.GetEncoding(949),

                            //// Gets or sets the password used to open password protected workbooks.
                            //Password = "password",

                            //// Gets or sets an array of CSV separator candidates. The reader 
                            //// autodetects which best fits the input data. Default: , ; TAB | # 
                            //// (CSV only)
                            //AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },

                            //// Gets or sets a value indicating whether to leave the stream open after
                            //// the IExcelDataReader object is disposed. Default: false
                            //LeaveOpen = false,

                            //// Gets or sets a value indicating the number of rows to analyze for
                            //// encoding, separator and field count in a CSV. When set, this option
                            //// causes the IExcelDataReader.RowCount property to throw an exception.
                            //// Default: 0 - analyzes the entire file (CSV only, has no effect on other
                            //// formats)
                            //AnalyzeInitialCsvRows = 0,
                        }))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    EmptyColumnNamePrefix = "Column",
                                    UseHeaderRow = true,

                                    ReadHeaderRow = (rowReader) =>
                                    {
                                        // F.ex skip the first row and use the 2nd row as column headers:
                                        for (int i = 0; i < intRowIndex; i++)
                                        {
                                            rowReader.Read();
                                        }
                                    },
                                    FilterRow = rowReader =>
                                    {
                                        var hasData = false;
                                        for (var i = 0; i < rowReader.FieldCount; i++)
                                        {
                                            if (rowReader[i] == null || string.IsNullOrEmpty(rowReader[i].ToString()))
                                            {
                                                continue;
                                            }

                                            hasData = true;
                                            break;
                                        }

                                        return hasData;
                                    },
                                }
                            });
                            reader.Close();
                            stream.Close();

                            return result;
                        }
                    }
                    else
                    {
                        throw new System.ApplicationException("Invalid File");
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Error($"Error In mfReadFile : {strFilePath}", ex);
                throw (ex);
            }
        }
        #endregion File Parsing

        /// <summary>
        /// 저장용 기본 Dataset 반환
        /// </summary>
        /// <returns></returns>
        private DataSet GetSaveDefaultDataSet()
        {
            DataSet dsData = new DataSet();

            DataTable dtFI = new DataTable("FI");
            dtFI.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("FileID", typeof(Int64))
                , new DataColumn("FileName", typeof(string))
                , new DataColumn("OriginFilePath", typeof(string))
                , new DataColumn("BackupFilePath", typeof(string))
            });

            DataTable dtFC = new DataTable("FC");
            dtFC.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("FileID", typeof(Int64))
                , new DataColumn("ColID", typeof(int))
                , new DataColumn("ColumnName", typeof(string))
                , new DataColumn("UnitName", typeof(string))
            });

            DataTable dtHeader = new DataTable("H");
            dtHeader.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("BatchID", typeof(Int64))
                , new DataColumn("PlantCode", typeof(string))
                , new DataColumn("ProcessGroupCode", typeof(string))
                , new DataColumn("InspectTypeCode", typeof(string))
                , new DataColumn("SampleName", typeof(string))
                , new DataColumn("SampleDate", typeof(DateTime))
                , new DataColumn("BatchIndex", typeof(int))
            });

            DataTable dtData = new DataTable("D");
            dtData.Columns.AddRange(new DataColumn[]{
                new DataColumn("ColID", typeof(int))
                , new DataColumn("RowIndex", typeof(int))
                , new DataColumn("InspectValue", typeof(string))
                , new DataColumn("BatchIndex", typeof(int))
            });

            dsData.Tables.Add(dtFI);
            dsData.Tables.Add(dtFC);
            dsData.Tables.Add(dtHeader);
            dsData.Tables.Add(dtData);

            return dsData;
        }

        private ILog SetLog()
        {
            String FilePath;
            Hierarchy hierarchy = new Hierarchy();
            RollingFileAppender rollingAppender = new RollingFileAppender();
            PatternLayout layout = new PatternLayout();

            ILog log;

            string strName = $"{m_strPlantCode}.{m_strMeasureName}";

            FilePath = $"logs\\{strName}\\log.log";

            hierarchy.Configured = true;

            rollingAppender.Name = "CheckFile";
            rollingAppender.ImmediateFlush = true;
            rollingAppender.File = FilePath;
            rollingAppender.Encoding = Encoding.UTF8;
            rollingAppender.AppendToFile = true;

            //rollingAppender.DatePattern = "yyyyMMdd'.log''";
            rollingAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            rollingAppender.LockingModel = new RollingFileAppender.MinimalLock();
            rollingAppender.StaticLogFileName = true;
            rollingAppender.MaxSizeRollBackups = 5;
            rollingAppender.MaximumFileSize = "500KB";

            layout = new log4net.Layout.PatternLayout("[ %date ] [%thread] [ %-5level ] %logger : \"%message\" - %exception%newline");
            rollingAppender.Layout = layout;
            rollingAppender.ActivateOptions();

            TextBoxAppender textBoxAppender = new TextBoxAppender();
            textBoxAppender.AppenderTextBox = txtLog;
            textBoxAppender.Threshold = log4net.Core.Level.All;
            textBoxAppender.Layout = layout;
            textBoxAppender.Name = string.Format("TextBoxAppender_{0}", txtLog.Name);
            textBoxAppender.ActivateOptions();

            log4net.Repository.ILoggerRepository repository = null;
            var vQuery = LogManager.GetAllRepositories().AsEnumerable().Where(w => w.Name.Equals(strName));
            foreach (var loggerRepository in vQuery)
            {
                repository = loggerRepository;
            }

            if (repository == null)
            {
                repository = LogManager.CreateRepository(strName);
                BasicConfigurator.Configure(repository, rollingAppender, textBoxAppender);
            }

            log = LogManager.GetLogger(strName, "CheckFileLogger");

            return log;
        }
        #endregion Method
    }
}
