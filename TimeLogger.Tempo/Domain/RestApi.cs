using System;
using System.Net;
using RestSharp;
using TimeLogger.Cache.Core;
using TimeLogger.Tempo.Core;

namespace TimeLogger.Tempo.Domain
{
    internal class TempoSession
    {
        public string SessionId { get; set; }
        public string Username { get; set; }
    }
    public class RestApiProxy : ITempoProxy
    {
        private readonly Uri _jiraBaseUrl;
        private readonly RestClient _client;

        public RestApiProxy(Uri jiraBaseUrl)
        {
            _client = new RestClient(jiraBaseUrl.GetLeftPart(UriPartial.Authority));
            _client.FollowRedirects = false;
            _jiraBaseUrl = jiraBaseUrl;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public object GetSessionToken(string username, string password)
        {
            var url = new Uri(_jiraBaseUrl, "/jira/rest/gadget/1.0/login");
            var request = new RestRequest(url);
            request.AddParameter("os_username", username);
            request.AddParameter("os_password", password);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = _client.Post(request);
            foreach (var cookie in response.Cookies)
            {
                if (cookie.Name == "JSESSIONID")
                    return new TempoSession {SessionId = cookie.Value, Username = username};
            }
            return null;
        }

        public bool IsValidSessionToken(object sessionToken)
        {
            var sessionObj = sessionToken as TempoSession;
            if (sessionObj == null) 
                return false;
            
            var url = new Uri(_jiraBaseUrl, "/browse/");
            var request = new RestRequest(url);
            request.AddCookie("JSESSIONID", sessionObj.SessionId);
            var response = _client.Get(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public void AddWorkLog(WorkLog work, object sessionToken)
        {
            var sessionObj = sessionToken as TempoSession;
            if (sessionObj == null) 
                throw new ArgumentException("Invalid sessionToken provided", "sessionToken");
            var distributedTime = (int) Math.Ceiling((float) work.Minutes/work.TicketCodes.Count);
            for (int i = 0; i < work.TicketCodes.Count; i++)
            {
                var url = new Uri(_jiraBaseUrl, "/rest/tempo-rest/1.0/worklogs/{issue}");
                var request = new RestRequest(url);
                request.AddCookie("JSESSIONID", sessionObj.SessionId);

                request.AddParameter("username", sessionObj.Username);
                request.AddParameter("type", "issue");
                request.AddParameter("issue", work.TicketCodes[i].Trim(), ParameterType.UrlSegment);
                request.AddParameter("issue", work.TicketCodes[i].Trim());
                request.AddParameter("date", work.Date.ToString("dd/MMM/yy"));
                request.AddParameter("enddate", work.Date.ToString("dd/MMM/yy"));
                request.AddParameter("time", String.Format("{0}m", distributedTime));
                request.AddParameter("comment", work.Comment);

                _client.Post(request);
            }
        }
    }
}
