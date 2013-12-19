using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class NappingState : State<States>
    {
        private readonly IOfficeManager _officeManager;
        private readonly ITimeLoggingConsumer _consumer;

        public NappingState(IOfficeManager officeManager, ITimeLoggingConsumer consumer)
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
            get { return States.Napping; }
        }
        
        public override bool Transition(States previousState)
        {
            switch (previousState)
            {
                case States.Capturing:
                    _consumer.Reset();
                    _officeManager.RemindMeInABit();
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