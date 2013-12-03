using System;
using TimeLogger.Tempo.Core;

namespace TimeLogger.Tempo.Domain
{
    public class RestApiProxy : ITempoProxy
    {
        private readonly Uri _jiraBaseUrl;

        public RestApiProxy(Uri jiraBaseUrl)
        {
            _jiraBaseUrl = jiraBaseUrl;
        }

        public string GetSessionToken()
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidSessionToken(string sessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void AddWorkLog(Data.Core.WorkLog work)
        {
            throw new System.NotImplementedException();
        }
    }
}
