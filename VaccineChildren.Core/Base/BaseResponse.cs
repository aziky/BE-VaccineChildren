using System.Text.Json.Serialization;
using VaccineChildren.Core.Store;

namespace VaccineChildren.Core.Base;

public class BaseResponse<T>
{
    [JsonPropertyOrder(1)]
    public StatusCodeHelper StatusCode { get; set; }

    [JsonPropertyOrder(2)]
    public string Code { get; set; }

    [JsonPropertyOrder(3)]
    public string? Message { get; set; }

    [JsonPropertyOrder(4)]
    public T? Data { get; set; }
    
    [JsonPropertyOrder(5)]
    public int TotalCount { get; set; }
    public BaseResponse(StatusCodeHelper statusCode, string code, T? data, string? message)
    {
        StatusCode = statusCode;
        Code = code;
        Message = message;
        Data = data;
        TotalCount = 0;
    }

    public BaseResponse(StatusCodeHelper statusCode, string code, T? data)
    {
        StatusCode = statusCode;
        Code = code;
        Data = data;
        TotalCount = 0;
    }

    public BaseResponse(StatusCodeHelper statusCode, string code, string? message)
    {
        StatusCode = statusCode;
        Code = code;
        Message = message;
        TotalCount = 0;
    }

    public static BaseResponse<T> OkResponse(T? data, string? mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.Name(), data, mess);
    }
    public static BaseResponse<T> OkResponse(string? mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.OK, StatusCodeHelper.OK.Name(), mess);
    }
    
    public static BaseResponse<T> CreateResponse(string? mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.Created, StatusCodeHelper.Created.Name(), mess);
    }

    public static BaseResponse<T> UnauthorizedResponse(string mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.Unauthorized, StatusCodeHelper.Unauthorized.Name(), mess);
    }

    public static BaseResponse<T> NotFoundResponse(string mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.NotFound, StatusCodeHelper.NotFound.Name(), mess);
    }

    public static BaseResponse<T> BadRequestResponse(string mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.BadRequest, StatusCodeHelper.BadRequest.Name(), mess);
    }

    public static BaseResponse<T> InternalErrorResponse(string mess)
    {
        return new BaseResponse<T>(StatusCodeHelper.ServerError, StatusCodeHelper.ServerError.Name(), mess);
    }

    public static object ErrorResponse(string internalServerError, string exMessage)
    {
        return new
        {
            Status = "Error",
            Message = internalServerError,
            Details = exMessage
        };
    }
}