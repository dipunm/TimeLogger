using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using TimeLogger.Core.Data;

namespace TimeLogger.Domain.Data
{
    public class RavenBasedWorkRepository : IWorkRepository
    {
        private readonly EmbeddableDocumentStore _documentStore;

        public RavenBasedWorkRepository(string path)
        {
            _documentStore = new EmbeddableDocumentStore
                {
                    DataDirectory = path
                };
            _documentStore.Initialize();
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
                return session.Query<WorkLog>()
                              .Where(l => l.Date.Date == date.Date)
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
    }
}