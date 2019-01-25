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
    public class BiaCheckBoxTest : IClassFixture<MockAppFixture>, IDisposable
    {
        private readonly MockAppFixture _mockApp;

        public BiaCheckBoxTest(MockAppFixture mockApp)
        {
            _mockApp = mockApp;

            var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<TabControl>().Single());
            tab.EmulateChangeSelectedIndex(4);
        }

        public void Dispose()
        {
            _mockApp.Check();
        }

        private WPFBiaCheckBox FindControl(string name)
        {
            var ctrl = new WPFBiaCheckBox(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

            // マウスボタン押下中になっていることがあるので戻す。
            ctrl.MouseUp(MouseButtonType.Left, 0, 0);

            return ctrl;
        }

        [Fact]
        // CanExecuteは考慮されているか？
        public void IsChecked()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            // IsEnabled:True
            {
                var ctrl = FindControl("BiaCheckBoxViewModel.IsCheckedX");

                Assert.True(ctrl.IsEnabled);
                Assert.False(ctrl.IsChecked);

                ctrl.Click(MouseButtonType.Left, 5, 5);
                Assert.True(ctrl.IsChecked);
                Assert.True((bool) vm.BiaCheckBoxViewModel.IsCheckedX);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Right, 5, 5);
                Assert.True(ctrl.IsChecked);
                Assert.True((bool) vm.BiaCheckBoxViewModel.IsCheckedX);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Middle, 5, 5);
                Assert.True(ctrl.IsChecked);
                Assert.True((bool) vm.BiaCheckBoxViewModel.IsCheckedX);
            }

            // IsEnabled:False
            {
                var ctrl = FindControl("BiaCheckBoxViewModel.IsCheckedY");

                Assert.False(ctrl.IsEnabled);
                Assert.False(ctrl.IsChecked);

                ctrl.Click(MouseButtonType.Left, 5, 5);
                Assert.False(ctrl.IsChecked);
                Assert.False((bool) vm.BiaCheckBoxViewModel.IsCheckedY);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Right, 5, 5);
                Assert.False(ctrl.IsChecked);
                Assert.False((bool) vm.BiaCheckBoxViewModel.IsCheckedY);

                // 左クリックのみ
                ctrl.Click(MouseButtonType.Middle, 5, 5);
                Assert.False(ctrl.IsChecked);
                Assert.False((bool) vm.BiaCheckBoxViewModel.IsCheckedY);
            }
        }
    }

    public class WPFBiaCheckBox: WPFControlBase<BiaCheckBox>
    {
        public WPFBiaCheckBox(AppVar appVar) : base(appVar)
        {
        }

        public bool IsChecked
        {
            get => Getter<bool>(nameof(IsChecked));
            set => InvokeStatic(EmulateChangeIsChecked, value);
        }

        private static void EmulateChangeIsChecked(BiaCheckBox c, bool value) => c.IsChecked = value;
    }
}