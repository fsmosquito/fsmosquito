namespace FsMosquito.SimConnect
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    [DesignerCategory("")]
    public sealed class FsMosquitoShimContext : ApplicationContext
    {
        private readonly ISimConnectEventSource _eventSource;

        public FsMosquitoShimContext(ISimConnectEventSource eventSource)
        {
            _eventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));

            // If this is uncommented, the form will show, we usually don't want that.
            //if (_eventSource is Form)
            //{
            //    MainForm = _eventSource as Form;
            //}
        }
    }
}
