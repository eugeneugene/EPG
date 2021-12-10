using System.Runtime.InteropServices;

namespace BloomCS
{
    public class AutoPinner : IDisposable
    {
        GCHandle _pinnedArray;
        private bool disposedValue;

        public AutoPinner(object obj)
        {
            _pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }
        public static implicit operator IntPtr(AutoPinner ap)
        {
            return ap._pinnedArray.AddrOfPinnedObject();
        }
        public void Dispose1()
        {
            _pinnedArray.Free();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pinnedArray.Free();
                }

                disposedValue = true;
            }
        }

        ~AutoPinner()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
