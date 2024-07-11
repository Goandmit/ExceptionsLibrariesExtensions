using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для TransferForm.xaml
    /// </summary>
    public partial class TransferForm : Window
    {
        public TransferForm()
        {
            InitializeComponent();
            DataContext = new TransferFormVM();            
        }
    }
}
