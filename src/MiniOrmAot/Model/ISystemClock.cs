namespace MiniOrmAot.Model; 

public interface ISystemClock {
    /// <summary>
    /// Retrieves the current system time in UTC.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}

public class SystemClock : ISystemClock {
    public static readonly ISystemClock Instance = new SystemClock();
    
    public static DateTimeOffset GetNow() => Instance.UtcNow;
    
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}