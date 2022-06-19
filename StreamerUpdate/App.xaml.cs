using System;
using System.Diagnostics;
using Ninject;
using StreamerUpdate.API;

namespace StreamerUpdate
{
  using System.Windows;

  public partial class App
  {
    private IKernel container;

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      ConfigureContainer();
      ComposeObjects();
      Current.MainWindow.Show();
     
      YoutubeHandler handler = new YoutubeHandler();
      handler.authenticate().ContinueWith(task =>
      {
        handler.listBroadcasts();
      });
    }

    private void ConfigureContainer()
    {
      this.container = new StandardKernel(new MainModule());
    }

    private void ComposeObjects()
    {
      var mw = this.container.Get<MainWindow>();
      mw.DataContext = container.Get<MainWindowViewModel>();
      Current.MainWindow = mw;
    }
  }
}
