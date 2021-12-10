using System.Runtime.InteropServices;

namespace BloomCS
{
    public class Bloom : IDisposable
    {
        private readonly IntPtr bloom;
        private bool disposedValue;

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CreateBloom")]
        private static extern IntPtr _create_bloom();

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "DestroyBloom")]
        private static extern void _destroy_bloom([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Create")]
        private static extern void _create([In] IntPtr Bloom, [In] string FileName);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Open")]
        private static extern void _open([In] IntPtr Bloom, [In] string FileName);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Store")]
        private static extern void _store([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Load")]
        private static extern void _load([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Close")]
        private static extern void _close([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Abort")]
        private static extern void _abort([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "Allocate")]
        private static extern void _allocate([In] IntPtr Bloom, [In] uint Elements);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "PutString")]
        private static extern void _put_string([In] IntPtr Bloom, [In] string String);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "PutArray")]
        private static extern void _put_array([In] IntPtr Bloom, [In] IntPtr buffer, [In] uint length);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CheckString")]
        private static extern bool _check_string([In] IntPtr Bloom, [In] string String);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CheckArray")]
        private static extern bool _check_array([In] IntPtr Bloom, [In] IntPtr buffer, [In] uint length);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "HeaderVersion")]
        private static extern ushort _header_version([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "HeaderSize")]
        private static extern ulong _header_size([In] IntPtr Bloom);

        [DllImport("Bloom.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "HeaderHashFunc")]
        private static extern byte _header_hash_func([In] IntPtr Bloom);

        public Bloom()
        {
            bloom = _create_bloom();
            if (bloom == IntPtr.Zero)
                throw new InvalidOperationException("Error creating Bloom Filter");
        }

        public void Create(string FileName) => _create(bloom, FileName);
        public void Open(string FileName) => _open(bloom, FileName);
        public void Store() => _store(bloom);
        public void Load() => _load(bloom);
        public void Close() => _close(bloom);
        public void Abort() => _abort(bloom);
        public void Allocate(uint Elements) => _allocate(bloom, Elements);

        public void PutString(string String) => _put_string(bloom, String);
        public void PutArray(byte[] Array)
        {
            using var pinnedArray = new AutoPinner(Array);
            _put_array(bloom, pinnedArray, (uint)Array.Length);
        }

        public bool CheckString(string String) => _check_string(bloom, String);
        public bool CheckArray(byte[] Array)
        {
            using var pinnedArray = new AutoPinner(Array);
            return _check_array(bloom, pinnedArray, (uint)Array.Length);
        }

        public ushort HeaderVersion() => _header_version(bloom);
        public ulong HeaderSize() => _header_size(bloom);
        public byte HeaderHashFunc() => _header_hash_func(bloom);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _destroy_bloom(bloom);
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