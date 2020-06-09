using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRPDaemon.COM
{
    public static class CustomExtension
    {
        #region Convert
        public static int ToInt(this object obj)
        {
            decimal dblo = 0;
            decimal.TryParse(obj.ToString(), out dblo);
            dblo = Math.Truncate(dblo);

            int o = 0;
            if (!int.TryParse(dblo.ToString(), out o))
                return 0;
            else
                return o;
        }
        public static Int64 ToInt64(this object obj)
        {
            decimal dblo = 0;
            decimal.TryParse(obj.ToString(), out dblo);
            dblo = Math.Truncate(dblo);

            Int64 o = 0;
            if (!Int64.TryParse(dblo.ToString(), out o))
                return 0;
            else
                return o;
        }
        public static decimal ToDecimal(this object obj)
        {
            decimal o = 0;
            if (!decimal.TryParse(obj.ToString(), out o))
                return 0;
            else
                return o;
        }
        public static bool ToBool(this object obj)
        {
            bool bl = false;

            if (!bool.TryParse(obj.ToString(), out bl))
                return false;
            else
                return bl;
        }
        public static string ToDateString(this object obj)
        {
            DateTime o;
            if (DateTime.TryParse(obj.ToString(), out o))
                return o.ToString("yyyy-MM-dd");
            else
                return string.Empty;
        }
        public static string ToFormatString(this object obj)
        {
            return obj.ToFormatString(0);
        }
        public static string ToFormatString(this object obj, int intPointLength)
        {
            return string.Format("{0:N" + intPointLength.ToString() + "}", obj);
        }
        #endregion
    }
}
