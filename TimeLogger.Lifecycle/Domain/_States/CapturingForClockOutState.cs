using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class CapturingForClockOutState : CapturingState
    {
        public CapturingForClockOutState(ITimeLoggingConsumer consumer, IOfficeManager officeManager)
            : base(consumer, officeManager)
        {
        }

        public override List<States> AllowedFutureStates
        {
            get { return new List<States>() {States.Unregistering}; }
        }

        public override States StateDescriptor
        {
            get { return States.CapturingForClockOut; }
        }

        public override States ExecuteTask()
        {
            throw new System.NotImplementedException();
        }

        protected override States NextState
        {
            get { return States.Unregistering; }
        }
    }
}