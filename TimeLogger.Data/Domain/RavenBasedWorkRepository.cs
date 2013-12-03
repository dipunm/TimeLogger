using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using TimeLogger.Data.Core;

namespace TimeLogger.Data.Domain
{
    public class RavenBasedWorkRepository : IWorkRepository
    {
        private readonly EmbeddableDocumentStore _documentStore;

        private RavenBasedWorkRepository(EmbeddableDocumentStore dataStore)
        {
            _documentStore = dataStore;
            _documentStore.UseEmbeddedHttpServer = true;
        }

        public RavenBasedWorkRepository(string path) 
            : this(new EmbeddableDocumentStore()
            {
                DataDirectory = path
            })
        {
        }

        public RavenBasedWorkRepository()
            : this(new EmbeddableDocumentStore()
            {
                RunInMemory = true
            })
        {
        }
        
        public void Initialise()
        {
            _documentStore.Initialize();
            if (!_documentStore.DatabaseCommands.GetIndexNames(0, 3).Contains("WorkLogs/ByDate"))
            {
                _documentStore.DatabaseCommands.PutIndex("WorkLogs/ByDate", new IndexDefinitionBuilder<WorkLog, DateTime>()
                    {
                        Map = logs => from log in logs
                                      select new { log.Date }
                    });
            }
        }

        public void AddLog(WorkLog log)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Store(log);
                session.SaveChanges();
            }
        }

        public IList<WorkLog> GetLogsForDate(DateTime date)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                return session.Query<WorkLog>("WorkLogs/ByDate")
                              .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromMinutes(4)))
                              .Where(l => l.Date == date)
                              .ToList();
            }
        }

        public void RemoveLog(WorkLog log)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                session.Delete(log);
                session.SaveChanges();
            }
        }

        public Uri GetManagerUrl()
        {
            return new Uri(_documentStore.Configuration.ServerUrl);
        }
    }
}