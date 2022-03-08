using System.Net;
using System.Text.Json.Serialization;

namespace Ada.FirstCatering.API.Responses;

public class BaseResponse
{
    public ResponseStatus ResponseStatus { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; set; } = null!;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public HttpStatusCode? StatusCode { get; set; }

    public BaseResponse(ResponseStatus responseStatus)
    {
        ResponseStatus = responseStatus;
    }

    public BaseResponse(ResponseStatus responseStatus, string message)
    {
        ResponseStatus = responseStatus;
        Message = message;
    }
}

public class BaseResponse<T> : BaseResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T Data { get; set; }

    public BaseResponse(T data) : base(ResponseStatus.Success)
    {
        Data = data;
        if(data == null)
        {
            this.Message = "No data found";
            ResponseStatus = ResponseStatus.Error;
        }
    }

    public BaseResponse(ResponseStatus responseStatus, T data) : base(responseStatus)
    {
        Data = data;
        if(data == null)
        {
            this.Message = "No data found";
            ResponseStatus = ResponseStatus.Error;
        }
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResponseStatus
{
    Success = 1,
    Error = 2
}