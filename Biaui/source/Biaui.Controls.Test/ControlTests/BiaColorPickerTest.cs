using System;
using System.Windows.Controls;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly;
using Codeer.Friendly.Windows.KeyMouse;
using RM.Friendly.WPFStandardControls;
using Xunit;

namespace Biaui.Controls.Test.ControlTests
{
    [Collection("GUI")]
    public class BiaColorPickerTest : IClassFixture<MockAppFixture>, IDisposable
    {
        private readonly MockAppFixture _mockApp;

        public BiaColorPickerTest(MockAppFixture mockApp)
        {
            _mockApp = mockApp;

            var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<TabControl>().Single());
            tab.EmulateChangeSelectedIndex(0);
        }

        public void Dispose()
        {
            _mockApp.Check();
        }

        // ReSharper disable once UnusedMember.Local
        private WPFBiaColorPicker FindControl(string name)
        {
            var ctrl = new WPFBiaColorPicker(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

            // マウスボタン押下中になっていることがあるので戻す。
            ctrl.MouseUp(MouseButtonType.Left, 0, 0);

            return ctrl;
        }

        [Fact]
        public void Simple()
        {
        }
    }

    public class WPFBiaColorPicker : WPFControlBase<BiaColorPicker>
    {
        public WPFBiaColorPicker(AppVar appVar) : base(appVar)
        {
        }

        public double Red
        {
            get => Getter<double>("Red");
            set => InvokeStatic(EmulateChangeRed, value);
        }

        public double Green
        {
            get => Getter<double>("Green");
            set => InvokeStatic(EmulateChangeGreen, value);
        }

        public double Blue
        {
            get => Getter<double>("Blue");
            set => InvokeStatic(EmulateChangeBlue, value);
        }

        public double Hue
        {
            get => Getter<double>("Hue");
            set => InvokeStatic(EmulateChangeHue, value);
        }

        public double Saturation
        {
            get => Getter<double>("Saturation");
            set => InvokeStatic(EmulateChangeSaturation, value);
        }

        public double Value
        {
            get => Getter<double>("Value");
            set => InvokeStatic(EmulateChangeValue, value);
        }

        public bool IsReadOnly
        {
            get => Getter<bool>("IsReadOnly");
            set => InvokeStatic(EmulateChangeIsReadOnly, value);
        }

        private static void EmulateChangeRed(BiaColorPicker c, double value) => c.Red = value;
        private static void EmulateChangeGreen(BiaColorPicker c, double value) => c.Green = value;
        private static void EmulateChangeBlue(BiaColorPicker c, double value) => c.Blue = value;

        private static void EmulateChangeHue(BiaColorPicker c, double value) => c.Hue = value;
        private static void EmulateChangeSaturation(BiaColorPicker c, double value) => c.Saturation = value;
        private static void EmulateChangeValue(BiaColorPicker c, double value) => c.Value = value;

        private static void EmulateChangeIsReadOnly(BiaColorPicker c, bool value) => c.IsReadOnly = value;
    }
}