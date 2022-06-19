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
      (DataContext as MainWindowViewModel).AppControlItem = _appControl;
      this.Unloaded += (s, e) => { _appControl.Dispose(); };
    }

    private void StartStreamingClick(object sender, RoutedEventArgs e)
    {
      StartStreamingButton.IsEnabled = false;
    }

    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
      _appControl.Cleanup();
    }
  }
}
