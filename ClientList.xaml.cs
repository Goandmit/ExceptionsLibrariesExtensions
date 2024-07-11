using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для ClientList.xaml
    /// </summary>
    public partial class ClientList : Window
    {
        public ClientList()
        {
            InitializeComponent();
            DataContext = new ClientListVM();
        }
    }
}
