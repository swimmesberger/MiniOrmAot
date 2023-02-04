using System;

namespace MiniOrmAot.Common {
    public class SqlIgnoreAttribute : Attribute {
        public StatementType[] Types { get; }

        public SqlIgnoreAttribute(params StatementType[] types) {
            Types = types;
        }
    }
}