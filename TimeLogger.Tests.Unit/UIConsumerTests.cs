using NUnit.Framework;

namespace TimeLogger.Tests.Unit
{
    [TestFixture]
    public class UIConsumerTests
    {
        public void GetTimings_NotYetSet_LaunchesWelcomeWindow(){}
        public void GetTimings_WelcomeWindowReturnedFalse_ReLaunchWelcomeWindow(){}
        public void GetTimings_AlreadySet_WelcomeWindowNotLaunched(){}
        public void GetStartTime_NotYetSet_LaunchesWelcomeWindow(){}
        public void GetStartTime_WelcomeWindowReturnedFalse_ReLaunchWelcomeWindow(){}
        public void GetStartTime_AlreadySet_WelcomeWindowNotLaunched(){}
        public void LogTime_EndTimeHasBeenSet_NoPromptWindow(){}
        public void LogTime_StartTimeSet_PromptWindow(){}
        public void LogTime_PromptReturnsFalse_OfficeManagerAskedToRemindMeInABit(){}
        public void LogTime_PromptReturnsFalseButSnoozeDisabled_OfficeManagerNotAskedToRemindMeInABit(){}
        public void LogTime_WorkShouldBeLogged_SetCompleteActionCalled(){}
        public void LogTime_WorkShouldBeLogged_TimeTakenToRespondIsAddedToTimeToLog(){}
        public void LogTime_WorkShouldBeLogged_ResetCalledWithRoundedMinutesValue(){}
        public void LogTime_LoggerCompleted_OfficeManagerGivenWorkToLog(){}
        public void LogTime_LoggerCompleted_ExtraMinutesLoggedWithTimeLoggingTicket(){}
        public void GetEndTime_ReturnsNow(){}
        public void GetEndTime_ResetsStartTime(){}
        public void GetEndTime_NowThrowsException_ResetsStartTime() { }
    }
}
