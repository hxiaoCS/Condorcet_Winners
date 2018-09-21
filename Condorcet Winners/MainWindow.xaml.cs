using System.Windows;

namespace Condorcet_Winners
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VM vm = new VM();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            vm.Compare();
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            vm.LoadVotes();
        }
    }
}
