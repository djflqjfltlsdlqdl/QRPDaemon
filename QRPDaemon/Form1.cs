using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

using ExcelDataReader;
using QRPDaemon.COM;

namespace QRPDaemon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ultraButton1.Click += UltraButton1_Click;
            ultraButton2.Click += UltraButton2_Click;
            ultraButton3.Click += UltraButton3_Click;
            ultraButton4.Click += UltraButton4_Click;

            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            button4.Click += Button4_Click;
            button5.Click += Button5_Click;
            button6.Click += Button6_Click;

            btnNetwork.Click += BtnNetwork_Click;
        }

        private void UltraButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (var stream = File.Open(fn_FileSelect(), FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                    {
                        string strValue;
                        string strSampleID;
                        string strSampleDate;
                        DateTime dateSampleDate;
                        do
                        {
                            while (reader.Read())
                            {
                                strValue = reader.GetString(0);
                                switch(strValue)
                                {
                                    case "Sample ID:":
                                        strSampleID = reader.GetString(1);
                                        break;
                                    case "Sample Date/Time:":
                                        strSampleDate = string.Format("{0} {1}", reader.GetValue(2), reader.GetValue(3));
                                        DateTime.TryParse(strSampleDate, out dateSampleDate);
                                        break;
                                }

                                if (reader.Depth.Equals(18))
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
                                    for (int i = 0; i < 18; i++)
                                    {
                                        rowReader.Read();
                                    }
                                }
                            }
                        });
                        
                        ultraGrid1.SetDataBinding(result, string.Empty);
                    }

                }
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
        }

        private void UltraButton2_Click(object sender, EventArgs e)
        {
            try
            {
                string strFilePath = fn_FileSelect();
                mfParsing_Anion_03(strFilePath);
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
        }

        private void UltraButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string strFilePath = fn_FileSelect();
                mfParsing_Density_03(strFilePath);
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
        }

        private void UltraButton4_Click(object sender, EventArgs e)
        {
            try
            {
                string strFilePath = fn_FileSelect();
                mfParsing_TOC_03(strFilePath);
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            mfParsing_Density_05(fn_FileSelect());
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            mfParsing_Cation_05_7900(fn_FileSelect());
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            mfParsing_Cation_05_M90(fn_FileSelect());
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            mfParsing_Cation_05_OES(fn_FileSelect());
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            mfParsing_Anion_05(fn_FileSelect());
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            mfParsing_TOC_05(fn_FileSelect());
        }

        private void BtnNetwork_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string strNetworkFullPath = @"\\192.168.16.7\23.H202_Data\1._밀도계";
            SharedDirectory sd = new SharedDirectory();
            try
            {
                int intErrCode = sd.mfConnectNetworkDrive(strNetworkFullPath, "511208", "511208p");
                if (intErrCode.Equals(0))
                {
                    sb.AppendLine("네트워크 연결 성공!");
                    System.IO.DirectoryInfo diInfo = new DirectoryInfo(strNetworkFullPath);
                    if (diInfo.Exists)
                    {
                        System.IO.FileInfo[] fiFiles = diInfo.GetFiles();
                        foreach (System.IO.FileInfo fi in fiFiles)
                        {
                            sb.AppendLine(fi.Name);
                        }
                    }
                    else
                    {
                        sb.AppendLine("폴더가 존재하지 않습니다!");
                    }
                }
                else
                {
                    sb.AppendLine("네트워크 경로 에러!");
                    sb.AppendLine(string.Format("{0} : {1}", intErrCode, sd.mfGetConnectErrorMessage(intErrCode)));
                }
                ultraTextEditor1.Value = sb.ToString();
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
            finally
            {
                sd.mfDisconnectNetworkDrive(strNetworkFullPath);
            }
        }

        private string fn_FileSelect(string strFilter = null)
        {
            try
            {
                using (OpenFileDialog file = new OpenFileDialog())
                {
                    file.Filter = strFilter ?? "All files (*.*)|*.*|Word Documents(*.doc;*.docx)|*.doc;*.docx|Excel Worksheets(*.xls;*.xlsx)|*.xls;*.xlsx|PowerPoint Presentations(*.ppt;*.pptx)|*.ppt;*.pptx|Office Files(*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx)|*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx|Text Files(*.txt)|*.txt|Portable Document Format Files(*.pdf)|*.pdf|Image Files|*.jpg;*.jpeg;*.bmp;*.gif;*.png;*.tiff;*.ico;*.raw;*.pcx";
                    file.RestoreDirectory = true;
                    if (file.ShowDialog().Equals(DialogResult.OK))
                    {
                        return file.FileName;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
                return null;
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
                string m_strPlantCode = "03";
                string m_strProcessGroupCode = "50";
                string m_strInspectTypeCode = "";
                string m_strBackupFilePath = "BackupFilePath";
                string m_strMeasureName = "음이온";
                int m_intRowIndex = 0;
                
                // RowIndex : 0
                DataSet dsData = mfReadFile(strFilePath, m_intRowIndex);

                DataSet dsFile = GetSaveDefaultDataSet();
                DateTime dateSampleDate = DateTime.MaxValue;

                DataRow dr;
                int intBatchIndex = 1;
                int intRowCount = dsData.Tables[0].Rows.Count;

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
                    ultraTextEditor1.Text = strErrRtn;
                    dateSampleDate = DateTime.MaxValue;
                }
                else
                {
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
            string m_strPlantCode = "03";
            string m_strProcessGroupCode = "50";
            string m_strBackupFilePath = "BackupFilePath";
            string m_strMeasureName = "밀도계";
            try
            {
                // RowIndex : 2
                DataSet dsData = mfReadFile(strFilePath, 2);

                DataSet dsFile = GetSaveDefaultDataSet();
                //var vDataRow = dsData.Tables[0].AsEnumerable().Where(w => !w.Field<string>("Unique Sample Id").Equals(string.Empty) && !w.Field<string>("Sample Name").Equals(string.Empty));
                DateTime dateSampleDate = DateTime.MaxValue;

                DataRow dr;
                int intBatchIndex = 1;
                int intRowCount = dsData.Tables[0].Rows.Count;

                for (int i = 0; i < intRowCount; i++)
                {
                    if (!dsData.Tables[0].Rows[i]["Unique Sample Id"].ToString().Equals(string.Empty) && !dsData.Tables[0].Rows[i]["Sample Name"].ToString().Equals(string.Empty))
                    {
                        DateTime.TryParse(dsData.Tables[0].Rows[i]["Date"].ToString(), out dateSampleDate);
                        dr = dsFile.Tables["H"].NewRow();
                        dr["BatchID"] = 0;
                        dr["PlantCode"] = "03";
                        dr["ProcessGroupCode"] = "50";
                        dr["InspectTypeCode"] = "";
                        dr["SampleName"] = dsData.Tables[0].Rows[i]["Sample Name"];
                        dr["SampleDate"] = dsData.Tables[0].Rows[i]["Date"];
                        dr["BatchIndex"] = intBatchIndex;
                        dsFile.Tables["H"].Rows.Add(dr);

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
                    ultraTextEditor1.Text = strErrRtn;
                    dateSampleDate = DateTime.MaxValue;
                }
                else
                {
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
        /// 공통
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private DateTime mfParsing_TOC_03(string strFilePath)
        {
            int m_intRowIndex = 21;
            string m_strPlantCode = "03";
            string m_strProcessGroupCode = "50";
            string m_strInspectTypeCode = "";
            string m_strBackupFilePath = string.Empty;
            string m_strMeasureName = string.Empty;

            try
            {

                DateTime dateSampleDate = DateTime.MaxValue;
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
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

                        // Gets or sets an array of CSV separator candidates. The reader 
                        // autodetects which best fits the input data. Default: , ; TAB | # 
                        // (CSV only)
                        AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },

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
                        string strSampleDate;
                        bool bolBreak = false;
                        do
                        {
                            while (reader.Read())
                            {
                                string strvalue = reader.GetString(0) ?? "";
                                switch (strvalue)
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
                                },
                                FilterRow = rowReader =>
                                {
                                    var hasData = true;
                                    switch(rowReader.GetString(0))
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
                            ultraTextEditor1.Text = strErrRtn;
                            dateSampleDate = DateTime.MaxValue;
                        }
                        else
                        {
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
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Density_05(string strFilePath)
        {
            try
            {
                DataSet dsData = mfReadFile(strFilePath, 2);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
                }
            }
            catch (Exception ex)
            {
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
            }
        }
        /// <summary>
        /// 울산 ICP-MS(7900)
        /// </summary>
        /// <param name="strFilePath">파일경로</param>
        private void mfParsing_Cation_05_7900(string strFilePath)
        {
            try
            {
                DataSet dsData = mfReadFile(strFilePath, 0);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
                }
            }
            catch (Exception ex)
            {
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
            }
        }
        /// <summary>
        /// 울산 ICP-MS(A-M90)
        /// </summary>
        /// <param name="strFilePath"></param>
        private void mfParsing_Cation_05_M90(string strFilePath)
        {
            try
            {
                DataSet dsData = mfReadFile(strFilePath, 2);
                if(dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
                }
            }
            catch (Exception ex)
            {
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
            }
        }

        /// <summary>
        /// 울산 ICP-OES
        /// </summary>
        /// <param name="strFilePath"></param>
        private void mfParsing_Cation_05_OES(string strFilePath)
        {
            try
            {
                DataSet dsData = mfReadFile(strFilePath, 2);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
                }
            }
            catch (Exception ex)
            {
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
            }
        }

        /// <summary>
        /// 울산 ICP-MS(A-M90)
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
                                        //DateTime.TryParse(strSampleDate, out dateSampleDate);
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
                                    for (int i = 0; i < 26; i++)
                                    {
                                        rowReader.Read();
                                    }
                                }
                            }
                        });

                        ultraGrid1.SetDataBinding(result, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
            }
        }

        /// <summary>
        /// 울산 TOC
        /// </summary>
        /// <param name="strFilePath"></param>
        private void mfParsing_TOC_05(string strFilePath)
        {
            try
            {
                DataSet dsData = mfReadFile(strFilePath, 10);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
                }
            }
            catch (Exception ex)
            {
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
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
                //logger.Error($"Error In mfReadFile : {strFilePath}", ex);
                throw (ex);
            }
        }

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
    }
}
