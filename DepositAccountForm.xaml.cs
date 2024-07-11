using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для DepositAccountForm.xaml
    /// </summary>
    public partial class DepositAccountForm : Window
    {
        public DepositAccountForm()
        {
            InitializeComponent();
            DataContext = new AccountFormsVM();
        }
    }
}
