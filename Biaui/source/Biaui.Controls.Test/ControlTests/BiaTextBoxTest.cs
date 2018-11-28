using System;
using System.Threading;
using System.Windows.Forms;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.KeyMouse;
using RM.Friendly.WPFStandardControls;
using Xunit;

namespace Biaui.Controls.Test.ControlTests
{
    [Collection("GUI")]
    public class BiaTextBoxTest : IClassFixture<MockAppFixture>, IDisposable
    {
        private readonly MockAppFixture _mockApp;

        public BiaTextBoxTest(MockAppFixture mockApp)
        {
            _mockApp = mockApp;

            var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<System.Windows.Controls.TabControl>().Single());
            tab.EmulateChangeSelectedIndex(6);
        }

        public void Dispose()
        {
            _mockApp.Check();
        }

        private WPFBiaTextBox FindControl(string name)
        {
            var ctrl = new WPFBiaTextBox(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

            // マウスボタン押下中になっていることがあるので戻す。
            ctrl.MouseUp(MouseButtonType.Left, 0, 0);

            return ctrl;
        }

        [Fact]
        public void Text()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ca = FindControl("BiaTextBoxViewModel.TextA");
            var cb = FindControl("BiaTextBoxViewModel.TextB");
            var cc = FindControl("BiaTextBoxViewModel.TextC");

            ca.Text = "";
            cb.Text = "";
            cc.Text = "";

            {
                ca.Text = "X";
                Assert.Equal("X", ca.Text);
                Assert.Equal("X", (string) vm.BiaTextBoxViewModel.TextA);

                vm.BiaTextBoxViewModel.TextA = "0";
                Assert.Equal("0", ca.Text);
                Assert.Equal("0", (string) vm.BiaTextBoxViewModel.TextA);
            }

            // IsEnable:False
            {
                cb.Text = "X";
                Assert.Equal("X", cb.Text);
                Assert.Equal("X", (string) vm.BiaTextBoxViewModel.TextB);

                vm.BiaTextBoxViewModel.TextB = "0";
                Assert.Equal("0", cb.Text);
                Assert.Equal("0", (string) vm.BiaTextBoxViewModel.TextB);
            }

            // IsReadOnly:True
            {
                cc.Text = "X";
                Assert.Equal("X", cc.Text);
                Assert.Equal("X", (string) vm.BiaTextBoxViewModel.TextC);

                vm.BiaTextBoxViewModel.TextC = "0";
                Assert.Equal("0", cc.Text);
                Assert.Equal("0", (string) vm.BiaTextBoxViewModel.TextC);
            }
        }

        [Fact]
        public void KeyInput()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ca = FindControl("BiaTextBoxViewModel.TextA");
            var cb = FindControl("BiaTextBoxViewModel.TextB");
            var cc = FindControl("BiaTextBoxViewModel.TextC");

            ca.Text = "";
            cb.Text = "";
            cc.Text = "";

            {
                var x = ca.Size.Width / 2;
                var y = ca.Size.Height / 2;

                ca.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("123{ENTER}");
                Thread.Sleep(50);

                Assert.Equal("123", ca.Text);
                Assert.Equal("123", (string) vm.BiaTextBoxViewModel.TextA);
            }

            // IsEnable:False
            {
                var x = cb.Size.Width / 2;
                var y = cb.Size.Height / 2;

                cb.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("123{ENTER}");
                Thread.Sleep(50);

                Assert.Equal("", cb.Text);
                Assert.Equal("", (string) vm.BiaTextBoxViewModel.TextB);
            }

            // IsReadOnly:True
            {
                var x = cc.Size.Width / 2;
                var y = cc.Size.Height / 2;

                cc.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("123{ENTER}");
                Thread.Sleep(50);

                Assert.Equal("", cc.Text);
                Assert.Equal("", (string) vm.BiaTextBoxViewModel.TextC);
            }
        }

        [WpfFact]
        public void ClipboardCopy()
        {
            var ca = FindControl("BiaTextBoxViewModel.TextA");
            var cc = FindControl("BiaTextBoxViewModel.TextC");

            ca.Text = "ABC";
            cc.Text = "ABC";

            {
                var x = ca.Size.Width / 2;
                var y = ca.Size.Height / 2;

                SetClipboard("123");

                ca.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("^a^c");
                Thread.Sleep(50);

                var data = GetClipboard();
                Assert.Equal("ABC", data);
            }

            // IsReadOnly:True
            {
                var x = cc.Size.Width / 2;
                var y = cc.Size.Height / 2;

                SetClipboard("123");

                cc.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("^a^c");
                Thread.Sleep(50);

                var data = GetClipboard();
                Assert.Equal("ABC", data);
            }
        }

        [WpfFact]
        public void ClipboardPaste()
        {
            var vm = _mockApp.MainWindow.Dynamic().DataContext;

            var ca = FindControl("BiaTextBoxViewModel.TextA");
            var cb = FindControl("BiaTextBoxViewModel.TextB");
            var cc = FindControl("BiaTextBoxViewModel.TextC");

            ca.Text = "ABC";
            cb.Text = "ABC";
            cc.Text = "ABC";

            {
                var x = ca.Size.Width / 2;
                var y = ca.Size.Height / 2;

                SetClipboard("777");

                ca.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("^a^v{ENTER}");
                Thread.Sleep(50);

                Assert.Equal("777", ca.Text);
                Assert.Equal("777", (string) vm.BiaTextBoxViewModel.TextA);
            }

            // IsEnable:False
            {
                var x = cb.Size.Width / 2;
                var y = cb.Size.Height / 2;

                SetClipboard("777");

                cb.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("^a^v{ENTER}");
                Thread.Sleep(50);

                Assert.Equal("ABC", cb.Text);
                Assert.Equal("ABC", (string) vm.BiaTextBoxViewModel.TextB);
            }

            // IsReadOnly:True
            {
                var x = cc.Size.Width / 2;
                var y = cc.Size.Height / 2;

                SetClipboard("777");

                cc.Click(MouseButtonType.Left, x, y);
                Thread.Sleep(50);
                SendKeys.SendWait("^a^v{ENTER}");
                Thread.Sleep(50);

                Assert.Equal("ABC", cc.Text);
                Assert.Equal("ABC", (string) vm.BiaTextBoxViewModel.TextC);
            }
        }

        private static void SetClipboard(string t)
        {
            while (true)
            {
                try
                {
                    System.Windows.Clipboard.SetText(t);
                    break;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }

        private static string GetClipboard()
        {
            while (true)
            {
                try
                {
                    return System.Windows.Clipboard.GetText();
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }

    }

    public class WPFBiaTextBox : WPFControlBase<BiaTextBox>
    {
        public WPFBiaTextBox(AppVar appVar) : base(appVar)
        {
        }

        public string Text
        {
            get => Getter<string>(nameof(Text));
            set => InvokeStatic(EmulateChangeText, value);
        }

        private static void EmulateChangeText(BiaTextBox c, string value) => c.Text = value;
    }
}