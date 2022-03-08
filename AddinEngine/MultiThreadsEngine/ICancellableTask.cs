namespace MultiThreadsEngine
{
    public interface ICancellableTask
    {
        string Id { get; set; }
        void Cancel();
    }
}