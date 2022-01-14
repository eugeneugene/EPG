namespace CSAdapter
{
    public class PasswordException : Exception
    {
        public ErrorClass PasswordErrorClass { get; }
        public long ErrorCode { get; }

        public PasswordException(ErrorClass errorClass, long errorCode, string errorMessage)
            : base(errorMessage)
        {
            PasswordErrorClass = errorClass;
            ErrorCode = errorCode;
        }
    }
}
