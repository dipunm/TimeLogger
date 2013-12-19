using System;
using System.Collections.Generic;
using TimeLogger.Wpf.ViewModels;

namespace TimeLogger.Lifecycle.Core
{
    public class ExitState : State<States>
    {
        private readonly IApplication _application;

        public ExitState(IApplication application)
        {
            _application = application;
        }

        public override List<States> AllowedFutureStates
        {
            get { new List<States>(); }
        }

        public override States StateDescriptor
        {
            get { return States.Exit; }
        }

        public override bool Transition(States previousState)
        {
            switch (previousState)
            {
                case States.CapturingForExit:
                case States.UnregisteringForExit:
                case States.Unregistered:
                    _application.Shutdown();
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