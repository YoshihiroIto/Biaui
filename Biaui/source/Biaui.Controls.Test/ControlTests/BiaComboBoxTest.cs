using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.KeyMouse;
using RM.Friendly.WPFStandardControls;
using Xunit;

namespace Biaui.Controls.Test.ControlTests;

[Collection("GUI")]
public class BiaComboboxTest : IClassFixture<MockAppFixture>, IDisposable
{
    private readonly MockAppFixture _mockApp;

    public BiaComboboxTest(MockAppFixture mockApp)
    {
        _mockApp = mockApp;

        var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<System.Windows.Controls.TabControl>().Single());
        tab.EmulateChangeSelectedIndex(7);
    }

    public void Dispose()
    {
        _mockApp.Check();
    }

    private WPFBiaCombobox FindControl(string name)
    {
        var ctrl = new WPFBiaCombobox(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

        // マウスボタン押下中になっていることがあるので戻す。
        ctrl.MouseUp(MouseButtonType.Left, 0, 0);

        return ctrl;
    }

    [Fact]
    public void SelectedItem()
    {
        var vm = _mockApp.MainWindow.Dynamic().DataContext;

        var items = (ObservableCollection<string>) vm.BiaComboBoxViewModel.LongItems;

        var ca = FindControl("BiaComboBoxViewModel.SelectedA");
        var cb = FindControl("BiaComboBoxViewModel.SelectedB");

        vm.BiaComboBoxViewModel.SelectedA = items[0];
        vm.BiaComboBoxViewModel.SelectedB = items[0];

        {
            Assert.Equal(items[0], (string) ca.SelectedItem);
            Assert.Equal(items[0], (string) vm.BiaComboBoxViewModel.SelectedA);

            ca.SelectedItem = items[10];
            Assert.Equal(items[10], (string) ca.SelectedItem);
            Assert.Equal(items[10], (string) vm.BiaComboBoxViewModel.SelectedA);

            vm.BiaComboBoxViewModel.SelectedA =items[20]; 
            Assert.Equal(items[20], (string) ca.SelectedItem);
            Assert.Equal(items[20], (string) vm.BiaComboBoxViewModel.SelectedA);
        }

        {
            Assert.Equal(items[0], (string) cb.SelectedItem);
            Assert.Equal(items[0], (string) vm.BiaComboBoxViewModel.SelectedB);

            cb.SelectedItem = items[10];
            Assert.Equal(items[10], (string) cb.SelectedItem);
            Assert.Equal(items[10], (string) vm.BiaComboBoxViewModel.SelectedB);

            vm.BiaComboBoxViewModel.SelectedB =items[20]; 
            Assert.Equal(items[20], (string) cb.SelectedItem);
            Assert.Equal(items[20], (string) vm.BiaComboBoxViewModel.SelectedB);
        }
    }

    [Fact]
    public void PopupListBox()
    {
        var vm = _mockApp.MainWindow.Dynamic().DataContext;

        var items = (ObservableCollection<string>) vm.BiaComboBoxViewModel.LongItems;

        var ca = FindControl("BiaComboBoxViewModel.SelectedA");
        var cb = FindControl("BiaComboBoxViewModel.SelectedB");
        var cc = FindControl("BiaComboBoxViewModel.SelectedC");

        vm.BiaComboBoxViewModel.SelectedA = items[0];
        vm.BiaComboBoxViewModel.SelectedB = items[0];

        // IsEnable:True
        {
            ca.Click(MouseButtonType.Left, 5, 5);
            ca.SendKey(Keys.Down);
            ca.SendKey(Keys.Enter);
            Assert.Equal(items[1], (string) ca.SelectedItem);
            Assert.Equal(items[1], (string) vm.BiaComboBoxViewModel.SelectedA);

            ca.Click(MouseButtonType.Left, 5, 5);
            ca.SendKey(Keys.Down);
            ca.SendKey(Keys.Down);
            ca.SendKey(Keys.Down);
            ca.SendKey(Keys.Enter);
            Assert.Equal(items[4], (string) ca.SelectedItem);
            Assert.Equal(items[4], (string) vm.BiaComboBoxViewModel.SelectedA);
        }

        // caのフォーカスを外す
        cc.Click();

        // IsEnable:False
        {
            cb.Click(MouseButtonType.Left, 5, 5);
            cb.SendKey(Keys.Down);
            cb.SendKey(Keys.Enter);
            Assert.Equal(items[0], (string) cb.SelectedItem);
            Assert.Equal(items[0], (string) vm.BiaComboBoxViewModel.SelectedB);

            cb.Click(MouseButtonType.Left, 5, 5);
            cb.SendKey(Keys.Down);
            cb.SendKey(Keys.Down);
            cb.SendKey(Keys.Down);
            cb.SendKey(Keys.Enter);
            Assert.Equal(items[0], (string) cb.SelectedItem);
            Assert.Equal(items[0], (string) vm.BiaComboBoxViewModel.SelectedB);
        }
    }

    [Fact]
    public void Wheel()
    {
        const int WheelDeltaUnit = 120;

        var vm = _mockApp.MainWindow.Dynamic().DataContext;

        var items = (ObservableCollection<string>) vm.BiaComboBoxViewModel.LongItems;

        var ca = FindControl("BiaComboBoxViewModel.SelectedA");
        var cb = FindControl("BiaComboBoxViewModel.SelectedB");
        var cc = FindControl("BiaComboBoxViewModel.SelectedC");

        vm.BiaComboBoxViewModel.SelectedA = items[0];
        vm.BiaComboBoxViewModel.SelectedB = items[0];

        // IsEnable:True
        {
            ca.Click(MouseButtonType.Left, 5, 5);
            ca.MouseWheel(-WheelDeltaUnit);
            Assert.Equal(items[1], (string) ca.SelectedItem);
            Assert.Equal(items[1], (string) vm.BiaComboBoxViewModel.SelectedA);

            ca.MouseWheel(-WheelDeltaUnit);
            Assert.Equal(items[2], (string) ca.SelectedItem);
            Assert.Equal(items[2], (string) vm.BiaComboBoxViewModel.SelectedA);
        }

        // caのフォーカスを外す
        cc.Click();

        // IsEnable:False
        {
            cb.Click(MouseButtonType.Left, 5, 5);
            cb.MouseWheel(-WheelDeltaUnit);
            Assert.Equal(items[0], (string) cb.SelectedItem);
            Assert.Equal(items[0], (string) vm.BiaComboBoxViewModel.SelectedB);

            cb.MouseWheel(-WheelDeltaUnit);
            Assert.Equal(items[0], (string) cb.SelectedItem);
            Assert.Equal(items[0], (string) vm.BiaComboBoxViewModel.SelectedB);
        }
    }
}

public class WPFBiaCombobox : WPFControlBase<BiaComboBox>
{
    public WPFBiaCombobox(AppVar appVar) : base(appVar)
    {
    }

    public object SelectedItem
    {
        get => Getter<object>(nameof(SelectedItem));
        set => InvokeStatic(EmulateChangeSelectedItem, value);
    }

    private static void EmulateChangeSelectedItem(BiaComboBox c, object value) => c.SelectedItem = value;
}