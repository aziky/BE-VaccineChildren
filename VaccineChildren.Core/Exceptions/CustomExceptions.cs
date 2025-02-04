namespace VaccineChildren.Core.Exceptions;

public static class CustomExceptions
{
    public class NoDataFoundException : Exception
    {
        public NoDataFoundException() : base("No data found for the specified query.")
        {
        }

        public NoDataFoundException(string message) : base(message)
        {
        }

        public NoDataFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}