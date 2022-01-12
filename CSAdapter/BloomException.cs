namespace BloomCS
{
    public enum BloomErrorClass { BLOOM_NOERROR, BLOOM_WIN32ERROR, BLOOM_LIBERROR, BLOOM_STDERROR };

    public class BloomException : Exception
    {
        public BloomErrorClass BloomErrorClass { get; }
        public long ErrorCode { get; }

        public BloomException(BloomErrorClass errorClass, long errorCode, string errorMessage)
            : base(errorMessage)
        {
            BloomErrorClass = errorClass;
            ErrorCode = errorCode;
        }
    }
}
