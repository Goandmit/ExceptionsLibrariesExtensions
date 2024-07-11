using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для NonDepositAccountForm.xaml
    /// </summary>
    public partial class NonDepositAccountForm : Window
    {
        public NonDepositAccountForm()
        {
            InitializeComponent();
            DataContext = new AccountFormsVM();
        }
    }
}
