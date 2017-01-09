using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace PowerPoint.RemoteSlideShow.Server
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex appRunCheck = new Mutex(false, XProvider.Value.AssemblyValue.Name))
            {
                if (appRunCheck.WaitOne(0, false) == true)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainFrame());
                }
                else
                {
                    MessageBox.Show("프로그램이 이미 실행중입니다.", XProvider.Value.AssemblyValue.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                appRunCheck.Close();
            }
        }
    }
}
