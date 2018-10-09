using System.Runtime.CompilerServices;

namespace MyTask
{
    public interface IAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }
}