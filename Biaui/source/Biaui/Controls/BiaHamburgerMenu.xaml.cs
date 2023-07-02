using System.Windows;
using System.Windows.Controls;

namespace Biaui.Controls;

public class BiaHamburgerMenu : MenuItem
{
    static BiaHamburgerMenu()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHamburgerMenu),
            new FrameworkPropertyMetadata(typeof(BiaHamburgerMenu)));
    }
}