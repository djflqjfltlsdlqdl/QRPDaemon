using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRPDaemon.COM;
using System.IO;

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;

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
        private string m_strTargetFilePath = string.Empty;
        private string m_strIntervar = string.Empty;
        private string m_strMeasureName = string.Empty;
        private string m_strFileExtension = string.Empty;

        private DateTime m_strDateCheck = new DateTime();
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
            get { return m_strPlantCode; }
            set { m_strPlantCode = value; }
        }
        /// <summary>
        /// 원본파일경로
        /// </summary>
        public string OriginFilePath
        {
            get { return m_strOriginFilePath; }
            set { m_strOriginFilePath = value; }
        }
        /// <summary>
        /// 복사경로
        /// </summary>
        public string TargetFilePath
        {
            get { return m_strTargetFilePath; }
            set { m_strTargetFilePath = value; }
        }
        /// <summary>
        /// 주기
        /// </summary>
        public string Intervar
        {
            get { return m_strIntervar; }
            set { m_strIntervar = value; }
        }
        /// <summary>
        /// 실험기기명
        /// </summary>
        public string MeasureName
        {
            get { return m_strMeasureName; }
            set { m_strMeasureName = value; }
        }
        /// <summary>
        /// 파일 확장자
        /// </summary>
        public string FileExtension
        {
            get { return m_strFileExtension; }
            set { m_strFileExtension = value; }
        }
        #endregion Var

        public ucFile()
        {
            InitializeComponent();

            this.Load += UcFile_Load;

            this.btnStart.Click += new EventHandler(btnStart_Click);
            this.btnStop.Click += new EventHandler(btnStop_Click);
            this.txtInterval.KeyDown += new KeyEventHandler(txtInterval_KeyDown);
        }

        private void UcFile_Load(object sender, EventArgs e)
        {
            m_strDateCheck = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            InitControl();
            mfReadLogData();
        }

        #region Initialize

        private void InitControl()
        {
            tsStatusLabel.Text = string.Empty;
            btnStop.Enabled = false;

            // 주기
            txtInterval.Text = Intervar;

            #region DataGridView
            dgvLogList.ReadOnly = true;
            dgvLogList.AllowUserToAddRows = false;
            dgvLogList.AllowUserToDeleteRows = false;
            dgvLogList.RowHeadersVisible = false;

            dgvLogList.Columns.Add("IFDateTime", "IF일시");
            dgvLogList.Columns.Add("IFMessage", "IF처리메세지");

            dgvLogList.Columns["IFMessage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            #endregion
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
                if (!this.m_bolProgFlag)
                {
                    //if (m_thread == null || !m_thread.IsAlive)
                    //{
                    //    m_thread = new System.Threading.Thread(this.mfDoWork);
                    //    this.m_bolProgFlag = true;
                    //    m_thread.Start();
                    //}
                    this.m_bolProgFlag = true;
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

                if (m_strDateCheck.AddHours(24) < DateTime.Now)
                {
                    this.m_strDateCheck = this.m_strDateCheck.AddDays(1);
                    dgvLogList.Rows.Clear();
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
        /// 작업
        /// </summary>
        private async Task mfDoWork()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (this.m_bolProgFlag)
                    {
                        mfSetToolStripStatusLabel("Start...");

                        for(int i=0; i<5; i++)
                        {
                            System.Threading.Thread.Sleep(5000);
                            mfAddGridMessage(string.Format("처리완료{0}", i));
                        }

                        System.IO.DirectoryInfo diInfo = new System.IO.DirectoryInfo(this.OriginFilePath);
                        if (diInfo.Exists)
                        {
                            DateTime Now = DateTime.Now;
                            if (PlantCode.Equals("03"))
                                Now = Now - Properties.Settings.Default.StartTime_03;
                            else if (PlantCode.Equals("05"))
                                Now = Now - Properties.Settings.Default.StartTime_05;

                            string strTargetPath = string.Format(@"{0}\{1}\{2}", this.TargetFilePath, Now.ToString("yyyy-MM-dd"), MeasureName);
                            System.IO.DirectoryInfo diTarget = new System.IO.DirectoryInfo(strTargetPath);
                            if (!diTarget.Exists)
                                diTarget.Create();

                            System.IO.FileInfo[] getFiles = diInfo.GetFiles();
                            foreach (System.IO.FileInfo fi in getFiles)
                            {
                                if (fi.Extension.Equals(this.FileExtension))
                                {
                                    if (PlantCode.Equals("03"))
                                    {
                                        switch (MeasureName)
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
                                    else if(PlantCode.Equals("05"))
                                    {
                                        //switch (MeasureName)
                                        //{
                                        //    case "밀도계":
                                        //        mfParsing_Density(fi.FullName);
                                        //        break;
                                        //    case "TOC":
                                        //        mfParsing_TOC(fi.FullName);
                                        //        break;
                                        //    case "음이온":
                                        //        mfParsing_Anion(fi.FullName);
                                        //        break;
                                        //    case "양이온":
                                        //        mfParsing_Cation(fi.FullName);
                                        //        break;
                                        //}
                                    }
                                }
                                fi.MoveTo(System.IO.Path.Combine(strTargetPath, fi.Name));
                            }
                        }
                        else
                        {
                            this.mfAddGridMessage("파일경로 에러!");
                        }

                        mfSetToolStripStatusLabel("처리 완료..." + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                catch (System.Exception ex)
                {
                    this.mfAddGridMessage(ex.ToString());
                }
                finally
                {
                    this.m_bolProgFlag = false;
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
                if (this.m_timerMain == null)
                    m_timerMain = new Timer();
                if (this.m_timerSub == null)
                    m_timerSub = new Timer();

                this.m_timerMain.Tick += new EventHandler(m_timerMain_Tick);
                this.m_timerSub.Tick += new EventHandler(m_timerSub_Tick);

                this.m_timerSub.Interval = 1000;

                int intInterval = Convert.ToInt32(this.txtInterval.Text);
                this.m_timerMain.Interval = intInterval * 1000 * 60;

                this.tsStatusLabel.Text = "시작...";

                this.m_timerMain.Start();
                this.m_timerSub.Start();

                if (!this.m_bolProgFlag)
                {
                    this.m_bolProgFlag = true;
                    Task work = mfDoWork();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.mfAddGridMessage(ex.ToString());
            }
            finally
            {
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
                this.m_timerMain.Tick -= new EventHandler(m_timerMain_Tick);
                this.m_timerSub.Tick -= new EventHandler(m_timerSub_Tick);

                this.m_bolProgFlag = false;
                this.m_timerMain.Stop();
                this.m_timerSub.Stop();

                this.m_timerMain = null;
                this.m_timerSub = null;

                lbInterval.BackColor = System.Drawing.Color.Red;
                tsStatusLabel.Text = "중지...";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        #region Invoke
        /// <summary>
        /// Log리스트 Invoke Delegate
        /// </summary>
        /// <param name="Messages"></param>
        delegate void SetGridCallBack(string Messages);
        /// <summary>
        /// Lot리스트 기록메소드
        /// </summary>
        /// <param name="strMessageLine"></param>
        private void mfAddGridMessage(string strMessage)
        {
            try
            {
                if (this.dgvLogList.InvokeRequired)
                {
                    SetGridCallBack cb = new SetGridCallBack(mfAddGridMessage);
                    this.dgvLogList.Invoke(cb, new object[] { strMessage });
                }
                else
                {
                    string strDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    this.dgvLogList.Rows.Add(strDateTime, strMessage);

                    // 바인딩후 가장 마지막 행으로 스크롤 이동
                    this.dgvLogList.FirstDisplayedScrollingRowIndex = this.dgvLogList.Rows.Count - 1;

                    new Log().mfWriteLog(CommonCode.CreateLogPathName, m_strMeasureName, strDateTime, strMessage);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 상태창 Invoke Delegate
        /// </summary>
        /// <param name="strText"></param>
        delegate void SetStatusLabelCallBack(string strText);
        /// <summary>
        /// 상태창 Text 변경 메소드
        /// </summary>
        /// <param name="strMessage"></param>
        private void mfSetToolStripStatusLabel(string strMessage)
        {
            try
            {
                if (this.stStrip.InvokeRequired)
                {
                    SetStatusLabelCallBack cb = new SetStatusLabelCallBack(mfSetToolStripStatusLabel);
                    this.stStrip.Invoke(cb, new object[] { strMessage });
                }
                else
                {
                    this.stStrip.Items["tsStatusLabel"].Text = strMessage;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

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
                    this.dgvLogList.Rows.Add(strMessageLine[0], strMessageLine[1]);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region File Parsing
        /// <summary>
        /// 양이온
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Cation_03(string strFilePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string strValue;
                string strSampleID;
                string strSampleDate;
                string strSampleDate_01;
                string strSampleDate_02;

                using (var reader = new StreamReader(strFilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {
                            sb.AppendFormat("{0, 3} : {1}", csv.Context.Row, csv.Context.RawRecord);

                            if (csv.TryGetField<string>(0, out strValue))
                            {
                                if (strValue.Equals("Sample ID:"))
                                {
                                    csv.TryGetField<string>(1, out strSampleID);
                                }
                                else if (strValue.Equals("Sample Date/Time:"))
                                {
                                    csv.TryGetField<string>(2, out strSampleDate_01);
                                    csv.TryGetField<string>(3, out strSampleDate_02);
                                    strSampleDate = strSampleDate_01 + strSampleDate_02;
                                    DateTime dtSampleDate;
                                    DateTime.TryParse(strSampleDate, out dtSampleDate);
                                }
                                else if (strValue.StartsWith("Results"))
                                {
                                    csv.Read();
                                    csv.ReadHeader();
                                    break;
                                }
                            }
                        }
                        csv.Configuration.RegisterClassMap<CationMapper>();
                        var records = csv.GetRecords<Cation>().ToList();
                        foreach (var dd in records)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 음이온
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Anion_03(string strFilePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                DataTable dtData = new DataTable();
                dtData.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("InspectDate", typeof(DateTime))
                    , new DataColumn("SampleName", typeof(string))
                    , new DataColumn("Info", typeof(string))
                    , new DataColumn("F", typeof(decimal))
                    , new DataColumn("CI", typeof(decimal))
                    , new DataColumn("NO2", typeof(decimal))
                    , new DataColumn("Br", typeof(decimal))
                    , new DataColumn("NO3", typeof(decimal))
                    , new DataColumn("PO4", typeof(decimal))
                    , new DataColumn("SO4", typeof(decimal))
                });
                DataRow dr;
                using (var reader = new StreamReader(strFilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.Delimiter = "\t";
                        csv.Configuration.MissingFieldFound = null;
                        csv.Configuration.IgnoreBlankLines = true;

                        csv.Configuration.RegisterClassMap<AnionMapper>();

                        var records = csv.GetRecords<Anion>().ToList();

                        foreach (var dd in records)
                        {
                            if (dd.InspectDate.Trim().Length.Equals(0))
                            {
                                continue;
                            }

                            dr = dtData.NewRow();
                            dr["InspectDate"] = dd.InspectDate.Substring(0, 19);
                            dr["SampleName"] = dd.SampleName;
                            dr["Info"] = dd.Info;
                            dr["F"] = dd.F.HasValue ? (object)dd.F.Value : DBNull.Value;
                            dr["CI"] = dd.CI.HasValue ? (object)dd.CI.Value : DBNull.Value;
                            dr["NO2"] = dd.NO2.HasValue ? (object)dd.NO2.Value : DBNull.Value;
                            dr["Br"] = dd.Br.HasValue ? (object)dd.Br.Value : DBNull.Value;
                            dr["NO3"] = dd.NO3.HasValue ? (object)dd.NO3.Value : DBNull.Value;
                            dr["PO4"] = dd.PO4.HasValue ? (object)dd.PO4.Value : DBNull.Value;
                            dr["SO4"] = dd.SO4.HasValue ? (object)dd.SO4.Value : DBNull.Value;
                            dtData.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 밀도계
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Density_03(string strFilePath)
        {
            try
            {
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        //// 1. Use the reader methods
                        //do
                        //{
                        //    while (reader.Read())
                        //    {
                        //        // reader.GetDouble(0);
                        //    }
                        //} while (reader.NextResult());

                        // 2. Use the AsDataSet extension method
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            // Gets or sets a value indicating whether to set the DataColumn.DataType 
                            // property in a second pass.
                            UseColumnDataType = true,

                            // Gets or sets a callback to determine whether to include the current sheet
                            // in the DataSet. Called once per sheet before ConfigureDataTable.
                            FilterSheet = (tableReader, sheetIndex) => true,

                            // Gets or sets a callback to obtain configuration options for a DataTable. 
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                // Gets or sets a value indicating the prefix of generated column names.
                                EmptyColumnNamePrefix = "Column",

                                // Gets or sets a value indicating whether to use a row from the 
                                // data as column names.
                                UseHeaderRow = true,

                                // Gets or sets a callback to determine which row is the header row. 
                                // Only called when UseHeaderRow = true.
                                ReadHeaderRow = (rowReader) => {
                                    // F.ex skip the first row and use the 2nd row as column headers:
                                    rowReader.Read();
                                    rowReader.Read();
                                },

                                // Gets or sets a callback to determine whether to include the 
                                // current row in the DataTable.
                                FilterRow = (rowReader) => {
                                    return true;
                                },

                                // Gets or sets a callback to determine whether to include the specific
                                // column in the DataTable. Called once per column after reading the 
                                // headers.
                                FilterColumn = (rowReader, columnIndex) => {
                                    return true;
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// TOC
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_TOC_03(string strFilePath)
        {
            try
            {
                using (var reader = new StreamReader(strFilePath))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //csv.Configuration.HasHeaderRecord = false;
                        //csv.Configuration.RegisterClassMap<CationMapper>();
                        //var records = csv.GetRecords<CationMapper>().ToList();
                        //foreach (var dd in records)
                        //{

                        //}
                        csv.Configuration.MissingFieldFound = null;
                        csv.Configuration.IgnoreBlankLines = true;

                        csv.Configuration.RegisterClassMap<CationMapper>();

                        var records = csv.GetRecords<Cation>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                this.mfAddGridMessage(ex.ToString());
            }
        }
        /// <summary>
        /// 밀도계
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Density_05(string strFilePath)
        {
            try
            {
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // 2. Use the AsDataSet extension method
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            // Gets or sets a value indicating whether to set the DataColumn.DataType 
                            // property in a second pass.
                            UseColumnDataType = true,

                            // Gets or sets a callback to determine whether to include the current sheet
                            // in the DataSet. Called once per sheet before ConfigureDataTable.
                            FilterSheet = (tableReader, sheetIndex) => true,

                            // Gets or sets a callback to obtain configuration options for a DataTable. 
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                // Gets or sets a value indicating the prefix of generated column names.
                                EmptyColumnNamePrefix = "Column",

                                // Gets or sets a value indicating whether to use a row from the 
                                // data as column names.
                                UseHeaderRow = true,

                                // Gets or sets a callback to determine which row is the header row. 
                                // Only called when UseHeaderRow = true.
                                ReadHeaderRow = (rowReader) => {
                                    // F.ex skip the first row and use the 2nd row as column headers:
                                    rowReader.Read();
                                    rowReader.Read();
                                },

                                // Gets or sets a callback to determine whether to include the 
                                // current row in the DataTable.
                                FilterRow = (rowReader) => {
                                    return true;
                                },

                                // Gets or sets a callback to determine whether to include the specific
                                // column in the DataTable. Called once per column after reading the 
                                // headers.
                                FilterColumn = (rowReader, columnIndex) => {
                                    return true;
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.mfAddGridMessage(ex.ToString());
            }
        }
        #endregion File Parsing
        #endregion Method

        #region 양이온
        [Serializable]
        public class Cation
        {
            public string IS { get; set; }
            public string Analyte { get; set; }
            public string Mass { get; set; }
            public string Conc { get; set; }
            public string Unit { get; set; }
            public string SD { get; set; }
            public string RSD1 { get; set; }
            public string Intensity { get; set; }
            public string RSD2 { get; set; }
            public string BlankIntens { get; set; }
            public string Mode { get; set; }
        }

        public class CationMapper : ClassMap<Cation>
        {
            public CationMapper()
            {
                //Map(m => m.IS).Name("IS").Index(0);
                //Map(m => m.Analyte).Name("Analyte").Index(1);
                //Map(m => m.Mass).Name("Mass").Index(2);
                //Map(m => m.Conc).Name("Conc.").Index(3);
                //Map(m => m.Unit).Name("Unit").Index(4);
                //Map(m => m.SD).Name("SD").Index(5);
                //Map(m => m.RSD1).Name("RSD").Index(6);
                //Map(m => m.Intensity).Name("Intensity").Index(7);
                //Map(m => m.RSD2).Name("RSD1").Index(8);
                //Map(m => m.BlankIntens).Name("Blank Intens.").Index(9);
                //Map(m => m.Mode).Name("Mode").Index(10);

                //Map(m => m.IS).Index(0).Name("Analyte");
                //Map(m => m.Analyte).Index(1).Name("Mass");
                //Map(m => m.Mass).Index(2).Name("Conc.");
                //Map(m => m.Conc).Index(3).Name("Unit");
                //Map(m => m.Unit).Index(4).Name("SD");
                //Map(m => m.SD).Index(5).Name("RSD");
                //Map(m => m.RSD1).Index(6).Name("Intensity");
                //Map(m => m.Intensity).Index(7).Name("RSD");
                //Map(m => m.RSD2).Index(8).Name("Blank Intens.");
                //Map(m => m.BlankIntens).Index(9).Name("Mode");
                //Map(m => m.Mode).Index(10);

                Map(m => m.IS).Index(0);
                Map(m => m.Analyte).Index(1);
                Map(m => m.Mass).Index(2);
                Map(m => m.Conc).Index(3);
                Map(m => m.Unit).Index(4);
                Map(m => m.SD).Index(5);
                Map(m => m.RSD1).Index(6);
                Map(m => m.Intensity).Index(7);
                Map(m => m.RSD2).Index(8);
                Map(m => m.BlankIntens).Index(9);
                Map(m => m.Mode).Index(10);
            }
        }
        #endregion 양이온

        #region 음이온
        [Serializable]
        public class Anion
        {
            public string InspectDate { get; set; }
            public string SampleName { get; set; }
            public string Info { get; set; }
            public decimal? F { get; set; }
            public decimal? CI { get; set; }
            public decimal? NO2 { get; set; }
            public decimal? Br { get; set; }
            public decimal? NO3 { get; set; }
            public decimal? PO4 { get; set; }
            public decimal? SO4 { get; set; }
        }
        //public class Anion
        //{
        //    public string InspectDate { get; set; }
        //    public string SampleName { get; set; }
        //    public string Info { get; set; }
        //    public string F { get; set; }
        //    public string CI { get; set; }
        //    public string NO2 { get; set; }
        //    public string Br { get; set; }
        //    public string NO3 { get; set; }
        //    public string PO4 { get; set; }
        //    public string SO4 { get; set; }
        //}

        public class AnionMapper : ClassMap<Anion>
        {
            public AnionMapper()
            {
                Map(m => m.InspectDate).Name("Determination start").Index(0);
                Map(m => m.SampleName).Name("Ident").Index(1);
                Map(m => m.Info).Name("Info 1").Index(2);
                Map(m => m.F).Name("Anoin.F.Concentration").Index(3);
                Map(m => m.CI).Name("Anoin.Cl.Concentration").Index(4);
                Map(m => m.NO2).Name("Anoin.NO2.Concentration").Index(5);
                Map(m => m.Br).Name("Anoin.Br.Concentration").Index(6);
                Map(m => m.NO3).Name("Anoin.NO3.Concentration").Index(7);
                Map(m => m.PO4).Name("Anoin.PO4.Concentration").Index(8);
                Map(m => m.SO4).Name("Anoin.SO4.Concentration").Index(9);
            }
        }
        #endregion 음이온
    }
}
