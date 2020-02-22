using System.Diagnostics;
using System.Windows;
using Biaui.Controls;

namespace Biaui.Showcase
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new Data();
        }

        private void BiaButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var b = (BiaHyperlinkButton) sender;
            
            using var proc = Process.Start(new ProcessStartInfo("cmd", $"/c start {b.Content}") {CreateNoWindow = true});

            proc?.WaitForExit();
        }
    }
}
