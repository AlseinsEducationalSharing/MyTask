using System;
using System.Collections.Generic;

namespace MyTask
{
    class MyTaskCompletionSource<T>
    {
        private event Action _continuation;

        private bool _completed;

        private T _value;

        private Exception _exception;

        public MyTaskCompletionSource() => Task = new MyTask(new MyAwaiter(this));

        public void SetResult(T value)
        {
            _value = value;
            _completed = true;
            _continuation?.Invoke();
        }

        public void SetException(Exception exception)
        {
            _exception = exception;
            _completed = true;
            _continuation?.Invoke();
        }

        public void SetException(IEnumerable<Exception> exceptions) => SetException(new AggregateException(exceptions));

        public void SetCanceled() => SetException(new OperationCanceledException());

        public ITask<T> Task { get; }

        private class MyTask : ITask<T>
        {
            private IAwaiter<T> _awaiter;

            public MyTask(IAwaiter<T> awaiter) => _awaiter = awaiter;

            public IAwaiter<T> GetAwaiter() => _awaiter;
        }

        private class MyAwaiter : IAwaiter<T>
        {
            private MyTaskCompletionSource<T> _target;

            public MyAwaiter(MyTaskCompletionSource<T> target) => _target = target;

            public bool IsCompleted => _target._completed;

            public void OnCompleted(Action continuation) => _target._continuation += continuation;

            public T GetResult() => _target._exception == null ? _target._value : throw _target._exception;
        }
    }
}