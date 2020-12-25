namespace FsMosquito.SimConnect
{
    using CliWrap.EventStream;
    using FsMosquito.SimConnect.Properties;
    using Microsoft.Extensions.Logging;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Represents the main FSMosquito Client form. Provides a minimalistic UI. Signals the Adapter when WinProc is called with a message of our indicated type.
    /// </summary>
    [DesignerCategory("")]
    internal class FsMosquitoClientForm : Form, IObserver<MqttClientEvent>
    {
        private const int PulseInterval = 1000;

        private readonly System.Timers.Timer _pulseMqttStatusTimer = new System.Timers.Timer(PulseInterval);
        private readonly System.Timers.Timer _pulseSimConnectStatusTimer = new System.Timers.Timer(PulseInterval);
        private readonly ILogger<FsMosquitoClientForm> _logger;
        private Color? _nextSimConnectStatusColor = null;
        private Color? _nextMqttStatusColor = null;

        private Panel _simConnectStatus;
        private Panel _mqttStatus;

        public FsMosquitoClientForm(ProcessMonitor<FsMosquitoSimConnectClientShim> processMonitor, ISimConnectMqttClient fsMqtt, ILogger<FsMosquitoClientForm> logger)
        {
            FsMqtt = fsMqtt ?? throw new ArgumentNullException(nameof(fsMqtt));
            ProcessMonitor = processMonitor ?? throw new ArgumentNullException(nameof(processMonitor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            fsMqtt.Subscribe(this);
            ((IObservable<CommandEvent>)ProcessMonitor).Subscribe(new SimConnectShimObserver(this));

            _pulseMqttStatusTimer.Elapsed += PulseMqttStatusTimer_Elapsed;
            _pulseSimConnectStatusTimer.Elapsed += PulseSimConnectStatusTimer_Elapsed;
            InitializeControls();

            ProcessMonitor.Start();
        }

        public ISimConnectMqttClient FsMqtt
        {
            get;
            private set;
        }

        public ProcessMonitor<FsMosquitoSimConnectClientShim> ProcessMonitor
        {
            get;
            private set;
        }

        #region Form Event Handlers
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        #endregion

        #region Pulse Event Handlers
        private void PulseSimConnectStatusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_nextSimConnectStatusColor.HasValue == false || _nextSimConnectStatusColor.Value == _simConnectStatus.BackColor)
            {
                _simConnectStatus.BackColor = Color.Green;
                return;
            }

            _simConnectStatus.BackColor = _nextSimConnectStatusColor.Value;
            _nextSimConnectStatusColor = null;
        }

        private void PulseMqttStatusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_nextMqttStatusColor.HasValue == false || _nextMqttStatusColor.Value == _mqttStatus.BackColor)
            {
                _mqttStatus.BackColor = Color.Green;
                return;
            }

            _mqttStatus.BackColor = _nextMqttStatusColor.Value;
            _nextMqttStatusColor = Color.Green;
        }
        #endregion

        #region IObserver<MqttClientEvent>

        void IObserver<MqttClientEvent>.OnCompleted()
        {
            _nextMqttStatusColor = Color.Orange;
        }

        void IObserver<MqttClientEvent>.OnError(Exception error)
        {
            _nextMqttStatusColor = Color.Red;
        }

        void IObserver<MqttClientEvent>.OnNext(MqttClientEvent clientEvent)
        {
            switch (clientEvent)
            {
                case MqttClientConnectedEvent:
                    _mqttStatus.BackColor = Color.Green;
                    _pulseMqttStatusTimer.Start();
                    break;
                case MqttClientDisconnectedEvent:
                    _mqttStatus.BackColor = Color.Orange;
                    _pulseMqttStatusTimer.Stop();
                    break;
                case MqttClientMessageReceivedEvent:
                    _nextMqttStatusColor = Color.Blue;
                    break;
            }
        }
        #endregion

        #region Initialize Controls
        private void InitializeControls()
        {
            // Add the status panel
            var statusPanel = new Panel
            {
                Height = 20,
                Dock = DockStyle.Top
            };

            _simConnectStatus = new Panel
            {
                Width = 20,
                Height = 20,
                BackColor = Color.Orange,
                Dock = DockStyle.Left
            };

            var simConnectStatusToolTip = new ToolTip();
            simConnectStatusToolTip.SetToolTip(_simConnectStatus, "SimConnect.dll");

            statusPanel.Controls.Add(_simConnectStatus);

            _mqttStatus = new Panel
            {
                Width = 20,
                Height = 20,
                BackColor = Color.Orange,
                Dock = DockStyle.Right
            };

            var mqttStatusToolTip = new ToolTip();
            mqttStatusToolTip.SetToolTip(_mqttStatus, FsMqtt.MqttBrokerUrl);

            statusPanel.Controls.Add(_mqttStatus);

            // Add the Mosquito Picture
            var pb1 = new PictureBox
            {
                Location = new Point((Width / 2) - 80, (Height / 2) - 100),
                Width = 150,
                Height = 150,
                Image = Resources.mosquito,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Top,
            };

            var lblMosquito = new Label
            {
                Height = 140,
                Text = "FSMosquito",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                Dock = DockStyle.Top,
            };

            Controls.Add(lblMosquito);
            Controls.Add(pb1);
            Controls.Add(statusPanel);

            // Set properties
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            ShowIcon = false;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }
        #endregion

        #region Nested Classes
        private sealed class SimConnectShimObserver : IObserver<CommandEvent>
        {
            public SimConnectShimObserver(FsMosquitoClientForm parent)
            {
                Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            }

            public FsMosquitoClientForm Parent { get; }

            void IObserver<CommandEvent>.OnCompleted()
            {
                Parent._simConnectStatus.BackColor = Color.Orange;
            }

            void IObserver<CommandEvent>.OnError(Exception error)
            {
                Parent._simConnectStatus.BackColor = Color.Red;
            }

            void IObserver<CommandEvent>.OnNext(CommandEvent cmdEvent)
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent started:
                        Parent._simConnectStatus.BackColor = Color.Green;
                        Parent._pulseSimConnectStatusTimer.Start();
                        break;
                    case StandardOutputCommandEvent stdOut:
                        Parent._nextSimConnectStatusColor = Color.Blue;
                        break;
                    case StandardErrorCommandEvent stdErr:
                        Parent._nextSimConnectStatusColor = Color.Purple;
                        break;
                    case ExitedCommandEvent exited:
                        Parent._simConnectStatus.BackColor = Color.Orange;
                        Parent._pulseSimConnectStatusTimer.Stop();
                        break;
                }
            }
        }
        #endregion
    }
}
