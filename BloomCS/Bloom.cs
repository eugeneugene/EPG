using System.Runtime.InteropServices;

namespace BloomCS
{
    public class Bloom : IDisposable
    {
        private readonly IntPtr bloomContainer;
        private bool disposedValue;

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateBloom")]
        private static extern IntPtr _create_bloom();

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "DestroyBloom")]
        private static extern void _destroy_bloom([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetErrorClass")]
        private static extern int _get_error_class([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetErrorCode")]
        private static extern long _get_error_code([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetErrorMessage")]
        private static extern int _get_error_message([In] IntPtr BloomContainer, [In] IntPtr buffer, [In, Out] IntPtr bufferLength);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Create")]
        private static extern int _create([In] IntPtr BloomContainer, [In] string FileName);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Open")]
        private static extern int _open([In] IntPtr BloomContainer, [In] string FileName);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Store")]
        private static extern int _store([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Load")]
        private static extern int _load([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Close")]
        private static extern int _close([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Abort")]
        private static extern int _abort([In] IntPtr BloomContainer);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Allocate")]
        private static extern int _allocate([In] IntPtr BloomContainer, [In] uint Elements);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "PutString")]
        private static extern int _put_string([In] IntPtr BloomContainer, [In] string String);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "PutArray")]
        private static extern int _put_array([In] IntPtr BloomContainer, [In] byte[] buffer, [In] uint length);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CheckString")]
        private static extern int _check_string([In] IntPtr BloomContainer, [In] string String);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CheckArray")]
        private static extern int _check_array([In] IntPtr BloomContainer, [In] byte[] buffer, [In] uint length);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "HeaderVersion")]
        private static extern ushort _header_version([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "HeaderSize")]
        private static extern ulong _header_size([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "HeaderHashFunc")]
        private static extern byte _header_hash_func([In] IntPtr Bloom);

        private string GetErrorMessage()
        {
            IntPtr buffer = IntPtr.Zero;
            IntPtr bufferLength = Marshal.AllocHGlobal(sizeof(long));
            Marshal.WriteInt64(bufferLength, 0);
            if (_get_error_message(bloomContainer, buffer, bufferLength) == 0)
            {
                // Можем получить сообщение
                int size = (int)Marshal.ReadInt64(bufferLength) + 1;
                buffer = Marshal.AllocHGlobal(size);
                Marshal.WriteInt64(bufferLength, size);
                _ = _get_error_message(bloomContainer, buffer, bufferLength);
            }
            Marshal.FreeHGlobal(bufferLength);
            var ret = Marshal.PtrToStringUni(buffer);
            return ret ?? string.Empty;
        }

        public Bloom()
        {
            bloomContainer = _create_bloom();
            if (bloomContainer == IntPtr.Zero)
                throw new InvalidOperationException("Error creating Bloom Filter");
        }

        public void Create(string FileName)
        {
            var res = _create(bloomContainer, FileName);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void Open(string FileName)
        {
            var res = _open(bloomContainer, FileName);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void Store()
        {
            var res = _store(bloomContainer);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void Load()
        {
            var res = _load(bloomContainer);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void Close()
        {
            var res = _close(bloomContainer);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void Abort()
        {
            var res = _abort(bloomContainer);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void Allocate(uint Elements)
        {
            var res = _allocate(bloomContainer, Elements);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void PutString(string String)
        {
            var res = _put_string(bloomContainer, String);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public void PutArray(byte[] Array)
        {
            var res = _put_array(bloomContainer, Array, (uint)Array.Length);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
        }

        public bool CheckString(string String)
        {
            var res = _check_string(bloomContainer, String);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
            return res != 0;
        }

        public bool CheckArray(byte[] Array)
        {
            var res = _check_array(bloomContainer, Array, (uint)Array.Length);
            if (res < 0)
            {
                throw new BloomException(
                    errorClass: (BloomErrorClass)_get_error_class(bloomContainer),
                    errorCode: _get_error_code(bloomContainer),
                    errorMessage: GetErrorMessage());
            }
            return res != 0;
        }

        public ushort HeaderVersion() => _header_version(bloomContainer);
        public ulong HeaderSize() => _header_size(bloomContainer);
        public byte HeaderHashFunc() => _header_hash_func(bloomContainer);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _destroy_bloom(bloomContainer);
                }

                disposedValue = true;
            }
        }

        ~Bloom()
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
