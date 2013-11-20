using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Embedded;
using TimeLogger.Models;

namespace TimeLogger.Services
{
    public class RavenLogRepository : ILogRepository
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

        public IList<WorkLog> GetAllLogs()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<WorkLog>().ToList();
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
