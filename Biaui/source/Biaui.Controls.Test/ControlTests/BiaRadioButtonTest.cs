using System;
using System.Windows.Controls;
using Biaui.Controls.Test.Helper;
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.KeyMouse;
using RM.Friendly.WPFStandardControls;
using Xunit;

namespace Biaui.Controls.Test.ControlTests;

[Collection("GUI")]
public class BiaRadioButtonTest : IClassFixture<MockAppFixture>, IDisposable
{
    private readonly MockAppFixture _mockApp;

    public BiaRadioButtonTest(MockAppFixture mockApp)
    {
        _mockApp = mockApp;

        var tab = new WPFTabControl(_mockApp.MainWindow.LogicalTree().ByType<TabControl>().Single());
        tab.EmulateChangeSelectedIndex(5);
    }

    public void Dispose()
    {
        _mockApp.Check();
    }

    private WPFBiaRadioButton FindControl(string name)
    {
        var ctrl = new WPFBiaRadioButton(_mockApp.MainWindow.LogicalTree().ByBinding(name).Single());

        // マウスボタン押下中になっていることがあるので戻す。
        ctrl.MouseUp(MouseButtonType.Left, 0, 0);

        return ctrl;
    }

    [Fact]
    // CanExecuteは考慮されているか？
    public void IsChecked()
    {
        var vm = _mockApp.MainWindow.Dynamic().DataContext;

        var ax = FindControl("BiaRadioButtonViewModel.GroupA_X");
        var ay = FindControl("BiaRadioButtonViewModel.GroupA_Y");
        var az = FindControl("BiaRadioButtonViewModel.GroupA_Z");
        var bx = FindControl("BiaRadioButtonViewModel.GroupB_X");
        var by = FindControl("BiaRadioButtonViewModel.GroupB_Y");
        var bz = FindControl("BiaRadioButtonViewModel.GroupB_Z");
        var cx = FindControl("BiaRadioButtonViewModel.GroupC_X");
        var cy = FindControl("BiaRadioButtonViewModel.GroupC_Y");
        var cz = FindControl("BiaRadioButtonViewModel.GroupC_Z");

        ax.Click(MouseButtonType.Left, 5, 5);
        Assert.True(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.False(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.False(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        ay.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.True(ay.IsChecked);
        Assert.False(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.False(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        az.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.False(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        bx.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.True(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.False(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        by.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.True(by.IsChecked);
        Assert.False(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        bz.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.True(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        cx.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.True(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        cy.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.True(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        cz.Click(MouseButtonType.Left, 5, 5);
        Assert.False(ax.IsChecked);
        Assert.False(ay.IsChecked);
        Assert.True(az.IsChecked);
        Assert.False(bx.IsChecked);
        Assert.False(by.IsChecked);
        Assert.True(bz.IsChecked);
        Assert.False(cx.IsChecked);
        Assert.False(cy.IsChecked);
        Assert.False(cz.IsChecked);
        Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
        Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
        Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
        Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
        Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
        Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
        Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
        Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
        Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);

        // 左クリックのみ
        foreach (var b in new[] {MouseButtonType.Right, MouseButtonType.Middle})
        foreach (var r in new[] {ax, ay, az, bx, by, bz, cx, cy, cz})
        {
            r.Click(b, 5, 5);
            Assert.False(ax.IsChecked);
            Assert.False(ay.IsChecked);
            Assert.True(az.IsChecked);
            Assert.False(bx.IsChecked);
            Assert.False(by.IsChecked);
            Assert.True(bz.IsChecked);
            Assert.False(cx.IsChecked);
            Assert.False(cy.IsChecked);
            Assert.False(cz.IsChecked);
            Assert.Equal(ax.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_X);
            Assert.Equal(ay.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Y);
            Assert.Equal(az.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupA_Z);
            Assert.Equal(bx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_X);
            Assert.Equal(by.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Y);
            Assert.Equal(bz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupB_Z);
            Assert.Equal(cx.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_X);
            Assert.Equal(cy.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Y);
            Assert.Equal(cz.IsChecked, (bool) vm.BiaRadioButtonViewModel.GroupC_Z);
        }
    }
}

public class WPFBiaRadioButton : WPFControlBase<BiaRadioButton>
{
    public WPFBiaRadioButton(AppVar appVar) : base(appVar)
    {
    }

    public bool IsChecked
    {
        get => Getter<bool>(nameof(IsChecked));
        set => InvokeStatic(EmulateChangeIsChecked, value);
    }

    private static void EmulateChangeIsChecked(BiaRadioButton c, bool value) => c.IsChecked = value;
}