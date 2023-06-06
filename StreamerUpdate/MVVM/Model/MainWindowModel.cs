using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.Camera;

namespace StreamerUpdate.MVVM.Model
{
    public class MainWindowModel : ReactiveObject
    {
        public MainWindowModel(Calendar calendar, CalendarBuilder calendarBuilder)
        {
            CameraState = CameraState.UNKNOWN;
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

        private Calendar _calendar;
    }
}