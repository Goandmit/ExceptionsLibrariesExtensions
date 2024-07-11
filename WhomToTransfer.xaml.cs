using System.Windows;

namespace ExceptionsLibrariesExtensions
{
    /// <summary>
    /// Логика взаимодействия для WhomToTransfer.xaml
    /// </summary>
    public partial class WhomToTransfer : Window
    {
        public WhomToTransfer()
        {
            InitializeComponent();
            DataContext = new WhomToTransferVM();
        }
    }
}
