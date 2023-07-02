using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Biaui.Behaviors;

public class ListBoxBehavior : Behavior<ListBox>
{
    #region ItemSelectionStartingCommand

    public ICommand? ItemSelectionStartingCommand
    {
        get => _ItemSelectionStartingCommand;
        set
        {
            if (value != _ItemSelectionStartingCommand)
                SetValue(ItemSelectionStartingCommandProperty, value);
        }
    }

    private ICommand? _ItemSelectionStartingCommand;

    public static readonly DependencyProperty ItemSelectionStartingCommandProperty =
        DependencyProperty.Register(
            nameof(ItemSelectionStartingCommand),
            typeof(ICommand),
            typeof(ListBoxBehavior),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (ListBoxBehavior) s;
                    self._ItemSelectionStartingCommand = (ICommand) e.NewValue;
                }));

    #endregion

    #region ItemSelectionCompletedCommand

    public ICommand? ItemSelectionCompletedCommand
    {
        get => _ItemSelectionCompletedCommand;
        set
        {
            if (value != _ItemSelectionCompletedCommand)
                SetValue(ItemSelectionCompletedCommandProperty, value);
        }
    }

    private ICommand? _ItemSelectionCompletedCommand;

    public static readonly DependencyProperty ItemSelectionCompletedCommandProperty =
        DependencyProperty.Register(
            nameof(ItemSelectionCompletedCommand),
            typeof(ICommand),
            typeof(ListBoxBehavior),
            new PropertyMetadata(
                default(ICommand),
                (s, e) =>
                {
                    var self = (ListBoxBehavior) s;
                    self._ItemSelectionCompletedCommand = (ICommand) e.NewValue;
                }));

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.PreviewKeyDown += AssociatedObjectOnPreviewKeyDown;
        AssociatedObject.KeyUp += AssociatedObjectOnKeyUp;

        AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnPreviewMouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.PreviewKeyDown -= AssociatedObjectOnPreviewKeyDown;
        AssociatedObject.KeyUp -= AssociatedObjectOnKeyUp;

        AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObjectOnPreviewMouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp -= AssociatedObjectOnMouseLeftButtonUp;
    }

    private static bool IsPressControlKey => (Keyboard.Modifiers & ModifierKeys.Control) != 0;

    private bool _isInCtrlA;
    private bool _isInKeySelection;
    private bool _isInClickSelection;

    private void AssociatedObjectOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if ((e.Key == Key.A && IsPressControlKey) ||
            (e.Key == Key.Divide && IsPressControlKey) ||
            (e.Key == Key.Oem2 && IsPressControlKey))
        {
            if (_isInCtrlA == false)
            {
                _isInCtrlA = true;
                ItemSelectionStartingCommand?.Execute(null);
            }
        }

        var isMove =
            e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right ||
            e.Key == Key.Prior || e.Key == Key.Next ||
            e.Key == Key.End || e.Key == Key.Home;

        if (isMove)
        {
            if (_isInKeySelection == false)
            {
                _isInKeySelection = true;
                ItemSelectionStartingCommand?.Execute(null);
            }
        }
    }

    private void AssociatedObjectOnKeyUp(object sender, KeyEventArgs e)
    {
        if (_isInCtrlA)
        {
            _isInCtrlA = false;
            ItemSelectionCompletedCommand?.Execute(null);
        }

        if (_isInKeySelection)
        {
            _isInKeySelection = false;
            ItemSelectionCompletedCommand?.Execute(null);
        }
    }

    private void AssociatedObjectOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_isInClickSelection == false)
        {
            _isInClickSelection = true;

            ItemSelectionStartingCommand?.Execute(null);
        }
    }

    private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isInClickSelection = false;
        ItemSelectionCompletedCommand?.Execute(null);
    }
}