namespace MiniOrmAot.Analyzers.Mapping {
    public abstract class PropertyNamingPolicy {
        public static PropertyNamingPolicy SnakeCase { get; } = new PropertySnakeCaseNamingPolicy();

        public abstract string PolicyName { get; }
        
        public abstract string ConvertName(string name);
    }
}