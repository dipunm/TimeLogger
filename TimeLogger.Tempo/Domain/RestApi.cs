using System;
using RestSharp;
using TimeLogger.Cache.Core;
using TimeLogger.Tempo.Core;

namespace TimeLogger.Tempo.Domain
{
    public class RestApiProxy : ITempoProxy
    {
        private readonly Uri _jiraBaseUrl;
        private readonly RestClient _client;

        public RestApiProxy(Uri jiraBaseUrl)
        {
            _client = new RestClient();
            _jiraBaseUrl = jiraBaseUrl;
        }

        public string GetSessionToken()
        {
            var url = new Uri(_jiraBaseUrl, "/jira/rest/login");
            var request = new RestRequest(url);
            var response = _client.Post(request);
            response.
        }

        public bool IsValidSessionToken(string sessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void AddWorkLog(WorkLog work)
        {
            throw new System.NotImplementedException();
        }
    }
}
