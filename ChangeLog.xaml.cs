using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для ChangeLog.xaml
    /// </summary>
    public partial class ChangeLog : Window
    {
        public ChangeLog()
        {
            InitializeComponent();
            DataContext = new ChangeLogVM();
        }
    }
}
