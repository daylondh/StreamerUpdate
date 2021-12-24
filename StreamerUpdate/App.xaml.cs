﻿using Ninject;

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
    }

    private void ComposeObjects()
    {
      Current.MainWindow = this.container.Get<MainWindow>();
    }
  }
}
