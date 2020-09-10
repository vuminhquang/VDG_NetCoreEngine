namespace AddinEngine.Abstract
{
    public interface ICancellableTask
    {
        string Id { get; set; }
        void Cancel();
    }
}
