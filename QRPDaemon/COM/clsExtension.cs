using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        #region Invoke
        /// <summary>
        /// Contorl Invoke 처리
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action">Invoke 시 진행할 메소드</param>
        public static void mfInvokeIfRequired(this System.ComponentModel.ISynchronizeInvoke obj, MethodInvoker action)
        {
            try
            {
                obj.mfInvokeIfRequired(action, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// Contorl Invoke 처리
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action">Invoke 시 진행할 메소드</param>
        /// <param name="bolAsync">Return Async</param>
        public static void mfInvokeIfRequired(this System.ComponentModel.ISynchronizeInvoke obj, MethodInvoker action, bool bolAsync)
        {
            try
            {
                if (obj.InvokeRequired)
                {
                    IAsyncResult ar = obj.BeginInvoke(action, null);
                    if (bolAsync)
                    {
                        if (ar.AsyncWaitHandle.WaitOne(100, false))
                        {
                            obj.EndInvoke(ar);
                        }
                    }
                }
                else
                {
                    action.Invoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void InvokeCallBack(IAsyncResult ar)
        {
            try
            {
                System.ComponentModel.ISynchronizeInvoke obj = (System.ComponentModel.ISynchronizeInvoke)ar.AsyncState;
                obj.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
        }
        #endregion
    }
}
