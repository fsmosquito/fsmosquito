namespace FsMosquito.SimConnect
{
    using FsMosquito.SimConnect.Properties;
    using Microsoft.Extensions.Options;
    using System.Drawing;
    using System.Windows.Forms;

    internal class FsMosquitoClientContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        public FsMosquitoClientContext(IOptions<FsMosquitoSimConnectClientOptions> options, FsMosquitoClientForm clientForm)
        {
            // Initialize Tray Icon
            _trayIcon = new NotifyIcon()
            {
                Icon = Icon.FromHandle(Resources.mosquito.GetHicon()),
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true,
                Text = "FSMosquito Client",
            };

            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, (sender, e) => Application.Exit());
            _trayIcon.Click += (sender, e) =>
            {
                if (MainForm == null)
                {
                    MainForm = clientForm;
                }

                if (!MainForm.Visible)
                {
                    MainForm.Show();
                }
                MainForm.Activate();
            };

            _trayIcon.BalloonTipClosed += (sender, e) =>
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            };

            if (options.Value.ShowWindowOnStartup)
            {
                MainForm = clientForm;
                MainForm.Show();
                MainForm.Activate();
            }
        }
    }
}
