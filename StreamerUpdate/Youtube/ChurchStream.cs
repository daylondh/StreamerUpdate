using System;
using Google.Apis.YouTube.v3.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StreamerUpdate.API
{
    public class ChurchStream : ReactiveObject
    {
        public ChurchStream(string name, YoutubeHandler youtubeHandler)
        {
            Name = name;
            YoutubeHandler = youtubeHandler;
        }
        
        
        public bool CreateBroadcast()
        {
            var snippet = new LiveBroadcastSnippet
            {
                Title = Name,
                ScheduledStartTime = DateTime.Now
            };
            var status = new LiveBroadcastStatus
            {
                PrivacyStatus = "private"
            };
            var broadcast = YoutubeHandler.MakeBroadcast(snippet, status);
            if (broadcast == null) return false;
            Broadcast = broadcast;
            return true;
        }

        public bool BindBroadcast()
        {
            return false;
        }

        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public LiveBroadcast? Broadcast { get; set; }
        [Reactive]
        public bool IsLive { get; set; }
        private YoutubeHandler YoutubeHandler { get; set; }
    }
}