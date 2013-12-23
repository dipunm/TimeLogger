namespace TimeLogger.Wpf.ViewModels
{
    public class StateChangedEventArgs
    {
        public StateChangedEventArgs(State state)
        {
            State = state;
        }
        public State State { get; private set; }
    }
}