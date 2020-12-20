namespace FsMosquito.SimConnect
{
    using FsMosquito.Extensions;
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Represents the main FSMosquito form. Provides a minimalistic UI. Signals the Adapter when WinProc is called with a message of our indicated type.
    /// </summary>
    [DesignerCategory("")]
    public sealed class FsMosquitoForm : Form, ISimConnectEventSource
    {
        private readonly IFsSimConnect _simConnect;
        private readonly ConcurrentDictionary<IObserver<SimConnectMessage>, Unsubscriber> _observers;

        private IContainer components = null;

        public FsMosquitoForm(IFsSimConnect simConnect)
        {
            _simConnect = simConnect ?? throw new ArgumentNullException(nameof(simConnect));

            InitializeComponent();

            _observers = new ConcurrentDictionary<IObserver<SimConnectMessage>, Unsubscriber>();
        }

        IntPtr ISimConnectEventSource.Handle
        {
            get
            {
                var handle = Handle; // Use a separate variable as to not pass the thread context
                return handle;
            }
        }

        IDisposable IObservable<SimConnectMessage>.Subscribe(IObserver<SimConnectMessage> observer)
        {
            return _observers.GetOrAdd(observer, new Unsubscriber(this, observer));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == _simConnect.MessageId)
            {
                var message = new SimConnectMessage()
                {
                    HWnd = m.HWnd,
                    LParam = m.LParam,
                    Msg = m.Msg,
                    WParam = m.WParam,
                };

                // Fire and forget as to not block the UI thread.
                Task.Run(() => OnSimConnectMessage(message)).Forget();
            }

            base.WndProc(ref m);
        }

        protected override void OnClosed(EventArgs e)
        {
            Parallel.ForEach(_observers.Keys, (observer) =>
            {
                try
                {
                    observer.OnCompleted();
                }
                catch (Exception)
                {
                    // Do Nothing.
                }
            });
            base.OnClosed(e);
        }

        private void OnSimConnectMessage(SimConnectMessage msg)
        {
            Parallel.ForEach(_observers.Keys, (observer) =>
            {
                try
                {
                    observer.OnNext(msg);
                }
                catch (Exception)
                {
                    // Do Nothing.
                }
            });
        }

        private sealed class Unsubscriber : IDisposable
        {
            private readonly FsMosquitoForm _parent;
            private readonly IObserver<SimConnectMessage> _observer;

            public Unsubscriber(FsMosquitoForm parent, IObserver<SimConnectMessage> observer)
            {
                _parent = parent ?? throw new ArgumentNullException(nameof(parent));
                _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            }

            public void Dispose()
            {
                if (_observer != null && _parent._observers.ContainsKey(_observer))
                {
                    _parent._observers.TryRemove(_observer, out _);
                }
            }
        }

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(0, 0);
            ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Text = "FsMosquito SimConnect Shim";
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}
