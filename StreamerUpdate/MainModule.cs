﻿using Ninject.Modules;
using StreamerUpdate.API;
using System.Net.Http;

namespace StreamerUpdate
{
  class MainModule : NinjectModule
  {
    public override void Load()
    {
      Bind<HttpClient>().ToSelf().InSingletonScope();
      Bind<YTPoster>().ToSelf().InSingletonScope();
      Bind<Youtube>().ToSelf().InSingletonScope();

      Bind<Calendar>().ToSelf().InSingletonScope();
      Bind<CalendarBuilder>().ToSelf().InSingletonScope();
      Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
    }
  }
}
