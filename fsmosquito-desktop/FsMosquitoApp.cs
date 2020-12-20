namespace FsMosquito
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IFsMosquitoApp
    {
        Task Initialize();
    }

    public class FsMosquitoApp : IFsMosquitoApp
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FsMosquitoApp> _logger;
        private BrowserWindow _mainWindow;

        public FsMosquitoApp(IWebHostEnvironment environment, ILogger<FsMosquitoApp> logger)
        {
            _env = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IntPtr MainWindowHandle
        {
            get;
            private set;
        }

        public async Task Initialize()
        {
            _logger.LogInformation($"FSMosquito Desktop Started Initialization.");

            // Configure Electron to not quit when all the windows are closed
            // This is the "Minimize to tray" behavior.
            Electron.WindowManager.IsQuitOnWindowAllClosed = false;
            Electron.IpcMain.On("hideToSystemTray", (e) =>
            {
                _mainWindow.Hide();
            });

            Electron.IpcMain.On("getHostName", (e) =>
            {
                Electron.IpcMain.Send(_mainWindow, "hostName", Environment.MachineName);
            });

            var trayMenuSettingIcon = Path.Combine(_env.ContentRootPath, "Assets/fsmosquito.png");

            // configure the Main Browser Window
            var options = new BrowserWindowOptions
            {
                MinWidth = 800,
                MinHeight = 400,
                Width = 1152,
                Height = 940,
                Show = false,
                DarkTheme = true,
                Center = true,
                AutoHideMenuBar = true,
                Icon = trayMenuSettingIcon,

                WebPreferences = new WebPreferences
                {
                    // Disable IPC
                    // NodeIntegration = false,
                }
            };

            _mainWindow = await Electron.WindowManager.CreateWindowAsync(options);
            await _mainWindow.WebContents.Session.ClearCacheAsync();
            _mainWindow.OnReadyToShow += () =>
            {
                _mainWindow.Show();
                ConfigureTray();
            };

            _mainWindow.OnMinimize += () =>
            {
                _mainWindow.Hide();
            };

            _mainWindow.SetTitle("FSMosquito Desktop");
            _mainWindow.RemoveMenu();
            _mainWindow.SetMenuBarVisibility(false);

            _logger.LogInformation($"FSMosquito Desktop Completed Initialization.");
        }

        public void ConfigureTray()
        {
            var menu = new List<MenuItem>() {
                //new MenuItem
                //{
                //  Label = "Create Contact",
                //  Click = () => Electron
                //        .WindowManager
                //        .BrowserWindows
                //        .First()
                //        .LoadURL($"http://localhost:{BridgeSettings.WebPort}/Contacts/Create")
                //},
                new MenuItem
                {
                  Label = "Close FSMosquito",
                  Click = () =>
                  {
                      Electron.App.Exit();
                  }
                }
            };

            var envVars = Environment.GetEnvironmentVariables();
            if (_env.IsDevelopment() || (envVars.Contains("FSMOSQUITO_ENVIRONMENT") && (string)envVars["FSMOSQUITO_ENVIRONMENT"] == "Development"))
            {
                menu.Insert(0, new MenuItem
                {
                    Label = "Show Development Tools",
                    Click = () => _mainWindow.WebContents.OpenDevTools()
                });
            }

            Electron.Tray.Show(Path.Combine(_env.ContentRootPath, "Assets/fsmosquito.png"), menu.ToArray());

            // This *actually* sets the tray icon.
            Electron.Tray.SetImage(Path.Combine(_env.ContentRootPath, "Assets/fsmosquito.png"));
            Electron.Tray.OnClick += (e, rect) =>
            {
                _mainWindow.Show();
            };
            Electron.Tray.SetToolTip("FSMosquito");
        }
    }
}
