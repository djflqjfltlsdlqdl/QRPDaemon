using System;
using System.Runtime.InteropServices;
using System.Text;

namespace QRPDaemon.COM
{
    public class SharedDirectory
    {
        public int mfConnectNetworkDrive(string strNetworkFullPath, string strAccessID, string strAccessPW)
        {
            try
            {
                SharedAPI cls = new SharedAPI();
                int intResult = cls.ConnectRemoteServer(strNetworkFullPath, strAccessID, strAccessPW);

                //연결을 끊고 다시 연결한다.
                if (intResult.Equals(1219))
                {
                    // 연결이 끊어질때까지 최대 30회 반복
                    bool f = true;
                    int x = 0;
                    while (f)
                    {
                        if (cls.CancelRemoteServer(strNetworkFullPath).Equals(0))
                        {
                            f = false;
                        }
                        x++;

                        if (x.Equals(30))
                        {
                            f = false;
                        }
                    }
                    intResult = cls.ConnectRemoteServer(strNetworkFullPath, strAccessID, strAccessPW);
                }

                return intResult;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        // 공유해제 
        public int mfDisconnectNetworkDrive(string server)
        {
            return new SharedAPI().CancelRemoteServer(server);
        }

        public string mfGetConnectErrorMessage(int intErrorCode)
        {
            if (intErrorCode.Equals(0))
                return "연결을 성공했습니다.";
            else if (intErrorCode.Equals(53))
                return "네트워크 경로를 찾을 수 없습니다.";
            else if (intErrorCode.Equals(85))
                return "네트워크 드라이버가 이미 사용 중입니다.";
            else if (intErrorCode.Equals(1203))
                return "네트워크 경로가 존재하지 않거나, 잘못 입력하거나, 현재 사용할 수 없습니다. 시스템 관리자에게 문의하십시오.";
            else if (intErrorCode.Equals(1219))
                return "둘 이상의 사용자 이름을 사용하여 동일한 사용자에 의해 서버 나 공유 리소스에 중복 연결은 허용되지 않습니다. 서버 나 공유 리소스에 대한 이전의 모든 연결을 해제하고 다시 시도하십시오.";
            else if (intErrorCode.Equals(1326))
                return "사용자이름 또는 암호가 일치 하지 않습니다.";
            else
                return "네트워크 연결이 실패했습니다.";
        }
    }

    class SharedAPI
    {
        // 구조체 선언 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        // API 함수 선언 
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetUseConnection(
                  IntPtr hwndOwner,
                  [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE lpNetResource,
                  string lpPassword,
                  string lpUserID,
                  uint dwFlags,
                  StringBuilder lpAccessName,
                  ref int lpBufferSize,
                  out uint lpResult);

        // API 함수 선언 (공유해제) 
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Auto)]
        public static extern int WNetCancelConnection2A(string lpName, int dwFlags, int fForce);

        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2")]
        public static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        /// <summary>
        /// 공유 폴더에 대한 네트워크 연결을 만든다.        
        /// 그 드라이브 이름은 StringBuilder 를 통해 값이 반환된다.
        /// </summary>
        /// <param name="server">원격 공유폴더 경로</param>
        /// <param name="remoteUserId">원격 사용자 아이디</param>
        /// <param name="remotePassword">원격 비밀번호</param>
        /// <param name="localDriveName">사용가능한 드라이브 명, NULL일때 시스템이 자동으로 설정한다.</param>
        /// <returns>
        /// 0    : 성공 (0 이 아닌 값은 오류가 발생했음을 알리는 오류 코드)
        /// 85   : 네트드라이버를 설정할때, 네트워크 드라이버가 이미사용중일 때(시스템에 자동으로 생성되게 하면 localDriveName에 NULL를 입력 sb로 반환한다.)
        /// 234  : capacity 값은 공유 폴더의 경로를 담을 수 있도록 충분히 주어야 한다. 그렇지 않으면 오류 코드 234를 반환할 것이다.
        /// 1203 : 공유폴더경로 오류
        /// 1326 : 사용자/암호가 일치 하지 않는다.
        /// </returns>
        public int ConnectRemoteServer(string server, string remoteUserId, string remotePassword)
        {
            int result = 0;
            int capacity = 64;
            uint resultFlags = 0;
            // flags 가 0x80 이 아닌 값이 사용되면 StringBuilder는 일반적으로 공유 폴더의 UNC 이름을 반환한다.
            uint flags = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
            NETRESOURCE ns = new NETRESOURCE();
            ns.dwType = 1;              // 공유 디스크 
            ns.lpLocalName = null;   // 로컬 드라이브 지정하지 않음 
            ns.lpRemoteName = server;
            ns.lpProvider = null;

            result = WNetUseConnection(IntPtr.Zero, ref ns, remotePassword, remoteUserId, flags,
                                        sb, ref capacity, out resultFlags);

            return result;
        }

        // 공유해제 
        public int CancelRemoteServer(string server)
        {
            //return WNetCancelConnection2A(server, 1, 0);
            return WNetCancelConnection2(server, 0, true);
        }
    }
}
