using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для CreatingAccountForm.xaml
    /// </summary>
    public partial class CreatingAccountForm : Window
    {
        public CreatingAccountForm()
        {
            InitializeComponent();
            DataContext = new CreatingAccountFormVM();
        }
    }
}
