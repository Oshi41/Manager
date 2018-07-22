using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Manager.Behaviors
{
    // Used on sub-controls of an expander to bubble the mouse wheel scroll event up 
    public class BubbleScrollEvent : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var parent = sender as FrameworkElement;

            while (parent != null)
            {
                if (parent is ScrollViewer)
                    break;
                
                parent = (FrameworkElement) VisualTreeHelper.GetParent(parent);
            }
            
            if (!(parent is ScrollViewer viewer))
                return;

            // [-1; 1]
            var offset = e.Delta / Math.Abs(e.Delta);

            if (viewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset - offset);
            }

            if (viewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                viewer.ScrollToVerticalOffset(viewer.VerticalOffset - offset);
            }
        }
    }
}