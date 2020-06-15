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
            CommonCode.m_strCreateLogPathName = exeFileInfo.Directory.FullName.ToString() + @"\CreateLog\";
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

    public static class COMResources
    {
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int WM_ALTF4 = 0x0010;

        /// <summary>
        /// 폰트명
        /// </summary>
        public static string FontName { get; set; }
        /// <summary>
        /// 언어
        /// </summary>
        public static string Lang { get; set; }
        /// <summary>
        /// DB Connection String
        /// </summary>
        public static string ConnString { get; set; }
        /// <summary>
        /// Update File Server path
        /// </summary>
        public static string UpdaterServerPath { get; set; }
        /// <summary>
        /// Barcode Scan 변수
        /// </summary>
        //public static string SCAN { get; set; }
        /// <summary>
        /// Ctrl + V 체크
        /// </summary>
        //public static bool CTRLV { get; set; }
        /// <summary>
        /// 사용자ID
        /// </summary>
        public static string UserID { get; set; }
        /// <summary>
        /// 사용자명
        /// </summary>
        public static string UserName { get; set; }
        /// <summary>
        /// JIK 코드
        /// </summary>
        public static string JIK_CD { get; set; }
        /// <summary>
        /// JIK 명
        /// </summary>
        public static string JIK_NM { get; set; }

        /// <summary>
        /// 공장코드
        /// </summary>
        public static string PL_CD { get; set; }
        /// <summary>
        /// 공장명
        /// </summary>
        public static string PL_NM { get; set; }
        /// <summary>
        /// 단말기번호
        /// </summary>
        public static string IPC_NO { get; set; }
        /// <summary>
        /// 단말기명
        /// </summary>
        public static string IPC_NAME { get; set; }
        /// <summary>
        /// 단말기 구분
        /// </summary>
        public static string IPC_GUBN { get; set; }
        /// <summary>
        /// 단말기 Password
        /// </summary>
        public static string IPC_PASS { get; set; }
        /// <summary>
        /// 차종코드
        /// </summary>
        public static string GMODEL { get; set; }
        /// <summary>
        /// 차종명
        /// </summary>
        public static string GMODEL_NM { get; set; }
        /// <summary>
        /// 라인코드
        /// </summary>
        public static string LINE_CD { get; set; }
        /// <summary>
        /// 라인명
        /// </summary>
        public static string LINE_NM { get; set; }
        /// <summary>
        /// 공정코드
        /// </summary>
        public static string PROC_CD { get; set; }
        /// <summary>
        /// 공정명
        /// </summary>
        public static string PROC_NM { get; set; }
        /// <summary>
        /// IP 주소
        /// </summary>
        public static string IP_ADDRESS { get; set; }
        /// <summary>
        /// EVENT
        /// </summary>
        public static string EVENT { get; set; }
        /// <summary>
        /// 작업조
        /// </summary>
        public static string WORK_SHIFT { get; set; }
        /// <summary>
        /// 공정구분
        /// </summary>
        public static string PROC_GUBN { get; set; }
        /// <summary>
        /// 공정구분명
        /// </summary>
        public static string PROC_GUBN_NM { get; set; }
        /// <summary>
        /// 모델
        /// </summary>
        public static string MODEL_NO { get; set; }
        /// <summary>
        /// 차대번호
        /// </summary>
        public static string VEH_NO { get; set; }
        /// <summary>
        /// VIN(Vehicle Identification Number)
        /// </summary>
        public static string VIN_NO { get; set; }
        /// <summary>
        /// 결함부위 중그룹코드
        /// </summary>
        public static string MIDDLE_CD { get; set; }
        /// <summary>
        /// 결함부위 중그룹명
        /// </summary>
        public static string MIDDLE_NM { get; set; }
        /// <summary>
        /// 결함부위 소그룹코드
        /// </summary>
        public static string SMALL_CD { get; set; }
        /// <summary>
        /// 결함부위 소그룹명
        /// </summary>
        public static string SMALL_NM { get; set; }
        /// <summary>
        /// 위치코드
        /// </summary>
        public static string POSITION_CD { get; set; }
        /// <summary>
        /// 결함부위 코드
        /// </summary>
        public static string REGION_NO { get; set; }
        /// <summary>
        /// 결함부위 명
        /// </summary>
        public static string REGION_NM { get; set; }
        /// <summary>
        /// 결함현상코드
        /// </summary>
        public static string DEFECT_CD { get; set; }
        /// <summary>
        /// 결함현상명
        /// </summary>
        public static string DEFECT_NM { get; set; }
        /// <summary>
        /// 전달사항
        /// </summary>
        public static string NOTICE { get; set; }
        /// <summary>
        /// 작업일
        /// </summary>
        public static System.DateTime WORKDATE { get; set; }
        /// <summary>
        /// COM Poart
        /// </summary>
        public static string COM_Port { get; set; }

        /// <summary>
        /// 모델(OLD)
        /// </summary>
        public static string MODEL_NO_OLD { get; set; }
        /// <summary>
        /// 차대번호(OLD)
        /// </summary>
        public static string VEH_NO_OLD { get; set; }
        /// <summary>
        /// VIN(Vehicle Identification Number)(OLD)
        /// </summary>
        public static string VIN_NO_OLD { get; set; }
        /// <summary>
        /// 공정_공장코드
        /// </summary>
        public static string SHOP { get; set; }
        /// <summary>
        /// 공정_라인코드
        /// </summary>
        public static string LINE { get; set; }
    }
}
