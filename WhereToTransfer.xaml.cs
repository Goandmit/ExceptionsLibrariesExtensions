using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для WhereToTransfer.xaml
    /// </summary>
    public partial class WhereToTransfer : Window
    {
        public WhereToTransfer()
        {
            InitializeComponent();
            DataContext = new WhereToTransferVM();
        }
    }
}
