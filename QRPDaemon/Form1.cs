using CsvHelper;
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
        }

        private void UltraButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ultraTextEditor1.Clear();

                //if (System.IO.File.Exists(CommonCode.EnvXMLFileName))
                //{
                //    #region Var
                //    System.Xml.XmlDocument xmlDocHeader = new System.Xml.XmlDocument();
                //    System.Xml.XmlNode node;
                //    #endregion Var

                //    // 기본 XML 파일 읽기
                //    xmlDocHeader.Load(CommonCode.EnvXMLFileName);

                //    node = xmlDocHeader.SelectSingleNode("Files/File").Attributes()
                //}
                StringBuilder sb = new StringBuilder();
                string strValue;
                using (var reader = new StreamReader(fn_FileSelect()))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //csv.Configuration.HasHeaderRecord = false;
                        //csv.Configuration.IgnoreBlankLines = false;

                        csv.Configuration.HasHeaderRecord = false;

                        while (csv.Read())
                        {
                            sb.AppendFormat("{0, 3} : {1}", csv.Context.Row, csv.Context.RawRecord);
                            
                            //if (csv.TryGetField<string>(0, out strValue))
                            //{
                            //    switch (strValue)
                            //    {
                            //        case "Sample ID:":
                            //            break;
                            //    }
                            //}
                        }

                        ultraTextEditor1.Value = sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
            }
        }

        public class Cation
        {
            public string IS { get; set; }
        }

        private void UltraButton2_Click(object sender, EventArgs e)
        {
            try
            {
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

            }
            catch (Exception ex)
            {
                ultraTextEditor1.Value = ex.ToString();
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
    }
}
