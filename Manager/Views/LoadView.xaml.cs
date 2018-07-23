using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Manager.Views
{
    public partial class LoadView
    {
        public LoadView()
        {
            InitializeComponent();
        }

        private void CloseDialogHost(object sender, RoutedEventArgs e)
        {
            var input = sender as IInputElement;
            DialogHost.CloseDialogCommand.Execute(null, input);
        }

        private void CloseCurrentWindow(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;

            window.DialogResult = true;
            window.Close();
        }
    }
}