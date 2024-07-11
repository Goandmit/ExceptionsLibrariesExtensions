using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для CreatingClientForm.xaml
    /// </summary>
    public partial class CreatingClientForm : Window
    {
        public CreatingClientForm()
        {
            InitializeComponent();
            DataContext = new CreatingClientFormVM();
        }
    }
}
