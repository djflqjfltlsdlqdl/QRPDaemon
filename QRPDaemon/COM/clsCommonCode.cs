using System;
using System.Runtime.Serialization;

namespace QRPDaemon.COM
{
    public static class CommonCode
    {
        private static string m_strEnvXMLFileName = string.Empty;
        private static string m_strExecutablePath = string.Empty;
        private static string m_strDefaultXMLPath = string.Empty;
        private static string m_strMeasureDataXMLPath = string.Empty;
        private static string m_strCreateXMLFilePath = string.Empty;
        private static string m_strCreateLogPathName = string.Empty;

        /// <summary>
        /// 환경설정 XML 파일경로
        /// </summary>
        public static string EnvXMLFileName
        {
            get { return CommonCode.m_strEnvXMLFileName; }
            set { CommonCode.m_strEnvXMLFileName = value; }
        }

        /// <summary>
        /// 프로그램 실행경로
        /// </summary>
        public static string ExecutablePath
        {
            get { return CommonCode.m_strExecutablePath; }
            set { CommonCode.m_strExecutablePath = value; }
        }

        /// <summary>
        /// XML 생성용 기본 XML 파일경로
        /// </summary>
        public static string DefaultXMLPath
        {
            get { return CommonCode.m_strDefaultXMLPath; }
            set { CommonCode.m_strDefaultXMLPath = value; }
        }

        /// <summary>
        /// 측정데이터 기본 XML 파일경로
        /// </summary>
        public static string MeasureDataXMLPath
        {
            get { return CommonCode.m_strMeasureDataXMLPath; }
            set { CommonCode.m_strMeasureDataXMLPath = value; }
        }

        /// <summary>
        /// XML 파일생성 경로
        /// </summary>
        public static string CreateXMLFilePath
        {
            get { return CommonCode.m_strCreateXMLFilePath; }
            set { CommonCode.m_strCreateXMLFilePath = value; }
        }

        /// <summary>
        /// XML 파일생성 Log 경로
        /// </summary>
        public static string CreateLogPathName
        {
            get { return CommonCode.m_strCreateLogPathName; }
            set { CommonCode.m_strCreateLogPathName = value; }
        }

        static CommonCode()
        {
            System.IO.FileInfo exeFileInfo = new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath);
            CommonCode.m_strEnvXMLFileName = @"XML\FileSetting.xml";
            CommonCode.m_strCreateLogPathName = exeFileInfo.Directory.FullName.ToString() + @"\Logs\";
            CommonCode.m_strCreateXMLFilePath = exeFileInfo.Directory.FullName.ToString() + @"\RESULT\";
            CommonCode.m_strDefaultXMLPath = @"XML\Default.xml";
            CommonCode.m_strMeasureDataXMLPath = @"XML\MeasureData.xml";
        }
    }

    #region 트랜잭션 처리시 Return 처리
    public class TransErrRtn
    {
        private int intErrNum;              //에러번호
        private string strErrMessage;       //에러메세지    
        private System.Collections.ArrayList arrReturnValue = new System.Collections.ArrayList();   //반환할 결과값
        private string strSystemMessage;
        private string strSystemStackTrace;
        private string strSystemInnerException;
        private string strInterfaceResultCode;
        private string strInterfaceResultMessage;

        public int ErrNum
        {
            get { return intErrNum; }
            set { intErrNum = value; }
        }

        public string ErrMessage
        {
            get { return strErrMessage; }
            set { strErrMessage = value; }
        }

        public string SystemMessage
        {
            get { return strSystemMessage; }
            set { strSystemMessage = value; }
        }

        public string SystemStackTrace
        {
            get { return strSystemStackTrace; }
            set { strSystemStackTrace = value; }
        }

        public string SystemInnerException
        {
            get { return strSystemInnerException; }
            set { strSystemInnerException = value; }
        }

        public string InterfaceResultCode
        {
            get { return strInterfaceResultCode; }
            set { strInterfaceResultCode = value; }
        }

        public string InterfaceResultMessage
        {
            get { return strInterfaceResultMessage; }
            set { strInterfaceResultMessage = value; }
        }

        public TransErrRtn()
        {
            intErrNum = 0;
            strErrMessage = "";
            arrReturnValue.Clear();
            strSystemMessage = "";
            strSystemStackTrace = "";
            strSystemInnerException = "";
            strInterfaceResultCode = "";
            strInterfaceResultMessage = "";
        }

        /// <summary>
        /// 리턴값 배열 초기화
        /// </summary>
        public void mfInitReturnValue()
        {
            arrReturnValue.Clear();
        }

        /// <summary>
        /// 리턴값 배열에 값을 추가
        /// </summary>
        /// <param name="strValue"></param>
        public void mfAddReturnValue(string strValue)
        {
            arrReturnValue.Add(strValue);
        }

        /// <summary>
        /// 리턴갑 배열에 값을 삭제
        /// </summary>
        /// <param name="intIndex"></param>
        public void mfDeleteReturnValue(int intIndex)
        {
            arrReturnValue.Remove(intIndex);
        }

        /// <summary>
        /// 리턴값 배열 얻기
        /// </summary>
        /// <returns></returns>
        public System.Collections.ArrayList mfGetReturnValue()
        {
            return arrReturnValue;
        }

        /// <summary>
        /// 리턴갑 배열중 특정값 얻기
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public string mfGetReturnValue(int intIndex)
        {
            return arrReturnValue[intIndex].ToString();
        }

        /// <summary>
        /// 에러메세지 구조체 정보를 문자열로 변환
        /// </summary>
        /// <param name="Err">트랜잭션처리정보 구조체</param>
        /// <returns>Encoding값</returns>
        public string mfEncodingErrMessage(TransErrRtn Err)
        {
            string strErr = "";
            string strErrSep = "<Err>";
            string strOutSep = "<OUT>";

            strErr = Err.intErrNum.ToString() + strErrSep +
                     Err.strErrMessage + strErrSep +
                     Err.strSystemMessage + strErrSep +
                     Err.strSystemStackTrace + strErrSep +
                     Err.strSystemInnerException + strErrSep +
                     Err.strInterfaceResultCode + strErrSep +       //추가
                     Err.strInterfaceResultMessage + strErrSep;     //추가

            if (Err.arrReturnValue.Count > 0)
            {
                for (int i = 0; i < Err.arrReturnValue.Count; i++)
                {
                    strErr = strErr + Err.arrReturnValue[i].ToString() + strOutSep;
                }
            }
            return strErr;
        }

        /// <summary>
        /// 에러메시지 문자를 구조체로 변환
        /// </summary>
        /// <param name="strErr">트랜잭션처리정보 문자열</param>
        /// <returns>Decoding값</returns>
        public TransErrRtn mfDecodingErrMessage(string strErr)
        {
            TransErrRtn errMsg = new TransErrRtn();

            string[] arrErrSep = { "<Err>" };
            string[] arrOutSep = { "<OUT>" };

            string[] arrErrMsg = strErr.Split(arrErrSep, System.StringSplitOptions.None);

            errMsg.intErrNum = arrErrMsg[0].ToInt();
            errMsg.strErrMessage = arrErrMsg[1];
            errMsg.strSystemMessage = arrErrMsg[2];
            errMsg.strSystemStackTrace = arrErrMsg[3];
            errMsg.strSystemInnerException = arrErrMsg[4];
            errMsg.strInterfaceResultCode = arrErrMsg[5];       //추가
            errMsg.strInterfaceResultMessage = arrErrMsg[6];    //추가

            string[] arrOutput = arrErrMsg[7].Split(arrOutSep, System.StringSplitOptions.None);
            for (int i = 0; i < arrOutput.Length; i++)
                errMsg.mfAddReturnValue(arrOutput[i]);

            return errMsg;

        }
    }
    #endregion
}