namespace TimeLogger.Lifecycle.Core
{
    public interface IStatefulContext<T>
    {
        State<T> State { get; set; }
    }
}