namespace ThinkWorld.Domain.Events.Results;

public record OperationResult<T> : OperationResult
{
    public T? Result { get; init; }
    
    private OperationResult()
    {
    }
    
    public OperationResult(T? result)
    {
        Result = result;
    }
    
    public OperationResult(params string[] errors) : base(errors)
    {
        Errors = errors;
    }
    
    public static OperationResult<T> Succeeded(T? result) => new(result);
    public static OperationResult<T> Succeeded() => new();
    public static OperationResult<T> Failed(params string[] errors) => new(errors);
    
}

public record OperationResult
{
    public string[] Errors { get; init; }
    public bool HasErrors => Errors.Length > 0;
    
    protected OperationResult()
    {
    }
    
    protected OperationResult(params string[] errors)
    {
        Errors = errors;
    }

    public static OperationResult Succeeded() => new();
    public static OperationResult Failed(params string[] errors) => new(errors);
}