using System.Diagnostics;
using System.Windows;
using Biaui.Controls;

namespace Biaui.ControlCatalog.Pages
{
    public partial class ButtonPage
    {
        public ButtonPage()
        {
            Name = "Button";
            InitializeComponent();
        }

        private void BiaButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var b = (BiaHyperlinkButton) sender;
            
            using var proc = Process.Start(new ProcessStartInfo("cmd", $"/c start {b.Content}") {CreateNoWindow = true});

            proc?.WaitForExit();
        }
    }
}