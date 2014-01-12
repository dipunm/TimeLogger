using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Database.Server;
using TimeLogger.Cache.Core;

namespace TimeLogger.Main
{
    public class RavenTempStorage : ITempStorage
    {
        private readonly EmbeddableDocumentStore _documentStore;

        private RavenTempStorage(EmbeddableDocumentStore dataStore)
        {
            _documentStore = dataStore;
            _documentStore.UseEmbeddedHttpServer = true;
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(_documentStore.Configuration.Port);
        }

        public RavenTempStorage(string path) 
            : this(new EmbeddableDocumentStore()
            {
                DataDirectory = path
            })
        {
        }

        public RavenTempStorage()
            : this(new EmbeddableDocumentStore()
            {
                RunInMemory = true
            })
        {
        }
        
        public void Initialise()
        {
            _documentStore.Initialize();
            if (!_documentStore.DatabaseCommands.GetIndexNames(0, 5).Contains("WorkLogs/BySession"))
            {
                _documentStore.DatabaseCommands.PutIndex("WorkLogs/BySession", new IndexDefinitionBuilder<WorkLog, string>()
                    {
                        Map = logs => from log in logs
                                      select new { log.SessionToken }
                    });
            }

        }


        public List<WorkLog> GetAllLogs(string sessionKey)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                return session.Query<WorkLog>("WorkLogs/BySession")
                       .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromMinutes(4)))
                       .Where(l => l.SessionToken == sessionKey)
                       .ToList();
            }
        }

        public void AddRange(IEnumerable<WorkLog> logs, string sessionToken)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                foreach (var log in logs)
                {
                    log.SessionToken = sessionToken;
                    session.Store(log);
                }
                session.SaveChanges();
            }
        }

        public string GenerateSessionKey()
        {
            return Guid.NewGuid().ToString();
        }

        public IEnumerable<string> GetNonArchivedSessionKeys()
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                return session.Query<WorkLog>()
                    .Where(l =>  l.Archived != true)
                    .Select(l => l.SessionToken)
                    .Distinct()
                    .ToList();
            }
        }

        public Uri GetManagementUri()
        {
            return new Uri(_documentStore.Configuration.ServerUrl);
        }
    }
}