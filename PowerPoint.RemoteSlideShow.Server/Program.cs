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
            string assemblyName = XProvider.Value.AssemblyValue.Name;

            using (Mutex appRunCheck = new Mutex(false, assemblyName))
            {
                if (appRunCheck.WaitOne(0, false) == true)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainFrame());
                }
                else
                {
                    MessageBox.Show("프로그램이 이미 실행중입니다.", assemblyName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                appRunCheck.Close();
            }
        }
    }
}
