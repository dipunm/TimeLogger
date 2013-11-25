using System;

namespace TimeLogger.Core
{
    public interface IPromptManager
    {
        void Prompt(TimeSpan sleepAllowance, Action continueAction);
    }
}