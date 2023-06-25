using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StreamerUpdate.API
{
    public class YoutubeHandler : ReactiveObject
    {
        protected readonly string[] Scopes = {YouTubeService.Scope.Youtube};

        public YouTubeService? YoutubeService { get; private set; }


        [Reactive] public bool Authenticated { get; set; }

        public async Task Authenticate()
        {
            var clientSecretsFile = GoogleClientSecrets.FromFile("Resources/client_secrets.json");
            if (clientSecretsFile == null)
                Trace.WriteLine("Cannot find client secrets; will not be able to authenticate!");

            var credential =
                await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecretsFile.Secrets, Scopes, "user",
                    CancellationToken.None);

            YoutubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Churchstreamer"
            });
            if (YoutubeService != null) Authenticated = true;
        }

        public List<LiveBroadcast> GetPendingBroadcasts()
        {
            var listRequest = YoutubeService.LiveBroadcasts.List("id");
            listRequest.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Upcoming;
            var response = listRequest.Execute();
            return response.Items.ToList();
        }

        public void KillPendingBroadcasts()
        {
            var broadcasts = GetPendingBroadcasts();
            foreach (var broadcast in broadcasts) YoutubeService.LiveBroadcasts.Delete(broadcast.Id).Execute();
        }

        public ChurchStream MakeStream(string title)
        {
            var cs = new ChurchStream(title);
            var broadcast = new LiveBroadcast
            {
                Kind = "youtube#liveBroadcast",
                Snippet = new LiveBroadcastSnippet
                {
                    Title = title,
                    ScheduledStartTime = DateTime.Now,
                    ScheduledEndTime = DateTime.Now.AddHours(2)
                },
                Status = new LiveBroadcastStatus {PrivacyStatus = "public"}
            };
            var insertRequest = YoutubeService.LiveBroadcasts.Insert(broadcast, "id,snippet,status");
            var response = insertRequest.Execute();
            cs.Broadcast = response;
            YoutubeService.LiveBroadcasts.Bind(response.Id, "id,snippet,contentDetails,status").Execute();
            return cs;

        }
    }
}