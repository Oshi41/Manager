using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Manager.Behaviors
{
    public class DoubliClickBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseDoubleClick += OnDoubleClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            
            AssociatedObject.MouseDoubleClick -= OnDoubleClick;

        }


        public static readonly DependencyProperty RowDoubleClickProperty = DependencyProperty.Register(
                                                        "RowDoubleClick", typeof(ICommand), typeof(DoubliClickBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand RowDoubleClick
        {
            get { return (ICommand) GetValue(RowDoubleClickProperty); }
            set { SetValue(RowDoubleClickProperty, value); }
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is DependencyObject originSource))
                return;
            
            if (!(ItemsControl.ContainerFromElement((DataGrid)sender, originSource) is DataGridRow row))
                return;
            
            RowDoubleClick?.Execute(row.DataContext);
        }
    }
}