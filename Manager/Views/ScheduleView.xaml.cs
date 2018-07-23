using System.Windows;

namespace Manager.Views
{
    public partial class ScheduleView
    {
        public ScheduleView()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;

            window.DialogResult = true;
            window.Close();
        }
    }
}