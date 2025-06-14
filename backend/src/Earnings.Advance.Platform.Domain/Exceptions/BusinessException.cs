namespace Earnings.Advance.Platform.Domain.Exceptions
{
    /// <summary>
    /// Exception for business rules
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, Exception innerException) : base(message, innerException) { }
    }
}
