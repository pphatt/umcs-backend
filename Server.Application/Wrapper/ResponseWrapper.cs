namespace Server.Application.Wrapper;

// Result pattern.
public class ResponseWrapper : IResponseWrapper
{
    private List<string> _messages = new();

    public List<string>? Messages
    {
        get => _messages.Any() ? _messages : null;
        set => _messages = value ?? new();
    }

    private string _message = string.Empty;

    public string? Message
    {
        get => _message.Any() ? _message : null;
        set => _message = value ?? string.Empty;
    }

    public bool IsSuccessful { get; set; }

    public static IResponseWrapper Fail()
        => new ResponseWrapper { IsSuccessful = false };

    public static IResponseWrapper Fail(string message)
        => new ResponseWrapper { IsSuccessful = true, Message = message };

    public static IResponseWrapper Fail(List<string> messages)
        => new ResponseWrapper { IsSuccessful = false, Messages = messages };

    public static Task<IResponseWrapper> FailAsync()
        => Task.FromResult(Fail());

    public static Task<IResponseWrapper> FailAsync(string message)
        => Task.FromResult(Fail(message));

    public static Task<IResponseWrapper> FailAsync(List<string> messages)
        => Task.FromResult(Fail(messages));

    public static IResponseWrapper Success()
        => new ResponseWrapper { IsSuccessful = true };

    public static IResponseWrapper Success(string message)
        => new ResponseWrapper { IsSuccessful = false, Message = message };

    public static IResponseWrapper Success(List<string> messages)
        => new ResponseWrapper { IsSuccessful = true, Messages = messages };

    public static Task<IResponseWrapper> SuccessAsync()
        => Task.FromResult(Success());

    public static Task<IResponseWrapper> SuccessAsync(string message)
        => Task.FromResult(Success(message));

    public static Task<IResponseWrapper> SuccessAsync(List<string> messages)
        => Task.FromResult(Success(messages));
}

public class ResponseWrapper<T> : ResponseWrapper, IResponseWrapper<T>
{
    public T ResponseData { get; set; } = default!;

    public static new IResponseWrapper<T> Fail()
        => new ResponseWrapper<T> { IsSuccessful = false };

    public static new IResponseWrapper<T> Fail(string message)
        => new ResponseWrapper<T> { IsSuccessful = false, Message = message };

    public static new IResponseWrapper<T> Fail(List<string> messages)
        => new ResponseWrapper<T> { IsSuccessful = false, Messages = messages };

    public static new Task<IResponseWrapper<T>> FailAsync()
        => Task.FromResult(Fail());

    public static new Task<IResponseWrapper<T>> FailAsync(string message)
        => Task.FromResult(Fail(message));

    public static new Task<IResponseWrapper<T>> FailAsync(List<string> messages)
        => Task.FromResult(Fail(messages));

    public static new IResponseWrapper<T> Success()
        => new ResponseWrapper<T> { IsSuccessful = true };

    public static new IResponseWrapper<T> Success(string message)
        => new ResponseWrapper<T> { IsSuccessful = true, Message = message };

    public static new IResponseWrapper<T> Success(List<string> messages)
        => new ResponseWrapper<T> { IsSuccessful = true, Messages = messages };

    public static IResponseWrapper<T> Success(T data)
        => new ResponseWrapper<T> { IsSuccessful = true, ResponseData = data };

    public static IResponseWrapper<T> Success(string message, T data)
        => new ResponseWrapper<T> { IsSuccessful = true, Message = message, ResponseData = data };

    public static IResponseWrapper<T> Success(List<string> messages, T data)
        => new ResponseWrapper<T> { IsSuccessful = true, Messages = messages, ResponseData = data };

    public static new Task<IResponseWrapper<T>> SuccessAsync()
        => Task.FromResult(Success());

    public static new Task<IResponseWrapper<T>> SuccessAsync(string message)
        => Task.FromResult(Success(message));

    public static new Task<IResponseWrapper<T>> SuccessAsync(List<string> messages)
        => Task.FromResult(Success(messages));

    public static Task<IResponseWrapper<T>> SuccessAsync(T data)
        => Task.FromResult(Success(data));

    public static Task<IResponseWrapper<T>> SuccessAsync(string message, T data)
        => Task.FromResult(Success(message, data));

    public static Task<IResponseWrapper<T>> SuccessAsync(List<string> messages, T data)
        => Task.FromResult(Success(messages, data));
}
