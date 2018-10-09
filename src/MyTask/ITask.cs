namespace MyTask
{
    public interface ITask<T>
    {
        IAwaiter<T> GetAwaiter();
    }
}