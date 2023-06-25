using System.ComponentModel;
using System.Windows;

namespace StreamerUpdate
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();

    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
      var vm = (DataContext as MainWindowViewModel);
      vm.Init();
    }

    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
      (DataContext as MainWindowViewModel).Cleanup();
    }
  }
}
