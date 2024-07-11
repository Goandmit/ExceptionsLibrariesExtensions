using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для ReplenishAccountForm.xaml
    /// </summary>
    public partial class ReplenishAccountForm : Window
    {
        public ReplenishAccountForm()
        {
            InitializeComponent();
            DataContext = new ReplenishAccountFormVM();
        }
    }
}
