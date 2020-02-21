using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Biaui.Internals;

namespace Biaui.Controls.Internals
{
    internal class BiaHamburgerMenuHost : Control
    {
        #region HamburgerMenuItem
        
        public BiaHamburgerMenu? MenuItem
        {
            get => _menuItem;
            set
            {
                if (value != _menuItem)
                    SetValue(MenuItemProperty, value);
            }
        }
        
        private BiaHamburgerMenu? _menuItem;
        
        public static readonly DependencyProperty MenuItemProperty =
            DependencyProperty.Register(
                nameof(MenuItem),
                typeof(BiaHamburgerMenu),
                typeof(BiaHamburgerMenuHost),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (BiaHamburgerMenuHost) s;
                        self._menuItem = (BiaHamburgerMenu)e.NewValue;

                        self.UpdateMenuItem();
                    }));

        #endregion

        static BiaHamburgerMenuHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BiaHamburgerMenuHost),
                new FrameworkPropertyMetadata(typeof(BiaHamburgerMenuHost)));
        }
        
        public override void OnApplyTemplate()
        {
            UpdateMenuItem();
        }
        
        private void UpdateMenuItem()
        {
            var menu = this.Descendants().OfType<Menu?>().FirstOrDefault();
            if (menu == null)
                return;

            menu.Items.Clear();
            
            if (MenuItem == null)
                return;

            menu.Items.Add(MenuItem);
        }
    }
}