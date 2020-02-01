using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Biaui.Internals;

namespace Biaui.Controls
{
    public class BiaColorPicker : Control
    {
        #region Red

        public double Red
        {
            get => _Red;
            set
            {
                if (NumberHelper.AreClose(value, _Red) == false)
                    SetValue(RedProperty, Boxes.Double(value));
            }
        }

        private double _Red;

        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register(nameof(Red), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Red = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region Green

        public double Green
        {
            get => _Green;
            set
            {
                if (NumberHelper.AreClose(value, _Green) == false)
                    SetValue(GreenProperty, Boxes.Double(value));
            }
        }

        private double _Green;

        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register(nameof(Green), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Green = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region Blue

        public double Blue
        {
            get => _Blue;
            set
            {
                if (NumberHelper.AreClose(value, _Blue) == false)
                    SetValue(BlueProperty, Boxes.Double(value));
            }
        }

        private double _Blue;

        public static readonly DependencyProperty BlueProperty =
            DependencyProperty.Register(nameof(Blue), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Blue = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region Alpha

        public double Alpha
        {
            get => _Alpha;
            set
            {
                if (NumberHelper.AreClose(value, _Alpha) == false)
                    SetValue(AlphaProperty, Boxes.Double(value));
            }
        }

        private double _Alpha;

        public static readonly DependencyProperty AlphaProperty =
            DependencyProperty.Register(nameof(Alpha), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Alpha = (double) e.NewValue;
                    }));

        #endregion

        #region RedMaximum

        public double RedMaximum
        {
            get => _RedMaximum;
            set
            {
                if (NumberHelper.AreClose(value, _RedMaximum) == false)
                    SetValue(RedMaximumProperty, Boxes.Double(value));
            }
        }

        private double _RedMaximum = 1.0;

        public static readonly DependencyProperty RedMaximumProperty =
            DependencyProperty.Register(nameof(RedMaximum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._RedMaximum = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region GreenMaximum

        public double GreenMaximum
        {
            get => _GreenMaximum;
            set
            {
                if (NumberHelper.AreClose(value, _GreenMaximum) == false)
                    SetValue(GreenMaximumProperty, Boxes.Double(value));
            }
        }

        private double _GreenMaximum = 1.0;

        public static readonly DependencyProperty GreenMaximumProperty =
            DependencyProperty.Register(nameof(GreenMaximum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._GreenMaximum = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region BlueMaximum

        public double BlueMaximum
        {
            get => _BlueMaximum;
            set
            {
                if (NumberHelper.AreClose(value, _BlueMaximum) == false)
                    SetValue(BlueMaximumProperty, Boxes.Double(value));
            }
        }

        private double _BlueMaximum = 1.0;

        public static readonly DependencyProperty BlueMaximumProperty =
            DependencyProperty.Register(nameof(BlueMaximum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._BlueMaximum = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region AlphaMaximum

        public double AlphaMaximum
        {
            get => _AlphaMaximum;
            set
            {
                if (NumberHelper.AreClose(value, _AlphaMaximum) == false)
                    SetValue(AlphaMaximumProperty, Boxes.Double(value));
            }
        }

        private double _AlphaMaximum = 1.0;

        public static readonly DependencyProperty AlphaMaximumProperty =
            DependencyProperty.Register(nameof(AlphaMaximum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double1,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._AlphaMaximum = (double) e.NewValue;
                    }));

        #endregion

        #region RedMinimum

        public double RedMinimum
        {
            get => _RedMinimum;
            set
            {
                if (NumberHelper.AreClose(value, _RedMinimum) == false)
                    SetValue(RedMinimumProperty, Boxes.Double(value));
            }
        }

        private double _RedMinimum;

        public static readonly DependencyProperty RedMinimumProperty =
            DependencyProperty.Register(nameof(RedMinimum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._RedMinimum = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region GreenMinimum

        public double GreenMinimum
        {
            get => _GreenMinimum;
            set
            {
                if (NumberHelper.AreClose(value, _GreenMinimum) == false)
                    SetValue(GreenMinimumProperty, Boxes.Double(value));
            }
        }

        private double _GreenMinimum;

        public static readonly DependencyProperty GreenMinimumProperty =
            DependencyProperty.Register(nameof(GreenMinimum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._GreenMinimum = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region BlueMinimum

        public double BlueMinimum
        {
            get => _BlueMinimum;
            set
            {
                if (NumberHelper.AreClose(value, _BlueMinimum) == false)
                    SetValue(BlueMinimumProperty, Boxes.Double(value));
            }
        }

        private double _BlueMinimum;

        public static readonly DependencyProperty BlueMinimumProperty =
            DependencyProperty.Register(nameof(BlueMinimum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._BlueMinimum = (double) e.NewValue;
                        self.RgbToHsv();
                    }));

        #endregion

        #region AlphaMinimum

        public double AlphaMinimum
        {
            get => _AlphaMinimum;
            set
            {
                if (NumberHelper.AreClose(value, _AlphaMinimum) == false)
                    SetValue(AlphaMinimumProperty, Boxes.Double(value));
            }
        }

        private double _AlphaMinimum;

        public static readonly DependencyProperty AlphaMinimumProperty =
            DependencyProperty.Register(nameof(AlphaMinimum), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._AlphaMinimum = (double) e.NewValue;
                    }));

        #endregion

        #region IsVisibleAlphaEditor

        public bool IsVisibleAlphaEditor
        {
            get => _IsVisibleAlphaEditor;
            set
            {
                if (value != _IsVisibleAlphaEditor)
                    SetValue(IsVisibleAlphaEditorProperty, Boxes.Bool(value));
            }
        }

        private bool _IsVisibleAlphaEditor = true;

        public static readonly DependencyProperty IsVisibleAlphaEditorProperty =
            DependencyProperty.Register(
                nameof(IsVisibleAlphaEditor),
                typeof(bool),
                typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.BoolTrue,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._IsVisibleAlphaEditor = (bool) e.NewValue;
                    }));

        #endregion

        #region Hue

        public double Hue
        {
            get => _Hue;
            set
            {
                if (NumberHelper.AreClose(value, _Hue) == false)
                    SetValue(HueProperty, Boxes.Double(value));
            }
        }

        private double _Hue;

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(nameof(Hue), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Hue = (double) e.NewValue;
                        self.HsvToRgb();
                    }));

        #endregion

        #region Saturation

        public double Saturation
        {
            get => _Saturation;
            set
            {
                if (NumberHelper.AreClose(value, _Saturation) == false)
                    SetValue(SaturationProperty, Boxes.Double(value));
            }
        }

        private double _Saturation;

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Saturation = (double) e.NewValue;
                        self.HsvToRgb();
                    }));

        #endregion

        #region Value

        public double Value
        {
            get => _Value;
            set
            {
                if (NumberHelper.AreClose(value, _Value) == false)
                    SetValue(ValueProperty, Boxes.Double(value));
            }
        }

        private double _Value;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.Double0,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._Value = (double) e.NewValue;
                        self.HsvToRgb();
                    }));

        #endregion

        #region IsReadOnly

        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (value != _IsReadOnly)
                    SetValue(IsReadOnlyProperty, Boxes.Bool(value));
            }
        }

        private bool _IsReadOnly;

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BiaColorPicker),
                new PropertyMetadata(
                    Boxes.BoolFalse,
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._IsReadOnly = (bool) e.NewValue;
                    }));

        #endregion

        #region StartedContinuousEditingCommand

        public ICommand? StartedContinuousEditingCommand
        {
            get => _StartedContinuousEditingCommand;
            set
            {
                if (value != _StartedContinuousEditingCommand)
                    SetValue(StartedContinuousEditingCommandProperty, value);
            }
        }

        private ICommand? _StartedContinuousEditingCommand;

        public static readonly DependencyProperty StartedContinuousEditingCommandProperty =
            DependencyProperty.Register(
                nameof(StartedContinuousEditingCommand),
                typeof(ICommand),
                typeof(BiaColorPicker),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._StartedContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        #region EndContinuousEditingCommand

        public ICommand? EndContinuousEditingCommand
        {
            get => _EndContinuousEditingCommand;
            set
            {
                if (value != _EndContinuousEditingCommand)
                    SetValue(EndContinuousEditingCommandProperty, value);
            }
        }

        private ICommand? _EndContinuousEditingCommand;

        public static readonly DependencyProperty EndContinuousEditingCommandProperty =
            DependencyProperty.Register(
                nameof(EndContinuousEditingCommand),
                typeof(ICommand),
                typeof(BiaColorPicker),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._EndContinuousEditingCommand = (ICommand) e.NewValue;
                    }));

        #endregion

        #region StartedBatchEditingCommand

        public ICommand? StartedBatchEditingCommand
        {
            get => _StartedBatchEditingCommand;
            set
            {
                if (value != _StartedBatchEditingCommand)
                    SetValue(StartedBatchEditingCommandProperty, value);
            }
        }

        private ICommand? _StartedBatchEditingCommand;

        public static readonly DependencyProperty StartedBatchEditingCommandProperty =
            DependencyProperty.Register(
                nameof(StartedBatchEditingCommand),
                typeof(ICommand),
                typeof(BiaColorPicker),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._StartedBatchEditingCommand = (ICommand) e.NewValue;

                        if (self._wheelBackground != null)
                            self._wheelBackground.StartedBatchEditingCommand = self._StartedBatchEditingCommand;

                        if (self._boxBackground != null)
                            self._boxBackground.StartedBatchEditingCommand = self._StartedBatchEditingCommand;
                    }));

        #endregion

        #region EndBatchEditingCommand

        public ICommand? EndBatchEditingCommand
        {
            get => _EndBatchEditingCommand;
            set
            {
                if (value != _EndBatchEditingCommand)
                    SetValue(EndBatchEditingCommandProperty, value);
            }
        }

        private ICommand? _EndBatchEditingCommand;

        public static readonly DependencyProperty EndBatchEditingCommandProperty =
            DependencyProperty.Register(
                nameof(EndBatchEditingCommand),
                typeof(ICommand),
                typeof(BiaColorPicker),
                new PropertyMetadata(
                    default(ICommand),
                    (s, e) =>
                    {
                        var self = (BiaColorPicker) s;
                        self._EndBatchEditingCommand = (ICommand) e.NewValue;

                        if (self._wheelBackground != null)
                            self._wheelBackground.EndBatchEditingCommand = self._EndBatchEditingCommand;

                        if (self._boxBackground != null)
                            self._boxBackground.EndBatchEditingCommand = self._EndBatchEditingCommand;
                    }));

        #endregion

        static BiaColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaColorPicker),
                new FrameworkPropertyMetadata(typeof(BiaColorPicker)));
        }

        public BiaColorPicker()
        {
            SizeChanged += (_, e) =>
            {
                if (_redEditor == null)
                    return;

                if (e.NewSize.Width > 300)
                {
                    if (_redEditor != null)
                        _redEditor.Caption = "Red";

                    if (_greenEditor != null)
                        _greenEditor.Caption = "Green";

                    if (_blueEditor != null)
                        _blueEditor.Caption = "Blue";

                    if (_alphaEditor != null)
                        _alphaEditor.Caption = "Alpha";

                    if (_hueEditor != null)
                        _hueEditor.Caption = "Hue";

                    if (_saturationEditor != null)
                        _saturationEditor.Caption = "Saturation";

                    if (_valueEditor != null)
                        _valueEditor.Caption = "Value";
                }
                else
                {
                    if (_redEditor != null)
                        _redEditor.Caption = "R";

                    if (_greenEditor != null)
                        _greenEditor.Caption = "G";

                    if (_blueEditor != null)
                        _blueEditor.Caption = "B";

                    if (_alphaEditor != null)
                        _alphaEditor.Caption = "A";

                    if (_hueEditor != null)
                        _hueEditor.Caption = "H";

                    if (_saturationEditor != null)
                        _saturationEditor.Caption = "S";

                    if (_valueEditor != null)
                        _valueEditor.Caption = "V";
                }
            };
        }

        public override void OnApplyTemplate()
        {
            foreach (var e in this.Descendants().OfType<BiaNumberEditor>())
            {
                switch (e.Name)
                {
                    case "RedEditor":
                        _redEditor = e;
                        break;

                    case "GreenEditor":
                        _greenEditor = e;
                        break;

                    case "BlueEditor":
                        _blueEditor = e;
                        break;

                    case "AlphaEditor":
                        _alphaEditor = e;
                        break;

                    case "HueEditor":
                        _hueEditor = e;
                        break;

                    case "SaturationEditor":
                        _saturationEditor = e;
                        break;

                    case "ValueEditor":
                        _valueEditor = e;
                        break;
                }
            }

            _valueBar = this.Descendants().OfType<BiaColorBar>().First();
            _wheelBackground = this.Descendants().OfType<BiaHsvWheelBackground>().FirstOrDefault();
            _boxBackground = this.Descendants().OfType<BiaHsvBoxBackground>().FirstOrDefault();

            SetContinuousEditingCommand();
        }

        private BiaNumberEditor? _redEditor;
        private BiaNumberEditor? _greenEditor;
        private BiaNumberEditor? _blueEditor;
        private BiaNumberEditor? _alphaEditor;

        private BiaNumberEditor? _hueEditor;
        private BiaNumberEditor? _saturationEditor;
        private BiaNumberEditor? _valueEditor;

        private BiaColorBar? _valueBar;
        private BiaHsvWheelBackground? _wheelBackground;
        private BiaHsvBoxBackground? _boxBackground;

        private bool _isConverting;

        private void RgbToHsv()
        {
            if (_isConverting)
                return;

            _isConverting = true;

            StartedBatchEditingCommand?.ExecuteIfCan(null);

            (Hue, Saturation, Value) = ColorSpaceHelper.RgbToHsv(Red, Green, Blue);

            EndBatchEditingCommand?.ExecuteIfCan(null);

            _isConverting = false;
        }

        private void HsvToRgb()
        {
            if (_isConverting)
                return;

            _isConverting = true;

            StartedBatchEditingCommand?.ExecuteIfCan(null);

            (Red, Green, Blue) = ColorSpaceHelper.HsvToRgb(Hue, Saturation, NumberHelper.Clamp01(Value));

            EndBatchEditingCommand?.ExecuteIfCan(null);

            _isConverting = false;
        }

        private int _editingDepth;
        private (double, double, double) _continuousEditingStartValue;

        private void SetContinuousEditingCommand()
        {
            var started = new DelegateCommand(() =>
            {
                if (_editingDepth == 0)
                {
                    _continuousEditingStartValue = (Red, Green, Blue);

                    StartedContinuousEditingCommand?.ExecuteIfCan(null);
                }

                ++_editingDepth;
            });

            var end = new DelegateCommand(() =>
            {
                Debug.Assert(_editingDepth > 0);

                --_editingDepth;
                if (_editingDepth == 0)
                {
                    if (EndContinuousEditingCommand != null)
                    {
                        if (EndContinuousEditingCommand.CanExecute(null))
                        {
                            var changedValue = (Red, Green, Blue);

                            (Red, Green, Blue) = _continuousEditingStartValue;

                            EndContinuousEditingCommand.Execute(null);

                            (Red, Green, Blue) = changedValue;
                        }
                    }
                }
            });

            if (_redEditor != null)
            {
                _redEditor.StartedContinuousEditingCommand = started;
                _redEditor.EndContinuousEditingCommand = end;
            }

            if (_greenEditor != null)
            {
                _greenEditor.StartedContinuousEditingCommand = started;
                _greenEditor.EndContinuousEditingCommand = end;
            }

            if (_blueEditor != null)
            {
                _blueEditor.StartedContinuousEditingCommand = started;
                _blueEditor.EndContinuousEditingCommand = end;
            }

            if (_alphaEditor != null)
            {
                _alphaEditor.StartedContinuousEditingCommand = started;
                _alphaEditor.EndContinuousEditingCommand = end;
            }

            if (_hueEditor != null)
            {
                _hueEditor.StartedContinuousEditingCommand = started;
                _hueEditor.EndContinuousEditingCommand = end;
            }

            if (_saturationEditor != null)
            {
                _saturationEditor.StartedContinuousEditingCommand = started;
                _saturationEditor.EndContinuousEditingCommand = end;
            }

            if (_valueEditor != null)
            {
                _valueEditor.StartedContinuousEditingCommand = started;
                _valueEditor.EndContinuousEditingCommand = end;
            }

            if (_valueBar != null)
            {
                _valueBar.StartedContinuousEditingCommand = started;
                _valueBar.EndContinuousEditingCommand = end;
            }

            if (_wheelBackground != null)
            {
                _wheelBackground.StartedContinuousEditingCommand = started;
                _wheelBackground.EndContinuousEditingCommand = end;
            }

            if (_boxBackground != null)
            {
                _boxBackground.StartedContinuousEditingCommand = started;
                _boxBackground.EndContinuousEditingCommand = end;
            }
        }

        private class DelegateCommand : ICommand
        {
            private readonly Action _execute;

            public DelegateCommand(Action execute)
            {
                _execute = execute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            bool ICommand.CanExecute(object parameter) => true;
            void ICommand.Execute(object parameter) => _execute?.Invoke();
        }
    }
}