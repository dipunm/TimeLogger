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

        protected RavenBasedWorkRepository(EmbeddableDocumentStore dataStore)
        {
            _documentStore = dataStore;
            _documentStore.Initialize();
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