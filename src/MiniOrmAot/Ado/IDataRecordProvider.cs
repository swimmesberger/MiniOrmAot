using MiniOrmAot.Mapping;

namespace MiniOrmAot.Ado; 

public interface IDataRecordProvider<T> {
    IDataRecordMapper<T> DataRecordMapper { get; }
}