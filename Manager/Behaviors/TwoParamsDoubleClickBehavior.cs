using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Manager.Behaviors
{
    public class TwoParamsDoubleClickBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty FirstParameterProperty = DependencyProperty.Register(
                                                        "FirstParameter", typeof(object), typeof(TwoParamsDoubleClickBehavior), new PropertyMetadata(default(object)));

        public object FirstParameter
        {
            get { return GetValue(FirstParameterProperty); }
            set { SetValue(FirstParameterProperty, value); }
        }


        public static readonly DependencyProperty SecondParameterProperty = DependencyProperty.Register(
                                                        "SecondParameter", typeof(object), typeof(TwoParamsDoubleClickBehavior), new PropertyMetadata(default(object)));

        public object SecondParameter
        {
            get { return GetValue(SecondParameterProperty); }
            set { SetValue(SecondParameterProperty, value); }
        }


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
                                                        "Command", typeof(ICommand), typeof(TwoParamsDoubleClickBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
                                                        "Condition", typeof(bool), typeof(TwoParamsDoubleClickBehavior), new PropertyMetadata(true));

        public bool Condition
        {
            get { return (bool) GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            
            AssociatedObject.MouseLeftButtonDown += OnMouseDown;
        }
        

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1 && Condition)
            {
                var param = FirstParameter ?? SecondParameter;
                Command?.Execute(param);
            }
        }
    }
}