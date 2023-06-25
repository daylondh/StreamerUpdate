using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.API;
using StreamerUpdate.Camera;
using StreamerUpdate.OBSInterop;

namespace StreamerUpdate.MVVM.Model
{
    public class MainWindowModel : ReactiveObject
    {
        private Calendar _calendar;
        public  IObsRunner ObsRunner { get; }

        public MainWindowModel(Calendar calendar, ServiceCalculator serviceCalc, CalendarBuilder calendarBuilder,
            YoutubeHandler youtubeHandler, IObsRunner obsRunner, AudioInputMonitor audioInputMonitor)
        {
            InputMonitor = audioInputMonitor;
            ObsRunner = obsRunner;
            CameraState = CameraState.UNKNOWN;
            YoutubeHandler = youtubeHandler;
          /*  YoutubeHandler.Authenticate().ContinueWith(t =>
            {
                if (YoutubeHandler.Authenticated)
                {
                    YoutubeHandler.KillPendingBroadcasts();
                }

            });
            */
            _calendar = calendar;
            var dt = DateTime.Now;
            calendarBuilder.Build(dt.Year);
            calendar.Print();
            var dateName = calendar.GetName(dt.Month, dt.Day);
            var serviceName = serviceCalc.CalculateService();
            ServiceName = $"{dateName} - {serviceName}";
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