using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using System.IO;

//对常用Win32 API函数及结构的声明
using System.Runtime.InteropServices;


namespace zclTimeKeep
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();


            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimedEvent);
            timer.Interval = 300000;//每5分钟执行一次
            //timer.Interval = 300;
            timer.Enabled = true;
        }

        //定时执行事件
        private void TimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            //业务逻辑代码
            GainTime();
        }
        protected override void OnStart(string[] args)
        {
            this.WriteLog("zclTime服务：【服务开始】");
            setTime();
        }

        protected override void OnStop()
        {
            this.WriteLog("zclTime服务：【服务停止】");
            GainTime();
        }

        protected override void OnShutdown()
        {
            this.WriteLog("zclTime服务：【计算机关闭】");
            GainTime();
        }
        #region 记录日志
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg"></param>
        private void WriteLog(string msg)
        {

            //string path = @"C:\log.txt";

            //该日志文件会存在windows服务程序目录下
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                FileStream fs;
                fs = File.Create(path);
                fs.Close();
            }

            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString() + "   " + msg);
                    sw.Close();
                }
            }
        }
        #endregion
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

            /// <summary>
            /// 从System.DateTime转换。
            /// </summary>
            /// <param name="time">System.DateTime类型的时间。</param>
            public void FromDateTime(DateTime time)
            {
                wYear = (ushort)time.Year;
                wMonth = (ushort)time.Month;
                wDayOfWeek = (ushort)time.DayOfWeek;
                wDay = (ushort)time.Day;
                wHour = (ushort)time.Hour;
                wMinute = (ushort)time.Minute;
                wSecond = (ushort)time.Second;
                wMilliseconds = (ushort)time.Millisecond;
            }
            /// <summary>
            /// 转换为System.DateTime类型。
            /// </summary>
            /// <returns></returns>
            public DateTime ToDateTime()
            {
                return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
            }
            /// <summary>
            /// 静态方法。转换为System.DateTime类型。
            /// </summary>
            /// <param name="time">SYSTEMTIME类型的时间。</param>
            /// <returns></returns>
            public static DateTime ToDateTime(SystemTime time)
            {
                return time.ToDateTime();
            }
        }

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime Time);
        private void setTime() //获取文件中的时间然后设置到系统中
        {
            byte[] array = new byte[100];
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\zclTime.txt";
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                FileStream fs;
                fs = File.Create(path);
                fs.Close();
            }
            else
            {
                string s = File.ReadAllText(path);
                DateTime t = Convert.ToDateTime(s);

                DateTime t1 = DateTime.Now;
                if (DateTime.Compare(t, t1) > 0)
                {
                    SystemTime st = new SystemTime();
                    st.FromDateTime(t);
                    //调用Win32 API设置系统时间
                    SetLocalTime(ref st);
                }               
            }
        }

        private void GainTime() //获取系统时间然后更新到文件中
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\zclTime.txt";
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                FileStream fs;
                fs = File.Create(path);
                fs.Close();
            }

            using (FileStream fs = new FileStream(path, FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.Flush();
                    sw.Close();
                }
            }
        }

    }
}