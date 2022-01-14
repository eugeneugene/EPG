using System.Runtime.InteropServices;
using System.Text;

namespace CSAdapter
{
    public class Bloom : IDisposable
    {
        private readonly IntPtr bloomContainer;
        private bool disposedValue;

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateBloom")]
        private static extern IntPtr _create_bloom();

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyBloom")]
        private static extern void _destroy_bloom([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorClass")]
        private static extern int _get_error_class([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorCode")]
        private static extern long _get_error_code([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorMessage")]
        private static extern int _get_error_message([In] IntPtr BloomContainer, [In, Out] StringBuilder lpString, [In] uint bufferLength);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorMessageLength")]
        private static extern uint _get_error_message_length([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Create")]
        private static extern int _create([In] IntPtr BloomContainer, [In] string FileName);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Open")]
        private static extern int _open([In] IntPtr BloomContainer, [In] string FileName);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Store")]
        private static extern int _store([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Load")]
        private static extern int _load([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Close")]
        private static extern int _close([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Abort")]
        private static extern int _abort([In] IntPtr BloomContainer);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "Allocate")]
        private static extern int _allocate([In] IntPtr BloomContainer, [In] uint Elements);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "PutString")]
        private static extern int _put_string([In] IntPtr BloomContainer, [In] string String);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "PutArray")]
        private static extern int _put_array([In] IntPtr BloomContainer, [In] byte[] buffer, [In] uint length);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "CheckString")]
        private static extern int _check_string([In] IntPtr BloomContainer, [In] string String);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "CheckArray")]
        private static extern int _check_array([In] IntPtr BloomContainer, [In] byte[] buffer, [In] uint length);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "HeaderVersion")]
        private static extern ushort _header_version([In] IntPtr Bloom);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "HeaderSize")]
        private static extern uint _header_size([In] IntPtr Bloom);

        [DllImport("BloomDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "HeaderHashFunc")]
        private static extern byte _header_hash_func([In] IntPtr Bloom);

        private string GetErrorMessage()
        {
            var length = (int)_get_error_message_length(bloomContainer);
            StringBuilder buffer = new(length + 1);
            if (_get_error_message(bloomContainer, buffer, (uint)buffer.Capacity) > 0)
                return buffer.ToString();

            return string.Empty;
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
                    errorClass: (ErrorClass)_get_error_class(bloomContainer),
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
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
