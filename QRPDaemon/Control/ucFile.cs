using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using ExcelDataReader;
using QRPDaemon.COM;

namespace QRPDaemon.Control
{
    public partial class ucFile : UserControl
    {
        #region Var
        private System.Windows.Forms.Timer m_timerMain;
        private System.Windows.Forms.Timer m_timerSub;

        private bool m_bolProgFlag = false;
        private string m_strPlantCode = string.Empty;
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
        /// 공장코드
        /// </summary>
        public string PlantCode
        {
            //get { return m_strPlantCode; }
            set { m_strPlantCode = value; }
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
            st.SetTime(new TimeSpan(0, 0, 0), mfDeleteGrid);
        }

        #region Initialize

        private void InitControl()
        {
            tsStatusLabel.Text = string.Empty;
            btnStop.Enabled = false;

            // 주기
            txtInterval.Text = m_intIntervar.ToString();

            #region DataGridView
            dgvLogList.ReadOnly = true;
            dgvLogList.AllowUserToAddRows = false;
            dgvLogList.AllowUserToDeleteRows = false;
            dgvLogList.RowHeadersVisible = false;

            dgvLogList.Columns.Add("IFDateTime", "IF일시");
            dgvLogList.Columns.Add("IFMessage", "IF처리메세지");

            dgvLogList.Columns["IFMessage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            #endregion

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
                    if (m_bolProgFlag)
                    {
                        mfSetToolStripStatusLabel("Start...");
                        SharedDirectory sd = new SharedDirectory();
                        try
                        {
                            System.IO.DirectoryInfo diInfo = new System.IO.DirectoryInfo(m_strOriginFilePath);
                            if (diInfo.Exists)
                            {
                                DateTime Now = DateTime.Now;
                                if (m_strPlantCode.Equals("03"))
                                    Now = Now - Properties.Settings.Default.StartTime_03;
                                else if (m_strPlantCode.Equals("05"))
                                    Now = Now - Properties.Settings.Default.StartTime_05;

                                string strTargetPath = string.Format(@"{0}\{1}\{2}", m_strBackupFilePath, Now.ToString("yyyy-MM-dd"), m_strMeasureName);
                                System.IO.DirectoryInfo diTarget = new System.IO.DirectoryInfo(strTargetPath);
                                if (!diTarget.Exists)
                                    diTarget.Create();

                                System.IO.FileInfo[] getFiles = diInfo.GetFiles();
                                foreach (System.IO.FileInfo fi in getFiles)
                                {
                                    if (fi.Extension.Equals(m_strFileExtension))
                                    {
                                        mfAddGridMessage(string.Format("1.Parsing Start : {0}", fi.FullName));
                                        if (m_strPlantCode.Equals("03"))
                                        {
                                            switch (m_strMeasureName)
                                            {
                                                case "밀도계":
                                                    mfParsing_Density_03(fi.FullName);
                                                    break;
                                                case "TOC":
                                                    mfParsing_TOC_03(fi.FullName);
                                                    break;
                                                case "음이온":
                                                    mfParsing_Anion_03(fi.FullName);
                                                    break;
                                                case "양이온":
                                                    mfParsing_Cation_03(fi.FullName);
                                                    break;
                                            }
                                        }
                                        else if (m_strPlantCode.Equals("05"))
                                        {
                                            switch (m_strMeasureName)
                                            {
                                                case "밀도계":
                                                    mfParsing_Density_05(fi.FullName);
                                                    break;
                                                case "TOC":
                                                    mfParsing_TOC_05(fi.FullName);
                                                    break;
                                                case "ICP-MS(7900)":
                                                    mfParsing_Cation_05_7900(fi.FullName);
                                                    break;
                                                case "ICP-MS(A-M90)":
                                                    mfParsing_Cation_05_M90(fi.FullName);
                                                    break;
                                                case "ICP-OES":
                                                    mfParsing_Cation_05_M90(fi.FullName);
                                                    break;
                                                case "ILC":
                                                    mfParsing_Anion_05(fi.FullName);
                                                    break;
                                            }
                                        }
                                        mfAddGridMessage(string.Format("2.Parsing End : {0}", fi.FullName));
                                    }

                                    //fi.MoveTo(System.IO.Path.Combine(strTargetPath, fi.Name));
                                    //fi.CopyTo(System.IO.Path.Combine(strTargetPath, fi.Name), true);

                                    mfAddGridMessage(string.Format("3.File BackUp : {0}", fi.FullName));
                                }
                            }
                            else
                            {
                                mfAddGridMessage("파일경로 에러!");
                            }

                            mfSetToolStripStatusLabel("처리 완료..." + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        catch (System.Exception ex)
                        {
                        }
                        finally
                        {
                            sd.mfDisconnectNetworkDrive(m_strOriginFilePath);
                            sd.mfDisconnectNetworkDrive(m_strBackupFilePath);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    mfAddGridMessage(ex.ToString());
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

                tsStatusLabel.Text = "시작...";

                //m_timerMain.Start();
                //m_timerSub.Start();

                if (!m_bolProgFlag)
                {
                    m_bolProgFlag = true;
                    Task work = mfDoWork();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                mfAddGridMessage(ex.ToString());
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
        /// Lot리스트 기록메소드
        /// </summary>
        /// <param name="strMessageLine"></param>
        private void mfAddGridMessage(string strMessage)
        {
            try
            {
                dgvLogList.mfInvokeIfRequired(() =>
                {
                    string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    dgvLogList.Rows.Add(strDateTime, strMessage);

                    // 바인딩후 가장 마지막 행으로 스크롤 이동
                    dgvLogList.FirstDisplayedScrollingRowIndex = dgvLogList.Rows.Count - 1;

                    new Log().mfWriteLog(CommonCode.CreateLogPathName, m_strMeasureName, strDateTime, strMessage);
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                string[] strLogMessages = new Log().mfReadRow(CommonCode.CreateLogPathName, m_strMeasureName);
                int intCnt = strLogMessages.Length;

                char[] strSplit = new char[] { '|' };
                for (int i = 0; i < intCnt; i++)
                {
                    string[] strMessageLine = strLogMessages[i].Split(strSplit);
                    dgvLogList.Rows.Add(strMessageLine[0], strMessageLine[1]);
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
            dgvLogList.mfInvokeIfRequired(() =>
            {
                dgvLogList.Rows.Clear();
            });
        }

        #region File Parsing
        /// <summary>
        /// 전주 양이온
        /// 개별
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Cation_03(string strFilePath)
        {
            try
            {
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                    {
                        string strSampleID;
                        string strSampleDate;
                        DateTime dateSampleDate;
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
                        } while (reader.NextResult());

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

                        reader.Close();
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 전주 음이온
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Anion_03(string strFilePath)
        {
            try
            {
                // RowIndex : 0
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 전주 밀도계
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Density_03(string strFilePath)
        {
            try
            {
                // RowIndex : 2
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 전주 TOC
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_TOC_03(string strFilePath)
        {
            try
            {
                // RowIndex : 10
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 울산 밀도계
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Density_05(string strFilePath)
        {
            try
            {
                // RowIndex : 2
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
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
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 울산 양이온(7900)
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Cation_05_7900(string strFilePath)
        {
            try
            {
                // RowIndex : 0
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 울산 양이온 ICP-OES
        /// 공통
        /// </summary>
        /// <param name="strFilePath"></param>
        private void mfParsing_Cation_05_OES(string strFilePath)
        {
            try
            {
                // RowIndex : 2
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 울산 음이온(ILC)
        /// 개별
        /// </summary>
        /// <param name="strFilePath"></param>
        private void mfParsing_Anion_05(string strFilePath)
        {
            try
            {
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        string strSampleID;
                        DateTime dateSampleDate;
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
                                        dateSampleDate = reader.GetDateTime(2);
                                        bolBreak = true;
                                        break;
                                }

                                if (bolBreak)
                                {
                                    break;
                                }

                            }
                        } while (reader.NextResult());

                        DataSet dsData = reader.AsDataSet(new ExcelDataSetConfiguration()
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
                    }
                }
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 울산 TOC
        /// 공통
        /// </summary>
        /// <param name="strFilePath"></param>
        private void mfParsing_TOC_05(string strFilePath)
        {
            try
            {
                // RowIndex : 10
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    
                }
            }
            catch (Exception ex)
            {
                mfAddGridMessage(ex.ToString());
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
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
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
                mfAddGridMessage(ex.ToString());
                throw ex;
            }
        }
        #endregion File Parsing
        #endregion Method
    }
}
