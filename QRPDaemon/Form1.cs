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
                DataSet dsData = mfReadFile(fn_FileSelect(), 0);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
                }
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
                DataSet dsData = mfReadFile(fn_FileSelect(), 2);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
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
                DataSet dsData = mfReadFile(fn_FileSelect(), 10);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    ultraGrid1.SetDataBinding(dsData, dsData.Tables[0].TableName);
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
            mfParsing_Anion_05(fn_FileSelect());
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
                        string strSampleDate;
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
    }
}
