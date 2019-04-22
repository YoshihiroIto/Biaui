using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaFilteringComboBox : ComboBox
    {
        #region FilterWords

        public string FilterWords
        {
            get => _filterWords;
            set
            {
                if (value != _filterWords)
                    SetValue(FilterWordsProperty, value);
            }
        }

        private string _filterWords;

        public static readonly DependencyProperty FilterWordsProperty =
            DependencyProperty.Register(
                nameof(FilterWords),
                typeof(string),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._filterWords = (string) e.NewValue;
                    }));

        #endregion

        #region IsEnableMouseWheel

        public bool IsMouseWheelEnabled
        {
            get => _isMouseWheelEnabled;
            set
            {
                if (value != _isMouseWheelEnabled)
                    SetValue(IsMouseWheelEnabledProperty, value);
            }
        }

        private bool _isMouseWheelEnabled = true;

        public static readonly DependencyProperty IsMouseWheelEnabledProperty =
            DependencyProperty.Register(
                nameof(IsMouseWheelEnabled),
                typeof(bool),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._isMouseWheelEnabled = (bool) e.NewValue;
                    }));

        #endregion

        #region StartedContinuousEditingCommand

        public ICommand StartedContinuousEditingCommand
        {
            get => _StartedContinuousEditingCommand;
            set
            {
                if (value != _StartedContinuousEditingCommand)
                    SetValue(StartedContinuousEditingCommandProperty, value);
            }
        }

        private ICommand _StartedContinuousEditingCommand;

        public static readonly DependencyProperty StartedContinuousEditingCommandProperty =
            DependencyProperty.Register(
                nameof(StartedContinuousEditingCommand),
                typeof(ICommand),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._StartedContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        #region EndContinuousEditingCommand

        public ICommand EndContinuousEditingCommand
        {
            get => _EndContinuousEditingCommand;
            set
            {
                if (value != _EndContinuousEditingCommand)
                    SetValue(EndContinuousEditingCommandProperty, value);
            }
        }

        private ICommand _EndContinuousEditingCommand;

        public static readonly DependencyProperty EndContinuousEditingCommandProperty =
            DependencyProperty.Register(
                nameof(EndContinuousEditingCommand),
                typeof(ICommand),
                typeof(BiaFilteringComboBox),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaFilteringComboBox) s;
                        self._EndContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        static BiaFilteringComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaFilteringComboBox),
                new FrameworkPropertyMetadata(typeof(BiaFilteringComboBox)));
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (IsMouseWheelEnabled)
                return;

            if (IsDropDownOpen)
                return;

            var parentScrollViewer = this.GetParent<ScrollViewer>();
            if (parentScrollViewer == null)
                return;

            e.Handled = true;

            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = this
            };

            parentScrollViewer.RaiseEvent(args);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);

            if (StartedContinuousEditingCommand != null)
                if (StartedContinuousEditingCommand.CanExecute(null))
                    StartedContinuousEditingCommand.Execute(null);


            _ContinuousEditingStartValue = SelectedItem;
            _isSelectedOnPopup = false;

            var dropDown = FindDropDown();
            dropDown.PreviewMouseDown += DropDownOnPreviewMouseDown;
            dropDown.PreviewKeyDown += DropDownOnPreviewKeyDown;
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            var dropDown = FindDropDown();
            dropDown.PreviewMouseDown += DropDownOnPreviewMouseDown;
            dropDown.PreviewKeyDown += DropDownOnPreviewKeyDown;

            if (_isSelectedOnPopup)
                SetValue();
            else
                Discard();
        }

        private FrameworkElement FindDropDown() =>
            this.Descendants()
                .OfType<FrameworkElement>()
                .First(x => x.Name == "dropdown");

        private bool _isSelectedOnPopup;

        private void DropDownOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isSelectedOnPopup = true;
        }

        private void DropDownOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                _isSelectedOnPopup = true;
        }

        private object _ContinuousEditingStartValue;

        private void SetValue()
        {
            if (EndContinuousEditingCommand != null)
            {
                if (EndContinuousEditingCommand.CanExecute(null))
                {
                    var changedValue = SelectedItem;
                    SelectedItem = _ContinuousEditingStartValue;

                    EndContinuousEditingCommand.Execute(null);

                    SelectedItem = changedValue;
                }
            }
        }

        private void Discard()
        {
            var done = false;

            if (EndContinuousEditingCommand != null)
            {
                if (EndContinuousEditingCommand.CanExecute(null))
                {
                    SelectedItem = _ContinuousEditingStartValue;

                    EndContinuousEditingCommand.Execute(null);

                    done = true;
                }
            }

            if (done == false)
                SelectedItem = _ContinuousEditingStartValue;
        }
    }
}