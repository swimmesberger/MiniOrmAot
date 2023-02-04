using System.Runtime.Serialization;

namespace MiniOrmAot.Model; 

public class MiniOrmException : Exception {
    public MiniOrmException() { }
    protected MiniOrmException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public MiniOrmException(string? message) : base(message) { }
    public MiniOrmException(string? message, Exception? innerException) : base(message, innerException) { }
}