namespace StreamProcessing.Map.Exceptions;

public sealed class InvalidClassException : Exception
{
    public InvalidClassException()
    {
    }

    public InvalidClassException(string message)
        : base(message)
    {
    }

    public InvalidClassException(string message, Exception inner)
        : base(message, inner)
    {
    }
}