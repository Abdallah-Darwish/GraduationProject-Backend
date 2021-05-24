using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DockerBroker.Services
{
    public class AsyncAutoResetEvent : IDisposable
    {
        private readonly Queue<TaskCompletionSource> _waiters = new();
        private bool _isSet = false;
        private object _lck = new();

        public AsyncAutoResetEvent(bool initialIsSet = true)
        {
            _isSet = initialIsSet;
        }
        public Task WaitOne()
        {
            lock (_lck)
            {
                if (_isSet)
                {
                    _isSet = false;
                    return Task.CompletedTask;
                }

                TaskCompletionSource waiter = new();
                _waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        public void Release()
        {
            lock (_lck)
            {
                if (_isSet){return;}

                if (_waiters.Count == 0)
                {
                    _isSet = true;
                    return;
                }
                _waiters.Dequeue().SetResult();
            }
        }

        public void Dispose()
        {
            foreach (var waiter in _waiters)
            {
                waiter.SetResult();
            }
            _waiters.Clear();
        }
    }
}