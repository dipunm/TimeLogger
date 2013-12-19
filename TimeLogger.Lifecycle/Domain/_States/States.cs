using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public enum States
    {
        Unregistered,
        Registered,
        Sleeping,
        Capturing,
        CapturingForExit,
        CapturingForClockOut,
        Napping,
        Unregistering,
        UnregisteringForExit,
        Exit
    };

    public class OfficeManagerStateMachine : StateMachine<States>
    {
        public OfficeManagerStateMachine(IOfficeManager officeManager, ITimeLoggingConsumer consumer) 
            : base(new List<State<States>>
                {
                    new UnRegisteredState(officeManager),
                    new CapturingState(consumer, officeManager),
                    new NappingState(officeManager, consumer),
                    new RegisteringState(officeManager, consumer),
                    new SleepingState(officeManager, consumer),
                    new CapturingForClockOutState(consumer, officeManager),
                    new CapturingForExitState(consumer, officeManager)
                }, States.Unregistered)
        {
        }
    }
}