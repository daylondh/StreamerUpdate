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

    private void StartStreamingClick(object sender, RoutedEventArgs e)
    {
      StartStreamingButton.IsEnabled = false;
    }

    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
      _appControl.Cleanup();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
      var vm = (DataContext as MainWindowViewModel);
      vm.AppControlItem = _appControl;
      vm.Init();
      this.Unloaded += (s, e) => { _appControl.Dispose(); };
    }
  }
}
