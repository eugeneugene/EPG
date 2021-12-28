using System;
using System.Threading;

namespace BFM.Code
{
    public class SimpleReaderWriterLock<T> : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new();
        private T _value;
        private bool disposedValue;

        public SimpleReaderWriterLock(T Value)
        {
            _value = Value;
        }

        public T Value
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _value;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _value = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _lock.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
