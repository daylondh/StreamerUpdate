using Ninject.Modules;
using System.Net.Http;
using StreamerUpdate.MVVM.Model;

namespace StreamerUpdate
{
  class MainModule : NinjectModule
  {
    public override void Load()
    {
      Bind<HttpClient>().ToSelf().InSingletonScope();
      Bind<MainWindowModel>().ToSelf().InSingletonScope();
      Bind<Calendar>().ToSelf().InSingletonScope();
      Bind<CalendarBuilder>().ToSelf().InSingletonScope();
      Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
    }
  }
}
