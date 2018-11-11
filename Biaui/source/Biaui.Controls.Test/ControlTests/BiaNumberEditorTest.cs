using System;
using System.Threading;
using System.Windows.Forms;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly;
using Codeer.Friendly.Windows.KeyMouse;
using RM.Friendly.WPFStandardControls;
using Xunit;

namespace Biaui.Controls.Test.ControlTests
{
    [Collection("GUI")]
    public class BiaNumberEditorTest : IClassFixture<MockAppFixture>, IDisposable
    {
        private readonly MockAppFixture _mockApp;

        public BiaNumberEditorTest(MockAppFixture mockApp)
        {
            _mockApp = mockApp;
        }

        public void Dispose()
        {
            _mockApp.Check();
        }

        private WPFBiaNumberEditor FindControl(string name)
        {
            var ctrl = new WPFBiaNumberEditor(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

            // マウスボタン押下中になっていることがあるので戻す。
            ctrl.MouseUp(MouseButtonType.Left, 0, 0);

            return ctrl;
        }

        [Fact]
        public void MinMax()
        {
            var ctrl = FindControl("BiaNumberEditorViewModel.MinMaxValue");

            // バインディングで値を扱えているか
            Assert.Equal(50, ctrl.Value);

            // 直接値を入れて変更できるか？
            ctrl.Value = 40;
            Assert.Equal(40, ctrl.Value);

            // 最小値でのクランプ
            ctrl.Value = -10;
            Assert.Equal(0, ctrl.Value);

            // 最大値でのクランプ
            ctrl.Value = 110;
            Assert.Equal(100, ctrl.Value);

            // 最小値を変更しても値のクランプが行われるか
            ctrl.Value = 0;
            ctrl.Minimum = 10;
            Assert.Equal(10, ctrl.Value);

            // 最大値を変更しても値のクランプが行われるか
            ctrl.Value = 110;
            ctrl.Maximum = 100;
            Assert.Equal(100, ctrl.Value);
        }

        [Fact]
        public void Spin()
        {
            var ctrl = FindControl("BiaNumberEditorViewModel.SpinValue");

            var decX = 5;
            var decY = 5;

            var incX = ctrl.Size.Width - 5;
            var incY = 5;

            // 左クリック以外は反応しない
            ctrl.Click(MouseButtonType.Middle, 5, 5);
            ctrl.Click(MouseButtonType.Right, 5, 5);
            Assert.Equal(50, ctrl.Value);

            // -----------------------------------------------------
            // デクリメント　
            // -----------------------------------------------------
            ctrl.Value = 50;

            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(49, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, decX, decY);
            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(47, ctrl.Value);

            // Ctrl押下で５倍
            ctrl.App.KeyDown(Keys.ControlKey);
            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(42, ctrl.Value);
            ctrl.App.KeyUp(Keys.ControlKey);
            

            // 最小値クランプ
            ctrl.Value = 1;
            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(0, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(0, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(0, ctrl.Value);

            // -----------------------------------------------------
            // インクリメント
            // -----------------------------------------------------
            ctrl.Value = 50;

            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(51, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, incX, incY);
            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(53, ctrl.Value);

            // Ctrl押下で５倍
            ctrl.App.KeyDown(Keys.ControlKey);
            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(58, ctrl.Value);
            ctrl.App.KeyUp(Keys.ControlKey);


            // 最大値クランプ
            ctrl.Value = 99;
            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(100, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(100, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(100, ctrl.Value);

            // -----------------------------------------------------
            // リードオンリー
            // -----------------------------------------------------
            ctrl.Value = 50;
            ctrl.IsReadOnly = true;

            ctrl.Click(MouseButtonType.Left, decX, decY);
            Assert.Equal(50, ctrl.Value);

            ctrl.Click(MouseButtonType.Left, incX, incY);
            Assert.Equal(50, ctrl.Value);
        }

        [Fact]
        public void TextInput()
        {
            var ctrl = FindControl("BiaNumberEditorViewModel.TextInputValue");

            var x = ctrl.Size.Width / 2;
            var y = 5;

            // Enter押下での確定
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("12");
            ctrl.SendKey(Keys.Enter);
            Thread.Sleep(50);
            Assert.Equal(12, ctrl.Value);

            // Tab押下での確定
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("23");
            ctrl.SendKey(Keys.Tab);
            Thread.Sleep(50);
            Assert.Equal(23, ctrl.Value);

            // フォーカス外しで確定
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("34");
            ctrl.MouseMove(-5, -5);
            ctrl.Click(MouseButtonType.Left, -5, -5);
            Thread.Sleep(50);
            Assert.Equal(34, ctrl.Value);

            // ESC押下での確定
            ctrl.Value = 99;
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("99");
            ctrl.SendKey(Keys.Escape);
            Thread.Sleep(50);
            Assert.Equal(99, ctrl.Value);

            // 式を受け付ける
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("{(}1{+}2{)}{*}6");
            ctrl.SendKey(Keys.Enter);
            ctrl.SendKey(Keys.Enter);
            Thread.Sleep(50);
            Assert.Equal(18, ctrl.Value);

            // 式を評価後のキャンセル
            ctrl.Value = 99;
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("{(}1{+}2{)}{*}6");

            ctrl.SendKey(Keys.Enter);
            ctrl.SendKey(Keys.Escape);
            Thread.Sleep(50);
            Assert.Equal(99, ctrl.Value);

            // 関数を受け付ける
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("pow{(}2,3{)}");
            ctrl.SendKey(Keys.Enter);
            ctrl.SendKey(Keys.Enter);
            Thread.Sleep(50);
            Assert.Equal(8, ctrl.Value);

            // 定数を受け付ける
            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("PI");
            ctrl.SendKey(Keys.Enter);
            ctrl.SendKey(Keys.Enter);
            Thread.Sleep(50);
            Assert.Equal(3.142, ctrl.Value);

            // リードオンリーで編集できない
            ctrl.Value = 70;
            ctrl.IsReadOnly = true;

            ctrl.Click(MouseButtonType.Left, x, y);
            ctrl.SendKeys("12");
            ctrl.SendKey(Keys.Enter);
            Thread.Sleep(50);
            Assert.Equal(70, ctrl.Value);
        }

        //public static IEnumerable<object[]> SliderTestData
        //    => Enum.GetValues(typeof(BiaNumberEditorMode)).OfType<BiaNumberEditorMode>().Select(x => new object[] {x});

        // todo:テスト時WideRangeは安定しないのでテストから外す
        [Theory]
        //[MemberData(nameof(SliderTestData))]
        [InlineData(BiaNumberEditorMode.Simple)]
        public void Slider(BiaNumberEditorMode mode)
        {
            var ctrl = FindControl("BiaNumberEditorViewModel.SliderValue" + mode);

            ctrl.Mode = mode;

            var w = ctrl.Size.Width;
            var x = w / 2;
            var y = 5;

            ctrl.SliderMinimum = 25;
            ctrl.SliderMaximum = 75;

            // 左にスライド
            ctrl.Value = 50;
            ctrl.MouseDown(MouseButtonType.Left, x, y);
            ctrl.MouseUp(MouseButtonType.Left, x - 10, y);
            Assert.True(ctrl.Value < 50);

            // 右にスライド
            ctrl.Value = 50;
            ctrl.MouseMove(x, y);
            ctrl.MouseDown(MouseButtonType.Left, x, y);
            ctrl.MouseUp(MouseButtonType.Left, x + 10, y);
            Assert.True(ctrl.Value > 50);

            // 最小スライダー値クランプ
            ctrl.Value = 25;
            ctrl.MouseMove(x, y);
            ctrl.MouseDown(MouseButtonType.Left, x, y);
            ctrl.MouseUp(MouseButtonType.Left, 0, y);
            Assert.Equal(25, ctrl.Value);

            // 最大スライダー値クランプ
            ctrl.Value = 75;
            ctrl.MouseMove(x, y);
            ctrl.MouseDown(MouseButtonType.Left, x, y);
            ctrl.MouseUp(MouseButtonType.Left, w, y);
            Assert.Equal(75, ctrl.Value);

            // リードオンリーで編集できない
            ctrl.Value = 50;
            ctrl.IsReadOnly = true;

            ctrl.MouseMove(x, y);
            ctrl.MouseDown(MouseButtonType.Left, x, y);
            ctrl.MouseUp(MouseButtonType.Left, w, y);
            Assert.Equal(50, ctrl.Value);
        }
    }

    public class WPFBiaNumberEditor : WPFControlBase<BiaNumberEditor>
    {
        public WPFBiaNumberEditor(AppVar appVar) : base(appVar)
        {
        }

        public double Value
        {
            get => Getter<double>("Value");
            set => InvokeStatic(EmulateChangeValue, value);
        }

        public double Maximum
        {
            get => Getter<double>("Maximum");
            set => InvokeStatic(EmulateChangeMaximum, value);
        }

        public double Minimum
        {
            get => Getter<double>("Minimum");
            set => InvokeStatic(EmulateChangeMinimum, value);
        }

        public double SliderMaximum
        {
            get => Getter<double>("SliderMaximum");
            set => InvokeStatic(EmulateChangeSliderMaximum, value);
        }

        public double SliderMinimum
        {
            get => Getter<double>("SliderMinimum");
            set => InvokeStatic(EmulateChangeSliderMinimum, value);
        }

        public bool IsReadOnly
        {
            get => Getter<bool>("IsReadOnly");
            set => InvokeStatic(EmulateChangeIsReadOnly, value);
        }

        public BiaNumberEditorMode Mode
        {
            get => Getter<BiaNumberEditorMode>("Mode");
            set => InvokeStatic(EmulateChangeMode, value);
        }

        private static void EmulateChangeValue(BiaNumberEditor c, double value) => c.Value = value;
        private static void EmulateChangeMinimum(BiaNumberEditor c, double value) => c.Minimum = value;
        private static void EmulateChangeMaximum(BiaNumberEditor c, double value) => c.Maximum = value;
        private static void EmulateChangeSliderMinimum(BiaNumberEditor c, double value) => c.SliderMinimum = value;
        private static void EmulateChangeSliderMaximum(BiaNumberEditor c, double value) => c.SliderMaximum = value;
        private static void EmulateChangeIsReadOnly(BiaNumberEditor c, bool value) => c.IsReadOnly = value;
        private static void EmulateChangeMode(BiaNumberEditor c, BiaNumberEditorMode value) => c.Mode = value;
    }
}