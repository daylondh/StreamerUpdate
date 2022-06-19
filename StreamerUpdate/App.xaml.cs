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
    }

    private void ConfigureContainer()
    {
      this.container = new StandardKernel(new MainModule());
      container.Get<Youtube>();
      var builder = container.Get<CalendarBuilder>();
      builder.Build(2022);
      container.Get<Calendar>().Print();
    }

    private void ComposeObjects()
    {
      Current.MainWindow = this.container.Get<MainWindow>();
    }
  }
}
