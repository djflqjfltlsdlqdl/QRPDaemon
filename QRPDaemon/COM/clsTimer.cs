using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QRPDaemon.COM
{
    public delegate void stCallBackDelegate();
    /// <summary>
    /// 스케쥴 타이머 클래스(특정시간에 메소드 호출)
    /// </summary>
    public class clsScheduledTimer
    {
        private System.Threading.Timer _timer;
        public clsScheduledTimer() { }
        public static TimeSpan GetDueTime(TimeSpan A, TimeSpan B)
        {
            if (A < B)
            {
                return B.Subtract(A);
            }
            else
            {
                return new TimeSpan(24, 0, 0).Subtract(A.Subtract(B));
            }
        }
        public void SetTime(TimeSpan _time, stCallBackDelegate callback)
        {
            if (this._timer != null)
            {
                this._timer = null;
            }

            TimeSpan Now = DateTime.Now.TimeOfDay;
            TimeSpan DueTime = GetDueTime(Now, _time);

            this._timer = new System.Threading.Timer(new System.Threading.TimerCallback(delegate (object _callback)
            {
                ((stCallBackDelegate)_callback)();
            }), callback, DueTime, new TimeSpan(0, 0, 10));
        }
    }
}
