using Ninject.Modules;
using System.Net.Http;
using Google.Apis.YouTube.v3;
using StreamerUpdate.API;
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
      Bind<YoutubeHandler>().ToSelf().InSingletonScope();
    }
  }
}
