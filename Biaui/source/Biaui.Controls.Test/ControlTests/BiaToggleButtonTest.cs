using System;
using System.Windows.Controls;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.KeyMouse;
using RM.Friendly.WPFStandardControls;
using Xunit;

namespace Biaui.Controls.Test.ControlTests
{
    [Collection("GUI")]
    public class BiaToggleButtonTest : IClassFixture<MockAppFixture>, IDisposable
    {
        private readonly MockAppFixture _mockApp;

        public BiaToggleButtonTest(MockAppFixture mockApp)
        {
            _mockApp = mockApp;

            var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<TabControl>().Single());
            tab.EmulateChangeSelectedIndex(3);
        }

        public void Dispose()
        {
            _mockApp.Check();
        }

        private WPFBiaToggleButton FindControl(string name)
        {
            var ctrl = new WPFBiaToggleButton(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

            // マウスボタン押下中になっていることがあるので戻す。
            ctrl.MouseUp(MouseButtonType.Left, 0, 0);

            return ctrl;
        }

        [Fact]
        // 左クリックのみできるか？
        public void Command()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ctrl = FindControl("BiaToggleButtonViewModel.CommandA");

            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            Assert.Equal(3, (int) vm.BiaToggleButtonViewModel.CountA);

            ctrl.Click(MouseButtonType.Middle, 5, 5);
            Assert.Equal(3, (int) vm.BiaToggleButtonViewModel.CountA);

            ctrl.Click(MouseButtonType.Right, 5, 5);
            Assert.Equal(3, (int) vm.BiaToggleButtonViewModel.CountA);
        }

        [Fact]
        // パラメータは考慮されているか？
        public void CommandWithParam()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ctrl = FindControl("BiaToggleButtonViewModel.CommandB");

            ctrl.Click(MouseButtonType.Left, 5, 5);

            Assert.Equal("ParamB", (string) vm.BiaToggleButtonViewModel.ResultB);
        }

        [Fact]
        // CanExecuteは考慮されているか？
        public void CommandCanExecute()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ctrl = FindControl("BiaToggleButtonViewModel.CommandC");

            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            Assert.Equal(3, (int) vm.BiaToggleButtonViewModel.CountC);

            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            Assert.Equal(3, (int) vm.BiaToggleButtonViewModel.CountC);

            Assert.False(ctrl.IsEnabled);
        }

        [Fact]
        // CanExecuteは考慮されているか？
        public void IsChecked()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            // IsEnabled:True
            {
                var ctrl = FindControl("BiaToggleButtonViewModel.IsCheckedX");

                Assert.True(ctrl.IsEnabled);
                Assert.False(ctrl.IsChecked);

                ctrl.Click(MouseButtonType.Left, 5, 5);
                Assert.True(ctrl.IsChecked);
                Assert.True((bool) vm.BiaToggleButtonViewModel.IsCheckedX);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Right, 5, 5);
                Assert.True(ctrl.IsChecked);
                Assert.True((bool) vm.BiaToggleButtonViewModel.IsCheckedX);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Middle, 5, 5);
                Assert.True(ctrl.IsChecked);
                Assert.True((bool) vm.BiaToggleButtonViewModel.IsCheckedX);
            }

            // IsEnabled:False
            {
                var ctrl = FindControl("BiaToggleButtonViewModel.IsCheckedY");

                Assert.False(ctrl.IsEnabled);
                Assert.False(ctrl.IsChecked);

                ctrl.Click(MouseButtonType.Left, 5, 5);
                Assert.False(ctrl.IsChecked);
                Assert.False((bool) vm.BiaToggleButtonViewModel.IsCheckedY);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Right, 5, 5);
                Assert.False(ctrl.IsChecked);
                Assert.False((bool) vm.BiaToggleButtonViewModel.IsCheckedY);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Middle, 5, 5);
                Assert.False(ctrl.IsChecked);
                Assert.False((bool) vm.BiaToggleButtonViewModel.IsCheckedY);
            }
        }
    }

    public class WPFBiaToggleButton : WPFControlBase<BiaToggleButton>
    {
        public WPFBiaToggleButton(AppVar appVar) : base(appVar)
        {
        }

        public bool IsChecked
        {
            get => Getter<bool>(nameof(IsChecked));
            set => InvokeStatic(EmulateChangeIsChecked, value);
        }

        private static void EmulateChangeIsChecked(BiaToggleButton c, bool value) => c.IsChecked = value;
    }
}