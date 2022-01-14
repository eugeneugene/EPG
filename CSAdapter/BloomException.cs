namespace CSAdapter
{
    public class BloomException : Exception
    {
        public ErrorClass BloomErrorClass { get; }
        public long ErrorCode { get; }

        public BloomException(ErrorClass errorClass, long errorCode, string errorMessage)
            : base(errorMessage)
        {
            BloomErrorClass = errorClass;
            ErrorCode = errorCode;
        }
    }
}
