using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Embedded;
using TimeLogger.Core.Data;

namespace TimeLogger.Domain.Data
{
    public class RavenBasedWorkRepository : IWorkRepository
    {
        private readonly EmbeddableDocumentStore _documentStore;
        public RavenLogRepository(string path)
        {
            _documentStore = new EmbeddableDocumentStore()
                {
                    DataDirectory = path
                };
            _documentStore.Initialize();
        }

        public void AddLog(WorkLog log)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(log);
                session.SaveChanges();
            }
        }

        public IList<WorkLog> GetLogsForDate(DateTime date)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<WorkLog>()
                    .Where(l => l.Date.Date == date.Date)
                    .ToList();
            }
        }

        public void RemoveLog(WorkLog log)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Delete(log);
                session.SaveChanges();
            }
        }
    }
}
