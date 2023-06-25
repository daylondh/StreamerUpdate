using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace StreamerUpdate.API
{
  public class YoutubeHandler
  {
    private YouTubeService service;
    public YoutubeHandler()
    {
    }

    public async Task authenticate()
    {
     
      UserCredential credential;

      using (var stream = new FileStream("Resources/client_secrets.json", FileMode.Open, FileAccess.Read))
      {
        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
          GoogleClientSecrets.FromStream(stream).Secrets,
          // This OAuth 2.0 access scope allows an application to upload files to the
          // authenticated user's YouTube channel, but doesn't allow other types of access.
          new[] { YouTubeService.Scope.Youtube },
          "user",
          CancellationToken.None
        );

      }

      this.service = new YouTubeService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = "Churchstreamer"
      });

    }

    public void listBroadcasts()
    { 


    }
  }
}