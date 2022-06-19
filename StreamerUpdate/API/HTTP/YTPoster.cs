using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamerUpdate.API
{
    // A lot of these can probably be abstracted into a Poster class which can then be used for other use cases (Twitch, Facebook, etc.) 
    public class YTPoster
    {
        private const string BASE_URL = "https://youtube.googleapis.com/youtube";
        private const string VERSION = "v3";
        private const string URL = BASE_URL + "/" + VERSION;
        private string API_KEY = "";
        private readonly HttpClient client;

        public YTPoster(string apiKey, HttpClient client)
        {
            this.client = client;
            API_KEY = apiKey;
        }

        /// <summary>
        ///     Executes an API call to method name with authentication
        /// </summary>
        /// <param name="method">The method you'd like to execute. HINT: to use nested methods, use 'topmethodname/lowermethodname'</param>
        /// <param name="parameters">Parameters</param>
        public Task<HttpResponseMessage> execute(string method, Dictionary<string, object> parameters)
        {
            parameters.Add("");
        }

        /// <summary>
        ///     Executes an API call to method name without authentication
        /// </summary>
        /// <param name="method">The method you'd like to execute. HINT: to use nested methods, use 'topmethodname/lowermethodname'</param>
        /// <param name="parameters">Parameters.</param>
        public static Task<HttpResponseMessage> rawExecute(string method, Dictionary<string, string> parameters)
        {
            var post = URL + "/" + method;
            var content = new FormUrlEncodedContent(parameters);
            
            return client.PostAsync(post, content);
        }
    }
}