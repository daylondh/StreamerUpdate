using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.API;
using StreamerUpdate.Camera;
using StreamerUpdate.OBSInterop;
using System;
using System.Threading.Tasks;

namespace StreamerUpdate.MVVM.Model
{
  public class MainWindowModel : ReactiveObject
  {
    private Calendar _calendar;
    private readonly ServiceCalculator _serviceCalc;
    private readonly CalendarBuilder _calendarBuilder;
    public IObsRunner ObsRunner { get; }

    public MainWindowModel(Calendar calendar, ServiceCalculator serviceCalc, CalendarBuilder calendarBuilder,
        YoutubeHandler youtubeHandler, IObsRunner obsRunner, AudioInputMonitor audioInputMonitor)
    {
      InputMonitor = audioInputMonitor;
      ObsRunner = obsRunner;
      CameraState = CameraState.UNKNOWN;
      YoutubeHandler = youtubeHandler;
      _calendar = calendar;
      _serviceCalc = serviceCalc;
      _calendarBuilder = calendarBuilder;

    }

    public void Init()
    {
      Task.Run(() =>
      {
        var dt = DateTime.Now;
        _calendarBuilder.Build(dt.Year);
        _calendar.Print();
        var dateName = _calendar.GetName(dt.Month, dt.Day);
        var serviceName = _serviceCalc.CalculateService();
        ServiceName = $"{dateName} - {serviceName}";
        YoutubeHandler.Authenticate();
      });
    }

    public void StartStreaming()
    {
      InputMonitor.BeQuiet();
      ObsRunner.Go();
    }

    public void StopStreaming()
    {
      ObsRunner.Stop();
    }

    [Reactive]
    public CameraState CameraState { get; set; }

    [Reactive]
    public string ServiceName { get; set; }
    public AudioInputMonitor InputMonitor { get; }
    public YoutubeHandler YoutubeHandler { get; set; }
  }
}