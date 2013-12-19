using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using TimeLogger.Cache.Core;
using TimeLogger.Data.Core;

namespace TimeLogger.Data.Remote.Domain
{
    public class TempoArchive : IRemoteArchive
    {
        private readonly Uri _jiraBaseUrl;
        private readonly RestClient _client;
        private string _sessionId;
        private string _username;

        public static void BypassSslErrors()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;
        }

        public TempoArchive()
        {
            _client = new RestClient();
            _jiraBaseUrl = new Uri("https://jira.memhq.com:8443");
        }

        public Reason Authenticate(string username, string password)
        {
            var url = new Uri(_jiraBaseUrl, "/rest/gadget/1.0/login");
            var request = new RestRequest(url);
            request.AddParameter("os_username", username, ParameterType.GetOrPost);
            request.AddParameter("os_password", password, ParameterType.GetOrPost);
            request.AddParameter("os_captcha", String.Empty, ParameterType.GetOrPost);
            var response = _client.Post<AuthResult>(request);
            if (response.ResponseStatus == ResponseStatus.Completed &&
                response.Data != null && response.Data.SuccessStatus == "Success")
            {
                _sessionId = response.Cookies.First(c => c.Name == "JSESSIONID").Value;
                _username = username;
                return Reason.Success;
            }
            return Reason.Unknown;
        }

        private class AuthResult
        {
            public string SuccessStatus { get; set; }
        }

        public Reason AddWorkLog(WorkLog work)
        {
            if (work.TicketCodes != null && work.TicketCodes.Any())
            {
                var mins = (int) Math.Ceiling((double) work.Minutes/work.TicketCodes.Count());
                foreach (var ticketCode in work.TicketCodes)
                {
                    var url = new Uri(_jiraBaseUrl, "/rest/tempo-rest/1.0/worklogs/" + ticketCode);
                    var request = new RestRequest(url);
                    request.AddCookie("JSESSIONID", _sessionId);
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                    request.AddParameter("user", _username, ParameterType.GetOrPost);
                    request.AddParameter("type", "issue", ParameterType.GetOrPost);
                    request.AddParameter("issue", ticketCode, ParameterType.GetOrPost);
                    request.AddParameter("date", work.Date.ToString("dd/MMM/yy"), ParameterType.GetOrPost);
                    request.AddParameter("enddate", work.Date.ToString("dd/MMM/yy"), ParameterType.GetOrPost);
                    request.AddParameter("time", String.Format("{0}m", mins), ParameterType.GetOrPost);
                    request.AddParameter("comment", work.Comment, ParameterType.GetOrPost);

                    var response = _client.Post(request);

                    if (response.ResponseStatus != ResponseStatus.Completed)
                        return Reason.Network;

                    if (response.StatusCode == HttpStatusCode.Accepted)
                        return Reason.Success;
                }
            }
            return Reason.Unknown;
        }
    }
}
