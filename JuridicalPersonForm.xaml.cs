using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для JuridicalPersonForm.xaml
    /// </summary>
    public partial class JuridicalPersonForm : Window
    {
        public JuridicalPersonForm()
        {
            InitializeComponent();
            DataContext = new ClientFormsVM();
        }        
    }
}
