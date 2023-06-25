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

        public List<LiveBroadcast> GetAllBroadcasts()
        {
            var listRequest = YoutubeService.LiveBroadcasts.List("snippet,contentDetails,status");
            listRequest.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
            listRequest.Mine = true;
            var response = listRequest.Execute();
            return response.Items.ToList();
        }

        public void KillPendingBroadcasts()
        {
            var broadcasts = GetAllBroadcasts();
            foreach (var broadcast in broadcasts)
            {
                var transitionRequest = YoutubeService.LiveBroadcasts.Transition(
                    LiveBroadcastsResource.TransitionRequest.BroadcastStatusEnum.Complete, broadcast.Id, "status");
                var response = transitionRequest.Execute();
            }
        }
        
    }
}