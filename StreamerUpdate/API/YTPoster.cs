using System.Collections.Generic;
using System.Net.Http;

namespace StreamerUpdate.API
{
  // A lot of these can probably be abstracted into a Poster class which can then be used for other use cases (Twitch, Facebook, etc.) 
  public class YTPoster
  {
    private readonly HttpClient _client;
    private const string BASE_URL = "https://youtube.googleapis.com/youtube";
    private const string VERSION = "v3";
    private const string URL = BASE_URL + "/" + VERSION;

    public YTPoster(HttpClient client)
    {
      _client = client;
    }

    public string ApiKey { get; set; }


    /// <summary>
    /// Executes an API call to method name with authentication
    /// </summary>
    /// <param name="method">The method you'd like to execute. HINT: to use nested methods, use 'topmethodname/lowermethodname'</param>
    /// <param name="parameters">Parameters</param>
    public void execute(string method, Dictionary<string, object> parameters)
    {

    }
    /// <summary>
    /// Executes an API call to method name without authentication
    /// </summary>
    /// <param name="method">The method you'd like to execute. HINT: to use nested methods, use 'topmethodname/lowermethodname'</param>
    /// <param name="parameters">Parameters.</param>
    public void rawExecute(string method, Dictionary<string, string> parameters)
    {
      string post = URL + "/" + method;
      var content = new FormUrlEncodedContent(parameters);

    }


  }
}