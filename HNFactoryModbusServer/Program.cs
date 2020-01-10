using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HNFactoryModbusServer
{
    static class Program
    {
        static public LogForm _logForm = null;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _logForm = new LogForm();
            _logForm.Show();//不show 会出问题
            _logForm.Visible = false;

            Application.Run(new ServerMainForm());
        }
    }
}
