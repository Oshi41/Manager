using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Manager.Control
{
    public class SelectiveStackPanel : SelectiveScrollingGrid
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
                                                        "Orientation", typeof(Orientation), typeof(SelectiveStackPanel), new PropertyMetadata(default(Orientation)));

        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            var added = visualAdded != null;
            var removed = visualRemoved != null;

            if (added != removed)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (removed)
                    {
                        ColumnDefinitions.RemoveAt(ColumnDefinitions.Count - 1);
                    }
                    else
                    {
                        ColumnDefinitions.Add(new ColumnDefinition());
                        visualAdded.SetValue(Grid.ColumnProperty, ColumnDefinitions.Count - 1);
                    }
                }
                else
                {
                    if (removed)
                    {
                        RowDefinitions.RemoveAt(RowDefinitions.Count - 1);
                    }
                    else
                    {
                        RowDefinitions.Add(new RowDefinition());
                        visualAdded.SetValue(Grid.RowProperty, RowDefinitions.Count - 1);
                    }
                }
            }
            
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}