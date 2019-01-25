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
    }
}
