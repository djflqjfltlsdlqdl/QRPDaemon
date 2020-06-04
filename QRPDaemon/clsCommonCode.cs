namespace QRPDaemon
{
    public static class CommonCode
    {
        private static string m_strEnvXMLFileName = string.Empty;
        private static string m_strExecutablePath = string.Empty;
        private static string m_strDefaultXMLPath = string.Empty;
        private static string m_strMeasureDataXMLPath = string.Empty;
        private static string m_strCreateXMLFilePath = string.Empty;
        private static string m_strCreateLogPathName = string.Empty;
        private static string m_strCopyLogPathName = string.Empty;
        private static string m_strConnectIP = string.Empty;
        private static string m_strFolderPath = string.Empty;
        private static string m_strAccessID = string.Empty;
        private static string m_strAccessPW = string.Empty;
        private static string m_strCompleteXMLFilePath = string.Empty;

        private static object m_objDBLock = new object();

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

        /// <summary>
        /// 파일복사 Log 파일경로
        /// </summary>
        public static string CopyLogPathName
        {
            get { return CommonCode.m_strCopyLogPathName; }
            set { CommonCode.m_strCopyLogPathName = value; }
        }

        /// <summary>
        /// 파일서버 연결IP
        /// </summary>
        public static string ConnectIP
        {
            get { return CommonCode.m_strConnectIP; }
            set { CommonCode.m_strConnectIP = value; }
        }

        /// <summary>
        /// 파일서버 폴더명
        /// </summary>
        public static string FolderPath
        {
            get { return CommonCode.m_strFolderPath; }
            set { CommonCode.m_strFolderPath = value; }
        }

        /// <summary>
        /// 파일서버 ID
        /// </summary>
        public static string AccessID
        {
            get { return CommonCode.m_strAccessID; }
            set { CommonCode.m_strAccessID = value; }
        }

        /// <summary>
        /// 파일서버 패스워드
        /// </summary>
        public static string AccessPW
        {
            get { return CommonCode.m_strAccessPW; }
            set { CommonCode.m_strAccessPW = value; }
        }

        /// <summary>
        /// 서버 업로드 완료된 XML 파일 이동폴더 경로
        /// </summary>
        public static string CompleteXMLFilePath
        {
            get { return CommonCode.m_strCompleteXMLFilePath; }
            set { CommonCode.m_strCompleteXMLFilePath = value; }
        }

        /// <summary>
        /// ReferenceID 생성시 Thread 동기화를 위한 변수
        /// </summary>
        public static object DBLock
        {
            get { return CommonCode.m_objDBLock; }
            set { CommonCode.m_objDBLock = value; }
        }


        static CommonCode()
        {
            System.IO.FileInfo exeFileInfo = new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath);
            CommonCode.m_strEnvXMLFileName = @"XML\FileSetting.xml";
            CommonCode.m_strCreateLogPathName = exeFileInfo.Directory.FullName.ToString() + @"\CreateLog\";
            CommonCode.m_strCreateXMLFilePath = exeFileInfo.Directory.FullName.ToString() + @"\RESULT\";
            CommonCode.m_strDefaultXMLPath = @"XML\Default.xml";
            CommonCode.m_strMeasureDataXMLPath = @"XML\MeasureData.xml";

            // 파일복사관련
            // 네트워크 경로설정
            //CommonCode.m_strConnectIP = "dt-6cr4237dv6";
            CommonCode.m_strConnectIP = "10.103.1.92";
            CommonCode.m_strFolderPath = @"SPC";
            CommonCode.m_strAccessID = "smis";
            CommonCode.m_strAccessPW = "smis";
            CommonCode.m_strCopyLogPathName = exeFileInfo.Directory.FullName.ToString() + @"\CopyLog\";
            CommonCode.m_strCompleteXMLFilePath = exeFileInfo.Directory.FullName.ToString() + @"\COMPLETE\";
        }
    }
}
