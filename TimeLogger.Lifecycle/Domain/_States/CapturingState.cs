using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class CapturingState : State<States>
    {
        private readonly ITimeLoggingConsumer _consumer;
        private readonly IOfficeManager _officeManager;

        public CapturingState(ITimeLoggingConsumer consumer, IOfficeManager officeManager)
        {
            _consumer = consumer;
            _officeManager = officeManager;
        }

        public override List<States> AllowedFutureStates
        {
            get 
            { 
                return new List<States>
                    {
                        States.Napping, 
                        States.Sleeping
                    }; 
            }
        }

        public override States StateDescriptor
        {
            get { return States.Capturing; }
        }

        public override bool Transition(States previousState)
        {
            switch (previousState)
            {
                case States.Sleeping:
                case States.Napping:
                    var timeToLog = _officeManager.GetTimeToLog();
                    _consumer.BeginLogTime(timeToLog, NextState);
                    return true;
                default:
                    return false;
            }
        }

        public override States ExecuteTask()
        {
            throw new System.NotImplementedException();
        }

        protected virtual States NextState {
            get { return States.Sleeping; }
        }
    }
}