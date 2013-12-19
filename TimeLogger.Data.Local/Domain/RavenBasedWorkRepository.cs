using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using TimeLogger.Cache.Core;
using TimeLogger.Data.Core;

namespace TimeLogger.Data.Local.Domain
{
    public class RavenBasedLocalRepository : ILocalRepository
    {
        private readonly EmbeddableDocumentStore _documentStore;

        private RavenBasedLocalRepository(EmbeddableDocumentStore dataStore)
        {
            _documentStore = dataStore;
            _documentStore.UseEmbeddedHttpServer = true;
        }

        public RavenBasedLocalRepository(string path) 
            : this(new EmbeddableDocumentStore()
            {
                DataDirectory = path
            })
        {
        }

        public RavenBasedLocalRepository()
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

        public void SendAllToArchive(IRemoteArchive archive)
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                var logs = session.Query<WorkLog>()
                       .Where(l => l.Archived == false)
                       .ToList();
                foreach (var work in logs)
                {
                   // archive.AddWorkLog(work, "");
                    work.Archived = true;
                    work.ArchiveDate = DateTime.Now;
                }
            }
        }
    }
}