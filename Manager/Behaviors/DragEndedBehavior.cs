using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Manager.Behaviors
{
    public class DragEndedBehavior : Behavior<Slider>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                                                              "Value", typeof(int),
                                                                                              typeof(DragEndedBehavior)
                                                                                              , new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragComplited));
        }

        private void OnDragComplited(object sender, DragCompletedEventArgs e)
        {
            Value = (int)AssociatedObject.Value;
        }
    }
}