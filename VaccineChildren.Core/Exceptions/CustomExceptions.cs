namespace VaccineChildren.Core.Exceptions;

/// <summary>
/// Lớp chứa các exception tùy chỉnh cho ứng dụng
/// </summary>
public static class CustomExceptions
{
    /// <summary>
    /// Exception khi không tìm thấy dữ liệu
    /// </summary>
    public class NoDataFoundException : Exception
    {
        public NoDataFoundException() : base("No data found for the specified query.") { }

        public NoDataFoundException(string message) : base(message) { }

        public NoDataFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception khi tham số đầu vào không hợp lệ
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException() : base("Validation failed for the provided data.") { }

        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception khi không tìm thấy một thực thể (Entity) cụ thể
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, string entityId) 
            : base($"{entityName} with ID {entityId} was not found.") { }

        public EntityNotFoundException(string message) : base(message) { }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception cho các lỗi logic kinh doanh
    /// </summary>
    public class BusinessLogicException : Exception
    {
        public BusinessLogicException() : base("Business logic error occurred.") { }

        public BusinessLogicException(string message) : base(message) { }

        public BusinessLogicException(string message, Exception innerException) : base(message, innerException) { }
    }
}
