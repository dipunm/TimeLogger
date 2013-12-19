using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeLogger.Lifecycle.Core;

namespace TimeLogger.Lifecycle.Domain.States
{
    /// <summary>
    /// At the start of the application, 
    /// before a user has started the working day! 
    /// </summary>
    public class IdleState : State 
    {
    }

    /// <summary>
    /// When waiting paitiently for the timer
    /// to change our state
    /// </summary>
    public class SleepingState : State
    {
    }

    public class PromptState : State
    {
    }

    public class LoggingState : State
    {
    }

    public class ClosingState : State
    {
    }



}
