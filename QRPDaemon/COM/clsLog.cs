using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRPDaemon.COM
{
    public class Log
    {
        /// <summary>
        /// Log 기록 메소드
        /// </summary>
        /// <param name="strLogPath">로그폴더경로</param>
        /// <param name="strKey">키</param>
        /// <param name="strIFDateTime">인터페이스시간</param>
        /// <param name="strMassge">메세지</param>
        public void mfWriteLog(string strLogPath, string strKey, string strIFDateTime, string strMassge)
        {
            try
            {
                string m_strLogPrefix = strLogPath;
                string m_strLogExt = @".LOG";
                string strDateMonth = DateTime.Now.ToString("yyyyMM") + @"\";
                string strDateNow = DateTime.Now.ToString("yyyyMMdd");
                string strPath = string.Format("{0}{1}{2}", m_strLogPrefix + strDateMonth, strDateNow + "-" + strKey, m_strLogExt);
                string strDir = System.IO.Path.GetDirectoryName(strPath);
                string strSplit = "|";

                System.IO.DirectoryInfo diDir = new System.IO.DirectoryInfo(strDir);
                if (!diDir.Exists)
                {
                    diDir.Create();
                    diDir = new System.IO.DirectoryInfo(strDir);
                }

                if (diDir.Exists)
                {
                    lock (this)
                    {
                        using (System.IO.StreamWriter swStream = System.IO.File.AppendText(strPath))
                        {
                            string strLog = strIFDateTime + strSplit + strMassge;
                            swStream.WriteLine(strLog);
                            swStream.Flush();
                            swStream.Close();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Log 기록 메소드
        /// </summary>
        /// <param name="strLogPath">로그폴더경로</param>
        /// <param name="strIFDateTime">인터페이스시간</param>
        /// <param name="strMassge">메세지</param>
        public void mfWriteLog(string strLogPath, string strIFDateTime, string strMassge)
        {
            try
            {
                string m_strLogPrefix = strLogPath;
                string m_strLogExt = @".LOG";
                string strDateMonth = DateTime.Now.ToString("yyyyMM") + @"\";
                string strDateNow = DateTime.Now.ToString("yyyyMMdd");
                string strPath = string.Format("{0}{1}{2}", m_strLogPrefix, strDateNow, m_strLogExt);
                string strDir = System.IO.Path.GetDirectoryName(strPath);
                string strSplit = "|";

                System.IO.DirectoryInfo diDir = new System.IO.DirectoryInfo(strDir);
                if (!diDir.Exists)
                {
                    diDir.Create();
                    diDir = new System.IO.DirectoryInfo(strDir);
                }

                if (diDir.Exists)
                {
                    lock (this)
                    {
                        using (System.IO.StreamWriter swStream = System.IO.File.AppendText(strPath))
                        {
                            strMassge = strMassge.Replace(Environment.NewLine, " ");
                            string strLog = strIFDateTime + strSplit + strMassge;
                            swStream.WriteLine(strLog);
                            swStream.Flush();
                            swStream.Close();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Log 읽기 메소드
        /// </summary>
        /// <param name="strLogPath">로그폴더경로</param>
        /// <param name="strKey">키</param>
        /// <returns></returns>
        public string[] mfReadRow(string strLogPath, string strKey)
        {
            try
            {
                string m_strLogPrefix = strLogPath;
                string m_strLogExt = @".LOG";
                string strDateMonth = DateTime.Now.ToString("yyyyMM") + @"\";
                string strDateNow = DateTime.Now.ToString("yyyyMMdd");
                string strPath = string.Format("{0}{1}{2}", m_strLogPrefix + strDateMonth, strDateNow + "-" + strKey, m_strLogExt);

                //Log File 존재여부 체크
                if (!System.IO.File.Exists(strPath))
                    return new string[] { };

                lock (this)
                {
                    string[] strMessageLines = System.IO.File.ReadAllLines(strPath);
                    return strMessageLines;
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Log 읽기 메소드
        /// </summary>
        /// <param name="strLogPath">로그폴더경로</param>
        /// <returns></returns>
        public string[] mfReadRow(string strLogPath)
        {
            try
            {
                string m_strLogPrefix = strLogPath;
                string m_strLogExt = @".LOG";
                string strDateMonth = DateTime.Now.ToString("yyyyMM") + @"\";
                string strDateNow = DateTime.Now.ToString("yyyyMMdd");                
                string strPath = string.Format("{0}{1}{2}", m_strLogPrefix, strDateNow, m_strLogExt);

                //Log File 존재여부 체크
                if (!System.IO.File.Exists(strPath))
                    return new string[] { };

                lock (this)
                {
                    string[] strMessageLines = System.IO.File.ReadAllLines(strPath);
                    return strMessageLines;
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                throw ex;
            }
        }
    }
}
