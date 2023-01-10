using System.Runtime.Serialization;

namespace Answer.King.Domain.Repositories.Models;

[Serializable]
public class PaymentException : Exception
{
    public PaymentException(string message) : base(message)
    {
    }

    public PaymentException() : base()
    {
    }

    public PaymentException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected PaymentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
