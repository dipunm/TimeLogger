namespace TimeLogger.Core
{
    public interface ITimer
    {
        double Interval { get; set; }
        bool AutoReset { get; set; }
        bool Enabled { get; set; }
        void Start();
        void Stop();
    }
}
