using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class CapturingForExitState : CapturingState
    {
        public CapturingForExitState(ITimeLoggingConsumer consumer, IOfficeManager officeManager) 
            : base(consumer, officeManager)
        {
        }

        public override List<States> AllowedFutureStates
        {
            get
            {
                return new List<States>() { States.UnregisteringForExit };
            }
        }

        public override States StateDescriptor
        {
            get
            {
                return States.CapturingForExit;
            }
        }

        public override States ExecuteTask()
        {
            throw new System.NotImplementedException();
        }

        protected override States NextState
        {
            get { return States.UnregisteringForExit; }
        }
    }
}