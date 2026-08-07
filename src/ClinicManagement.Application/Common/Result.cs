namespace ClinicManagement.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success()
    {
        return new Result(true, Error.None);
    }
    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
    public static implicit operator Result(Error error) => Failure(error);
}


public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
    public static new Result<TValue> Failure(Error error) => new(default, false, error);

    // -------------------------------------------------------------------
    // Implicit Conversion Operator: TValue -> Result<TValue>
    // -------------------------------------------------------------------
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    // -------------------------------------------------------------------
    // Implicit Conversion Operator: Error -> Result<TValue>
    // -------------------------------------------------------------------
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}