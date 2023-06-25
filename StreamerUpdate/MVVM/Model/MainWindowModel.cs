using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.API;
using StreamerUpdate.Camera;

namespace StreamerUpdate.MVVM.Model
{
    public class MainWindowModel : ReactiveObject
    {
        private Calendar _calendar;

        public MainWindowModel(Calendar calendar, CalendarBuilder calendarBuilder, YoutubeHandler youtubeHandler)
        {
            CameraState = CameraState.UNKNOWN;
            YoutubeHandler = youtubeHandler;
            YoutubeHandler.Authenticate().ContinueWith(t =>
            {
                if (YoutubeHandler.Authenticated)
                {
                    YoutubeHandler.KillPendingBroadcasts();
                }

            });
            _calendar = calendar;
            var dt = DateTime.Now;
            calendarBuilder.Build(dt.Year);
            calendar.Print();
            ServiceName = calendar.GetName(dt.Month, dt.Day);
        }

        [Reactive] 
        public CameraState CameraState { get; set; }

        [Reactive] 
        public string ServiceName { get; set; }

        public YoutubeHandler YoutubeHandler { get; set; }
    }
}