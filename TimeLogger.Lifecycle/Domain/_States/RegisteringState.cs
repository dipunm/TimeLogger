using System;
using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class RegisteringState : State<States>
    {
        private readonly IOfficeManager _officeManager;
        private readonly ITimeLoggingConsumer _consumer;

        public RegisteringState(IOfficeManager officeManager, ITimeLoggingConsumer consumer)
        {
            _officeManager = officeManager;
            _consumer = consumer;
        }

        public override List<States> AllowedFutureStates
        {
            get { return new List<States> { States.Sleeping }; }
        }

        public override States StateDescriptor
        {
            get { return States.Registered; }
        }

        public override bool Transition(States previousState)
        {
            switch (previousState)
            {
                case States.Unregistered:
                    _officeManager.ClockIn(_consumer);
                    return true;
                default:
                    return false;
            }
        }

        public override States ExecuteTask()
        {
            throw new NotImplementedException();
        }
    }
}