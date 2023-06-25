using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace StreamerUpdate.API
{
 
  public class YoutubeHandler
  {
    protected readonly string[] Scopes = {YouTubeService.Scope.Youtube};
    public YouTubeService? YoutubeService { get; private set; }
    public YoutubeHandler()
    {
    }

    public async Task Authenticate()
    {
      var clientSecretsFile = GoogleClientSecrets.FromFile("Resources/client_secrets.json");
      var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecretsFile.Secrets, Scopes, "user", CancellationToken.None);

      YoutubeService = new YouTubeService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = "Churchstreamer"
      });
    }

    public void ListBroadcasts()
    { 
      var listRequest = YoutubeService.LiveBroadcasts.List("snippet,contentDetails,status");
      listRequest.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
      listRequest.Mine = true;
      var response = listRequest.Execute();
      var t = response.Items[0].Snippet.Title;
      Console.Write(t);
    }
  }
}