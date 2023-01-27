using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Answer.King.Infrastructure;

[ExcludeFromCodeCoverage]
[Serializable]
public class LiteDbConnectionFactoryException : Exception
{
    public LiteDbConnectionFactoryException(string message)
        : base(message)
    {
    }

    public LiteDbConnectionFactoryException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected LiteDbConnectionFactoryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
