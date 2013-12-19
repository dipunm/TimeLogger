namespace TimeLogger.UserInterface.ViewModels
{
    public class ComposerState
    {
        public ComposerState(bool isLogging, bool hasTimeToLog, string description)
        {
            IsLogging = isLogging;
            HasTimeToLog = hasTimeToLog;
            Description = description;
        }

        public bool IsLogging { get; private set; }
        public bool HasTimeToLog { get; private set; }
        public string Description { get; private set; }

        public static ComposerState Idle
        {
            get { return new ComposerState(false, false, "Idle"); }
        }

        public static ComposerState Sleeping
        {
            get { return new ComposerState(false, true, "Sleeping"); }
        }

        public static ComposerState SleepingFresh
        {
            get { return new ComposerState(false, false, "Sleeping"); }
        }

        public static ComposerState Responding
        {
            get { return new ComposerState(true, false, "Logging"); }
        }
    }
}