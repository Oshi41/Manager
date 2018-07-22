using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Manager.Behaviors
{
    public class PasswordBindBehavior : Behavior<PasswordBox>
    {
        private bool _flag;

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
                                                                                                 "Password",
                                                                                                 typeof(SecureString),
                                                                                                 typeof(
                                                                                                     PasswordBindBehavior
                                                                                                 ),
                                                                                                 new
                                                                                                     FrameworkPropertyMetadata(null,
                                                                                                                               FrameworkPropertyMetadataOptions
                                                                                                                                   .BindsTwoWayByDefault));

        public SecureString Password
        {
            get { return (SecureString) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += OnPassChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PasswordChanged -= OnPassChanged;

        }

        private void OnPassChanged(object sender, RoutedEventArgs e)
        {
            ExecuteArbiter(() => { Password = AssociatedObject.SecurePassword.Copy(); });
        }

        private void ExecuteArbiter(Action action)
        {
            if (_flag || action == null)
                return;

            _flag = true;

            action();

            _flag = false;
        }
    }
}