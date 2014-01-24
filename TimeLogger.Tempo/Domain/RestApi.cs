using System;
using System.Net;
using System.Text.RegularExpressions;
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
            if (work.TicketCodes == null || work.TicketCodes.Count < 1)
                return;

            var distributedTime = (int) Math.Ceiling((float) work.Minutes/work.TicketCodes.Count);
            for (int i = 0; i < work.TicketCodes.Count; i++)
            {
                var ticketCode = work.TicketCodes[i].Trim().TrimEnd(',');
                var url = new Uri(_jiraBaseUrl, "/rest/tempo-rest/1.0/worklogs/" + ticketCode);
                var request = new RestRequest(url);
                
                
                //get remaining estimate
                var req = new RestRequest(
                    String.Format(
                        "/rest/tempo-rest/1.0/worklogs/remainingEstimate/calculate/{0}/{1}/{1}/0?username={2}",
                        ticketCode,
                        work.Date.ToString("dd-MMM-yy"),
                        sessionObj.Username
                        ));
                req.AddHeader("Accept", String.Empty);
                req.AddCookie("JSESSIONID", sessionObj.SessionId);
                var resp = _client.Get(req);
                
                var timeAsString = resp.Content;
                
                var matchHours = Regex.Match(timeAsString, @"(\d+)h");
                int nHours = 0;
                if(matchHours.Success)
                    nHours = int.Parse(matchHours.Groups[1].Value);

                var matchMinutes = Regex.Match(timeAsString, @"(\d+)m");
                int nMinutes = 0;
                if (matchMinutes.Success)
                    nMinutes = int.Parse(matchMinutes.Groups[1].Value);
                

                var remainingMins = nMinutes + (nHours * 60) - distributedTime;
                if(remainingMins > 0)
                    request.AddParameter("remainingEstimate", String.Format("{0}m", remainingMins));
                else
                    request.AddParameter("remainingEstimate", String.Format("0"));
                //end get remaining estimate

                request.AddCookie("JSESSIONID", sessionObj.SessionId);

                request.AddParameter("user", sessionObj.Username);
                request.AddParameter("type", "issue");
                request.AddParameter("issue", ticketCode, ParameterType.UrlSegment);
                request.AddParameter("issue", ticketCode);
                request.AddParameter("date", work.Date.ToString("dd/MMM/yy"));
                request.AddParameter("enddate", work.Date.ToString("dd/MMM/yy"));
                request.AddParameter("time", String.Format("{0}m", distributedTime));
                request.AddParameter("comment", work.Comment);
                
                var response = _client.Post(request);
            }
        }
    }
}
