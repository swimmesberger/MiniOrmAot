using System.Runtime.Serialization;

namespace MiniOrmAot.Model; 

public class MiniOrmDbException : MiniOrmException {
    public MiniOrmDbException() { }
    protected MiniOrmDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public MiniOrmDbException(string? message) : base(message) { }
    public MiniOrmDbException(string? message, Exception? innerException) : base(message, innerException) { }
}