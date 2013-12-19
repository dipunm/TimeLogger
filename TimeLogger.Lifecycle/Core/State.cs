using System;
using System.Collections.Generic;

namespace TimeLogger.Lifecycle.Core
{
    public abstract class State<T>
    {
        public abstract List<T> AllowedFutureStates { get; }
        public abstract T StateDescriptor { get; }
        public abstract bool Transition(T previousState);
        public abstract T ExecuteTask();
    }

    public abstract class State
    {
        public abstract Dictionary<string, Action> GetExecutableActions();
    }

    ///
    /// Application State:  What actions can a user make?
    ///                     What actions should occur after logging time?
    ///                     Should the prompt window be shown?
    ///                     
    /// TESTING:            What it all started for.
    ///                     What do we want to test?
    ///                     States: Idle, Hidden, Prompt, Logger, 
    /// 
}