﻿using CsvHelper;
using ExcelDataReader;
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

using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;

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
        }

        private void UltraButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ultraTextEditor1.Clear();
                StringBuilder sb = new StringBuilder();
                string strValue;
                string strSampleID;
                string strSampleDate;
                string strSampleDate_01;
                string strSampleDate_02;

                using (var reader = new StreamReader(fn_FileSelect()))
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
                        ultraGrid1.SetDataBinding(records, string.Empty);

                        ultraTextEditor1.Value = sb.ToString();
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
                ultraTextEditor1.Clear();

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
                using (var reader = new StreamReader(fn_FileSelect()))
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

                ultraGrid1.SetDataBinding(dtData, string.Empty);

                ultraTextEditor1.Value = sb.ToString();
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
                using (var stream = File.Open(fn_FileSelect(), FileMode.Open, FileAccess.Read))
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

                        // The result of each spreadsheet is in result.Tables

                        ultraGrid1.SetDataBinding(result, string.Empty);
                    }
                }
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
                using (var reader = new StreamReader(fn_FileSelect()))
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

                        ultraGrid1.SetDataBinding(records, string.Empty);
                    }
                }
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
            throw new NotImplementedException();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
        /// 울산 밀도계
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
                using (var stream = File.Open(strFilePath, FileMode.Open, FileAccess.Read))
                {
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
                                    //rowReader.Read();
                                    //rowReader.Read();
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
                DataSet dsData = mfReadFile(strFilePath, 4);
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
                                    }
                                }
                            });

                            return result;
                        }
                    }
                    else if (Path.GetExtension(strFilePath).ToUpper().Equals(".CSV") || Path.GetExtension(strFilePath).ToUpper().Equals(".REP") || Path.GetExtension(strFilePath).ToUpper().Equals(".TXT"))
                    {
                        using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
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
                                    }
                                }
                            });
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
                //this.mfAddGridMessage(ex.ToString());
                ultraTextEditor1.Text = ex.ToString();
                throw ex;
            }
        }
        #region 사용안함
        /*
        /// <summary>
        ///    Excel 파일을 DataTable 으로 변환하여 반환
        /// </summary>
        /// <param name="FileName">Excel File 명 PullPath</param>
        /// <param name="UseHeader">첫번째 줄을 Field 명으로 사용할 것이지 여부</param>
        public DataTable OpenExcel_DataTable(string FileName, bool UseHeader, string strSheetName = "")
        {
            //// 확장명 XLS (Excel 97~2003 용)
            //string ConnectStrFrm_Excel97_2003 =
            //    "Provider=Microsoft.Jet.OLEDB.4.0;" +
            //    "Data Source=\"{0}\";" +
            //    "Mode=ReadWrite|Share Deny None;" +
            //    "Extended Properties='Excel 8.0; HDR={1}; IMEX={2}';" +
            //    "Persist Security Info=False";

            // 확장명 XLSX (Excel 2007 이상용)
            string ConnectStrFrm_Excel =
                "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=\"{0}\";" +
                "Mode=ReadWrite|Share Deny None;" +
                "Extended Properties='Excel 12.0; HDR={1}; IMEX={2}';" +
                "Persist Security Info=False";

            string[] HDROpt = { "NO", "YES" };
            string HDR = "";
            string ConnStr = "";

            if (UseHeader)
                HDR = HDROpt[1];
            else
                HDR = HDROpt[0];

            int ExcelType = ExcelFileType(FileName);

            switch (ExcelType)
            {
                case (-2): throw new System.Exception(FileName + "의 형식검사중 오류가 발생하였습니다.");
                case (-1): throw new System.Exception(FileName + "은 엑셀 파일형식이 아닙니다.");
                case (0):
                    //ConnStr = string.Format(ConnectStrFrm_Excel97_2003, FileName, HDR, "1");
                    ConnStr = string.Format(ConnectStrFrm_Excel, FileName, HDR, "1");
                    break;
                case (1):
                    ConnStr = string.Format(ConnectStrFrm_Excel, FileName, HDR, "1");
                    break;
            }

            System.Data.OleDb.OleDbConnection excelConnection = null;
            System.Data.OleDb.OleDbCommand dbCommand = null;
            System.Data.OleDb.OleDbDataAdapter dataAdapter = null;

            DataTable dtExcelData = new DataTable();

            try
            {
                excelConnection = new System.Data.OleDb.OleDbConnection(ConnStr);
                excelConnection.Open();
                string strSQL;
                if (strSheetName.Equals(string.Empty))
                    strSQL = "select * from [" + strSheetName + "$]";
                else
                    strSQL = "select * from [" + strSheetName + "$]";
                dbCommand = new System.Data.OleDb.OleDbCommand(strSQL, excelConnection);
                dataAdapter = new System.Data.OleDb.OleDbDataAdapter(dbCommand);

                dataAdapter.Fill(dtExcelData);
            }
            catch (System.Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
            finally
            {
                dataAdapter.Dispose();
                dbCommand.Dispose();

                if (excelConnection != null)
                    excelConnection.Close();

                excelConnection.Dispose();
            }

            return dtExcelData;
        }

        /// <summary>
        ///    Excel 파일의 형태를 반환한다.
        ///    -2 : Error  
        ///    -1 : 엑셀파일아님
        ///     0 : 97-2003 엑셀 파일 (xls)
        ///     1 : 2007 이상 파일 (xlsx)
        /// </summary>
        /// <param name="XlsFile">Excel File 명 전체 경로</param>
        private static int ExcelFileType(string XlsFile)
        {
            byte[,] ExcelHeader = {
                { 0xD0, 0xCF, 0x11, 0xE0, 0xA1 }, // XLS  File Header
                { 0x50, 0x4B, 0x03, 0x04, 0x14 }  // XLSX File Header
            };

            // result -2=error, -1=not excel , 0=xls , 1=xlsx
            int result = -1;

            // 
            System.IO.FileStream FS = new System.IO.FileStream(XlsFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);

            try
            {
                byte[] FH = new byte[5];

                FS.Read(FH, 0, 5);

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (FH[j] != ExcelHeader[i, j]) break;
                        else if (j == 4) result = i;
                    }
                    if (result >= 0) break;
                }
            }
            catch
            {
                result = (-2);
            }
            finally
            {
                FS.Close();
            }
            return result;
        }
        */
        #endregion 사용안함

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
