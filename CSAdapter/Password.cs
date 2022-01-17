using System.Runtime.InteropServices;
using System.Text;

namespace CSAdapter
{
    public class Password : IDisposable
    {
        private readonly IntPtr passwordContainer;
        private bool disposedValue;

        public enum Modes { NoMode = 0, Lowers = 0x01, LowersForced = 0x11, Capitals = 0x02, CapitalsForced = 0x22, Numerals = 0x04, NumeralsForced = 0x44, Symbols = 0x08, SymbolsForced = 0x88 };

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreatePassword")]
        private static extern IntPtr _create_password(int mode, [In] string IncludeSymbols, [In] string ExcludeSymbols);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyPassword")]
        private static extern void _destroy_password([In] IntPtr PasswordContainer);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorClass")]
        private static extern int _get_error_class([In] IntPtr PasswordContainer);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorCode")]
        private static extern long _get_error_code([In] IntPtr PasswordContainer);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorMessage")]
        private static extern int _get_error_message([In] IntPtr PasswordContainer, [In, Out] StringBuilder lpString, [In] uint bufferLength);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetErrorMessageLength")]
        private static extern uint _get_error_message_length([In] IntPtr PasswordContainer);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GenerateWord")]
        private static extern int _generate_word([In] IntPtr PasswordContainer, [In] uint Length);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GenerateRandomWord")]
        private static extern int _generate_random_word([In] IntPtr PasswordContainer, [In] uint Length);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWordLength")]
        private static extern uint _get_word_length([In] IntPtr PasswordContainer);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWord")]
        private static extern int _get_word([In] IntPtr PasswordContainer, [In, Out] StringBuilder lpString, [In] uint bufferLength);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHyphenatedLength")]
        private static extern uint _get_hyphenated_word_length([In] IntPtr PasswordContainer);

        [DllImport("PasswordDll.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHyphenatedWord")]
        private static extern int _get_hyphenated_word([In] IntPtr PasswordContainer, [In, Out] StringBuilder lpString, [In] uint bufferLength);

        private string GetErrorMessage()
        {
            var length = (int)_get_error_message_length(passwordContainer);
            StringBuilder buffer = new(length + 1);
            if (_get_error_message(passwordContainer, buffer, (uint)buffer.Capacity) > 0)
                return buffer.ToString();

            return string.Empty;
        }

        public Password(Modes mode, string includeSymbols, string excludeSymbols)
        {
            passwordContainer = _create_password((int)mode, includeSymbols, excludeSymbols);
            if (passwordContainer == IntPtr.Zero)
                throw new InvalidOperationException("Error creating Bloom Filter");
        }

        public bool GenerateWord(uint Length)
        {
            var res = _generate_word(passwordContainer, Length);
            if (res < 0)
            {
                throw new PasswordException(
                    errorClass: (ErrorClass)_get_error_class(passwordContainer),
                    errorCode: _get_error_code(passwordContainer),
                    errorMessage: GetErrorMessage());
            }
            return res > 0;
        }

        public bool GenerateRandomWord(uint Length)
        {
            var res = _generate_random_word(passwordContainer, Length);
            if (res < 0)
            {
                throw new PasswordException(
                    errorClass: (ErrorClass)_get_error_class(passwordContainer),
                    errorCode: _get_error_code(passwordContainer),
                    errorMessage: GetErrorMessage());
            }
            return res > 0;
        }

        public uint GetWordLength()
        {
            return _get_word_length(passwordContainer);
        }

        public uint GetHyphenatedWordLength()
        {
            return _get_hyphenated_word_length(passwordContainer);
        }

        public string GetWord()
        {
            var Length = (int)_get_word_length(passwordContainer);
            StringBuilder stringBuilder = new(Length + 1);
            var res = _get_word(passwordContainer, stringBuilder, (uint)stringBuilder.Capacity);
            if (res < 0)
            {
                throw new PasswordException(
                    errorClass: (ErrorClass)_get_error_class(passwordContainer),
                    errorCode: _get_error_code(passwordContainer),
                    errorMessage: GetErrorMessage());
            }
            return stringBuilder.ToString();
        }

        public string GetHyphenatedWord()
        {
            var Length = (int)_get_hyphenated_word_length(passwordContainer);
            StringBuilder stringBuilder = new(Length + 1);
            var res = _get_hyphenated_word(passwordContainer, stringBuilder, (uint)stringBuilder.Capacity);
            if (res < 0)
            {
                throw new PasswordException(
                    errorClass: (ErrorClass)_get_error_class(passwordContainer),
                    errorCode: _get_error_code(passwordContainer),
                    errorMessage: GetErrorMessage());
            }
            return stringBuilder.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _destroy_password(passwordContainer);
                }

                disposedValue = true;
            }
        }

        ~Password()
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
