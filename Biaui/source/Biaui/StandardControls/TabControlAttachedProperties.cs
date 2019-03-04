using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Biaui.Internals;

namespace Biaui.StandardControls
{
    public partial class TabControlAttachedProperties
    {
        public static readonly DependencyProperty IsVisibleAddButtonProperty =
            DependencyProperty.RegisterAttached(
                "IsVisibleAddButton",
                typeof(bool),
                typeof(TabControlAttachedProperties),
                new FrameworkPropertyMetadata(Boxes.BoolFalse));

        public static bool GetIsVisibleAddButton(DependencyObject target)
        {
            return (bool) target.GetValue(IsVisibleAddButtonProperty);
        }

        public static void SetIsVisibleAddButton(DependencyObject target, bool value)
        {
            target.SetValue(IsVisibleAddButtonProperty, value);
        }


        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.RegisterAttached(
                "AddCommand",
                typeof(ICommand),
                typeof(TabControlAttachedProperties),
                new FrameworkPropertyMetadata(null));

        public static ICommand GetAddCommand(DependencyObject target)
        {
            return (ICommand) target.GetValue(AddCommandProperty);
        }

        public static void SetAddCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(AddCommandProperty, value);
        }


        public static readonly DependencyProperty AddCommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "AddCommandParameter",
                typeof(object),
                typeof(TabControlAttachedProperties),
                new FrameworkPropertyMetadata(null));

        public static object GetAddCommandParameter(DependencyObject target)
        {
            return target.GetValue(AddCommandParameterProperty);
        }

        public static void SetAddCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(AddCommandParameterProperty, value);
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var control = (DependencyObject) sender;

            while (true)
            {
                var parent = VisualTreeHelper.GetParent(control);

                Debug.Assert(parent != null);

                if (parent is TabControl tabControl)
                {
                    var command = GetAddCommand(tabControl);
                    if (command == null)
                        return;

                    var param = GetAddCommandParameter(tabControl);
                    if (command.CanExecute(param) == false)
                        return;

                    command.Execute(param);
                    return;
                }

                control = parent;
            }
        }
    }
}