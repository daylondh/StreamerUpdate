using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamerUpdate.API.HTTP
{
    public interface Poster
    {
        


        /// <summary>
        /// Executes an API call to method name with authentication
        /// </summary>
        /// <param name="method">The method you'd like to execute. HINT: to use nested methods, use 'topmethodname/lowermethodname'</param>
        /// <param name="parameters">Parameters</param>
         void execute(string method, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes an API call to method name without authentication
        /// </summary>
        /// <param name="method">The method you'd like to execute. HINT: to use nested methods, use 'topmethodname/lowermethodname'</param>
        /// <param name="parameters">Parameters.</param>
         Task<HttpResponseMessage> rawExecute(string method, Dictionary<string, string> parameters);

    }
}