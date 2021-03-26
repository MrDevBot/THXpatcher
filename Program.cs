using System;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using THXpatcher.Properties;

namespace THX_Service_Manager
{
    static class Program
    {
        #region Static Vars
        internal static NotifyIcon NotificationObject = new NotifyIcon();
        internal static readonly string[] PrefixBlacklist = new string[] { "Razer Synapse Service Pro", "THXHelper", "audiodg", "RZSurroundHelper" };
        //Razer Synapse 3 and THXService are restarted by ServiceControllers 


        //C:\Program Files (x86)\Razer\Synapse3\Service\Razer Synapse Service.exe

        internal static ServiceController Synapse = new ServiceController("Razer Synapse Service");
        internal static ServiceController THX = new ServiceController("THXService");
        internal static ServiceController RzSndSrv = new ServiceController("RzSndSrv");

        #endregion
        #region Helpers
        internal static void Displaynotify(string Product, string Title, string Text, int ShowTime)
        {
            NotificationObject.Icon = Resources.razer;
            NotificationObject.Text = Product;
            NotificationObject.Visible = true;
            NotificationObject.BalloonTipTitle = Title;
            NotificationObject.BalloonTipText = Text;
            NotificationObject.ShowBalloonTip(ShowTime);
        }
        #endregion

        #region Main Program
        static void Main()
        {
            //lets notify the user
            Displaynotify("THX-Patcher", "THX Restarter", "restarting THX related services...", 2500);

            if (Synapse.CanStop) Synapse.Stop();
            if (THX.CanStop) THX.Stop();
            if (RzSndSrv.CanStop) RzSndSrv.Stop(); 

            if (Synapse.Status != ServiceControllerStatus.Running) Synapse.Start();
            if (THX.Status != ServiceControllerStatus.Running) THX.Start();
            if (RzSndSrv.Status != ServiceControllerStatus.Running) RzSndSrv.Start();

            //now lets kill & restart the associated programs (filepath taken from process object so we dont need to check what version / headset)
            foreach (var process in Process.GetProcesses())
            {
                //I know, nested foreach. its a small program though and I am lazy
                foreach (string Target in PrefixBlacklist)
                {
                    string FilePath;

                    if (process.ProcessName.StartsWith(Target))
                    {
                        FilePath = process.MainModule.FileName;
                        process.Kill();
                        Process.Start(FilePath);
                    }
                }
            }

            Displaynotify("THX-Service", "THX Restarter", "Done!", 2500);
            Task.Delay(2600);
        }
        #endregion
    }
}
