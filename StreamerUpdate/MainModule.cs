using Ninject.Modules;
using StreamerUpdate.API;
using StreamerUpdate.MVVM.Model;
using StreamerUpdate.OBSInterop;
using System.Net.Http;

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
      Bind<YoutubeHandler>().ToSelf().InSingletonScope();
      Bind<IObsRunner>().To<ObsRunner>().InSingletonScope();
    }
  }
}
