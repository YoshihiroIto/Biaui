using System.Windows;
using System.Windows.Controls;
using Biaui.ControlCatalog.Pages;

namespace Biaui.ControlCatalog
{
    public partial class MainWindow
    {
        public UserControl[] Pages { get; private set; }

        #region SelectedPage
        
        public UserControl SelectedPage
        {
            get => _SelectedPage;
            set
            {
                if (value != _SelectedPage)
                    SetValue(SelectedPageProperty, value);
            }
        }
        
        private UserControl _SelectedPage;
        
        public static readonly DependencyProperty SelectedPageProperty =
            DependencyProperty.Register(
                nameof(SelectedPage),
                typeof(UserControl),
                typeof(MainWindow),
                new PropertyMetadata(
                    default,
                    (s, e) =>
                    {
                        var self = (MainWindow) s;
                        self._SelectedPage = (UserControl)e.NewValue;
                    }));
        
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            Pages = new UserControl[]
            {
                new ButtonPage(),
                new ToggleButtonPage(),
                new CheckBoxPage()
            };

            SelectedPage = Pages[0];
        }
    }
}