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
    public class BiaButtonTest : IClassFixture<MockAppFixture>, IDisposable
    {
        private readonly MockAppFixture _mockApp;

        public BiaButtonTest(MockAppFixture mockApp)
        {
            _mockApp = mockApp;

            var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<TabControl>().Single());
            tab.EmulateChangeSelectedIndex(2);
        }

        public void Dispose()
        {
            _mockApp.Check();
        }

        private WPFBiaButton FindControl(string name)
        {
            var ctrl = new WPFBiaButton(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

            // マウスボタン押下中になっていることがあるので戻す。
            ctrl.MouseUp(MouseButtonType.Left, 0, 0);

            return ctrl;
        }

        [Fact]
        // 左クリックのみできるか？
        public void Command()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ctrl = FindControl("BiaButtonViewModel.CommandA");

            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            Assert.Equal(3, (int) vm.BiaButtonViewModel.CountA);

            ctrl.Click(MouseButtonType.Middle, 5, 5);
            Assert.Equal(3, (int) vm.BiaButtonViewModel.CountA);

            ctrl.Click(MouseButtonType.Right, 5, 5);
            Assert.Equal(3, (int) vm.BiaButtonViewModel.CountA);
        }

        [Fact]
        // パラメータは考慮されているか？
        public void CommandWithParam()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ctrl = FindControl("BiaButtonViewModel.CommandB");

            ctrl.Click(MouseButtonType.Left, 5, 5);

            Assert.Equal("ParamB", (string) vm.BiaButtonViewModel.ResultB);
        }

        [Fact]
        // CanExecuteは考慮されているか？
        public void CommandCanExecute()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ctrl = FindControl("BiaButtonViewModel.CommandC");

            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            Assert.Equal(3, (int) vm.BiaButtonViewModel.CountC);

            ctrl.Click(MouseButtonType.Left, 5, 5);
            ctrl.Click(MouseButtonType.Left, 5, 5);
            Assert.Equal(3, (int) vm.BiaButtonViewModel.CountC);

            Assert.False(ctrl.IsEnabled);
        }
    }

    public class WPFBiaButton : WPFControlBase<BiaButton>
    {
        public WPFBiaButton(AppVar appVar) : base(appVar)
        {
        }
    }
}