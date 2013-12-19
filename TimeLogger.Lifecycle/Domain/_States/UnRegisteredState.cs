using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public class UnRegisteredState : State<States>
    {
        private readonly IOfficeManager _officeManager;

        public UnRegisteredState(IOfficeManager officeManager)
        {
            _officeManager = officeManager;
        }

        public override List<States> AllowedFutureStates
        {
            get { return new List<States> { States.Registered }; }
        }

        public override States StateDescriptor
        {
            get { return States.Unregistered; }
        }

        public override bool Transition(States previousState)
        {
            switch (previousState)
            {
                case States.CapturingForClockOut:
                    _officeManager.ClockOut();
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