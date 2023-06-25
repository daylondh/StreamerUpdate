using Google.Apis.YouTube.v3.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StreamerUpdate.API
{
    public class ChurchStream : ReactiveObject
    {
        public ChurchStream(string name)
        {
            Name = name;
        }
        
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public LiveBroadcast? Broadcast { get; set; }
        [Reactive]
        public bool IsLive { get; set; }
    }
}