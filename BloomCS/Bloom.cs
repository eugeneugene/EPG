using System.Runtime.InteropServices;

namespace BloomCS
{
    public static class Bloom
    {
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

        public static IntPtr CreateBloom() => _create_bloom();
        public static void DestroyBloom(IntPtr Bloom) => _destroy_bloom(Bloom);

        public static void Create(IntPtr Bloom, string FileName) => _create(Bloom, FileName);
        public static void Open(IntPtr Bloom, string FileName) => _open(Bloom, FileName);
        public static void Store(IntPtr Bloom) => _store(Bloom);
        public static void Load(IntPtr Bloom) => _load(Bloom);
        public static void Close(IntPtr Bloom) => _close(Bloom);
        public static void Abort(IntPtr Bloom) => _abort(Bloom);
        public static void Allocate(IntPtr Bloom, uint Elements) => _allocate(Bloom, Elements);

        public static void PutString(IntPtr Bloom, string String) => _put_string(Bloom, String);
        public static void PutArray(IntPtr Bloom, byte[] Array)
        {
            using var pinnedArray = new AutoPinner(Array);
            _put_array(Bloom, pinnedArray, (uint)Array.Length);
        }
        public static bool CheckString(IntPtr Bloom, string String) => _check_string(Bloom, String);
        public static bool CheckArray(IntPtr Bloom, byte[] Array)
        {
            using var pinnedArray = new AutoPinner(Array);
            return _check_array(Bloom, pinnedArray, (uint)Array.Length);
        }

        public static ushort HeaderVersion(IntPtr Bloom) => _header_version(Bloom);
        public static ulong HeaderSize(IntPtr Bloom) => _header_size(Bloom);
        public static byte HeaderHashFunc(IntPtr Bloom) => _header_hash_func(Bloom);
    }
}