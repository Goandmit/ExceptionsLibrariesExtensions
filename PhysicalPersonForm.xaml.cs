using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для PhysicalPersonForm.xaml
    /// </summary>
    public partial class PhysicalPersonForm : Window
    {
        public PhysicalPersonForm()
        {
            InitializeComponent();
            DataContext = new ClientFormsVM();
        }        
    }
}
