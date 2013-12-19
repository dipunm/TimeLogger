using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class SleepingState : State<States>
    {
        private readonly IOfficeManager _officeManager;
        private readonly ITimeLoggingConsumer _consumer;

        public SleepingState(IOfficeManager officeManager, ITimeLoggingConsumer consumer)
        {
            _officeManager = officeManager;
            _consumer = consumer;
        }

        public override List<States> AllowedFutureStates
        {
            get { return new List<States> { States.Capturing }; }
        }

        public override States StateDescriptor
        {
            get { return States.Sleeping; }
        }

        public override bool Transition(States previousState)
        {
            switch (previousState)
            {
                case States.Capturing:
                case States.Registered:
                    _officeManager.Sleep();
                    _consumer.Reset();
                    return true;
                default:
                    return false;
            }
        }

        public override States ExecuteTask()
        {
            throw new System.NotImplementedException();
        }
    }
}