namespace Server.Application.Wrapper;

public interface IResponseWrapper
{
    string? Message { get; set; }

    List<string>? Messages { get; set; }

    bool IsSuccessful { get; set; }
}

public interface IResponseWrapper<T> : IResponseWrapper
{
    T ResponseData { get; set; }
}
